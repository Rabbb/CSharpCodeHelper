using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeLib01;

/// <summary>
/// 2023-3-1 Ciaran 共键字典
/// </summary>
public class MultipleDictionary<TKey, TValue> : IDictionary<TKey, ICollection<TValue>> where TKey : notnull
{
    private readonly Dictionary<TKey, ICollection<TValue>> storage;
    private readonly Func<ICollection<TValue>> GetCollection;

    public MultipleDictionary() : this(false)
    {
    }

    public MultipleDictionary(bool keyHasSameValue)
    {
        storage = new Dictionary<TKey, ICollection<TValue>>();
        if (keyHasSameValue)
            this.GetCollection = () => new List<TValue>();
        else
            this.GetCollection = () => new HashSet<TValue>();
    }

    public MultipleDictionary(Func<ICollection<TValue>> getCollection)
    {
        storage = new Dictionary<TKey, ICollection<TValue>>();
        this.GetCollection = getCollection;
    }

    public void Add(TKey key, TValue value)
    {
        if (!storage.ContainsKey(key)) storage.Add(key, GetCollection());
        storage[key].Add(value);
    }

    public bool Remove(TKey key, TValue value)
    {
        return storage.ContainsKey(key) && storage[key].Remove(value);
    }

    public void Add(TKey key, ICollection<TValue> value)
    {
        ICollection<TValue> values;
        if (!storage.ContainsKey(key)) storage.Add(key, values = GetCollection());
        else values = storage[key];
        if (value is null || value.Count == 0) return;
        foreach (var value1 in value)
        {
            values.Add(value1);
        }
    }

    public bool Remove(TKey key)
    {
        if (!storage.ContainsKey(key)) return false;
        var has_item = storage.Count > 0;
        if (has_item) storage[key].Clear();
        return has_item;
    }

    public bool TryGetValue(TKey key, out ICollection<TValue> value)
    {
#pragma warning disable CS8601
        if (storage.TryGetValue(key, out value)) return true;
        value = GetCollection();
        return false;
#pragma warning restore CS8601
    }

    public ICollection<TKey> Keys => storage.Keys;
    public ICollection<ICollection<TValue>> Values => storage.Values;

    public bool ContainsKey(TKey key)
    {
        return storage.ContainsKey(key);
    }

    public ICollection<TValue> this[TKey key]
    {
        get => !storage.ContainsKey(key) ? GetCollection() : storage[key];
        set
        {
            this.Remove(key);
            this.Add(key, value);
        }
    }

    public IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> GetEnumerator()
    {
        return storage.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, ICollection<TValue>> item)
    {
        var key = item.Key;
        this.Add(key, item.Value);
    }

    public void Clear()
    {
        storage.Clear();
    }

    public bool Contains(KeyValuePair<TKey, ICollection<TValue>> item)
    {
        var key = item.Key;
        var value = item.Value;
        return storage.ContainsKey(key)
               && value != null
               && value.Count > 0
               && storage[key].All(value1 => value.Contains(value1));
    }

    public void CopyTo(KeyValuePair<TKey, ICollection<TValue>>[] array, int arrayIndex)
    {
        ((IDictionary<TKey, ICollection<TValue>>)storage).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, ICollection<TValue>> item)
    {
        var key = item.Key;
        var value = item.Value;
        if (!storage.ContainsKey(key) || value is null || value.Count == 0) return false;
        var values = storage[key];
        return value.Select(value1 => values.Remove(value1)).Aggregate((b1, b2) => b1 || b2);
    }

    public int Count => storage.Count;
    public bool IsReadOnly => false;
}

public static class MultipleDictionaryExtension
{
    /// <summary>
    /// 2023-3-1 Ciaran 共键字典, 不允许相同键相同值
    /// </summary>
    /// <param name="source">源列表</param>
    /// <param name="keySelector">键选择器</param>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <returns></returns>
    public static MultipleDictionary<TKey, TValue> ToMultipleDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector) where TKey : notnull => ToMultipleDictionary(source, keySelector, false);

    /// <summary>
    /// 2023-3-1 Ciaran 共键字典, 可选是否允许相同键相同值
    /// </summary>
    /// <param name="source">源列表</param>
    /// <param name="keySelector">键选择器</param>
    /// <param name="keyHasSameValue">允许相同键相同值</param>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <returns></returns>
    public static MultipleDictionary<TKey, TValue> ToMultipleDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector, bool keyHasSameValue) where TKey : notnull
    {
        var dict = new MultipleDictionary<TKey, TValue>(keyHasSameValue);
        foreach (var value in source)
        {
            dict.Add(keySelector(value), value);
        }

        return dict;
    }
}