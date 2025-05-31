using UnityEngine;

namespace NnUtils.Scripts
{
    public static class EasingsObsolete
    {
        public enum Type
        {
            None,
            SineIn, SineOut, SineInOut, SineOutIn,
            QuadIn, QuadOut, QuadInOut,
            CubicIn, CubicOut, CubicInOut, CubicOutIn,
            QuartIn, QuartOut, QuartInOut,
            ExpoIn, ExpoOut, ExpoInOut
        }
        
        public static float Ease(float t, Type easing) =>
            easing switch
            {
                Type.SineIn => SineIn(t),
                Type.SineOut => SineOut(t),
                Type.SineInOut => SineInOut(t),
                Type.SineOutIn => SineOutIn(t),
                Type.QuadIn => QuadIn(t),
                Type.QuadOut => QuadOut(t),
                Type.QuadInOut => QuadInOut(t),
                Type.CubicIn => CubicIn(t),
                Type.CubicOut => CubicOut(t),
                Type.CubicInOut => CubicInOut(t),
                Type.CubicOutIn => CubicOutIn(t),
                Type.QuartIn => QuartIn(t),
                Type.QuartOut => QuartOut(t),
                Type.QuartInOut => QuartInOut(t),
                Type.ExpoIn => ExpoIn(t),
                Type.ExpoOut => ExpoOut(t),
                Type.ExpoInOut => ExpoInOut(t),
                _ => t
            };

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