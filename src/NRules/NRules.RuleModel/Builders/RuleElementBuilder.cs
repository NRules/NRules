using System.Threading;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Base class for rule element builders.
    /// </summary>
    public abstract class RuleElementBuilder
    {
        private static int _declarationCounter = 0;

        protected string DeclarationName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                var counter = Interlocked.Increment(ref _declarationCounter);
                return $"$var{counter}$";
            }
            return name;
        }
    }
}