using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public int this[int index] => (index >= 0 && index < _map.Length) ? _map[index] : -1;

        public static void SetElementAt(object[] target, int index, object value)
        {
            if (index >= 0)
            {
                target[index] = value;
            }
        }

        public static IndexMap CreateMap(IEnumerable<Declaration> declarations, IEnumerable<Declaration> baseDeclarations)
        {
            var positionMap = declarations.ToIndexMap();
            var map = baseDeclarations
                .Select(positionMap.IndexOrDefault).ToArray();
            return new IndexMap(map);
        }
    }
}
