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
        
        public virtual void Reset()
        {
        }

        public void Execute()
        {
            if (!toggle)
            {
                Debug.LogWarning("Action [" + ActionType + "] Disabled - " + actionName);
                return;
            }

            Activate();
        }

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