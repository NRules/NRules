namespace NRules.Samples.MissManners.Domain
{
    public class Chosen
    {
        public int Id { get; }
        public string GuestName { get; }
        public Hobby Hobby { get; }

        public Chosen(int id, string guestName, Hobby hobby)
        {
            Id = id;
            GuestName = guestName;
            Hobby = hobby;
        }

        public override string ToString()
        {
            return $"Chosen={Id}|{GuestName}|{Hobby.Name}";
        }
    }
}