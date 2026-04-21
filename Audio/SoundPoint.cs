using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameCoreU.Audio
{
    public class SoundPoint : MonoBehaviour
    {
        [Title("Sound Point Data")]
        [ShowInInspector, ReadOnly]
        private SoundPointData soundPointData;

        [Title("Parts")]
        [SerializeField] private AudioSource audioSource;

        private bool isPlaying;

        public SoundPointData SoundPointData => soundPointData;
        public AudioSource AudioSource => audioSource;
        public bool IsPlaying => isPlaying;

        public void Play(SoundPointData newSoundPointData)
        {
            if (newSoundPointData == null)
            {
                UnityEngine.Debug.LogError("SoundPoint [ERROR] >> Tried to play with null SoundPointData.");
                return;
            }

            if (audioSource == null)
            {
                UnityEngine.Debug.LogError("SoundPoint [ERROR] >> AudioSource is missing.");
                return;
            }

            soundPointData = newSoundPointData;
            soundPointData.Init();
            soundPointData.Play(audioSource);

            isPlaying = true;
        }

        public void Stop()
        {
            if (audioSource != null)
                audioSource.Stop();

            isPlaying = false;
        }

        public void ResetSoundPoint()
        {
            Stop();

            if (audioSource != null)
            {
                audioSource.clip = null;
                audioSource.mute = false;
                audioSource.loop = false;
            }

            soundPointData = null;
        }

        public void Mute()
        {
            if (audioSource != null)
                audioSource.mute = true;
        }

        public void UnMute()
        {
            if (audioSource != null)
                audioSource.mute = false;
        }

        private void Update()
        {
            if (!isPlaying || audioSource == null)
                return;

            if (!audioSource.isPlaying)
            {
                isPlaying = false;
                Frame.Sound.ReleaseSoundPoint(this);
                return;
            }

            if (soundPointData != null &&
                soundPointData.followReferencePosition &&
                soundPointData.referencePosition != null)
            {
                transform.position = soundPointData.referencePosition.position;
            }
        }
    }
}