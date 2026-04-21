using System.Collections.Generic;
using System.Linq;
using FrameCoreU.Unity;
using UnityEngine;
using Sirenix.OdinInspector;

namespace FrameCoreU.Events.Library
{
    public class EventManagerLibrary : MonoBehaviour
    {
        [System.Serializable]
        public class EventManagerEntry
        {
            [InlineButton("TriggerEvent")] [HideLabel]
            public string eventName = "Event";

            [HideLabel] [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
            public EventManager eventManager;

            public void TriggerEvent()
            {
                if (!EditorInteractions.InPlayerButton())
                    return;

                if (eventManager == null)
                {
                    Debug.LogError("EventManagerLibrary >> EventManager is null.");
                    return;
                }

                eventManager.Activate();
            }
        }

        [SerializeField] private List<EventManagerEntry> eventManagers = new();

        public bool TriggerRandomEventManager()
        {
            if (eventManagers == null || eventManagers.Count == 0)
            {
                Debug.LogError("EventManagerLibrary >> No EventManagers available.");
                return false;
            }

            EventManagerEntry entry = eventManagers[Random.Range(0, eventManagers.Count)];

            if (entry == null || entry.eventManager == null)
            {
                Debug.LogError("EventManagerLibrary >> Random EventManager entry is null.");
                return false;
            }

            entry.eventManager.Activate();
            return true;
        }

        public bool TriggerEventManager(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                Debug.LogError("EventManagerLibrary >> Event name is null or empty.");
                return false;
            }

            EventManagerEntry entry = eventManagers.FirstOrDefault(e => e != null && e.eventName == eventName);

            if (entry == null)
            {
                Debug.LogError("EventManagerLibrary >> No EventManager found for: " + eventName);
                return false;
            }

            if (entry.eventManager == null)
            {
                Debug.LogError("EventManagerLibrary >> EventManager is null for: " + eventName);
                return false;
            }

            entry.eventManager.Activate();
            return true;
        }

        private void OnValidate()
        {
            foreach (var entry in eventManagers)
            {
                if (entry?.eventManager == null)
                    continue;

                if (entry.eventName == "Event")
                {
                    entry.eventName = entry.eventManager.eventManagerName;
                }
            }
        }
    }
}