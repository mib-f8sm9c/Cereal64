using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace Cereal64.Common
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

        [CategoryAttribute("Rom Project Elements"),
        DescriptionAttribute("Raw data files used in the project"),
        TypeConverter(typeof(CollectionConverter))]
        public ReadOnlyCollection<RomFile> Files { get { return _files.AsReadOnly(); } }
        private List<RomFile> _files;

        [CategoryAttribute("Rom Project Elements"),
        DescriptionAttribute("DMA Profiles used to define how data is sorted into the RAM segments"),
        TypeConverter(typeof(CollectionConverter))] //Array converter to allow it to open up in the propertygrid
        public ReadOnlyCollection<DMAProfile> DMAProfiles { get { return _dmaProfiles.AsReadOnly(); } }
        private List<DMAProfile> _dmaProfiles;

        private RomProject()
        {
            _files = new List<RomFile>();
            _dmaProfiles = new List<DMAProfile>();

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
            //Save to location
            //Change the project path
        }

        public static void Load(string filePath)
        {
            //Load file
            //Load into each part
            //Change the project path

            //NOTE: The XML Serialization needs to be expanded so that anyone can subscribe their own N64DataElement types and be able to
            //       load them correctly. UGH
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

            foreach (DMAProfile profile in _dmaProfiles)
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
            foreach (DMAProfile profile in _dmaProfiles)
                dmaNodes.Nodes.Add(profile.GetAsTreeNode());

            node.Nodes.Add(dmaNodes);

            return node;
        }
    }
}
