namespace NRules.RuleModel;

/// <summary>
/// Interface for facts that provide custom identity.
/// </summary>
public interface IIdentityProvider
{
    /// <summary>
    /// Retrieves the identity of the fact.
    /// Equality of the identity objects is used to compare facts for the purpose of
    /// inserting, updating and removing them within the rule session.
    /// </summary>
    /// <returns>Fact identity.</returns>
    object GetIdentity();
}