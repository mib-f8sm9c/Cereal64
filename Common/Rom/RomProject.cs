using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;
using System.IO;
using Ionic.Zlib;
using Ionic.Zip;
using System.Text.RegularExpressions;

namespace Cereal64.Common.Rom
{
    public class RomProject : IXMLSerializable, ITreeNodeElement
    {
        private const string ROMPROJECT = "RomProject";
        private const string PROJECTNAME = "ProjectName";
        private const string SELECTEDDMA = "SelectedDMA";
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

        public DmaProfile SelectedDmaProfile { get { if (DMAProfiles.Count == 0) return null; return DMAProfiles[0]; } }

        [Browsable(false)]
        public int DmaProfileIndex { get { return _dmaProfileIndex; } }
        private int _dmaProfileIndex;

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

        //COME UP WITH BETTER NAME
        public ReadOnlyCollection<RomItem> Items { get { return _items.AsReadOnly(); } }
        private List<RomItem> _items;

        private RomProject()
        {
            _files = new List<RomFile>();
            _dmaProfiles = new List<DmaProfile>();
            _dmaProfileIndex = -1;
            _items = new List<RomItem>();

            ProjectName = "New Rom Project";
            RomInfo = new UserDefinedRomInfo();
        }

        public void AddRomFile(RomFile file)
        {
            _files.Add(file);
        }

        public void AddRomItem(RomItem item)
        {
            _items.Add(item);
        }

        public void AddDmaProfile(DmaProfile profile)
        {
            _dmaProfiles.Add(profile);
            if (_dmaProfileIndex == -1)
                _dmaProfileIndex = 0;
        }

        public void RemoveRomFile(RomFile file)
        {
            if (_files.Contains(file))
                _files.Remove(file);
        }

        public void RemoveRomItem(RomItem item)
        {
            if (_items.Contains(item))
                _items.Remove(item);
        }

        public void RemoveDmaProfile(DmaProfile profile)
        {
            if (_dmaProfiles.Contains(profile))
            {
                if(_dmaProfileIndex >= _dmaProfiles.IndexOf(profile))
                    _dmaProfileIndex--; //Will bring it to -1 when _dmaProfiles is emptied
                _dmaProfiles.Remove(profile);
            }
        }

        public void MoveRomFile(RomFile file, int newIndex)
        {
            if (!_files.Contains(file) || newIndex < 0 || newIndex >= _files.Count)
                return;

            _files.Remove(file);
            _files.Insert(newIndex, file);
        }

        public void MoveRomItem(RomItem item, int newIndex)
        {
            if (!_items.Contains(item) || newIndex < 0 || newIndex >= _items.Count)
                return;

            _items.Remove(item);
            _items.Insert(newIndex, item);
        }

        public static void Save(string filePath)
        {
            //Get as xml
            XElement xml = Instance.GetAsXML();

            XElement directoryXml = new XElement("directory");

            //Save to location
            using (var fs = File.Create(filePath))
            {
                using (ZipOutputStream s = new ZipOutputStream(fs))
                {
                    s.PutNextEntry("ProjectInfo");
                    byte[] bytes = Encoding.ASCII.GetBytes(xml.ToString());
                    s.Write(bytes, 0, bytes.Length);

                    for (int i = 0; i < Instance.Files.Count; i++)
                    {
                        s.PutNextEntry(Instance.Files[i].GetXMLPath());
                        bytes = Instance.Files[i].GetAsBytes();
                        s.Write(bytes, 0, bytes.Length);

                        XElement fileElement = new XElement("File");
                        fileElement.Add(new XAttribute("ID", Instance.Files[i].GetXMLName()));
                        fileElement.Add(new XAttribute("Path", Instance.Files[i].GetXMLPath()));
                        directoryXml.Add(fileElement);
                    }

                    foreach (RomItem item in Instance.Items)
                    {
                        bytes = Encoding.ASCII.GetBytes(item.GetAsXML().ToString());
                        s.PutNextEntry(item.GetXMLPath());
                        s.Write(bytes, 0, bytes.Length);

                        XElement fileElement = new XElement("Item");
                        fileElement.Add(new XAttribute("Name", item.GetXMLName()));
                        fileElement.Add(new XAttribute("Path", item.GetXMLPath()));
                        directoryXml.Add(fileElement);
                    }

                    s.PutNextEntry("Directory");
                    bytes = Encoding.ASCII.GetBytes(directoryXml.ToString());
                    s.Write(bytes, 0, bytes.Length);

                }
            }

            //Change the project path (???)
            Instance.ProjectPath = Path.GetDirectoryName(filePath);
        }

