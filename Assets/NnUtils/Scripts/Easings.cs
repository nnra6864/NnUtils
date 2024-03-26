using UnityEngine;

namespace Assets.NnUtils.Scripts
{
    public class Easings
    {
        public enum Types
        {
            None,
            SineIn, SineOut, SineInOut, SineOutIn,
            QuadIn, QuadOut, QuadInOut,
            QuartIn, QuartOut, QuartInOut,
            CubicIn, CubicOut, CubicInOut, CubicOutIn,
            ExpoIn, ExpoOut, ExpoInOut
        }
        public static float Ease(float t, Types easing)
        {
            switch (easing)
            {
                case Types.SineIn: return SineIn(t);
                case Types.SineOut: return SineOut(t);
                case Types.SineInOut: return SineInOut(t);
                case Types.SineOutIn: return SineOutIn(t);
                case Types.QuadIn: return QuadIn(t);
                case Types.QuadOut: return QuadOut(t);
                case Types.QuadInOut: return QuadInOut(t);
                case Types.QuartIn: return QuartIn(t);
                case Types.QuartOut: return QuartOut(t);
                case Types.QuartInOut: return QuartInOut(t);
                case Types.CubicIn: return CubicIn(t);
                case Types.CubicOut: return CubicOut(t);
                case Types.CubicInOut: return CubicInOut(t);
                case Types.CubicOutIn: return CubicOutIn(t);
                case Types.ExpoIn: return ExpoIn(t);
                case Types.ExpoOut: return ExpoOut(t);
                case Types.ExpoInOut: return ExpoInOut(t);
                default: return t;
            }
        }
        #region Sine
        public static float SineIn(float t) => 1 - Mathf.Cos(t * Mathf.PI / 2f);
        public static float SineOut(float t) => Mathf.Sin(t * Mathf.PI / 2f);
        public static float SineInOut(float t) => -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
        public static float SineOutIn(float t) => t < 0.5f ? SineOut(2 * t) / 2 : 0.5f + SineIn(2 * t - 1) / 2;
        public static float SineInBack(float t)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return c3 * t * t * t - c1 * t * t;
        }

        public static float EaseInOutBack(float t)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            return t < 0.5f
                ? Mathf.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2) / 2f
                : (Mathf.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2f;
        }
        #endregion
        #region Quad
        public static float QuadIn(float t) => t * t;
        public static float QuadOut(float t) => 1 - (1 - t) * (1 - t);
        public static float QuadInOut(float t) => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        #endregion
        #region Quart
        public static float QuartIn(float t) => t * t * t * t;
        public static float QuartOut(float t) => 1 - Mathf.Pow(1 - t, 4);
        public static float QuartInOut(float t) => t < 0.5f ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;
        #endregion
        #region Cubic
        public static float CubicIn(float t) => Mathf.Pow(t, 3f);
        public static float CubicOut(float t) => 1f - Mathf.Pow(1f - t, 3f);
        public static float CubicInOut(float t) => t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        public static float CubicOutIn(float t) => t < 0.5f ? CubicOut(t * 2) / 2 : 0.5f + CubicIn(t * 2 - 1) / 2;
        #endregion
        #region Quint
        public static float QuintOut(float t) => 1 - Mathf.Pow(1 - t, 5);
        public static float QuintInOut(float t) => t < 0.5f ? 16 * Mathf.Pow(t, 5) : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
        public static float CubicInQuintOut(float t) => t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        #endregion
        #region Expo
        public static float ExpoIn(float t) => t == 0 ? 0 : Mathf.Pow(2, 10 * t - 10);
        public static float ExpoOut(float t) => t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
        public static float ExpoInOut(float t) => t == 0 ? 0 : t == 1 ? 1 : t < 0.5f ? Mathf.Pow(2, 20 * t - 10) / 2 : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
        #endregion
        #region Circ
        public static float CircIn(float t) => 1f - Mathf.Sqrt(1f - t * t);
        public static float CircOut(float t) => Mathf.Sqrt(1f - Mathf.Pow(1f - t, 2f));
        public static float CircInOut(float t) => t < 0.5f ? (1f - Mathf.Sqrt(1f - 4f * t * t)) / 2f : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) / 2f;
        public static float CircOutIn(float t) => t < 0.5f ? CircOut(2f * t) / 2f : (CircIn(2f * t - 1f) + 1f) / 2f;
        #endregion
    }
}