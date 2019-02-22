using System.ComponentModel;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Root of the query method chain.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Internal query builder.
        /// This method is intended for framework use only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IQueryBuilder Builder { get; }
    }

    /// <summary>
    /// Intermediate query chain element.
    /// </summary>
    /// <typeparam name="TSource">Type of the element the query operates on.</typeparam>
    public interface IQuery<out TSource>
    {
        /// <summary>
        /// Internal query builder.
        /// This method is intended for framework use only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IQueryBuilder Builder { get; }
    }

    /// <summary>
    /// Intermediate query chain element used for Collect modifiers.
    /// </summary>
    /// <typeparam name="TSource">Type of the element the query operates on.</typeparam>
    public interface ICollectQuery<out TSource> : IQuery<TSource>
    {
    }

    /// <summary>
    /// Intermediate query chain element used for OrderBy modifiers.
    /// </summary>
    /// <typeparam name="TSource">Type of the element the query operates on.</typeparam>
    public interface IOrderedQuery<out TSource> : IQuery<TSource>
    {
    }
}
