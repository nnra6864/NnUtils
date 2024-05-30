using UnityEngine;

namespace NnUtils.Scripts.UI.RadialMenu
{
    [CreateAssetMenu(menuName = "NnUtils/RadialMenu/SubmenuItem", fileName = "RadialSubmenuItem")]
    public class RadialSubmenuItem : RadialMenuItem
    {
        public RadialMenuContent Content;

        public void Reset() => CloseOnSelect = false;
    }
}