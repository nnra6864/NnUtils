using NnUtils.Scripts.Audio;
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
                if (_instance != null) return _instance;
                
                _instance = FindFirstObjectByType<NnManager>();
                if (_instance != null)
                {
                    DontDestroyOnLoad(_instance);
                    return _instance;
                }

                var go = new GameObject("NnManager");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<NnManager>();
                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        [SerializeField] private TimeManager _timeManager;
        public static TimeManager TimeManager =>
            Instance._timeManager ?? (Instance._timeManager = Instance.gameObject.GetOrAddComponent<TimeManager>());
    
        [SerializeField] private AudioManager _audioManager;
        public static AudioManager AudioManager =>
            Instance._audioManager ?? (Instance._audioManager = Instance.gameObject.GetOrAddComponent<AudioManager>());
    }
}