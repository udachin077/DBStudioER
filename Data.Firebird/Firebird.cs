using Settings;
using FirebirdSql.Data.FirebirdClient;
using Base;

namespace Data.Firebird
{
    public class Firebird
    {
        protected FbConnection fbConnection;

        public Firebird(ProgramName programName)
        {
            FbConnectionStringBuilder fbConnectionStringBuilder = new FbConnectionStringBuilder
            {
                Charset = "WIN1251",
                UserID = "SYSDBA",
                Password = "masterkey",
                ServerType = 0,
                DataSource = "LOCALHOST"            
            };
            if (programName == ProgramName.WinAl) 
                fbConnectionStringBuilder.Database = AppSettings.String.GetValue(KeyName.WinAlDatabase);
            else if (programName == ProgramName.ASUDScada) 
                fbConnectionStringBuilder.Database = AppSettings.String.GetValue(KeyName.ASUDScadaDatabase);
            fbConnection = new FbConnection(fbConnectionStringBuilder.ConnectionString);
        }
    }
}
