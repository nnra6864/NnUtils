using UnityEngine;

namespace NnUtils.Scripts.UI
{
    public class RadioNuttonManager : MonoBehaviour
    {
        [SerializeField] private RadioNutton _selected;
        private void Awake() => _selected.SelectNuttonWithoutEvent();
        public void SelectRadioNutton(RadioNutton nutton)
        {
            if (_selected == nutton) return;
            if (_selected != null) _selected.DeselectNutton();
            _selected = nutton;
            _selected.SelectNutton();
        }
    }
}
