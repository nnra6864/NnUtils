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
            var soundEmitters = new GameObject("SoundEmitters").transform;
            soundEmitters.SetParent(transform);
            foreach (var sound in _sounds)
            {
                var emitter = new GameObject($"{sound.Name}Emitter").AddComponent<SoundEmitter>();
                emitter.transform.SetParent(soundEmitters);
                var source = emitter.gameObject.AddComponent<AudioSource>();
                emitter.Init(sound, source, false, false, false);
                _emitters.Add(sound.Name, emitter);
            }
        }

        public void Play(string soundName) => GetEmitter(_emitters, soundName).Play();
        public void Play(string soundName, float pitch) => GetEmitter(_emitters, soundName).Play(pitch);
        public void UnPause(string soundName) => GetEmitter(_emitters, soundName).UnPause();
        public void Pause(string soundName) => GetEmitter(_emitters, soundName).Pause();
        public void Stop(string soundName) => GetEmitter(_emitters, soundName).Stop();

        public void PlayAt(string soundName, Vector3 pos) =>
            PlayAt(_sounds.Find(x => x.Name == soundName), pos);
        public void PlayAt(Sound sound, Vector3 pos)
        {
            var emitter = new GameObject($"{sound.Name}Emitter").AddComponent<SoundEmitter>();
            emitter.transform.position = pos;
            emitter.Init(sound);
            emitter.Play();
        }
        
        public void PlayAt(string soundName, Vector3 pos, float pitch) =>
            PlayAt(_sounds.Find(x => x.Name == soundName), pos, pitch);
        public void PlayAt(Sound sound, Vector3 pos, float pitch)
        {
            var emitter = new GameObject($"{sound.Name}Emitter").AddComponent<SoundEmitter>();
            emitter.transform.position = pos;
            emitter.Init(sound);
            emitter.Play(pitch);
        }
        
        private SoundEmitter GetEmitter(Dictionary<string, SoundEmitter> emitters, string soundName) =>
            _emitters.GetValueOrDefault(soundName);
    }
}