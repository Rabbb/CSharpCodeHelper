using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace CodeLib01;

public static class NameValueCollectionExtension
{
    /// <summary>
    /// 转字典
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static Dictionary<string, string> ToDictionary(this System.Collections.Specialized.NameValueCollection collection)
    {
        int count = collection.Count;
        var dict = new Dictionary<string, string>(count);
        for (int i = 0; i < count; i++)
        {
            dict.Add(collection.GetKey(i), collection.Get(i));
        }

        return dict;
    }

    /// <summary>
    /// 转字典
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="format_value">格式化函数</param>
    /// <returns></returns>
    public static Dictionary<string, string> ToDictionary(this System.Collections.Specialized.NameValueCollection collection, Func<string, string>? format_value)
    {
        if (format_value is null)
        {
            return ToDictionary(collection);
        }

        int count = collection.Count;
        var dict = new Dictionary<string, string>(count);
        for (int i = 0; i < count; i++)
        {
            dict.Add(collection.GetKey(i), format_value(collection.Get(i)));
        }

        return dict;
    }

    /// <summary>
    /// 转字典
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="format_value">格式化函数</param>
    /// <returns></returns>
    public static Dictionary<string, string> ToDictionary(this System.Collections.Specialized.NameValueCollection collection, Func<string, string, string>? format_value)
    {
        if (format_value is null)
        {
            return ToDictionary(collection);
        }

        int count = collection.Count;
        var dict = new Dictionary<string, string>(count);
        for (int i = 0; i < count; i++)
        {
            string key = collection.GetKey(i);
            dict.Add(key, format_value(key, collection.Get(i)));
        }

        return dict;
    }

    /// <summary>
    /// 转多值字典
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static Dictionary<string, string[]> ToValuesDictionary(this System.Collections.Specialized.NameValueCollection collection)
    {
        int count = collection.Count;
        var dict = new Dictionary<string, string[]>(count);
        for (int i = 0; i < count; i++)
        {
            dict.Add(collection.GetKey(i), collection.GetValues(i));
        }

        return dict;
    }

    public static IEnumerable<KeyValuePair<string, string>> AsKeyValueEnumerable(this System.Collections.Specialized.NameValueCollection collection)
    {
        for (var e1 = new NameValueEnumerator(collection); e1.MoveNext();)
        {
            yield return e1.Current;
        }
    }

    public static IEnumerable<KeyValuePair<string, string[]>> AsKeyValuesEnumerable(this System.Collections.Specialized.NameValueCollection collection)
    {
        for (var e1 = new NameValuesEnumerator(collection); e1.MoveNext();)
        {
            yield return e1.Current;
        }
    }

    public static short? GetShort(this NameValueCollection that, string key)
    {
        return that[key]?.AsInt16();
    }

    public static int? GetInt(this NameValueCollection that, string key)
    {
        return that[key]?.AsInt32();
    }

    public static long? GetLong(this NameValueCollection that, string key)
    {
        return that[key]?.AsInt64();
    }

    public static double? GetDouble(this NameValueCollection that, string key)
    {
        return that[key]?.AsDouble();
    }

    public static decimal? GetDecimal(this NameValueCollection that, string key)
    {
        return that[key]?.AsDecimal();
    }

    public static DateTime? GetDateTime(this NameValueCollection that, string key)
    {
        return that[key]?.AsDateTime();
    }

    public class NameValueEnumerator : IEnumerator<KeyValuePair<string, string>>
    {
        System.Collections.IEnumerator key_enumerator;
        int _pos = -1;
        NameValueCollection _coll;

        MethodInfo GetValueMethod;

        public NameValueEnumerator(NameValueCollection collection)
        {
            this._coll = collection;
            key_enumerator = collection.GetEnumerator();
        }

        public KeyValuePair<string, string> Current => new KeyValuePair<string, string>((string)key_enumerator.Current, this._coll.Get(_pos));

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

    public class NameValuesEnumerator : IEnumerator<KeyValuePair<string, string[]>>
    {
        System.Collections.IEnumerator key_enumerator;
        int _pos = -1;
        NameValueCollection _coll;

        MethodInfo GetValueMethod;

        public NameValuesEnumerator(NameValueCollection collection)
        {
            this._coll = collection;
            key_enumerator = collection.GetEnumerator();
        }

        public KeyValuePair<string, string[]> Current => new KeyValuePair<string, string[]>((string)key_enumerator.Current, this._coll.GetValues(_pos));

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