using System;

namespace NRules.Fluent
{
    /// <summary>
    /// Represents errors that occur when instantiating rule classes.
    /// </summary>
#if (NET45 || NETSTANDARD2_0)
    [System.Serializable]
#endif
    public class RuleActivationException : Exception
    {
        internal RuleActivationException(string message, Type ruleType, Exception innerException)
            : base(message, innerException)
        {
            RuleTypeName = ruleType.AssemblyQualifiedName;
        }

#if (NET45 || NETSTANDARD2_0)
        [System.Security.SecuritySafeCritical]
        protected RuleActivationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
#endif

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
