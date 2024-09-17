using System.Data.OleDb;

namespace Data.Msde
{
    public class Msde
    {
        protected OleDbConnection oleDbConnection;

        public Msde()
        {
            OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new OleDbConnectionStringBuilder
            {
                ConnectionString = "Database=protokol;User Id=sa;Password=;Connect Timeout=5;",
                Provider = "sqloledb"
            };
            oleDbConnection = new OleDbConnection(oleDbConnectionStringBuilder.ConnectionString);
        }
    }
}
