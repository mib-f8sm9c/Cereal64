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

        public ReadOnlyCollection<N64DataElement> Elements { get { return _elements.Elements; } }
        private N64DataElementCollection _elements;

        public ReadOnlyCollection<IN64ElementContainer> ElementContainers { get { return _elementContainers.AsReadOnly(); } }
        private List<IN64ElementContainer> _elementContainers;

        public RomFile(string fileName, int fileID)
        {
            FileName = fileName;
            FileID = fileID;

            _elements = new N64DataElementCollection();
            _elementContainers = new List<IN64ElementContainer>();
        }

        public void AddElement(N64DataElement element)
        {
            if (!_elements.AddElement(element))
                return;

            foreach (IN64ElementContainer container in _elementContainers)
            {
                container.AddElement(element);
            }
        }

        public void RemoveElement(N64DataElement element)
        {
            _elements.RemoveElement(element);
        }

        public void AddElementContainer(IN64ElementContainer container)
        {
            if (_elementContainers.Contains(container))
                return;

            _elementContainers.Add(container);

            //Add prevoiusly existing elements
            foreach (N64DataElement element in _elements.Elements)
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
            return _elements.GetElementAt(offset);
        }

        //public byte[] GetAsBytes() { }
    }
}
