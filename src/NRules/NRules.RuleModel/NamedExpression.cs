using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Expression with a name used by an aggregator.
    /// </summary>
    public class NamedExpression
    {
        private readonly string _name;
        private readonly LambdaExpression _expression;

        internal NamedExpression(string name, LambdaExpression expression)
        {
            _name = name;
            _expression = expression;
        }

        /// <summary>
        /// Expression name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Expression value.
        /// </summary>
        public LambdaExpression Expression
        {
            get { return _expression; }
        }
    }
}