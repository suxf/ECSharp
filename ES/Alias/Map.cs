using System.Collections.Generic;

namespace ES.Alias
{
    /// <summary>
    /// Represents a collection of keys and values.
    /// <para>Same as Dictionary, Name with C++ Style.</para>
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class Map<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull { }
}
