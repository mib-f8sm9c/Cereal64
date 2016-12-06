using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common.DataElements;
using System.ComponentModel;
using Cereal64.Common.Utils;
using Cereal64.Common.Utils.Encoding;
using System.Xml.Linq;

namespace Cereal64.Common.DataElements.Encoding
{
    //For now, lets make MIO0Block not editable, you have to create a new one to make it work.
    // Also of note, it will use the same constructor for encoding raw data & decoding compressed data
    public class MIO0Block : NestedN64DataElement
    {
        [CategoryAttribute("MIO Settings"),
        DescriptionAttribute("Header for the MIO0 block")]
        public MIO0.MIO0Header Header { get; private set; }

        [BrowsableAttribute(false)]
        public byte[] LayoutBytes { get; private set; }

        [BrowsableAttribute(false)]
        public byte[] CompressedBytes { get; private set; }

        [BrowsableAttribute(false)]
        public byte[] UncompressedBytes { get; private set; }

        [BrowsableAttribute(false)]
        public byte[] DecodedData { get; private set; }
        //To do: when DecodedData changes, allow it to set a _changed flag, and recalculate on the next runthrough?

        public MIO0Block(int offset, byte[] rawData)
            : base(offset, rawData)
        {
        }

        public MIO0Block(int offset, MIO0.MIO0Header header, byte[] layout, byte[] compressed, byte[] uncompressed)
            : base(offset, null)
        {
            Header = header;
            LayoutBytes = layout;
            CompressedBytes = compressed;
            UncompressedBytes = uncompressed;

            DecodedData = MIO0.Decode(RawData);
            SetUpInternalData(DecodedData);
        }

        public MIO0Block(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
        }

        protected override byte[] InternalData
        {
            get
            {
                return DecodedData;
            }
        }

        protected override void SetUpInternalData(byte[] rawData)
        {
            if (rawData == null)
                return;

            //Decode
            DecodedData = MIO0.Decode(rawData);

            InitializeNewInternalData();
        }

        public static MIO0Block ReadMIO0BlockFrom(byte[] data, int offset, int length = -1)
        {
            if(length == -1)
                length = MIO0.FindLengthOfMIO0Block(data, offset);
            byte[] mio0Data = new byte[length];
            Array.Copy(data, offset, mio0Data, 0, length);

            return new MIO0Block(offset, mio0Data);
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes(Header.GetAsBytes(), LayoutBytes, CompressedBytes, UncompressedBytes);
            }
            set
            {
                if (value == null)
                    return;

                //Determine if we're being passed MIO0 data to decode or raw data to encode
                uint firstVal = ByteHelper.ReadUInt(value, 0);
                if (firstVal == MIO0.MIO0_AS_UINT) //MIO0 header, decode
                {
                    MIO0.MIO0Header header = new MIO0.MIO0Header(value, 0);
                    Header = header;

                    //Layout
                    LayoutBytes = new byte[header.CompLoc - MIO0.MIO0Header.Size];
                    Array.Copy(value, MIO0.MIO0Header.Size, LayoutBytes, 0, LayoutBytes.Length);

                    //Compressed
                    CompressedBytes = new byte[header.RawLoc - header.CompLoc];
                    Array.Copy(value, header.CompLoc, CompressedBytes, 0, CompressedBytes.Length);

                    //Uncompressed
                    UncompressedBytes = new byte[value.Length - header.RawLoc];
                    Array.Copy(value, header.RawLoc, UncompressedBytes, 0, UncompressedBytes.Length);

                    if (DecodedData == null)
                    {
                        //Decode it
                        DecodedData = MIO0.Decode(value);
                    }
                }
                else //Raw data, encode
                {
                    DecodedData = value;
                    RawData = MIO0.Encode(value);
                }
            }
        }
        
        public override int RawDataSize
        {
            get { return MIO0.MIO0Header.Size + LayoutBytes.Length + CompressedBytes.Length + UncompressedBytes.Length; }
        }

    }
}
