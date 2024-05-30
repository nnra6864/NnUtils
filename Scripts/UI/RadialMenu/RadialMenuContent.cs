using System.Collections.Generic;
using UnityEngine;

namespace NnUtils.Scripts.UI.RadialMenu
{
    [CreateAssetMenu(menuName = "NnUtils/RadialMenu/Content", fileName = "RadialMenuContent")]
    public class RadialMenuContent : ScriptableObject
    {
        public string Name;
        public List<RadialMenuItem> Items;
    }
}