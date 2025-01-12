using TMPro;

namespace NnUtils.TextUtils.Scripts
{
    // TODO: Add Windows and possibly OSX compatibility
    public static class SystemFont
    {
        /// <summary>
        /// Generates a <see cref="TMP_FontAsset"/> from a font name
        /// </summary>
        /// <param name="fontName">Name or path to a font</param>
        /// <returns><see cref="TMP_FontAsset"/> generated from a font</returns>
        public static TMP_FontAsset GenerateFontFromName(string fontName) => GenerateFontFromPath(GetFontPath(fontName));

        /// <summary>
        /// Generates a <see cref="TMP_FontAsset"/> from a font path
        /// </summary>
        /// <param name="fontPath">Path to a font</param>
        /// <returns><see cref="TMP_FontAsset"/> generated from a font</returns>
        public static TMP_FontAsset GenerateFontFromPath(string fontPath) => TMP_FontAsset.CreateFontAsset(new(fontPath));

        /// <summary>
        /// Gets the name of a font located at <paramref name="fontPath"/>
        /// </summary>
        /// <param name="fontPath">Path to the font</param>
        /// <returns>Name of the font</returns>
        public static string GetFontName(string fontPath = "")
        {
            // Define the process to get the actual Font Name
            System.Diagnostics.Process process = new()
            {
                StartInfo = new()
                {
                    FileName               = "fc-query",
                    Arguments              = $"--format=\"%{{family}}\\n\" \"{fontPath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                }
            };
            
            // Get the actual font name
            process.Start();
            var fontName = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            // Return the font name
            return fontName;
        }

        /// <summary>
        /// Gets the path to the font by its name
        /// </summary>
        /// <param name="fontName">Name or path to a font<br/>Leave empty to get the system font</param>
        /// <returns>Path to the font</returns>
        public static string GetFontPath(string fontName = "")
        {
            // Check if the input is a valid file path and return it
            if (System.IO.File.Exists(fontName))
                return fontName;
            
            // Define the process to find a font by name
            System.Diagnostics.Process process = new()
            {
                StartInfo = new()
                {
                    FileName        = "fc-match",
                    Arguments       = $"-f \"%{{file}}\\n\" \"{fontName}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow  = true
                }
            };
            
            // Get the font path
            process.Start();
            var fontPath = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            // Return the font path
            return fontPath;
        }
    }
}