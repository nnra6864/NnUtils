using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NnUtils.Scripts.UI.Slideshow
{
    public class Slideshow : MonoBehaviour
    {
        private readonly Dictionary<Image, Coroutine> _transitionRoutines = new();
        
        public SlideshowImage[] Images;
        public SlideshowTransition[] Transitions;

        public void LoadData(SlideshowImage[] images, SlideshowTransition[] transitions)
        {
            Images = images;
            Transitions = transitions;
        }

        private Coroutine _displayImagesRoutine;

        private IEnumerator DisplayImagesRoutine()
        {
            while (true)
            {
                var image = Images[Random.Range(0, Images.Length)];
                var transition = Transitions[Random.Range(0, Transitions.Length)];

                float lerpPos = 0;
                while (lerpPos < 1)
                {
                    var t = Misc.Tween(ref lerpPos, transition.Duration, transition.Easing);
                }
            }
        }
    }
}