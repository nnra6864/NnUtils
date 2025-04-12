using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.NnUtils.Scripts.Editor
{
    public class TextureRemapper : EditorWindow
    {
        private List<TextureMapping> _textureMappings = new();
        private string _outputName = "RemappedTexture";
        private Vector2 _scrollPosition;

        [System.Serializable]
        public class ChannelMapping
        {
            public int InputChannel; // 0=R, 1=G, 2=B, 3=A
            public bool Invert;
            public int OutputChannel; // 0=R, 1=G, 2=B, 3=A
        }

        [System.Serializable]
        public class TextureMapping
        {
            public Texture2D Texture;
            public List<ChannelMapping> ChannelMappings = new();

            public TextureMapping()
            {
                ChannelMappings.Add(new());
            }
        }

        [MenuItem("NnUtils/Texture Remapper")]
        public static void ShowWindow()
        {
            GetWindow<TextureRemapper>("Texture Remapper");
        }

        private void OnEnable()
        {
            if (_textureMappings.Count == 0) _textureMappings.Add(new());
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Texture Remapper", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            for (int i = 0; i < _textureMappings.Count; i++)
            {
                // Texture Input
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _textureMappings[i].Texture = (Texture2D)EditorGUILayout.ObjectField("",
                    _textureMappings[i].Texture, typeof(Texture2D), false, GUILayout.Width(65));
            
                EditorGUILayout.LabelField("Channel Mappings", EditorStyles.boldLabel);

                for (int j = 0; j < _textureMappings[i].ChannelMappings.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    string[] channels = { "R", "G", "B", "A" };

                    // Input Mapping
                    EditorGUILayout.LabelField("From", GUILayout.Width(35));
                    _textureMappings[i].ChannelMappings[j].InputChannel = EditorGUILayout.Popup(
                        _textureMappings[i].ChannelMappings[j].InputChannel, channels,
                        GUILayout.Width(30));
                    
                    // Output Mapping
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("To", GUILayout.Width(20));
                    _textureMappings[i].ChannelMappings[j].OutputChannel = EditorGUILayout.Popup(
                        _textureMappings[i].ChannelMappings[j].OutputChannel, channels,
                        GUILayout.Width(30));

                    // Invert Checkbox
                    GUILayout.Space(10);
                    _textureMappings[i].ChannelMappings[j].Invert = GUILayout.Toggle(
                        _textureMappings[i].ChannelMappings[j].Invert, "", GUILayout.Width(18));
                    EditorGUILayout.LabelField("Invert", GUILayout.Width(50));
                    
                    // Remove Mapping Button
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Remove Mapping", GUILayout.Width(120)))
                    {
                        _textureMappings[i].ChannelMappings.RemoveAt(j);
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                
                // Add Mapping Button
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Mapping", GUILayout.Width(120)))
                {
                    _textureMappings[i].ChannelMappings.Add(new());
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    break;
                }
                
                // Remove Texture Button
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove Texture", GUILayout.Width(120)))
                {
                    _textureMappings.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    break;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
                
            // Add Texture Button
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Texture", GUILayout.Width(120))) _textureMappings.Add(new());
            GUILayout.EndHorizontal();
        
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
        
            // Output Name
            EditorGUILayout.BeginHorizontal();
            _outputName = EditorGUILayout.TextField("Output Name", _outputName);
            
            // Create Button
            GUI.enabled = ValidateInputs();
            if (GUILayout.Button("Create Remapped Texture")) CreateRemappedTexture();
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private bool ValidateInputs() =>
            !string.IsNullOrEmpty(_outputName) &&
            _textureMappings.All(mapping => mapping.Texture &&
                                            mapping.ChannelMappings.Count != 0);

        private void CreateRemappedTexture()
        {
            // Get size from the first texture
            if (_textureMappings.Count == 0 || !_textureMappings[0].Texture)
            {
                EditorUtility.DisplayDialog("Error", "No textures selected!", "OK");
                return;
            }

            // Make all textures readable
            Dictionary<Texture2D, TextureSettings> originalSettings = new();
            foreach (var mapping in _textureMappings.Where(mapping => mapping.Texture))
                originalSettings[mapping.Texture] = MakeTextureReadable(mapping.Texture);

            // Find the largest dimensions
            int width = 0;
            int height = 0;
            foreach (var mapping in _textureMappings.Where(mapping => mapping.Texture))
            {
                width  = Mathf.Max(width, mapping.Texture.width);
                height = Mathf.Max(height, mapping.Texture.height);
            }

            // Create output texture
            Texture2D outputTexture = new(width, height, TextureFormat.RGBA32, false);
        
            // Fill with default values (black with full alpha)
            Color defaultColor = new(0, 0, 0, 1);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    outputTexture.SetPixel(x, y, defaultColor);

            // Apply channel mappings
            foreach (var mapping in _textureMappings)
            {
                if (!mapping.Texture) continue;

                foreach (var channelMapping in mapping.ChannelMappings)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Calculate normalized coordinates
                            var u = x / (float)(width - 1);
                            var v = y / (float)(height - 1);
                        
                            // Get pixel coordinates on the source texture
                            var srcX = Mathf.FloorToInt(u * (mapping.Texture.width - 1));
                            var srcY = Mathf.FloorToInt(v * (mapping.Texture.height - 1));
                        
                            // Get the source color
                            var srcColor = mapping.Texture.GetPixel(srcX, srcY);
                        
                            // Get the current color from the output texture
                            var outColor = outputTexture.GetPixel(x, y);
                        
                            // Get the channel value
                            var value = channelMapping.InputChannel switch
                            {
                                0 => srcColor.r,
                                1 => srcColor.g,
                                2 => srcColor.b,
                                3 => srcColor.a,
                                _ => 0
                            };

                            // Apply inversion if needed
                            if (channelMapping.Invert) value = 1.0f - value;
                        
                            // Set the channel in the output color
                            switch (channelMapping.OutputChannel)
                            {
                                case 0: outColor.r = value; break;
                                case 1: outColor.g = value; break;
                                case 2: outColor.b = value; break;
                                case 3: outColor.a = value; break;
                            }
                        
                            // Apply the output color
                            outputTexture.SetPixel(x, y, outColor);
                        }
                    }
                }
            }
        
            outputTexture.Apply();

            // Save the texture
            var path = AssetDatabase.GetAssetPath(_textureMappings[0].Texture);
            var directory = Path.GetDirectoryName(path);
            var newPath = Path.Combine(directory, $"{_outputName}.png");
        
            // Avoid overwriting
            int counter = 1;
            while (File.Exists(newPath))
            {
                newPath = Path.Combine(directory, $"{_outputName}_{counter}.png");
                counter++;
            }
        
            // Write to file
            var bytes = outputTexture.EncodeToPNG();
            File.WriteAllBytes(newPath, bytes);
            AssetDatabase.Refresh();
        
            // Restore original texture settings
            foreach (var kvp in originalSettings)
            {
                RestoreTextureSettings(kvp.Value);
            }
        
            // Set proper import settings on the new texture
            var newImporter = AssetImporter.GetAtPath(newPath) as TextureImporter;
            if (newImporter != null)
            {
                newImporter.isReadable  = false;
                newImporter.sRGBTexture = false;
                AssetDatabase.ImportAsset(newPath);
            }
        
            // Clean up
            DestroyImmediate(outputTexture);
        
            // Select the newly created asset
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2D>(newPath);
        }

        private class TextureSettings
        {
            public bool IsReadable;
            public TextureImporterCompression Compression;
            public bool SRGB;
            public string Path;
        }

        private TextureSettings MakeTextureReadable(Texture2D texture)
        {
            var path = AssetDatabase.GetAssetPath(texture);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        
            var settings = new TextureSettings
            {
                IsReadable  = importer.isReadable,
                Compression = importer.textureCompression,
                SRGB        = importer.sRGBTexture,
                Path        = path
            };
        
            // Make texture readable
            if (!importer.isReadable)
            {
                importer.isReadable         = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.sRGBTexture        = false;
                AssetDatabase.ImportAsset(path);
            }
        
            return settings;
        }

        private void RestoreTextureSettings(TextureSettings settings)
        {
            var importer = AssetImporter.GetAtPath(settings.Path) as TextureImporter;
            if (!importer) return;
            
            importer.isReadable         = settings.IsReadable;
            importer.textureCompression = settings.Compression;
            importer.sRGBTexture        = settings.SRGB;
            AssetDatabase.ImportAsset(settings.Path);
        }
    }
}