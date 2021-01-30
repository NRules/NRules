namespace NRules.AgendaFilters
{
    /// <summary>
    /// Base interface for stateful agenda filters that store some state related to
    /// the activations and need to update that state during the activation lifecycle.
    /// </summary>
    public interface IStatefulAgendaFilter : IAgendaFilter
    {
        /// <summary>
        /// Called by the engine when activation is selected from the agenda,
        /// before rule's actions are executed.
        /// </summary>
        /// <param name="context">Agenda context.</param>
        /// <param name="activation">Rule activation.</param>
        void OnFiring(AgendaContext context, Activation activation);
    }
}