using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Cereal64.Common.Utils
{
    /// <summary>
    /// A custom list class to allow for shrinking an initial array, but not growing it
    /// </summary>
    public class ShrinkingList<T> : IList<T>
    {
        public bool IsFixedSize { get { return true; } } //Not true entirely, but maybe this'll disallow adding
        public bool IsReadOnly { get { return false; } }

        private T[] _internalData;
        private int _startIndex = 0;
        private int _endIndex = 0;

        public ShrinkingList(int capacity)
        {
            _internalData = new T[capacity];
            _endIndex = capacity;
        }

        public ShrinkingList(T[] data)
        {
            _internalData = data;
            _endIndex = data.Length;
        }

        public T this[int index]
        {
            get
            {
                return _internalData[index + _startIndex];
            }
            set
            {
                _internalData[index + _startIndex] = value;
            }
        }

        public void Add(T value)
        {
            throw new Exception();
        }

        public void CopyTo(T[] newData, int newIndex)
        {
            Array.Copy(_internalData, _startIndex, newData, newIndex, Count);
        }

        public void CleanData()
        {
            if (_startIndex != 0 || _endIndex != _internalData.Length)
            {
                //Recopy the array into a new one
                T[] newArray = new T[Count];
                _startIndex = 0;
                _endIndex = newArray.Length;
                _internalData = newArray;
            }
        }

        public void Clear()
        {
            _endIndex = 0;
            _startIndex = 0;
            _internalData = new T[0];
        }

        public bool Contains(T value)
        {
            for (int i = _startIndex; i < _endIndex; i++)
            {
                if (_internalData[i].Equals(value))
                    return true;
            }
            return false;
        }

        public int IndexOf(T value)
        {
            for (int i = _startIndex; i < _endIndex; i++)
            {
                if (_internalData[i].Equals(value))
                    return i - _startIndex;
            }
            return -1;
        }

        public void Insert(int index, T value)
        {
            throw new Exception();
        }

        public bool Remove(T value)
        {
            throw new Exception();
        }

        public void RemoveAt(int index)
        {
            throw new Exception();
        }

        public void RemoveFromStart(int count)
        {
            _startIndex += count;
            if (_startIndex > _endIndex)
                Clear(); //Just clean out the data, it's no longer needed
        }

        public void RemoveFromEnd(int count)
        {
            _endIndex -= count;
            if (_endIndex < _startIndex)
                Clear(); //Just clean out the data, it's no longer needed
        }

        public int Count { get { return _endIndex - _startIndex; } }

        public IEnumerator<T> GetEnumerator()
        {
            return new ShrinkingListEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ShrinkingListEnumerator<T> : IEnumerator<T>
    {
        public T Current { get { return _list[_index]; } }
        object IEnumerator.Current { get { return Current; } }

        private ShrinkingList<T> _list;
        private int _index;
        
        public ShrinkingListEnumerator(ShrinkingList<T> list)
        {
            _list = list;
            _index = -1;

        }
        public void Reset()
        {
            _index = -1;
        }

        public bool MoveNext()
        {
            _index++;

            if (_index >= _list.Count)

                return false;

            else

                return true;
        }

        public void Dispose()
        {
            _list = null;
        }
    }
}
