using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Cereal64.Common
{
    public class RomFile : IN64ElementContainer
    {
        //TO DO: Add an Unsorted IContainer to hold elements that didn't get sorted into existing containers

        private const string ROMFILE = "RomFile";
        private const string FILEID = "FileId";
        private const string N64ELEMENTS = "N64Elements";

        public string FileName { get; set; }

        public string RelativeFilePath { get; set; } //Relative to the project file

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

            if(_elementContainers.Count != 0)
            {
                bool foundContainer = false;
                foreach (IN64ElementContainer container in _elementContainers)
                {
                    container.AddElement(element);
                    foundContainer = true;
                }

                if (!foundContainer)
                {
                    //Toss it into the misc container
                }
            }
        }

        public void RemoveElement(N64DataElement element)
        {
            _elements.RemoveElement(element);

            //TO DO: Add container search for elements
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

        public XElement GetAsXML()
        {
            XElement xml = new XElement(ROMFILE);
            xml.Add(new XAttribute(FILEID, FileID));

            //handle saving all the data here
            XElement elements = new XElement(N64ELEMENTS);
            foreach (N64DataElement element in _elements.Elements)
            {
                elements.Add(element.GetAsXML());
            }

            return xml;
        }

        public TreeNode GetAsTreeNode()
        {
            TreeNode RomFileNode = new TreeNode();
            RomFileNode.Text = string.Format("File {0}", this.FileID);

            if (_elementContainers.Count == 0)
            {
                //Just display all elements
                TreeNode elements = new TreeNode();
                foreach (N64DataElement element in _elements.Elements)
                {
                    //elements.Nodes.Add(element.GetAsTreeNode());
                }

                RomFileNode.Nodes.Add(elements);
            }
            else
            {
                foreach (IN64ElementContainer container in _elementContainers)
                    RomFileNode.Nodes.Add(container.GetAsTreeNode());
            }

            return RomFileNode;
        }
    }
}
