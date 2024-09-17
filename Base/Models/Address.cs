namespace Base.Models
{
    public class Address
    {
        public Address(int id, string name, string sensor = null)
        {
            Id = id;
            Name = name;
            Sensor = sensor;
        }

        public int Id { get; }
        public string Name { get; }
        public string Sensor { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
