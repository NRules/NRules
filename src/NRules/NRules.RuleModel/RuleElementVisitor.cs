namespace NRules.RuleModel
{
    /// <summary>
    /// Visitor to traverse rule definition (or its part).
    /// </summary>
    /// <typeparam name="TContext">Traversal context.</typeparam>
    public class RuleElementVisitor<TContext>
    {
        public void Visit(TContext context, RuleElement element)
        {
            element.Accept(context, this);
        }

        protected internal virtual void VisitPattern(TContext context, PatternElement element)
        {
            foreach (ConditionElement condition in element.Conditions)
            {
                condition.Accept(context, this);
            }
            element.Source?.Accept(context, this);
        }

        protected internal virtual void VisitBinding(TContext context, BindingElement element)
        {
        }

        protected internal virtual void VisitCondition(TContext context, ConditionElement element)
        {
        }

        protected internal virtual void VisitAggregate(TContext context, AggregateElement element)
        {
            foreach (var expression in element.Expressions)
            {
                expression.Accept(context, this);
            }
            element.Source?.Accept(context, this);
        }

        protected internal virtual void VisitNamedExpression(TContext context, NamedExpressionElement element)
        {
        }

        protected internal virtual void VisitNot(TContext context, NotElement element)
        {
            element.Source.Accept(context, this);
        }

        protected internal virtual void VisitExists(TContext context, ExistsElement element)
        {
            element.Source.Accept(context, this);
        }

        protected internal virtual void VisitForAll(TContext context, ForAllElement element)
        {
            element.BasePattern.Accept(context, this);
            foreach (PatternElement pattern in element.Patterns)
            {
                pattern.Accept(context, this);
            }
        }

        protected internal virtual void VisitAnd(TContext context, AndElement element)
        {
            VisitGroup(context, element);
        }

        protected internal virtual void VisitOr(TContext context, OrElement element)
        {
            VisitGroup(context, element);
        }

        private void VisitGroup(TContext context, GroupElement element)
        {
            foreach (RuleLeftElement childElement in element.ChildElements)
            {
                childElement.Accept(context, this);
            }
        }

        protected internal virtual void VisitActionGroup(TContext context, ActionGroupElement element)
        {
            foreach (ActionElement action in element.Actions)
            {
                action.Accept(context, this);
            }
        }

        protected internal virtual void VisitAction(TContext context, ActionElement element)
        {
        }

        protected internal virtual void VisitDependencyGroup(TContext context, DependencyGroupElement element)
        {
            foreach (DependencyElement dependency in element.Dependencies)
            {
                dependency.Accept(context, this);
            }
        }
        
        protected internal virtual void VisitDependency(TContext context, DependencyElement element)
        {
        }

        protected internal virtual void VisitFilterGroup(TContext context, FilterGroupElement element)
        {
            foreach (FilterElement filter in element.Filters)
            {
                filter.Accept(context, this);
            }
        }

        protected internal virtual void VisitFilter(TContext context, FilterElement element)
        {
        }
    }
}