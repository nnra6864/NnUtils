using UnityEditor;
using UnityEngine;

namespace NnUtils.Scripts.UI.Editor
{
    [CustomEditor(typeof(InteractiveImageScript))]
    public class InteractiveImageScriptEditor : UnityEditor.Editor
    {
        private SerializedProperty _imageProperty;
        private SerializedProperty _defaultSpriteProperty;
        private SerializedProperty _colorProperty;
        private SerializedProperty _materialProperty;
        private SerializedProperty _raycastTargetProperty;
        private SerializedProperty _raycastPaddingProperty;
        private SerializedProperty _maskableProperty;
        private SerializedProperty _scalingModeProperty;

        private void OnEnable()
        {
            _imageProperty = serializedObject.FindProperty("Image");
            _defaultSpriteProperty = serializedObject.FindProperty("DefaultSprite");
            _colorProperty = serializedObject.FindProperty("m_Color");
            _materialProperty = serializedObject.FindProperty("m_Material");
            _raycastTargetProperty = serializedObject.FindProperty("m_RaycastTarget");
            _raycastPaddingProperty = serializedObject.FindProperty("m_RaycastPadding");
            _maskableProperty = serializedObject.FindProperty("m_Maskable");
            _scalingModeProperty = serializedObject.FindProperty("ScalingMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_imageProperty, new GUIContent("Image", "Name from ProjectImageManager, local path or URL"));
            EditorGUILayout.PropertyField(_defaultSpriteProperty, new GUIContent("Default Sprite", "Fallback sprite if image is not found"));
            EditorGUILayout.PropertyField(_colorProperty, new GUIContent("Color", "Color applied to the image"));
            EditorGUILayout.PropertyField(_materialProperty, new GUIContent("Material", "Material applied to the image"));
            EditorGUILayout.PropertyField(_raycastTargetProperty, new GUIContent("Raycast", "Whether raycast will detect the image"));
            EditorGUILayout.PropertyField(_raycastPaddingProperty, new GUIContent("Raycast Padding", "Padding for the raycast"));
            EditorGUILayout.PropertyField(_maskableProperty, new GUIContent("Maskable", "Whether masks affect the image"));
            EditorGUILayout.PropertyField(_scalingModeProperty, new GUIContent("Scaling Mode", "Scaling mode applied to the image"));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}