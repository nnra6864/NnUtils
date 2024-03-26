using UnityEditor;
using UnityEngine;

namespace NnUtils.Scripts.UI.Editor
{
    [CustomPropertyDrawer(typeof(ChanceList<>))]
    public class ChanceListEditor : PropertyDrawer
    {
        private const int _itemsIndent = 5;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, "Chance List");

            position.x = _itemsIndent;
            
            var list = property.FindPropertyRelative("_list");
            for (int i = 0; i < list.arraySize; i++)
            {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var el = list.GetArrayElementAtIndex(i);
                var ca = el.FindPropertyRelative("_weight");
                var c = el.FindPropertyRelative("Chance");
                
                ca.intValue = EditorGUI.IntField(position, "Chance Amount", ca.intValue);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                el.serializedObject.ApplyModifiedProperties();
                
                EditorGUI.LabelField(position, "Chance", $"{c.floatValue}%");
            }
            
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            if (GUILayout.Button("Add"))
            {
                list.InsertArrayElementAtIndex(list.arraySize);
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 6 + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}