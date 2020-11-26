namespace NRules.Samples.MissManners.Domain
{
    public enum Gender
    {
        Male,
        Female,
    }

    public class Guest
    {
        public Guest(string name, Gender sex, Hobby hobby)
        {
            Name = name;
            Sex = sex;
            Hobby = hobby;
        }

        public string Name { get; }
        public Gender Sex { get; }
        public Hobby Hobby { get; }

        public override string ToString()
        {
            return $"[G={Name}{(Sex == Gender.Male ? "M" : "F")}{Hobby.Name}]";
        }
    }
}