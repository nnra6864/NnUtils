using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NnUtils.Scripts.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class EnvelopedImage : Image
    {
        private Sprite _currentSprite;
        [HideInInspector] [SerializeField] private AspectRatioFitter _aspectRatioFitter;

        protected override void Awake()
        {
            SetupARF();
            base.Awake();
        }

        private void SetupARF()
        {
            _aspectRatioFitter            ??= gameObject.GetComponent<AspectRatioFitter>();
            _aspectRatioFitter.aspectMode =   AspectRatioFitter.AspectMode.EnvelopeParent;
        }

#if UNITY_EDITOR
        
        protected override void Reset()
        {
            SetupARF();
            base.Reset();
        }
        
        [MenuItem(itemName: "GameObject/UI/EnvelopedImage")]
        private static void CreateEnvelopedImage()
        {
            // Add the EnvelopedImage to the scene
            var obj = new GameObject("EnvelopedImage");
            obj.AddComponent<EnvelopedImage>();
            
            // Set its parent to the active object if not null
            obj.transform.SetParent(Selection.activeTransform, false);

            // Select the newly created object
            Selection.activeTransform = obj.transform;
            
            // Set object as dirty to ensure it's saved
            EditorUtility.SetDirty(obj);
        }
        
        #endif
        
        private void Update()
        {
            if (sprite == _currentSprite) return;
            _currentSprite = sprite;
            if (_currentSprite == null) return;
            var rect = _currentSprite.rect;
            _aspectRatioFitter.aspectRatio = rect.width / rect.height;
        }
    }
}