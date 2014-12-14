using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Production rule definition in the canonical rule model.
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
        private readonly List<string> _tags;
        private readonly string _name;
        private readonly int _priority;
        private readonly string _description;
        private readonly GroupElement _leftHandSide;
        private readonly ActionGroupElement _rightHandSide;

        public static int DefaultPriority
        {
            get { return 0; }
        }

        public RuleDefinition(string name, string description, IEnumerable<string> tags, int priority, 
            GroupElement leftHandSide, ActionGroupElement rightHandSide)
        {
            _name = name;
            _description = description;
            _priority = priority;
            _tags = new List<string>(tags);

            _leftHandSide = leftHandSide;
            _rightHandSide = rightHandSide;
        }

        public string Name
        {
            get { return _name; }
        }

        public int Priority
        {
            get { return _priority; }
        }

        public string Description
        {
            get { return _description; }
        }

        public IEnumerable<string> Tags
        {
            get { return _tags; }
        }

        public GroupElement LeftHandSide
        {
            get { return _leftHandSide; }
        }

        public ActionGroupElement RightHandSide
        {
            get { return _rightHandSide; }
        }
    }
}