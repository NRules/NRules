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
        private readonly ISession _session;
        private readonly IRuleDefinition _rule;

        public ResolutionContext(ISession session, IRuleDefinition rule)
        {
            _session = session;
            _rule = rule;
        }

        public ISession Session { get { return _session; } }
        public IRuleDefinition Rule { get { return _rule; } }
    }
}