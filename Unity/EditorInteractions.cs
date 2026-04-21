using FrameCoreU.Pooling;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace FrameCoreU.Unity
{
    
    public static class EditorInteractions
    {
        public static void AddToPool(Transform prefab)
        {
            if (EditorInteractions.InEditorButton())
            {
                if (prefab == null)
                {
                    Debug.LogError("Prefab is null");
                    return;
                }
                GameObject poolCore = GameObject.Find("Pool");
                if (poolCore == null)
                {
                    Debug.LogError("Pool - object not in scene");
                    return;
                }
                PoolCore pool = poolCore.GetComponent<PoolCore>();
                if (pool == null)
                {
                    pool = poolCore.AddComponent<PoolCore>();
                    Debug.LogError("PoolCore not in scene");
                    return;
                }
                pool.AddToPool(prefab); 
    #if UNITY_EDITOR
                UnityEditor.Selection.activeGameObject = poolCore;
                EditorInteractions.SetDirty(poolCore.GetComponent<PoolCore>());
    #endif
            };
        }

        public static void SetDirty(Object dirtyObject)
        {
    #if UNITY_EDITOR
            EditorUtility.SetDirty(dirtyObject);
    #endif
        }

        public static bool InPlayerButton()
        {
            if (Application.isEditor && Application.isPlaying)
            {
                return true;
            }
            else
            {
                Debug.Log("WARNING >> Cannot Run This Function Unless In Play Mode.");
                return false;
            };
        }

        public static bool InEditorButton()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                return true;
            }
            else
            {
                Debug.Log("WARNING >> Cannot Run This Function Unless In Editor Mode.");
                return false;
            };
        }
    }
}