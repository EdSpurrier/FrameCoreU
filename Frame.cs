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