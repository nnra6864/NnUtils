using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NnUtils.Scripts.UI.RadialMenu
{
    [RequireComponent(typeof(Image))]
    public class RadialMenuItemScript : NnBehaviour
    {
        private RadialMenuItem _menuItem;
        public RadialMenuItem MenuItem => _menuItem;
        protected RadialMenuScript _radialMenuScript;
        [SerializeField] private Image _image;

        [Header("Animation")]
        public Vector3 StartPosition;
        public Vector3 StartRotation;
        public Vector3 StartScale;
        public AnimationParams OpenAnimationParams;
        public AnimationParams CloseAnimationParams;

        private void Reset() => _image = GetComponent<Image>();

        public void UpdateData(RadialMenuItem item, RadialMenuScript radialMenuScript)
        {
            _menuItem = item;
            _radialMenuScript = radialMenuScript;
            _image.sprite = _menuItem.Sprite;
        }

        public virtual void Create()
        {
            transform.localPosition = StartPosition;
            transform.localRotation = Quaternion.Euler(StartRotation);
            transform.localScale = StartScale;
            Animate(OpenAnimationParams);
        }

        public virtual void Destroy()
        {
            Animate(CloseAnimationParams);
            StartRoutineIf(ref _destroyRoutine, DestroyRoutine(), () => !IsAnimating);
        }
        private Coroutine _destroyRoutine;
        private IEnumerator DestroyRoutine()
        {
            yield return new WaitWhile(() => IsAnimating);
            Destroy(gameObject);
        }

        public virtual void Enter()
        {
            
        }

        public virtual void Leave()
        {
            
        }

        public virtual void Down()
        {
            
        }

        public virtual void Up()
        {
            if (_menuItem.CloseOnSelect) _radialMenuScript.Close();
        }
    }
}