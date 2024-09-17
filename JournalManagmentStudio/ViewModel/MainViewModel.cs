using Base;
using Settings;
using Base.Models;

namespace JournalManagmentStudio.Journal.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {    
        private OnlineJournalViewModel slaveViewModel;
        private bool filterVisibility;
        private bool allColumnsVisibility;
        private bool tableIsUnlocked;
        private bool customQueryVisibility;
        private System.Media.SoundPlayer filterSP = new System.Media.SoundPlayer(Properties.Resources.sound_3);
        private System.Media.SoundPlayer tableLock = new System.Media.SoundPlayer(Properties.Resources.sound_1);
        private System.Media.SoundPlayer tableUnlock = new System.Media.SoundPlayer(Properties.Resources.sound_2);

        public OnlineJournalViewModel SlaveViewModel { get => slaveViewModel; set { slaveViewModel = value; OnPropertyChanged(); } }
        public bool FilterVisibility { get => filterVisibility; set { filterVisibility = value; OnPropertyChanged(); filterSP.Play(); } }
        public bool AllColumnsVisibility { get => allColumnsVisibility; set { allColumnsVisibility = value; OnPropertyChanged(); } }
        public bool TableIsUnlocked { get => tableIsUnlocked; set { tableIsUnlocked = value; OnPropertyChanged(); 
                if (tableIsUnlocked == true)
                {
                    tableUnlock.Play();
                }
                else
                {
                    tableLock.Play();
                    AllColumnsVisibility = false;
                }
            } }
        public bool CustomQueryVisibility { get => customQueryVisibility; set { customQueryVisibility = value; OnPropertyChanged(); } }

        public MainViewModel()
        {
            ApplicationSettingsViewModel_ProgramChanged();
            if (AppSettings.DWord.GetValue(KeyName.AutoConnect, typeof(bool)))
                slaveViewModel.Connect.Execute(null);
        }

        private void ApplicationSettingsViewModel_ProgramChanged()
        {
            switch ((ProgramName)AppSettings.DWord.GetValue(KeyName.Program, typeof(byte)))
            {
                case ProgramName.WinAl:
                    SlaveViewModel = new WinAlViewModel();
                    SlaveViewModel.Program = new Program("/base;component/Images/winal.ico", ProgramName.WinAl, DbMS.Firebird);
                    break;
                case ProgramName.AsuOdsMsde:
                    SlaveViewModel = new AsuOdsMsdeViewModel();
                    SlaveViewModel.Program = new Program("/base;component/Images/asuods.png", ProgramName.AsuOdsMsde, DbMS.Msde);
                    break;
                case ProgramName.AsuOdsSqlServer:
                    break;
                case ProgramName.Horizon:
                    SlaveViewModel = new HorizonViewModel();
                    SlaveViewModel.Program = new Program("/base;component/Images/horiz.ico", ProgramName.Horizon, DbMS.MSAccess);
                    break;
                case ProgramName.ASUDScada:
                    SlaveViewModel = new ASUDScadaViewModel();
                    SlaveViewModel.Program = new Program("/base;component/Images/scada.ico", ProgramName.ASUDScada, DbMS.Firebird);
                    break;
                default:
                    break;
            }
        }
    }
}
