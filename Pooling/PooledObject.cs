using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameCoreU.Pooling
{
    // Added by PoolCore to every object it creates, so the object knows which pool entry to go back to.
    // Also runs delayed despawns - it only ticks while one is scheduled.
    [DisallowMultipleComponent]
    public class PooledObject : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] public PoolCore Pool { get; private set; }
        [ShowInInspector, ReadOnly] public bool IsSpawned { get; private set; }

        internal PoolCore.PoolObject Entry { get; private set; }

        private float _despawnAt;

        internal void Bind(PoolCore pool, PoolCore.PoolObject entry)
        {
            Pool = pool;
            Entry = entry;
            enabled = false;
        }

        internal void MarkSpawned()
        {
            IsSpawned = true;
            enabled = false;
        }

        internal void MarkDespawned()
        {
            IsSpawned = false;
            enabled = false;
        }

        public void Despawn(float delay = 0f)
        {
            if (Pool == null)
            {
                Destroy(gameObject, delay);
                return;
            }

            if (delay <= 0f)
            {
                Pool.Despawn(gameObject);
                return;
            }

            _despawnAt = Time.time + delay;
            enabled = true;
        }

        private void Update()
        {
            if (Time.time < _despawnAt) return;

            enabled = false;
            Pool.Despawn(gameObject);
        }
    }
}
