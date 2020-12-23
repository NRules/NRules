using System;
using NRules.Fluent.Dsl;
using NRules.Samples.SimpleRules.Domain;

namespace NRules.Samples.SimpleRules.Rules
{
    public class OrderAmountCalculationRule : Rule
    {
        public override void Define()
        {
            Order order = null;

            When()
                .Match(() => order);

            Filter()
                .OnChange(() => order.Quantity, () => order.UnitPrice, () => order.PercentDiscount);

            Then()
                .Do(ctx => ctx.Update(order, CalculateAmount));
        }

        private static void CalculateAmount(Order order)
        {
            order.Amount = order.UnitPrice * order.Quantity * (1.0 - order.PercentDiscount / 100.0);
            Console.WriteLine("Order amount calculated. Id={0}, Amount={1}", order.Id, order.Amount);
        }
    }
}
