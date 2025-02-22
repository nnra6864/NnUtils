using System;

namespace NnUtils.Scripts.UI.Slideshow
{
    [Serializable]
    public class SlideshowTransition
    {
        public SlideshowTransitionType TransitionType;
        public float Duration;
        public Easings.Type Easing;

        public SlideshowTransition() : this(SlideshowTransitionType.None, 0, Easings.Type.None) { }
        
        public SlideshowTransition(SlideshowTransitionType transitionType, float duration, Easings.Type easing)
        {
            TransitionType = transitionType;
            Duration = duration;
            Easing = easing;
        }
        
        
    }
}