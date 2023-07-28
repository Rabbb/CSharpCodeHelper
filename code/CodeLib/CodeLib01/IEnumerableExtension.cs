using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS0219,CS8600,CS8602,CS8603,CS8604,CS8618,CS8619,CS8625,CS8714
namespace CodeLib01;

public static class IEnumerableExtension
{
    /// <summary>
    /// 以指定数量为长度周期的列表枚举, 遍历一个列表<br/>
    /// 2023-1-3 Ciaran
    /// </summary>
    public static IEnumerable<List<TSource>> Period<TSource>(this IEnumerable<TSource> source, int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        // 2023-1-3 Ciaran 读取以 指定数量为一个周期, 处理对应数量内的列表
        using var enumerator = source.GetEnumerator();
        while (true)
        {
            List<TSource> list = new List<TSource>(count);
            for (int i = 0; i < count; i++)
            {
                if (enumerator.MoveNext())
                {
                    list.Add(enumerator.Current);
                }
                else
                {
                    break;
                }
            }

            if (list.Count == 0)
            {
                yield break;
            }

            yield return list;
        }
    }

    /// <summary>
    /// 以指定数量为长度周期, 遍历处理一个列表<br/>
    /// 2023-1-3 Ciaran
    /// </summary>
    public static void ForPeriod<TSource>(this IEnumerable<TSource> source, int count, Action<List<TSource>> action)
    {
        source.Period(count).ForEach(action);
    }


    /// <summary>
    /// Where Not 的逻辑<br/>
    /// 2022-12-8 Ciaran
    /// </summary>
    public static IEnumerable<TSource> WhereNot<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, bool> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        return predicate != null ? WhereNotIterator(source, predicate) : throw new ArgumentNullException(nameof(predicate));
    }

    private static IEnumerable<TSource> WhereNotIterator<TSource>(
        IEnumerable<TSource> source,
        Func<TSource, bool> predicate)
    {
        foreach (TSource source1 in source)
        {
            if (!predicate(source1))
                yield return source1;
        }
    }

    /// <summary>
    /// Where Not 的逻辑<br/>
    /// 2022-12-8 Ciaran
    /// </summary>
    public static IEnumerable<TSource> WhereNot<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, int, bool> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        return predicate != null ? WhereNotIterator(source, predicate) : throw new ArgumentNullException(nameof(predicate));
    }

    private static IEnumerable<TSource> WhereNotIterator<TSource>(
        IEnumerable<TSource> source,
        Func<TSource, int, bool> predicate)
    {
        int index = -1;
        foreach (TSource source1 in source)
        {
            if (!predicate(source1, checked(++index)))
                yield return source1;
        }
    }

    /// <summary>
    /// 映射为对应的映射函数
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static IEnumerable<Func<TResult>> SelectFunc<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(p => new Func<TResult>(() => selector(p)));

    /// <summary>
    /// 摘要:
    ///      计算 dynamic 对象序列的和。
    /// 
    ///  参数:
    ///    source:
    ///      用于计算和的对象序列。
    /// 
    ///  返回结果:
    ///      dynamic对象之和。
    /// 
    ///  异常:
    ///    dynamic:System.ArgumentNullException: source 为 null。
    ///    dynamic:实际类型未定义加法运算符
    ///    
    /// </summary>
    public static dynamic Sum(this IEnumerable<dynamic> source)
    {
        dynamic result = null;
        var enumerable = source.ToList();
        if (!enumerable.Any()) return result;
        foreach (var item in enumerable)
        {
            if (item == null) continue;
            result ??= Activator.CreateInstance(((object)item).GetType());
            result += item;
        }

        return result;
    }

    public static R FirstOrDefault<T, R>(this IEnumerable<T> source, Func<T, bool> predicate, Func<T, R> value_selector)
    {
        return source.Where(predicate).Select(value_selector).FirstOrDefault();
    }

    public static string StringJoin<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join(separator, source);
    }

    /// <summary>
    /// 2021-10-27 Ciaran 获取集合的枚举数
    /// </summary>
    /// <param name="enumerable1"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T?> AsEnumerable<T>(this IEnumerable enumerable1)
    {
        var enumerator1 = enumerable1.GetEnumerator();
        while (enumerator1.MoveNext())
        {
            yield return (T?)enumerator1.Current;
        }
    }

    /// <summary>
    /// 2021-10-27 Ciaran 遍历枚举数
    /// </summary>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        using var enum1 = source.GetEnumerator();
        while (enum1.MoveNext())
        {
            action(enum1.Current);
        }
    }

    /// <summary>
    /// 遍历<br/> 2023-1-28 Ciaran
    /// </summary>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        using var enum1 = source.GetEnumerator();
        int i = 0;
        while (enum1.MoveNext())
        {
            action(enum1.Current, i);
            checked
            {
                i++;
            }
        }
    }

    /// <summary>
    /// 遍历<br/> 2023-1-28 Ciaran<br/>
    /// 注意: 遍历过程中, 不可修改枚举列表
    /// </summary>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int, IEnumerable<T>> action)
    {
        var enumerable = source.ToList();
        using var enum1 = enumerable.GetEnumerator();
        int i = 0;
        while (enum1.MoveNext())
        {
            action(enum1.Current, i, enumerable);
            checked
            {
                i++;
            }
        }
    }

    /// <summary>
    /// 2022-6-21 Ciaran 返回当前元素 乘以 倍数的枚举数
    /// </summary>
    /// <param name="value"></param>
    /// <param name="repeat_count"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Repeat<T>(this T value, int repeat_count)
    {
        for (int i = 0; i < repeat_count; i++)
        {
            yield return value;
        }
    }

    /// <summary>
    /// 2022-10-18 Ciaran 获取双向链表的 节点枚举数
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<LinkedListNode<T>> AsNodeEnumerable<T>(this LinkedList<T> list)
    {
        var head = list.First;
        var current = head;
        do
        {
            if (current is null) break;

            yield return current;
            current = current.Next;
        } while (current != head);
    }
}

public static class IEnumerableHelper
{
    /// <summary>
    /// 2022-10-19 Ciaran 返回一个空的枚举数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Empty<T>()
    {
        yield break;
    }
}