using Base;
using System;
using System.Data;
using System.Data.OleDb;

namespace Data.Msde
{
    public partial class MsdeController : Msde, IDatabaseManagementSystem
    {
        private int counter;

        public MsdeController() : base()
        {
            InitializeDataAdapter();
            oleDbDataAdapter.RowUpdated += (sender, e) =>
            {
                ++counter;
                CounterChanged?.Invoke(counter);
            };
        }

        protected OleDbDataAdapter oleDbDataAdapter;
        public event Action<int> CounterChanged;

        public void Connect(ref DataSet ds)
        {
            ds.Clear();
            oleDbDataAdapter.Fill(ds, "Protdisp");
        }

        public void CreateDataSet()
        {
            throw new NotImplementedException();
        }    

        public void Merge(ref DataTable journal)
        {
            OleDbCommand oleDbCommand = new OleDbCommand
            {
                Connection = oleDbConnection,
                CommandText = "INSERT INTO [Protdisp] ([h_street], [h_nom], [h_noma], [h_korp], [h_pod], [term], [dat], [dat_tip], [dat_name], [dtn], [dtk], [prost_text], [prost], [pradres]) " +
                   "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
            };
            // Параметры команды вcтавки
            oleDbCommand.Parameters.Add("h_street", OleDbType.VarWChar, 60, "h_street");
            oleDbCommand.Parameters.Add("h_nom", OleDbType.SmallInt, 2, "h_nom");
            oleDbCommand.Parameters.Add("h_noma", OleDbType.VarWChar, 10, "h_noma");
            oleDbCommand.Parameters.Add("h_korp", OleDbType.SmallInt, 2, "h_korp");
            oleDbCommand.Parameters.Add("h_pod", OleDbType.SmallInt, 2, "h_pod");
            oleDbCommand.Parameters.Add("term", OleDbType.SmallInt, 2, "term");
            oleDbCommand.Parameters.Add("dat", OleDbType.SmallInt, 2, "dat");
            oleDbCommand.Parameters.Add("dat_tip", OleDbType.SmallInt, 2, "dat_tip");
            oleDbCommand.Parameters.Add("dat_name", OleDbType.VarWChar, 20, "dat_name");
            oleDbCommand.Parameters.Add("dtn", OleDbType.Date, 8, "dtn");
            oleDbCommand.Parameters.Add("dtk", OleDbType.Date, 8, "dtk");
            oleDbCommand.Parameters.Add("prost_text", OleDbType.VarWChar, 20, "prost_text");
            oleDbCommand.Parameters.Add("prost", OleDbType.BigInt, 8, "prost");
            oleDbCommand.Parameters.Add("pradres", OleDbType.VarWChar, 60, "pradres");
            try
            {
                // Открытие подключения
                oleDbConnection?.Open();
                // Добавление строк
                counter = 0;
                foreach (DataRow item in journal.Select())
                {
                    ++counter;
                    CounterChanged?.Invoke(counter);
                    try
                    {
                        oleDbCommand.Parameters["h_street"].Value = item["h_street"];
                        oleDbCommand.Parameters["h_nom"].Value = item["h_nom"];
                        oleDbCommand.Parameters["h_noma"].Value = item["h_noma"];
                        oleDbCommand.Parameters["h_korp"].Value = item["h_korp"];
                        oleDbCommand.Parameters["h_pod"].Value = item["h_pod"];
                        oleDbCommand.Parameters["term"].Value = item["term"];
                        oleDbCommand.Parameters["dat"].Value = item["dat"];
                        oleDbCommand.Parameters["dat_tip"].Value = item["dat_tip"];
                        oleDbCommand.Parameters["dat_name"].Value = item["dat_name"];
                        oleDbCommand.Parameters["dtn"].Value = item["dtn"];
                        oleDbCommand.Parameters["dtk"].Value = item["dtk"];
                        oleDbCommand.Parameters["prost_text"].Value = item["prost_text"];
                        oleDbCommand.Parameters["prost"].Value = item["prost"];
                        oleDbCommand.Parameters["pradres"].Value = item["pradres"];
                        oleDbCommand.ExecuteNonQuery();

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
                oleDbConnection?.Close();
                oleDbCommand?.Dispose();
            }
        }

        public void QueryExecute(string query)
        {
            OleDbConnection _oleDbConnection = new OleDbConnection("Provider=sqloledb;User Id=sa;Password=;Connect Timeout=3;");
            OleDbCommand oleDbCommand = new OleDbCommand
            {
                Connection = _oleDbConnection
            };
            oleDbCommand.CommandText = query;
            try
            {
                // Открытие подключения
                _oleDbConnection?.Open();
                oleDbCommand.ExecuteNonQuery();
            }
            finally
            {
                // Закрытие подключения
                _oleDbConnection?.Close();
                oleDbCommand?.Dispose();
            }
        }

        public void Update(ref DataSet ds)
        {
            counter = 0;
            oleDbDataAdapter.Update(ds, "Protdisp");
        }

        private void InitializeDataAdapter()
        {
            oleDbDataAdapter = new OleDbDataAdapter();
            InitializeDataAdapterDeleteCommand();
            InitializeDataAdapterInsertCommand();
            InitializeDataAdapterSelectCommand();
            InitializeDataAdapterUpdateCommand();
        }
        /// <summary>
        /// Инициализация команды DELETE.
        /// </summary>
        private void InitializeDataAdapterDeleteCommand()
        {
            oleDbDataAdapter.DeleteCommand = new OleDbCommand(
                "DELETE FROM " +
                    "[Protdisp] " +
                "WHERE " +
                    "Id=?", oleDbConnection);
            OleDbParameter DeleteParameter = oleDbDataAdapter.DeleteCommand.Parameters.Add("@p1", OleDbType.BigInt, 32, "Id");
            DeleteParameter.SourceVersion = DataRowVersion.Original;
        }
        /// <summary>
        /// Инициализация команды INSERT.
        /// </summary>
        private void InitializeDataAdapterInsertCommand()
        {
            oleDbDataAdapter.InsertCommand = new OleDbCommand(
                "INSERT INTO " +
                    "[Protdisp] ([h_street], [h_nom], [h_noma], [h_dec], [h_char], [h_korp], [h_pod], [term], [dat], [dat_tip], [dat_name], [dtn], [dtk], [prost_text], [prost], [pradres]) " +
                "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", oleDbConnection);
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p2", OleDbType.VarWChar, 60, "h_street");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p3", OleDbType.SmallInt, 2, "h_nom");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p4", OleDbType.VarWChar, 10, "h_noma");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p5", OleDbType.SmallInt, 2, "h_dec");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p6", OleDbType.SmallInt, 2, "h_char");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p7", OleDbType.SmallInt, 2, "h_korp");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p8", OleDbType.SmallInt, 2, "h_pod");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p9", OleDbType.SmallInt, 2, "term");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p10", OleDbType.SmallInt, 2, "dat");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p11", OleDbType.SmallInt, 2, "dat_tip");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p12", OleDbType.VarWChar, 20, "dat_name");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p13", OleDbType.Date, 8, "dtn");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p14", OleDbType.Date, 8, "dtk");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p15", OleDbType.VarWChar, 20, "prost_text");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p16", OleDbType.BigInt, 8, "prost");
            oleDbDataAdapter.InsertCommand.Parameters.Add("@p17", OleDbType.VarWChar, 60, "pradres");
        }
        /// <summary>
        /// Инициализация команды SELECT.
        /// </summary>
        private void InitializeDataAdapterSelectCommand()
        {
            oleDbDataAdapter.SelectCommand = new OleDbCommand(
                "SELECT " +
                    "[Id], [h_street], [h_nom], [h_noma], [h_dec], [h_char], [h_korp], [h_pod], [term], [dat], [dat_tip], [dat_name], [dtn], [dtk], [prost_text], [prost], [pradres] " +
                "FROM " +
                    "[Protdisp] " +
                "ORDER BY " +
                    "dtn ASC", oleDbConnection);
        }
        /// <summary>
        /// Инициализация команды UPDATE.
        /// </summary>
        private void InitializeDataAdapterUpdateCommand()
        {
            oleDbDataAdapter.UpdateCommand = new OleDbCommand(
                    "UPDATE " +
                        "[Protdisp] " +
                    "SET " +
                        "[h_street]=?, [h_nom]=?, [h_noma]=?, [h_dec]=?, [h_char]=?, [h_korp]=?, [h_pod]=?, [term]=?, [dat]=?, [dat_tip]=?, [dat_name]=?, [dtn]=?, [dtk]=?, [prost_text]=?, [prost]=?, [pradres]=? " +
                    "WHERE " +
                        "Id=?", oleDbConnection);
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p2", OleDbType.VarWChar, 60, "h_street");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p3", OleDbType.SmallInt, 2, "h_nom");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p4", OleDbType.VarWChar, 10, "h_noma");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p5", OleDbType.SmallInt, 2, "h_dec");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p6", OleDbType.SmallInt, 2, "h_char");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p7", OleDbType.SmallInt, 2, "h_korp");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p8", OleDbType.SmallInt, 2, "h_pod");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p9", OleDbType.SmallInt, 2, "term");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p10", OleDbType.SmallInt, 2, "dat");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p11", OleDbType.SmallInt, 2, "dat_tip");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p12", OleDbType.VarWChar, 20, "dat_name");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p13", OleDbType.Date, 8, "dtn");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p14", OleDbType.Date, 8, "dtk");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p15", OleDbType.VarWChar, 20, "prost_text");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p16", OleDbType.BigInt, 8, "prost");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p17", OleDbType.VarWChar, 60, "pradres");
            oleDbDataAdapter.UpdateCommand.Parameters.Add("@p1", OleDbType.BigInt, 8, "id");
        }        
    }
}
