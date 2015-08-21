using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule repeatability.
    /// </summary>
    public enum RuleRepeatability
    {
        /// <summary>
        /// Rule will fire every time a matching set of facts is inserted or updated.
        /// </summary>
        Repeatable = 0,

        /// <summary>
        /// Rule will not fire with the same combination of facts, unless that combination was previously deactivated (i.e. through retraction).
        /// </summary>
        NonRepeatable = 1,
    }

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
        /// Rule description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Rule repeatability.
        /// </summary>
        RuleRepeatability Repeatability { get; }

        /// <summary>
        /// Tags applied to the rule.
        /// </summary>
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// Rule priority.
        /// </summary>
        PriorityElement Priority { get; }

        /// <summary>
        /// Rule's dependencies.
        /// </summary>
        DependencyGroupElement DependencyGroup { get; }

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
        private readonly string _description;
        private readonly PriorityElement _priority;
        private readonly RuleRepeatability _repeatability;
        private readonly DependencyGroupElement _dependencies;
        private readonly GroupElement _leftHandSide;
        private readonly ActionGroupElement _rightHandSide;

        public static int DefaultPriority
        {
            get { return 0; }
        }

        public static RuleRepeatability DefaultRepeatability
        {
            get { return RuleRepeatability.Repeatable; }
        }

        public RuleDefinition(string name, string description, RuleRepeatability repeatability, IEnumerable<string> tags,
            PriorityElement priority, DependencyGroupElement dependencies, GroupElement leftHandSide, ActionGroupElement rightHandSide)
        {
            _name = name;
            _description = description;
            _repeatability = repeatability;
            _tags = new List<string>(tags);

            _priority = priority;
            _dependencies = dependencies;
            _leftHandSide = leftHandSide;
            _rightHandSide = rightHandSide;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        public RuleRepeatability Repeatability
        {
            get { return _repeatability; }
        }

        public IEnumerable<string> Tags
        {
            get { return _tags; }
        }

        public PriorityElement Priority
        {
            get { return _priority; }
        }

        public DependencyGroupElement DependencyGroup
        {
            get { return _dependencies; }
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