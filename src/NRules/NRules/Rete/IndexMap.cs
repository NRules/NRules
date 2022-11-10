using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Rete;

[DebuggerDisplay("[{string.Join(\" \",_map)}]")]
internal class IndexMap : IEquatable<IndexMap>, IEnumerable<int>
{
    private readonly int[] _map;

    private IndexMap(int[] map)
    {
        _map = map;
    }

    public static IndexMap Empty = new(Array.Empty<int>());
    public static IndexMap Unit = new(new[] { 0 });

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

    public bool Equals(IndexMap other)
    {
        if (_map.Length != other._map.Length)
            return false;

        for (var i = 0; i < _map.Length; i++)
        {
            if (_map[i] != other._map[i])
                return false;
        }

        return true;
    }

    public IEnumerator<int> GetEnumerator()
    {
        return _map.AsEnumerable().GetEnumerator();
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((IndexMap)obj);
    }

    public override int GetHashCode()
    {
        return _map.Length;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
