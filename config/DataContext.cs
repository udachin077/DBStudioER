using Base;
using Settings;
using System;
using System.IO;

namespace config
{
    internal class DataContext : BaseViewModel
    {
        private RelayCommand applySettings;
        private RelayCommand defaultSettings;
        private RelayCommand selectFileDialog;
        private RelayCommand selectFolderDialog;

        private BrowserDialog browserDialog;
        private string winAlDatabasePath;
        private string horizonDatabasePath;
        private string projectFolderPath;
        private string asudScadaDatabasePath;
        private ProgramName programName;
        private DbMS dbms;
        #region Parameters
        public bool WinAl { get { return programName == ProgramName.WinAl; } set { if (value) { dbms = DbMS.Firebird; programName = ProgramName.WinAl; } OnPropertyChanged(); } }
        public bool Horizon { get { return programName == ProgramName.Horizon; } set { if (value) { dbms = DbMS.MSAccess; programName = ProgramName.Horizon; } OnPropertyChanged(); } }
        public bool AsuOdsMsde { get { return programName == ProgramName.AsuOdsMsde; } set { if (value) { dbms = DbMS.Msde; programName = ProgramName.AsuOdsMsde; } OnPropertyChanged(); } }
        public bool ASUDScada { get { return programName == ProgramName.ASUDScada; } set { if (value) { dbms = DbMS.Firebird; programName = ProgramName.ASUDScada; } OnPropertyChanged(); } }
        public bool AsuOdsSqlServer { get { return programName == ProgramName.AsuOdsSqlServer; } set { if (value) { dbms = DbMS.SqlServer; programName = ProgramName.AsuOdsSqlServer; } OnPropertyChanged(); } }
        public bool AutoConnect { get; set; }
        public string WinAlDatabasePath { get => winAlDatabasePath; set { winAlDatabasePath = value; OnPropertyChanged(); } }
        public string HorizonDatabasePath { get => horizonDatabasePath; set { horizonDatabasePath = value; OnPropertyChanged(); } }
        public string ASUDScadaDatabasePath { get => asudScadaDatabasePath; set { asudScadaDatabasePath = value; OnPropertyChanged(); } }
        public string ProjectFolderPath { get => projectFolderPath; set { projectFolderPath = value; OnPropertyChanged(); } }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public DataContext()
        {
            AutoConnect = AppSettings.DWord.GetValue(KeyName.AutoConnect, typeof(bool));
            dbms = (DbMS)AppSettings.DWord.GetValue(KeyName.DbMS, typeof(byte));
            programName = (ProgramName)AppSettings.DWord.GetValue(KeyName.Program, typeof(byte));
            try
            {
                WinAlDatabasePath = AppSettings.String.GetValue(KeyName.WinAlDatabase);
                HorizonDatabasePath = AppSettings.String.GetValue(KeyName.HorizonDatabase);
                ASUDScadaDatabasePath = AppSettings.String.GetValue(KeyName.ASUDScadaDatabase);
                //ProjectFolderPath = AppSettings.String.GetValue(KeyName.ProjectFolderPath);
            }
            catch
            {
                DefaultSettings.Execute(null);
                ApplySettings.Execute(null);
            }
        }

        /// <summary>
        /// Применить настройки.
        /// </summary>
        public RelayCommand ApplySettings => applySettings ?? (applySettings = new RelayCommand((obj) =>
        {
            AppSettings.DWord.SetValue(KeyName.DbMS, dbms);
            AppSettings.DWord.SetValue(KeyName.Program, programName);
            AppSettings.DWord.SetValue(KeyName.AutoConnect, AutoConnect);
            AppSettings.String.SetValue(KeyName.HorizonDatabase, horizonDatabasePath);
            AppSettings.String.SetValue(KeyName.WinAlDatabase, winAlDatabasePath);
            AppSettings.String.SetValue(KeyName.ASUDScadaDatabase, asudScadaDatabasePath);
            //AppSettings.String.SetValue(KeyName.ProjectFolder, projectFolderPath);
        }));

        /// <summary>
        /// Установка путей по умолчанию.
        /// </summary>
        public RelayCommand DefaultSettings => defaultSettings ?? (defaultSettings = new RelayCommand((obj) =>
        {
            WinAlDatabasePath = Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), @"1Alarm\journal.db");
            HorizonDatabasePath = Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), @"MIS\Horizont\Data\DnKElv.mdb");
            ASUDScadaDatabasePath = Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), @"1Tekon\ASUD Scada\SCADA\journal.db");
            //ProjectFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Journal Studio\Project");
        }));

        /// <summary>
        /// Открыть окно выбора файла.
        /// </summary>
        public RelayCommand SelectFileDialog => selectFileDialog ?? (selectFileDialog = new RelayCommand((obj) =>
        {
            browserDialog = browserDialog ?? new BrowserDialog();
            browserDialog.OpenFileTitle = "Расположение базы данных";
            if (Equals(obj, nameof(ProgramName.WinAl)) || Equals(obj, nameof(ProgramName.ASUDScada)))
            {
                browserDialog.OpenFileFilter = $"Файл базы данных (*.{nameof(FileExtension.db)})|*.{nameof(FileExtension.db)}";
                string _value = browserDialog.OpenFile();
                if (_value == null)
                    return;

                if (Equals(obj, nameof(ProgramName.WinAl)))
                    WinAlDatabasePath = _value;
                else if (Equals(obj, nameof(ProgramName.ASUDScada)))
                    ASUDScadaDatabasePath = _value;
            }
            else if (Equals(obj, nameof(ProgramName.Horizon)))
            {
                browserDialog.OpenFileFilter = $"Файл базы данных (*.{nameof(FileExtension.mdb)})|*.{nameof(Base.FileExtension.mdb)}";
                string _value = browserDialog.OpenFile();
                if (_value == null)
                    return;
                HorizonDatabasePath = _value;
            }
        }));

        /// <summary>
        /// Открыть окно выбора папки.
        /// </summary>
        public RelayCommand SelectFolderDialog => selectFolderDialog ?? (selectFolderDialog = new RelayCommand((obj) =>
        {
            browserDialog = browserDialog ?? new BrowserDialog();
            string _value = browserDialog.OpenFolder();
            if (_value == null)
                return;
            ProjectFolderPath = _value;
        })); 
    }
}
