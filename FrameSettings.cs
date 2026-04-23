using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameCoreU
{
    [CreateAssetMenu(menuName = "FrameCoreU/Frame Settings")]
    public class FrameSettings : ScriptableObject
    {
        [Header("Performance")]
        [HideLabel]
        [SuffixLabel("Target Frame Rate", Overlay = true)]
        public int targetFrameRate = 60;
    }
}