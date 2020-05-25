using NRules.Rete;

namespace NRules.Utilities
{
    internal interface IArgumentMap
    {
        IndexMap FactMap { get; }
        int Count { get; }
    }

    internal class ArgumentMap : IArgumentMap
    {
        public ArgumentMap(IndexMap factMap, int count)
        {
            FactMap = factMap;
            Count = count;
        }

        public IndexMap FactMap { get; }
        public int Count { get; }
    }
}