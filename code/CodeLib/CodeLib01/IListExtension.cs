using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#pragma warning disable CS0219,CS8600,CS8602,CS8603,CS8604,CS8618,CS8619,CS8625,CS8714
namespace CodeLib01;

public static class IListExtension
{
    #region 2022-11-18 Ciaran Add Tuple

    public static Tuple<T1> Add<T1>(this IList<Tuple<T1>> list, T1 t1)
    {
        var tuple = new Tuple<T1>(t1);
        list.Add(tuple);
        return tuple;
    }

    public static Tuple<T1, T2> Add<T1, T2>(this IList<Tuple<T1, T2>> list, T1 t1, T2 t2)
    {
        var tuple = new Tuple<T1, T2>(t1, t2);
        list.Add(tuple);
        return tuple;
    }

    public static Tuple<T1, T2, T3> Add<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list, T1 t1, T2 t2, T3 t3)
    {
        var tuple = new Tuple<T1, T2, T3>(t1, t2, t3);
        list.Add(tuple);
        return tuple;
    }

    public static Tuple<T1, T2, T3, T4> Add<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list, T1 t1, T2 t2, T3 t3, T4 t4)
    {
        var tuple = new Tuple<T1, T2, T3, T4>(t1, t2, t3, t4);
        list.Add(tuple);
        return tuple;
    }

    public static Tuple<T1, T2, T3, T4, T5> Add<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
    {
        var tuple = new Tuple<T1, T2, T3, T4, T5>(t1, t2, t3, t4, t5);
        list.Add(tuple);
        return tuple;
    }

    public static Tuple<T1, T2, T3, T4, T5, T6> Add<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
    {
        var tuple = new Tuple<T1, T2, T3, T4, T5, T6>(t1, t2, t3, t4, t5, t6);
        list.Add(tuple);
        return tuple;
    }

    public static Tuple<T1, T2, T3, T4, T5, T6, T7> Add<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
    {
        var tuple = new Tuple<T1, T2, T3, T4, T5, T6, T7>(t1, t2, t3, t4, t5, t6, t7);
        list.Add(tuple);
        return tuple;
    }

    public static Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> Add<T1, T2, T3, T4, T5, T6, T7, TRest>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, TRest t8) where TRest : notnull
    {
        var tuple = new Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>(t1, t2, t3, t4, t5, t6, t7, t8);
        list.Add(tuple);
        return tuple;
    }

    #endregion

    /// <summary>
    /// 扩展源方法<br/> 2022-10-31 Ciaran
    /// </summary>
    public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array) => Array.AsReadOnly(array);


    /// <summary>
    /// 2022-10-31 Ciaran 添加列表到链表
    /// </summary>
    public static void AddLastRange<T>(this LinkedList<T> list, IEnumerable<T> items)
    {
        foreach (var data in items) list.AddLast(data);
    }

    /// <summary>
    /// 2022-10-31 Ciaran 添加列表到链表
    /// </summary>
    public static List<LinkedListNode<T>> AddLastRangeNodes<T>(this LinkedList<T> list, IEnumerable<T> items)
    {
        var enumerable = items as T[] ?? items.ToArray();
        List<LinkedListNode<T>> nodes = new List<LinkedListNode<T>>(enumerable.Length);
        nodes.AddRange(enumerable.Select(list.AddLast));

        return nodes;
    }

    /// <summary>
    /// 2022-10-31 Ciaran 链表中删除列表
    /// </summary>
    public static void RemoveRange<T>(this LinkedList<T> list, IEnumerable<LinkedListNode<T>> datas)
    {
        foreach (var data in datas)
        {
            list.Remove(data);
        }
    }

    /// <summary>
    /// 2022-10-31 Ciaran 链表中提取并删除元素
    /// </summary>
    public static List<T> PickValues<T>(this LinkedList<T> list, Func<LinkedListNode<T>, bool> predicate)
    {
        var datas = list.AsNodeEnumerable().Where(predicate).ToArray();
        list.RemoveRange(datas);
        return datas.Select(d => d.Value).ToList();
    }

    /// <summary>
    /// 2022-11-10 Ciaran 链表中提取并删除元素
    /// </summary>
    public static T? PickValue<T>(this LinkedList<T> list, Func<LinkedListNode<T>, bool> predicate)
    {
        var data = list.AsNodeEnumerable().FirstOrDefault(predicate);
        if (data == null) return default;
        list.Remove(data);
        return data.Value;
    }


    /// <summary>
    /// 格式化数组, 使符合长度(左填充)<br/>2022-6-17 Ciaran
    /// </summary>
    /// <param name="source"></param>
    /// <param name="len">输出长度</param>
    /// <param name="value">填充用的值</param>
    /// <exception cref="ArgumentOutOfRangeException">"len 参数不能小于0."</exception>
    public static T[] FormatLength<T>(this T[] source, int len, T value)
    {
        T[] new_cs;
        if (len < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(len), "len 参数不能小于0.");
        }

        if (source.Length >= len)
        {
            new_cs = new T[len];
            Array.Copy(sourceArray: source, sourceIndex: source.Length - len, destinationArray: new_cs,
                destinationIndex: 0, length: len);
        }
        else
        {
            new_cs = new T[len];
            Array.Copy(sourceArray: source, sourceIndex: 0, destinationArray: new_cs,
                destinationIndex: len - source.Length, length: source.Length);
            for (int i = len - source.Length - 1; i >= 0; i--)
            {
                new_cs[i] = value;
            }
        }

        return new_cs;
    }

    /// <summary>
    /// 格式化集合, 使符合长度(左填充)<br/>2022-6-17 Ciaran
    /// </summary>
    /// <param name="source"></param>
    /// <param name="len">输出长度</param>
    /// <param name="value">填充用的值</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static List<T> FormatLength<T>(this List<T> source, int len, T value)
    {
        if (len < 0)
            throw new ArgumentOutOfRangeException(nameof(len));

        return source.Count >= len ? source.Skip(source.Count - len).ToList() : value.Repeat(len - source.Count).Concat(source).ToList();
    }

    /// <summary>
    /// 优化<see cref="IList&lt;T&gt;"/> 的Add方法
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void Add<T1, T2>(this IList<KeyValuePair<T1, T2>> dict, T1 key, T2 value)
    {
        dict.Add(new KeyValuePair<T1, T2>(key, value));
    }

    public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            set.Add(item);
        }
    }
}