namespace NRules.Samples.MissManners.Domain
{
    public enum ContextState
    {
        StartUp = 0,
        AssignSeats = 1,
        MakePath = 2,
        CheckDone = 3,
        PrintResults = 4,
    }

    public class Context
    {
        public Context()
        {
            State = ContextState.StartUp;
        }

        public ContextState State { get; private set; }

        public void SetState(ContextState state)
        {
            State = state;
        }

        public override string ToString()
        {
            return State.ToString();
        }
    }
}