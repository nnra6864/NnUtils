using UnityEngine;

namespace NnUtils.Scripts.UI
{
    public class ProjectImageManagerScript : MonoBehaviour
    {
        [SerializeField] private ProjectImage[] _images;
        public ProjectImage[] Images => _images;
    }
}