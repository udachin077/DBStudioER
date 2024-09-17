using System.ComponentModel;

namespace Base
{
    public enum DbMS : byte
    {
        [Description("Firebird")]
        Firebird = 1,
        [Description("MSDE 2000")]
        Msde = 2,
        [Description("Microsoft Access")]
        MSAccess = 3,
        [Description("Microsoft SQL Server")]
        SqlServer = 4
    }
}
