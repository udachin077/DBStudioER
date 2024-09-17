using Base.Models;
using JournalManagmentStudio.Project.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace JournalManagmentStudio.Project.ViewModel
{
    public class AsuOdsMsdeViewModel : ProjectTableViewModel
    {
        private Sensor _selectedSensor;
        private string _selectedSensorName;

        public AsuOdsMsdeViewModel(ProjectParameter parameter) : base(parameter)
        {
            SelectedSensor = Sensors[0];
        }

        // Улица
        public string Street { get; set; } = "Улица";
        // Дом
        public string House { get; set; } = "Дом";
        // Корпус
        public short Housing { get; set; }
        // Подъезд
        public short Door { get; set; }
        // Список типов контактов
        public List<Sensor> Sensors { get; } = new List<Sensor>
                {
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
        // Выбранный контакт
        public Sensor SelectedSensor { get => _selectedSensor; set { _selectedSensor = value; SelectedSensorName = _selectedSensor.Name; OnPropertyChanged(); } }
        // Имя контакта
        public string SelectedSensorName { get => _selectedSensorName; set { _selectedSensorName = value; OnPropertyChanged(); } }

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
            _row["h_street"] = Street;
            _row["h_nom"] = DBNull.Value;
            _row["h_noma"] = House;
            _row["h_korp"] = Housing;
            _row["h_pod"] = Door;
            _row["term"] = DBNull.Value;
            _row["dat"] = DBNull.Value;
            _row["dat_tip"] = (short)SelectedSensor.Id;
            _row["dat_name"] = SelectedSensorName;
            _row["dtn"] = Date.ToLongDateString() + " " + dt1.ToLongTimeString();
            _row["dtk"] = Date.ToLongDateString() + " " + dt2.ToLongTimeString();
            _row["prost_text"] = (dt2 - dt1).ToString(@"hh\:mm\:ss");
            _row["prost"] = (dt2 - dt1).TotalSeconds;
            _row["pradres"] = DBNull.Value;

            return _row;
        }
    }
}
