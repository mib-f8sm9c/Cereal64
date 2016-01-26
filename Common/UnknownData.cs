using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common.F3DZEX.DataElements
{
    public class UnknownData : N64DataElement
    {
        //Use a list to make adding/removing data easier
        private List<byte> _rawData;

        //Need this so it can split 
        private RomFile _parent;

        public UnknownData(RomFile parent, int index, byte[] rawData)
            : base(index, rawData)
        {
            _parent = parent;
        }

        public override byte[] RawData
        {
            get
            {
                return _rawData.ToArray();
            }
            set
            {
                _rawData = value.ToList();
            }
        }

        public override int RawDataSize { get { return _rawData.Count; } }

        //public byte[] RemoveData(int offset, int length)
        //{
        //    if(offset < this.Address.Offset || offset > _rawData.
        //}
    }
}
