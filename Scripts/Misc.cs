using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace NnUtils.Scripts
{
    public static class Misc
    {
        public static float LimitVelocity(float speed, float currentVelocity, float maxSpeed) =>
            speed + currentVelocity > maxSpeed ? Mathf.Clamp(maxSpeed - currentVelocity, 0, maxSpeed) :
            speed + currentVelocity < -maxSpeed ? Mathf.Clamp(-maxSpeed - currentVelocity, -maxSpeed, 0) : speed;

        public static Vector2 CapVelocityDelta(Vector2 currentVelocity, Vector2 deltaVelocity, float maxSpeed)
        {
            var newVel = currentVelocity + deltaVelocity;
    
            if (newVel.magnitude > maxSpeed && newVel.magnitude > currentVelocity.magnitude)
            {
                var directionOfChange = (newVel - currentVelocity).normalized;
                var maxChange = Mathf.Max(maxSpeed - currentVelocity.magnitude, 0);
                return directionOfChange * maxChange;
            }

            return deltaVelocity;
        }
        
        #region Is Pointer Over UI
        public static bool IsPointerOverUI => IsPointerOverUIElement(GetEventSystemRaycastResults());
        private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResults)
        {
            foreach (var curRaycastResult in eventSystemRaycastResults)
                if (curRaycastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                    return true;
            return false;
        }
        private static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new(EventSystem.current)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> raycastResults = new();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults;
        }
        #endregion

        public static Vector2 CapV2WithingRects(Vector2 vector, float min, float max) =>
            IsValueInRange(vector.x, min, max) && IsValueInRange(vector.y, min, max)
            ? vector
            : !IsValueInRange(vector.x, min, max)
                ? new Vector2(Mathf.Clamp(vector.x, min, max), vector.y)
                : new Vector2(vector.x, Mathf.Clamp(vector.y, min, max));

        public static bool IsValueInRange(float value, float min, float max) => value >= min && value <= max;

        public static float Remap(float value, float min, float max, float newMin, float newMax) =>
            Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(min, max, value));

        /// <summary>
        /// Updates the lerp position and returns the eased value
        /// </summary>
        /// <param name="lerpPos"></param>
        /// <param name="lerpTime"></param>
        /// <param name="unscaled"></param>
        /// <param name="easingType"></param>
        /// <returns></returns>
        public static float UpdateLerpPos(ref float lerpPos, float lerpTime = 1, bool unscaled = false, Easings.Types easingType = Easings.Types.None)
        {
            if (lerpTime == 0) lerpPos = 1;
            else lerpPos = Mathf.Clamp01(lerpPos += (unscaled ? Time.unscaledDeltaTime : Time.deltaTime) / lerpTime);
            return Easings.Ease(lerpPos, easingType);
        }

        /// <summary>
        /// Reverses the lerp position and returns the eased value
        /// </summary>
        /// <param name="lerpPos"></param>
        /// <param name="lerpTime"></param>
        /// <param name="unscaled"></param>
        /// <param name="easingType"></param>
        /// <param name="invertEasing"></param>
        /// <returns></returns>
        public static float ReverseLerpPos(ref float lerpPos, float lerpTime = 1, bool unscaled = false, Easings.Types easingType = Easings.Types.None, bool invertEasing = true)
        {
            if (lerpTime == 0) lerpPos = 0;
            else lerpPos = Mathf.Clamp01(lerpPos -= (unscaled ? Time.unscaledDeltaTime : Time.deltaTime) / lerpTime);
            var t = Easings.Ease(invertEasing ? 1 - lerpPos : lerpPos, easingType);
            return invertEasing ? 1 - t : t;
        }
        
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null) component = gameObject.AddComponent<T>();
            return component;
        }

        public static void StartRoutineIf(this MonoBehaviour sender, ref Coroutine target, IEnumerator routine, Func<bool> startIf)
        {
            if (!startIf()) return;
            target = sender.StartCoroutine(routine);
        }

        public static void StartNullRoutine(this MonoBehaviour sender, ref Coroutine target, IEnumerator routine)
        {
            if (target != null) return;
            target = sender.StartCoroutine(routine);
        }
        
        public static void RestartRoutine(this MonoBehaviour sender, ref Coroutine target, IEnumerator routine)
        {
            if (target != null) sender.StopCoroutine(target);
            target = sender.StartCoroutine(routine);
        }

        public static void StopRoutine(this MonoBehaviour sender, ref Coroutine target)
        {
            if (target != null) sender.StopCoroutine(target);
            target = null;
        }

        public static int RandomInvert => Random.Range(0, 2) == 0 ? 1 : -1;

        public static Vector2 AbsV2(Vector2 input) => new(Mathf.Abs(input.x), Mathf.Abs(input.y));
        public static Vector3 AbsV3(Vector3 input) => new(Mathf.Abs(input.x), Mathf.Abs(input.y), Mathf.Abs(input.z));

        /// <summary>
        /// Returns position of the pointer.
        /// <br/>
        /// If the screen is touched it will return the position of the touchIndex(0 by default).
        /// </summary>
        /// <param name="touchIndex"></param>
        /// <returns></returns>
        public static Vector2 GetPointerPos(int touchIndex = 0)
        {
            Vector2 pos = Input.mousePosition;
            if (Input.touchCount > touchIndex) pos = Input.GetTouch(touchIndex).position;
            return pos;
        }
        
        public static Quaternion VectorToQuaternion(Vector3 vec) => Quaternion.Euler(vec.x, vec.y, vec.z);

        public static float RadialSelection()
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 centerPos = new(Screen.width / 2, Screen.height / 2);
            var dir = mousePos - centerPos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            angle += angle < 0 ? 360 : 0;
            return 360 - angle;
        }
        
        public static float RadialSelection(Vector2 centerPos)
        {
            Vector2 mousePos = Input.mousePosition;
            var dir = mousePos - centerPos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            angle += angle < 0 ? 360 : 0;
            return 360 - angle;
        }
    }
}