#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameCoreU.Events
{
    // Editor only: links every FrameCoreEvent to the component holding it (FrameCoreEvent.Owner), so the event's
    // Console logs select that object when clicked. It finds events anywhere in a component's serialized data: direct
    // fields, lists and arrays, nested serializable classes (e.g. OnOffToggleEvent) and FrameActions.
    //
    // Runs by itself when a scene or prefab is saved and when you select an object in a scene or prefab. The
    // FrameCoreU > Link Event Owners menu does every prefab under Assets plus the open scenes in one go - run it once to
    // link everything that already exists.
    //
    // Components that come from a prefab are left to that prefab: their copies pick the owner up from it (Unity points
    // the reference at each copy), so linking never adds prefab overrides.
    [InitializeOnLoad]
    public static class FrameCoreEventOwners
    {
        private const int MAX_DEPTH = 8;

        private static readonly Dictionary<Type, FieldInfo[]> FieldCache = new();

        static FrameCoreEventOwners()
        {
            EditorSceneManager.sceneSaving += OnSceneSaving;
            PrefabStage.prefabSaving += OnPrefabSaving;
            Selection.selectionChanged += OnSelectionChanged;
        }

        [MenuItem("FrameCoreU/Link Event Owners")]
        private static void LinkEverything()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("FrameCoreU >> Leave Play mode before linking event owners.");
                return;
            }

            int prefabCount = 0, prefabEvents = 0;
            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (root == null)
                    continue;

                PrefabAssetType type = PrefabUtility.GetPrefabAssetType(root);
                if (type != PrefabAssetType.Regular && type != PrefabAssetType.Variant)
                    continue;

                int linked = LinkHierarchy(root);
                if (linked == 0)
                    continue;

                PrefabUtility.SavePrefabAsset(root);
                prefabCount++;
                prefabEvents += linked;
            }

            int sceneEvents = 0;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                    continue;

                int linked = LinkScene(scene);
                if (linked > 0)
                    EditorSceneManager.MarkSceneDirty(scene);
                sceneEvents += linked;
            }

            Debug.Log($"FrameCoreU >> Linked {prefabEvents} event(s) in {prefabCount} prefab(s) and {sceneEvents} in the open scene(s)."
                      + (sceneEvents > 0 ? " Save the scene(s) to keep them." : ""));
        }

        private static void OnSceneSaving(UnityEngine.SceneManagement.Scene scene, string path)
        {
            LinkScene(scene);
        }

        private static void OnPrefabSaving(GameObject root)
        {
            LinkHierarchy(root);
        }

        // Selecting something in a scene or prefab links its components (only once - after that nothing changes).
        // Prefab assets selected in the Project window are left to the menu and to saving the prefab.
        private static void OnSelectionChanged()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            foreach (GameObject go in Selection.gameObjects)
            {
                if (go == null || EditorUtility.IsPersistent(go))
                    continue;

                int linked = 0;
                foreach (MonoBehaviour component in go.GetComponents<MonoBehaviour>())
                    linked += LinkComponent(component);

                if (linked > 0 && go.scene.IsValid())
                    EditorSceneManager.MarkSceneDirty(go.scene);
            }
        }

        public static int LinkScene(UnityEngine.SceneManagement.Scene scene)
        {
            int linked = 0;
            foreach (GameObject root in scene.GetRootGameObjects())
                linked += LinkHierarchy(root);
            return linked;
        }

        public static int LinkHierarchy(GameObject root)
        {
            int linked = 0;
            foreach (MonoBehaviour component in root.GetComponentsInChildren<MonoBehaviour>(true))
                linked += LinkComponent(component);
            return linked;
        }

        // Links every FrameCoreEvent in this component's data to it. Returns how many changed.
        public static int LinkComponent(MonoBehaviour component)
        {
            if (component == null)
                return 0;

            // From a prefab (a scene copy, or a prefab nested in another): the prefab itself is linked instead
            if (PrefabUtility.GetCorrespondingObjectFromSource(component) != null)
                return 0;

            int linked = 0;
            HashSet<object> visited = new(ReferenceComparer.Instance);
            Walk(component, component, 0, visited, ref linked);

            if (linked > 0)
                EditorUtility.SetDirty(component);
            return linked;
        }

        private static void Walk(object value, MonoBehaviour owner, int depth, HashSet<object> visited, ref int linked)
        {
            if (value == null || depth > MAX_DEPTH)
                return;

            Type type = value.GetType();
            if (!type.IsValueType && !visited.Add(value))
                return;

            if (value is FrameCoreEvent frameCoreEvent && frameCoreEvent.SetOwner(owner))
                linked++;

            if (value is IList list)
            {
                foreach (object item in list)
                {
                    if (item != null && MayHoldEvents(item.GetType()))
                        Walk(item, owner, depth + 1, visited, ref linked);
                }
                return;
            }

            foreach (FieldInfo field in FieldsOf(type))
                Walk(field.GetValue(value), owner, depth + 1, visited, ref linked);
        }

        // The instance fields of a type (and its base classes, up to MonoBehaviour) that could hold a FrameCoreEvent
        private static FieldInfo[] FieldsOf(Type type)
        {
            if (FieldCache.TryGetValue(type, out FieldInfo[] cached))
                return cached;

            List<FieldInfo> fields = new();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            for (Type current = type; current != null && current != typeof(MonoBehaviour) && current != typeof(object); current = current.BaseType)
            {
                foreach (FieldInfo field in current.GetFields(flags))
                {
                    if (field.IsNotSerialized || !MayHoldEvents(field.FieldType))
                        continue;
                    fields.Add(field);
                }
            }

            cached = fields.ToArray();
            FieldCache[type] = cached;
            return cached;
        }

        // Could a value of this type be, or contain, a FrameCoreEvent? Leaves out Unity objects (they're other objects'
        // data), strings, primitives and enums, and Unity/.NET types other than lists and arrays.
        private static bool MayHoldEvents(Type type)
        {
            if (type == typeof(FrameCoreEvent))
                return true;
            if (type.IsPrimitive || type.IsEnum || type.IsPointer || type == typeof(string) || type == typeof(decimal))
                return false;
            if (typeof(UnityEngine.Object).IsAssignableFrom(type) || typeof(Delegate).IsAssignableFrom(type))
                return false;

            if (type.IsArray)
                return MayHoldEvents(type.GetElementType());
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return MayHoldEvents(type.GetGenericArguments()[0]);

            string ns = type.Namespace ?? "";
            return !(ns == "System" || ns.StartsWith("System.") || ns == "UnityEngine" || ns.StartsWith("UnityEngine.")
                     || ns.StartsWith("UnityEditor") || ns.StartsWith("Unity.") || ns.StartsWith("Sirenix"));
        }

        private sealed class ReferenceComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceComparer Instance = new();
            public new bool Equals(object a, object b) => ReferenceEquals(a, b);
            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}
#endif
