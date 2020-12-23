using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            return repository.Compile(default);
        }

        /// <summary>
        /// Compiles all rules in the repository into a session factory.
        /// Use <see cref="RuleCompiler"/> explicitly if only need to compile a subset of rules.
        /// </summary>
        /// <param name="repository">Rule repository.</param>
        /// <param name="cancellationToken">Enables cooperative cancellation of the rules compilation.</param>
        /// <returns>Session factory.</returns>
        /// <seealso cref="RuleCompiler"/>
        public static ISessionFactory Compile(this IRuleRepository repository, CancellationToken cancellationToken)
        {
            var compiler = new RuleCompiler();
            ISessionFactory factory = compiler.Compile(repository.GetRules(), cancellationToken);
            return factory;
        }
    }
}