        public static void Load(string filePath)
        {
            //XmlDocument doc = new XmlDocument();
            XElement romProjectNode = null;
            XElement romDirectoryNode = null;
            Dictionary<int, byte[]> romData = new Dictionary<int, byte[]>();
            List<Tuple<string, XElement>> romItems = new List<Tuple<string, XElement>>();
            using (ZipFile zip = ZipFile.Read(filePath))
            {
                foreach (ZipEntry e in zip)
                {
                    if (e.FileName == "ProjectInfo")
                    {
                        MemoryStream projectStream = new MemoryStream();
                        e.Extract(projectStream);
                        romProjectNode = XElement.Parse(Encoding.ASCII.GetString(projectStream.ToArray()));
                    }
                    else if (e.FileName == "Directory")
                    {
                        MemoryStream projectStream = new MemoryStream();
                        e.Extract(projectStream);
                        romDirectoryNode = XElement.Parse(Encoding.ASCII.GetString(projectStream.ToArray()));
                    }
                    //else if (Regex.IsMatch(e.FileName, @"^\d+$"))//Numbers only, rom file
                    //{
                    //    MemoryStream projectStream = new MemoryStream();
                    //    e.Extract(projectStream);
                    //    byte[] data = projectStream.ToArray();
                    //    //projectStream.Read(data, 0, (int)projectStream.Length);
                    //    romData.Add(int.Parse(e.FileName), data);
                    //}
                    //else //Rom item
                    //{
                    //    MemoryStream projectStream = new MemoryStream();
                    //    e.Extract(projectStream);
                    //    XElement itemElement = XElement.Parse(Encoding.ASCII.GetString(projectStream.ToArray()));
                    //    romItems.Add(e.FileName, itemElement);
                    //}
                }

                //Load up the directory here
                foreach(XElement element in romDirectoryNode.Elements())
                {
                    if(element.Name == "File")
                    {
                        string elementPath = element.Attribute("Path").Value;
                        int elementNum = int.Parse(element.Attribute("ID").Value);

                        if(zip.ContainsEntry(elementPath))
                        {
                            ZipEntry e = zip.Entries.First(en => en.FileName == elementPath);

                            MemoryStream projectStream = new MemoryStream();
                            e.Extract(projectStream);
                            byte[] data = projectStream.ToArray();

                            romData.Add(elementNum, data);
                        }
                    }
                    else if (element.Name == "Item")
                    {
                        string elementPath = element.Attribute("Path").Value;
                        string elementName = element.Attribute("Name").Value;

                        if (zip.ContainsEntry(elementPath))
                        {
                            ZipEntry e = zip.Entries.First(en => en.FileName == elementPath);

                            MemoryStream projectStream = new MemoryStream();
                            e.Extract(projectStream);
                            XElement itemElement = XElement.Parse(Encoding.ASCII.GetString(projectStream.ToArray()));

                            romItems.Add(new Tuple<string, XElement>(elementName, itemElement));
                        }
                    }
                }

            }

            if (romProjectNode == null)
                return;

            //Start up the monster here boyo
            _instance = new RomProject();
            _instance.ProjectName = romProjectNode.Attribute(PROJECTNAME).Value;
            _instance._dmaProfileIndex = int.Parse(romProjectNode.Attribute(SELECTEDDMA).Value);
            _instance.ProjectPath = Path.GetDirectoryName(filePath);

            foreach (XElement element in romProjectNode.Elements())
            {
                if (element.Name == UserDefinedRomInfo.USERDEFINEDINFO)
                {
                    //Handle loading the user info
                    _instance.RomInfo = new UserDefinedRomInfo(element);
                }
                else if (element.Name == ROMFILES)
                {
                    foreach (XElement fileElement in element.Elements())
                    {
                        //Load a file
                        //Get the raw data
                        XAttribute attribute = fileElement.Attribute(RomFile.FILEID);
                        if (attribute == null)
                            continue;

                        int fileNum;
                        if (!int.TryParse(attribute.Value, out fileNum))
                            continue;

                        //Pass into the file using XElement
                        _instance.AddRomFile(new RomFile(fileElement, romData[fileNum]));
                    }
                }
                else if (element.Name == DMAPROFILES)
                {
                    foreach (XElement profileElement in element.Elements())
                    {
                        //Load the profiles
                        _instance.AddDmaProfile(new DmaProfile(profileElement));
                    }
                }
            }

            //Finish up the rom items here
            foreach(Tuple<string, XElement> itemPair in romItems)
            {
                _instance._items.Add(RomItemFactory.CreateRomItemFromType(itemPair.Item1, itemPair.Item2));
            }

            //Here, do stuff
            foreach (RomFile file in _instance.Files)
            {
                foreach (DataElements.N64DataElement element in file.Elements)
                {
                    element.PostXMLLoad();
                }
            }
        }

        public void Reset()
        {
            _instance = new RomProject();
        }

        public bool FindRamOffset(DmaAddress address, out RomFile file, out int fileOffset)
        {
            file = null;
            fileOffset = 0;

            if (SelectedDmaProfile == null || !SelectedDmaProfile.RamSegments.ContainsKey(address.Segment))
                return false;

            foreach (DmaSegment segment in SelectedDmaProfile.RamSegments[address.Segment])
            {
                if (address.Offset >= segment.RamStartOffset && address.Offset < segment.RamEndOffset)
                {
                    file = segment.File;
                    fileOffset = address.Offset - segment.RamStartOffset;
                    return true;
                }
            }

            return false;
        }

        public XElement GetAsXML()
        {
            XElement xml = new XElement(ROMPROJECT);
            xml.Add(new XAttribute(PROJECTNAME, ProjectName));
            xml.Add(new XAttribute(SELECTEDDMA, _dmaProfileIndex));

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
