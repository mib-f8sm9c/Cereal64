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

        public void AddNewDMAProfile(string profileName)
        {
            if(!_dmaProfiles.Any(dma => dma.ProfileName == profileName))
            {
                _dmaProfiles.Add(new DMAProfile(profileName));
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

        public bool FindDMAAddress(byte segment, int offset, out RomFile file, out int fileOffset)
        {
            file = null;
            fileOffset = 0;

            if (!SelectedDMAProfile.RAMSegments.ContainsKey(segment))
                return false;

            DMASegment dma = SelectedDMAProfile.RAMSegments[segment];

            file = dma.File;
            fileOffset = offset - dma.StartOffset;

            return dma.EndOffset > offset && offset >= dma.StartOffset; //double check that the offset falls within the range of the dma segment
        }

        /// <summary>
        /// Applies the DMA segments to the N64Addresses in all N64DataElements
        /// </summary>
        /// <param name="profile">The DMA profile to use as reference</param>
        public void ApplyProfile(DMAProfile profile)
        {
            if (!_dmaProfiles.Contains(profile))
                return;

            for (int i = 0; i < RomProject.Instance.Files.Count; i++)
            {
                //profile.RAMSegments.Any(
            }

            SelectedDMAProfile = profile;
        }
    }

    public class DMAProfile
    {
        public Dictionary<byte, DMASegment> RAMSegments;
        public string ProfileName;

        public DMAProfile(string name)
        {
            ProfileName = name;
            RAMSegments = new Dictionary<byte, DMASegment>();
        }
    }

    public struct DMASegment
    {
        public RomFile File;
        public int StartOffset;
        public int EndOffset;
    }
}
