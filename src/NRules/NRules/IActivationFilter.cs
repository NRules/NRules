namespace NRules
{
    internal interface IActivationFilter
    {
        bool Accept(Activation activation);
    }
}
