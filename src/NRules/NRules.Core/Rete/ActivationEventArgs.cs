using System;

namespace NRules.Core.Rete
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