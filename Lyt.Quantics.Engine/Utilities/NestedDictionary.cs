namespace Lyt.Quantics.Engine.Utilities;

/// <summary> Represents a collection of keys and values. </summary>
/// <typeparam name="TKey1">The type of the key</typeparam>
/// <typeparam name="TValue">The type of the value</typeparam>
public class NestedDictionary<TKey1, TValue> : Dictionary<TKey1, TValue>
    where TKey1 : notnull
{
    /// <summary>
    /// Initializes a new instance of this class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
    /// </summary>
    public NestedDictionary() : base() { }

    /// <summary>
    /// Initializes a new instance of this class that is empty, has the specified initial capacity, and uses the default equality comparer for the key type.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the dictionary can contain.</param>
    /// <exception cref="ArgumentOutOfRangeException">capacity is less than 0.</exception>
    public NestedDictionary(int capacity) : base(capacity) { }

    /// <summary>
    /// Initializes a new instance of this class that is empty, has the default initial capacity, and uses the specified equality comparer.
    /// </summary>
    /// <param name="comparer">The comparer implementation to use when comparing keys, or null to use the default comparer for the type of the key.</param>
    public NestedDictionary(IEqualityComparer<TKey1>? comparer) : base(comparer) { }

    /// <summary>
    /// Initializes a new instance of this class that contains elements copied from the specified dictionary and uses the default equality comparer for the key type.
    /// </summary>
    /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
    /// <exception cref="ArgumentNullException">dictionary is null</exception>
    /// <exception cref="ArgumentException">dictionary contains one or more duplicate keys</exception>
    public NestedDictionary(IDictionary<TKey1, TValue> dictionary) : base(dictionary) { }

    /// <summary>
    /// Initializes a new instance of this class that is empty, has the specified initial capacity, and uses the specified comparer.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the dictionary can contain.</param>
    /// <param name="comparer">The comparer implementation to use when comparing keys, or null to use the default comparer for the type of the key.</param>
    /// <exception cref="ArgumentOutOfRangeException">capacity is less than 0.</exception>
    public NestedDictionary(int capacity, IEqualityComparer<TKey1>? comparer) : base(capacity, comparer) { }

    /// <summary>
    /// Initializes a new instance of this class that contains elements copied from the specified dictionary and uses the specified comparer.
    /// </summary>
    /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
    /// <param name="comparer">The comparer implementation to use when comparing keys, or null to use the default comparer for the type of the key</param>
    /// <exception cref="ArgumentNullException">dictionary is null</exception>
    /// <exception cref="ArgumentException">dictionary contains one or more duplicate keys</exception>
    public NestedDictionary(IDictionary<TKey1, TValue> dictionary, IEqualityComparer<TKey1>? comparer) 
        : base(dictionary, comparer) { }
}

