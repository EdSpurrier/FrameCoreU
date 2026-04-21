using Sirenix.OdinInspector;

namespace FrameCoreU.Data
{
    [System.Serializable]
    public class FloatRange
    {
        [HorizontalGroup("Split", 0.5f)]
        [HideLabel]
        [SuffixLabel("(float) Min", Overlay = true)]
        public float min = -1f;

        [HorizontalGroup("Split", 0.5f)]
        [HideLabel]
        [SuffixLabel("(float) Max", Overlay = true)]
        public float max = 1f;
    }
}