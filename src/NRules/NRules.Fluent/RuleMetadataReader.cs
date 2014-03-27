using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;

namespace NRules.Fluent
{
    internal class RuleMetadataReader
    {
        private readonly Type _ruleType;

        public RuleMetadataReader(Type ruleType)
        {
            _ruleType = ruleType;
        }

        public string Name
        {
            get 
            {
                var attribute = GetAttributes<NameAttribute>().SingleOrDefault();
                string value = attribute != null ? attribute.Value : _ruleType.FullName;
                return value;
            }
        }

        public string Description
        {
            get 
            {
                var attribute = GetAttributes<DescriptionAttribute>().SingleOrDefault();
                string value = attribute != null ? attribute.Value : string.Empty;
                return value;
            }
        }

        public IEnumerable<string> Tags
        {
            get 
            {
                var attributes = GetAttributes<TagAttribute>();
                var values = attributes.Select(a => a.Value);
                return values;
            }
        }

        private T[] GetAttributes<T>() where T: Attribute
        {
            return _ruleType.GetCustomAttributes(true).OfType<T>().ToArray();
        }
    }
}
