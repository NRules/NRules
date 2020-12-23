namespace NRules.Samples.MissManners.Domain
{
    public class Path
    {
        public int Id { get; }
        public int SeatId { get; }
        public string GuestName { get; }

        public Path(int id, int seatId, string guestName)
        {
            Id = id;
            SeatId = seatId;
            GuestName = guestName;
        }

        public override string ToString()
        {
            return $"[Path={Id}|{GuestName}->{SeatId}]";
        }
    }
}