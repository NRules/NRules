using System;

namespace NRules
{
    internal class ActivationEventArgs : EventArgs
    {
        public ActivationEventArgs(Activation activation)
        {
            Activation = activation;
        }

        public Activation Activation { get; }
    }
}