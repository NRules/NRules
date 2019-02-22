using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    public class RuleTransformation : RuleElementVisitor<RuleTransformation.Context>
    {
        public class Context
        {
            private readonly Stack<RuleElement> _elements = new Stack<RuleElement>();

            internal bool IsModified { get; set; }

            internal RuleElement Pop()
            {
                return _elements.Pop();
            }

            internal void Push(RuleElement element)
            {
                _elements.Push(element);
            }
        }

        public IRuleDefinition Transform(IRuleDefinition rule)
        {
            var lhsContext = new Context();
            var lhs = Transform<GroupElement>(lhsContext, rule.LeftHandSide);

            var rhsContext = new Context();
            var rhs = Transform<ActionGroupElement>(rhsContext, rule.RightHandSide);

            if (lhsContext.IsModified || rhsContext.IsModified)
            {
                var transformedRule = Element.RuleDefinition(rule.Name, rule.Description, rule.Priority,
                    rule.Repeatability, rule.Tags, rule.Properties, rule.DependencyGroup, lhs, rule.FilterGroup, rhs);
                return transformedRule;
            }
            return rule;
        }

        private void Result(Context context, RuleElement element)
        {
            context.Pop();
            context.Push(element);
            context.IsModified = true;
        }

        private T Transform<T>(Context context, RuleElement element) where T : RuleElement
        {
            if (element == null) return null;

            bool savedIsModified = context.IsModified;
            context.IsModified = false;

            context.Push(element);
            Visit(context, element);
            context.IsModified |= savedIsModified;

            return (T)context.Pop();
        }

        protected internal override void VisitPattern(Context context, PatternElement element)
        {
            var conditions = element.Conditions.Select(x => Transform<ConditionElement>(context, x)).ToList();
            var source = Transform<PatternSourceElement>(context, element.Source);
            if (context.IsModified)
            {
                var newElement = Element.Pattern(element.Declaration, conditions, source);
                Result(context, newElement);
            }
        }

        protected internal override void VisitBinding(Context context, BindingElement element)
        {
        }

        protected internal override void VisitAggregate(Context context, AggregateElement element)
        {
            var source = Transform<PatternElement>(context, element.Source);
            if (context.IsModified)
            {
                var aggregateExpressions = element.Expressions.Select(x => new KeyValuePair<string, LambdaExpression>(x.Name, x.Expression));
                var newElement = Element.Aggregate(element.ResultType, element.Name, aggregateExpressions, source, element.CustomFactoryType);
                Result(context, newElement);
            }
        }

        protected internal override void VisitActionGroup(Context context, ActionGroupElement element)
        {
            var actions = element.Actions.Select(x => Transform<ActionElement>(context, x)).ToList();
            if (context.IsModified)
            {
                var newElement = Element.ActionGroup(actions);
                Result(context, newElement);
            }
        }

        protected internal override void VisitCondition(Context context, ConditionElement element)
        {
        }

        protected internal override void VisitAction(Context context, ActionElement element)
        {
        }

        protected internal override void VisitAnd(Context context, AndElement element)
        {
            var childElements = element.ChildElements.Select(x => Transform<RuleLeftElement>(context, x)).ToList();
            if (CollapseSingleGroup(context, childElements)) return;
            if (SplitOrGroup(context, element, childElements)) return;
            if (context.IsModified)
            {
                var newElement = Element.AndGroup(childElements);
                Result(context, newElement);
            }
        }

        protected internal override void VisitOr(Context context, OrElement element)
        {
            var childElements = element.ChildElements.Select(x => Transform<RuleLeftElement>(context, x)).ToList();
            if (CollapseSingleGroup(context, childElements)) return;
            if (MergeOrGroups(context, element, childElements)) return;
            if (context.IsModified)
            {
                var newElement = Element.OrGroup(childElements);
                Result(context, newElement);
            }
        }

        protected internal override void VisitNot(Context context, NotElement element)
        {
            var source = Transform<RuleLeftElement>(context, element.Source);
            if (context.IsModified)
            {
                var newElement = Element.Not(source);
                Result(context, newElement);
            }
        }

        protected internal override void VisitExists(Context context, ExistsElement element)
        {
            var source = Transform<RuleLeftElement>(context, element.Source);
            if (context.IsModified)
            {
                var newElement = Element.Exists(source);
                Result(context, newElement);
            }
        }

        protected internal override void VisitForAll(Context context, ForAllElement element)
        {
            var basePattern = Transform<PatternElement>(context, element.BasePattern);
            var patterns = element.Patterns.Select(x => Transform<PatternElement>(context, x)).ToList();

            //forall -> not(base and not(patterns))

            Declaration baseDeclaration = basePattern.Declaration;
            var baseParameter = baseDeclaration.ToParameterExpression();

            var negatedPatterns = new List<RuleLeftElement>();
            foreach (var pattern in patterns)
            {
                var parameter = pattern.Declaration.ToParameterExpression();

                var conditions = new List<ConditionElement>
                {
                    Element.Condition(
                        Expression.Lambda(
                            Expression.ReferenceEqual(baseParameter, parameter),
                            baseParameter, parameter))
                };
                conditions.AddRange(pattern.Conditions);

                negatedPatterns.Add(
                    Element.Not(
                        Element.Pattern(pattern.Declaration, conditions, pattern.Source)
                    ));
            }

            var result = Element.Not(
                Element.AndGroup(
                    new RuleLeftElement[] { basePattern }.Concat(negatedPatterns)));
            Result(context, result);
        }

        private bool CollapseSingleGroup(Context context, IList<RuleLeftElement> childElements)
        {
            if (childElements.Count == 1 &&
                childElements.Single() is GroupElement)
            {
                Result(context, childElements.Single());
                return true;
            }
            return false;
        }

        private bool SplitOrGroup(Context context, AndElement element, IList<RuleLeftElement> childElements)
        {
            if (!childElements.OfType<OrElement>().Any()) return false;

            var groups = new List<IList<RuleLeftElement>>();
            groups.Add(new List<RuleLeftElement>());
            ExpandOrElements(groups, childElements, 0);

            var andElements = groups.Select(Element.AndGroup).ToList();
            var orElement = Element.OrGroup(andElements);
            Result(context, orElement);
            return true;
        }

        private bool MergeOrGroups(Context context, OrElement element, IList<RuleLeftElement> childElements)
        {
            if (!childElements.OfType<OrElement>().Any()) return false;
            var newChildElements = new List<RuleLeftElement>();
            foreach (var childElement in childElements)
            {
                if (childElement is OrElement childOrElement)
                {
                    newChildElements.AddRange(childOrElement.ChildElements);
                }
                else
                {
                    newChildElements.Add(childElement);
                }

            }
            var orElement = Element.OrGroup(newChildElements);
            Result(context, orElement);
            return true;
        }

        private void ExpandOrElements(IList<IList<RuleLeftElement>> groups, IList<RuleLeftElement> childElements, int index)
        {
            if (index == childElements.Count) return;

            var currentElement = childElements[index];
            var orElement = currentElement as OrElement;

            var count = groups.Count;
            for (int i = 0; i < count; i++)
            {
                if (orElement != null)
                {
                    int offset = groups.Count;
                    var orElementChildren = orElement.ChildElements.ToList();
                    var firstChild = orElementChildren.First();
                    var restChildren = orElementChildren.Skip(1).ToList();
                    for (int j = 0; j < restChildren.Count; j++)
                    {
                        groups.Add(new List<RuleLeftElement>(groups[i]));
                        groups[offset + j].Add(restChildren[j]);
                    }
                    groups[i].Add(firstChild);
                }
                else
                {
                    groups[i].Add(currentElement);
                }
            }

            ExpandOrElements(groups, childElements, index + 1);
        }
    }
}