using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrameCoreU.Events.Library
{
    public class EventLibrary : MonoBehaviour
    {
        [SerializeField] private List<FrameCoreEvent> events = new();

        public bool TriggerEvent(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                Debug.LogError("EventLibrary >> Event name is null or empty.");
                return false;
            }

            FrameCoreEvent frameCoreEvent = events.FirstOrDefault(e => e != null && e.eventName == eventName);

            if (frameCoreEvent == null)
            {
                Debug.LogError("EventLibrary >> No FrameCoreEvent found for: " + eventName);
                return false;
            }

            frameCoreEvent.Activate();
            return true;
        }
    }
}