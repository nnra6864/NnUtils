using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Assets.NnUtils.Scripts.Editor
{
    public class ARMtoMAS : UnityEditor.Editor
    {
        [MenuItem("Assets/ARM to MAS", false, 20)]
        private static void ConvertARMtoMAS()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is not Texture2D sourceTexture) continue;
                var assetPath = AssetDatabase.GetAssetPath(sourceTexture);
                
                // Read the source texture
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer == null) continue;
                
                // Store original import settings
                var originalIsReadable = importer.isReadable;
                var originalCompression = importer.textureCompression;
                var originalSRGB = importer.sRGBTexture;
                
                // Temporarily change settings to read the texture
                importer.isReadable         = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.sRGBTexture        = false;
                
                AssetDatabase.ImportAsset(assetPath);
                
                // Read the source texture after changing import settings
                sourceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                
                // Create a new texture for the MAS format
                var masTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
                
                // Convert ARM to MAS
                for (var y = 0; y < sourceTexture.height; y++)
                {
                    for (var x = 0; x < sourceTexture.width; x++)
                    {
                        var pixel = sourceTexture.GetPixel(x, y);
                        var masPixel = new Color(
                            pixel.b,       // R = Metal (from B in ARM)
                            pixel.r,       // G = AO (from R in ARM)
                            0,             // B = unused
                            1.0f - pixel.g // A = Smoothness (inverted from G/Roughness in ARM)
                        );
                        masTexture.SetPixel(x, y, masPixel);
                    }
                }
                masTexture.Apply();
                
                // Generate the new filename
                var directory = Path.GetDirectoryName(assetPath);
                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                var extension = Path.GetExtension(assetPath);
                
                // Replace ARM with MAS (case-insensitive)
                var newFileName = Regex.Replace(fileName, "arm", "MAS", RegexOptions.IgnoreCase);
                
                // If ARM wasn't found, append MAS
                if (newFileName == fileName)
                {
                    newFileName += "_MAS";
                }
                
                var newPath = Path.Combine(directory, newFileName + extension);
                
                // Make sure we don't overwrite existing files
                var counter = 1;
                while (File.Exists(newPath))
                {
                    newPath = Path.Combine(directory, newFileName + "_" + counter + extension);
                    counter++;
                }
                
                // Save the new texture
                var bytes = masTexture.EncodeToPNG();
                File.WriteAllBytes(newPath, bytes);
                AssetDatabase.Refresh();
                
                // Clean up
                DestroyImmediate(masTexture);
                
                // Restore original import settings
                importer.isReadable         = originalIsReadable;
                importer.textureCompression = originalCompression;
                importer.sRGBTexture        = originalSRGB;
                AssetDatabase.ImportAsset(assetPath);
                
                // Set proper import settings for the new texture
                var newImporter = AssetImporter.GetAtPath(newPath) as TextureImporter;
                if (newImporter != null)
                {
                    newImporter.isReadable  = false;
                    newImporter.sRGBTexture = false;
                    AssetDatabase.ImportAsset(newPath);
                }
                
                Debug.Log("Converted " + assetPath + " to " + newPath);
            }
        }
    
        [MenuItem("Assets/ARM to MAS", true)]
        private static bool ValidateConvertARMtoMAS()
        {
            return Selection.activeObject is Texture2D;
        }
    }
}