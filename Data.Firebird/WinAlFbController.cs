using Base;
using Settings;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;

namespace Data.Firebird
{
    public partial class WinAlFbController : Firebird, IDatabaseManagementSystem
    {
        private int counter;

        public WinAlFbController() : base(ProgramName.WinAl)
        {
            InitializeDataAdapter();
            fbDataAdapter.RowUpdated += (sender, e) =>
            {
                ++counter;
                CounterChanged?.Invoke(counter);
            };
        }

        protected FbDataAdapter fbDataAdapter;
        public event Action<int> CounterChanged;

        public void Connect(ref DataSet ds)
        {
            ds.Clear();
            fbDataAdapter.Fill(ds, "journal_hardware_alerts");
            // Добавляет столбец с продолжительностью, если его не существует
            if (ds.Tables["journal_hardware_alerts"].Columns.Contains("prost_text") == false)
                ds.Tables["journal_hardware_alerts"].Columns.Add("prost_text", typeof(TimeSpan));
            // Считает продолжительность и добавляет в таблицу
            DateTime? date_close;
            var dc = DateTime.Now;
            foreach (var item in ds.Tables["journal_hardware_alerts"].Select())
            {
                if (item["date_close"] as DateTime? == null)
                {
                    date_close = new DateTime(dc.Year, dc.Month, dc.Day, dc.Hour, dc.Minute, dc.Second, dc.Kind);
                }
                else
                    date_close = item["date_close"] as DateTime?;

                item["prost_text"] = (TimeSpan)(date_close - (item["date_open"] as DateTime?));
            }
            ds.AcceptChanges();
            FbDataAdapter _fbDataAdapter = new FbDataAdapter();
            // Список сигналов (контактов)
            _fbDataAdapter.SelectCommand = new FbCommand("SELECT id_alert, name FROM hardware_alerts ORDER BY name ASC;", fbConnection);
            _fbDataAdapter.Fill(ds, "hardware_alerts");
            _fbDataAdapter?.Dispose();
        }

