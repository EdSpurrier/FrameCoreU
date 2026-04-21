using UnityEngine;

namespace FrameCoreU.Unity
{
    public static class ExtensionMethods
    {

        public static GameObject SpawnObject(this Transform transform, Vector3 position, Quaternion rotation)
        {
            return Frame.Pools.SpawnObject(transform, position, rotation);
        }
        
    }
}