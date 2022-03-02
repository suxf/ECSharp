using System.Collections.Concurrent;

namespace ES.Alias
{
    /// <summary>
    /// Represents a thread-safe collection of key/value pairs that can be accessed by multiple threads concurrently.
    /// <para>Same as ConcurrentDictionary, Name with C++ Style.</para>
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class ConcurrentMap<TKey, TValue> : ConcurrentDictionary<TKey, TValue> where TKey : notnull { }
}
