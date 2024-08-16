using UnityEngine;
using UnityEngine.Audio;

namespace NnUtils.Scripts.Audio
{
    [CreateAssetMenu(fileName = "Sound", menuName = "NnUtils/Sound")]
    public class Sound : ScriptableObject
    {
        public AudioMixerGroup Group;
        public string Name = "Sound";
        public AudioClip Clip;
        [Range(0, 1)] public float Volume = 1;
        public Vector2 PitchRange = Vector2.one;
        public bool GetPitchOnPlay = true;
        public bool Unscaled;
        public bool Loop;
        [Range(0, 1)] public float SpatialBlend;
        public bool FadeIn;
        public float FadeInTime = 1;
        public Easings.Types FadeInEasing = Easings.Types.SineIn;
        public bool FadeOut;
        public float FadeOutTime = 1;
        public Easings.Types FadeOutEasing = Easings.Types.SineOut;
    }
}