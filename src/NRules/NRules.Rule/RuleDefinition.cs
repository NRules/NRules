using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public interface IRuleDefinition
    {
        string Handle { get; }
        string Name { get; }
        int Priority { get; }
        GroupElement LeftHandSide { get; }
        IEnumerable<IRuleAction> RightHandSide { get; }
    }

    internal class RuleDefinition : IRuleDefinition
    {
        private readonly IList<RuleAction> _rightSide;

        public static int DefaultPriority
        {
            get { return 0; }
        }

        public RuleDefinition()
        {
            Name = string.Empty;
            Priority = DefaultPriority;
            Handle = Guid.NewGuid().ToString();
            _rightSide = new List<RuleAction>();
        }

        public string Handle { get; private set; }
        public string Name { get; set; }
        public int Priority { get; set; }

        public GroupElement LeftHandSide { get; internal set; }

        public IEnumerable<IRuleAction> RightHandSide
        {
            get { return _rightSide; }
        }

        public void AddAction(RuleAction action)
        {
            _rightSide.Add(action);
        }
    }
}