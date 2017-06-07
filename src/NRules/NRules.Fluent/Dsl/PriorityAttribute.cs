using System;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Sets rule's priority.
    /// If multiple rules get activated at the same time, rules with higher priority get executed first.
    /// Priority value can be positive, negative or zero.
    /// Default priority is zero.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class PriorityAttribute : Attribute
    {
        public PriorityAttribute(int value)
        {
            Value = value;
        }

        internal int Value { get; }
    }
}