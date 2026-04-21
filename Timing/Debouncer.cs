using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCoreU.Timing
{
    [System.Serializable]
    public class Debounce
    {
        [HorizontalGroup("Split", 0.7f)]
        [HideLabel]
        [SuffixLabel("Debounce (s)", Overlay = true)]
        public float debounceTime = 0f;

        [HorizontalGroup("Split", 0.3f)]
        [HideLabel]
        [SuffixLabel("db (s)", Overlay = true)]
        public float currentDebounceTime = 0f;

        [HideInInspector]
        private bool active = false;

        public bool Active => active;

        public bool CheckDebounce()
        {
            if (active)
                return true;

            if (debounceTime <= 0f)
                return false;

            currentDebounceTime = debounceTime;
            active = true;

            Frame.Debouncer?.ActivateDebounce(this);

            return false;
        }

        public bool DebounceUpdate()
        {
            if (currentDebounceTime <= 0f)
            {
                active = false;
                currentDebounceTime = 0f;
                return false;
            }

            currentDebounceTime -= Time.deltaTime;
            return true;
        }

        public void Reset()
        {
            active = false;
            currentDebounceTime = 0f;
        }
    }

    [System.Serializable]
    public class Debouncer
    {
        [Title("Debounces")]
        public List<Debounce> debounces = new();

        private readonly List<Debounce> debouncesCompleted = new();

        public void ActivateDebounce(Debounce debounce)
        {
            if (debounce == null)
                return;

            if (!debounces.Contains(debounce))
                debounces.Add(debounce);
        }

        public void Update()
        {
            UpdateDebounces();
            ClearCompletedDebounces();
        }

        private void UpdateDebounces()
        {
            foreach (Debounce debounce in debounces)
            {
                if (debounce == null)
                {
                    debouncesCompleted.Add(debounce);
                    continue;
                }

                if (!debounce.DebounceUpdate())
                    debouncesCompleted.Add(debounce);
            }
        }

        private void ClearCompletedDebounces()
        {
            foreach (Debounce debounce in debouncesCompleted)
            {
                debounces.Remove(debounce);
            }

            debouncesCompleted.Clear();
        }

        public void ClearAll()
        {
            debounces.Clear();
            debouncesCompleted.Clear();
        }
    }
}