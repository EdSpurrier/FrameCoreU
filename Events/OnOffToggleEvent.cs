using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameCoreU.Events
{
    [System.Serializable]
    public class OnOffToggleEvent
    {
        [ToggleGroup("enabled", "$eventName")]
        public bool enabled = false;

        [HideInInspector]
        public string eventName = "Toggle Event Name";

        [HideInInspector]
        [ToggleGroup("enabled")]
        public bool isOn = false;

        [FoldoutGroup("enabled/On Event")]
        [HideLabel]
        public FrameCoreEvent onEvent;

        [FoldoutGroup("enabled/Off Event")]
        [HideLabel]
        public FrameCoreEvent offEvent;

        public void Toggle()
        {
            SetState(!isOn);
        }

        public void SetState(bool state)
        {
            if (!enabled)
                return;

            if (state == isOn)
                return;

            isOn = state;

            if (isOn)
                onEvent?.Activate();
            else
                offEvent?.Activate();
        }
    }
}