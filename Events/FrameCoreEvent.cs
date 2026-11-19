using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
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

        // The component this event belongs to. The editor fills it in (FrameCoreEventOwners: on save, on selecting the
        // object, or FrameCoreU > Link Event Owners), and pooled/spawned copies keep it, pointing at their own copy.
        // Logs pass it as their context, so clicking one in the Console selects the object. Unlinked events still work.
        [SerializeField, HideInInspector] private UnityEngine.Object owner;

        public UnityEngine.Object Owner => owner;

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

            // A Unity Event or action that throws is logged against this event's object (click to select it), and the
            // rest of the event still runs - one broken action no longer stops the ones after it
            try
            {
                triggerEvent?.Invoke();
            }
            catch (Exception exception)
            {
                LogFailure("its Unity Event", exception);
            }

            foreach (FrameAction action in actions)
            {
                if (action == null) continue;

                try
                {
                    action.Execute(owner);
                }
                catch (Exception exception)
                {
                    LogFailure("action '" + action.DisplayName + "'", exception);
                }
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
                if (Frame.HasCore)
                {
                    Frame.Events.ActivateTimedEvent(this);
                }
                else
                {
                    // Nothing can count the delay down without FrameCore (e.g. while leaving Play mode)
                    Debug.LogWarning("FrameCoreEvent >> " + Describe() + " has a " + processTime + "s delay but there's no FrameCore to time it - triggering now.", owner);
                    TriggerEvent();
                }
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
            if (!ShouldLog(owner))
                return;

            // FrameCore can already be gone (leaving Play mode, or a scene without one): fall back to Unity's clock
            float time = Frame.HasCore && Frame.Events != null ? Frame.Events.eventCoreTime : Time.time;
            Debug.Log("FrameCoreEvent [" + time.ToString("0.00") + "] >> " + Describe() + " - Triggered", owner);
        }

        // Two Console entries, both selecting the owner when clicked: a one-line summary of what failed where, then the
        // exception itself with its stack trace
        private void LogFailure(string what, Exception exception)
        {
            Debug.LogError("FrameCoreEvent >> " + what + " in " + Describe() + " threw " + exception.GetType().Name + ": " + exception.Message, owner);
            Debug.LogException(exception, owner);
        }

        // "Event Name on 'Object' (Component)" - or just the name if the event isn't linked to its owner yet
        public string Describe()
        {
            if (owner is Component component && component != null)
                return eventName + " on '" + component.name + "' (" + component.GetType().Name + ")";
            if (owner != null)
                return eventName + " on '" + owner.name + "'";
            return eventName;
        }

        // FrameSettings' Event Logging: All (also when there's no FrameCore/FrameSettings), Linked Only, or Off
        public static bool ShouldLog(UnityEngine.Object eventOwner)
        {
            FrameSettings settings = Frame.Settings;
            EventLogging logging = settings != null ? settings.eventLogging : EventLogging.All;
            return logging == EventLogging.All || (logging == EventLogging.LinkedOnly && eventOwner != null);
        }
        
#if UNITY_EDITOR
        // Editor only (FrameCoreEventOwners): returns true if it changed
        public bool SetOwner(UnityEngine.Object value)
        {
            if (owner == value)
                return false;
            owner = value;
            return true;
        }

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