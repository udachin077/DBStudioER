using Base;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Data.SqlServer
{
    public class SqlController : SqlServer, IDatabaseManagementSystem
    {
        public SqlController() : base()
        {
            InitializeDataAdapter();
        }


        protected SqlDataAdapter sqlDataAdapter;

        private void InitializeDataAdapter()
        {
            sqlDataAdapter = new SqlDataAdapter();
            InitializeDataAdapterDeleteCommand();
            InitializeDataAdapterInsertCommand();
            InitializeDataAdapterSelectCommand();
            InitializeDataAdapterUpdateCommand();
        }

        public void Connect(ref DataSet ds)
        {
            ds.Clear();
            sqlDataAdapter.Fill(ds, "Protdisp");
        }

        public void CreateDataSet()
        {
            throw new NotImplementedException();
        }

        public void Drop(ref DataSet ds)
        {
            SqlCommand sqlCommand = new SqlCommand
            {
                CommandText = $"DELETE Protdisp",
                Connection = sqlConnection
            };
            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            finally
            {
                sqlCommand?.Dispose();
                sqlConnection?.Close();
                ds.AcceptChanges();
            }
        }

        public void Insert(ref DataTable journal)
        {
            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = "INSERT INTO [Protdisp] ([h_street], [h_nom], [h_noma], [h_korp], [h_pod], [term], [dat], [dat_tip], [dat_name], [dtn], [dtk], [prost_text], [prost], [pradres]) " +
                  "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
            };
            // Параметры команды вcтавки
            sqlCommand.Parameters.Add("h_street", SqlDbType.NVarChar, 60, "h_street");
            sqlCommand.Parameters.Add("h_nom", SqlDbType.SmallInt, 2, "h_nom");
            sqlCommand.Parameters.Add("h_noma", SqlDbType.NVarChar, 10, "h_noma");
            sqlCommand.Parameters.Add("h_korp", SqlDbType.SmallInt, 2, "h_korp");
            sqlCommand.Parameters.Add("h_pod", SqlDbType.SmallInt, 2, "h_pod");
            sqlCommand.Parameters.Add("term", SqlDbType.SmallInt, 2, "term");
            sqlCommand.Parameters.Add("dat", SqlDbType.SmallInt, 2, "dat");
            sqlCommand.Parameters.Add("dat_tip", SqlDbType.SmallInt, 2, "dat_tip");
            sqlCommand.Parameters.Add("dat_name", SqlDbType.NVarChar, 20, "dat_name");
            sqlCommand.Parameters.Add("dtn", SqlDbType.Date, 8, "dtn");
            sqlCommand.Parameters.Add("dtk", SqlDbType.Date, 8, "dtk");
            sqlCommand.Parameters.Add("prost_text", SqlDbType.NVarChar, 20, "prost_text");
            sqlCommand.Parameters.Add("prost", SqlDbType.BigInt, 8, "prost");
            sqlCommand.Parameters.Add("pradres", SqlDbType.NVarChar, 60, "pradres");
            try
            {
                // Открытие подключения
                sqlConnection?.Open();
                // Добавление строк
                foreach (DataRow item in journal.Select())
                {
                    try
                    {
                        sqlCommand.Parameters["h_street"].Value = item["h_street"];
                        sqlCommand.Parameters["h_nom"].Value = item["h_nom"];
                        sqlCommand.Parameters["h_noma"].Value = item["h_noma"];
                        sqlCommand.Parameters["h_korp"].Value = item["h_korp"];
                        sqlCommand.Parameters["h_pod"].Value = item["h_pod"];
                        sqlCommand.Parameters["term"].Value = item["term"];
                        sqlCommand.Parameters["dat"].Value = item["dat"];
                        sqlCommand.Parameters["dat_tip"].Value = item["dat_tip"];
                        sqlCommand.Parameters["dat_name"].Value = item["dat_name"];
                        sqlCommand.Parameters["dtn"].Value = item["dtn"];
                        sqlCommand.Parameters["dtk"].Value = item["dtk"];
                        sqlCommand.Parameters["prost_text"].Value = item["prost_text"];
                        sqlCommand.Parameters["prost"].Value = item["prost"];
                        sqlCommand.Parameters["pradres"].Value = item["pradres"];
                        sqlCommand.ExecuteNonQuery();

                        item.Delete();
                    }
                    catch
                    {
                        // Если произошла ошибка, то строка не будет удалена и останется в таблице.
                        // Выполнение переходит к следующей строке
                        continue;
                    }
                }
            }
            finally
            {
                journal.AcceptChanges();
                // Закрытие подключения
                sqlConnection?.Close();
                sqlCommand?.Dispose();
            }
        }

        public void Update(ref DataSet ds)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Инициализация команды DELETE.
        /// </summary>
        private void InitializeDataAdapterDeleteCommand()
        {
        }
        /// <summary>
        /// Инициализация команды INSERT.
        /// </summary>
        private void InitializeDataAdapterInsertCommand()
        {

        }
        /// <summary>
        /// Инициализация команды SELECT.
        /// </summary>
        private void InitializeDataAdapterSelectCommand()
        {

        }
        /// <summary>
        /// Инициализация команды UPDATE.
        /// </summary>
        private void InitializeDataAdapterUpdateCommand()
        {

        }

        public void QueryExecute(string query)
        {
            throw new NotImplementedException();
        }
    }
}
