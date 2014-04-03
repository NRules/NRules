using System;
using System.Linq;
using NRules.Fluent.Dsl;

namespace NRules.Fluent
{
    public interface IRuleMetadata
    {
        Type RuleType { get; }
        string Name { get; }
        string Description { get; }
        string[] Tags { get; }
        bool IsTagged(string tag);
    }

    internal class RuleMetadata : IRuleMetadata
    {
        public RuleMetadata(Type ruleType)
        {
            RuleType = ruleType;
            Name = GetAttributes<NameAttribute>().Select(a => a.Value).SingleOrDefault() ?? RuleType.FullName;
            Description = GetAttributes<DescriptionAttribute>().Select(a => a.Value).SingleOrDefault() ?? string.Empty;
            Tags = GetAttributes<TagAttribute>().Select(a => a.Value).ToArray();
        }

        public Type RuleType { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[] Tags { get; private set; }

        public bool IsTagged(string tag)
        {
            return Tags.Contains(tag);
        }

        private T[] GetAttributes<T>() where T : Attribute
        {
            return RuleType.GetCustomAttributes(true).OfType<T>().ToArray();
        }
    }
}