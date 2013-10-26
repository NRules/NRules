using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Rule
{
    /// <summary>
    /// Action that executes when the rule fires.
    /// </summary>
    public class ActionElement
    {
        private readonly List<Declaration> _declarations;

        /// <summary>
        /// Expression that represents the rule action.
        /// </summary>
        public LambdaExpression Expression { get; set; }

        /// <summary>
        /// List of declarations referenced by the action expression.
        /// </summary>
        public IEnumerable<Declaration> Declarations
        {
            get { return _declarations; }
        }

        internal ActionElement(IEnumerable<Declaration> declarations, LambdaExpression expression)
        {
            _declarations = new List<Declaration>(declarations);
            Expression = expression;
        }
    }
}