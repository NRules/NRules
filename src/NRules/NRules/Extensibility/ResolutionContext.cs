using NRules.RuleModel;

namespace NRules.Extensibility
{
    /// <summary>
    /// Context for dependency resolution.
    /// </summary>
    public interface IResolutionContext
    {
        /// <summary>
        /// Rules engine session that requested dependency resolution.
        /// </summary>
        ISession Session { get; }

        /// <summary>
        /// Rule that requested dependency resolution.
        /// </summary>
        IRuleDefinition Rule { get; }
    }

    internal class ResolutionContext : IResolutionContext
    {
        public ResolutionContext(ISession session, IRuleDefinition rule)
        {
            Session = session;
            Rule = rule;
        }

        public ISession Session { get; }
        public IRuleDefinition Rule { get; }
    }
}