using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NnUtils.Scripts.UI
{
    public class RadioNutton : MonoBehaviour
    {
        [SerializeField] private Graphic _nuttonGraphic;
        [ColorUsage(showAlpha: true, hdr: true)]
        [SerializeField] private Color32
            _normalColor,
            _hoveredColor,
            _clickedColor,
            _selectedColor;

        [SerializeField] private UnityEvent OnClick;
        
        private bool _isSelected;
        
        public void HoverNutton()
        {
            if (_isSelected) return;
            _nuttonGraphic.color = _hoveredColor;
        }

        public void UnhoverNutton()
        {
            if (_isSelected) return;
            _nuttonGraphic.color = _normalColor;
        }

        public void PointerDown()
        {
            if (_isSelected) return;
            _nuttonGraphic.color = _clickedColor;
        }

        public void SelectNutton()
        {
            if (_isSelected) return;
            _nuttonGraphic.color = _selectedColor;
            OnClick?.Invoke();
            _isSelected = true;
        }

        public void SelectNuttonWithoutEvent()
        {
            if (_isSelected) return;
            _nuttonGraphic.color = _selectedColor;
            _isSelected = true;
        }
        
        public void DeselectNutton()
        {
            _nuttonGraphic.color = _normalColor;
            _isSelected = false;
        }
    }
}
