namespace NRules.Utilities;
#if NETSTANDARD2_0
internal static class QueueExtensions
{
    public static bool TryDequeue<T>(this Queue<T> queue, out T? item)
    {
        if (queue.Count == 0)
        {
            item = default;
            return false;
        }

        item = queue.Dequeue();
        return true;
    }

    public static bool TryPeek<T>(this Queue<T> queue, out T? item)
    {
        if (queue.Count == 0)
        {
            item = default;
            return false;
        }

        item = queue.Peek();
        return true;
    }
}
#endif
