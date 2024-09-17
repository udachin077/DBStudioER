using Base.Models;
using JournalManagmentStudio.Project.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace JournalManagmentStudio.Project.ViewModel
{
    internal class ASUDScadaViewModel : ProjectTableViewModel
    {
        private List<Sensor> sensors;
        private Sensor selectedSensor;
        private List<Address> addresses;
        private Address selectedAddress;
        private List<Tag> tags;
        private Tag selectedTag;

        public ASUDScadaViewModel(ProjectParameter parameter) : base(parameter)
        {
            sensors = new List<Sensor>();
            addresses = new List<Address>();
            tags = new List<Tag>();
            FillLists();
            SelectedSensor = sensors[0];
            SelectedAddress = addresses[0];
            SelectedTag = tags[0];
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
        public List<Tag> Tags { get => tags; set { tags = value; OnPropertyChanged(); } }
        // Выбранный концентратор
        public Tag SelectedTag { get => selectedTag; set { selectedTag = value; OnPropertyChanged(); } }

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
            _row["ADDRESS_ID"] = selectedAddress.Id;
            _row["ADDRESS_NAME"] = selectedAddress.Name;
            _row["DATE_OPEN"] = Date.ToLongDateString() + " " + dt1.ToLongTimeString();
            _row["DATE_ACCEPT"] = Date.ToLongDateString() + " " + dt1.ToLongTimeString();
            _row["DATE_CLOSE"] = Date.ToLongDateString() + " " + dt2.ToLongTimeString();
            _row["ALERT_ID"] = selectedSensor.Id;
            _row["ALERT_NAME"] = selectedSensor.Name;
            _row["TAG_ID"] = selectedTag.Id;
            _row["TAG_NAME"] = selectedTag.Name;
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
            foreach (DataRow row in base.mainDataSet.Tables["item_tags"].Rows)
            {
                var cells = row.ItemArray;
                Tags.Add(new Tag(Convert.ToInt32(cells[0]), cells[1].ToString().Trim()));
            }
        }
    }
}
