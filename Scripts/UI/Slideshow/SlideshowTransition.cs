using System;
using NnUtils.Modules.Easings;

namespace NnUtils.Scripts.UI.Slideshow
{
    [Serializable]
    public class SlideshowTransition
    {
        public SlideshowTransitionType TransitionType;
        public float Duration;
        public EasingType Easing;

        public SlideshowTransition() : this(SlideshowTransitionType.None, 0, EasingType.Linear) { }

        public SlideshowTransition(SlideshowTransitionType transitionType, float duration, EasingType easing)
        {
            TransitionType = transitionType;
            Duration = duration;
            Easing = easing;
        }


    }
}
