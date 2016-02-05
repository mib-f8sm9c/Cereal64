﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;
using System.IO;

namespace Cereal64.Common.Rom
{
    public class RomProject : IXMLSerializable, ITreeNodeElement
    {
        private const string ROMPROJECT = "RomProject";
        private const string PROJECTNAME = "ProjectName";
        private const string ROMFILES = "RomFiles";
        private const string DMAPROFILES = "DmaProfiles";

        private static RomProject _instance = null;
        private static object syncObject = new object();

        [CategoryAttribute("Rom Project Settings"),
        DescriptionAttribute("Name of the Project")]
        public string ProjectName { get; set; }
        
        [Browsable(false)]
        public string ProjectPath { get; private set; }

        //[CategoryAttribute("Rom Project Settings"),
        //DescriptionAttribute("More info on the Rom Project")]//        TypeConverter(typeof(UserDefinedRomInfoTypeConverter))]
        [Browsable(false)]
        public UserDefinedRomInfo RomInfo { get; private set; }

        public static RomProject Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (syncObject)
                    {
                        if (_instance == null)
                            _instance = new RomProject();
                    }
                }
                return _instance;
            }
        }

        ///NOTE: ALLOW TO MANIPULATE IN THE PROPERTYGRID, OR DISALLOW?
        [CategoryAttribute("Rom Project Elements"),
        DescriptionAttribute("Raw data files used in the project"),
        TypeConverter(typeof(CollectionConverter))]
        public ReadOnlyCollection<RomFile> Files { get { return _files.AsReadOnly(); } }
        private List<RomFile> _files;

        [CategoryAttribute("Rom Project Elements"),
        DescriptionAttribute("DMA Profiles used to define how data is sorted into the RAM segments"),
        TypeConverter(typeof(CollectionConverter))] //Array converter to allow it to open up in the propertygrid
        public ReadOnlyCollection<DmaProfile> DMAProfiles { get { return _dmaProfiles.AsReadOnly(); } }
        private List<DmaProfile> _dmaProfiles;

        private RomProject()
        {
            _files = new List<RomFile>();
            _dmaProfiles = new List<DmaProfile>();

            ProjectName = "New Rom Project";
            RomInfo = new UserDefinedRomInfo();
        }

        public void AddRomFile(RomFile file)
        {
            _files.Add(file);
        }

        public void RemoveRomFile(RomFile file)
        {
            if (_files.Contains(file))
                _files.Remove(file);
        }

        public static void Save(string filePath)
        {
            //Get as xml
            XElement xml = Instance.GetAsXML();

            //Save to location
            using (XmlTextWriter writer = new XmlTextWriter(filePath, Encoding.ASCII))
            {
                writer.Formatting = Formatting.Indented;
                xml.Save(writer);
            }

            //Change the project path (???)
            Instance.ProjectPath = Path.GetDirectoryName(filePath);
        }

        public static void Load(string filePath)
        {
            //Load file
            XmlDocument doc = new XmlDocument();

            doc.Load(filePath);

            //Load into each part
            XmlElement romProjectNode = doc.DocumentElement;//. GetElementById(ROMPROJECT);
            if (romProjectNode == null)
                return;


            //Start up the monster here boyo
            XElement projectElement = XElement.Load(romProjectNode.CreateNavigator().ReadSubtree());

            RomProject newProj = new RomProject();
            newProj.ProjectName = projectElement.Attribute(PROJECTNAME).Value;
            newProj.ProjectPath = Path.GetDirectoryName(filePath);

            foreach (XElement element in projectElement.Elements())
            {
                if (element.Name == UserDefinedRomInfo.USERDEFINEDINFO)
                {
                    //Handle loading the user info
                    newProj.RomInfo = new UserDefinedRomInfo(element);
                }
                else if (element.Name == ROMFILES)
                {
                    foreach (XElement fileElement in element.Elements())
                    {
                        //Load a file
                        //Get the raw data
                        XAttribute attribute = fileElement.Attribute(RomFile.FILENAME);
                        if (attribute == null)
                            continue;

                        string fileP = attribute.Value;
                        if (!File.Exists(fileP))
                            continue;

                        byte[] data = File.ReadAllBytes(fileP);

                        //Pass into the file using XElement
                        newProj.AddRomFile(new RomFile(fileElement, data));
                    }
                }
                else if (element.Name == DMAPROFILES)
                {
                    foreach (XElement profileElement in element.Elements())
                    {
                        //Load the profiles
                    }
                }
            }

            _instance = newProj;
            //NOTE: The XML Serialization needs to be expanded so that anyone can subscribe their own N64DataElement types and be able to
            //       load them correctly. UGH
        }

        public bool FindRamOffset(DmaAddress address, out RomFile file, out int fileOffset)
        {
            file = null;
            fileOffset = 0;

            //To do: fill this in

            return false;
        }

        public XElement GetAsXML()
        {
            XElement xml = new XElement(ROMPROJECT);
            xml.Add(new XAttribute(PROJECTNAME, ProjectName));

            xml.Add(RomInfo.GetAsXML());

            //Files
            XElement files = new XElement(ROMFILES);

            foreach (RomFile file in _files)
                files.Add(file.GetAsXML());

            xml.Add(files);

            //Profiles
            XElement profiles = new XElement(DMAPROFILES);

            foreach (DmaProfile profile in _dmaProfiles)
                profiles.Add(profile.GetAsXML());

            xml.Add(profiles);

            return xml;
        }

        public TreeNode GetAsTreeNode()
        {
            TreeNode node = new TreeNode();
            node.Tag = this;
            node.Text = "Rom Project";
            //Add all stuff to the tree node

            node.Nodes.Add(RomInfo.GetAsTreeNode());

            TreeNode fileNodes = new TreeNode();
            fileNodes.Text = "Rom Files";
            foreach (RomFile file in _files)
                fileNodes.Nodes.Add(file.GetAsTreeNode());

            node.Nodes.Add(fileNodes);

            TreeNode dmaNodes = new TreeNode();
            dmaNodes.Text = "Dma Profiles";
            foreach (DmaProfile profile in _dmaProfiles)
                dmaNodes.Nodes.Add(profile.GetAsTreeNode());

            node.Nodes.Add(dmaNodes);

            return node;
        }

    }
}