        public void CreateDataSet()
        {
            DataSet dataSet = new DataSet();
            FbDataAdapter _fbDataAdapter = new FbDataAdapter
            {
                SelectCommand = new FbCommand("SELECT id_address, name FROM address ORDER BY name ASC", fbConnection)
            };
            try
            {
                _fbDataAdapter.Fill(dataSet, "address");
                _fbDataAdapter.SelectCommand.CommandText = "SELECT id_alert, name FROM hardware_alerts ORDER BY name ASC";
                _fbDataAdapter.Fill(dataSet, "hardware_alerts");
                _fbDataAdapter.SelectCommand.CommandText = "SELECT id_conc, conc_direction, conc_number, conc_ip FROM concentrators ORDER BY conc_direction, conc_number ASC";
                _fbDataAdapter.Fill(dataSet, "concentrators");
                dataSet.WriteXml(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WinAlDataSet.ds"), XmlWriteMode.WriteSchema);
            }
            finally
            {
                _fbDataAdapter?.Dispose();
            }
        }

        public void Merge(ref DataTable journal)
        {
            FbCommand fbCommand = new FbCommand
            {
                Connection = fbConnection,
                CommandText = "INSERT INTO journal_hardware_alerts (conc_id, sensor_number, address_id, date_open, date_accept, date_close, alert_id) VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7)"
            };
            // Параметры команды втавки
            fbCommand.Parameters.Add("@p1", FbDbType.Integer, 32, "CONC_ID");
            fbCommand.Parameters.Add("@p2", FbDbType.Integer, 32, "SENSOR_NUMBER");
            fbCommand.Parameters.Add("@p3", FbDbType.Integer, 32, "ADDRESS_ID");
            fbCommand.Parameters.Add("@p4", FbDbType.TimeStamp, 64, "DATE_OPEN");
            fbCommand.Parameters.Add("@p5", FbDbType.TimeStamp, 64, "DATE_ACCEPT");
            fbCommand.Parameters.Add("@p6", FbDbType.TimeStamp, 64, "DATE_CLOSE");
            fbCommand.Parameters.Add("@p7", FbDbType.Integer, 32, "ALERT_ID");
            try
            {
                // Открытие подключения
                fbConnection?.Open();
                // Добавление строк    
                counter = 0;
                foreach (DataRow item in journal.Select())
                {
                    ++counter;
                    CounterChanged?.Invoke(counter);
                    try
                    {
                        fbCommand.Parameters["@p1"].Value = item["CONC_ID"];
                        fbCommand.Parameters["@p2"].Value = item["SENSOR_NUMBER"];
                        fbCommand.Parameters["@p3"].Value = item["ADDRESS_ID"];
                        fbCommand.Parameters["@p4"].Value = item["DATE_OPEN"];
                        fbCommand.Parameters["@p5"].Value = item["DATE_ACCEPT"];
                        fbCommand.Parameters["@p6"].Value = item["DATE_CLOSE"];
                        fbCommand.Parameters["@p7"].Value = item["ALERT_ID"];
                        fbCommand.ExecuteNonQuery();
                        // Если строка была добавлено, то удаляем ее из таблицы.
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
                // Сохраняем изменения после добавления
                journal.AcceptChanges();
                // Закрытие подключения
                fbConnection?.Close();
                fbCommand?.Dispose();
            }
        }

        public void QueryExecute(string query)
        {
            FbConnectionStringBuilder fbConnectionStringBuilder = new FbConnectionStringBuilder();
            fbConnectionStringBuilder.Charset = "WIN1251";
            fbConnectionStringBuilder.UserID = "SYSDBA";
            fbConnectionStringBuilder.Password = "masterkey";
            fbConnectionStringBuilder.ServerType = 0;
            fbConnectionStringBuilder.DataSource = "LOCALHOST";
            fbConnectionStringBuilder.Database = AppSettings.String.GetValue(KeyName.WinAlDatabase);
            FbConnection _fbConnection = new FbConnection(fbConnectionStringBuilder.ConnectionString);
            FbCommand fbCommand = new FbCommand
            {
                Connection = _fbConnection
            };
            fbCommand.CommandText = query;
            try
            {
                // Открытие подключения
                _fbConnection?.Open();
                fbCommand.ExecuteNonQuery();
            }
            finally
            {
                // Закрытие подключения
                _fbConnection?.Close();
                fbCommand?.Dispose();
            }
        }

        public void Update(ref DataSet ds)
        {
            counter = 0;
            fbDataAdapter.Update(ds, "journal_hardware_alerts");
        }

        private void InitializeDataAdapter()
        {
            fbDataAdapter = new FbDataAdapter();
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
            fbDataAdapter.DeleteCommand = new FbCommand(
            "DELETE FROM journal_hardware_alerts " +
            "WHERE id_record=@p1;", fbConnection);
            FbParameter fbDeleteParameter = fbDataAdapter.DeleteCommand.Parameters.Add("@p1", FbDbType.Integer, 32, "ID_RECORD");
            fbDeleteParameter.SourceVersion = DataRowVersion.Original;
        }
        /// <summary>
        /// Инициализация команды INSERT.
        /// </summary>
        private void InitializeDataAdapterInsertCommand()
        {
            fbDataAdapter.InsertCommand = new FbCommand(
            "INSERT INTO journal_hardware_alerts (conc_id, sensor_number, address_id, date_open, date_accept, date_close, alert_id) " +
            "VALUES" +
                "(" +
                    "(SELECT id_conc FROM concentrators WHERE conc_direction=@p2 AND conc_number=@p3), @p4, " +
                    "(SELECT id_address FROM address WHERE name=@p5), @p6, @p7, @p8, " +
                    "(SELECT id_alert FROM hardware_alerts WHERE name=@p9)" +
                ");", fbConnection);
            fbDataAdapter.InsertCommand.Parameters.Add("@p2", FbDbType.Integer, 32, "CONC_DIRECTION");
            fbDataAdapter.InsertCommand.Parameters.Add("@p3", FbDbType.Integer, 32, "CONC_NUMBER");
            fbDataAdapter.InsertCommand.Parameters.Add("@p4", FbDbType.Integer, 32, "SENSOR_NUMBER");
            fbDataAdapter.InsertCommand.Parameters.Add("@p5", FbDbType.VarChar, 64, "ADDRESS_NAME");
            fbDataAdapter.InsertCommand.Parameters.Add("@p6", FbDbType.TimeStamp, 64, "DATE_OPEN");
            fbDataAdapter.InsertCommand.Parameters.Add("@p7", FbDbType.TimeStamp, 64, "DATE_ACCEPT");
            fbDataAdapter.InsertCommand.Parameters.Add("@p8", FbDbType.TimeStamp, 64, "DATE_CLOSE");
            fbDataAdapter.InsertCommand.Parameters.Add("@p9", FbDbType.VarChar, 64, "ALERT_NAME");

        }
        /// <summary>
        /// Инициализация команды SELECT.
        /// </summary>
        private void InitializeDataAdapterSelectCommand()
        {
            fbDataAdapter.SelectCommand = new FbCommand(
            "SELECT " +
                "jha.id_record," +
                "jha.date_open," +
                "jha.date_accept," +
                "jha.date_close," +
                "jha.alert_id," +
                "jha.sensor_number," +
                "a.name AS address_name," +
                "ha.name AS alert_name," +
                "c.conc_direction," +
                "c.conc_number," +
                "c.conc_ip " +
            "FROM journal_hardware_alerts jha " +
            "JOIN address a ON a.id_address = jha.address_id " +
            "JOIN hardware_alerts ha ON ha.id_alert = jha.alert_id " +
            "JOIN concentrators c ON c.id_conc = jha.conc_id " +
            "ORDER BY jha.id_record ASC;", fbConnection);
        }
        /// <summary>
        /// Инициализация команды UPDATE.
        /// </summary>
        private void InitializeDataAdapterUpdateCommand()
        {
            fbDataAdapter.UpdateCommand = new FbCommand(
            "UPDATE journal_hardware_alerts " +
            "SET " +
                "conc_id=(SELECT id_conc FROM concentrators WHERE conc_direction=@p2 AND conc_number=@p3), " +
                "sensor_number=@p4, " +
                "address_id=(SELECT id_address FROM address WHERE name=@p5), " +
                "date_open=@p6, " +
                "date_accept=@p7, " +
                "date_close=@p8, " +
                "alert_id=(SELECT id_alert FROM hardware_alerts WHERE name=@p9) " +
            "WHERE id_record=@p1;", fbConnection);
            fbDataAdapter.UpdateCommand.Parameters.Add("@p1", FbDbType.Integer, 32, "ID_RECORD");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p2", FbDbType.Integer, 32, "CONC_DIRECTION");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p3", FbDbType.Integer, 32, "CONC_NUMBER");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p4", FbDbType.Integer, 32, "SENSOR_NUMBER");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p5", FbDbType.VarChar, 64, "ADDRESS_NAME");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p6", FbDbType.TimeStamp, 64, "DATE_OPEN");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p7", FbDbType.TimeStamp, 64, "DATE_ACCEPT");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p8", FbDbType.TimeStamp, 64, "DATE_CLOSE");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p9", FbDbType.VarChar, 64, "ALERT_NAME");
        }    
    }
}
