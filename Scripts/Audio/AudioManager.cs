using System.Collections.Generic;
using UnityEngine;

namespace NnUtils.Scripts.Audio
{
    public class AudioManager : NnBehaviour
    {
        [SerializeField] private List<Sound> _sounds;
        private Dictionary<string, AudioSource> _objectSources;
    }
}