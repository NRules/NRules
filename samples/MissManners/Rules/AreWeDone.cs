using NRules.Fluent.Dsl;
using NRules.Samples.MissManners.Domain;

namespace NRules.Samples.MissManners.Rules
{
    [Name("AreWeDone")]
    [Priority(1)]
    public class AreWeDone : Rule
    {
        public override void Define()
        {
            Context context = default;
            LastSeat lastSeat = default;
            Seating seating = default;

            When()
                .Match(() => context, c => c.State == ContextState.CheckDone)
                .Match(() => lastSeat)
                .Match(() => seating, s => s.RightSeatId == lastSeat.SeatId);

            Then()
                .Do(ctx => context.SetState(ContextState.PrintResults))
                .Do(ctx => ctx.Update(context));
        }
    }
}