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
        /// <returns></returns>
        public static IEnumerable<IRuleDefinition> GetRules(this IRuleRepository repository)
        {
            var ruleSets = repository.GetRuleSets();
            return ruleSets.SelectMany(rs => rs.Rules);
        }

        /// <summary>
        /// Compiles all rules in the repository into a session factory.
        /// </summary>
        /// <param name="repository">Rule repository.</param>
        /// <returns>Session factory.</returns>
        public static ISessionFactory Compile(this IRuleRepository repository)
        {
            IRuleCompiler compiler = new RuleCompiler();
            ISessionFactory factory = compiler.Compile(repository.GetRules());
            return factory;
        }
    }
}