using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Windows.Forms;
using Cereal64.Common.DataElements;

namespace Cereal64.Common.Rom
{
    public class RomFile : IN64ElementContainer
    {
        //TO DO: Add an Unsorted IContainer to hold elements that didn't get sorted into existing containers

        public const string ROMFILE = "RomFile";
        private const string FILEID = "FileId";
        public const string FILENAME = "FileName";
        private const string N64ELEMENTS = "N64Elements";
        private const string N64CONTAINERS = "N64Containers";

        public string FileName { get; set; }

        public string RelativeFilePath { get; set; } //Relative to the project file

        public int FileID { get; set; }

        public int FileLength { get; set; }

        public ReadOnlyCollection<N64DataElement> Elements { get { return _elements.Elements; } }
        private N64DataElementCollection _elements;

        public ReadOnlyCollection<IN64ElementContainer> ElementContainers { get { return _elementContainers.AsReadOnly(); } }
        private List<IN64ElementContainer> _elementContainers;
        private MiscN64ElementContainer _miscElementContainer; //contains elements not sorted into existing containers

        public RomFile(XElement xml, byte[] rawData)
        {
            _elements = new N64DataElementCollection();
            _elements.UnknownDataAdded += _elements_UnknownDataAdded;
            _elements.UnknownDataRemoved += _elements_UnknownDataRemoved;

            _elementContainers = new List<IN64ElementContainer>();
            _miscElementContainer = new MiscN64ElementContainer();

            XAttribute attribute = xml.Attribute(FILENAME);
            if (attribute != null)
                FileName = attribute.Value;

            attribute = xml.Attribute(FILEID);
            if (attribute != null)
                FileID = int.Parse(attribute.Value);

            //Here, start the arduous process of making all the N64Data Elements
            XElement n64Elements = xml.Element(N64ELEMENTS);
            if (n64Elements != null)
            {
                foreach (XElement element in n64Elements.Elements())
                {
                    N64DataElement newElement = N64DataElementFactory.CreateN64DataElementFromType(element.Name.ToString(),
                        element, rawData);

                    AddElement(newElement);
                }
            }

            XElement n64Conatiners = xml.Element(N64CONTAINERS);
            if (n64Conatiners != null)
            {
                foreach (XElement element in n64Conatiners.Elements())
                {
                    IN64ElementContainer newContainer = N64ElementContainerFactory.CreateN64ElementContainerFromType(element.Name.ToString());

                    AddElementContainer(newContainer);
                }
            }
        }

        public RomFile(string fileName, int fileID, UnknownData rawFileData)
        {
            FileName = fileName;
            FileID = fileID;

            _elements = new N64DataElementCollection();
            _elements.UnknownDataAdded += _elements_UnknownDataAdded;
            _elements.UnknownDataRemoved += _elements_UnknownDataRemoved;

            _elementContainers = new List<IN64ElementContainer>();
            _miscElementContainer = new MiscN64ElementContainer();

            _elements.AddElement(rawFileData);
        }

        public bool AddElement(N64DataElement element)
        {
            if (!_elements.AddElement(element))
                return false;

            if(_elementContainers.Count != 0)
            {
                bool foundContainer = false;
                foreach (IN64ElementContainer container in _elementContainers)
                {
                    if(container.AddElement(element))
                        foundContainer = true;
                }

                if (!foundContainer)
                {
                    //Toss it into the misc container
                    _miscElementContainer.AddElement(element);
                }
            }

            return true;
        }

        public bool RemoveElement(N64DataElement element)
        {
            if(!_elements.Elements.Contains(element))
                return false;

            _elements.RemoveElement(element);

            foreach (IN64ElementContainer container in _elementContainers)
            {
                container.RemoveElement(element);
            }

            _miscElementContainer.RemoveElement(element);

            return true;
        }

