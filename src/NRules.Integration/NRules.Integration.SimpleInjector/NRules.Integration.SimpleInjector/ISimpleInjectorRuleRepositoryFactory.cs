using NRules.RuleModel;

namespace NRules.Integration.SimpleInjector;

public interface ISimpleInjectorRuleRepositoryFactory
{
    public IRuleRepository CreateNew(string name);
}