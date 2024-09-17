using Base;
using JournalManagmentStudio.Project.Services;
using System;
using System.ComponentModel;
using System.Data;

namespace JournalManagmentStudio.Project.ViewModel
{
    public class ProjectTableViewModel : BaseViewModel, IDataErrorInfo
    {
        private RelayCommand addRow;
        private RelayCommand deleteRow;
        private RelayCommand drop;
        private RelayCommand accept;
        private DataTable mainTable;
        protected DataSet mainDataSet;
        private int tableCount;
        private string tableFileFullName;
        private string dataSetFileFullName;

        public DataTable MainTable { get => mainTable; set { mainTable = value; OnPropertyChanged(); } }
        public DataRowView SelectedRow { get; set; }
        public int TableCount { get => tableCount; set { tableCount = value; OnPropertyChanged(); } }

        public DateTime Date { get; set; } = DateTime.Now;
        public string TimeOpen { get; set; } = "00:00";
        public string TimeClose { get; set; } = "00:00";
        protected Random random = new Random();

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(TimeOpen):
                        if (TimeOpen != null && !System.Text.RegularExpressions.Regex.IsMatch(TimeOpen, @"^(([0,1][0-9])|(2[0-3])):[0-5][0-9]$", System.Text.RegularExpressions.RegexOptions.Compiled))
                            return "Неверный формат";
                        break;
                    case nameof(TimeClose):
                        if (TimeClose != null && !System.Text.RegularExpressions.Regex.IsMatch(TimeClose, @"^(([0,1][0-9])|(2[0-3])):[0-5][0-9]$", System.Text.RegularExpressions.RegexOptions.Compiled))
                            return "Неверный формат";
                        break;
                }
                return string.Empty;
            }
        }

        #region Constructors
        public ProjectTableViewModel()
        {
            mainDataSet = new DataSet();
            mainTable = new DataTable();
        }

        public ProjectTableViewModel(ProjectParameter parameter) : this()
        {
            tableFileFullName = parameter.PathTable;
            dataSetFileFullName = parameter.PathDataSet;

            MainTable.ReadXml(tableFileFullName);
            TableCount = MainTable.Rows.Count;
            if (string.IsNullOrEmpty(dataSetFileFullName) == false)
                mainDataSet.ReadXml(dataSetFileFullName);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Add => addRow ?? (addRow = new RelayCommand((obj) =>
        {
            DataRow row = BuildNewDataRow();
            MainTable.Rows.Add(row);
            TableCount = mainTable.Rows.Count;
            mainTable.WriteXml(tableFileFullName, XmlWriteMode.WriteSchema);
        }, obj => mainTable != null));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Delete => deleteRow ?? (deleteRow = new RelayCommand((obj) =>
        {
            DataRow dataRow = SelectedRow?.Row;
            dataRow?.Delete();
            MainTable.AcceptChanges();
            TableCount = mainTable.Rows.Count;
            mainTable.WriteXml(tableFileFullName, XmlWriteMode.WriteSchema);
        }, obj => SelectedRow != null));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Drop => drop ?? (drop = new RelayCommand((obj) =>
        {
            if (WarningMessage.Show("Все данные из таблицы будут удалены.\nПродолжить?") == false)
                return;

            MainTable.Clear();
            MainTable.AcceptChanges();
            TableCount = mainTable.Rows.Count;
            mainTable.WriteXml(tableFileFullName, XmlWriteMode.WriteSchema);
        }, obj => mainTable != null));

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand Accept => accept ?? (accept = new RelayCommand((obj) =>
        {
            MainTable.AcceptChanges();
            TableCount = mainTable.Rows.Count;
            mainTable.WriteXml(tableFileFullName, XmlWriteMode.WriteSchema);
        }, obj => mainTable != null));

        public string Error => throw new NotImplementedException();

        protected virtual DataRow BuildNewDataRow()
        {
            return MainTable.NewRow(); ;
        }
    }
}
