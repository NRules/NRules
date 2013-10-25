using System;

namespace NRules.Fluent.Dsl
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RulePriorityAttribute : Attribute
    {
        public RulePriorityAttribute(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; private set; }
    }
}