namespace MissManners.Domain
{
    public class Seating
    {
        public int Id { get; private set; }
        public int Pid { get; private set; }
        public bool PathDone { get; private set; }
        public int LeftSeatId { get; private set; }
        public string LeftGuestName { get; private set; }
        public int RightSeatId { get; private set; }
        public string RightGuestName { get; private set; }

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
            return string.Format("[Seating={0}|{1}|L({2}->{3})R({4}->{5})|?{6}]", 
                Id, Pid, LeftGuestName, LeftSeatId, RightGuestName, RightSeatId,
                PathDone ? "Y" : "N");
        }
    }
}