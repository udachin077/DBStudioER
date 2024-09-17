namespace Base.Models
{
    public class Concentrator
    {
        public Concentrator(int id, int direction, int number, string ip = null)
        {
            Id = id;
            Direction = direction;
            Number = number;
            IP = ip;
        }

        public int Id { get; }
        public int Direction { get; }
        public int Number { get; }
        public string IP { get; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(IP) ? $"Напр. {Direction} №{Number}" : IP;
        }
    }
}
