using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Cereal64.Common
{
    public class DMAManager
    {
        private static DMAManager _instance = null;
        private static object syncObject = new object();

        public static DMAManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (syncObject)
                    {
                        if (_instance == null)
                            _instance = new DMAManager();
                    }
                }
                return _instance;
            }
        }

        public ReadOnlyCollection<DMAProfile> DMAProfiles { get { return _dmaProfiles.AsReadOnly(); } }
        private List<DMAProfile> _dmaProfiles;

        public DMAProfile SelectedDMAProfile;

        private DMAManager()
        {
            _dmaProfiles = new List<DMAProfile>();
        }

        public void AddNewDMAProfile(DMAProfile profile)
        {
            if(!_dmaProfiles.Any(dma => dma.ProfileName == profile.ProfileName))
            {
                _dmaProfiles.Add(profile);
            }
        }

        public void RemoveDMAProfile(string profileName)
        {
            DMAProfile profile = _dmaProfiles.First(dma => dma.ProfileName == profileName);

            if (profile != null)
            {
                _dmaProfiles.Remove(profile);
            }
        }

        public void ClearDMAProfilers()
        {
            _dmaProfiles.Clear();
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

        public bool RamOffsetToFileOffset(int ramAddress, out RomFile file, out int fileOffset)
        {
            file = null;
            fileOffset = -1;

            byte ramSegment = (byte)(ramAddress >> 24);
            int ramOffset = ramAddress & 0x00FFFFFF;

            //If the RAMsegments doesn't contain the segment number, return null
            if (!SelectedDMAProfile.RAMSegments.ContainsKey(ramSegment))
                return false;

            foreach (DMASegment dmaSegment in SelectedDMAProfile.RAMSegments[ramSegment])
            {
                //If it's outside the range of the RAMSegment, continue
                if (ramOffset < dmaSegment.RAMStartOffset || ramOffset >= dmaSegment.RAMEndOffset)
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

            foreach (List<DMASegment> dma in SelectedDMAProfile.RAMSegments.Values)
            {
                foreach (DMASegment dmaSegment in dma)
                {
                    if (dmaSegment.File == file)
                    {
                        if (fileOffset < dmaSegment.FileStartOffset || fileOffset >= dmaSegment.FileEndOffset)
                            continue;

                        //Success
                        ramOffset = (fileOffset - dmaSegment.FileStartOffset) + dmaSegment.RAMStartOffset;
                        return true;
                    }
                }
            }

            return false;
        }

    }

    public class DMAProfile
    {
        public Dictionary<byte, List<DMASegment>> RAMSegments;
        public string ProfileName;

        public DMAProfile(string name)
        {
            ProfileName = name;
            RAMSegments = new Dictionary<byte, List<DMASegment>>();
        }

        public bool AddDMASegment(byte segment, DMASegment dma)
        {
            if (dma.FileStartOffset >= dma.FileEndOffset)
                return false;
            if (dma.RAMStartOffset >= dma.RAMEndOffset)
                return false;

            if (!RAMSegments.Keys.Contains(segment))
                RAMSegments.Add(segment, new List<DMASegment>());

            foreach(DMASegment seg in RAMSegments[segment])
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
                    if ((dma.RAMStartOffset >= seg.RAMStartOffset && dma.RAMStartOffset <= seg.RAMEndOffset) ||
                        (dma.RAMEndOffset >= seg.RAMStartOffset && dma.RAMEndOffset <= seg.RAMEndOffset) ||
                        (seg.RAMStartOffset >= dma.RAMStartOffset && seg.RAMStartOffset <= dma.RAMEndOffset))
                        return false;
                }
            }

            RAMSegments[segment].Add(dma);

            return true;
        }
    }

    public struct DMASegment
    {
        public RomFile File;
        public int FileStartOffset;
        public int FileEndOffset; //Exclusive
        public int RAMStartOffset;

        public int RAMEndOffset //Exclusive
        { get { return RAMStartOffset + FileEndOffset - FileStartOffset; } }
    }
}
