using MissManners.Domain;
using NRules.Fluent.Dsl;

namespace MissManners.Rules
{
    [Name("AreWeDone")]
    public class AreWeDone : Rule
    {
        public override void Define()
        {
            Context context = null;
            LastSeat lastSeat = null;
            Seating seating = null;

            Priority(1);

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