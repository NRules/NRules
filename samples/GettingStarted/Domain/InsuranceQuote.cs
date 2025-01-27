namespace Domain;

public class InsuranceQuote(Driver driver, decimal basePremium)
{
    public Driver Driver { get; } = driver;
    public decimal BasePremium { get; } = basePremium;
    public decimal FinalPremium { get; private set; } = basePremium;

    public void ApplySurcharge(decimal amount) => FinalPremium += amount;
    public void ApplyDiscount(decimal amount) => FinalPremium -= amount;
}