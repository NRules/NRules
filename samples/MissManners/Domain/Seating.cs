namespace NRules.Samples.MissManners.Domain
{
    public class Seating
    {
        public int Id { get; }
        public int Pid { get; }
        public bool PathDone { get; private set; }
        public int LeftSeatId { get; }
        public string LeftGuestName { get; }
        public int RightSeatId { get; }
        public string RightGuestName { get; }

        public Seating(int id, int pid, bool pathDone, int leftSeatId, string leftGuestName, int rightSeatId, string rightGuestName)
        {
            Id = id;
            Pid = pid;
            PathDone = pathDone;
            LeftSeatId = leftSeatId;
            LeftGuestName = leftGuestName;
            RightSeatId = rightSeatId;
            RightGuestName = rightGuestName;
        }

        public void SetPathDone()
        {
            PathDone = true;
        }

        public override string ToString()
        {
            return $"[Seating={Id}|{Pid}|L({LeftGuestName}->{LeftSeatId})R({RightGuestName}->{RightSeatId})|?{(PathDone ? "Y" : "N")}]";
        }
    }
}