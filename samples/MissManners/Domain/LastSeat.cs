namespace NRules.Samples.MissManners.Domain
{
    public class LastSeat
    {
        public int SeatId { get; }

        public LastSeat(int seatId)
        {
            SeatId = seatId;
        }

        public override string ToString()
        {
            return $"LastSeat={SeatId}";
        }
    }
}