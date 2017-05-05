using System;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating expressions as part of rules execution.
    /// </summary>
#if NET45
    [System.Serializable]
#endif
    public class RuleExpressionEvaluationException : RuleExecutionException
    {
        internal RuleExpressionEvaluationException(string message, string expression, Exception innerException)
            : base(message, innerException)
        {
            Expression = expression;
        }

#if NET45
        [System.Security.SecuritySafeCritical]
        protected RuleExpressionEvaluationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            Expression = info.GetString("Expression");
        }

        [System.Security.SecurityCritical]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("Expression", Expression, typeof(string));
        }
#endif

        /// <summary>
        /// Expression that caused exception.
        /// </summary>
        public string Expression { get; private set; }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (!string.IsNullOrEmpty(Expression))
                {
                    return message + Environment.NewLine + Expression;
                }
                return message;
            }
        } 
    }
}