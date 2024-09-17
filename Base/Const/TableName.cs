using System.ComponentModel;

namespace Base
{
    public enum TableName
    {
        [Description("Horizon")]
        DoorEvents,
        [Description("Horizon")]
        Doors,
        [Description("WinAl")]
        journal_hardware_alerts,
        [Description("WinAl, ASUDScada")]
        address,
        [Description("WinAl, ASUDScada")]
        hardware_alerts,
        [Description("WinAl")]
        concentrators,
        [Description("AsuOdsMsde")]
        Protdisp,
        [Description("ASUDScada")]
        journal_hardware,
        [Description("ASUDScada")]
        item_tags
    }
}
