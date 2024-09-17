using Base;
using Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace JournalManagmentStudio.Journal.ViewModel
{
    internal class OnlineJournalViewModel : BaseViewModel
    {
        private System.Media.SoundPlayer successful = new System.Media.SoundPlayer(Properties.Resources.sound_5);
        private RelayCommand addRow;
        private RelayCommand applyFilter;
        private RelayCommand deleteRow;
        private RelayCommand drop;
        private RelayCommand connect;
        private RelayCommand createDataSet;
        private RelayCommand customQueryExecute;
        private RelayCommand resetFilter;
        private RelayCommand update;
        private RelayCommand merge;
        private IDatabaseManagementSystem databaseManagementSystem;
        private DataTable mainTable;
        protected DataSet mainDataSet;
        private int tableCount;
        protected void UpdateInformationMessage(string message)
        {
            // Обновление информации
            InformationMessage.Clear();
            InformationMessage.Append(message);
            OnPropertyChanged(nameof(InformationMessage));
        }
        protected void UpdateAdditionallyMessage(string message)
        {
            // Обновление дополнительной информации
            AdditionallyMessage.Clear();
            AdditionallyMessage.Append(message);
            OnPropertyChanged(nameof(AdditionallyMessage));
        }
        private System.Timers.Timer timer = new System.Timers.Timer
        {
            AutoReset = false,
            Interval = 3000
        };
        private bool allDays = true;
        private bool thisDay;
        private bool thisWeek;
        private bool thisMonth;
        private DateTime now = DateTime.Now;
        private DateTime? beginDate;
        private DateTime? endDate;
        private char atr;
        private List<Sensor> sensors;
        private Sensor selectedSensor;
        // Формат времени, имя столбца id контакта, имя столбца даты
        // "MM.dd.yyyy", "dat_tip", "dtn" АСУ ОДС
        // "dd.MM.yyyy", "door_id", "opentime" Горизонт
        // "dd.MM.yyyy", "ALERT_ID", "DATE_OPEN" WinAl
        private readonly string dateFormat, nameColumnSensorId, nameColumnDate, tableName;
        private StringBuilder filterBuilder = new StringBuilder();
        private bool isOperationProgress;
        private DateTime? customBeginDate;
        private DateTime? customEndDate;
        private Program program;
        private bool showWarningBeforAddNewRow = true;
        private int counterRowDeleted;
        private bool lockFlag;

        #region Constructors
        protected OnlineJournalViewModel()
        {
            mainDataSet = new DataSet();
            timer.Elapsed += (sender, e) => { UpdateInformationMessage(null); };
        }
        protected OnlineJournalViewModel(string nameColumnSensorId, string nameColumnDate, string tableName, string dateFormat) : this()
        {
            this.tableName = tableName;
            this.dateFormat = dateFormat;
            atr = dateFormat == "MM.dd.yyyy" ? '#' : '\'';
            this.nameColumnSensorId = nameColumnSensorId;
            this.nameColumnDate = nameColumnDate;
        }
        public OnlineJournalViewModel(string nameColumnSensorId, string nameColumnDate, string tableName, IDatabaseManagementSystem databaseManagementSystem, string dateFormat = "dd.MM.yyyy") : this(nameColumnSensorId, nameColumnDate, tableName, dateFormat)
        {
            this.databaseManagementSystem = databaseManagementSystem;
        }
        #endregion

        #region properties
        public Program Program { get => program; set { program = value; OnPropertyChanged(); } }
        public DataTable MainTable { get => mainTable; set { mainTable = value; OnPropertyChanged(); } }
        public DataRowView SelectedRow { get; set; }
        public int TableCount { get => tableCount; set { tableCount = value; OnPropertyChanged(); } }
        public List<Sensor> Sensors { get => sensors; set { sensors = value; OnPropertyChanged(); } }
        public Sensor SelectedSensor { get => selectedSensor; set { selectedSensor = value; OnPropertyChanged(); } }
        public bool AllDays { get => allDays; set { allDays = value; if (value) { beginDate = null; endDate = null; } OnPropertyChanged(); } }
        public bool ThisDay { get => thisDay; set { thisDay = value; if (value) { beginDate = now; endDate = null; } } }
        public bool ThisWeek { get => thisWeek; set { thisWeek = value; if (value) { beginDate = now.StartOfWeek(DayOfWeek.Monday); endDate = now; } } }
        public bool ThisMonth { get => thisMonth; set { thisMonth = value; if (value) { beginDate = new DateTime(now.Year, now.Month, 1); endDate = now; } OnPropertyChanged(); } }
        public bool Custom { get; set; }
        public DateTime? CustomBeginDate { get => customBeginDate; set { customBeginDate = value; OnPropertyChanged(); } }
        public DateTime? CustomEndDate { get => customEndDate; set { customEndDate = value; OnPropertyChanged(); } }
        public bool IsOperationProgress { get => isOperationProgress; protected set { isOperationProgress = value; OnPropertyChanged(); } }
        public StringBuilder InformationMessage { get; set; } = new StringBuilder();
        public StringBuilder AdditionallyMessage { get; set; } = new StringBuilder();
        public string QueryText { get; set; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Add => addRow ?? (addRow = new RelayCommand((obj) =>
        {
            if (showWarningBeforAddNewRow == true)
            {
                if (WarningMessage.Show("Добавление записей данным способом не рекомендуется.\nПродолжить?") == false)
                    return;
                showWarningBeforAddNewRow = false;
            }
            DataRow row = MainTable.NewRow();
            MainTable.Rows.Add(row);
        }, obj => mainTable != null));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand ApplyFilter => applyFilter ?? (applyFilter = new RelayCommand((obj) =>
        {
            FilterBuild();
            MainTable.DefaultView.RowFilter = filterBuilder.ToString();
        }));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Connect => connect ?? (connect = new RelayCommand(async (obj) =>
        {
            IsOperationProgress = true;
            UpdateInformationMessage("Получение данных");
            try
            {
                await Task.Run(() => databaseManagementSystem.Connect(ref mainDataSet));
                //
                UpdateInformationMessage("Обновление параметров");
                await FillSensorList;
                //
                successful.Play();
                MainTable = mainDataSet.Tables[tableName];

                // При повторном нажатии подключении обработчик добавлен не будет
                if (lockFlag == false)
                {
                    mainTable.RowDeleted += (sender, e) => ++counterRowDeleted;
                    lockFlag = true;
                }

                TableCount = mainTable.Rows.Count;
                SelectedSensor = Sensors[0];
                IsOperationProgress = false;
                UpdateInformationMessage("Готово");
            }
            catch (Exception ex)
            {
                IsOperationProgress = false;
                ErrorMessage.Show(ex.Message);
                UpdateInformationMessage("Ошибка подключения к базе данных");
            }
            finally
            {
                timer.Stop();
                timer.Start();
            }
        }));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand CreateDataSet => createDataSet ?? (createDataSet = new RelayCommand(async (obj) =>
        {
            IsOperationProgress = true;
            UpdateInformationMessage("Создание переносимой библиотеки");
            try
            {
                await Task.Run(() => databaseManagementSystem.CreateDataSet());
                IsOperationProgress = false;
                UpdateInformationMessage($"Файл сохранен на Рабочий стол");
            }
            catch (Exception ex)
            {
                IsOperationProgress = false;
                ErrorMessage.Show(ex.Message);
                UpdateInformationMessage("Ошибка при создании переносимой библиотеки");
            }
            finally
            {
                timer.Stop();
                timer.Start();
            }
        }, obj => tableName != nameof(TableName.Protdisp)));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand CustomQueryExecute => customQueryExecute ?? (customQueryExecute = new RelayCommand(async (obj) =>
        {
            await Task.Run(() =>
            {
                databaseManagementSystem.QueryExecute(QueryText);
            });
        }));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Delete => deleteRow ?? (deleteRow = new RelayCommand((obj) =>
        {
            SelectedRow?.Row.Delete();
        }, obj => SelectedRow != null));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Drop => drop ?? (drop = new RelayCommand(async (obj) =>
        {
            if (false == WarningMessage.Show("При большом количестве удаляемых записей операция может занять длительное время.\nПродолжить?"))
                return;

            IsOperationProgress = true;
            UpdateInformationMessage("Подготовка к удалению");
            var _collection = MainTable.Select($"{nameColumnDate}<{atr}{DateTime.Now.Date.AddYears(-1).ToString(dateFormat)}{atr}");
            int _allRows = _collection.Length;
            databaseManagementSystem.CounterChanged += (e) => {
                UpdateAdditionallyMessage($"{e} из {_allRows}");
            };
            await Task.Run(() => {
                foreach (var row in _collection)
                {
                    row.Delete();
                }
            });
            try
            {
                UpdateInformationMessage("Удаление записей");
                await Task.Run(() => databaseManagementSystem.Update(ref mainDataSet));
                successful.Play();
                mainDataSet.AcceptChanges();
                TableCount = mainTable.Rows.Count;
                IsOperationProgress = false;
                UpdateInformationMessage("Журнал событий очищен");
            }
            catch (Exception ex)
            {
                IsOperationProgress = false;
                ErrorMessage.Show(ex.Message);
                UpdateInformationMessage("Ошибка при очистке журнала");
            }
            finally
            {
                databaseManagementSystem.CounterChanged += (e) => {
                    UpdateAdditionallyMessage($"{e} из {_allRows}");
                };
                UpdateAdditionallyMessage(null);
                timer.Stop();
                timer.Start();
            }
        }, obj => mainTable != null));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand ResetFilter => resetFilter ?? (resetFilter = new RelayCommand((obj) =>
        {
            FilterReset();
            MainTable.DefaultView.RowFilter = string.Empty;
        }));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Update => update ?? (update = new RelayCommand(async (obj) =>
        {
            IsOperationProgress = true;
            UpdateInformationMessage("Обновление");
            try
            {
                await Task.Run(() => databaseManagementSystem.Update(ref mainDataSet));
                TableCount = mainTable.Rows.Count;
                IsOperationProgress = false;
                UpdateInformationMessage("Обновление завершено");
            }
            catch (Exception ex)
            {
                IsOperationProgress = false;
                ErrorMessage.Show(ex.Message);
                UpdateInformationMessage("Ошибка при обновлении");
            }
            finally
            {
                timer.Stop();
                timer.Start();
            }
        }, obj => mainTable != null));

        /// <summary>
        ///  
        /// </summary>
        public RelayCommand Merge => merge ?? (merge = new RelayCommand(async (obj) =>
        {
            BrowserDialog browserDialog = new BrowserDialog();
            browserDialog.OpenFileTitle = "Выбор файла таблицы";
            browserDialog.OpenFileFilter = $"Файл таблицы (*.{nameof(FileExtension.dt)})|*.{nameof(FileExtension.dt)}";
            string _value = browserDialog.OpenFile();
            if (_value == null)
                return;

            IsOperationProgress = true;
            UpdateInformationMessage("Добавление записей");
            DataTable _dt = new DataTable();
            try
            {
                _dt.ReadXml(_value);
                int count = _dt.Rows.Count;
                int added = 0;
                await Task.Run(() =>
                {
                    databaseManagementSystem.CounterChanged += (e) => { 
                        UpdateAdditionallyMessage($"{e} из {count}");
                        added = e;
                    };
                    databaseManagementSystem.Merge(ref _dt);
                    databaseManagementSystem.CounterChanged -= (e) => {
                        UpdateAdditionallyMessage($"{e} из {count}");
                    };
                });
                IsOperationProgress = false;
                UpdateInformationMessage($"Добавление записей завершено {added}/{count}");
            }
            catch (Exception ex)
            {
                IsOperationProgress = false;
                ErrorMessage.Show(ex.Message);
                UpdateInformationMessage("Ошибка при добавлении записей");
            }
            finally
            {
                UpdateAdditionallyMessage(null);
                _dt.WriteXml(_value, XmlWriteMode.WriteSchema);
                timer.Stop();
                timer.Start();
            }
        }));

        private void FilterBuild()
        {
            if (Custom == true)
            {
                if (customBeginDate == null || customEndDate == null)
                {
                    ErrorMessage.Show("Интервал времени не задан.");
                    return;
                }

                beginDate = customBeginDate;
                endDate = customEndDate;
            }

            filterBuilder.Clear();
            // Id сигнала
            int idSensor = selectedSensor == null ? 0 : selectedSensor.Id;
            // Время открытия
            string begin_Date = beginDate?.Date.ToString(dateFormat);
            // Время закрытия
            string end_Date = endDate?.AddDays(1).Date.ToString(dateFormat);
            // Выбран только контакт
            if (idSensor != 0 && begin_Date == null && end_Date == null)
            {
                filterBuilder.Append($"{nameColumnSensorId}={idSensor}");
            }
            // Выбран контакт и время открытия
            else if (idSensor != 0 && begin_Date != null && end_Date == null)
            {
                filterBuilder.Append($"{nameColumnSensorId}={idSensor} AND {nameColumnDate}>={atr}{begin_Date}{atr}");
            }
            // Выбран контакт и время закрытия
            else if (idSensor != 0 && begin_Date == null && end_Date != null)
            {
                filterBuilder.Append($"{nameColumnSensorId}={idSensor} AND {nameColumnDate}<={atr}{end_Date}{atr}");
            }
            // Выбран контакт, время открытия и время закрытия
            else if (idSensor != 0 && begin_Date != null && end_Date != null)
            {
                filterBuilder.Append($"{nameColumnSensorId}={idSensor} AND ({nameColumnDate}>={atr}{begin_Date}{atr} AND {nameColumnDate}<={atr}{end_Date}{atr})");
            }
            // Выбрано время открытия и время закрытия
            else if (idSensor == 0 && begin_Date != null && end_Date != null)
            {
                filterBuilder.Append($"{nameColumnDate}>={atr}{begin_Date}{atr} AND {nameColumnDate}<={atr}{end_Date}{atr}");
            }
            // Выбрано только время открытия
            else if (idSensor == 0 && begin_Date != null && end_Date == null)
            {
                filterBuilder.Append($"{nameColumnDate}>={atr}{begin_Date}{atr}");
            }
            // Выбрано только время закрытия
            else if (idSensor == 0 && begin_Date == null && end_Date != null)
            {
                filterBuilder.Append($"{nameColumnDate}<={atr}{end_Date}{atr}");
            }
        }

        private void FilterReset()
        {
            SelectedSensor = Sensors[0];
            AllDays = true;
            beginDate = null;
            endDate = null;
        }

        // Заполнение листов контактов, адресов...
        protected virtual Task FillSensorList => Task.Run(() => { });
    }
}
