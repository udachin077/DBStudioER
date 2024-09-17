using System.Data.SqlClient;

namespace Data.SqlServer
{
    public class SqlServer
    {
        protected SqlConnection sqlConnection;

        public SqlServer()
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = ".",
                IntegratedSecurity = true,
                ConnectTimeout = 5,
                InitialCatalog = "protokol"
            };
            sqlConnection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
        }
    }
}
