namespace NRules.Rule
{
    public interface IRuleDefinition
    {
        string Name { get; }
        int Priority { get; }
        GroupElement LeftHandSide { get; }
        ActionGroupElement RightHandSide { get; }
    }

    internal class RuleDefinition : IRuleDefinition
    {
        public static int DefaultPriority
        {
            get { return 0; }
        }

        public RuleDefinition(string name, int priority, GroupElement leftHandSide, ActionGroupElement rightHandSide)
        {
            Name = name;
            Priority = priority;
            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide;
        }

        public string Name { get; private set; }
        public int Priority { get; private set; }

        public GroupElement LeftHandSide { get; private set; }
        public ActionGroupElement RightHandSide { get; private set; }
    }
}