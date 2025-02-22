using System;

namespace NnUtils.Scripts.UI.Slideshow
{
    [Serializable]
    public class SlideshowImage
    {
        public InteractiveImageScript Image;
        public SlideshowTransition Transition;

        public SlideshowImage(InteractiveImageScript image, SlideshowTransition transition)
        {
            Image = image;
            Transition = transition;
        }
    }
}