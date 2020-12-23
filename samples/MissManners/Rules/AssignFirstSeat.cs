using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.Samples.MissManners.Domain;

namespace NRules.Samples.MissManners.Rules
{
    [Name("AssignFirstSeat")]
    public class AssignFirstSeat : Rule
    {
        public override void Define()
        {
            Context context = default;
            Guest guest = default;
            Count count = default;

            When()
                .Match(() => context, c => c.State == ContextState.StartUp)
                .Match(() => guest)
                .Match(() => count);
            Then()
                .Do(ctx => AssignSeat(ctx, context, guest, count));
        }

        private void AssignSeat(IContext ctx, Context context, Guest guest, Count count)
        {
            var cnt = count.Value;

            var seating = new Seating(cnt, 0, true, 1, guest.Name, 1, guest.Name);
            ctx.Insert(seating);

            var path = new Path(cnt, 1, guest.Name);
            ctx.Insert(path);

            count.Increment();
            ctx.Update(count);

            context.SetState(ContextState.AssignSeats);
            ctx.Update(context);
        }
    }
}
