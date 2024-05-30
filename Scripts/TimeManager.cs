//Attach this script to your game manager and get a reference to it there

using System;
using System.Collections;
using UnityEngine;

namespace NnUtils.Scripts
{
    public class TimeManager : NnBehaviour
    {
        private float _fixedTimeStep;
        /// <summary>
        /// Set to true when the game is paused to pause transitions
        /// </summary>
        public bool IsPaused;
        private Coroutine _changeTimeScaleRoutine;
        private void Awake() => _fixedTimeStep = Time.fixedDeltaTime;
        
        /// <summary>
        /// Set via <see cref="ChangeTimeScale(float, float)"/> function
        /// <returns><see cref="Time.timeScale"/></returns>
        /// </summary>
        public float TimeScale
        {
            get => Time.timeScale;
            private set
            {
                if (value == Time.timeScale) return;
                Time.timeScale = value;
                Time.fixedDeltaTime = _fixedTimeStep * TimeScale;
                OnTimeScaleChanged?.Invoke();
            }
        }
        /// <summary>
        /// Invoked on every TimeScale change
        /// </summary>
        public Action OnTimeScaleChanged;
        /// <summary>
        /// Invoked when TimeScale reached the target value
        /// </summary>
        public Action OnTimeScaleTransitioned;
        /// <summary>
        /// Invoked when using <see cref="ChangeTimeScale(float[],float[])"/> and all the TimeScales have been applied
        /// </summary>
        public Action OnTimeScalesTransitioned;
        
        public void ChangeTimeScale(float timeScale, float time = 0)
            => RestartRoutine(ref _changeTimeScaleRoutine, ChangeTimeScaleRoutine(timeScale, time, Easings.Types.None));
        public void ChangeTimeScale(float timeScale, float time, Easings.Types easing)
            => RestartRoutine(ref _changeTimeScaleRoutine, ChangeTimeScaleRoutine(timeScale, time, easing));
        public void ChangeTimeScale(float timeScale, float time, AnimationCurve curve)
            => RestartRoutine(ref _changeTimeScaleRoutine, ChangeTimeScaleRoutine(timeScale, time, curve));
        
        private IEnumerator ChangeTimeScaleRoutine(float timeScale, float time, Easings.Types easing)
        {
            var startTimeScale = TimeScale;
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                if (IsPaused) { yield return null; continue; }
                var t = Misc.UpdateLerpPos(ref lerpPos, time, true, easing);
                TimeScale = Mathf.LerpUnclamped(startTimeScale, timeScale, t);
                yield return null;
            }
            _changeTimeScaleRoutine = null;
            OnTimeScaleTransitioned?.Invoke();
        }
        private IEnumerator ChangeTimeScaleRoutine(float timeScale, float time, AnimationCurve curve)
        {
            var startTimeScale = TimeScale;
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                if (IsPaused) { yield return null; continue; }
                var t = curve.Evaluate(Misc.UpdateLerpPos(ref lerpPos, time, true));
                TimeScale = Mathf.LerpUnclamped(startTimeScale, timeScale, t);
                yield return null;
            }
            _changeTimeScaleRoutine = null;
            OnTimeScaleTransitioned?.Invoke();
        }
        
        public void ChangeTimeScale(float[] timeScales, float[] times)
            => RestartRoutine(ref _changeTimeScaleRoutine, ChangeTimeScaleRoutine(timeScales, times, new Easings.Types[]{}));
        public void ChangeTimeScale(float[] timeScales, float[] times, Easings.Types[] easings)
            => RestartRoutine(ref _changeTimeScaleRoutine, ChangeTimeScaleRoutine(timeScales, times, easings));
        public void ChangeTimeScale(float[] timeScales, float[] times, AnimationCurve[] curves)
            => RestartRoutine(ref _changeTimeScaleRoutine, ChangeTimeScaleRoutine(timeScales, times, curves));
        
        private IEnumerator ChangeTimeScaleRoutine(float[] timeScales, float[] times, Easings.Types[] easings)
        {
            for (int i = 0; i < timeScales.Length; i++)
            {
                var startTimeScale = TimeScale;
                var timeScale = timeScales[i];
                var time = times.Length - 1 < i ? 0 : times[i];
                var easing = easings.Length - 1 < i ? Easings.Types.None : easings[i];
                float lerpPos = 0;
                while (lerpPos < 1)
                {
                    if (IsPaused) { yield return null; continue; }
                    var t = Misc.UpdateLerpPos(ref lerpPos, time, true, easing);
                    TimeScale = Mathf.LerpUnclamped(startTimeScale, timeScale, t);
                    yield return null;
                }
                _changeTimeScaleRoutine = null;
                OnTimeScaleTransitioned?.Invoke();
            }
            OnTimeScalesTransitioned?.Invoke();
        }
        private IEnumerator ChangeTimeScaleRoutine(float[] timeScales, float[] times, AnimationCurve[] curves)
        {
            for (int i = 0; i < timeScales.Length; i++)
            {
                var startTimeScale = TimeScale;
                var timeScale = timeScales[i];
                var time = times.Length - 1 < i ? 0 : times[i];
                var curve = curves.Length - 1 < i ? new AnimationCurve() : curves[i];
                float lerpPos = 0;
                while (lerpPos < 1)
                {
                    if (IsPaused) { yield return null; continue; }
                    var t = curve.Evaluate(Misc.UpdateLerpPos(ref lerpPos, time, true));
                    TimeScale = Mathf.LerpUnclamped(startTimeScale, timeScale, t);
                    yield return null;
                }
                _changeTimeScaleRoutine = null;
                OnTimeScaleTransitioned?.Invoke();
            }
            OnTimeScalesTransitioned?.Invoke();
        }
    }
}