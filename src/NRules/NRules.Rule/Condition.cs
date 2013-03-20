using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Rule
{
    public class Condition
    {
        private readonly List<Declaration> _declarations;

        public LambdaExpression Expression { get; set; }
        public IEnumerable<Declaration> Declarations
        {
            get { return _declarations; }
        }

        public Condition(IEnumerable<Declaration> declarations, LambdaExpression expression)
        {
            _declarations = new List<Declaration>(declarations);
            Expression = expression;
        }
    }
}