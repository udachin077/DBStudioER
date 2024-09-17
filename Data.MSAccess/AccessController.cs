using Base;
using Settings;
using System;
using System.Data;
using System.Data.OleDb;

namespace Data.Access
{
    public partial class AccessController : Access, IDatabaseManagementSystem
    {
        // Команда, используемая для добавления открытия
        private readonly string CommandTextOpen = "INSERT INTO `DoorEvents` (`opentime`, `door_id`, `isopen`, `create_time`, `record_time`, `create_operator`) VALUES (?, ?, -1, ?, ?, ?)";
        // Команда, используемая для добавления закрытия
        private readonly string CommandTextClose = "INSERT INTO `DoorEvents` (`opentime`, `door_id`, `isopen`, `create_time`, `record_time`, `create_operator`) VALUES (?, ?, 0, ?, ?, ?)";
        private int counter;

        public AccessController() : base()
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
            bool key = AppSettings.DWord.GetValue(KeyName.PKey, typeof(bool));
            if (key == false)
            {
                SetPrimaryKeyDoorsEvent();
            }
            ds.Clear();
            oleDbDataAdapter.Fill(ds, "DoorEvents");
            // Получение дополнительных таблиц
            OleDbDataAdapter _dataAdapter = new OleDbDataAdapter();
            _dataAdapter.SelectCommand = new OleDbCommand("SELECT `id`, `address`, `name`, `term_id` FROM `Doors` ORDER BY address, name ASC", oleDbConnection);
            _dataAdapter.Fill(ds, "Doors");
            _dataAdapter?.Dispose();
        }

