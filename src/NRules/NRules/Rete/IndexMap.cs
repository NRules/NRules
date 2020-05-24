using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Rete
{
    [DebuggerDisplay("[{string.Join(\" \",_map)}]")]
    internal class IndexMap
    {
        private readonly int[] _map;

        private IndexMap(int[] map)
        {
            _map = map;
        }

        public static IndexMap Empty = new IndexMap(new int[0]);
        public static IndexMap Unit = new IndexMap(new[] {0});

        public bool HasData => _map.Any(x => x >= 0);
        public int Length => _map.Length;
        public int this[int index] => (index >= 0 && index < _map.Length) ? _map[index] : -1;

        public static IndexMap CreateMap(IEnumerable<Declaration> declarations, IEnumerable<Declaration> baseDeclarations)
        {
            var positionMap = declarations.ToIndexMap();
            var map = baseDeclarations
                .Select(positionMap.IndexOrDefault).ToArray();
            return new IndexMap(map);
        }

        public static IndexMap Compose(IndexMap first, IndexMap second)
        {
            var map = first._map.Select(x => second[x]).ToArray();
            return new IndexMap(map);
        }
    }
}
