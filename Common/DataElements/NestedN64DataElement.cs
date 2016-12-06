using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Cereal64.Common.Utils;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace Cereal64.Common.DataElements
{
    public abstract class NestedN64DataElement : N64DataElement
    {
        private const string ELEMENTS = "Elements";

        public N64DataElement Element
        {
            get
            {
                if (Elements.Count != 1)
                    return null;

                return Elements[0];
            }
        }
        public ReadOnlyCollection<N64DataElement> Elements { get { return _elements.Elements.Where(e => !(e is UnknownData)).ToList().AsReadOnly(); } }
        protected N64DataElementCollection _elements;

        protected abstract byte[] InternalData { get; }

        public NestedN64DataElement(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
            //Now I need to copy in by hand all the following elements??
            //File data is the whole file, needs to separate it out
            _elements = new N64DataElementCollection();

            SetUpInternalData(RawData);

            InitializeNewInternalData();

            XElement elements = xml.Element(ELEMENTS);
            if (elements != null)
            {
                foreach (XElement el in elements.Elements())
                {
                    AddElement(N64DataElementFactory.CreateN64DataElementFromType(el.Name.ToString(),
                            el, InternalData));
                }
            }
        }

        public NestedN64DataElement(int offset, byte[] rawData)
            : base(offset, rawData)
        {
            _elements = new N64DataElementCollection();
            //Create an UnknownData to span all the internal data
            SetUpInternalData(rawData);
            InitializeNewInternalData();
        }

        /// <summary>
        /// Sets up the internal data 
        /// </summary>
        protected abstract void SetUpInternalData(byte[] rawData);

        protected void InitializeNewInternalData()
        {
            if (_elements == null)
                _elements = new N64DataElementCollection();

            if (_elements.Elements.Count > 0)
                _elements.ClearElements();

            if(InternalData != null)
                _elements.AddElement(new UnknownData(0, InternalData));
        }

        public bool AddElement(N64DataElement element)
        {
            if (_elements.Elements.Contains(element))
                return false;

            //Detect if the file size has increased due to this addition
            int fileOffsetEnd = element.FileOffset + element.RawDataSize;

            if (fileOffsetEnd > InternalData.Length || element.FileOffset < 0)
                return false;

            return _elements.AddElement(element);
        }

        public bool SwapElement(N64DataElement element)
        {
            throw new NotImplementedException();
        }

        public bool RemoveElement(N64DataElement element)
        {
            if (_elements.Elements.Contains(element))
                return false;

            if (_elements.RemoveElement(element))
            {
                return _elements.AddElement(new UnknownData(element.FileOffset, element.RawData));
            }

            return false;
        }

        public void ClearElements()
        {
            _elements.ClearElements();
            _elements.AddElement(new UnknownData(0, InternalData));
        }

        public override XElement GetAsXML()
        {
            XElement baseXml = base.GetAsXML();

            XElement elements = new XElement(ELEMENTS);

            //Here, add each element in elements as a new element
            foreach (N64DataElement element in _elements.Elements)
                elements.Add(element.GetAsXML());

            baseXml.Add(elements);

            return baseXml;
        }

        public override TreeNode GetAsTreeNode()
        {
            //To do: fix this up please!!
            return GetAsTreeNode();
        }

        public override void PostXMLLoad()
        {
            //If anything needs to be done post-loading via xml, do it here
        }
    }
}
