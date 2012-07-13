using System;
using System.Linq;
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
            try
            {
                var arg = _tuple.Where(f => f.FactType == typeof(T)).Select(f => f.Object).Cast<T>().First();
                return arg;
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(string.Format("Could not get rule argument of type {0}", typeof(T)), e);
            }
        }
    }
}