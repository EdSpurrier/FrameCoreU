using FrameCoreU.Unity;
using UnityEngine;

namespace FrameCoreU.Pooling
{
    // Despawns the object a set time after it's spawned (or enabled) - for effects, debris and anything else
    // that should clean itself up. Returns it to its pool if it came from one, otherwise destroys it.
    public class PoolLifetime : MonoBehaviour
    {
        [Tooltip("Seconds after being spawned before it despawns.")]
        [SerializeField, Min(0f)] private float lifetime = 3f;

        private float _timeLeft;

        private void OnEnable()
        {
            _timeLeft = lifetime;
        }

        private void Update()
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft > 0f) return;

            // Despawning deactivates the object, which stops this Update. Don't disable the component itself,
            // or OnEnable wouldn't restart the timer the next time it's spawned.
            gameObject.Despawn();
        }
    }
}
