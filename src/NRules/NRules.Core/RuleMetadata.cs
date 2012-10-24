using System;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Core
{
    internal class RuleMetadata
    {
        public RuleMetadata(IRule ruleInstance)
        {
            ApplyAttribute<RulePriorityAttribute>(ruleInstance, a => Priority = a.Priority);
        }

        private void ApplyAttribute<T>(IRule ruleInstance, Action<T> attributeFoundAction) where T : Attribute
        {
            var attribute = Attribute.GetCustomAttributes(ruleInstance.GetType(), true).OfType<T>().FirstOrDefault();
            if (attribute != null)
            {
                attributeFoundAction(attribute);
            }
        }

        public int? Priority { get; private set; }
    }
}