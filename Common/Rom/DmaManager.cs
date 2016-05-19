using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Cereal64.Common.Rom
{
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

        public DmaProfile(XElement xml)
        {
            XAttribute att = xml.Attribute(PROFILENAME);
            if (att != null)
                ProfileName = att.Value;

            att = xml.Attribute(TAGINFO);
            if (att != null)
                TagInfo = att.Value;

            RamSegments = new Dictionary<byte, List<DmaSegment>>();

            foreach (XElement segment in xml.Elements())
            {
                if (segment.Name == SEGMENT)
                {
                    byte segNum = byte.Parse(segment.Attribute(SEGMENTNUM).Value);

                    if(!RamSegments.ContainsKey(segNum))
                        RamSegments.Add(segNum, new List<DmaSegment>());

                    foreach (XElement dmaXml in segment.Elements())
                    {
                        int id = int.Parse(dmaXml.Attribute(DmaSegment.FILEID).Value);
                        RomFile file = RomProject.Instance.Files.FirstOrDefault(f => f.FileID == id);
                        if(file != null)
                            RamSegments[segNum].Add(new DmaSegment(file, dmaXml));
                    }
                }
            }
        }

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
                    seg.Add(dmaSeg.GetAsXML());
                xml.Add(seg);
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
        public const string FILEID = "FileId";
        private const string FILESTARTOFFSET = "FileStartOffset";
        private const string FILEENDOFFSET = "FileEndOffset";
        private const string RAMSTARTOFFSET = "RamStartOffset";
        private const string RAMSEGMENT = "RamSegment";
        private const string TAGINFO = "TagInfo";

        public RomFile File;
        public int FileStartOffset;
        public int FileEndOffset; //Exclusive
        public int RamStartOffset;
        public byte RamSegment; //Duplicate data, but I think it's important
        public string TagInfo;

        public int RamEndOffset //Exclusive
        { get { return RamStartOffset + FileEndOffset - FileStartOffset; } }

        public DmaSegment(RomFile file, XElement xml)
        {
            File = file;
            FileStartOffset = int.Parse(xml.Attribute(FILESTARTOFFSET).Value);
            FileEndOffset = int.Parse(xml.Attribute(FILEENDOFFSET).Value);
            RamStartOffset = int.Parse(xml.Attribute(RAMSTARTOFFSET).Value);
            RamSegment = byte.Parse(xml.Attribute(RAMSEGMENT).Value);
            TagInfo = xml.Attribute(TAGINFO).Value;
        }
        
        public XElement GetAsXML()
        {
            XElement xml = new XElement(DMASEGMENT);

            xml.Add(new XAttribute(FILEID, File.FileID));
            xml.Add(new XAttribute(FILESTARTOFFSET, FileStartOffset));
            xml.Add(new XAttribute(FILEENDOFFSET, FileEndOffset));
            xml.Add(new XAttribute(RAMSTARTOFFSET, RamStartOffset));
            xml.Add(new XAttribute(RAMSEGMENT, RamSegment));
            xml.Add(new XAttribute(TAGINFO, TagInfo));

            return xml;
        }
    }
}