        public void CreateDataSet()
        {
            DataSet dataSet = new DataSet();
            OleDbDataAdapter _oleDbDataAdapter = new OleDbDataAdapter
            {
                SelectCommand = new OleDbCommand("SELECT `id`, `address`, `name`, `term_id` FROM `Doors` ORDER BY address, name ASC", oleDbConnection)
            };
            try
            {
                _oleDbDataAdapter.Fill(dataSet, "Doors");
                dataSet.WriteXml(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "HorizonDataSet.ds"), XmlWriteMode.WriteSchema);
            }
            finally
            {
                _oleDbDataAdapter?.Dispose();
            }
        }

        public void Merge(ref DataTable journal)
        {
            OleDbCommand oleDbCommand = new OleDbCommand
            {
                Connection = oleDbConnection
            };
            // Параметры команды вcтавки
            oleDbCommand.Parameters.Add(new OleDbParameter("opentime", OleDbType.Date, 0));
            oleDbCommand.Parameters.Add(new OleDbParameter("door_id", OleDbType.Integer, 0));
            oleDbCommand.Parameters.Add(new OleDbParameter("create_time", OleDbType.Date, 0));
            oleDbCommand.Parameters.Add(new OleDbParameter("record_time", OleDbType.Date, 0));
            oleDbCommand.Parameters.Add(new OleDbParameter("create_operator", OleDbType.VarWChar, 50));
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
                        oleDbCommand.Parameters["door_id"].Value = item["door_id"];
                        oleDbCommand.Parameters["create_operator"].Value = item["create_operator"];

                        oleDbCommand.CommandText = CommandTextOpen;
                        oleDbCommand.Parameters["opentime"].Value = item["opentime"];
                        oleDbCommand.Parameters["create_time"].Value = item["opentime"];
                        oleDbCommand.Parameters["record_time"].Value = item["opentime"];
                        oleDbCommand.ExecuteNonQuery();

                        oleDbCommand.CommandText = CommandTextClose;
                        oleDbCommand.Parameters["opentime"].Value = item["closetime"];
                        oleDbCommand.Parameters["create_time"].Value = item["closetime"];
                        oleDbCommand.Parameters["record_time"].Value = item["closetime"];
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
                // Применение изменений таблицы (удаление добавленных строк)
                journal.AcceptChanges();
                // Закрытие подключения
                oleDbConnection?.Close();
                oleDbCommand?.Dispose();
            }
        }

        public void QueryExecute(string query)
        {
            OleDbCommand oleDbCommand = new OleDbCommand
            {
                Connection = oleDbConnection
            };
            oleDbCommand.CommandText = query;
            try
            {
                // Открытие подключения
                oleDbConnection?.Open();
                oleDbCommand.ExecuteNonQuery();
            }
            finally
            {
                // Закрытие подключения
                oleDbConnection?.Close();
                oleDbCommand?.Dispose();
            }
        }

        public void Update(ref DataSet ds)
        {
            counter = 0;
            oleDbDataAdapter.Update(ds, "DoorEvents");
        }

        private void SetPrimaryKeyDoorsEvent()
        {
            OleDbCommand oleDbCommand = new OleDbCommand();
            oleDbCommand.Connection = oleDbConnection;
            try
            {
                oleDbCommand.CommandText = "ALTER TABLE DoorEvents ADD COLUMN Id COUNTER(1,1)";
                oleDbConnection?.Open();
                oleDbCommand.ExecuteNonQuery();
                oleDbCommand.CommandText = "ALTER TABLE DoorEvents ADD PRIMARY KEY (Id)";
                oleDbCommand.ExecuteNonQuery();
                AppSettings.DWord.SetValue(KeyName.PKey, true);
            }
            catch (Exception ex)
            {
                // Возможно ключ уже установлен
                Console.WriteLine(ex.Message);
            }
            finally
            {
                oleDbCommand?.Dispose();
                oleDbConnection?.Close();
            }
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
                    "`DoorEvents` " +
                "WHERE " +
                    "`id`=?", oleDbConnection); 
            oleDbDataAdapter.DeleteCommand.Parameters.Add(new OleDbParameter("id", OleDbType.BigInt, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "id", DataRowVersion.Original, false, null));
            // `opentime`=? AND `door_id`=? AND `isopen`=? AND `create_time`=? AND `record_time`=? AND `create_operator`=?"
            //oleDbDataAdapter.DeleteCommand.Parameters.Add(new OleDbParameter("opentime", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "opentime", DataRowVersion.Original, false, null));
            //oleDbDataAdapter.DeleteCommand.Parameters.Add(new OleDbParameter("door_id", OleDbType.Integer, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "door_id", DataRowVersion.Original, false, null));
            //oleDbDataAdapter.DeleteCommand.Parameters.Add(new OleDbParameter("isopen", OleDbType.Boolean, 2, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "isopen", DataRowVersion.Original, false, null));
            //oleDbDataAdapter.DeleteCommand.Parameters.Add(new OleDbParameter("create_time", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "create_time", DataRowVersion.Original, false, null));
            //oleDbDataAdapter.DeleteCommand.Parameters.Add(new OleDbParameter("record_time", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "record_time", DataRowVersion.Original, false, null));
            //oleDbDataAdapter.DeleteCommand.Parameters.Add(new OleDbParameter("create_operator", OleDbType.VarWChar, 50, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "create_operator", DataRowVersion.Original, false, null));
        }
        /// <summary>
        /// Инициализация команды INSERT.
        /// </summary>
        private void InitializeDataAdapterInsertCommand()
        {
            oleDbDataAdapter.InsertCommand = new OleDbCommand(
                "INSERT INTO " +
                    "`DoorEvents` (`opentime`, `door_id`, `isopen`, `create_time`, `record_time`, `create_operator`) " +
                "VALUES(?, ?, ?, ?, ?, ?)", oleDbConnection);
            oleDbDataAdapter.InsertCommand.Parameters.Add(new OleDbParameter("opentime", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "opentime", DataRowVersion.Current, false, null));
            oleDbDataAdapter.InsertCommand.Parameters.Add(new OleDbParameter("door_id", OleDbType.Integer, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "door_id", DataRowVersion.Current, false, null));
            oleDbDataAdapter.InsertCommand.Parameters.Add(new OleDbParameter("isopen", OleDbType.Boolean, 2, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "isopen", DataRowVersion.Current, false, null));
            oleDbDataAdapter.InsertCommand.Parameters.Add(new OleDbParameter("create_time", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "create_time", DataRowVersion.Current, false, null));
            oleDbDataAdapter.InsertCommand.Parameters.Add(new OleDbParameter("record_time", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "record_time", DataRowVersion.Current, false, null));
            oleDbDataAdapter.InsertCommand.Parameters.Add(new OleDbParameter("create_operator", OleDbType.VarWChar, 50, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "create_operator", DataRowVersion.Current, false, null));
        }
        /// <summary>
        /// Инициализация команды SELECT.
        /// </summary>
        private void InitializeDataAdapterSelectCommand()
        {
            oleDbDataAdapter.SelectCommand = new OleDbCommand(
                "SELECT " +
                    "Doors.address AS address, " +
                    "Doors.name AS sensor," +
                    "Terms.name AS term_name," +
                    "Terms.address AS term_address, " +
                    "DoorEvents.id," +
                    "DoorEvents.opentime, " +
                    "DoorEvents.door_id, " +
                    "DoorEvents.isopen, " +
                    "DoorEvents.create_time, " +
                    "DoorEvents.record_time, " +
                    "DoorEvents.create_operator " +
                "FROM (" +
                    "(`DoorEvents` " +
                        "LEFT OUTER JOIN " +
                            "`Doors` " +
                        "ON " +
                            "DoorEvents.door_id = Doors.id) " +
                        "LEFT OUTER JOIN " +
                            "`Terms` " +
                        "ON " +
                            "Doors.term_id = Terms.id)", oleDbConnection);
        }
        /// <summary>
        /// Инициализация команды UPDATE.
        /// </summary>
        private void InitializeDataAdapterUpdateCommand()
        {
            oleDbDataAdapter.UpdateCommand = new OleDbCommand(
                "UPDATE " +
                    "`DoorEvents` " +
                "SET " +
                    "`opentime`=?, " +
                    "`door_id`=?,  " +
                    "`isopen`=?, " +
                    "`create_time`=?, " +
                    "`record_time`=?, " +
                    "`create_operator`=? " +
                "WHERE " +
                    "`id`=?", oleDbConnection);
            oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("opentime", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "opentime", DataRowVersion.Current, false, null));
            oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("door_id", OleDbType.Integer, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "door_id", DataRowVersion.Current, false, null));
            oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("isopen", OleDbType.Boolean, 2, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "isopen", DataRowVersion.Current, false, null));
            oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("create_time", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "create_time", DataRowVersion.Current, false, null));
            oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("record_time", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "record_time", DataRowVersion.Current, false, null));
            oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("create_operator", OleDbType.VarWChar, 50, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "create_operator", DataRowVersion.Current, false, null));
            oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("id", OleDbType.BigInt, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "id", DataRowVersion.Original, false, null));
            //+`opentime`=? AND `door_id`=? AND `isopen`=? AND `create_time`=? AND `record_time`=? AND `create_operator`=?"
            //oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("old_opentime", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "opentime", DataRowVersion.Original, false, null));
            //oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("old_door_id", OleDbType.Integer, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "door_id", DataRowVersion.Original, false, null));
            //oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("old_isopen", OleDbType.Boolean, 2, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "isopen", DataRowVersion.Original, false, null));
            //oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("old_create_time", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "create_time", DataRowVersion.Original, false, null));
            //oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("old_record_time", OleDbType.Date, 0, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "record_time", DataRowVersion.Original, false, null));
            //oleDbDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("old_create_operator", OleDbType.VarWChar, 50, ParameterDirection.Input, ((byte)(0)), ((byte)(0)), "create_operator", DataRowVersion.Original, false, null));
        }


    }
}
