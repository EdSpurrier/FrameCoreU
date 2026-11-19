using FrameCoreU.Audio;
using FrameCoreU.Pooling;
using FrameCoreU.Timing;
using FrameCoreU.Events;
using FrameCoreU.Scene;

namespace FrameCoreU
{
    public static class Frame
    {
        public static FrameCore Core { get; private set; }

        // False before FrameCore wakes up, after it's destroyed (e.g. while leaving Play mode), or in a scene without one -
        // check it before using Events/Pools/Sound/Scenes/Debouncer from code that can run at those times
        public static bool HasCore => Core != null;

        // Null when there's no FrameCore or it has no FrameSettings assigned
        public static FrameSettings Settings => Core != null ? Core.Settings : null;

        public static EventCore Events => Core.Events;
        public static PoolCore Pools => Core.Pools;
        public static SoundCore Sound => Core.Sound;
        public static SceneCore Scenes => Core.Scenes;
        public static Debouncer Debouncer => Core.Debouncer;
        
        internal static void SetCore(FrameCore core)
        {
            Core = core;
        }

        internal static void ClearCore(FrameCore core)
        {
            if (Core == core)
                Core = null;
        }
    }
}