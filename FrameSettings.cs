using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameCoreU
{
    // What FrameCoreEvents log to the Console when they fire
    public enum EventLogging
    {
        All,            // every event
        LinkedOnly,     // only events that know their owning object (clicking the log selects it)
        Off
    }

    [CreateAssetMenu(menuName = "FrameCoreU/Frame Settings")]
    public class FrameSettings : ScriptableObject
    {
        [Header("Performance")]
        [HideLabel]
        [SuffixLabel("Target Frame Rate", Overlay = true)]
        public int targetFrameRate = 60;

        [Header("Debugging")]
        [Tooltip("What FrameCoreEvents log to the Console when they fire. All: every event. Linked Only: only events that know their owning object - clicking the log selects it (run FrameCoreU > Link Event Owners once to link existing ones). Off: nothing.")]
        public EventLogging eventLogging = EventLogging.All;
    }
}