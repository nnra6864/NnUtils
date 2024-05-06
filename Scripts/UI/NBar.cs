using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NnUtils.Scripts.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class NBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectMask2D _mask;
        [SerializeField] private TMP_Text _valueText;
        [HideInInspector] [SerializeField] private string _textFormat = "value/max";
        public string TextFormat
        {
            get => _textFormat;
            set
            {
                _textFormat = value;
                Value = Value;
            }
        }

        [HideInInspector] [SerializeField] private float _min, _max = 1;
        public float Min
        {
            get => _min;
            set
            {
                if (Mathf.Approximately(_min, value)) return;
                var targetValue = Misc.Remap(Value, _min, _max, value, _max);
                _min = value;
                Value = targetValue;
            }
        }
        public float Max
        {
            get => _max;
            set
            {
                if (Mathf.Approximately(_max, value)) return;
                var targetValue = Misc.Remap(Value, _min, _max, _min, value);
                _max = value;
                Value = targetValue;
            }
        }

        [HideInInspector] [SerializeField] private float _value;
        public float Value
        {
            get => _value;
            set
            {
                var newText = _textFormat
                    .Replace("value", value.ToString(value % 1 == 0 ? "F0" : "F2"))
                        .Replace("max", _max.ToString());
                if (Mathf.Approximately(_value, value) && _valueText.text == newText) return;
                _value = value;
                if (_valueText != null) _valueText.text = newText;
                if (_mask == null) return;
                var pad = _mask.padding;
                pad.z = (1 - Misc.Remap(_value, Min, Max, 0, 1)) * _rectTransform.rect.width;
                _mask.padding = pad;
#if UNITY_EDITOR
                EditorUtility.SetDirty(_mask);
#endif
            }
        }
        
        private void Reset()
        {
            _rectTransform = GetComponent<RectTransform>();
            _min = 0;
            _max = 1;
            _mask = GetComponentInChildren<RectMask2D>();
            Value = 0.5f;
            _valueText = GetComponentInChildren<TMP_Text>();
        }
    }
}