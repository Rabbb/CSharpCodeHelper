using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS0219,CS8600,CS8602,CS8603,CS8604,CS8618,CS8619,CS8625,CS8714

namespace CodeLib01;

public static class DictionaryHelper
{
    /// <summary>
    /// 2021-2-1 Ciaran 如果存在键，则返回关联的值，否则返回默认值
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
    {
        source.TryGetValue(key, out var value);
        return value;
    }


    /// <summary>
    /// 2022-6-14 Ciaran如果存在键，则返回关联的值，否则返回指定值
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <param name="default_value"></param>
    /// <returns></returns>
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue default_value) => !source.TryGetValue(key, out var value) ? default_value : value;


    /// <summary>
    /// 2022-11-14 Ciaran 字典获取值, 否则, 添加值并返回
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict.TryGetValue(key, out var value2)) return value2;
        dict.Add(key, value);
        return value;
    }

    /// <summary>
    /// 2022-11-15 Ciaran 字典获取值, 否则, 添加实时值并返回
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="getValue"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        Func<TKey, TValue> getValue)
    {
        if (dict.TryGetValue(key, out var value)) return value;
        value = getValue(key);
        dict.Add(key, value);
        return value;
    }

    /// <summary>
    /// 2022-11-15 Ciaran 如果存在键，则设置值，否则新增属性值，返回设置的值
    /// </summary>
    public static TValue Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
    {
        if (source.ContainsKey(key))
            source[key] = value;
        else
            source.Add(key, value);

        return value;
    }

    /// <summary>
    /// 2023-7-29 Ciaran 如果存在键，则设置值，否则新增属性值。返回被替换的值（没有则返回默认值）
    /// </summary>
    public static TValue Replace<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
    {
        if (source.ContainsKey(key))
        {
            var old_value = source[key];
            source[key] = value;
            return old_value;
        }

        source.Add(key, value);
        return default(TValue);
    }

    /// <summary>
    /// 获取字典对应值最小的第一个键值对.<br/>
    /// 2022-5-30 (Ciaran)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TSelect"></typeparam>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static KeyValuePair<TKey, TValue>? MinBy<TKey, TValue, TSelect>(this IDictionary<TKey, TValue> source,
        Func<KeyValuePair<TKey, TValue>, TSelect?> selector) where TSelect : IComparable<TSelect>
    {
        KeyValuePair<TKey, TValue> min;
        TSelect? min_by, cur_by;
        KeyValuePair<TKey, TValue>[] list = source.ToArray();
        if (list.Length == 0)
        {
            return null;
        }

        min = list[0];
        min_by = selector(list[0]);
        for (int i = 1; i < list.Length; i++)
        {
            cur_by = selector(list[i]);
            if (min_by == null)
            {
                if (cur_by == null)
                    continue;
                min_by = cur_by;
                min = list[i];
            }
            else
            {
                if (cur_by is null || min_by.CompareTo(cur_by) <= 0) continue;
                min_by = cur_by;
                min = list[i];
            }
        }

        return min;
    }


    /// <summary>
    /// 获取字典对应值最大的第一个键值对.<br/>
    /// 2022-5-30 (Ciaran)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TSelect"></typeparam>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static KeyValuePair<TKey, TValue>? MaxBy<TKey, TValue, TSelect>(this IDictionary<TKey, TValue> source,
        Func<KeyValuePair<TKey, TValue>, TSelect?> selector) where TSelect : IComparable<TSelect>
    {
        KeyValuePair<TKey, TValue> max;
        TSelect? max_by, cur_by;
        KeyValuePair<TKey, TValue>[] list = source.ToArray();
        if (list.Length == 0)
        {
            return null;
        }

        max = list[0];
        max_by = selector(list[0]);
        for (int i = 1; i < list.Length; i++)
        {
            cur_by = selector(list[i]);
            if (max_by is null)
            {
                if (cur_by is null)
                    continue;
                max_by = cur_by;
                max = list[i];
            }
            else
            {
                if (cur_by is null || max_by.CompareTo(cur_by) >= 0) continue;
                max_by = cur_by;
                max = list[i];
            }
        }

        return max;
    }
        
        
    /// <summary>
    /// 2023-8-10 Ciaran ToDictionary方法扩展
    /// </summary>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, int, TKey> keySelector, Func<TSource, int, TValue> elementSelector)
    {
        var dict = new Dictionary<TKey, TValue>();
        int index = -1;
        foreach (TSource source1 in source)
        {
            checked
            {
                index++;
            }

            var key = keySelector(source1, index);
            var value = elementSelector(source1, index);
            dict.Add(key, value);
        }

        return dict;
    }
        
    /// <summary>
    /// 2023-8-10 Ciaran ToDictionary方法扩展
    /// </summary>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TSource>, TKey> keySelector, Func<TSource, int, IEnumerable<TSource>, TValue> elementSelector)
    {
        var dict = new Dictionary<TKey, TValue>();
        int index = -1;
        var enumerable = source.ToList();
        foreach (TSource source1 in enumerable)
        {
            checked
            {
                index++;
            }

            var key = keySelector(source1, index, enumerable);
            var value = elementSelector(source1, index, enumerable);
            dict.Add(key, value);
        }

        return dict;
    }
}