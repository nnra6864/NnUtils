using System.Collections;
using System.Linq;
using NnUtils.Modules.JSONUtils.Scripts.Types.Components.UI.Image;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NnUtils.Scripts.UI
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class InteractiveImageScript : MonoBehaviour
    {
        private static ProjectImageManagerScript ImageManager => NnManager.ImageManager;

        /// Stores the current image component
        private Image _imageComponent;

        /// Stores the current arf component
        private AspectRatioFitter _imageARFComponent;

        /// Name of the image to be used. <br/>
        /// Valid values are: <br/>
        /// - Name available in the <see cref="ProjectImageManagerScript"/> <br/>
        /// - Path to a local image <br/>
        /// - Image URL
        public string Image = "";

        /// This will be used if image was not found
        public Sprite DefaultSprite;

        /// Color applied to the image
        public UnityEngine.Color Color = UnityEngine.Color.white;

        /// Whether raycast can detect this component
        public bool Raycast = true;

        /// Padding for the raycast
        public Vector4 RaycastPadding = Vector4.zero;

        /// Whether a mask can be applied to this component
        public bool Maskable = true;

        /// Determines how the image will be scaled
        public AspectRatioFitter.AspectMode ScalingMode = AspectRatioFitter.AspectMode.None;

        private void Awake()
        {
            // Get components
            _imageComponent    = GetComponent<Image>();
            _imageARFComponent = GetComponent<AspectRatioFitter>();

            // Load the image
            LoadImage(Image, Color, Raycast, RaycastPadding, Maskable, ScalingMode);
        }

        /// Loads the image
        public void LoadImage(string image = "", UnityEngine.Color color = default, bool raycast = true, Vector4 raycastPadding = default,
            bool maskable = true,
            AspectRatioFitter.AspectMode scalingMode = AspectRatioFitter.AspectMode.None)
        {
            Image                          = image;
            _imageComponent.color          = Color          = color;
            _imageComponent.raycastTarget  = Raycast        = raycast;
            _imageComponent.raycastPadding = RaycastPadding = raycastPadding;
            _imageComponent.maskable       = Maskable       = maskable;
            _imageARFComponent.aspectMode  = ScalingMode    = scalingMode;

            // Load the image and apply scaling
            this.StopRoutine(ref _webImageRoutine);
            this.RestartRoutine(ref _loadImageRoutine, LoadImageRoutine());
        }

        private Coroutine _loadImageRoutine, _webImageRoutine;

        private IEnumerator LoadImageRoutine()
        {
            // Try to load the sprite from a file or project images and assign it to the Image component
            var sprite = ImageManager.Images.FirstOrDefault(x => x.Name == Image).Sprite ?? Misc.SpriteFromFile(Image);

            // Try to load the sprite from the web
            if (sprite == null)
            {
                var isDone = false;
                _webImageRoutine = StartCoroutine(Misc.SpriteFromURL(Image, s =>
                {
                    sprite = s;
                    isDone = true;
                }));
                yield return new WaitUntil(() => isDone);
                _webImageRoutine = null;
            }

            // Assign default sprite if it's still null
            if (sprite == null) sprite = DefaultSprite;

            // Update the Image component's sprite
            _imageComponent.sprite = sprite;

            // Scale the image
            ScaleImage();

            _loadImageRoutine = null;
        }

        // Applies proper image scaling
        private void ScaleImage()
        {
            if (!_imageComponent.sprite) return;
            var rect = _imageComponent.sprite.rect;
            _imageARFComponent.aspectRatio = rect.width / rect.height;
        }

#if UNITY_EDITOR

        [MenuItem(itemName: "GameObject/UI/InteractiveImage")]
        private static void CreateEnvelopedImage()
        {
            // Add the Interactive to the scene
            var obj = new GameObject("InteractiveImage");
            obj.AddComponent<InteractiveImageScript>();

            // Set its parent to the active object if not null
            obj.transform.SetParent(Selection.activeTransform, false);

            // Select the newly created object
            Selection.activeTransform = obj.transform;

            // Set object as dirty to ensure it's saved
            EditorUtility.SetDirty(obj);
        }

#endif
    }
}