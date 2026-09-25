namespace FrameCoreU.Pooling
{
    // Implement on any component of a pooled prefab (root or children) that needs to reset between uses.
    public interface IPoolable
    {
        // After the object is positioned and activated by PoolCore.SpawnObject
        void OnSpawned();

        // Just before the object is deactivated and put back in the pool
        void OnDespawned();
    }
}
