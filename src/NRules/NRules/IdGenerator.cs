namespace NRules;

internal interface IIdGenerator : ICanDeepClone<IIdGenerator>
{
    long NextTupleId();
}

internal class IdGenerator : IIdGenerator
{
    private long _nextId = 1;

    public IIdGenerator DeepClone()
    {
        return new IdGenerator { _nextId = _nextId };
    }

    public long NextTupleId()
    {
        return _nextId++;
    }
}
