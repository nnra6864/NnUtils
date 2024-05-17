using System;
using UnityEngine;

namespace NnUtils.Scripts
{
    public class WaitForSecondsWhileNot : CustomYieldInstruction
    {
        private float _timer;
        private float _seconds;
        private Func<bool> _isPaused;
        private bool _unscaled;

        /// <summary>
        /// Same as WaitForSeconds/WaitForSecondsRealtime but allows for pausing with bools
        /// <param name="seconds">Waiting period</param>
        /// <param name="whileNot">Pauses if true</param>
        /// <param name="unscaled">Uses unscaled time if true (default: false)</param>
        /// <example>
        /// <c>yield return new WaitForSecondsWhileNot(1, () => IsPaused, true)</c><br/>
        /// <c>yield return new WaitForSecondsWhileNot(0.5f, () => (IsPaused || IsJumping))</c>
        /// </example>
        /// </summary>
        public WaitForSecondsWhileNot(float seconds, Func<bool> whileNot, bool unscaled = false)
        {
            _seconds = seconds;
            _isPaused = whileNot;
            _unscaled = unscaled;
        }

        public override bool keepWaiting
        {
            get
            {
                if (_isPaused()) return true;
                _timer += _unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                return _timer < _seconds;
            }
        }
    }
}