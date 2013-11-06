using System;
using System.Diagnostics;

namespace NRules.RuleModel
{
    public static class RuleElementExtensions
    {
        [DebuggerStepThrough]
        public static void Match(this RuleElement element, Action<PatternElement> pattern, Action<AggregateElement> aggregate, Action<GroupElement> group)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element", "Rule element cannot be null");
            }
            else if (element is PatternElement)
            {
                pattern.Invoke((PatternElement) element);
            }
            else if (element is GroupElement)
            {
                group.Invoke((GroupElement) element);
            }
            else if (element is AggregateElement)
            {
                aggregate.Invoke((AggregateElement) element);
            }
            else
            {
                throw new ArgumentOutOfRangeException("element", string.Format("Unsupported rule element. ElementType={0}", element.GetType()));
            }
        }

        [DebuggerStepThrough]
        public static void Match(this GroupElement element, Action<AndElement> and, Action<OrElement> or, Action<NotElement> not, Action<ExistsElement> exists)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element", "Rule element cannot be null");
            }
            else if (element is AndElement)
            {
                and.Invoke((AndElement)element);
            }
            else if (element is OrElement)
            {
                or.Invoke((OrElement)element);
            }
            else if (element is NotElement)
            {
                not.Invoke((NotElement)element);
            }
            else if (element is ExistsElement)
            {
                exists.Invoke((ExistsElement)element);
            }
            else
            {
                throw new ArgumentOutOfRangeException("element", string.Format("Unsupported rule group element. ElementType={0}", element.GetType()));
            }
        }
    }
}