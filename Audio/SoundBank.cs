using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrameCoreU.Audio
{
    public class SoundBank : MonoBehaviour
    {
        [Title("Sound Data")]
        public List<SoundPointData> soundPoints = new();
        
        public void Play(int soundId)
        {
            if (soundId < 0 || soundId >= soundPoints.Count)
            {
                UnityEngine.Debug.LogWarning($"SoundBank >> Invalid sound id: {soundId}");
                return;
            }

            SoundPointData sound = soundPoints[soundId];

            if (sound == null)
            {
                UnityEngine.Debug.LogWarning($"SoundBank >> Sound at index {soundId} is null.");
                return;
            }

            if (sound.debounce != null && sound.debounce.CheckDebounce())
                return;

            Frame.Sound.GenerateSoundPoint(sound);
        }

        public void Play(string soundName)
        {
            SoundPointData sound = soundPoints.FirstOrDefault(soundData => soundData.soundName == soundName);

            if (sound == null)
            {
                UnityEngine.Debug.LogWarning($"SoundBank >> No sound found with name: {soundName}");
                return;
            }

            if (sound.debounce != null && sound.debounce.CheckDebounce())
                return;

            Frame.Sound.GenerateSoundPoint(sound);
        }

        private void OnValidate()
        {
            foreach (SoundPointData sound in soundPoints)
            {
                if (sound != null && sound.referencePosition == null)
                    sound.referencePosition = transform;
            }
        }
    }
}