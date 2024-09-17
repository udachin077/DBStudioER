using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Models
{ 
    public class Sensor
    {
        public Sensor(int id, string name, string address = null)
        {
            Id = id;
            Name = name;
            Address = address;
        }

        public int Id { get; }
        public string Name { get; }
        public string Address { get; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Address))
                return Name;
            return Address + " " + Name;
        }
    }
}
