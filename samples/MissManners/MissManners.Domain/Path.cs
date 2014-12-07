namespace NRules.Samples.MissManners.Domain
{
    public class Path
    {
        public int Id { get; private set; }
        public int SeatId { get; private set; }
        public string GuestName { get; private set; }

        public Path(int id, int seatId, string guestName)
        {
            Id = id;
            SeatId = seatId;
            GuestName = guestName;
        }

        public override string ToString()
        {
            return string.Format("[Path={0}|{1}->{2}]", Id, GuestName, SeatId);
        }
    }
}