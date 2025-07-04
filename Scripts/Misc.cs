using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NnUtils.Modules.Easings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
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
            var uiLayer = LayerMask.NameToLayer("UI");
            return eventSystemRaycastResults.Any(curRaycastResult => curRaycastResult.gameObject.layer == uiLayer);
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

        /// Gets an argument passed to the app
        public static string GetArg(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++) if (args[i] == name && args.Length > i + 1) return args[i + 1];
            return null;
        }
        
        public static Vector2 CapV2WithinRects(Vector2 vector, float min, float max) =>
            IsValueInRange(vector.x, min, max) && IsValueInRange(vector.y, min, max)
            ? vector
            : !IsValueInRange(vector.x, min, max)
                ? new(Mathf.Clamp(vector.x, min, max), vector.y)
                : new(vector.x, Mathf.Clamp(vector.y, min, max));

        public static bool IsValueInRange(float value, float min, float max) => value >= min && value <= max;

        public static float Remap(float value, float min, float max, float newMin, float newMax) =>
            Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(min, max, value));

        /// <summary>
        /// Updates the lerp position and returns the eased value
        /// </summary>
        /// <param name="lerpPos">Reference to the lerp pos</param>
        /// <param name="lerpTime">Lerp time in seconds</param>
        /// <param name="easingType">Applied easing</param>
        /// <param name="unscaled">Uses <see cref="Time.unscaledDeltaTime"/> if true</param>
        /// <returns>Eased lerp position</returns>
        public static float Tween(ref float lerpPos, float lerpTime = 1, Easings.Type easingType = Easings.Type.Linear, bool unscaled = false)
        {
            if (lerpTime == 0) lerpPos = 1;
            else lerpPos = Mathf.Clamp01(lerpPos += (unscaled ? Time.unscaledDeltaTime : Time.deltaTime) / lerpTime);
            return Easings.Ease(lerpPos, easingType);
        }

        /// <summary>
        /// Reverses the lerp position and returns the eased value
        /// </summary>
        /// <param name="lerpPos">Reference to the lerp pos</param>
        /// <param name="lerpTime">Lerp time in seconds</param>
        /// <param name="easingType">Applied easing</param>
        /// <param name="unscaled">Uses <see cref="Time.unscaledDeltaTime"/> if true</param>
        /// <param name="invertEasing">If ture, easing will be applied to (1 - lerpPos)</param>
        /// <returns>Eased lerp position</returns>
        public static float ReverseTween(ref float lerpPos, float lerpTime = 1, Easings.Type easingType = Easings.Type.Linear, bool invertEasing = true, bool unscaled = false)
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

        /// Returns -1 or 1
        public static int RandomInvert => Random.Range(0, 2) == 0 ? 1 : -1;
        
        /// Returns a value between -1 and 1
        public static float Random1 => Random.Range(-1, 1);
        
        /// Returns a value between 0 and 1
        public static float Random01 => Random.Range(0, 1);

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
            Vector2 centerPos = new(Screen.width / 2f, Screen.height / 2f);
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

        public static Texture2D TexFromFile(string path)
        {
            // Return if null
            if (string.IsNullOrEmpty(path)) return null;
            
            // Replace the relative with absolute path
            path = path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal));

            // If the image file doesn't exist, return null
            if (!File.Exists(path)) return null;

            // Read image data and store it into a Texture2D
            var data = File.ReadAllBytes(path);
            Texture2D tex = new(0, 0);
            
            return !tex.LoadImage(data) ? null : tex;
        }
        
        public static Sprite Texture2DToSprite(Texture2D texture) =>
            texture == null ? null : Sprite.Create(texture, new(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        public static Sprite SpriteFromFile(string path) => Texture2DToSprite(TexFromFile(path));
        
        /// <summary>
        /// Downloads a sprite from a URL and returns it through a callback when complete.
        /// <example>
        /// <code>
        /// var isDone = false;
        /// _webImageRoutine = StartCoroutine(Misc.SpriteFromURL(configImage.Image, s =>
        /// {
        ///     sprite = s;
        ///     isDone = true;
        /// }));
        /// yield return new WaitUntil(() => isDone);
        /// _webImageRoutine = null;
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="url">The URL of the image to download</param>
        /// <param name="onComplete">Callback that receives the downloaded sprite. Will be null if download fails.</param>
        public static IEnumerator SpriteFromURL(string url, Action<Sprite> onComplete)
        {
            // Return if no url is provided
            if (string.IsNullOrEmpty(url)) yield break;
            
            // Create a request and wait for it
            var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            // Create a sprite from the request
            Sprite downloadedSprite = null;
            if (request.result == UnityWebRequest.Result.Success)
            {
                var downloadedTexture = DownloadHandlerTexture.GetContent(request);
                downloadedSprite = Sprite.Create(downloadedTexture, new(0, 0, downloadedTexture.width, downloadedTexture.height), new(0.5f, 0.5f));
            }
        
            // Invoke the onComplete event
            onComplete?.Invoke(downloadedSprite);
        }

        public static void ExecuteNextFrame(this MonoBehaviour sender, Action action) =>
            sender.StartCoroutine(ExecuteNextFrameRoutine(action));

        private static IEnumerator ExecuteNextFrameRoutine(Action action)
        {
            yield return null;
            action?.Invoke();
        }
    }
}