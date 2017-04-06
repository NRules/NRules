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
                var transformedRule = new RuleDefinition(rule.Name, rule.Description, rule.Priority, 
                    rule.Repeatability, rule.Tags, rule.Properties, rule.DependencyGroup, lhs, rhs);
                return transformedRule;
            }
            return rule;
        }

        internal void Result(Context context, RuleElement element)
        {
            context.Pop();
            context.Push(element);
            context.IsModified = true;
        }

        internal T Transform<T>(Context context, RuleElement element) where T : RuleElement
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
                var newElement = new PatternElement(element.Declaration, element.Declarations, conditions, source);
                Result(context, newElement);
            }
        }

        protected internal override void VisitAggregate(Context context, AggregateElement element)
        {
            var source = Transform<PatternElement>(context, element.Source);
            if (context.IsModified)
            {
                var newElement = new AggregateElement(element.Declarations, element.ResultType,
                    element.Name, element.ExpressionMap, element.AggregatorFactory, source);
                Result(context, newElement);
            }
        }

        protected internal override void VisitActionGroup(Context context, ActionGroupElement element)
        {
            var actions = element.Actions.Select(x => Transform<ActionElement>(context, x)).ToList();
            if (context.IsModified)
            {
                var newElement = new ActionGroupElement(element.Declarations, actions);
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
                var newElement = new AndElement(element.Declarations, childElements);
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
                var newElement = new OrElement(element.Declarations, childElements);
                Result(context, newElement);
            }
        }

        protected internal override void VisitNot(Context context, NotElement element)
        {
            var source = Transform<RuleLeftElement>(context, element.Source);
            if (context.IsModified)
            {
                var newElement = new NotElement(element.Declarations, source);
                Result(context, newElement);
            }
        }

        protected internal override void VisitExists(Context context, ExistsElement element)
        {
            var source = Transform<RuleLeftElement>(context, element.Source);
            if (context.IsModified)
            {
                var newElement = new ExistsElement(element.Declarations, source);
                Result(context, newElement);
            }
        }

        protected internal override void VisitForAll(Context context, ForAllElement element)
        {
            var basePattern = Transform<PatternElement>(context, element.BasePattern);
            var patterns = element.Patterns.Select(x => Transform<PatternElement>(context, x)).ToList();

            //forall -> not(base and not(patterns))
            var symbolTable = new SymbolTable(element.Declarations);

            var notBuilder = new NotBuilder(symbolTable);
            var groupBuilder = notBuilder.Group(GroupType.And);

            Declaration declaration = basePattern.Declaration;
            var basePatternBuilder = groupBuilder.Pattern(declaration);
            foreach (var condition in basePattern.Conditions)
            {
                basePatternBuilder.Condition(condition.Expression);
            }

            var baseParameter = basePatternBuilder.Declaration.ToParameterExpression();

            foreach (var pattern in patterns)
            {
                var patternBuilder = groupBuilder
                    .Not()
                    .Pattern(pattern.Declaration);

                var parameter = patternBuilder.Declaration.ToParameterExpression();
                //Join is required to correlate negated patterns with the base pattern
                patternBuilder.Condition(
                    Expression.Lambda(
                        Expression.ReferenceEqual(baseParameter, parameter),
                        baseParameter, parameter));

                foreach (var condition in pattern.Conditions)
                {
                    patternBuilder.Condition(condition.Expression);
                }
            }

            IBuilder<NotElement> builder = notBuilder;
            Result(context, builder.Build());
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

            var andElements = groups.Select(x => new AndElement(element.Declarations, x)).ToList();
            var orElement = new OrElement(element.Declarations, andElements);
            Result(context, orElement);
            return true;
        }

        private bool MergeOrGroups(Context context, OrElement element, IList<RuleLeftElement> childElements)
        {
            if (!childElements.OfType<OrElement>().Any()) return false;
            var newChildElements = new List<RuleLeftElement>();
            foreach (var childElement in childElements)
            {
                var childOrElement = childElement as OrElement;
                if (childOrElement != null)
                {
                    newChildElements.AddRange(childOrElement.ChildElements);
                }
                else
                {
                    newChildElements.Add(childElement);
                }

            }
            var orElement = new OrElement(element.Declarations, newChildElements);
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