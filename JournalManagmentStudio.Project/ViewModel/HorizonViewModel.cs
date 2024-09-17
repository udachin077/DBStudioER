using Base.Models;
using JournalManagmentStudio.Project.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace JournalManagmentStudio.Project.ViewModel
{
    public class HorizonViewModel : ProjectTableViewModel
    {
        private List<Sensor> sensors;
        private Sensor selectedSensor;

        public HorizonViewModel(ProjectParameter parameter) : base(parameter)
        {
            sensors = new List<Sensor>();
            FillLists();
            SelectedSensor = sensors[0];
        }

        // Список типов контактов
        public List<Sensor> Sensors { get => sensors; set { sensors = value; OnPropertyChanged(); } }
        // Выбранный контакт
        public Sensor SelectedSensor { get => selectedSensor; set { selectedSensor = value; OnPropertyChanged(); } }
        // Оператор
        public string Operator { get; set; } = "Диспетчер";

        protected override DataRow BuildNewDataRow()
        {
            // Добавление секунд ко времени открытия
            DateTime dt1 = Convert.ToDateTime(TimeOpen).AddSeconds(random.Next(0, 59));
            // Добавление секунд ко времени закрытия
            DateTime dt2 = Convert.ToDateTime(TimeClose).AddSeconds(random.Next(0, 59));
            // Добавление секунд ко времени закрытия пока время открытия больше
            // или равно
            while (dt1 >= dt2) { dt2 = dt2.AddSeconds(random.Next(1, 59)); }

            DataRow _row = MainTable.NewRow();
            _row["address"] = SelectedSensor.Address;
            _row["sensor"] = SelectedSensor.Name;
            _row["opentime"] = Date.ToLongDateString() + " " + dt1.ToLongTimeString();
            _row["closetime"] = Date.ToLongDateString() + " " + dt2.ToLongTimeString();
            _row["prost_text"] = (dt2 - dt1).ToString(@"hh\:mm\:ss");
            _row["door_id"] = SelectedSensor.Id;
            //row["isopen"] = DBNull.Value;
            //row["create_time"] = DBNull.Value;
            //row["record_time"] = (short)SelectedSensor.Id;
            _row["create_operator"] = Operator;

            return _row;
        }

        private void FillLists()
        {
            foreach (DataRow row in base.mainDataSet.Tables["Doors"].Rows)
            {
                object[] cells = row.ItemArray;
                Sensors.Add(new Sensor(Convert.ToInt32(cells[0]), cells[2].ToString().Trim(), cells[1].ToString().Trim()));
            }
        }
    }
}
