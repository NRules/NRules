using System;
using System.Linq;
using NRules.Dsl;

namespace NRules.Inline
{
    internal static class RuleExtensions
    {
        public static void ApplyAttribute<T>(this IRule ruleInstance, Action<T> attributeFoundAction) where T : Attribute
        {
            var attribute = Attribute.GetCustomAttributes(ruleInstance.GetType(), true).OfType<T>().FirstOrDefault();
            if (attribute != null)
            {
                attributeFoundAction(attribute);
            }
        }
    }
}