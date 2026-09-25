using FrameCoreU.Pooling;
using UnityEngine;

namespace FrameCoreU.Unity
{
    public static class ExtensionMethods
    {

        public static GameObject SpawnObject(this Transform transform, Vector3 position, Quaternion rotation)
        {
            return Frame.Pools.SpawnObject(transform, position, rotation);
        }

        // Returns the object to the pool it was spawned from, or destroys it if it didn't come from a pool -
        // so it's the one call to use for "I'm done with this" either way.
        public static void Despawn(this GameObject gameObject, float delay = 0f)
        {
            if (gameObject == null)
                return;

            if (gameObject.TryGetComponent(out PooledObject pooled))
                pooled.Despawn(delay);
            else
                Object.Destroy(gameObject, delay);
        }
    }
}
