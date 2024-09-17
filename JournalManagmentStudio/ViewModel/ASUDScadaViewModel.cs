using Base;
using Base.Models;
using Data.Firebird;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace JournalManagmentStudio.Journal.ViewModel
{
    internal class ASUDScadaViewModel : OnlineJournalViewModel
    {
        private static ASUDScadaViewModel instance;

        public ASUDScadaViewModel() : base("ALERT_ID", "DATE_OPEN", nameof(TableName.journal_hardware), new ASUDScadaFbController())
        {

        }

        public static ASUDScadaViewModel GetInstanse()
        {
            if (instance == null)
                instance = new ASUDScadaViewModel();
            return instance;
        }

        protected override Task FillSensorList => Task.Run(() =>
        {
            if (Sensors != null)
            {
                Sensors.Clear();
                Sensors.Add(new Sensor(0, "Все"));
            }
            else
                Sensors = new List<Sensor>() { new Sensor(0, "Все") };

            foreach (DataRow row in mainDataSet?.Tables[nameof(TableName.hardware_alerts)].Rows)
            {
                object[] cells = row.ItemArray;
                Sensors.Add(new Sensor(Convert.ToInt32(cells[0]), cells[1].ToString().Trim()));
            };
        });
    }
}
