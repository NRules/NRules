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
        /// <remarks>This method must not evaluate agenda expressions.</remarks>
        /// <param name="context">Agenda context.</param>
        /// <param name="activation">Rule activation.</param>
        void Select(AgendaContext context, Activation activation);

        /// <summary>
        /// Called by the engine when activation is removed from the agenda.
        /// The agenda filter can use this method to remove any state associated with the activation.
        /// </summary>
        /// <remarks>This method must not evaluate agenda expressions.</remarks>
        /// <param name="context">Agenda context.</param>
        /// <param name="activation">Rule activation.</param>
        void Remove(AgendaContext context, Activation activation);
    }
}