        public bool AddElementContainer(IN64ElementContainer container)
        {
            if (_elementContainers.Contains(container))
                return false;

            if (_elementContainers.Count == 0)
            {
                //throw everything into the misc. element container
                foreach (N64DataElement element in _elements.Elements)
                    _miscElementContainer.AddElement(element);
            }

            _elementContainers.Add(container);

            //Add prevoiusly existing elements
            foreach (N64DataElement element in _elements.Elements)
            {
                if (container.AddElement(element))
                    _miscElementContainer.RemoveElement(element);
            }

            return true;
        }

        public bool RemoveElementContainer(IN64ElementContainer container)
        {
            if (!_elementContainers.Contains(container))
                return false;

            _elementContainers.Remove(container);

            return true;
        }

        public N64DataElement GetElementAt(int offset)
        {
            return _elements.GetElementAt(offset);
        }

        //public byte[] GetAsBytes() { }

        public XElement GetAsXML()
        {
            XElement xml = new XElement(ROMFILE);
            xml.Add(new XAttribute(FILENAME, FileName));
            xml.Add(new XAttribute(FILEID, FileID));

            //handle saving all the data here
            XElement elements = new XElement(N64ELEMENTS);
            foreach (N64DataElement element in _elements.Elements)
            {
                elements.Add(element.GetAsXML());
            }
            xml.Add(elements);

            XElement containers = new XElement(N64CONTAINERS);
            foreach (IN64ElementContainer container in _elementContainers)
            {
                containers.Add(new XElement(container.GetType().ToString()));
            }
            xml.Add(containers);

            return xml;
        }

        public TreeNode GetAsTreeNode()
        {
            TreeNode RomFileNode = new TreeNode();
            RomFileNode.Tag = this;
            RomFileNode.Text = string.Format("File {0}", this.FileID);

            if (_elementContainers.Count == 0)
            {
                //Just display all elements
                TreeNode elements = new TreeNode();
                elements.Text = "Elements";

                foreach (N64DataElement element in _elements.Elements)
                {
                    elements.Nodes.Add(element.GetAsTreeNode());
                }

                RomFileNode.Nodes.Add(elements);
            }
            else
            {
                foreach (IN64ElementContainer container in _elementContainers)
                    RomFileNode.Nodes.Add(container.GetAsTreeNode());

                RomFileNode.Nodes.Add(_miscElementContainer.GetAsTreeNode());
            }

            return RomFileNode;
        }

        /// <summary>
        /// Allow split data from the N64DataElementCollection to still be sorted into containers
        /// </summary>
        /// <param name="data"></param>
        private void _elements_UnknownDataRemoved(UnknownData data)
        {
            foreach (IN64ElementContainer container in _elementContainers)
            {
                container.RemoveElement(data);
            }
            _miscElementContainer.RemoveElement(data);
        }

        /// <summary>
        /// Allow split data from the N64DataElementCollection to still be sorted into containers
        /// </summary>
        /// <param name="data"></param>
        private void _elements_UnknownDataAdded(UnknownData data)
        {
            bool added = false;

            foreach (IN64ElementContainer container in _elementContainers)
            {
                if (container.AddElement(data))
                    added = true;
            }

            if(!added)
                _miscElementContainer.AddElement(data);
        }

        //To do: Find a way to cache this so we don't spend a huge amount of space always creaing new data
        public byte[] GetAsBytes()
        {
            byte [] bytes;

            //Go through each element, and add up how large it is
            int endOffset = int.MinValue;

            foreach (N64DataElement element in _elements.Elements)
            {
                endOffset = Math.Max(endOffset, element.FileOffset + element.RawDataSize);
            }

            bytes = new byte[endOffset];

            foreach (N64DataElement element in _elements.Elements)
            {
                Array.Copy(element.RawData, 0, bytes, element.FileOffset, element.RawDataSize);
            }

            return bytes;
        }

        public void LoadReferencesFromGUID()
        {
        }

    }
}
