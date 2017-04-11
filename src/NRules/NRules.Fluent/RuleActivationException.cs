using System;
/*using System.Runtime.Serialization;
using System.Security;*/

namespace NRules.Fluent
{
    /// <summary>
    /// Represents errors that occur when instantiating rule classes.
    /// </summary>
    //[Serializable]
    public class RuleActivationException : Exception
    {
        internal RuleActivationException(string message, Type ruleType, Exception innerException)
            : base(message, innerException)
        {
            RuleType = ruleType;
        }

        /*[SecuritySafeCritical]
        protected RuleActivationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            RuleType = (Type)info.GetValue("RuleType", typeof(Type));
        }*/

        /*[SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("RuleType", RuleType, typeof(Type));
        }*/

        /// <summary>
        /// Rule .NET type that caused exception.
        /// </summary>
        public Type RuleType { get; private set; }

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
