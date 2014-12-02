using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules
{
    public static class RuleRepositoryExtensions
    {
        /// <summary>
        /// Retrieves all rules from all rule sets contained in the repository.
        /// </summary>
        /// <returns>Collection of rules from the repository.</returns>
        public static IEnumerable<IRuleDefinition> GetRules(this IRuleRepository repository)
        {
            var ruleSets = repository.GetRuleSets();
            return ruleSets.SelectMany(rs => rs.Rules);
        }

        /// <summary>
        /// Compiles all rules in the repository into a session factory.
        /// Use <see cref="RuleCompiler"/> explicitly if only need to compile a subset of rules.
        /// </summary>
        /// <param name="repository">Rule repository.</param>
        /// <returns>Session factory.</returns>
        /// <seealso cref="RuleCompiler"/>
        public static ISessionFactory Compile(this IRuleRepository repository)
        {
            var compiler = new RuleCompiler();
            ISessionFactory factory = compiler.Compile(repository.GetRules());
            return factory;
        }
    }
}