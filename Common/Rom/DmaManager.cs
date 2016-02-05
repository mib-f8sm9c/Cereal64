using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Cereal64.Common.Rom
{
    public class DmaManager
    {
        private static DmaManager _instance = null;
        private static object syncObject = new object();

        public static DmaManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (syncObject)
                    {
                        if (_instance == null)
                            _instance = new DmaManager();
                    }
                }
                return _instance;
            }
        }

        public ReadOnlyCollection<DmaProfile> DMAProfiles { get { return _dmaProfiles.AsReadOnly(); } }
        private List<DmaProfile> _dmaProfiles;

        public DmaProfile SelectedDmaProfile;

        private DmaManager()
        {
            _dmaProfiles = new List<DmaProfile>();
        }

        public void AddNewDmaProfile(DmaProfile profile)
        {
            if(!_dmaProfiles.Any(dma => dma.ProfileName == profile.ProfileName))
            {
                _dmaProfiles.Add(profile);
            }
        }

        public void RemoveDmaProfile(string profileName)
        {
            DmaProfile profile = _dmaProfiles.First(dma => dma.ProfileName == profileName);

            if (profile != null)
            {
                _dmaProfiles.Remove(profile);
            }
        }

        public void ClearDMAProfilers()
        {
            _dmaProfiles.Clear();
        }

        public bool RamOffsetToFileOffset(int ramAddress, out RomFile file, out int fileOffset)
        {
            file = null;
            fileOffset = -1;

            byte ramSegment = (byte)(ramAddress >> 24);
            int ramOffset = ramAddress & 0x00FFFFFF;

            //If the RAMsegments doesn't contain the segment number, return null
            if (!SelectedDmaProfile.RamSegments.ContainsKey(ramSegment))
                return false;

            foreach (DmaSegment dmaSegment in SelectedDmaProfile.RamSegments[ramSegment])
            {
                //If it's outside the range of the RAMSegment, continue
                if (ramOffset < dmaSegment.RamStartOffset || ramOffset >= dmaSegment.RamEndOffset)
                    continue;

                //Success
                file = dmaSegment.File;
                fileOffset = dmaSegment.FileStartOffset;
                return true;
            }

            return false;
        }

        public bool FileOffsetToRamOffset(RomFile file, int fileOffset, out int ramOffset)
        {
            ramOffset = -1;

            foreach (List<DmaSegment> dma in SelectedDmaProfile.RamSegments.Values)
            {
                foreach (DmaSegment dmaSegment in dma)
                {
                    if (dmaSegment.File == file)
                    {
                        if (fileOffset < dmaSegment.FileStartOffset || fileOffset >= dmaSegment.FileEndOffset)
                            continue;

                        //Success
                        ramOffset = (fileOffset - dmaSegment.FileStartOffset) + dmaSegment.RamStartOffset;
                        return true;
                    }
                }
            }

            return false;
        }

        #region Old methods (Delete when DMAManager is working)

        //public bool FindDMAAddress(byte segment, int offset, out RomFile file, out int fileOffset)
        //{
        //    file = null;
        //    fileOffset = 0;

        //    if (!SelectedDMAProfile.RAMSegments.ContainsKey(segment))
        //        return false;

        //    DMASegment dma = SelectedDMAProfile.RAMSegments[segment];

        //    file = dma.File;
        //    fileOffset = offset - dma.FileStartOffset;

        //    return dma.FileEndOffset > offset && offset >= dma.FileStartOffset; //double check that the offset falls within the range of the dma segment
        //}

        ///// <summary>
        ///// Applies the DMA segments to the N64Addresses in all N64DataElements
        ///// </summary>
        ///// <param name="profile">The DMA profile to use as reference</param>
        //public void ApplyProfile(DMAProfile profile)
        //{
        //    if (!_dmaProfiles.Contains(profile))
        //        return;

        //    for (int i = 0; i < RomProject.Instance.Files.Count; i++)
        //    {
        //        //profile.RAMSegments.Any(
        //    }

        //    SelectedDMAProfile = profile;
        //}

        #endregion

    }

    public class DmaProfile : IXMLSerializable, ITreeNodeElement
    {
        private const string DMAPROFILE = "DmaProfile";
        private const string RAMSEGMENTS = "RamSegments";
        private const string PROFILENAME = "ProfileName";
        private const string TAGINFO = "TagInfo";
        private const string SEGMENT = "Segment";
        private const string SEGMENTNUM = "SegmentNum";

        public Dictionary<byte, List<DmaSegment>> RamSegments;
        public string ProfileName;
        public string TagInfo;

        public DmaProfile(string name)
        {
            ProfileName = name;
            RamSegments = new Dictionary<byte, List<DmaSegment>>();
            TagInfo = string.Empty;
        }

        public bool AddDmaSegment(byte segment, DmaSegment dma)
        {
            if (dma.FileStartOffset >= dma.FileEndOffset)
                return false;
            if (dma.RamStartOffset >= dma.RamEndOffset)
                return false;

            if (!RamSegments.Keys.Contains(segment))
                RamSegments.Add(segment, new List<DmaSegment>());

            foreach(DmaSegment seg in RamSegments[segment])
            {
                if (seg.File == dma.File)
                {
                    //Check if it conflicts with an existing DMASegment.
                    // Method: Check 3 of the 4 start/end values if they're in between the start/end of
                    //          the other segment
                    if ((dma.FileStartOffset >= seg.FileStartOffset && dma.FileStartOffset <= seg.FileEndOffset) ||
                        (dma.FileEndOffset >= seg.FileStartOffset && dma.FileEndOffset <= seg.FileEndOffset) ||
                        (seg.FileStartOffset >= dma.FileStartOffset && seg.FileStartOffset <= dma.FileEndOffset))
                        return false;
                    if ((dma.RamStartOffset >= seg.RamStartOffset && dma.RamStartOffset <= seg.RamEndOffset) ||
                        (dma.RamEndOffset >= seg.RamStartOffset && dma.RamEndOffset <= seg.RamEndOffset) ||
                        (seg.RamStartOffset >= dma.RamStartOffset && seg.RamStartOffset <= dma.RamEndOffset))
                        return false;
                }
            }

            RamSegments[segment].Add(dma);

            return true;
        }

        public XElement GetAsXML()
        {
            XElement xml = new XElement(DMAPROFILE);

            xml.Add(new XAttribute(PROFILENAME, ProfileName));
            xml.Add(new XAttribute(TAGINFO, TagInfo));

            foreach (byte segment in RamSegments.Keys)
            {
                XElement seg = new XElement(SEGMENT);
                seg.Add(new XAttribute(SEGMENTNUM, segment));
                foreach(DmaSegment dmaSeg in RamSegments[segment])
                    xml.Add(dmaSeg.GetAsXML());
            }

            return xml;
        }

        public TreeNode GetAsTreeNode()
        {
            TreeNode node = new TreeNode();
            node.Tag = this;
            node.Text = this.ProfileName;

            //To do: finish this
            foreach (byte seg in RamSegments.Keys)
            {
                TreeNode segNode = new TreeNode();
                segNode.Tag = RamSegments[seg];
                segNode.Text = string.Format("{0:X8}", seg);

                node.Nodes.Add(segNode);
            }

            return node;
        }
    }

    public struct DmaSegment : IXMLSerializable
    {
        private const string DMASEGMENT = "DmaSegment";
        private const string FILEID = "FileId";
        private const string FILESTARTOFFSET = "FileStartOffset";
        private const string FILEENDOFFSET = "FileEndOffset";
        private const string RAMSTARTOFFSET = "RamStartOffset";
        private const string TAGINFO = "TagInfo";

        public RomFile File;
        public int FileStartOffset;
        public int FileEndOffset; //Exclusive
        public int RamStartOffset;
        public string TagInfo;

        public int RamEndOffset //Exclusive
        { get { return RamStartOffset + FileEndOffset - FileStartOffset; } }

        public XElement GetAsXML()
        {
            XElement xml = new XElement(DMASEGMENT);

            xml.Add(new XAttribute(FILEID, File.FileID));
            xml.Add(new XAttribute(FILESTARTOFFSET, FileStartOffset));
            xml.Add(new XAttribute(FILEENDOFFSET, FileEndOffset));
            xml.Add(new XAttribute(RAMSTARTOFFSET, RamStartOffset));
            xml.Add(new XAttribute(TAGINFO, TagInfo));

            return xml;
        }
    }
}
