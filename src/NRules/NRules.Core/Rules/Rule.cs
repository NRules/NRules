using System;
using System.Collections.Generic;

namespace NRules.Core.Rules
{
    internal class Rule
    {
        private readonly ISet<IDeclaration> _declarations = new HashSet<IDeclaration>();
        private readonly IList<ICondition> _conditions = new List<ICondition>();
        private readonly IList<IJoinCondition> _joinConditions = new List<IJoinCondition>();
        private readonly IList<IRuleAction> _actions = new List<IRuleAction>();

        public Rule(string name)
        {
            Handle = Guid.NewGuid().ToString();
            Name = name;
        }

        public string Handle { get; private set; }
        public string Name { get; private set; }

        public ISet<IDeclaration> Declarations
        {
            get { return _declarations; }
        }

        public IList<ICondition> Conditions
        {
            get { return _conditions; }
        }

        public IList<IJoinCondition> JoinConditions
        {
            get { return _joinConditions; }
        }

        public IList<IRuleAction> Actions
        {
            get { return _actions; }
        }
    }
}