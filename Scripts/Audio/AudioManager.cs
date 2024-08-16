using System.Collections.Generic;
using UnityEngine;

namespace NnUtils.Scripts.Audio
{
    public class AudioManager : NnBehaviour
    {
        [SerializeField] private List<Sound> _sounds;
        private readonly Dictionary<string, SoundEmitter> _emitters = new();

        private void Awake()
        {
            _emitters.Clear();
            foreach (var sound in _sounds)
            {
                var source = gameObject.AddComponent<AudioSource>();
                var emitter = Instantiate(new GameObject(), transform).AddComponent<SoundEmitter>();
                emitter.Init(sound, source, false, false, false);
                _emitters.Add(sound.Name, emitter);
            }
        }

        public void Play(string soundName) => GetEmitter(_emitters, soundName).Play();
        public void UnPause(string soundName) => GetEmitter(_emitters, soundName).UnPause();
        public void Pause(string soundName) => GetEmitter(_emitters, soundName).Pause();
        public void Stop(string soundName) => GetEmitter(_emitters, soundName).Stop();

        public void PlayAt(string soundName, Vector3 pos) => PlayAt(_sounds.Find(x => x.Name == soundName), pos);
        public void PlayAt(Sound sound, Vector3 pos)
        {
            var emitter = Instantiate(new GameObject(), pos, Quaternion.identity).AddComponent<SoundEmitter>();
            emitter.Init(sound);
            emitter.Play();
        }
        
        private SoundEmitter GetEmitter(Dictionary<string, SoundEmitter> emitters, string soundName) =>
            _emitters.GetValueOrDefault(soundName);
    }
}