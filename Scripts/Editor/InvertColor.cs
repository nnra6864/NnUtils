using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.NnUtils.Scripts.Editor
{
    public class InvertColor : UnityEditor.Editor
    {
        [MenuItem("Assets/Invert Color", false, 102)]
        private static void InvertAllChannels()
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
                
                // Create a new texture with the same format
                var invertedTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
                
                // Invert all channels
                for (var y = 0; y < sourceTexture.height; y++)
                {
                    for (var x = 0; x < sourceTexture.width; x++)
                    {
                        var pixel = sourceTexture.GetPixel(x, y);
                        var invertedPixel = new Color(
                            1.0f - pixel.r,  // Invert R
                            1.0f - pixel.g,  // Invert G
                            1.0f - pixel.b,  // Invert B
                            1.0f - pixel.a   // Invert A
                        );
                        invertedTexture.SetPixel(x, y, invertedPixel);
                    }
                }
                invertedTexture.Apply();
                
                // Generate the new filename
                var directory = Path.GetDirectoryName(assetPath);
                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                var extension = Path.GetExtension(assetPath);
                var newFileName = fileName + "Inverted";
                var newPath = Path.Combine(directory, newFileName + extension);
                
                // Make sure we don't overwrite existing files
                var counter = 1;
                while (File.Exists(newPath))
                {
                    newPath = Path.Combine(directory, newFileName + "_" + counter + extension);
                    counter++;
                }
                
                // Save the new texture
                var bytes = invertedTexture.EncodeToPNG();
                File.WriteAllBytes(newPath, bytes);
                AssetDatabase.Refresh();
                
                // Clean up
                DestroyImmediate(invertedTexture);
                
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
                
                Debug.Log("Inverted color of " + assetPath + " to " + newPath);
            }
        }
    
        [MenuItem("Assets/Invert Color", true)]
        private static bool ValidateInvertAllChannels()
        {
            return Selection.activeObject is Texture2D;
        }
    }
}