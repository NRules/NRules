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
}
