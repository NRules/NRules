using System;

namespace NRules.Fluent
{
    /// <summary>
    /// Represents errors that occur while building rule definition using fluent DSL.
    /// </summary>
    [Serializable]
    public class RuleDefinitionException : Exception
    {
        internal RuleDefinitionException(string message, Type ruleType, Exception innerException)
            : base(message, innerException)
        {
            RuleTypeName = ruleType.AssemblyQualifiedName;
        }

        [System.Security.SecuritySafeCritical]
        protected RuleDefinitionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            RuleTypeName = info.GetString("RuleTypeName");
        }

        [System.Security.SecurityCritical]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            base.GetObjectData(info, context);
            info.AddValue("RuleTypeName", RuleTypeName, typeof(string));
        }

        /// <summary>
        /// Rule CLR type that caused exception.
        /// </summary>
        public Type RuleType => Type.GetType(RuleTypeName);

        /// <summary>
        /// Rule CLR type name that caused exception.
        /// </summary>
        public string RuleTypeName { get; }

        public override string Message
        {
            get
            {
                string message = base.Message + Environment.NewLine + RuleTypeName;
                return message;
            }
        }
    }
}
