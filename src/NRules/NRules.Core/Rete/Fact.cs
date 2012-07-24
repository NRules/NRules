using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Core.Rete
{
    internal class Fact : IEquatable<Fact>
    {
        public Fact(object @object)
        {
            Object = @object;
            FactType = GetFactType(@object);
            ChildTuples = new List<Tuple>();
        }

        private Type GetFactType(object @object)
        {
            Type result = @object.GetType();

            Type genericCollectionType = result.GetInterfaces().FirstOrDefault(x =>
                                                                               x.IsGenericType &&
                                                                               x.GetGenericTypeDefinition() ==
                                                                               typeof (IEnumerable<>));
            if (genericCollectionType != null)
            {
                result = typeof (IEnumerable<>).MakeGenericType(genericCollectionType.GetGenericArguments());
            }

            return result;
        }

        public Type FactType { get; private set; }
        public object Object { get; private set; }
        public IList<Tuple> ChildTuples { get; private set; }

        public bool Equals(Fact other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.FactType, FactType) && Equals(other.Object, Object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Fact)) return false;
            return Equals((Fact) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FactType.GetHashCode()*397) ^ Object.GetHashCode();
            }
        }
    }
}