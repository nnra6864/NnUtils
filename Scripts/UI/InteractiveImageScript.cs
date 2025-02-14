using System.Collections;
using System.Linq;
using NnUtils.Modules.JSONUtils.Scripts.Types.Components.UI.Image;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NnUtils.Scripts.UI
{
    [RequireComponent(typeof(AspectRatioFitter))]
    public class InteractiveImageScript : Image
    {
        private static ProjectImageManagerScript ImageManager => NnManager.ImageManager;

        /// Stores the current arf component
        private AspectRatioFitter _imageARFComponent;

        /// Name of the image to be used. <br/>
        /// Valid values are: <br/>
        /// - Name available in the <see cref="ProjectImageManagerScript"/> <br/>
        /// - Path to a local image <br/>
        /// - Image URL
        public string Image = "";

        /// Fallback sprite if image is not found
        public Sprite DefaultSprite;

        /// Determines how the image will be scaled
        public AspectRatioFitter.AspectMode ScalingMode = AspectRatioFitter.AspectMode.None;

        protected override void Awake()
        {
            // Get components
            _imageARFComponent = GetComponent<AspectRatioFitter>();

            // Load the image
            LoadData(Image, color, raycastTarget, raycastPadding, maskable, ScalingMode);
        }

        /// Loads the image
        public void LoadData(string image = "", Color imageColor = default,
            bool imageRaycast = true, Vector4 imageRaycastPadding = default, bool imageMaskable = true,
            AspectRatioFitter.AspectMode scalingMode = AspectRatioFitter.AspectMode.None)
        {
            Image                         = image;
            color                         = imageColor;
            raycastTarget                 = imageRaycast;
            raycastPadding                = imageRaycastPadding;
            maskable                      = imageMaskable;
            _imageARFComponent.aspectMode = ScalingMode = scalingMode;

            // Load the image and apply scaling
            this.StopRoutine(ref _webImageRoutine);
            this.RestartRoutine(ref _loadImageRoutine, LoadImageRoutine());
        }

        private Coroutine _loadImageRoutine, _webImageRoutine;

        private IEnumerator LoadImageRoutine()
        {
            // Try to load the sprite from a file or project images and assign it to the Image component
            sprite = ImageManager.Images.FirstOrDefault(x => x.Name == Image).Sprite ?? Misc.SpriteFromFile(Image);

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
            sprite ??= DefaultSprite;

            // Scale the image
            ScaleImage();

            _loadImageRoutine = null;
        }

        // Applies proper image scaling
        private void ScaleImage()
        {
            if (!sprite) return;
            var rect = sprite.rect;
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