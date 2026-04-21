using FrameCoreU.Audio;
using FrameCoreU.Pooling;
using FrameCoreU.Scene;
using FrameCoreU.Timing;
using FrameCoreU.Events;
using FrameCoreU.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameCoreU
{
    public class FrameCore : MonoBehaviour
    {
        [Title("Cores")]
        [InlineButton("ConnectCores")]
        [SerializeField] private EventCore events;
        [SerializeField] private PoolCore pools;
        [SerializeField] private SoundCore sound;
        [SerializeField] private SceneCore scenes;

        [Space(10)]
        [Title("Settings")]
        [HideLabel]
        [SerializeField] private FrameSettings settings;

        [Space(10)]
        [Title("Timing")]
        [HideLabel]
        [SerializeField] private Debouncer debouncer;
        
        public EventCore Events => events;
        public PoolCore Pools => pools;
        public SoundCore Sound => sound;
        public SceneCore Scenes => scenes;
        public Debouncer Debouncer => debouncer;

        private bool systemError = false;

        private void ConnectCores()
        {
            if (!EditorInteractions.InEditorButton())
                return;

            events = GetRequiredCore<EventCore>();
            pools = GetRequiredCore<PoolCore>();
            sound = GetRequiredCore<SoundCore>();
            scenes = GetRequiredCore<SceneCore>();

            EditorInteractions.SetDirty(this);
        }

        private T GetRequiredCore<T>() where T : Component
        {
            T[] found = GetComponentsInChildren<T>(true);

            if (found.Length == 0)
            {
                Debug.LogError($"FrameCore [ERROR] >> No {typeof(T).Name} found.");
                return null;
            }

            if (found.Length > 1)
            {
                Debug.LogError($"FrameCore [ERROR] >> Multiple {typeof(T).Name} components found.");
                return null;
            }

            Debug.Log($"FrameCore >> Connected {typeof(T).Name}");
            return found[0];
        }

        private void CheckComponent(Object obj, string componentName)
        {
            if (obj == null)
            {
                Debug.LogError($"FrameCore [ERROR] >> {componentName} is not attached.");
                systemError = true;
            }
        }

        private void CheckSetup()
        {
            systemError = false;

            CheckComponent(events, nameof(events));
            CheckComponent(pools, nameof(pools));
            CheckComponent(sound, nameof(sound));
            CheckComponent(scenes, nameof(scenes));
            
            if (systemError)
            {
                Debug.LogError("FrameCore incorrectly setup.");
                Debug.Break();
            }
        }

        private void Awake()
        {
            if (Frame.Core != null && Frame.Core != this)
            {
                Debug.LogError("FrameCore [ERROR] >> More than one FrameCore found in scene.");
                Destroy(gameObject);
                return;
            }

            Frame.SetCore(this);

            if (settings != null)
            {
                Application.targetFrameRate = settings.targetFrameRate;
            }
            else
            {
                Debug.LogWarning("FrameCore >> No FrameSettings assigned.");
            }

            CheckSetup();

            Debug.Log("FrameCore Started...");
        }

        private void OnDestroy()
        {
            Frame.ClearCore(this);
        }

        private void Update()
        {
            debouncer?.Update();
        }

        public void QuitApplication()
        {
            Application.Quit();
        }
    }
}