using System;
using System.Linq;
using System.Reflection;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Fluent
{
    /// <summary>
    /// Metadata associated with a rule defined using internal DSL.
    /// </summary>
    public interface IRuleMetadata
    {
        /// <summary>
        /// Rule's .NET type.
        /// </summary>
        Type RuleType { get; }

        /// <summary>
        /// Rule's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Rule's description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Tags applied to the rule.
        /// </summary>
        string[] Tags { get; }
    }

    /// <summary>
    /// Metadata associated with a rule defined using internal DSL.
    /// </summary>
    public class RuleMetadata : IRuleMetadata
    {
        private readonly Type _ruleType;
        private readonly string _name;
        private readonly string _description;
        private readonly string[] _tags;
        private readonly int? _priority;
        private readonly RuleRepeatability? _repeatability;

        public RuleMetadata(Type ruleType)
        {
            _ruleType = ruleType;
            _name = GetAttributes<NameAttribute>().Select(a => a.Value).SingleOrDefault() ?? RuleType.FullName;
            _description = GetAttributes<DescriptionAttribute>().Select(a => a.Value).SingleOrDefault() ?? string.Empty;
            _tags = GetAttributes<TagAttribute>().Select(a => a.Value).ToArray();
            _priority = GetAttributes<PriorityAttribute>().SingleNullable(a => a.Value);
            _repeatability = GetAttributes<RepeatabilityAttribute>().SingleNullable(a => a.Value);
        }

        /// <summary>
        /// Rule's .NET type.
        /// </summary>
        public Type RuleType
        {
            get { return _ruleType; }
        }

        /// <summary>
        /// Rule's name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Rule's description.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Tags applied to the rule.
        /// </summary>
        public string[] Tags
        {
            get { return _tags; }
        }

        /// <summary>
        /// Rule's priority.
        /// </summary>
        public int? Priority
        {
            get { return _priority; }
        }

        /// <summary>
        /// Rule's repeatability.
        /// </summary>
        public RuleRepeatability? Repeatability
        {
            get { return _repeatability; }
        }

        private T[] GetAttributes<T>() where T : Attribute
        {
            var typeInfo = _ruleType.GetTypeInfo();
            return typeInfo.GetCustomAttributes(true).OfType<T>().ToArray();
        }
    }
}