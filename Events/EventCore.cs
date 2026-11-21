using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCoreU.Events
{
    public class EventCore : MonoBehaviour
    {

        [Title("Events")] public List<FrameCoreEvent> timedEvents;
        List<FrameCoreEvent> timedEvents_Completed = new List<FrameCoreEvent>();

        [Title("System")] public float eventCoreTime = 0f;

        public void ActivateTimedEvent(FrameCoreEvent frameCoreEvent)
        {
            if (FrameCoreEvent.ShouldLog(frameCoreEvent.Owner))
                Debug.Log("Activated Timed Event >> " + frameCoreEvent.Describe(), frameCoreEvent.Owner);
            timedEvents.Add(frameCoreEvent);
            frameCoreEvent.active = true;
        }
        

        void Update()
        {
            eventCoreTime += Time.deltaTime;

            UpdateTimedEvents();

            ClearCompletedEvents();
        }
        
        public void UpdateTimedEvents()
        {
            // By index, up to the count at the start: a timed event's actions can start other timed events (e.g. one
            // rope's delayed On Cut snapping the others, whose On Cut is delayed too), which adds to this list while it's
            // being walked. Those new ones start counting from the next frame; removals wait for ClearCompletedEvents.
            int count = timedEvents.Count;
            for (int i = 0; i < count; i++)
            {
                FrameCoreEvent timedEvent = timedEvents[i];
                if (timedEvent == null)
                {
                    Debug.Log("Removing NULL Timed Event");
                    timedEvents_Completed.Add(timedEvent);
                    continue;
                }
                
                if (timedEvent.EventUpdate())
                {
                    Debug.Log("Timed Event Complete >> " + timedEvent.eventName);
                    timedEvents_Completed.Add(timedEvent);
                }
            }
        }
        
        public void ClearCompletedEvents()
        {
            foreach (FrameCoreEvent timedEvent in timedEvents_Completed)
            {
                timedEvents.Remove(timedEvent);
            }

            timedEvents_Completed.Clear();
        }

    }
}