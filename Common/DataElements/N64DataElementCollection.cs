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

        public int GetIndexOfElement(N64DataElement element)
        {
            return _elements.IndexOf(element);
        }

        public bool GetElementExactlyAt(int offset, out N64DataElement element)
        {
            element = SearchForOffset(offset, true);
            return element != null;
        }

        public bool GetElementAt(int offset, out N64DataElement element)
        {
            element = SearchForOffset(offset, false);
            return element != null;
        }

        private N64DataElement SearchForOffset(int offset, bool exactMatch)
        {
            if (offset < 0)
                return null;

            if (offset > _elements.Last().RawDataSize + _elements.Last().FileOffset)
                return null;

            //Binary search, to speed up the process (thanks mom!)
            int start = 0;
            int end = _elements.Count;

            //WARNING: THIS WILL BREAK IF THE OBJECT SPANS MULTIPLE OBJECTS AND LANDS ON THE LATTER OBJECTS!!! FIX THIS PLEASE!!

            if (start != end)
            {
                while (start <= end)
                {
                    int mid = (start + end) / 2;

                    if (_elements[mid].FileOffset > offset)
                    {
                        end = mid - 1;
                        continue;
                    }
                    if (_elements[mid].FileOffset + _elements[mid].RawDataSize - 1 < offset)
                    {
                        start = mid + 1;
                        continue;
                    }
                    if (_elements[mid].ContainsOffset(offset))
                    {
                        if (exactMatch && _elements[mid].FileOffset != offset)
                            return null;

                        return _elements[mid];
                    }
                }
            }

            return null;
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

            //Binary search, to speed up the process (thanks mom!)
            int start = 0;
            int end = _elements.Count;

            if(start != end)
            {
                while (start <= end)
                {
                    int mid = (start + end) / 2;

                    if (_elements[mid].FileOffset > endOffset)
                    {
                        indexToAdd = mid - 1;
                        end = indexToAdd;
                        continue;
                    }
                    if (_elements[mid].FileOffset + _elements[mid].RawDataSize - 1 < startOffset)
                    {
                        indexToAdd = mid + 1;
                        start = indexToAdd;
                        continue;
                    }
                    if (_elements[mid].ContainsOffset(startOffset) ||
                        _elements[mid].ContainsOffset(endOffset) ||
                        element.ContainsOffset(_elements[mid].FileOffset))
                    {
                        indexToAdd = mid;
                        insideElement = true;
                        break;
                    }
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
                    int startingIndex = indexToAdd;
                    for (; endingIndex < _elements.Count - 1; endingIndex++)
                    {
                        //If it doesn't run into this next data element, then it stops before it. Break out of the loop
                        if (!_elements[endingIndex + 1].ContainsOffset(endOffset) &&
                            !element.ContainsOffset(_elements[endingIndex + 1].FileOffset))
                            break;

                        //Only unlocked unknown data may be split
                        if (!(_elements[endingIndex + 1] is UnknownData) || ((UnknownData)_elements[endingIndex + 1]).Locked)
                            return false;
                    }

                    for (; startingIndex > 0; startingIndex--)
                    {
                        //If it doesn't run into this next data element, then it stops before it. Break out of the loop
                        if (!_elements[startingIndex - 1].ContainsOffset(startOffset) &&
                            !element.ContainsOffset(_elements[startingIndex - 1].FileOffset + _elements[startingIndex - 1].RawDataSize - 1))
                            break;

                        //Only unlocked unknown data may be split
                        if (!(_elements[startingIndex - 1] is UnknownData) || ((UnknownData)_elements[startingIndex - 1]).Locked)
                            return false;
                    }

                    //Step two: determine how to split the start/ends.
                    bool startUnknownLeftOver = false;
                    bool endUnknownLeftOver = false;

                    if (_elements[startingIndex].ContainsOffset(startOffset) &&
                        _elements[startingIndex].FileOffset != startOffset)
                        startUnknownLeftOver = true;

                    if (_elements[endingIndex].ContainsOffset(endOffset) &&
                        _elements[endingIndex].FileOffset + _elements[endingIndex].RawDataSize - 1 != endOffset)
                        endUnknownLeftOver = true;

                    //Splitting a single file in half here!
                    if (startUnknownLeftOver && endUnknownLeftOver && startingIndex == endingIndex)
                    {
                        //Determine which unknown data is bigger, then create the smaller one as a new UnknownData to 
                        int firstUnknownStart = 0;
                        int firstUnknownLength = startOffset - _elements[startingIndex].FileOffset;
                        int secondUnknownStart = (endOffset + 1) - _elements[endingIndex].FileOffset;
                        int secondUnknownLength = _elements[endingIndex].RawDataSize - secondUnknownStart;

                        UnknownData newUnknown;
                        if (firstUnknownLength > secondUnknownLength)
                        {
                            byte[] unknownData = new byte[secondUnknownLength];
                            Array.Copy(_elements[startingIndex].RawData, secondUnknownStart, unknownData, 0, secondUnknownLength);
                            newUnknown = new UnknownData(_elements[startingIndex].FileOffset + secondUnknownStart, unknownData);
                            _elements.Insert(startingIndex + 1, newUnknown);

                            _elements.Insert(startingIndex + 1, element);

                            ((UnknownData)_elements[startingIndex]).TruncateData(firstUnknownStart, firstUnknownLength);

                            if (UnknownDataAdded != null)
                                UnknownDataAdded(newUnknown);
                        }
                        else
                        {
                            byte[] unknownData = new byte[firstUnknownLength];
                            Array.Copy(_elements[startingIndex].RawData, firstUnknownStart, unknownData, 0, firstUnknownLength);
                            newUnknown = new UnknownData(_elements[startingIndex].FileOffset + firstUnknownStart, unknownData);

                            ((UnknownData)_elements[startingIndex]).TruncateData(secondUnknownStart, secondUnknownLength);

                            _elements.Insert(startingIndex, element);

                            _elements.Insert(startingIndex, newUnknown);

                            if (UnknownDataAdded != null)
                                UnknownDataAdded(newUnknown);
                        }
                    }
                    else
                    {
                        if (startUnknownLeftOver)
                        {
                            ((UnknownData)_elements[startingIndex]).TruncateData(0, startOffset - _elements[startingIndex].FileOffset);
                            
                            startingIndex++;
                        }
                        if (endUnknownLeftOver)
                        {
                            ((UnknownData)_elements[startingIndex]).TruncateData((endOffset + 1) - _elements[endingIndex].FileOffset, _elements[endingIndex].FileOffset + _elements[endingIndex].RawDataSize - (endOffset + 1));
                            
                            endingIndex--;
                        }

                        //Step 3: Remove the overlapping unknowns, insert the new data
                        for (int i = 0; i <= endingIndex - startingIndex; i++)
                        {
                            if (UnknownDataRemoved != null)
                                UnknownDataRemoved((UnknownData)_elements[startingIndex]);

                            _elements.RemoveAt(startingIndex);
                        }

                        _elements.Insert(startingIndex, element);
                    }

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
            return _elements.Remove(element);
        }

        public void ClearElements()
        {
            _elements.Clear();
        }

        public byte[] GetAsBytes()
        {
            if(_elements.Count == 0)
                return new byte[0];

            int lastIndex = 0;

            foreach (N64DataElement element in _elements)
            {
                lastIndex = Math.Max(lastIndex, element.FileOffset + element.RawDataSize);
            }

            byte[] bytes = new byte[lastIndex];

            foreach (N64DataElement element in _elements)
            {
                Array.Copy(element.RawData, 0, bytes, element.FileOffset, element.RawDataSize);
            }

            return bytes;
        }

    }
}
