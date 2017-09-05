namespace NRules
{
    internal interface IActivationFilter
    {
        bool Accept(Activation activation);
        void Remove(Activation activation);
    }
}
