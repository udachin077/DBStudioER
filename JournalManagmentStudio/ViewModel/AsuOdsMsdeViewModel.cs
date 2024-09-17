using Base;
using Base.Models;
using Data.Msde;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JournalManagmentStudio.Journal.ViewModel
{
    internal class AsuOdsMsdeViewModel : OnlineJournalViewModel
    {
        private static AsuOdsMsdeViewModel instance;

        public AsuOdsMsdeViewModel() : base("dat_tip", "dtn", nameof(TableName.Protdisp), new MsdeController(), "MM.dd.yyyy")
        {

        }

        public static AsuOdsMsdeViewModel GetInstanse()
        {
            if (instance == null)
                instance = new AsuOdsMsdeViewModel();
            return instance;
        }

        protected override Task FillSensorList => Task.Run(() =>
        {
            if (Sensors == null)
            {
                Sensors = new List<Sensor>
                    {
                        new Sensor(0, "Все"),
                        new Sensor(16, "Двери МП"),
                        new Sensor(8, "Авария лифта"),
                        new Sensor(9, "Двери шахты"),
                        new Sensor(19, "Щитовая"),
                        new Sensor(17, "Чердак"),
                        new Sensor(18, "Крыша"),
                        new Sensor(20, "Подвал"),
                        new Sensor(27, "Люк"),
                        new Sensor(15, "Пожар"),
                        new Sensor(7, "Затопление")
                    };
            }
        });
    }
}
