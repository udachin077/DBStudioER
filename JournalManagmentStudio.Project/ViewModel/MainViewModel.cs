using Base;
using Base.Models;
using JournalManagmentStudio.Project.Services;
using Settings;
using System;
using System.Collections.Generic;
using System.IO;

namespace JournalManagmentStudio.Project.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        private RelayCommand _new;
        private RelayCommand open;
        private RelayCommand cancel;
        private RelayCommand create;
        private RelayCommand selectFileDialog;
        private ProjectTableViewModel slaveViewModel;
        private ProjectParameter xmlParameter;
        private bool newProjectButtonIsPressed;
        private string pathDataSet;
        private string projectName;
        private Program selectedProgram;

        public string ProjectName { get => projectName; set { projectName = value; OnPropertyChanged(); } }
        public string PathDataSet { get => pathDataSet; set { pathDataSet = value; OnPropertyChanged(); } }
        public bool NewProjectButtonIsPressed { get => newProjectButtonIsPressed; set { newProjectButtonIsPressed = value; OnPropertyChanged(); } }
        public ProjectTableViewModel SlaveViewModel { get => slaveViewModel; set { slaveViewModel = value; OnPropertyChanged(); } }
        public Program SelectedProgram { get => selectedProgram; set { selectedProgram = value; OnPropertyChanged(); } }
        public List<Program> ProgramList { get; } = new List<Program>
        {           
            new Program("/base;component/Images/asuods.png", ProgramName.AsuOdsMsde, DbMS.Msde),
            new Program("/base;component/Images/winal.ico", ProgramName.WinAl, DbMS.Firebird),
            new Program("/base;component/Images/horiz.ico", ProgramName.Horizon, DbMS.MSAccess),
            new Program("/base;component/Images/scada.ico", ProgramName.ASUDScada, DbMS.Firebird)
        };

        public MainViewModel(string path = null)
        {
            // Если запушено с параметром, то path не будет равен null
            if (string.IsNullOrEmpty(path) == false)
            {
                // Чтение проекта и проверка существования файлов
                // Если не существуют выводим сообщение
                if (ProjectFilesExists(path) == false)
                {
                    ErrorMessage.Show("Не удалось загрузить проект.");
                }
                else
                {
                    // Установка имени проекта
                    ProjectName = xmlParameter.Name;
                    // Установка модели
                    SetSlaveViewModel();
                }
            }
        }

        public RelayCommand New => _new ?? (_new = new RelayCommand((obj) =>
        {
            // Устанавливает "показать" окно создания нового проекта
            NewProjectButtonIsPressed = true;
        }));

        public RelayCommand Open => open ?? (open = new RelayCommand((obj) =>
        {
            BrowserDialog browserDialog = new BrowserDialog();
            browserDialog.OpenFileDirectory = AppSettings.String.GetValue(KeyName.ProjectFolder, Hive.JPS);
            browserDialog.OpenFileTitle = "Выбор проекта";
            browserDialog.OpenFileFilter = $"Файл проекта (*.{nameof(FileExtension.jproj)})|*.{nameof(FileExtension.jproj)}";
            string _value = browserDialog.OpenFile();
            if (_value == null)
                return;

            // Чтение проекта и проверка существования файлов
            // Если не существуют выводим сообщение
            if (ProjectFilesExists(_value) == false)
            {
                ErrorMessage.Show("Не удалось загрузить проект.");
            }
            else
            {
                // Установка имени проекта
                ProjectName = xmlParameter.Name;
                // Установка модели
                SetSlaveViewModel();
            }
        }));

        public RelayCommand Create => create ?? (create = new RelayCommand((obj) =>
        {
            try
            {
            NewProjectButtonIsPressed = false;
            // Сохранение файла проекта с созданием таблицы
            // и копированием в папку проекта выбранного датасет
            this.xmlParameter = ProjectFileParser.Save(ProjectName, selectedProgram.ProgramName, PathDataSet);
            SetSlaveViewModel();
            }
            catch(Exception ex)
            {
                ErrorMessage.Show(ex.Message);
            }



        }, obj => !string.IsNullOrEmpty(ProjectName) && SelectedProgram != null));

        /// <summary>
        /// Select DataSet
        /// </summary>
        public RelayCommand SelectFileDialog => selectFileDialog ?? (selectFileDialog = new RelayCommand((obj) =>
        {
            BrowserDialog browserDialog = new BrowserDialog();
            browserDialog.OpenFileTitle = "Файл DataSet";
            browserDialog.OpenFileFilter = $"Файл DataSet (*.{nameof(FileExtension.ds)})|*.{nameof(FileExtension.ds)}";
            string _value = browserDialog.OpenFile();
            if (_value == null)
                return;
            PathDataSet = _value;
        }));

        public RelayCommand Cancel => cancel ?? (cancel = new RelayCommand((obj) =>
        {        
            NewProjectButtonIsPressed = false;
            SelectedProgram = null;
            PathDataSet = null;
            ProjectName = null;
        }));

        /// <summary>
        /// Читает файл проекта и проверяет существование файлов.
        /// Возвращает true, если файлы существуют.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool ProjectFilesExists(string path)
        {
            // Чтение файла проекта
            this.xmlParameter = ProjectFileParser.Load(path);
            // Проверка существования файла таблицы
            if (!File.Exists(xmlParameter.PathTable))
                return false;
            // Проверка датасет
            if (!string.IsNullOrEmpty(xmlParameter.PathDataSet))
                if (!File.Exists(xmlParameter.PathDataSet))
                    return false;
            // Если все файлы найдены
            return true;
        }

        /// <summary>
        /// Устанавливает <see cref="SlaveViewModel"/> на основании файла проекта.
        /// </summary>
        private void SetSlaveViewModel()
        {
            switch (xmlParameter.Program)
            {
                case ProgramName.WinAl:
                    SlaveViewModel = new WinAlViewModel(xmlParameter);
                    break;
                case ProgramName.AsuOdsMsde:
                    SlaveViewModel = new AsuOdsMsdeViewModel(xmlParameter);
                    break;
                case ProgramName.AsuOdsSqlServer:
                    break;
                case ProgramName.Horizon:
                    SlaveViewModel = new HorizonViewModel(xmlParameter);
                    break;
                case ProgramName.ASUDScada:
                    SlaveViewModel = new ASUDScadaViewModel(xmlParameter); 
                    break;
                default:
                    break;
            }
        }
    }
}
