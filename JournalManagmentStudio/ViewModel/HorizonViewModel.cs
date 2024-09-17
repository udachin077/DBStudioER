using Base;
using Base.Models;
using Data.Access;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace JournalManagmentStudio.Journal.ViewModel
{
    internal class HorizonViewModel : OnlineJournalViewModel
    {
        private static HorizonViewModel instance;

        public HorizonViewModel() : base("door_id", "opentime", nameof(TableName.DoorEvents), new AccessController())
        {

        }

        public static HorizonViewModel GetInstanse()
        {
            if (instance == null)
                instance = new HorizonViewModel();
            return instance;
        }

        protected override Task FillSensorList => Task.Run(() =>
              {
                  if (Sensors != null)
                  {
                      Sensors.Clear();
                  }
                  else
                      Sensors = new List<Sensor>();

                  Sensors.Add(new Sensor(0, "Все"));

                  foreach (DataRow row in mainDataSet?.Tables[nameof(TableName.Doors)].Rows)
                  {
                      object[] cells = row.ItemArray;
                      Sensors.Add(new Sensor(Convert.ToInt32(cells[0]), cells[1].ToString().Trim() + " " + cells[2].ToString().Trim()));
                  };
              });
    }
}
