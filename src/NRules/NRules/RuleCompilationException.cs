using System;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while compiling a rule.
    /// </summary>
#if NET45
    [System.Serializable]
#endif
    public class RuleCompilationException : Exception
    {
        internal RuleCompilationException(string message, string ruleName, Exception innerException)
            : base(message, innerException)
        {
            RuleName = ruleName;
        }

#if NET45
        [System.Security.SecuritySafeCritical]
        protected RuleCompilationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            RuleName = info.GetString("RuleName");
        }

        [System.Security.SecurityCritical]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("RuleName", RuleName, typeof(string));
        }
#endif

        /// <summary>
        /// Rule that caused exception.
        /// </summary>
        public string RuleName { get; private set; }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (!string.IsNullOrEmpty(RuleName))
                {
                    return message + Environment.NewLine + RuleName;
                }
                return message;
            }
        } 
    }
}