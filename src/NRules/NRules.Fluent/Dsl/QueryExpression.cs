namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Expression builder for queries.
    /// </summary>
    /// <typeparam name="TSource">Type of query source.</typeparam>
    public class QueryExpression<TSource> : IQuery<TSource>, ICollectQuery<TSource>, IOrderedQuery<TSource>
    {
        /// <summary>
        /// Constructs a query expression builder that wraps a <see cref="IQueryBuilder"/>.
        /// </summary>
        /// <param name="builder">Query builder to wrap.</param>
        public QueryExpression(IQueryBuilder builder)
        {
            Builder = builder;
        }

        /// <summary>
        /// Wrapped <see cref="IQueryBuilder"/>.
        /// </summary>
        public IQueryBuilder Builder { get; }
    }
}