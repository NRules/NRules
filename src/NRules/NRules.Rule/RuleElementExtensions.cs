using System;

namespace NRules.Rule
{
    public static class RuleElementExtensions
    {
        public static void Match(this RuleElement element, Action<PatternElement> pattern, Action<GroupElement> group, Action<AggregateElement> aggregate)
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

        public static void Match(this GroupElement group, Action<GroupElement> and, Action<GroupElement> or, Action<GroupElement> not, Action<GroupElement> exists)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group", "Group element cannot be null");
            }
            else if (group.GroupType == GroupType.And)
            {
                and.Invoke(group);
            }
            else if (group.GroupType == GroupType.Or)
            {
                or.Invoke(group);
            }
            else if (group.GroupType == GroupType.Not)
            {
                not.Invoke(group);
            }
            else if (group.GroupType == GroupType.Exists)
            {
                exists.Invoke(group);
            }
            else
            {
                throw new ArgumentOutOfRangeException("group", string.Format("Unsupported group type. GroupType={0}", group.GroupType));
            }
        }
    }
}