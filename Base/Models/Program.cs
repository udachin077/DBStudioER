namespace Base.Models
{
    public class Program
    {
        public Program(string image, ProgramName name, DbMS dbms)
        {
            Image = image;
            Name = name.GetDescription();
            DbMS = dbms.GetDescription();
            ProgramName = name;
        }

        public string Image { get; }
        public string Name { get; }
        public string DbMS { get; }
        public ProgramName ProgramName { get; }
    }
}
