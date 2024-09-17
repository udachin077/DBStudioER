using Application.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base
{
    public class JManagmentStudioEditPageViewModel : BaseViewModel
    {
        #region command
        private RelayCommand update;
        private RelayCommand connect;
        private RelayCommand addRow;
        private RelayCommand deleteRow;
        private RelayCommand drop;
        private RelayCommand applyFilter;
        private RelayCommand resetFilter;
        #endregion

        private IDatabaseManagementSystem databaseManagementSystem;

        #region table
        private DataTable mainTable;
        protected DataSet mainDataSet;
        private int tableCount;
        #endregion

        //private ErrorLevel errorLevel = (ErrorLevel)AppSettings.DWord.GetValue(KeyName.ErrorLevel, typeof(byte));
        protected void UpdateInformation(string message)
        {
            // Обновление информации
            Information.Clear();
            Information.Append(message);
            OnPropertyChanged(nameof(Information));
        }
        private System.Timers.Timer timer = new System.Timers.Timer
        {
            AutoReset = false,
            Interval = 3000
        };

        #region filter
        private bool allDays = true;
        private bool thisDay;
        private bool thisWeek;
        private bool thisMonth;
        private DateTime now = DateTime.Now;
        private DateTime? beginDate;
        private DateTime? endDate;
        private char atr;
        private List<ISensor> sensors;
        private ISensor selectedSensor;
        // Формат времени, имя столбца id контакта, имя столбца даты
        // "MM.dd.yyyy", "dat_tip", "dtn" АСУ ОДС
        // "dd.MM.yyyy", "door_id", "opentime" Горизонт
        // "dd.MM.yyyy", "ALERT_ID", "DATE_OPEN" WinAl
        private readonly string dateFormat, nameColumnSensorId, nameColumnDate, tableName;
        private StringBuilder filterBuilder = new StringBuilder();
        private bool isOperationProgress;
        private DateTime? customBeginDate;
        private DateTime? customEndDate;
        #endregion

        #region Constructors
        protected JManagmentStudioEditPageViewModel()
        {
            mainDataSet = new DataSet();
            timer.Elapsed += (sender, e) => { UpdateInformation(null); };
        }
        protected JManagmentStudioEditPageViewModel(string nameColumnSensorId, string nameColumnDate, string tableName, string dateFormat) : this()
        {
            this.tableName = tableName;
            this.dateFormat = dateFormat;
            atr = dateFormat == "MM.dd.yyyy" ? '#' : '\'';
            this.nameColumnSensorId = nameColumnSensorId;
            this.nameColumnDate = nameColumnDate;
        }
        public JManagmentStudioEditPageViewModel(string nameColumnSensorId, string nameColumnDate, string tableName, IDatabaseManagementSystem databaseManagementSystem, string dateFormat = "dd.MM.yyyy") : this(nameColumnSensorId, nameColumnDate, tableName, dateFormat)
        {
            this.databaseManagementSystem = databaseManagementSystem;
        }
        #endregion

        #region properties
        public DataTable MainTable { get => mainTable; set { mainTable = value; OnPropertyChanged(); } }
        public DataRowView SelectedRow { get; set; }
        public int TableCount { get => tableCount; set { tableCount = value; OnPropertyChanged(); } }
        public List<ISensor> Sensors { get => sensors; set { sensors = value; OnPropertyChanged(); } }
        public ISensor SelectedSensor { get => selectedSensor; set { selectedSensor = value; OnPropertyChanged(); } }
        public bool AllDays { get => allDays; set { allDays = value; if (value) { beginDate = null; endDate = null; } OnPropertyChanged(); } }
        public bool ThisDay { get => thisDay; set { thisDay = value; if (value) { beginDate = now; endDate = null; } } }
        public bool ThisWeek { get => thisWeek; set { thisWeek = value; if (value) { beginDate = now.StartOfWeek(DayOfWeek.Monday); endDate = now; } } }
        public bool ThisMonth { get => thisMonth; set { thisMonth = value; if (value) { beginDate = new DateTime(now.Year, now.Month, 1); endDate = now; } OnPropertyChanged(); } }
        public bool Custom { get; set; }
        public DateTime? CustomBeginDate { get => customBeginDate; set { customBeginDate = value; OnPropertyChanged(); } }
        public DateTime? CustomEndDate { get => customEndDate; set { customEndDate = value; OnPropertyChanged(); } }
        public bool IsOperationProgress { get => isOperationProgress; protected set { isOperationProgress = value; OnPropertyChanged(); } }
        public StringBuilder Information { get; set; } = new StringBuilder();
        #endregion

        // Обработка ошибки
        private void ErrorMessageExecute(string method, string message)
        {
            ErrorLevel errorLevel = (ErrorLevel)AppSettings.DWord.GetValue(KeyName.ErrorLevel, typeof(byte));
            if (errorLevel == ErrorLevel.Intermediate || errorLevel == ErrorLevel.Enabled) ErrorMessage.Show(message);
            if (errorLevel == ErrorLevel.Enabled) ErrorMessage.Write($"{method}. {message}");
        }

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Update => update ?? (update = new RelayCommand(async (obj) =>
        {
            IsOperationProgress = true;
            UpdateInformation("Обновление записей");
            try
            {
                await Task.Run(() => databaseManagementSystem.Update(ref mainDataSet));
                TableCount = mainTable.Rows.Count;
                IsOperationProgress = false;
                UpdateInformation("Обновление завершено");
            }
            catch (Exception ex)
            {
                IsOperationProgress = false;
                ErrorMessageExecute(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                UpdateInformation("Ошибка при обновлении записей");
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
        public RelayCommand Connect => connect ?? (connect = new RelayCommand(async (obj) =>
        {
            IsOperationProgress = true;
            UpdateInformation("Подключение к базе данных");
            try
            {
                await Task.Run(() => databaseManagementSystem.Connect(ref mainDataSet));
                FillSensorList();
                MainTable = mainDataSet.Tables[tableName];
                TableCount = mainTable.Rows.Count;
                SelectedSensor = Sensors[0];
                IsOperationProgress = false;
                UpdateInformation("Подключение выполнено");
            }
            catch (Exception ex)
            {
                IsOperationProgress = false;
                ErrorMessageExecute(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                UpdateInformation("Ошибка подключения к базе данных");
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
        public RelayCommand Drop => drop ?? (drop = new RelayCommand(async (obj) =>
        {
            if (!WarningMessage.Show("Все данные из таблицы будут удалены.\nПродолжить?"))
                return;

            IsOperationProgress = true;
            UpdateInformation("Очистка журнала событий");
            try
            {
                await Task.Run(() => databaseManagementSystem.Drop(ref mainDataSet));
                TableCount = mainTable.Rows.Count;
                IsOperationProgress = false;
                UpdateInformation("Журнал событий очищен");
            }
            catch (Exception ex)
            {
                IsOperationProgress = false;
                ErrorMessageExecute(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                UpdateInformation("Ошибка подключения к базе данных");
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
        public RelayCommand Add => addRow ?? (addRow = new RelayCommand((obj) =>
        {
            DataRow row = MainTable.NewRow();
            MainTable.Rows.Add(row);
        }, obj => mainTable != null));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Delete => deleteRow ?? (deleteRow = new RelayCommand((obj) =>
        {
            DataRow dataRow = SelectedRow?.Row;
            dataRow?.Delete();
        }, obj => SelectedRow != null));

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
        public RelayCommand ResetFilter => resetFilter ?? (resetFilter = new RelayCommand((obj) =>
        {
            FilterReset();
            MainTable.DefaultView.RowFilter = string.Empty;
        }));

        private void FilterBuild()
        {
            if (Custom)
            {
                if (customBeginDate == null || customEndDate == null)
                {
                    if (WarningMessage.Show("Интервал времени не задан."))
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

        protected virtual void FillSensorList()
        {

        }
    }
}