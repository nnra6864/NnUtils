using NnUtils.Modules.JSONUtils.Scripts.Types.Components.UI.Image;
using UnityEngine;

namespace NnUtils.Scripts
{
    [RequireComponent(typeof(TimeManager))]
    [RequireComponent(typeof(ProjectImageManagerScript))]
    public class NnManager : MonoBehaviour
    {
        private static NnManager _instance;
        public static NnManager Instance
        {
            get
            {
                // Return the instance if it's not null
                if (_instance != null) return _instance;
                
                // Try to find an instance and return it
                _instance = FindFirstObjectByType<NnManager>();
                if (_instance != null) return _instance;

                // Create a new object, don't destroy it on load, assign it to _instance and return it
                var go = new GameObject("NnManager");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<NnManager>();
                return _instance;
            }
        }
        
        private void Awake()
        {
            // If no instance is found, assign this and don't destroy it on load
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            // If instance was found, check if it's not this and destroy this game object
            else if (_instance != this) Destroy(gameObject);
        }

        private void Reset()
        {
            _timeManager = GetComponent<TimeManager>();
            _imageManager = GetComponent<ProjectImageManagerScript>();
        }

        [SerializeField] private TimeManager _timeManager;
        public static TimeManager TimeManager => Instance._timeManager;

        [SerializeField] private ProjectImageManagerScript _imageManager;
        public static ProjectImageManagerScript ImageManager => Instance._imageManager;
    }
}