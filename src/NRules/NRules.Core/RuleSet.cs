using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Core
{
    public class RuleSet
    {
        private readonly IList<Type> _ruleTypes;

        internal RuleSet(IEnumerable<Type> ruleTypes)
        {
            _ruleTypes = ruleTypes.ToList();
        }

        public IEnumerable<Type> RuleTypes
        {
            get { return _ruleTypes; }
        }
    }
}