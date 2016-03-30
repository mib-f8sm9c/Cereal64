using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Cereal64.Common.DataElements
{
    //To do: Go back and check all the offset-checking code. ContainsOffset doesn't consider
    //        that an end offset given as a parameter is an exclusive value

    //Also to do: Switch to using Lists better so you don't Array.Copy a bajillion times to do everything

    public class N64DataElementCollection
    {
        public delegate void AddedUnknownDataEvent(UnknownData newData);

        public event AddedUnknownDataEvent UnknownDataAdded = delegate { };

        public delegate void RemovedUnknownDataEvent(UnknownData newData);

        public event RemovedUnknownDataEvent UnknownDataRemoved = delegate { };

        //Will add elements in order by offset. Will also reject adding elements
        // when they conflict with existing elements
        private List<N64DataElement> _elements;

        public ReadOnlyCollection<N64DataElement> Elements { get { return _elements.AsReadOnly(); } }

        public N64DataElementCollection()
        {
            _elements = new List<N64DataElement>();
        }

        public bool HasElement(N64DataElement element)
        {
            return _elements.Exists(e => e == element);
        }

        public int GetIndexOfElement(N64DataElement element)
        {
            return _elements.IndexOf(element);
        }

        public bool HasElementExactlyAt(int offset)
        {
            return _elements.Exists(e => e.FileOffset == offset);
        }

        public N64DataElement GetElementAt(int offset)
        {
            return _elements.SingleOrDefault(e => e.ContainsOffset(offset));
        }

        public bool AddElement(N64DataElement element, bool overwriteUnknownData = true)
        {
            if (element.FileOffset < 0)
                return false;

            int startOffset = element.FileOffset;
            int endOffset = startOffset + element.RawDataSize - 1;

            //Find if it is inside a currently existing element, or if it can be added freely
            int indexToAdd = 0;
            bool insideElement = false;

            for (; indexToAdd < _elements.Count; indexToAdd++)
            {
                if (endOffset < _elements[indexToAdd].FileOffset)
                {
                    //Found where to insert the new element
                    break;
                }
                else if (_elements[indexToAdd].ContainsOffset(startOffset) ||
                    _elements[indexToAdd].ContainsOffset(endOffset) ||
                    element.ContainsOffset(_elements[indexToAdd].FileOffset))
                {
                    insideElement = true;
                    break;
                }
            }

            //If there were conflicts
            if (insideElement)
            {
                //Determine if we can overwrite unknown data with known data
                if (_elements[indexToAdd] is UnknownData && overwriteUnknownData && !((UnknownData)_elements[indexToAdd]).Locked)
                {
                    //Step one: determine how many elements this new one spans
                    int endingIndex = indexToAdd;
                    for (; endingIndex < _elements.Count - 1; endingIndex++)
                    {
                        //If it doesn't run into this next data element, then it stops before it. Break out of the loop
                        if (!_elements[endingIndex + 1].ContainsOffset(endOffset) &&
                            !element.ContainsOffset(_elements[endingIndex + 1].FileOffset))
                            break;

                        //Only unlocked unknown data may be split
                        if (!(_elements[endingIndex + 1] is UnknownData) || ((UnknownData)_elements[indexToAdd]).Locked)
                            return false;
                    }

                    //Step two: determine how to split the start/ends.
                    bool startUnknownLeftOver = false;
                    bool endUnknownLeftOver = false;

                    if (_elements[indexToAdd].ContainsOffset(startOffset) &&
                        _elements[indexToAdd].FileOffset != startOffset)
                        startUnknownLeftOver = true;

                    if (_elements[endingIndex].ContainsOffset(endOffset) &&
                        _elements[endingIndex].FileOffset + _elements[endingIndex].RawDataSize - 1 != endOffset)
                        endUnknownLeftOver = true;

                    if (startUnknownLeftOver)
                    {
                        byte[] unknownData = new byte[startOffset - _elements[indexToAdd].FileOffset];
                        Array.Copy(_elements[indexToAdd].RawData, 0, unknownData, 0, unknownData.Length);

                        UnknownData newData = new UnknownData(_elements[indexToAdd].FileOffset, unknownData);
                        _elements.Insert(indexToAdd, newData);

                        if (UnknownDataAdded != null)
                            UnknownDataAdded(newData);

                        indexToAdd++;
                        endingIndex++;
                    }

                    if (endUnknownLeftOver)
                    {
                        byte[] unknownData = new byte[_elements[endingIndex].FileOffset + _elements[endingIndex].RawDataSize - (endOffset + 1)];
                        Array.Copy(_elements[endingIndex].RawData, (endOffset + 1) - _elements[endingIndex].FileOffset, unknownData, 0, unknownData.Length);

                        UnknownData newData = new UnknownData(endOffset + 1, unknownData);
                        _elements.Insert(endingIndex + 1, newData);

                        if (UnknownDataAdded != null)
                            UnknownDataAdded(newData);
                    }

                    //Step 3: Remove the overlapping unknowns, insert the new data
                    for (int i = 0; i <= endingIndex - indexToAdd; i++)
                    {
                        if (UnknownDataRemoved != null)
                            UnknownDataRemoved((UnknownData)_elements[indexToAdd]);

                        _elements.RemoveAt(indexToAdd);
                    }
                    _elements.Insert(indexToAdd, element);

                    return true;
                }
                else
                {
                    return false;
                }
            }

            //Down here, there were no conflicts
            _elements.Insert(indexToAdd, element);
            return true;
        }

        public bool RemoveElement(N64DataElement element)
        {
            if (!HasElement(element))
                return false;

            _elements.Remove(element);
            return true;
        }

    }
}
