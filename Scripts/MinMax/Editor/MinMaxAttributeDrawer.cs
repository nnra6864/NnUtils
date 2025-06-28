// https://frarees.github.io/default-gist-license

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.NnUtils.Scripts.MinMax.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    internal class MinMaxDrawer : PropertyDrawer
    {
        private const string KVectorMinName = "x";
        private const string KVectorMaxName = "y";
        private const float KFloatFieldWidth = 16f;
        private const float KSpacing = 2f;
        private const float KRoundingValue = 100f;

        private static readonly int ControlHash = "Foldout".GetHashCode();
        private static readonly GUIContent Unsupported =
            EditorGUIUtility.TrTextContent("Unsupported field type");

        private bool pressed;
        private float pressedMin;
        private float pressedMax;

        private float Round(float value, float roundingValue) =>
            roundingValue == 0 ? value : Mathf.Round(value * roundingValue) / roundingValue;

        private float FlexibleFloatFieldWidth(float min, float max, bool hasDecimals)
        {
            var n = Mathf.Max(Mathf.Abs(min), Mathf.Abs(max));
            float floatFieldWidth = 14f + (Mathf.Floor(Mathf.Log10(Mathf.Abs(n)) + 1) * 8f);
            if (hasDecimals)
            {
                floatFieldWidth += 18f;
            }
            return floatFieldWidth;
        }
        
        private void SetVectorValue(SerializedProperty property,
                                    ref float min, ref float max, bool round)
        {
            if (!pressed || (pressed && !Mathf.Approximately(min, pressedMin)))
            {
                using var x = property.FindPropertyRelative(KVectorMinName);
                SetValue(x, ref min, round);
            }

            if (!pressed || (pressed && !Mathf.Approximately(max, pressedMax)))
            {
                using var y = property.FindPropertyRelative(KVectorMaxName);
                SetValue(y, ref max, round);
            }
        }

        private void SetValue(SerializedProperty property, ref float v, bool round)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                {
                    if (round)
                    {
                        v = Round(v, KRoundingValue);
                    }

                    property.floatValue = v;
                }
                    break;
                case SerializedPropertyType.Integer:
                {
                    property.intValue = Mathf.RoundToInt(v);
                }
                    break;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float min, max;

            label = EditorGUI.BeginProperty(position, label, property);

            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                {
                    var v = property.vector2Value;
                    min = v.x;
                    max = v.y;
                }
                    break;
                case SerializedPropertyType.Vector2Int:
                {
                    var v = property.vector2IntValue;
                    min = v.x;
                    max = v.y;
                }
                    break;
                default:
                    EditorGUI.LabelField(position, label, Unsupported);
                    return;
            }

            var attr = attribute as MinMaxAttribute;

            var ppp = EditorGUIUtility.pixelsPerPoint;
            var spacing = KSpacing * ppp;
            var fieldWidth = ppp * (attr is { DataFields: true, FlexibleFields: true }
                                        ? FlexibleFloatFieldWidth(attr.Min, attr.Max,
                                                                  property.propertyType ==
                                                                  SerializedPropertyType.Vector2)
                                        : KFloatFieldWidth);

            var indent = EditorGUI.indentLevel;

            var id = GUIUtility.GetControlID(ControlHash, FocusType.Keyboard, position);
            var r = EditorGUI.PrefixLabel(position, id, label);

            var sliderPos = r;

            if (attr is { DataFields: true })
            {
                sliderPos.x += fieldWidth + spacing;
                sliderPos.width -= (fieldWidth + spacing) * 2;
            }

            if (Event.current.type == EventType.MouseDown &&
                sliderPos.Contains(Event.current.mousePosition))
            {
                pressed = true;
                if (attr != null)
                {
                    min = Mathf.Clamp(min, attr.Min, attr.Max);
                    max = Mathf.Clamp(max, attr.Min, attr.Max);
                    pressedMin = min;
                    pressedMax = max;
                    SetVectorValue(property, ref min, ref max, attr.Round);
                }

                GUIUtility.keyboardControl = 0; // TODO keep focus but stop editing
            }

            if (pressed && Event.current.type == EventType.MouseUp)
            {
                if (attr is { Round: true }) SetVectorValue(property, ref min, ref max, true);
                pressed = false;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel = 0;
            if (attr != null)
            {
                EditorGUI.MinMaxSlider(sliderPos, ref min, ref max, attr.Min, attr.Max);
                EditorGUI.indentLevel = indent;
                if (EditorGUI.EndChangeCheck()) SetVectorValue(property, ref min, ref max, false);

                if (attr.DataFields)
                {
                    var minPos = r;
                    minPos.width = fieldWidth;

                    var vectorMinProp = property.FindPropertyRelative(KVectorMinName);
                    EditorGUI.showMixedValue = vectorMinProp.hasMultipleDifferentValues;
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.indentLevel = 0;
                    min = EditorGUI.DelayedFloatField(minPos, min);
                    EditorGUI.indentLevel = indent;
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (attr.Bound)
                        {
                            min = Mathf.Max(min, attr.Min);
                            min = Mathf.Min(min, max);
                        }

                        SetVectorValue(property, ref min, ref max, attr.Round);
                    }

                    vectorMinProp.Dispose();

                    var maxPos = position;
                    maxPos.x += maxPos.width - fieldWidth;
                    maxPos.width = fieldWidth;

                    var vectorMaxProp = property.FindPropertyRelative(KVectorMaxName);
                    EditorGUI.showMixedValue = vectorMaxProp.hasMultipleDifferentValues;
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.indentLevel = 0;
                    max = EditorGUI.DelayedFloatField(maxPos, max);
                    EditorGUI.indentLevel = indent;
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (attr.Bound)
                        {
                            max = Mathf.Min(max, attr.Max);
                            max = Mathf.Max(max, min);
                        }

                        SetVectorValue(property, ref min, ref max, attr.Round);
                    }

                    vectorMaxProp.Dispose();

                    EditorGUI.showMixedValue = false;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}

#endif