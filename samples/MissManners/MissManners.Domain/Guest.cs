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

        public string Name { get; private set; }
        public Gender Sex { get; private set; }
        public Hobby Hobby { get; private set; }

        public override string ToString()
        {
            return string.Format("[G={0}{1}{2}]", Name, Sex == Gender.Male ? "M" : "F", Hobby.Name);
        }
    }
}