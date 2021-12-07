using System;
using System.Diagnostics;

namespace NRules.RuleModel
{
    public static class RuleElementExtensions
    {
        /// <summary>
        /// Matches a rule element to an appropriate action based on the concrete type of the element.
        /// Type-safe implementation of discriminated union for rule elements.
        /// </summary>
        /// <param name="element">Rule element to match.</param>
        /// <param name="pattern">Action to invoke on the element if the element is a <see cref="PatternElement"/>.</param>
        /// <param name="aggregate">Action to invoke on the element if the element is an <see cref="AggregateElement"/>.</param>
        /// <param name="group">Action to invoke on the element if the element is a <see cref="GroupElement"/>.</param>
        /// <param name="exists">Action to invoke on the element if the element is an <see cref="ExistsElement"/>.</param>
        /// <param name="not">Action to invoke on the element if the element is a <see cref="NotElement"/>.</param>
        /// <param name="forall">Action to invoke on the element if the element is a <see cref="ForAllElement"/>.</param>
        [DebuggerStepThrough]
        public static void Match(this RuleElement element, 
            Action<PatternElement> pattern, 
            Action<AggregateElement> aggregate, 
            Action<GroupElement> group,
            Action<ExistsElement> exists, 
            Action<NotElement> not, 
            Action<ForAllElement> forall)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element), "Rule element cannot be null");

            switch (element)
            {
                case PatternElement pe:
                    pattern.Invoke(pe);
                    break;
                case GroupElement ge:
                    @group.Invoke(ge);
                    break;
                case AggregateElement ae:
                    aggregate.Invoke(ae);
                    break;
                case ExistsElement ee:
                    exists.Invoke(ee);
                    break;
                case NotElement ne:
                    not.Invoke(ne);
                    break;
                case ForAllElement fe:
                    forall.Invoke(fe);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element),
                        $"Unsupported rule element. ElementType={element.GetType()}");
            }
        }

        /// <summary>
        /// Matches a group element to an appropriate action based on the concrete type of the element.
        /// Type-safe implementation of discriminated union for group elements.
        /// </summary>
        /// <param name="element">Group element to match.</param>
        /// <param name="and">Action to invoke on the element if the element is a <see cref="AndElement"/>.</param>
        /// <param name="or">Action to invoke on the element if the element is a <see cref="OrElement"/>.</param>
        [DebuggerStepThrough]
        public static void Match(this GroupElement element, Action<AndElement> and, Action<OrElement> or)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element), "Group element cannot be null");

            switch (element)
            {
                case AndElement ae:
                    and.Invoke(ae);
                    break;
                case OrElement oe:
                    or.Invoke(oe);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element),
                        $"Unsupported group element. ElementType={element.GetType()}");
            }
        }
    }
}