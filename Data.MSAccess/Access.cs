using Settings;
using System.Data.OleDb;

namespace Data.Access
{
    public class Access
    {
        protected OleDbConnection oleDbConnection;

        public Access()
        {
            OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new OleDbConnectionStringBuilder
            {
                ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Jet OLEDB:Database Password=masterkey"
            };
            oleDbConnectionStringBuilder.DataSource = AppSettings.String.GetValue(KeyName.HorizonDatabase);
            oleDbConnection = new OleDbConnection(oleDbConnectionStringBuilder.ConnectionString);
        }
    }
}
