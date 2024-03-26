using UnityEngine;

namespace NnUtils.Scripts
{
    public class Nbug : MonoBehaviour
    {
        public enum Levels
        {
            Info,
            Warning,
            Error
        }
        
        #region Log
        public static void Log(params object[] input)
        {
#if UNITY_EDITOR
            Debug.Log(string.Join("", input));
#endif
        }

        public static void Log(Levels level = Levels.Info, params object[] input)
        {
#if UNITY_EDITOR
            Log(level, string.Join("", input));
#endif
        }

        public static void Log(string separator = "", Levels level = Levels.Info, params object[] input)
        {
#if UNITY_EDITOR
            Log(level, string.Join(separator, input));
#endif
        }

        private static void Log(Levels level, string input)
        {
#if UNITY_EDITOR
            switch (level)
            {
                case Levels.Info: Debug.Log(input); return;
                case Levels.Warning: Debug.LogWarning(input); return;
                case Levels.Error: Debug.LogError(input); return;
            }
#endif
        }
        #endregion
        #region DrawRay
        public static void DrawRay(Vector3 start, Vector3 direction, float duration = 0, bool depthTest = true)
        {
#if UNITY_EDITOR
            Debug.DrawRay(start, direction, UnityEngine.Color.white, duration, depthTest);
#endif
        }

        public static void DrawRay(Vector3 start, Vector3 direction, UnityEngine.Color color, float duration = 0, bool depthTest = true)
        {
#if UNITY_EDITOR
            Debug.DrawRay(start, direction, color, duration, depthTest);
#endif
        }
        #endregion
        #region DrawLine
        public static void DrawLine(Vector3 start, Vector3 end, float duration = 0, bool depthTest = true)
        {
#if UNITY_EDITOR
            Debug.DrawLine(start, end, UnityEngine.Color.white, duration, depthTest);
#endif
        }

        public static void DrawLine(Vector3 start, Vector3 end, UnityEngine.Color color, float duration = 0, bool depthTest = true)
        {
#if UNITY_EDITOR
            Debug.DrawLine(start, end, color, duration, depthTest);
#endif
        }
        #endregion
    }
}