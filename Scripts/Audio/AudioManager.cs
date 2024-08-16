using System.Collections.Generic;
using UnityEngine;

namespace NnUtils.Scripts.Audio
{
    public class AudioManager : NnBehaviour
    {
        [SerializeField] private List<Sound> _sounds;
        private Dictionary<string, SoundEmitter> _emitters = new();

        private void Awake()
        {
            _emitters.Clear();
            foreach (var sound in _sounds)
            {
                var source = gameObject.AddComponent<AudioSource>();
                var emitter = Instantiate(new GameObject(), transform).AddComponent<SoundEmitter>();
                emitter.Init(sound, source, false);
                _emitters.Add(sound.Name, emitter);
            }
        }
    }
}