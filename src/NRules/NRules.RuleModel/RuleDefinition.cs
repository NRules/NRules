using System.Collections.Generic;
using System.Diagnostics;

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
        /// Rule priority.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Rule repeatability.
        /// </summary>
        RuleRepeatability Repeatability { get; }

        /// <summary>
        /// Tags applied to the rule.
        /// </summary>
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// Properties attached to the rule.
        /// </summary>
        PropertyMap Properties { get; }

        /// <summary>
        /// Rule's dependencies.
        /// </summary>
        DependencyGroupElement DependencyGroup { get; }

        /// <summary>
        /// Rule's filters, that determine whether rule's match triggers actions.
        /// </summary>
        FilterGroupElement FilterGroup { get; }

        /// <summary>
        /// Rule left-hand side (conditions).
        /// </summary>
        GroupElement LeftHandSide { get; }

        /// <summary>
        /// Rule right-hand side (actions).
        /// </summary>
        ActionGroupElement RightHandSide { get; }
    }

    [DebuggerDisplay("{Name} ({Priority})")]
    internal class RuleDefinition : IRuleDefinition
    {
        private readonly List<string> _tags;

        public static int DefaultPriority => 0;
        public static RuleRepeatability DefaultRepeatability => RuleRepeatability.Repeatable;

        public RuleDefinition(string name, string description, int priority, 
            RuleRepeatability repeatability, IEnumerable<string> tags, IEnumerable<RuleProperty> properties,
            DependencyGroupElement dependencies, GroupElement leftHandSide, FilterGroupElement filters, ActionGroupElement rightHandSide)
        {
            Name = name;
            Description = description;
            Repeatability = repeatability;
            Priority = priority;
            _tags = new List<string>(tags);
            Properties = new PropertyMap(properties);

            DependencyGroup = dependencies;
            LeftHandSide = leftHandSide;
            FilterGroup = filters;
            RightHandSide = rightHandSide;
        }

        public string Name { get; }
        public int Priority { get; }
        public string Description { get; }
        public RuleRepeatability Repeatability { get; }
        public PropertyMap Properties { get; }
        public DependencyGroupElement DependencyGroup { get; }
        public FilterGroupElement FilterGroup { get; }
        public GroupElement LeftHandSide { get; }
        public ActionGroupElement RightHandSide { get; }
        public IEnumerable<string> Tags => _tags;
    }
}