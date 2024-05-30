using UnityEngine;

namespace NnUtils.Scripts.UI.RadialMenu
{
    [CreateAssetMenu(menuName = "NnUtils/RadialMenu/Item", fileName = "RadialMenuItem")]
    public class RadialMenuItem : ScriptableObject
    {
        public string Name;
        public Sprite Sprite;
        public RadialMenuItemScript Prefab;
        public bool CloseOnSelect = true;
    }
}