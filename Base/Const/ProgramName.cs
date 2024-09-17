using System.ComponentModel;

namespace Base
{
    public enum ProgramName : byte
    {
        [Description("WinAl")]
        WinAl = 1,
        [Description("АСУ ОДС")]
        AsuOdsMsde,
        [Description("Горизонт")]
        Horizon,
        [Description("АСУ ОДС")]
        AsuOdsSqlServer,
        [Description("ASUD Scada")]
        ASUDScada
    }
}