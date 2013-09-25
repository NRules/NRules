using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Rule
{
    public class ConditionElement
    {
        private readonly List<Declaration> _declarations;

        public LambdaExpression Expression { get; set; }

        public IEnumerable<Declaration> Declarations
        {
            get { return _declarations; }
        }

        internal ConditionElement(IEnumerable<Declaration> declarations, LambdaExpression expression)
        {
            _declarations = new List<Declaration>(declarations);
            Expression = expression;
        }
    }
}