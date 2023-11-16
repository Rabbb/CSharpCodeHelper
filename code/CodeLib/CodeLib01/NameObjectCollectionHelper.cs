using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace CodeLib01;

public static class NameObjectCollectionHelper
{
    /// <summary>
    /// 转字典
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static Dictionary<string, object> ToDictionary(this System.Collections.Specialized.NameObjectCollectionBase collection)
    {
        int count = collection.Count;
        var dict = new Dictionary<string, object>(count);
        foreach (var kv in collection.AsKeyValueEnumerable())
        {
            dict.Add(kv.Key, kv.Value);
        }

        return dict;
    }

    /// <summary>
    /// 转字典
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static Dictionary<string, T> ToDictionary<T>(this System.Collections.Specialized.NameObjectCollectionBase collection)
    {
        int count = collection.Count;
        var dict = new Dictionary<string, T>(count);
        foreach (var kv in collection.AsKeyValueEnumerable())
        {
            dict.Add(kv.Key, (T)kv.Value);
        }

        return dict;
    }

    /// <summary>
    /// 转字典
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="format_value">格式化函数</param>
    /// <returns></returns>
    public static Dictionary<string, T> ToDictionary<T>(this System.Collections.Specialized.NameObjectCollectionBase collection, Func<object, T>? format_value)
    {
        if (format_value is null)
        {
            return ToDictionary<T>(collection);
        }

        int count = collection.Count;
        var dict = new Dictionary<string, T>(count);
        foreach (var kv in collection.AsKeyValueEnumerable())
        {
            dict.Add(kv.Key, format_value(kv.Value));
        }

        return dict;
    }

    /// <summary>
    /// 转字典
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="format_value">格式化函数</param>
    /// <returns></returns>
    public static Dictionary<string, T> ToDictionary<T>(this System.Collections.Specialized.NameObjectCollectionBase collection, Func<string, object, T>? format_value)
    {
        if (format_value is null)
        {
            return ToDictionary<T>(collection);
        }

        int count = collection.Count;
        var dict = new Dictionary<string, T>(count);
        foreach (var kv in collection.AsKeyValueEnumerable())
        {
            dict.Add(kv.Key, format_value(kv.Key, kv.Value));
        }

        return dict;
    }

    public static IEnumerable<object> AsValueEnumerable(this System.Collections.Specialized.NameObjectCollectionBase collection)
    {
        for (var e1 = new NameObjectValueEnumerator(collection); e1.MoveNext();)
        {
            yield return e1.Current;
        }
    }

    public static IEnumerable<T> AsValueEnumerable<T>(this System.Collections.Specialized.NameObjectCollectionBase collection)
    {
        for (var e1 = new NameObjectValueEnumerator(collection); e1.MoveNext();)
        {
            yield return (T)e1.Current;
        }
    }

    public static IEnumerable<KeyValuePair<string, object>> AsKeyValueEnumerable(this System.Collections.Specialized.NameObjectCollectionBase collection)
    {
        for (var e1 = new NameObjectEnumerator(collection); e1.MoveNext();)
        {
            yield return e1.Current;
        }
    }

    public class NameObjectEnumerator : IEnumerator<KeyValuePair<string, object>>
    {
        System.Collections.IEnumerator key_enumerator;
        int _pos = -1;
        NameObjectCollectionBase _coll;

        MethodInfo GetValueMethod;

        public NameObjectEnumerator(NameObjectCollectionBase collectionBase)
        {
            this._coll = collectionBase;
            key_enumerator = collectionBase.GetEnumerator();

            GetValueMethod = typeof(NameObjectCollectionBase).GetMethod("BaseGet", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(int) }, null);
            if (GetValueMethod == null)
            {
                throw new TargetException("NameObjectCollectionBase 中缺少BaseGet(int) 方法");
            }
        }

        public KeyValuePair<string, object> Current => new KeyValuePair<string, object>((string)key_enumerator.Current, GetValueMethod.Invoke(this._coll, new object[] { _pos }));

        object System.Collections.IEnumerator.Current => this.Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (!key_enumerator.MoveNext()) return false;
            if (_pos < _coll.Count - 1)
            {
                _pos++;
                return true;
            }
            else
            {
                _pos = _coll.Count;
                return false;
            }
        }

        public void Reset()
        {
            key_enumerator.Reset();
            _pos = -1;
        }
    }


    public class NameObjectValueEnumerator : IEnumerator<object>
    {
        System.Collections.IEnumerator key_enumerator;
        int _pos = -1;
        NameObjectCollectionBase _coll;

        MethodInfo GetValueMethod;

        public NameObjectValueEnumerator(NameObjectCollectionBase collectionBase)
        {
            this._coll = collectionBase;
            key_enumerator = collectionBase.GetEnumerator();

            GetValueMethod = typeof(NameObjectCollectionBase).GetMethod("BaseGet", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(int) }, null);
            if (GetValueMethod == null)
            {
                throw new TargetException("NameObjectCollectionBase 中缺少BaseGet(int) 方法");
            }
        }

        public object Current => GetValueMethod.Invoke(this._coll, new object[] { _pos });

        object System.Collections.IEnumerator.Current => this.Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (!key_enumerator.MoveNext()) return false;
            if (_pos < _coll.Count - 1)
            {
                _pos++;
                return true;
            }
            else
            {
                _pos = _coll.Count;
                return false;
            }
        }

        public void Reset()
        {
            key_enumerator.Reset();
            _pos = -1;
        }
    }
}