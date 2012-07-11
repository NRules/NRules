using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
using Tuple = NRules.Core.Rete.Tuple;

namespace NRules.Core
{
    internal class ActionContext : IActionContext
    {
        private readonly Tuple _tuple;

        public ActionContext(Tuple tuple)
        {
            _tuple = tuple;
        }

        public T Arg<T>()
        {
            IEnumerable<Fact> elementOfCorrectType = _tuple.Elements.Where(f => f.FactType == typeof (T));
            List<T> elementAscorrectType = elementOfCorrectType.Select(f => f.Object).Cast<T>().ToList();

            if(!elementAscorrectType.Any())
            {
                throw new ApplicationException(string.Format("Could not get argument of type {0} from action context!", typeof(T)));
            }
            
            if(elementAscorrectType.Count > 1)
            {
                throw new ApplicationException(string.Format("Tuple contained more than one fact of type {0} in action context!", typeof(T)));
            }

            T element = elementAscorrectType.Single();
            return element;
        }
    }
}