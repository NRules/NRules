using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Expression with a name used by an aggregator.
    /// </summary>
    public class NamedExpression
    {
        internal NamedExpression(string name, LambdaExpression expression)
        {
            Name = name;
            Expression = expression;
        }

        /// <summary>
        /// Expression name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Expression value.
        /// </summary>
        public LambdaExpression Expression { get; }
    }
}