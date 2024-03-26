using UnityEditor;
using UnityEngine;

namespace NnUtils.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            base.OnGUI(position, property, label);
            GUI.enabled = true;
        }
    }
}