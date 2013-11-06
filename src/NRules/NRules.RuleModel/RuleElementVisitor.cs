namespace NRules.RuleModel
{
    public class RuleElementVisitor
    {
        public void Visit(RuleElement element)
        {
            element.Accept(this);
        }

        protected internal virtual void VisitPattern(PatternElement element)
        {
            foreach (ConditionElement condition in element.Conditions)
            {
                condition.Accept(this);
            }
            if (element.Source != null)
            {
                element.Source.Accept(this);
            }
        }

        protected internal virtual void VisitAggregate(AggregateElement element)
        {
            if (element.Source != null)
            {
                element.Source.Accept(this);
            }
        }

        protected internal virtual void VisitActionGroup(ActionGroupElement element)
        {
            foreach (ActionElement action in element.Actions)
            {
                action.Accept(this);
            }
        }

        protected internal virtual void VisitCondition(ConditionElement element)
        {
        }

        protected internal virtual void VisitAction(ActionElement element)
        {
        }

        protected internal void VisitAnd(AndElement element)
        {
            VisitGroup(element);
        }
        
        protected internal void VisitOr(OrElement element)
        {
            VisitGroup(element);
        }
        
        protected internal void VisitNot(NotElement element)
        {
            VisitGroup(element);
        }
        
        protected internal void VisitExists(ExistsElement element)
        {
            VisitGroup(element);
        }

        private void VisitGroup(GroupElement element)
        {
            foreach (RuleLeftElement childElement in element.ChildElements)
            {
                childElement.Accept(this);
            }
        }
    }
}