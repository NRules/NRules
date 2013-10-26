using System;

namespace NRules.Dsl
{
    /// <summary>
    /// Defines rule's priority (salience). Default is 0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RulePriorityAttribute : Attribute
    {
        /// <summary>
        /// Defines rule's priority.
        /// </summary>
        /// <param name="priority">Rule's priority value.</param>
        public RulePriorityAttribute(int priority)
        {
            Priority = priority;
        }

        /// <summary>
        /// Rule's priority.
        /// </summary>
        public int Priority { get; private set; }
    }
}