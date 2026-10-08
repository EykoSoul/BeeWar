using System;
using System.Collections;
using System.Collections.Generic;

namespace BeeWar.Collections
{
    /// <summary>
    /// Собственная обобщённая коллекция для сущностей пчелиной фермы.
    /// Реализует ICollection&lt;T&gt;, IEnumerable&lt;T&gt;, ICloneable.
    /// Ограничение: T должен быть ссылочным типом (where T : class).
    /// </summary>
    public class BeeCollection<T> : ICollection<T>, ICloneable
        where T : class
    {
        private T[] _items;
        private int _count;
        private int _version;
        private const int DefaultCapacity = 4;

        #region Конструкторы

        public BeeCollection()
        {
            _items = new T[DefaultCapacity];
            _count = 0;
        }

        public BeeCollection(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            _items = new T[capacity];
            _count = 0;
        }

        public BeeCollection(IEnumerable<T> source) : this()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (var item in source) Add(item);
        }

        #endregion

        #region ICollection<T>

        public int Count => _count;
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            EnsureCapacity(_count + 1);
            _items[_count++] = item;
            _version++;
        }

        public bool Remove(T item)
        {
            int idx = IndexOf(item);
            if (idx < 0) return false;
            RemoveAt(idx);
            return true;
        }

        public bool Contains(T item) => IndexOf(item) >= 0;

        public void Clear()
        {
            Array.Clear(_items, 0, _count);
            _count = 0;
            _version++;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex + _count > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            Array.Copy(_items, 0, array, arrayIndex, _count);
        }

        #endregion

        #region Дополнительные методы

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count) throw new IndexOutOfRangeException();
                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _count) throw new IndexOutOfRangeException();
                _items[index] = value ?? throw new ArgumentNullException(nameof(value));
                _version++;
            }
        }

        public int IndexOf(T item) => Array.IndexOf(_items, item, 0, _count);

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count) throw new IndexOutOfRangeException();
            _count--;
            Array.Copy(_items, index + 1, _items, index, _count - index);
            _items[_count] = null!;
            _version++;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > _count) throw new IndexOutOfRangeException();
            if (item == null) throw new ArgumentNullException(nameof(item));
            EnsureCapacity(_count + 1);
            Array.Copy(_items, index, _items, index + 1, _count - index);
            _items[index] = item;
            _count++;
            _version++;
        }

        public T[] ToArray()
        {
            var arr = new T[_count];
            Array.Copy(_items, arr, _count);
            return arr;
        }

        public int RemoveAll(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            int removed = 0;
            for (int i = _count - 1; i >= 0; i--)
            {
                if (match(_items[i]))
                {
                    RemoveAt(i);
                    removed++;
                }
            }
            return removed;
        }

        private void EnsureCapacity(int min)
        {
            if (_items.Length >= min) return;
            int newCap = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
            if (newCap < min) newCap = min;
            var newArr = new T[newCap];
            Array.Copy(_items, newArr, _count);
            _items = newArr;
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            var clone = new BeeCollection<T>(_count);
            Array.Copy(_items, clone._items, _count);
            clone._count = _count;
            return clone;
        }

        public BeeCollection<T> DeepClone()
        {
            var clone = new BeeCollection<T>(_count);
            for (int i = 0; i < _count; i++)
            {
                var item = _items[i];
                if (item is ICloneable c)
                    clone.Add((T)c.Clone());
                else
                    clone.Add(item);
            }
            return clone;
        }

        #endregion

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator() => new BeeEnumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Внутренний перечислитель

        private class BeeEnumerator<TItem> : IEnumerator<TItem>
            where TItem : class
        {
            private readonly BeeCollection<TItem> _collection;
            private readonly int _version;
            private int _index;
            private TItem? _current;

            public BeeEnumerator(BeeCollection<TItem> collection)
            {
                _collection = collection;
                _version = collection._version;
                _index = 0;
                _current = null;
            }

            public TItem Current => _current!;
            object? IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_version != _collection._version)
                    throw new InvalidOperationException(
                        "Коллекция была изменена во время перечисления.");

                if (_index < _collection._count)
                {
                    _current = _collection._items[_index];
                    _index++;
                    return true;
                }
                _current = null;
                return false;
            }

            public void Reset()
            {
                _index = 0;
                _current = null;
            }

            public void Dispose() { }
        }

        #endregion
    }
}