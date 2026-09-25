using System.Collections.Generic;
using UnityEngine;

namespace FrameCoreU.Pooling
{
    // Makes prefabs that come apart reusable: records the hierarchy as it was built (child parents, local
    // transforms, active states, Rigidbody kinematic flags) and puts it all back when the object is despawned -
    // including re-parenting children that were detached into the world (e.g. fractured debris) and stopping
    // anything still moving. Add it to the root of the pooled prefab.
    [DisallowMultipleComponent]
    public class PoolStateReset : MonoBehaviour, IPoolable
    {
        private struct TransformState
        {
            public Transform Transform;
            public Transform Parent;
            public Vector3 LocalPosition;
            public Quaternion LocalRotation;
            public Vector3 LocalScale;
            public bool Active;
        }

        private struct BodyState
        {
            public Rigidbody Body;
            public bool Kinematic;
        }

        private readonly List<TransformState> _transforms = new();
        private readonly List<BodyState> _bodies = new();

        private void Awake()
        {
            // Children come after their parents in this list, so restoring in order re-parents top-down.
            // The root is skipped - where it sits and whether it's active belong to the pool.
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child == transform) continue;

                _transforms.Add(new TransformState
                {
                    Transform = child,
                    Parent = child.parent,
                    LocalPosition = child.localPosition,
                    LocalRotation = child.localRotation,
                    LocalScale = child.localScale,
                    Active = child.gameObject.activeSelf
                });
            }

            foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>(true))
                _bodies.Add(new BodyState { Body = body, Kinematic = body.isKinematic });
        }

        public void OnSpawned() { }

        public void OnDespawned()
        {
            foreach (BodyState state in _bodies)
            {
                if (state.Body == null) continue;

                // Velocity can only be set on non-kinematic bodies
                if (!state.Body.isKinematic)
                {
                    state.Body.linearVelocity = Vector3.zero;
                    state.Body.angularVelocity = Vector3.zero;
                }

                state.Body.isKinematic = state.Kinematic;
            }

            foreach (TransformState state in _transforms)
            {
                // Destroyed while out in the world - nothing to put back
                if (state.Transform == null) continue;

                state.Transform.SetParent(state.Parent, false);
                state.Transform.localPosition = state.LocalPosition;
                state.Transform.localRotation = state.LocalRotation;
                state.Transform.localScale = state.LocalScale;
                state.Transform.gameObject.SetActive(state.Active);
            }
        }
    }
}
