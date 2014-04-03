using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Production rule definition.
    /// </summary>
    public interface IRuleDefinition
    {
        /// <summary>
        /// Rule name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Rule priority.
        /// </summary>
        int Priority { get; }
        
        /// <summary>
        /// Rule description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Tags applied to the rule.
        /// </summary>
        IEnumerable<string> Tags { get; }
        
        /// <summary>
        /// Rule left hand side (conditions).
        /// </summary>
        GroupElement LeftHandSide { get; }

        /// <summary>
        /// Rule right hand side (actions).
        /// </summary>
        ActionGroupElement RightHandSide { get; }
    }

    internal class RuleDefinition : IRuleDefinition
    {
        private readonly List<string> _tags = new List<string>();

        public static int DefaultPriority
        {
            get { return 0; }
        }

        public RuleDefinition(string name, string description, IEnumerable<string> tags, int priority, 
            GroupElement leftHandSide, ActionGroupElement rightHandSide)
        {
            Name = name;
            Description = description;
            Priority = priority;

            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide;

            if (tags != null) _tags.AddRange(tags);
        }

        public string Name { get; private set; }
        public int Priority { get; private set; }
        public string Description { get; private set; }
        public IEnumerable<string> Tags { get { return _tags; } }

        public GroupElement LeftHandSide { get; private set; }
        public ActionGroupElement RightHandSide { get; private set; }
    }
}