using System.Collections.Generic;

namespace NRules.Rule
{
    public interface IRuleDefinition
    {
        string Name { get; }
        int Priority { get; }
        GroupElement LeftHandSide { get; }
        IEnumerable<ActionElement> RightHandSide { get; }
    }

    internal class RuleDefinition : IRuleDefinition
    {
        public static int DefaultPriority
        {
            get { return 0; }
        }

        public RuleDefinition(string name, int priority, GroupElement leftHandSide, IEnumerable<ActionElement> rightHandSide)
        {
            Name = name;
            Priority = priority;
            LeftHandSide = leftHandSide;
            RightHandSide = new List<ActionElement>(rightHandSide);
        }

        public string Name { get; private set; }
        public int Priority { get; private set; }

        public GroupElement LeftHandSide { get; private set; }
        public IEnumerable<ActionElement> RightHandSide { get; private set; }
    }
}