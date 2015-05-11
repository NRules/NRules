using System;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator factory that creates new instances of the aggregator via a default constructor.
    /// </summary>
    /// <typeparam name="T">Type of aggregator.</typeparam>
    internal class DefaultAggregatorFactory<T> : IAggregatorFactory, IEquatable<DefaultAggregatorFactory<T>> where T : IAggregator, new()
    {
        private readonly Type _aggregatorType = typeof(T);

        public IAggregator Create()
        {
            return new T();
        }

        public bool Equals(DefaultAggregatorFactory<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _aggregatorType.Equals(other._aggregatorType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DefaultAggregatorFactory<T>) obj);
        }

        public override int GetHashCode()
        {
            return _aggregatorType.GetHashCode();
        }
    }
}