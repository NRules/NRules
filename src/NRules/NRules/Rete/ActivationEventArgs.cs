using System;

namespace NRules.Rete
{
    internal class ActivationEventArgs : EventArgs
    {
        public ActivationEventArgs(Activation activation)
        {
            Activation = activation;
        }

        public Activation Activation { get; private set; }
    }
}