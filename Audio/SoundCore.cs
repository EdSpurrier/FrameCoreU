using Sirenix.OdinInspector;
using System.Collections.Generic;
using FrameCoreU.Data;
using UnityEngine;
using FrameCoreU.Pooling;
using FrameCoreU.Unity;

namespace FrameCoreU.Audio
{
    [System.Serializable]
    public class PriorityLevelSetting
    {
        [HorizontalGroup("Split", 0.5f)]
        [HideLabel]
        public Level levelSetting;

        [HorizontalGroup("Split", 0.5f)]
        [HideLabel]
        public int priority;
    }

    public class SoundCore : MonoBehaviour
    {
        [Title("Sound Points")]
        [InlineButton("AddToPool")]
        [SerializeField] private Transform soundPointPrefab;

        [Title("Active Sound Points")]
        [SerializeField] private List<SoundPoint> soundPoints = new();

        
        [System.Serializable]
        public class PriorityLevelSetting
        {
            public Level levelSetting;
            public int priority;
        }

        [Title("System Settings")]
        [SerializeField] private List<PriorityLevelSetting> priorityLevels = new()
        {
            new PriorityLevelSetting { levelSetting = Level.VeryHigh, priority = 256 },
            new PriorityLevelSetting { levelSetting = Level.High, priority = 200 },
            new PriorityLevelSetting { levelSetting = Level.Medium, priority = 150 },
            new PriorityLevelSetting { levelSetting = Level.Low, priority = 100 },
            new PriorityLevelSetting { levelSetting = Level.VeryLow, priority = 50 }
        };

        private Dictionary<Level, int> priorityLookup;

        [Title("System")]
        [ShowInInspector, ReadOnly]
        private bool prefabInPool = false;

        private PoolCore pool;

        public Transform SoundPointPrefab => soundPointPrefab;
        public IReadOnlyList<SoundPoint> SoundPoints => soundPoints;

        private void Awake()
        {
            if (pool == null)
                pool = FindAnyObjectByType<PoolCore>();
            
            BuildPriorityLookup();
        }
        
        private void BuildPriorityLookup()
        {
            priorityLookup = new Dictionary<Level, int>();

            foreach (var setting in priorityLevels)
            {
                if (setting == null)
                    continue;

                if (priorityLookup.ContainsKey(setting.levelSetting))
                {
                    Debug.LogWarning($"SoundCore >> Duplicate priority setting found for {setting.levelSetting}. Overwriting previous value.");
                }

                priorityLookup[setting.levelSetting] = setting.priority;
            }
        }

        private void OnValidate()
        {
            if (pool == null)
                pool = FindAnyObjectByType<PoolCore>();

            prefabInPool = pool != null && soundPointPrefab != null && pool.ObjectInPool(soundPointPrefab);
        }

        private void AddToPool()
        {
            EditorInteractions.AddToPool(soundPointPrefab);
        }

        public void GenerateSoundPoint(SoundPointData soundPointData)
        {
            if (soundPointData == null)
            {
                UnityEngine.Debug.LogError("SoundCore [ERROR] >> SoundPointData is null.");
                return;
            }

            if (soundPointPrefab == null)
            {
                UnityEngine.Debug.LogError("SoundCore [ERROR] >> Sound point prefab is null.");
                return;
            }

            GameObject spawn = soundPointPrefab.SpawnObject(Vector3.zero, Quaternion.identity);

            if (spawn == null)
            {
                UnityEngine.Debug.LogError("SoundCore [ERROR] >> Failed to spawn sound point.");
                return;
            }

            SoundPoint soundPoint = spawn.GetComponent<SoundPoint>();

            if (soundPoint == null)
            {
                UnityEngine.Debug.LogError("SoundCore [ERROR] >> Spawned object does not contain a SoundPoint component.");
                return;
            }

            RegisterSoundPoint(soundPoint);
            soundPoint.Play(soundPointData);
        }

        public void RegisterSoundPoint(SoundPoint soundPoint)
        {
            if (soundPoint == null)
                return;

            if (!soundPoints.Contains(soundPoint))
                soundPoints.Add(soundPoint);
        }

        public void UnregisterSoundPoint(SoundPoint soundPoint)
        {
            if (soundPoint == null)
                return;

            if (soundPoints.Contains(soundPoint))
                soundPoints.Remove(soundPoint);
        }

        public void ReleaseSoundPoint(SoundPoint soundPoint)
        {
            if (soundPoint == null)
                return;

            UnregisterSoundPoint(soundPoint);

            soundPoint.ResetSoundPoint();
            soundPoint.transform.SetParent(transform);
            soundPoint.gameObject.SetActive(false);
        }

        public void DestroyAllSoundPoints()
        {
            for (int i = soundPoints.Count - 1; i >= 0; i--)
            {
                SoundPoint soundPoint = soundPoints[i];

                if (soundPoint != null)
                    ReleaseSoundPoint(soundPoint);
            }
        }

        public int GetPriority(Level levelSetting)
        {
            if (priorityLookup == null || priorityLookup.Count == 0)
            {
                BuildPriorityLookup();
            }

            if (!priorityLookup.TryGetValue(levelSetting, out int priority))
            {
                Debug.LogWarning($"SoundCore >> No priority found for {levelSetting}. Returning 0.");
                return 0;
            }

            return priority;
        }
    }
}