using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.Samples.MissManners.Domain;

namespace NRules.Samples.MissManners.Rules
{
    [Name("PathDone")]
    public class PathDone : Rule
    {
        public override void Define()
        {
            Context context = default;
            Seating seating = default;

            When()
                .Match(() => context, c => c.State == ContextState.MakePath)
                .Match(() => seating, s => !s.PathDone);

            Then()
                .Do(ctx => CompletePath(ctx, context, seating));
        }

        private void CompletePath(IContext ctx, Context context, Seating seating)
        {
            seating.SetPathDone();
            ctx.Update(seating);

            context.SetState(ContextState.CheckDone);
            ctx.Update(context);
        }
    }
}