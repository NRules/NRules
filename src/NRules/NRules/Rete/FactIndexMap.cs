using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Rete
{
    [DebuggerDisplay("[{string.Join(\" \",_map)}]")]
    internal class FactIndexMap
    {
        private readonly int[] _map;

        public FactIndexMap(int[] map)
        {
            _map = map;
        }

        public int Map(int index)
        {
            return (index >= 0) ? _map[index] : index;
        }

        public static void SetElementAt(ref object[] target, int index, int offset, object value)
        {
            if (index >= 0)
            {
                target[index + offset] = value;
            }
        }

        public static FactIndexMap CreateMap(IEnumerable<Declaration> declarations, IEnumerable<Declaration> baseDeclarations)
        {
            var positionMap = declarations.ToIndexMap();
            var map = baseDeclarations
                .Select(positionMap.IndexOrDefault).ToArray();
            return new FactIndexMap(map);
        }
    }
}
