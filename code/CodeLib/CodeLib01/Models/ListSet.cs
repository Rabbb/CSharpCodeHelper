using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CodeLib01.Models;

public class ListSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, ISet<T>, IDeserializationCallback, ISerializable
{
    private readonly List<T> _list = new List<T>();
    private readonly HashSet<T> _set = new HashSet<T>();

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        Remove(item);
        _set.Add(item);
        _list.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _set.Remove(_list[index]);
        _list.RemoveAt(index);
    }

    public T this[int index]
    {
        get => _list[index];
        set => Insert(index, value);
    }

    void ICollection<T>.Add(T item)
    {
        ((ISet<T>)this).Add(item);
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

    public bool Add(T item)
    {
        if (_set.Add(item))
        {
            _list.Add(item);
            return true;
        }

        return false;
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

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
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

    public void OnDeserialization(object? sender)
    {
        ((IDeserializationCallback)_set).OnDeserialization(sender);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ((ISerializable)_set).GetObjectData(info, context);
    }

    public int Count => _set.Count;

    public bool IsReadOnly => ((ISet<T>)_set).IsReadOnly;
}