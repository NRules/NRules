using System;
using System.Collections.Generic;

namespace NRules.Core.Rules
{
    internal class CompiledRule
    {
        private readonly ISet<IDeclaration> _declarations = new HashSet<IDeclaration>();
        private readonly IList<ICondition> _conditions = new List<ICondition>();
        private readonly IList<ICompositeDeclaration> _composites = new List<ICompositeDeclaration>();
        private readonly IList<IRuleAction> _actions = new List<IRuleAction>();

        public CompiledRule(string name)
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

        public IList<ICompositeDeclaration> Composites
        {
            get { return _composites; }
        }

        public IList<IRuleAction> Actions
        {
            get { return _actions; }
        }
    }
}