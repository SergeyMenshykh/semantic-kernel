// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SemanticKernel.Plugins.OpenApi.Model;

/// <summary>
/// Represents a freezable collection.
/// </summary>
/// <param name="list">The list to initialize the collection with.</param>
public sealed class FreezableCollection<T>(IList<T>? list = null) : IList<T>, IList, IReadOnlyList<T>
{
    private readonly Freezable _freezable = new();
    private readonly IList<T> _list = list ?? [];

    /// <summary>
    /// Makes the current instance unmodifiable.
    /// </summary>
    public void Freeze()
    {
        this._freezable.Freeze();
    }

    /// <inheritdoc />
    public int Count => this._list.Count;

    /// <inheritdoc />
    public T this[int index]
    {
        get => this._list[index];
        set
        {
            this._freezable.ThrowIfFrozen();
            this._list[index] = value;
        }
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
        return this._list.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int index)
    {
        this._list.CopyTo(array, index);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return this._list.GetEnumerator();
    }

    /// <inheritdoc />
    public int IndexOf(T item)
    {
        return this._list.IndexOf(item);
    }

    /// <inheritdoc />
    bool ICollection<T>.IsReadOnly => this._freezable.IsFrozen;

    /// <inheritdoc />
    void ICollection<T>.Add(T item)
    {
        this._freezable.ThrowIfFrozen();
        this._list.Add(item);
    }

    /// <inheritdoc />
    void ICollection<T>.Clear()
    {
        this._freezable.ThrowIfFrozen();
        this._list.Clear();
    }

    /// <inheritdoc />
    void IList<T>.Insert(int index, T item)
    {
        this._freezable.ThrowIfFrozen();
        this._list.Insert(index, item);
    }

    /// <inheritdoc />
    bool ICollection<T>.Remove(T item)
    {
        this._freezable.ThrowIfFrozen();
        return this._list.Remove(item);
    }

    /// <inheritdoc />
    void IList<T>.RemoveAt(int index)
    {
        this._freezable.ThrowIfFrozen();
        this._list.RemoveAt(index);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this._list.GetEnumerator();
    }

    /// <inheritdoc />
    bool ICollection.IsSynchronized => false;

    /// <inheritdoc />
    object ICollection.SyncRoot => (this._list is ICollection coll) ? coll.SyncRoot : this;

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }
        if (array.Rank != 1)
        {
            throw new ArgumentException("Arg_RankMultiDimNotSupported");
            ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
        }
        if (array.GetLowerBound(0) != 0)
        {
            ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
        }
        if (index < 0)
        {
            ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
        }
        if (array.Length - index < Count)
        {
            ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
        }
        if (array is T[] items)
        {
            this._list.CopyTo(items, index);
        }
        else
        {
            // Additional checks and logic for copying to a non-generic array  
            object[] objects = array as object[] ?? throw new ArgumentException("Incompatible array type");
            for (int i = 0; i < this._list.Count; i++)
            {
                objects[index++] = this._list[i];
            }
        }
    }

    /// <inheritdoc />
    bool IList.IsFixedSize => false;

    /// <inheritdoc />
    bool IList.IsReadOnly => this._freezable.IsFrozen;

    /// <inheritdoc />
    object? IList.this[int index]
    {
        get => this._list[index];
        set
        {
            this._freezable.ThrowIfFrozen();
            this._list[index] = (T)value!;
        }
    }

    /// <inheritdoc />
    int IList.Add(object? value)
    {
        this._freezable.ThrowIfFrozen();
        this._list.Add((T)value!);
        return this._list.Count - 1; // Return the index at which the item was added  
    }

    /// <inheritdoc />
    void IList.Clear()
    {
        this._freezable.ThrowIfFrozen();
        this._list.Clear();
    }

    private static bool IsCompatibleObject(object? item)
    {
        return (item is T) || (item == null && default(T) == null);
    }

    /// <inheritdoc />
    bool IList.Contains(object? value)
    {
        return IsCompatibleObject(value) && this.Contains((T)value!);
    }

    /// <inheritdoc />
    int IList.IndexOf(object? value)
    {
        return IsCompatibleObject(value) ? this.IndexOf((T)value!) : -1;
    }

    /// <inheritdoc />
    void IList.Insert(int index, object? value)
    {
        this._freezable.ThrowIfFrozen();
        this._list.Insert(index, (T)value!);
    }

    /// <inheritdoc />
    void IList.Remove(object? value)
    {
        this._freezable.ThrowIfFrozen();
        if (IsCompatibleObject(value))
        {
            this._list.Remove((T)value!);
        }
    }

    /// <inheritdoc />
    void IList.RemoveAt(int index)
    {
        this._freezable.ThrowIfFrozen();
        this._list.RemoveAt(index);
    }
}
