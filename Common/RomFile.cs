using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Cereal64.Common
{
    public class RomFile : IN64ElementContainer
    {
        public string FileName { get; set; }

        public int FileID { get; set; }

        public ReadOnlyCollection<N64DataElement> Elements { get { return _elements.AsReadOnly(); } }
        private List<N64DataElement> _elements;

        public ReadOnlyCollection<IN64ElementContainer> ElementContainers { get { return _elementContainers.AsReadOnly(); } }
        private List<IN64ElementContainer> _elementContainers;

        public RomFile(string fileName, int fileID)
        {
            FileName = fileName;
            FileID = fileID;

            _elements = new List<N64DataElement>();
            _elementContainers = new List<IN64ElementContainer>();
        }

        public void AddElement(N64DataElement element)
        {
            if (_elements.Contains(element))
                return;

            _elements.Add(element);

            foreach (IN64ElementContainer container in _elementContainers)
            {
                container.AddElement(element);
            }
        }

        public void RemoveElement(N64DataElement element)
        {
            if (!_elements.Contains(element))
                return;

            _elements.Remove(element);

            foreach (IN64ElementContainer container in _elementContainers)
            {
                container.RemoveElement(element);
            }
        }

        public void AddElementContainer(IN64ElementContainer container)
        {
            if (_elementContainers.Contains(container))
                return;

            _elementContainers.Add(container);

            //Add prevoiusly existing elements
            foreach (N64DataElement element in _elements)
            {
                container.AddElement(element);
            }
        }

        public void RemoveElementContainer(IN64ElementContainer container)
        {
            if (!_elementContainers.Contains(container))
                return;

            _elementContainers.Remove(container);
        }

        public N64DataElement GetElementAt(int offset)
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i].ContainsOffset(offset))
                    return _elements[i];
            }

            return null;
        }

        //public byte[] GetAsBytes() { }
    }
}
