using System;
using System.Collections;
using UnityEngine;

namespace NnUtils.Scripts
{
    public class NnBehaviour : MonoBehaviour
    {
        public void StartRoutineIf(ref Coroutine target, IEnumerator routine, Func<bool> startIf) =>
            Misc.StartRoutineIf(this, ref target, routine, startIf);

        public void StartNullRoutine(ref Coroutine target, IEnumerator routine) =>
            Misc.StartNullRoutine(this, ref target, routine);
        
        public void RestartRoutine(ref Coroutine target, IEnumerator routine) =>
            Misc.RestartRoutine(this, ref target, routine);

        public void StopRoutine(ref Coroutine target) =>
            Misc.StopRoutine(this, ref target);

        #region Animation
        private Coroutine _animatePositionRoutine, _animateRotationRoutine, _animateScaleRoutine;
        public bool IsAnimating =>
            _animatePositionRoutine != null || _animateRotationRoutine != null || _animateScaleRoutine != null;
        
        public void Animate(AnimationParams animParams)
        {
            RestartRoutine(ref _animatePositionRoutine, AnimatePositionRoutine(animParams));
            RestartRoutine(ref _animateRotationRoutine, AnimateRotationRoutine(animParams));
            RestartRoutine(ref _animateScaleRoutine, AnimateScaleRoutine(animParams));
        }

        private IEnumerator AnimatePositionRoutine(AnimationParams animParams)
        {
            var startPos = transform.localPosition;
            var targetPos = animParams.AdditivePosition ? startPos + animParams.Position : animParams.Position;
            var prevPos = startPos;
            float lerpPos = 0;

            while (lerpPos < 1)
            {
                var posT = Misc.UpdateLerpPos(ref lerpPos, animParams.PositionDuration, animParams.Unscaled, animParams.PositionEasing);
                var newPos = Vector3.LerpUnclamped(startPos, targetPos, posT);
                var posDelta = newPos - prevPos;
                transform.localPosition += posDelta;
                prevPos = newPos;
                yield return null;
            }

            _animatePositionRoutine = null;
        }

        private IEnumerator AnimateRotationRoutine(AnimationParams animParams)
        {
            var startRot = transform.localRotation.eulerAngles;
            var targetRot = animParams.AdditiveRotation ? startRot + animParams.Rotation : animParams.Rotation;
            var prevRot = startRot;
            float lerpRot = 0;

            while (lerpRot < 1)
            {
                var rotT = Misc.UpdateLerpPos(ref lerpRot, animParams.RotationDuration, animParams.Unscaled, animParams.RotationEasing);
                var newRot = Vector3.LerpUnclamped(startRot, targetRot, rotT);
                var rotDelta = newRot - prevRot;
                transform.localRotation *= Quaternion.Euler(rotDelta);
                prevRot = newRot;
                yield return null;
            }
            
            _animateRotationRoutine = null;
        }
        
        private IEnumerator AnimateScaleRoutine(AnimationParams animParams)
        {
            var startScale = transform.localScale;
            var targetScale = animParams.AdditiveScale ? startScale + animParams.Scale : animParams.Scale;
            var prevScale = startScale;
            float lerpScale = 0;
            
            while (lerpScale < 1)
            {
                var scaleT = Misc.UpdateLerpPos(ref lerpScale, animParams.ScaleDuration, animParams.Unscaled, animParams.ScaleEasing);
                var newScale = Vector3.LerpUnclamped(startScale, targetScale, scaleT);
                var scaleDelta = newScale - prevScale;
                transform.localScale += scaleDelta;
                prevScale = newScale;
                yield return null;
            }
            
            _animateScaleRoutine = null;
        }

        public void StopAnimation()
        {
            StopRoutine(ref _animatePositionRoutine);
            StopRoutine(ref _animateRotationRoutine);
            StopRoutine(ref _animateScaleRoutine);
        }
        #endregion
    }
}