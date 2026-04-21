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
            Debug.Log("Activated Timed Event >> " + frameCoreEvent.eventName);
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
            foreach (FrameCoreEvent timedEvent in timedEvents)
            {
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