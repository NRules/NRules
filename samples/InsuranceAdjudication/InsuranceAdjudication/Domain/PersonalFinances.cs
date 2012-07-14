namespace NRules.Core.IntegrationTests.Domain
{
    internal class PersonalFinances
    {
        private readonly decimal _liquidAssets;
        private readonly decimal _illiquidAssets;
        private readonly decimal _yearlyPreTaxIncome;
        private readonly decimal _totalDebt;

        public PersonalFinances(decimal liquidAssets, decimal illiquidAssets, decimal yearlyPreTaxIncome,
                                decimal totalDebt)
        {
            _liquidAssets = liquidAssets;
            _illiquidAssets = illiquidAssets;
            _yearlyPreTaxIncome = yearlyPreTaxIncome;
            _totalDebt = totalDebt;
        }

        public decimal LiquidAssets
        {
            get { return _liquidAssets; }
        }

        public decimal IlliquidAssets
        {
            get { return _illiquidAssets; }
        }

        public decimal YearlyPreTaxIncome
        {
            get { return _yearlyPreTaxIncome; }
        }

        public decimal TotalDebt
        {
            get { return _totalDebt; }
        }
    }
}