/// <summary> Represents a collection of keys and values where a key leads through nested dictionary to a value. </summary>
/// <typeparam name="TKey1">The type of the 1. key</typeparam>
/// <typeparam name="TKey2">The type of the 2. key</typeparam>
/// <typeparam name="TValue">The type of the value</typeparam>
public class NestedDictionary<TKey1, TKey2, TValue> : NestedDictionary<TKey1, NestedDictionary<TKey2, TValue>>
    where TKey1 : notnull
    where TKey2 : notnull
{
    private readonly int _capacity2;
    private readonly IEqualityComparer<TKey2>? _comparer2;

    /// <summary>
    /// Initializes a new instance of this class that is empty, has the default initial capacity, and uses the default equality comparer for the key types.
    /// </summary>
    public NestedDictionary() : base() { }

    /// <summary>
    /// Initializes a new instance of this class that is empty, all level dictionaries have the specified initial capacity, and uses the default equality comparer for the key types.
    /// </summary>
    /// <param name="capacity1">The initial number of elements that the 1. level dictionary can contain.</param>
    /// <param name="capacity2">The initial number of elements that the 2. level dictionary can contain.</param>
    /// <exception cref="ArgumentOutOfRangeException">a capacity is less than 0.</exception>
    public NestedDictionary(int capacity1, int capacity2) : base(capacity1) 
        => _capacity2 = capacity2;

    /// <summary>
    /// Initializes a new instance of this class that is empty, has the default initial capacity, and all level dictionaries use the specified equality comparers.
    /// </summary>
    /// <param name="comparer1">The comparer implementation to use when comparing 1. level keys, or null to use the default comparer for the type of the key.</param>
    /// <param name="comparer2">The comparer implementation to use when comparing 2. level keys, or null to use the default comparer for the type of the key.</param>
    public NestedDictionary(IEqualityComparer<TKey1>? comparer1, IEqualityComparer<TKey2>? comparer2) 
        : base(comparer1) 
        => _comparer2 = comparer2;

    /// <summary>
    /// Initializes a new instance of this class that contains elements copied from the specified dictionary and uses the default equality comparer for the key types.
    /// </summary>
    /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
    /// <exception cref="ArgumentNullException">dictionary is null</exception>
    /// <exception cref="ArgumentException">dictionary contains one or more duplicate keys</exception>
    public NestedDictionary(IDictionary<TKey1, NestedDictionary<TKey2, TValue>> dictionary) : base(dictionary) { }

    /// <summary>
    /// Initializes a new instance of this class that is empty, all level dictionaries have the specified initial capacity and use the specified comparer.
    /// </summary>
    /// <param name="capacity1">The initial number of elements that the 1. level dictionary can contain.</param>
    /// <param name="capacity2">The initial number of elements that the 2. level dictionary can contain.</param>
    /// <param name="comparer1">The comparer implementation to use when comparing 1. level keys, or null to use the default comparer for the type of the key.</param>
    /// <param name="comparer2">The comparer implementation to use when comparing 2. level keys, or null to use the default comparer for the type of the key.</param>
    /// <exception cref="ArgumentOutOfRangeException">capacity is less than 0.</exception>
    public NestedDictionary(
        int capacity1, int capacity2, IEqualityComparer<TKey1>? comparer1, IEqualityComparer<TKey2>? comparer2) 
        : base(capacity1, comparer1)
    {
        _capacity2 = capacity2;
        _comparer2 = comparer2;
    }

    /// <summary>
    /// Initializes a new instance of this class that contains elements copied from the specified dictionary and uses the specified comparer.
    /// </summary>
    /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
    /// <param name="comparer1">The comparer implementation to use when comparing 1. level keys, or null to use the default comparer for the type of the key.</param>
    /// <param name="comparer2">The comparer implementation to use when comparing 2. level keys, or null to use the default comparer for the type of the key.</param>
    /// <exception cref="ArgumentNullException">dictionary is null</exception>
    /// <exception cref="ArgumentException">dictionary contains one or more duplicate keys</exception>
    public NestedDictionary(IDictionary<TKey1, NestedDictionary<TKey2, TValue>> dictionary, IEqualityComparer<TKey1>? comparer1, IEqualityComparer<TKey2>? comparer2) : base(dictionary, comparer1) 
        => _comparer2 = comparer2;

    /// <summary> Gets or sets the value associated with the specified key. </summary>
    /// <param name="key1">The key of the value to get or set</param>
    /// <returns>The value associated with the specified key. If the specified key is not found and the key leads to a value, a get operation 
    /// throws a System.Collections.Generic.KeyNotFoundException. If the specified key is not found and the key leads to another nested dictionary (goes deeper),
    /// the nested dictionary is created. Set operation creates a new element with the specified key.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the lowest level of the collection</exception>
    public new NestedDictionary<TKey2, TValue> this[TKey1 key1]
    {
        get => TryGetValue(key1, out NestedDictionary<TKey2, TValue>? dict) ? dict : base[key1] = new NestedDictionary<TKey2, TValue>(_capacity2, _comparer2);
        set => base[key1] = value;
    }

    /// <summary> Gets the comparer that is used to determine equality of keys for the 2. level of the dictionary. </summary>
    public IEqualityComparer<TKey2>? Comparer2 => _comparer2;

    /// <summary> Adds the specified keys and value to the dictionaries. </summary>
    /// <param name="key1">The key of the element to add for 1. level.</param>
    /// <param name="key2">The key of the element to add for 2. level.</param>
    /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
    /// <exception cref="ArgumentNullException">A key is null</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the dictionary</exception>
    public void Add(TKey1 key1, TKey2 key2, TValue value) => this[key1].Add(key2, value);

    /// <summary> Determines whether the nested dictionary contains specified key chain. </summary>
    /// <param name="key1">The key to locate for 1. level.</param>
    /// <param name="key2">The key to locate for 2. level.</param>
    /// <returns>True if the nested dictionary contains an element with the specified key chain; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    public bool ContainsKey(TKey1 key1, TKey2 key2) 
        => TryGetValue(key1, out NestedDictionary<TKey2, TValue>? dict) && dict.ContainsKey(key2);

    /// <summary> Determines whether the nested dictionary contains a specific value. </summary>
    /// <param name="value">The value to locate in the nested dictionary. The value can be null for reference types.</param>
    /// <returns>True if the nested dictionary contains an element with the specified value; otherwise, false.</returns>
    public bool ContainsValue(TValue value)
    {
        foreach (NestedDictionary<TKey2, TValue> dict in Values)
        {
            if (dict.ContainsValue(value))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Removes the value or nested dictionary on a deeper level with the specified key chain from the nested dictionary.
    /// </summary>
    /// <param name="key1">The key for 1. level.</param>
    /// <param name="key2">The key for 2. level.</param>
    /// <returns>True if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the nested dictionary.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    public bool Remove(TKey1 key1, TKey2 key2)
    {
        if (TryGetValue(key1, out NestedDictionary<TKey2, TValue>? dict))
        {
            return dict.Remove(key2);
        }

        return false;
    }

    /// <summary> Gets the value associated with the specified key chain. </summary>
    /// <param name="key1">The key of the value to get 1. level.</param>
    /// <param name="key2">The key of the value to get 2. level.</param>
    /// <param name="value">When this method returns true, contains the value associated with the specified key chain, if the key chain is found; otherwise,
    /// the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>True if the nested dictionary contains an element with the specified key chain; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue? value)
    {
        value = default;
        return TryGetValue(key1, out NestedDictionary<TKey2, TValue>? dict) && dict.TryGetValue(key2, out value);
    }
}

/// <summary> Represents a collection of keys and values where a key leads through nested dictionary to a value. </summary>
/// <typeparam name="TKey1">The type of the 1. key</typeparam>
/// <typeparam name="TKey2">The type of the 2. key</typeparam>
/// <typeparam name="TKey3">The type of the 3. key</typeparam>
/// <typeparam name="TValue">The type of the value</typeparam>
public class NestedDictionary<TKey1, TKey2, TKey3, TValue> : NestedDictionary<TKey1, NestedDictionary<TKey2, TKey3, TValue>>
    where TKey1 : notnull
    where TKey2 : notnull
    where TKey3 : notnull
{
    private readonly int _capacity2;
    private readonly int _capacity3;

    private readonly IEqualityComparer<TKey2>? _comparer2;
    private readonly IEqualityComparer<TKey3>? _comparer3;

    /// <summary>
    /// Initializes a new instance of this class that is empty, has the default initial capacity, and uses the default equality comparer for the key types.
    /// </summary>
    public NestedDictionary() : base() { }

    /// <summary>
    /// Initializes a new instance of this class that is empty, all level dictionaries have the specified initial capacity, and uses the default equality comparer for the key types.
    /// </summary>
    /// <param name="capacity1">The initial number of elements that the 1. level dictionary can contain.</param>
    /// <param name="capacity2">The initial number of elements that the 2. level dictionary can contain.</param>
    /// <param name="capacity3">The initial number of elements that the 3. level dictionary can contain.</param>
    /// <exception cref="ArgumentOutOfRangeException">a capacity is less than 0.</exception>
    public NestedDictionary(int capacity1, int capacity2, int capacity3) : base(capacity1)
    {
        _capacity2 = capacity2;
        _capacity3 = capacity3;
    }

    /// <summary>
    /// Initializes a new instance of this class that is empty, has the default initial capacity, and all level dictionaries use the specified equality comparers.
    /// </summary>
    /// <param name="comparer1">The comparer implementation to use when comparing 1. level keys, or null to use the default comparer for the type of the key.</param>
    /// <param name="comparer2">The comparer implementation to use when comparing 2. level keys, or null to use the default comparer for the type of the key.</param>
    /// <param name="comparer3">The comparer implementation to use when comparing 3. level keys, or null to use the default comparer for the type of the key.</param>
    public NestedDictionary(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2, IEqualityComparer<TKey3> comparer3) : base(comparer1)
    {
        _comparer2 = comparer2;
        _comparer3 = comparer3;
    }

    /// <summary>
    /// Initializes a new instance of this class that contains elements copied from the specified dictionary and uses the default equality comparer for the key types.
    /// </summary>
    /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
    /// <exception cref="ArgumentNullException">dictionary is null</exception>
    /// <exception cref="ArgumentException">dictionary contains one or more duplicate keys</exception>
    public NestedDictionary(IDictionary<TKey1, NestedDictionary<TKey2, TKey3, TValue>> dictionary) : base(dictionary)
    {
    }

    /// <summary>
    /// Initializes a new instance of this class that is empty, all level dictionaries have the specified initial capacity and use the specified comparer.
    /// </summary>
    /// <param name="capacity1">The initial number of elements that the 1. level dictionary can contain.</param>
    /// <param name="capacity2">The initial number of elements that the 2. level dictionary can contain.</param>
    /// <param name="capacity3">The initial number of elements that the 3. level dictionary can contain.</param>
    /// <param name="comparer1">The comparer implementation to use when comparing 1. level keys, or null to use the default comparer for the type of the key.</param>
    /// <param name="comparer2">The comparer implementation to use when comparing 2. level keys, or null to use the default comparer for the type of the key.</param>
    /// <param name="comparer3">The comparer implementation to use when comparing 3. level keys, or null to use the default comparer for the type of the key.</param>
    /// <exception cref="ArgumentOutOfRangeException">capacity is less than 0.</exception>
    public NestedDictionary(int capacity1, int capacity2, int capacity3, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2, IEqualityComparer<TKey3> comparer3) : base(capacity1, comparer1)
    {
        _capacity2 = capacity2;
        _capacity3 = capacity3;
        _comparer2 = comparer2;
        _comparer3 = comparer3;
    }

    /// <summary>
    /// Initializes a new instance of this class that contains elements copied from the specified dictionary and uses the specified comparer.
    /// </summary>
    /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
    /// <param name="comparer1">The comparer implementation to use when comparing 1. level keys, or null to use the default comparer for the type of the key.</param>
    /// <param name="comparer2">The comparer implementation to use when comparing 2. level keys, or null to use the default comparer for the type of the key.</param>
    /// <param name="comparer3">The comparer implementation to use when comparing 3. level keys, or null to use the default comparer for the type of the key.</param>
    /// <exception cref="ArgumentNullException">dictionary is null</exception>
    /// <exception cref="ArgumentException">dictionary contains one or more duplicate keys</exception>
    public NestedDictionary(IDictionary<TKey1, NestedDictionary<TKey2, TKey3, TValue>> dictionary, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2, IEqualityComparer<TKey3> comparer3) : base(dictionary, comparer1)
    {
        _comparer2 = comparer2;
        _comparer3 = comparer3;
    }


    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key1">The key of the value to get or set</param>
    /// <returns>The value associated with the specified key. If the specified key is not found and the key leads to a value, a get operation 
    /// throws a System.Collections.Generic.KeyNotFoundException. If the specified key is not found and the key leads to another nested dictionary (goes deeper),
    /// the nested dictionary is created. Set operation creates a new element with the specified key.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the lowest level of the collection</exception>
    public new NestedDictionary<TKey2, TKey3, TValue> this[TKey1 key1]
    {
        get => TryGetValue(key1, out NestedDictionary<TKey2, TKey3, TValue>? dict) ? dict : base[key1] = new NestedDictionary<TKey2, TKey3, TValue>(_capacity2, _capacity3, _comparer2, _comparer3);
        set => base[key1] = value;
    }

    /// <summary> Gets the comparer that is used to determine equality of keys for the 2. level of the dictionary. </summary>
    public IEqualityComparer<TKey2>? Comparer2 => _comparer2;

    /// <summary> Gets the comparer that is used to determine equality of keys for the 3. level of the dictionary. </summary>
    public IEqualityComparer<TKey3>? Comparer3 => _comparer3;

    /// <summary> Adds the specified keys and value to the dictionaries. </summary>
    /// <param name="key1">The key of the element to add for 1. level.</param>
    /// <param name="key2">The key of the element to add for 2. level.</param>
    /// <param name="key3">The key of the element to add for 3. level.</param>
    /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
    /// <exception cref="ArgumentNullException">A key is null</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the dictionary</exception>
    public void Add(TKey1 key1, TKey2 key2, TKey3 key3, TValue value) => this[key1].Add(key2, key3, value);

    /// <summary> Adds the specified keys and value to the dictionaries. </summary>
    /// <param name="key1">The key of the element to add for 1. level.</param>
    /// <param name="key2">The key of the element to add for 2. level.</param>
    /// <param name="dict">The dictionary for 3. level.</param>
    /// <exception cref="ArgumentNullException">A key is null</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the dictionary</exception>
    public void Add(TKey1 key1, TKey2 key2, NestedDictionary<TKey3, TValue> dict) => this[key1].Add(key2, dict);

    /// <summary> Determines whether the nested dictionary contains specified key chain. </summary>
    /// <param name="key1">The key to locate for 1. level.</param>
    /// <param name="key2">The key to locate for 2. level.</param>
    /// <param name="key3">The key to locate for 3. level.</param>
    /// <returns>True if the nested dictionary contains an element with the specified key chain; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    public bool ContainsKey(TKey1 key1, TKey2 key2, TKey3 key3) 
        => TryGetValue(key1, out NestedDictionary<TKey2, TKey3, TValue>? dict) && dict.ContainsKey(key2, key3);

    /// <summary> Determines whether the nested dictionary contains specified key chain. </summary>
    /// <param name="key1">The key to locate for 1. level.</param>
    /// <param name="key2">The key to locate for 2. level.</param>
    /// <returns>True if the nested dictionary contains an element with the specified key chain; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    public bool ContainsKey(TKey1 key1, TKey2 key2) => TryGetValue(key1, out NestedDictionary<TKey2, TKey3, TValue>? dict) && dict.ContainsKey(key2);

    /// <summary> Determines whether the nested dictionary contains a specific value. </summary>
    /// <param name="value">The value to locate in the nested dictionary. The value can be null for reference types.</param>
    /// <returns>True if the nested dictionary contains an element with the specified value; otherwise, false.</returns>
    public bool ContainsValue(TValue value)
    {
        foreach (NestedDictionary<TKey2, TKey3, TValue> dict in Values)
        {
            if (dict.ContainsValue(value))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Removes the value or nested dictionary on a deeper level with the specified key chain from the nested dictionary.
    /// </summary>
    /// <param name="key1">The key for 1. level.</param>
    /// <param name="key2">The key for 2. level.</param>
    /// <param name="key3">The key for 3. level.</param>
    /// <returns>True if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the nested dictionary.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    public bool Remove(TKey1 key1, TKey2 key2, TKey3 key3)
    {
        if (TryGetValue(key1, out NestedDictionary<TKey2, TKey3, TValue>? dict))
        {
            return dict.Remove(key2, key3);
        } 

        return false;
    }

    /// <summary>
    ///  Removes the value or nested dictionary on a deeper level with the specified key chain from the nested dictionary.
    /// </summary>
    /// <param name="key1">The key for 1. level.</param>
    /// <param name="key2">The key for 2. level.</param>
    /// <returns>True if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the nested dictionary.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    public bool Remove(TKey1 key1, TKey2 key2)
    {
        if (TryGetValue(key1, out NestedDictionary<TKey2, TKey3, TValue>? dict))
        {
            return dict.Remove(key2);
        }

        return false;
    }

    /// <summary> Gets the value associated with the specified key chain. </summary>
    /// <param name="key1">The key of the value to get 1. level.</param>
    /// <param name="key2">The key of the value to get 2. level.</param>
    /// <param name="key3">The key of the value to get 3. level.</param>
    /// <param name="value">When this method returns true, contains the value associated with the specified key chain, if the key chain is found; otherwise,
    /// the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>True if the nested dictionary contains an element with the specified key chain; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    public bool TryGetValue(TKey1 key1, TKey2 key2, TKey3 key3, out TValue? value)
    {
        value = default;
        return TryGetValue(key1, out NestedDictionary<TKey2, TKey3, TValue>? dict) && dict.TryGetValue(key2, key3, out value);
    }

    /// <summary>
    /// Gets the nested dictionary of deeper level associated with the specified key chain.
    /// </summary>
    /// <param name="key1">The key of the value to get 1. level.</param>
    /// <param name="key2">The key of the value to get 2. level.</param>
    /// <param name="value">When this method returns true, contains the value associated with the specified key chain, if the key chain is found; otherwise,
    /// the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>True if the nested dictionary contains an element with the specified key chain; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key is null</exception>
    public bool TryGetValue(TKey1 key1, TKey2 key2, out NestedDictionary<TKey3, TValue>? value)
    {
        value = default;
        return TryGetValue(key1, out NestedDictionary<TKey2, TKey3, TValue>? dict) && dict.TryGetValue(key2, out value);
    }
}
