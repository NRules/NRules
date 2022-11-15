using NRules.Aggregators;
using NRules.Utilities;

namespace NRules
{
    /// <summary>
    /// Defines different modes of handling of unsupported types of lambda expressions
    /// when the compiler is comparing them for the purpose of node sharing in the Rete graph.
    /// </summary>
    public enum RuleCompilerUnsupportedExpressionsHandling
    {
        /// <summary>
        /// Rule compilation fails when it finds an unsupported expression.
        /// This is the default behavior.
        /// </summary>
        FailFast = 0,

        /// <summary>
        /// Rule compilation treats unsupported expressions as not equal, sacrificing efficiency
        /// in favor of compatibility.
        /// </summary>
        TreatAsNotEqual = 1,
    }

    /// <summary>
    /// Provides options to alter default behavior of <see cref="RuleCompiler"/>.
    /// </summary>
    public class RuleCompilerOptions
    {
        /// <summary>
        /// Determines compiler behavior when it finds an unsupported type of lambda expressions
        /// while comparing them for the purpose of node sharing in the Rete graph.
        /// </summary>
        public RuleCompilerUnsupportedExpressionsHandling UnsupportedExpressionHandling { get; set; } = RuleCompilerUnsupportedExpressionsHandling.FailFast;

        internal IRuleExpressionCompiler RuleExpressionCompiler { get; set; } = new RuleExpressionCompiler();

        internal IAggregatorRegistry AggregatorRegistry { get; set; } = new AggregatorRegistry();
    }
}