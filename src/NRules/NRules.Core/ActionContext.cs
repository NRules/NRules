using System.Linq;
using NRules.Core.Rete;

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
            return _tuple.Where(f => f.FactType == typeof (T)).Select(f => f.Object).Cast<T>().First();
        }
    }
}
