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
            Context context = null;
            LastSeat lastSeat = null;
            Seating seating = null;

            When()
                .Match<Context>(() => context, c => c.State == ContextState.CheckDone)
                .Match<LastSeat>(() => lastSeat)
                .Match<Seating>(() => seating, s => s.RightSeatId == lastSeat.SeatId);

            Then()
                .Do(ctx => context.SetState(ContextState.PrintResults))
                .Do(ctx => ctx.Update(context));
        }
    }
}