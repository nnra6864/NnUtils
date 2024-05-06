using System.Globalization;
using UnityEngine;

namespace NnUtils.Scripts
{
    public class Color : MonoBehaviour
    {
        public static Color32 HexToRgba(string hex, Color32 currentColor)
        {
            if (hex.Length < 1) return currentColor;
            int i = hex[0] == '#' ? 1 : 0;
            int r = currentColor.r, g = currentColor.g, b = currentColor.b, a = currentColor.a;
            if (hex.Length >= 2 + i) if (!int.TryParse($"{hex[0 + i]}{hex[1 + i]}", NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)) { }
            if (hex.Length >= 4 + i) if (!int.TryParse($"{hex[2 + i]}{hex[3 + i]}", NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)) { }
            if (hex.Length >= 6 + i) if (!int.TryParse($"{hex[4 + i]}{hex[5 + i]}", NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b)) { }
            if (hex.Length >= 8 + i) if (!int.TryParse($"{hex[6 + i]}{hex[7 + i]}", NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a)) { }
            return new Color32(byte.Parse(r.ToString()), byte.Parse(g.ToString()), byte.Parse(b.ToString()), byte.Parse(a.ToString()));
        }

        public static string FormatColor32ToRGBA(Color32 col, string format = "r, g, b, a")
        {
            for (int i = 0; i < format.Length; i++)
            {
                if (format[i] == 'r') format = format.Replace("r", col.r.ToString());
                if (format[i] == 'g') format = format.Replace("g", col.g.ToString());
                if (format[i] == 'b') format = format.Replace("b", col.b.ToString());
                if (format[i] == 'a') format = format.Replace("a", col.a.ToString());
            }
            return format;
        }

        public static UnityEngine.Color LerpInHSL(UnityEngine.Color startCol, UnityEngine.Color endCol, float t)
        {
            UnityEngine.Color.RGBToHSV(startCol, out var startH, out var startS, out var startV);
            UnityEngine.Color.RGBToHSV(endCol, out var endH, out var endS, out var endV);

            var h = Mathf.Lerp(startH, endH, t);
            var s = Mathf.Lerp(startS, endS, t);
            var v = Mathf.Lerp(startV, endV, t);

            return UnityEngine.Color.HSVToRGB(h, s, v);
        }
    }
}