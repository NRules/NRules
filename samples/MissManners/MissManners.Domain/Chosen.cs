namespace MissManners.Domain
{
    public class Chosen
    {
        public int Id { get; private set; }
        public string GuestName { get; private set; }
        public Hobby Hobby { get; private set; }

        public Chosen(int id, string guestName, Hobby hobby)
        {
            Id = id;
            GuestName = guestName;
            Hobby = hobby;
        }

        public override string ToString()
        {
            return string.Format("Chosen={0}|{1}|{2}", Id, GuestName, Hobby.Name);
        }
    }
}