using Sirenix.OdinInspector;
using System.Collections.Generic;
using FrameCoreU.Data;
using FrameCoreU.Timing;
using UnityEngine;

namespace FrameCoreU.Audio
{
    public enum SpatialSoundType
    {
        Spatial3d,
        Static2d
    }

    [System.Serializable]
    public class SoundPointData
    {
        [HorizontalGroup("Sound Point Header")]
        [HideLabel]
        public string soundName = "Sound Name";

        [HorizontalGroup("Sound Point Header")]
        [HideLabel]
        public Debounce debounce;

        public List<AudioClip> clips;

        [FoldoutGroup("Sound Point Data")]
        [HorizontalGroup("Sound Point Data/SplitA", 0.5f)]
        [BoxGroup("Sound Point Data/SplitA/Play Settings")]
        [ToggleLeft]
        public bool playOnAwake = false;

        [BoxGroup("Sound Point Data/SplitA/Play Settings")]
        [ToggleLeft]
        public bool loop = false;

        [BoxGroup("Sound Point Data/SplitA/Priority")]
        [HideLabel]
        [SuffixLabel("Priority", Overlay = true)]
        public Level prioritySetting = Level.Medium;

        private int priority = 256;

        [HorizontalGroup("Sound Point Data/Split", 0.5f)]
        [BoxGroup("Sound Point Data/Split/Spatial")]
        [HideLabel]
        public SpatialSoundType spatial3d = SpatialSoundType.Spatial3d;

        [BoxGroup("Sound Point Data/Split/Spatial")]
        [ShowIf("@this.spatial3d == SpatialSoundType.Spatial3d")]
        [HideLabel]
        [SuffixLabel("Ref Position", Overlay = true)]
        public Transform referencePosition;

        [BoxGroup("Sound Point Data/Split/Spatial")]
        [ShowIf("@this.spatial3d == SpatialSoundType.Spatial3d && this.referencePosition != null")]
        [ToggleLeft]
        public bool followReferencePosition;

        [BoxGroup("Sound Point Data/Split/Distance")]
        [HideLabel]
        [ShowIf("@this.spatial3d == SpatialSoundType.Spatial3d")]
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

        [BoxGroup("Sound Point Data/Split/Distance")]
        [HideLabel]
        [ShowIf("@this.spatial3d == SpatialSoundType.Spatial3d")]
        public FloatRange distance = new FloatRange
        {
            min = 5f,
            max = 20f
        };

        [HorizontalGroup("Sound Point Data/Split2", 0.5f)]
        [BoxGroup("Sound Point Data/Split2/Volume")]
        [ToggleLeft]
        public bool randomVolume = true;

        [BoxGroup("Sound Point Data/Split2/Volume")]
        [HideIf("randomVolume")]
        [SuffixLabel("Volume", Overlay = true)]
        [HideLabel]
        public float volume = 1f;

        [BoxGroup("Sound Point Data/Split2/Volume")]
        [ShowIf("randomVolume")]
        [HideLabel]
        public FloatRange volumeRange = new FloatRange
        {
            min = 0.8f,
            max = 1f,
        };

        [BoxGroup("Sound Point Data/Split2/Pitch")]
        [ToggleLeft]
        public bool randomPitch = true;

        [BoxGroup("Sound Point Data/Split2/Pitch")]
        [HideIf("randomPitch")]
        [SuffixLabel("Pitch", Overlay = true)]
        [HideLabel]
        public float pitch = 1f;

        [BoxGroup("Sound Point Data/Split2/Pitch")]
        [ShowIf("randomPitch")]
        [HideLabel]
        public FloatRange pitchRange = new FloatRange
        {
            min = 0.9f,
            max = 1f,
        };

        public void PositionSoundPoint(AudioSource audioSource)
        {
            if (spatial3d == SpatialSoundType.Spatial3d && referencePosition != null)
            {
                audioSource.transform.position = referencePosition.position;
            }
        }

        public void Apply(AudioSource audioSource)
        {
            audioSource.priority = priority;
            audioSource.playOnAwake = playOnAwake;
            audioSource.loop = loop;
            audioSource.rolloffMode = rolloffMode;
            audioSource.minDistance = distance.min;
            audioSource.maxDistance = distance.max;
            audioSource.spatialBlend = spatial3d == SpatialSoundType.Spatial3d ? 1f : 0f;

            PositionSoundPoint(audioSource);
        }

        public AudioClip GetClip()
        {
            if (clips == null || clips.Count == 0)
                return null;

            return clips[Random.Range(0, clips.Count)];
        }

        public float GetVolume()
        {
            return randomVolume ? Random.Range(volumeRange.min, volumeRange.max) : volume;
        }

        public float GetPitch()
        {
            return randomPitch ? Random.Range(pitchRange.min, pitchRange.max) : pitch;
        }

        public void Play(AudioSource audioSource)
        {
            Apply(audioSource);

            AudioClip clip = GetClip();
            if (clip == null)
            {
                UnityEngine.Debug.LogWarning($"SoundPointData >> No clips assigned for {soundName}");
                return;
            }

            audioSource.clip = clip;
            audioSource.volume = GetVolume();
            audioSource.pitch = GetPitch();
            audioSource.Play();
        }

        public void Init()
        {
            priority = Frame.Sound.GetPriority(prioritySetting);
        }
    }
}