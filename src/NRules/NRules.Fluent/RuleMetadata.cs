using System;
using System.Linq;
using NRules.Fluent.Dsl;

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

        /// <summary>
        /// Tests if the rule is tagged with a given tag.
        /// </summary>
        /// <param name="tag">Tag to test.</param>
        /// <returns><c>true</c> if the rule is tagged, <c>false</c> otherwise.</returns>
        bool IsTagged(string tag);
    }

    internal class RuleMetadata : IRuleMetadata
    {
        private readonly Type _ruleType;
        private readonly string _name;
        private readonly string _description;
        private readonly string[] _tags;

        public RuleMetadata(Type ruleType)
        {
            _ruleType = ruleType;
            _name = GetAttributes<NameAttribute>().Select(a => a.Value).SingleOrDefault() ?? RuleType.FullName;
            _description = GetAttributes<DescriptionAttribute>().Select(a => a.Value).SingleOrDefault() ?? string.Empty;
            _tags = GetAttributes<TagAttribute>().Select(a => a.Value).ToArray();
        }

        public Type RuleType
        {
            get { return _ruleType; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string[] Tags
        {
            get { return _tags; }
        }

        public bool IsTagged(string tag)
        {
            return _tags.Contains(tag);
        }

        private T[] GetAttributes<T>() where T : Attribute
        {
            return _ruleType.GetCustomAttributes(true).OfType<T>().ToArray();
        }       
    }
}