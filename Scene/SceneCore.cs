using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameCoreU.Scene
{
    public class SceneCore : MonoBehaviour
    {
        [SerializeField] private List<SceneCoreScene> scenes = new();

        public IReadOnlyList<SceneCoreScene> Scenes => scenes;

        private SceneCoreScene GetSceneByName(string sceneName)
        {
            foreach (SceneCoreScene scene in scenes)
            {
                if (scene.sceneName == sceneName)
                    return scene;
            }

            return null;
        }

        public void UnloadScene(string sceneName)
        {
            SceneCoreScene sceneCoreScene = GetSceneByName(sceneName);

            if (sceneCoreScene != null)
                scenes.Remove(sceneCoreScene);

            SceneManager.UnloadSceneAsync(sceneName);
        }

        public void ActivateScene(string sceneName)
        {
            SceneCoreScene sceneCoreScene = GetSceneByName(sceneName);

            if (sceneCoreScene != null)
            {
                sceneCoreScene.activateOnLoad = true;
                sceneCoreScene.ActivateScene();
            }
            else
            {
                Debug.LogError($"Cant Activate - Scene Not Loaded [SceneName:{sceneName}]");
            }
        }

        public void LoadScene(SceneCoreScene sceneCoreScene)
        {
            if (sceneCoreScene == null)
            {
                Debug.LogError("SceneCore [ERROR] >> Tried to load a null SceneCoreScene.");
                return;
            }

            if (GetSceneByName(sceneCoreScene.sceneName) == null)
            {
                scenes.Add(sceneCoreScene);
                StartCoroutine(sceneCoreScene.LoadAsyncScene());
            }
            else
            {
                Debug.LogError($"Scene Already Loaded [SceneName:{sceneCoreScene.sceneName}]");
            }
        }

        public void SceneCompleted(SceneCoreScene sceneCoreScene)
        {
            if (sceneCoreScene != null && scenes.Contains(sceneCoreScene))
                scenes.Remove(sceneCoreScene);
        }

        private void Update()
        {
            foreach (SceneCoreScene scene in scenes)
            {
                scene.Update();
            }
        }
    }
}