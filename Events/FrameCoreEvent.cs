using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using FrameCoreU.Timing;
using FrameCoreU.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace FrameCoreU.Events
{
    [System.Serializable]
    public class FrameCoreEvent
    {
        [HideLabel] [SuffixLabel("Event Name", Overlay = true)]
        public string eventName = "Event Name";

        private void ActivateTriggerEvent()
        {
            if (EditorInteractions.InPlayerButton())
            {
                TriggerEvent();
            }
        }

        bool hideActionDetails = false;

        [HideIfGroup("hideActionDetails")]


        [HorizontalGroup("Split", 0.5f)]
        [Button(ButtonSizes.Small), GUIColor(0f, 0.8f, 1)]
        public void HideActionDetails()
        {
            Debug.Log("Hiding Action Details...");
            hideActionDetails = !hideActionDetails;
        }
        
        [HideLabel] [HorizontalGroup("Split", 0.5f)] [SuffixLabel("Queue (s)", Overlay = true)]
        public float waitTime = 0f;

        [InlineButton("ActivateTriggerEvent")] [HideLabel]
        public Debounce debounce;
        
        [ListDrawerSettings(ShowFoldout = true, ListElementLabelName = "DisplayName")]
        [HideReferenceObjectPicker]
        [OdinSerialize, SerializeReference]
        public List<FrameAction> actions = new List<FrameAction>();


        [FoldoutGroup("Event")] [Title("Events")]
        public UnityEvent triggerEvent;

        [FoldoutGroup("hideActionDetails/System")]
        public bool active = false;

        [FoldoutGroup("hideActionDetails/System")]
        public EventManager eventManager;

        [FoldoutGroup("hideActionDetails/System")]
        public float processTime = 0f;

        public void ResetActions()
        {
            foreach (FrameAction action in actions)
            {
                if (action == null) continue;
                action.Reset();
            }
        }

        public void TriggerEvent()
        {
            DebugEvent();

            triggerEvent.Invoke();

            foreach (FrameAction action in actions)
            {
                if (action == null) continue;
                action.Execute();
            }

            if (eventManager)
            {
                eventManager.ActivateLoop();
            }

            ;

        }

        public void Activate(float queueTime = 0f)
        {
            // If debounce exists and is currently active, stop.
            if (debounce != null && debounce.CheckDebounce())
            {
                return;
            }
            
            ResetActions();

            processTime = waitTime + queueTime;

            if (processTime > 0f)
            {
                Frame.Events.ActivateTimedEvent(this);
            }
            else
            {
                TriggerEvent();
            }
        }


        // Update is called once per frame
        public bool EventUpdate()
        {
            if (!active)
            {
                return false;
            }

            if (processTime <= 0f)
            {
                TriggerEvent();
                return true;
            }

            processTime -= Time.deltaTime;

            return false;
        }

        void DebugEvent()
        {
            Debug.Log("FrameCoreEvent [" + Frame.Events.eventCoreTime + "] >> " + eventName + " - Triggered");
        }
        
#if UNITY_EDITOR
        public void ValidateData()
        {
            if (actions == null) return;

            foreach (FrameAction action in actions)
            {
                if (action == null) continue;
                action.ValidateData();
            }
        }
#endif

    }

}