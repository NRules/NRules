using System;

namespace NRules.Core.Rules
{
    internal interface IAggregate
    {
        void Add(params object[] facts);
        void Modify(params object[] facts);
        void Remove(params object[] facts);
        event EventHandler<EventArgs> ResultAdded;
        event EventHandler<EventArgs> ResultModified;
        event EventHandler<EventArgs> ResultRemoved;
    }
}