using NRules.Fluent;
using NRules.RuleModel;

namespace NRules.Integration.SimpleInjector;

public interface IRuleRepository<TService> : IRuleRepository 
    where TService : class;

public class RuleRepositoryWrapper<TService>: RuleRepository, IRuleRepository<TService>
    where TService : class
{
    public RuleRepositoryWrapper(IRuleActivator ruleActivator) : base(ruleActivator)
    {
    }
}
