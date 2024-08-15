using NnUtils.Scripts.Audio;
using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace NnUtils.Scripts
{
    public class NnManager : MonoBehaviour
    {
        private static NnManager _instance;
        public static NnManager Instance
        {
            get
            {
                if (_instance == null) 
                    _instance = new GameObject("NnManager", typeof(NnManager)).GetComponent<NnManager>();
                return _instance;
            }
            
            private set
            {
                if (_instance != null && _instance != value)
                {
                    Destroy(value.gameObject);
                    return;
                }
                _instance = value;
            }
        }
        
        private void Awake() => Instance = GetComponent<NnManager>();
    
        private static GameObject GameObject => Instance.gameObject;

        [SerializeField] private TimeManager _timeManager;
        public static TimeManager TimeManager =>
            Instance._timeManager ?? (Instance._timeManager = GameObject.GetOrAddComponent<TimeManager>());
    
        [SerializeField] private AudioManager _audioManager;
        public static AudioManager AudioManager =>
            Instance._audioManager ?? (Instance._audioManager = GameObject.GetOrAddComponent<AudioManager>());
    }
}