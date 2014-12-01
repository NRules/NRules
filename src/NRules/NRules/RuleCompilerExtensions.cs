using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules
{
    public static class RuleCompilerExtensions
    {
        /// <summary>
        /// Compiles rules from rule sets into a session factory.
        /// </summary>
        /// <param name="compiler">Rule compiler instance.</param>
        /// <param name="ruleSets">Rule sets to compile.</param>
        /// <returns>Session factory.</returns>
        public static ISessionFactory Compile(this IRuleCompiler compiler, IEnumerable<IRuleSet> ruleSets)
        {
            var rules = ruleSets.SelectMany(x => x.Rules);
            return compiler.Compile(rules);
        }
    }
}