namespace NRules.Rete
{
    internal interface ITerminal
    {
        ITupleSource Source { get; }
        IndexMap FactMap { get; }
    }

    internal class Terminal : ITerminal
    {
        public ITupleSource Source { get; }
        public IndexMap FactMap { get; }

        public Terminal(ITupleSource source, IndexMap factMap)
        {
            Source = source;
            FactMap = factMap;
        }
    }
}