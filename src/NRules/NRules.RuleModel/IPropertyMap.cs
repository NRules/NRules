using System.Collections.Generic;

namespace NRules.RuleModel;
public interface IPropertyMap : IReadOnlyCollection<RuleProperty>
{
    object this[string name] { get; }

    bool TryGetProperty(string name, out RuleProperty property);
}