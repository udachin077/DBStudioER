using Base;
using Settings;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;

namespace Data.Firebird
{
    public partial class ASUDScadaFbController : Firebird, IDatabaseManagementSystem
    {
        private int counter;

        public ASUDScadaFbController() : base(ProgramName.ASUDScada)
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
            fbDataAdapter.Fill(ds, "journal_hardware");
            // Добавляет столбец с продолжительностью, если его не существует
            if (ds.Tables["journal_hardware"].Columns.Contains("prost_text") == false)
                ds.Tables["journal_hardware"].Columns.Add("prost_text", typeof(TimeSpan));
            // Считает продолжительность и добавляет в таблицу
            DateTime? date_close;
            var dc = DateTime.Now;
            foreach (var item in ds.Tables["journal_hardware"].Select())
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
                _fbDataAdapter.SelectCommand.CommandText = "SELECT id_tag, name FROM item_tags ORDER BY name ASC";
                _fbDataAdapter.Fill(dataSet, "item_tags");
                dataSet.WriteXml(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ASUDScadaDataSet.ds"), XmlWriteMode.WriteSchema);
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
                CommandText = "INSERT INTO journal_hardware (id_record, tag_id, address_id, date_open, date_accept, date_close, alert_id, deleted) " +
                "VALUES((SELECT GEN_ID(GEN_JOURNAL_HARDWARE_ALERTS_ID, 1) FROM RDB$DATABASE), @p1, @p2, @p3, @p4, @p5, @p6, 0)"
            };
            // Параметры команды втавки
            fbCommand.Parameters.Add("@p1", FbDbType.Integer, 32, "TAG_ID");
            fbCommand.Parameters.Add("@p2", FbDbType.Integer, 32, "ADDRESS_ID");
            fbCommand.Parameters.Add("@p3", FbDbType.TimeStamp, 64, "DATE_OPEN");
            fbCommand.Parameters.Add("@p4", FbDbType.TimeStamp, 64, "DATE_ACCEPT");
            fbCommand.Parameters.Add("@p5", FbDbType.TimeStamp, 64, "DATE_CLOSE");
            fbCommand.Parameters.Add("@p6", FbDbType.Integer, 32, "ALERT_ID");
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
                        fbCommand.Parameters["@p1"].Value = item["TAG_ID"];
                        fbCommand.Parameters["@p2"].Value = item["ADDRESS_ID"];
                        fbCommand.Parameters["@p3"].Value = item["DATE_OPEN"];
                        fbCommand.Parameters["@p4"].Value = item["DATE_ACCEPT"];
                        fbCommand.Parameters["@p5"].Value = item["DATE_CLOSE"];
                        fbCommand.Parameters["@p6"].Value = item["ALERT_ID"];
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
            fbConnectionStringBuilder.Database = AppSettings.String.GetValue(KeyName.ASUDScadaDatabase);
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
            fbDataAdapter.Update(ds, "journal_hardware");
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
            "DELETE FROM journal_hardware " +
            "WHERE id_record=@p1;", fbConnection);
            FbParameter fbDeleteParameter = fbDataAdapter.DeleteCommand.Parameters.Add("@p1", FbDbType.Integer, 32, "ID_RECORD");
            fbDeleteParameter.SourceVersion = DataRowVersion.Original;
        }
        /// <summary>
        /// Инициализация команды INSERT.
        /// </summary>
        private void InitializeDataAdapterInsertCommand()
        {
            // RETURNING применим начиная с версии Firebird 2.0
            // В WinAl используется по умолчанию Firebird 1.5
            // Возвращает id записи, который может быть использован для удаления или изменения строки
            // RETURNING id_record, alert_id
            fbDataAdapter.InsertCommand = new FbCommand(
            "INSERT INTO journal_hardware (id_record, tag_id, address_id, date_open, date_accept, date_close, alert_id, deleted) " +
            "VALUES" +
                "(" +
                    "(SELECT GEN_ID(GEN_JOURNAL_HARDWARE_ALERTS_ID, 1) FROM RDB$DATABASE), " +
                    "(SELECT id_tag FROM item_tags WHERE name=@p2), " +
                    "(SELECT id_address FROM address WHERE name=@p3), @p4, @p5, @p6, " +
                    "(SELECT id_alert FROM hardware_alerts WHERE name=@p7), 0" +
                ") RETURNING id_record", fbConnection);
            fbDataAdapter.InsertCommand.Parameters.Add("@p2", FbDbType.Integer, 32, "TAG_NAME");
            fbDataAdapter.InsertCommand.Parameters.Add("@p3", FbDbType.VarChar, 64, "ADDRESS_NAME");
            fbDataAdapter.InsertCommand.Parameters.Add("@p4", FbDbType.TimeStamp, 64, "DATE_OPEN");
            fbDataAdapter.InsertCommand.Parameters.Add("@p5", FbDbType.TimeStamp, 64, "DATE_ACCEPT");
            fbDataAdapter.InsertCommand.Parameters.Add("@p6", FbDbType.TimeStamp, 64, "DATE_CLOSE");
            fbDataAdapter.InsertCommand.Parameters.Add("@p7", FbDbType.VarChar, 64, "ALERT_NAME");
            FbParameter InsertParameter_1 = fbDataAdapter.InsertCommand.Parameters.Add("@p1", FbDbType.Integer, 32, "ID_RECORD");
            InsertParameter_1.Direction = ParameterDirection.Output;
            //FbParameter InsertParameter_2 = fbDataAdapter.InsertCommand.Parameters.Add("@p1_1", FbDbType.Integer, 32, "ALERT_ID");
            //InsertParameter_2.Direction = ParameterDirection.Output;
        }
        /// <summary>
        /// Инициализация команды SELECT.
        /// </summary>
        private void InitializeDataAdapterSelectCommand()
        {
            fbDataAdapter.SelectCommand = new FbCommand(
            "SELECT " +
                "jh.id_record," +
                "jh.date_open," +
                "jh.date_accept," +
                "jh.date_close," +
                "jh.alert_id," +
                "a.name as address_name," +
                "ha.name as alert_name," +
                "t.name as tag_name " +
            "FROM journal_hardware jh " +
            "JOIN address a ON a.id_address = jh.address_id " +
            "JOIN hardware_alerts ha ON ha.id_alert = jh.alert_id " +
            "JOIN item_tags t ON t.id_tag = jh.tag_id " +
            "ORDER BY jh.id_record ASC;", fbConnection);
        }
        /// <summary>
        /// Инициализация команды UPDATE.
        /// </summary>
        private void InitializeDataAdapterUpdateCommand()
        {
            fbDataAdapter.UpdateCommand = new FbCommand(
            "UPDATE journal_hardware " +
            "SET " +
                "tag_id=(SELECT id_tag FROM item_tags WHERE name=@p2), " +
                "address_id=(SELECT id_address FROM address WHERE name=@p3), " +
                "date_open=@p4, " +
                "date_accept=@p5, " +
                "date_close=@p6, " +
                "alert_id=(SELECT id_alert FROM hardware_alerts WHERE name=@p7) " +
            "WHERE id_record=@p1;", fbConnection);
            fbDataAdapter.UpdateCommand.Parameters.Add("@p1", FbDbType.Integer, 32, "ID_RECORD");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p2", FbDbType.Integer, 32, "TAG_NAME");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p5", FbDbType.VarChar, 64, "ADDRESS_NAME");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p6", FbDbType.TimeStamp, 64, "DATE_OPEN");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p7", FbDbType.TimeStamp, 64, "DATE_ACCEPT");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p8", FbDbType.TimeStamp, 64, "DATE_CLOSE");
            fbDataAdapter.UpdateCommand.Parameters.Add("@p9", FbDbType.VarChar, 64, "ALERT_NAME");
        }
    }
}
