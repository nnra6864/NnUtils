namespace NnUtils.Scripts.UI.RadialMenu
{
    public class RadialSubmenuItemScript : RadialMenuItemScript
    {
        public override void Up()
        {
            base.Up();
            _radialMenuScript.Populate(((RadialSubmenuItem)MenuItem).Content);
        }
    }
}