using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameCoreU.Scene
{
    [System.Serializable]
    public class SceneCoreScene
    {
        public string sceneName;

        [Title("Load Settings")]
        public LoadSceneMode loadSceneMode;
        public ThreadPriority asyncSpeed;

        [Title("Activation Settings")]
        public bool activated = false;
        public bool activateOnLoad = true;

        [Title("System")]
        public float loadAmount = 0f;
        public float loadingTime = 0f;
        public bool loading = false;

        private AsyncOperation asyncLoad;

        public void ActivateScene()
        {
            if (activated)
                return;

            if (loading)
            {
                Debug.Log("Cannot Activate - Loading Scene...");
                activateOnLoad = true;
                return;
            }

            if (asyncLoad == null)
            {
                Debug.LogError($"SceneCoreScene [ERROR] >> Cannot activate scene because async operation is null. [SceneName:{sceneName}]");
                return;
            }

            Debug.Log("Activating Scene");
            asyncLoad.allowSceneActivation = true;

            activated = true;
            Frame.Scenes.SceneCompleted(this);
        }

        public void SetupLoadScene()
        {
            Application.backgroundLoadingPriority = asyncSpeed;

            loadingTime = 0f;
            loading = true;
            loadAmount = 0f;
            activated = false;
        }

        public IEnumerator LoadAsyncScene()
        {
            SetupLoadScene();

            asyncLoad = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

            if (asyncLoad == null)
            {
                Debug.LogError($"SceneCoreScene [ERROR] >> Failed to start loading scene. [SceneName:{sceneName}]");
                loading = false;
                yield break;
            }

            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                loadAmount = asyncLoad.progress;

                if (asyncLoad.progress >= 0.9f)
                {
                    loading = false;

                    if (activateOnLoad)
                        ActivateScene();
                }

                yield return null;
            }

            Debug.Log($"Scene Loaded - Load Time: {loadingTime}");
        }

        public void Update()
        {
            if (loading)
                loadingTime += Time.deltaTime;
        }
    }
}