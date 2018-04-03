namespace NRules
{
    internal interface IIdGenerator
    {
        long NextTupleId();
    }

    internal class IdGenerator : IIdGenerator
    {
        private long _nextId = 1;

        public long NextTupleId()
        {
            return _nextId++;
        }
    }
}
