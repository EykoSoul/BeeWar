using System;
using System.Collections;
using System.Collections.Generic;

namespace BeeWar.Collections
{
    /// <summary>
    /// Собственная обобщённая коллекция для сущностей пчелиной фермы.
    /// Реализует ICollection&lt;T&gt;, IEnumerable&lt;T&gt;, ICloneable.
    /// Ограничение: T должен быть ссылочным типом (where T : class).
    /// Сортировка и сравнение задаются извне через делегаты Func/Action/Predicate.
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

        #region Индексатор и базовые методы

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

        #region Сортировка и сравнение через делегаты

        /// <summary>
        /// Сортировка по внешнему правилу — Func&lt;T, T, int&gt;.
        /// Возвращает отрицательное, ноль или положительное число
        /// в зависимости от порядка пары элементов.
        /// </summary>
        public void Sort(Func<T, T, int> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            Array.Sort(_items, 0, _count, new FuncComparer<T>(comparer));
            _version++;
        }

        /// <summary>
        /// Сортировка через стандартный IComparer&lt;T&gt;.
        /// </summary>
        public void Sort(IComparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            Array.Sort(_items, 0, _count, comparer);
            _version++;
        }

        /// <summary>
        /// Сортировка по ключу — Func&lt;T, TKey&gt;.
        /// Ключ извлекается из элемента, затем элементы сравниваются по ключу.
        /// </summary>
        public void SortBy<TKey>(Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            Array.Sort(_items, 0, _count,
                new FuncComparer<T>((a, b) => keySelector(a).CompareTo(keySelector(b))));
            _version++;
        }

        /// <summary>
        /// Сравнение двух элементов по внешнему правилу.
        /// Возвращает отрицательное, ноль или положительное число.
        /// </summary>
        public int Compare(T a, T b, Func<T, T, int> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;
            return comparer(a, b);
        }

        /// <summary>
        /// Найти первый элемент по условию (Predicate&lt;T&gt;).
        /// </summary>
        public T? Find(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            for (int i = 0; i < _count; i++)
                if (match(_items[i])) return _items[i];
            return null;
        }

        /// <summary>
        /// Найти все элементы по условию.
        /// </summary>
        public BeeCollection<T> FindAll(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            var result = new BeeCollection<T>();
            for (int i = 0; i < _count; i++)
                if (match(_items[i])) result.Add(_items[i]);
            return result;
        }

        /// <summary>
        /// Выполнить действие над каждым элементом (Action&lt;T&gt;).
        /// </summary>
        public void ForEach(Action<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (int i = 0; i < _count; i++)
                action(_items[i]);
        }

        /// <summary>
        /// Минимальный элемент по ключу Func&lt;T, TKey&gt;.
        /// </summary>
        public T? MinBy<TKey>(Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (_count == 0) return null;
            T min = _items[0];
            var minKey = keySelector(min);
            for (int i = 1; i < _count; i++)
            {
                var key = keySelector(_items[i]);
                if (key.CompareTo(minKey) < 0)
                {
                    min = _items[i];
                    minKey = key;
                }
            }
            return min;
        }

        /// <summary>
        /// Максимальный элемент по ключу Func&lt;T, TKey&gt;.
        /// </summary>
        public T? MaxBy<TKey>(Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (_count == 0) return null;
            T max = _items[0];
            var maxKey = keySelector(max);
            for (int i = 1; i < _count; i++)
            {
                var key = keySelector(_items[i]);
                if (key.CompareTo(maxKey) > 0)
                {
                    max = _items[i];
                    maxKey = key;
                }
            }
            return max;
        }

        #endregion

        #region Вспомогательный компаратор

        /// <summary>
        /// Превращает Func&lt;T, T, int&gt; в IComparer&lt;T&gt;.
        /// Нужен, потому что Array.Sort хочет IComparer&lt;T&gt;, а мы хотим принимать делегат.
        /// </summary>
        private class FuncComparer<TItem> : IComparer<TItem>
            where TItem : class
        {
            private readonly Func<TItem, TItem, int> _compare;
            public FuncComparer(Func<TItem, TItem, int> compare) { _compare = compare; }
            public int Compare(TItem? x, TItem? y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return _compare(x, y);
            }
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

            public void Reset() { _index = 0; _current = null; }
            public void Dispose() { }
        }

        #endregion
    }
}