using NRules.Fluent.Dsl;
using NRules.Samples.MissManners.Domain;

namespace NRules.Samples.MissManners.Rules
{
    [Name("Continue")]
    public class Continue : Rule
    {
        public override void Define()
        {
            Context context = null;

            Priority(0);

            When()
                .Match<Context>(() => context, c => c.State == ContextState.CheckDone);

            Then()
                .Do(ctx => context.SetState(ContextState.AssignSeats))
                .Do(ctx => ctx.Update(context));
        }
    }
}