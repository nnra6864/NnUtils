using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace NnUtils.Scripts.UI.RadialMenu
{
    public class RadialMenuScript : NnBehaviour
    {
        [Header("Components")]
        [SerializeField] private InputActionAsset _nnActions;
        [SerializeField] private RectTransform _menu;
        [SerializeField] private RectTransform _itemsParent;
        [SerializeField] private TMP_Text _hoveredText;
        [SerializeField] private Image _hoveredImage;

        [Header("Values")]
        [Range(0, 1)] [SerializeField] private float _itemDistance = 0.5f;

        [Header("Animation")]
        [SerializeField] private bool _animateOnOpen = true;
        [SerializeField] private bool _animateOnClose = true;
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _startRotation;
        [SerializeField] private Vector3 _startScale;
        [SerializeField] private AnimationParams _openAnimationParams;
        [SerializeField] private AnimationParams _closeAnimationParams;
        
        [Header("Item Animation")]
        [SerializeField] private bool _overwriteOpenItemAnimation = true;
        [SerializeField] private bool _overwriteCloseItemAnimation = true;
        [SerializeField] private Vector3 _startItemRotation;
        [SerializeField] private Vector3 _startItemScale;
        [SerializeField] private AnimationParams _openItemAnimationParams;
        [SerializeField] private AnimationParams _closeItemAnimationParams;

        private InputAction _selectAction;
        private InputAction _backAction;
        private Stack<RadialMenuItem> _radialMenuItemsStack = new();
        private List<RadialMenuItemScript> _radialMenuItems = new();

        #region Properties
        private float _anglePerItem;
        private int _itemCount;
        public int ItemCount
        {
            get => _itemCount;
            private set
            {
                _itemCount = value;
                _anglePerItem = _itemCount == 0 ? 0 : 360f / _itemCount;
            }
        }

        private int _hoveredIndex = -1;
        public int HoveredIndex
        {
            get => _hoveredIndex;
            private set
            {
                if (_hoveredIndex == value) return;
                if (Misc.IsValueInRange(_hoveredIndex, 0, _radialMenuItems.Count - 1)) _radialMenuItems[_hoveredIndex]?.Leave();
                _hoveredIndex = value;
                OnHoveredIndexChanged?.Invoke(_hoveredIndex);
                if (Misc.IsValueInRange(_hoveredIndex, 0, _radialMenuItems.Count - 1)) _radialMenuItems[_hoveredIndex]?.Enter();
                if (Misc.IsValueInRange(_hoveredIndex, 0, _radialMenuItems.Count - 1)) Hovered = _radialMenuItems[_hoveredIndex];
            }
        }
        public Action<int> OnHoveredIndexChanged;
        
        private RadialMenuItemScript _hovered;
        public RadialMenuItemScript Hovered
        {
            get => _hovered;
            private set
            {
                _hovered = value;
                _hoveredText.text = _hovered.MenuItem.Name;
                _hoveredImage.sprite = _hovered.MenuItem.Sprite;
                OnHoveredChanged?.Invoke(_hovered);
            }
        }
        public Action<RadialMenuItemScript> OnHoveredChanged;

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get => _selectedIndex;
            private set
            {
                if (_selectedIndex == value) return;
                if (Misc.IsValueInRange(_selectedIndex, 0, _radialMenuItems.Count - 1)) _selectedIndex = value;
                _selectedIndex = value;
                OnSelectedIndexChanged?.Invoke(_selectedIndex);
                if (Misc.IsValueInRange(_selectedIndex, 0, _radialMenuItems.Count - 1)) _radialMenuItems[_selectedIndex]?.Down();
                if (Misc.IsValueInRange(_selectedIndex, 0, _radialMenuItems.Count - 1)) Selected = _radialMenuItems[_selectedIndex];
            }
        }
        public Action<int> OnSelectedIndexChanged;
        
        private RadialMenuItemScript _selected;
        public RadialMenuItemScript Selected
        {
            get => _selected;
            private set
            {
                _selected = value;
                OnSelectedChanged?.Invoke(_selected);
            }
        }
        public Action<RadialMenuItemScript> OnSelectedChanged;
        
        public Action<RadialMenuItemScript> OnClicked;
        public Action OnClosed;
        #endregion
        
        private void Awake()
        {
            _nnActions.Enable();
            var uiMap = _nnActions.FindActionMap("UI");
            _selectAction = uiMap.FindAction("Select");
            _backAction = uiMap.FindAction("Back");
            
            if (_animateOnOpen)
            {
                transform.localPosition = _startPosition;
                transform.localRotation = Quaternion.Euler(_startRotation);
                transform.localScale = _startScale;
            }
        }

        private void Start()
        {
            if (_animateOnOpen) Animate(_openAnimationParams);
        }

        private void Update()
        {
            Selection();
            if (_backAction.WasReleasedThisFrame()) Back();
        }

        public void Populate(RadialMenuContent content)
        {
            foreach (var item in _radialMenuItems) item.Destroy();
            _radialMenuItems.Clear();
            
            ItemCount = content.Items.Count;
            var menuSize = _itemsParent.rect.height / 2;
            
            for (int i = 0; i < ItemCount; i++)
            {
                var angle = 450 - (_anglePerItem * i) - (_anglePerItem / 2f);
                var radians = angle * Mathf.Deg2Rad;
                var itemPos = Vector2.Lerp(
                    Vector2.zero,
                    new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * menuSize,
                    _itemDistance);
                var item = Instantiate(content.Items[i].Prefab, _itemsParent);
                item.GetComponent<RectTransform>().anchoredPosition = itemPos;
                _radialMenuItems.Add(item);
                item.UpdateData(content.Items[i], this);
                item.StartPosition = item.transform.localPosition;
                if (_overwriteOpenItemAnimation) item.OpenAnimationParams = _openItemAnimationParams;
                if (_overwriteCloseItemAnimation) item.CloseAnimationParams = _closeItemAnimationParams;
            }

            foreach (var item in _radialMenuItems) item.Create();
            HoveredIndex = -1; //Needed to refresh the selected item
        }

        private void Selection()
        {
            if (ItemCount == 0 || _closeRoutine != null) return;
            var angle = Misc.RadialSelection(_menu.position);
            HoveredIndex = Mathf.FloorToInt(angle / _anglePerItem);
            if (_selectAction.IsPressed()) SelectedIndex = HoveredIndex;
            if (_selectAction.WasReleasedThisFrame()) Click();
        }

        private void Click()
        {
            if (Selected == null) return;
            _radialMenuItems[SelectedIndex].Up();
            OnClicked?.Invoke(Selected);
        }

        private void Back()
        {
            Close();
        }

        public void Close() => RestartRoutine(ref _closeRoutine, CloseRoutine());
        private Coroutine _closeRoutine;
        private IEnumerator CloseRoutine()
        {
            if (_animateOnClose)
            {
                Animate(_closeAnimationParams);
                foreach (var item in _radialMenuItems) item.Destroy();
                yield return new WaitWhile(() => IsAnimating);
            }
            Destroy(gameObject);
            _closeRoutine = null;
        }
        
    }
}