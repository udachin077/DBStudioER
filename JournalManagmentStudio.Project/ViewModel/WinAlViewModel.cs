using Base.Models;
using JournalManagmentStudio.Project.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace JournalManagmentStudio.Project.ViewModel
{
    public class WinAlViewModel : ProjectTableViewModel
    {
        private List<Address> addresses;
        private List<Sensor> sensors;
        private List<Concentrator> concentrators;
        private Sensor selectedSensor;
        private Address selectedAddress;
        private Concentrator selectedConcentrator;

        public WinAlViewModel(ProjectParameter parameter) : base(parameter)
        {
            sensors = new List<Sensor>();
            addresses = new List<Address>();
            concentrators = new List<Concentrator>();
            FillLists();
            SelectedSensor = sensors[0];
            SelectedAddress = addresses[0];
            SelectedConcentrator = concentrators[0];
        }

        // Список сигналов
        public List<Sensor> Sensors { get => sensors; set { sensors = value; OnPropertyChanged(); } }
        // Выбранный сигнал
        public Sensor SelectedSensor { get => selectedSensor; set { selectedSensor = value; OnPropertyChanged(); } }
        // Список адресов
        public List<Address> Addresses { get => addresses; set { addresses = value; OnPropertyChanged(); } }
        // Выбранный адрес
        public Address SelectedAddress { get => selectedAddress; set { selectedAddress = value; OnPropertyChanged(); } }
        // Список концентраторов
        public List<Concentrator> Concentrators { get => concentrators; set { concentrators = value; OnPropertyChanged(); } }
        // Выбранный концентратор
        public Concentrator SelectedConcentrator { get => selectedConcentrator; set { selectedConcentrator = value; OnPropertyChanged(); } }
        // Список номеров контактов
        public List<int> Contacts { get; set; } = new List<int>()
        {
            1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24
        };
        // Выбранный контакт
        public int SelectedContact { get; set; } = 1;

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
            _row["CONC_ID"] = SelectedConcentrator.Id;
            _row["CONC_DIRECTION"] = SelectedConcentrator.Direction;
            _row["CONC_NUMBER"] = SelectedConcentrator.Number;
            _row["SENSOR_NUMBER"] = SelectedContact;
            _row["ADDRESS_ID"] = SelectedAddress.Id;
            _row["ADDRESS_NAME"] = SelectedAddress.Name;
            _row["DATE_OPEN"] = Date.ToLongDateString() + " " + dt1.ToLongTimeString();
            _row["DATE_ACCEPT"] = Date.ToLongDateString() + " " + dt1.ToLongTimeString();
            _row["DATE_CLOSE"] = Date.ToLongDateString() + " " + dt2.ToLongTimeString();
            _row["ALERT_ID"] = SelectedSensor.Id;
            _row["ALERT_NAME"] = SelectedSensor.Name;
            _row["prost_text"] = (dt2 - dt1).ToString(@"hh\:mm\:ss");

            return _row;
        }

        private void FillLists()
        {
            foreach (DataRow row in base.mainDataSet.Tables["address"].Rows)
            {
                var cells = row.ItemArray;
                Addresses.Add(new Address(Convert.ToInt32(cells[0]), cells[1].ToString().Trim()));
            }
            foreach (DataRow row in base.mainDataSet.Tables["hardware_alerts"].Rows)
            {
                var cells = row.ItemArray;
                Sensors.Add(new Sensor(Convert.ToInt32(cells[0]), cells[1].ToString().Trim()));
            }
            foreach (DataRow row in base.mainDataSet.Tables["concentrators"].Rows)
            {
                var cells = row.ItemArray;
                Concentrators.Add(new Concentrator(Convert.ToInt32(cells[0]), Convert.ToInt32(cells[1]), Convert.ToInt32(cells[2])));
            }
        }
    }
}
