using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.Samples.MissManners.Domain;

namespace NRules.Samples.MissManners.Rules
{
    [Name("MakePath")]
    public class MakePath : Rule
    {
        public override void Define()
        {
            Context context = null;
            Seating seating = null;
            Path path = null;

            Priority(1);

            When()
                .Match<Context>(() => context, c => c.State == ContextState.MakePath)
                .Match<Seating>(() => seating, s => !s.PathDone)
                .Match<Path>(() => path, p => p.Id == seating.Pid)
                .Not<Path>(p => p.Id == seating.Id, p => p.GuestName == path.GuestName);

            Then()
                .Do(ctx => MakeNewPath(ctx, context, seating, path));
        }

        private void MakeNewPath(IContext ctx, Context context, Seating seating, Path path)
        {
            var newPath = new Path(seating.Id, path.SeatId, path.GuestName);
            ctx.Insert(newPath);
        }
    }
}
