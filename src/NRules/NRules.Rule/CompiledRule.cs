using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public interface ICompiledRule
    {
        string Handle { get; }
        string Name { get; }
        int Priority { get; }
        GroupElement LeftSide { get; }
        IEnumerable<IRuleAction> RightSide { get; }
    }

    internal class CompiledRule : ICompiledRule
    {
        private readonly IList<RuleAction> _rightSide;

        public static int DefaultPriority
        {
            get { return 0; }
        }

        public CompiledRule()
        {
            Name = string.Empty;
            Priority = DefaultPriority;
            Handle = Guid.NewGuid().ToString();
            LeftSide = new GroupElement(GroupType.And);
            _rightSide = new List<RuleAction>();
        }

        public string Handle { get; private set; }
        public string Name { get; set; }
        public int Priority { get; set; }

        public GroupElement LeftSide { get; set; }

        public IEnumerable<IRuleAction> RightSide
        {
            get { return _rightSide; }
        }

        public void AddAction(RuleAction action)
        {
            _rightSide.Add(action);
        }
    }
}