using System.Collections.Generic;
using NRules.Rule;

namespace NRules.Core
{
    public interface IRuleRepository
    {
        IEnumerable<IRuleDefinition> Rules { get; }
        ISessionFactory CreateSessionFactory();
    }

    public abstract class RuleRepository : IRuleRepository
    {
        public abstract IEnumerable<IRuleDefinition> Rules { get; }

        public ISessionFactory CreateSessionFactory()
        {
            var compiler = new RuleCompiler();
            var factory = compiler.Compile(Rules);
            return factory;
        }
    }
}