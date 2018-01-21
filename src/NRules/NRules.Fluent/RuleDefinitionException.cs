using System;

namespace NRules.Fluent
{
    /// <summary>
    /// Represents errors that occur while building rule definition using fluent DSL.
    /// </summary>
#if (NET45 || NETSTANDARD2_0)
    [System.Serializable]
#endif
    public class RuleDefinitionException : Exception
    {
        internal RuleDefinitionException(string message, Type ruleType, Exception innerException)
            : base(message, innerException)
        {
            RuleType = ruleType;
        }

#if (NET45 || NETSTANDARD2_0)
        [System.Security.SecuritySafeCritical]
        protected RuleDefinitionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            RuleType = (Type)info.GetValue("RuleType", typeof(Type));
        }

        [System.Security.SecurityCritical]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            base.GetObjectData(info, context);
            info.AddValue("RuleType", RuleType, typeof(Type));
        }
#endif

        /// <summary>
        /// Rule .NET type that caused exception.
        /// </summary>
        public Type RuleType { get; }

        public override string Message
        {
            get
            {
                string message = base.Message + Environment.NewLine + RuleType.FullName;
                return message;
            }
        }
    }
}
