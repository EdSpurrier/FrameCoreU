using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameCoreU.Events
{
    [System.Serializable]
    public abstract class FrameAction
    {
        [HorizontalGroup("Split", 0.50f)]
        [LabelText("$ActionType")]
        [ToggleLeft]
        public bool toggle = true;
        
        [HorizontalGroup("Split", 0.50f)]
        [HideLabel]
        [SuffixLabel("Action Name", Overlay = true)]
        public string actionName = "";

        public abstract string ActionType { get; }
        
        public string ToggleLabel => ActionType;
        
        public string DisplayName => string.IsNullOrEmpty(actionName) ? ActionType : ActionType + " - " + actionName;

        // The component whose FrameCoreEvent is running this action (FrameCoreEvent.Owner), set just before it runs. Pass
        // it as the context of any log (Log / LogWarning / LogError below do) so clicking the log in the Console selects
        // the object. Null if the event isn't linked to its owner, or outside a run (e.g. an editor button).
        [System.NonSerialized] private Object owner;

        public Object Owner => owner;
        
        public virtual void Reset()
        {
        }

        public void Execute()
        {
            Execute(null);
        }

        // eventOwner: the component whose event runs this action, so the action's logs point at it
        public void Execute(Object eventOwner)
        {
            owner = eventOwner;

            if (!toggle)
            {
                LogWarning("Action [" + ActionType + "] Disabled - " + actionName);
                return;
            }

            Activate();
        }

        // Logs that name the owning object and select it when clicked in the Console
        protected void Log(string message) => Debug.Log(message + Where, owner);
        protected void LogWarning(string message) => Debug.LogWarning(message + Where, owner);
        protected void LogError(string message) => Debug.LogError(message + Where, owner);

        private string Where => owner != null ? " (on '" + owner.name + "')" : "";

        protected abstract void Activate();
        
#if UNITY_EDITOR
        public virtual void ValidateData()
        {
            if (string.IsNullOrEmpty(actionName))
            {
                actionName = ActionType;
            }
        }
#endif
    }
}