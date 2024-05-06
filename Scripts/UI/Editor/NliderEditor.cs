using UnityEditor;

namespace NnUtils.Scripts.UI.Editor
{
    [CustomEditor(typeof(NBar))]
    public class NliderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space(10);
            var nlider = (NBar)target;
            EditorGUILayout.LabelField("Values", EditorStyles.boldLabel);
            var range = EditorGUILayout.Vector2Field("Range", new(nlider.Min, nlider.Max));
            nlider.Min = range.x;
            nlider.Max = range.y;
            nlider.Value = EditorGUILayout.Slider("Value", nlider.Value, nlider.Min, nlider.Max);
            nlider.TextFormat = EditorGUILayout.TextField("Format", nlider.TextFormat);
        }
    }
}