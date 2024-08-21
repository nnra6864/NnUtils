using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NnUtils.Scripts.Audio
{
    public class SoundEmitter : NnBehaviour
    {
        private static TimeManager TimeManager => NnManager.TimeManager;
        private Sound _sound;
        private AudioSource _source;
        private float _pitch;
        private bool _isPlaying;
        private bool _destroy, _destroySource, _destroyObject;

        public void Init(Sound sound, AudioSource source = null, bool destroy = false, bool destroySource = false, bool destroyObject = true)
        {
            _sound = sound;
            _pitch = Random.Range(sound.PitchRange.x, sound.PitchRange.y);
            _source = source ? source : gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.outputAudioMixerGroup = _sound.Group;
            _source.clip = _sound.Clip;
            _source.volume = _sound.Volume;
            _source.pitch = _pitch;
            _source.loop = _sound.Loop;
            _source.spatialBlend = _sound.SpatialBlend;
            _destroy = destroy;
            _destroySource = destroySource;
            _destroyObject = destroyObject;
            
            if (_sound.FadeIn) FadeIn();
            if (_sound.FadeOut) FadeOut();
            StartCoroutine(DestroyRoutine());
            if (!_sound.Unscaled) TimeManager.OnTimeScaleChanged += OnTimeScaleChanged;
        }

        public void Play()
        {
            if (_sound.GetPitchOnPlay)
            {
                _pitch = Random.Range(_sound.PitchRange.x, _sound.PitchRange.y);
                _source.pitch = _sound.Unscaled ? _pitch : _pitch * Time.timeScale;
            }
            _source.Play();
            _isPlaying = true;
        }
        public void Play(float pitch)
        {
            _pitch = pitch;
            _source.pitch = _sound.Unscaled ? _pitch : _pitch * Time.timeScale;
            _source.Play();
            _isPlaying = true;
        }
        public void UnPause()
        {
            _source.UnPause();
            _isPlaying = true;
        }

        public void Pause()
        {
            _source.Pause();
            _isPlaying = false;
        }

        public void Stop()
        {
            _source.Stop();
            _isPlaying = false;
        }

        #region ChangeVolume
        
        public void ChangeVolume(float volume, float time = 1, Easings.Types easing = Easings.Types.SineIn) => 
            RestartRoutine(ref _changeVolumeRoutine, ChangeVolumeRoutine(volume, time, easing));

        private Coroutine _changeVolumeRoutine;
        private IEnumerator ChangeVolumeRoutine(float volume, float time = 0, Easings.Types easing = Easings.Types.None)
        {
            var startVolume = _source.volume;
            float lerpPos = 0;
            
            while (lerpPos < 1)
            {
                var t = Misc.UpdateLerpPos(ref lerpPos, time, easingType: easing);
                _source.volume = Mathf.LerpUnclamped(startVolume, volume, t);
                yield return null;
            }

            _changeVolumeRoutine = null;
        }

        #endregion
        
        #region Fade
        //TODO: Support volume changes while fading
        
        private void FadeIn()
        {
            var vol = _source.volume;
            if (_sound.FadeInTime == 0) return;
            _source.volume = 0;
            ChangeVolume(vol, _sound.FadeInTime, _sound.FadeInEasing);
        }
        
        private void FadeOut() => StartNullRoutine(ref _fadeOutRoutine, FadeOutRoutine());
        private Coroutine _fadeOutRoutine;
        private IEnumerator FadeOutRoutine()
        {
            if (_sound.FadeOutTime <= 0)
            {
                _source.volume = 0;
                yield break;
            }

            var delay = _sound.Clip.length / _pitch - _sound.FadeOutTime;
            if (delay <= 0)
            {
                delay = 0;
                _sound.FadeOutTime = _sound.Clip.length;
            }
            
            yield return new WaitForSecondsWhileNot(delay, () => !_isPlaying);
            ChangeVolume(0, _sound.FadeOutTime, _sound.FadeOutEasing);
        }
        
        #endregion

        private void OnTimeScaleChanged(float timeScale) => _source.pitch = _pitch * timeScale;
        
        private IEnumerator DestroyRoutine()
        {
            yield return new WaitForSecondsWhileNot(_sound.Clip.length / _pitch + 1, () => !_isPlaying);
            if (_destroy) Destroy(this);
            if (_destroySource) Destroy(_source);
            if (_destroyObject) Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (!_sound.Unscaled) TimeManager.OnTimeScaleChanged -= OnTimeScaleChanged;
        }
    }
}