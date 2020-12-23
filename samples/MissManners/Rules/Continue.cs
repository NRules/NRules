using NRules.Fluent.Dsl;
using NRules.Samples.MissManners.Domain;

namespace NRules.Samples.MissManners.Rules
{
    [Name("Continue")]
    public class Continue : Rule
    {
        public override void Define()
        {
            Context context = default;

            When()
                .Match(() => context, c => c.State == ContextState.CheckDone);

            Then()
                .Do(ctx => context.SetState(ContextState.AssignSeats))
                .Do(ctx => ctx.Update(context));
        }
    }
}