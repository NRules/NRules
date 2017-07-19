using System;
using NRules.RuleModel;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Sets rule's repeatability, that is, how it behaves when it is activated with the same set of facts multiple times, 
    /// which is important for recursion control. By default rules are <see cref="RuleRepeatability.Repeatable"/>, 
    /// which means a rule will fire every time it is activated with the same set of facts.
    /// If repeatability is set to <see cref="RuleRepeatability.NonRepeatable"/> then the rule will not fire with the same combination of facts, 
    /// unless that combination was previously deactivated (i.e. through retraction).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RepeatabilityAttribute : Attribute
    {
        public RepeatabilityAttribute(RuleRepeatability value)
        {
            Value = value;
        }

        internal RuleRepeatability Value { get; }
    }
}