using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace CodeLib01.Models;

public class ListSet<T> : IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable,
    IList,
    ICollection,
    IReadOnlyList<T>,
    IReadOnlyCollection<T>, ISet<T>, IDeserializationCallback, ISerializable
{
    private readonly List<T> _list = new List<T>();
    private readonly HashSet<T> _set = new HashSet<T>();

    public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();


    public List<T> GetRange(int index, int count) => _list.GetRange(index, count);

    #region Set

    public bool Add(T item)
    {
        if (_set.Add(item))
        {
            _list.Add(item);
            return true;
        }

        return false;
    }

    void ICollection<T>.Add(T item)
    {
        ((ISet<T>)this).Add(item);
    }

    public int Add(object value)
    {
        return Add((T)value) ? this.Count - 1 : _list.IndexOf((T)value);
    }

    public void AddRange(IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            Add(item);
        }
    }

    public bool Remove(T item)
    {
        if (_set.Remove(item))
        {
            _list.Remove(item);
            return true;
        }

        return false;
    }

    void IList.Remove(object value)
    {
        Remove((T)value);
    }

    public int RemoveAll(Predicate<T> match)
    {
        _set.RemoveWhere(match);
        return _list.RemoveAll(match);
    }

    public void RemoveAt(int index)
    {
        _set.Remove(_list[index]);
        _list.RemoveAt(index);
    }

    public void RemoveRange(int index, int count)
    {
        for (; count > 0 && index < this.Count; count--)
        {
            RemoveAt(index);
        }
    }

    public void UnionWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));
        foreach (T obj in other)
            ((ISet<T>)this).Add(obj);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (other is ISet<T> set2)
        {
            foreach (T obj in _set)
                if (!set2.Contains(obj))
                    ((ISet<T>)this).Remove(obj);
        }
        else if (other is ICollection<T> collection)
        {
            foreach (T obj in _set)
                if (!collection.Contains(obj))
                    ((ISet<T>)this).Remove(obj);
        }
        else
        {
            var list = other.ToList();
            foreach (T obj in _set)
                if (!list.Contains(obj))
                    ((ISet<T>)this).Remove(obj);
        }
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));
        foreach (T obj in other)
            ((ISet<T>)this).Remove(obj);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));
        foreach (T obj in other)
            if (!((ISet<T>)this).Remove(obj))
                ((ISet<T>)this).Add(obj);
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return _set.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return _set.IsSupersetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return _set.IsProperSupersetOf(other);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return _set.IsProperSubsetOf(other);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        return _set.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        return _set.SetEquals(other);
    }

    public void Clear()
    {
        _set.Clear();
        _list.Clear();
    }


    public bool Contains(T item)
    {
        return _set.Contains(item);
    }

    bool IList.Contains(object value)
    {
        return _set.Contains((T)value);
    }

    #endregion


    #region List

    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => _list.ConvertAll(converter);

    public void CopyTo(T[] array) => this.CopyTo(array, 0);
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    public void CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);

    public void CopyTo(int index, T[] array, int arrayIndex, int count) => _list.CopyTo(index, array, arrayIndex, count);

    public bool Exists(Predicate<T> match) => _list.Exists(match);
    public T Find(Predicate<T> match) => _list.Find(match);
    public List<T> FindAll(Predicate<T> match) => _list.FindAll(match);
    public int FindIndex(Predicate<T> match) => _list.FindIndex(match);
    public int FindIndex(int startIndex, Predicate<T> match) => _list.FindIndex(startIndex, match);
    public int FindIndex(int startIndex, int count, Predicate<T> match) => _list.FindIndex(startIndex, count, match);
    public T FindLast(Predicate<T> match) => _list.FindLast(match);
    public int FindLastIndex(Predicate<T> match) => _list.FindLastIndex(match);
    public int FindLastIndex(int startIndex, Predicate<T> match) => _list.FindLastIndex(startIndex, match);
    public int FindLastIndex(int startIndex, int count, Predicate<T> match) => _list.FindLastIndex(startIndex, count, match);
    public void ForEach(Action<T> action) => _list.ForEach(action);


    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public int IndexOf(object value)
    {
        return IndexOf((T)value);
    }

    public void Insert(int index, T item)
    {
        if (!_set.Contains(item))
        {
            _list.Insert(index, item);
            _set.Add(item);
        }
    }

    public void Insert(int index, object value)
    {
        Insert(index, (T)value);
    }


    public void InsertRange(int index, IEnumerable<T> collection)
    {
        foreach (T item in collection)
            Insert(index++, item);
    }

    public int LastIndexOf(T item) => _list.LastIndexOf(item);
    public int LastIndexOf(T item, int index) => _list.LastIndexOf(item, index);
    public int LastIndexOf(T item, int index, int count) => _list.LastIndexOf(item, index, count);

    public T this[int index]
    {
        get => _list[index];
        set
        {
            var item = _list[index];
            _set.Remove(item);
            _set.Add(value);
            _list[index] = value;
        }
    }

    object IList.this[int index]
    {
        get => ((IList)_list)[index];
        set => this[index] = (T)value;
    }


    public ReadOnlyCollection<T> AsReadOnly() => new ReadOnlyCollection<T>((IList<T>)this);

    public int BinarySearch(int index, int count, T item, IComparer<T> comparer) => _list.BinarySearch(index, count, item, comparer);

    public int BinarySearch(T item) => BinarySearch(0, Count, item, (IComparer<T>)null);

    public int BinarySearch(T item, IComparer<T> comparer) => BinarySearch(0, Count, item, comparer);

    public void Reverse() => _list.Reverse(0, this.Count);

    public void Reverse(int index, int count) => _list.Reverse(index, count);

    public void Sort() => _list.Sort(0, _list.Count, (IComparer<T>)null);

    public void Sort(IComparer<T> comparer) => _list.Sort(0, _list.Count, comparer);

    public void Sort(int index, int count, IComparer<T> comparer) => _list.Sort(index, count, comparer);

    public void Sort(Comparison<T> comparison) => _list.Sort(comparison);
    public T[] ToArray() => _list.ToArray();

    public bool TrueForAll(Predicate<T> match) => _list.TrueForAll(match);

    public void TrimExcess()
    {
        _list.TrimExcess();
        _set.TrimExcess();
    }

    #endregion

    #region other

    public void OnDeserialization(object? sender)
    {
        ((IDeserializationCallback)_set).OnDeserialization(sender);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ((ISerializable)_set).GetObjectData(info, context);
    }


    public int Count => _set.Count;
    public object SyncRoot => ((ICollection)_list).SyncRoot;

    public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

    public bool IsReadOnly => ((ISet<T>)_set).IsReadOnly;
    public bool IsFixedSize => ((IList)_list).IsFixedSize;

    #endregion
}