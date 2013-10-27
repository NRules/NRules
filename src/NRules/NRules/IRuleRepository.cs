using System.Collections.Generic;
using NRules.Rule;

namespace NRules
{
    /// <summary>
    /// In-memory database of production rules.
    /// </summary>
    public interface IRuleRepository
    {
        /// <summary>
        /// Collection of rules in the repository.
        /// </summary>
        IEnumerable<IRuleDefinition> Rules { get; }

        /// <summary>
        /// Creates a compiled representation of the rules in the repository.
        /// </summary>
        /// <returns>New rules session factory.</returns>
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