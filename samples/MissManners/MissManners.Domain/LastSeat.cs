namespace MissManners.Domain
{
    public class LastSeat
    {
        public int SeatId { get; private set; }

        public LastSeat(int seatId)
        {
            SeatId = seatId;
        }

        public override string ToString()
        {
            return string.Format("LastSeat={0}", SeatId);
        }
    }
}