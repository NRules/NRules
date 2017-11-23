namespace NRules.AgendaFilters
{
    /// <summary>
    /// Filters rule matches (activations) before they are added to the agenda.
    /// If activation does not pass all the filters, it is not added to the agenda, and so the rule will not fire.
    /// </summary>
    public interface IAgendaFilter  {
        /// <summary>
        /// Tests rule activation whether it should be added to the agenda.
        /// </summary>
        /// <param name="activation">Rule activation.</param>
        /// <returns>Whether the activation should be added to the agenda - <c>true</c>, or not - <c>false</c>.</returns>
        bool Accept(Activation activation);
    }
}
