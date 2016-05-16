using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common.Utils.Encoding
{
    public static class MIO0
    {
        // MIO0 decompression code by HyperHacker (adapted from SF64Toolkit)
        public struct MIO0Header
        {
            public uint ID;			// always "MIO0"
            public uint OutputSize;	// decompressed data size
            public uint CompLoc;		// compressed data loc
            public uint RawLoc;		// uncompressed data loc

            public bool WriteToBytes(byte[] destination, int offset)
            {
                if (offset < 0 || offset + 0x10 > destination.Length)
                    return false;

                ByteHelper.WriteUInt(ID, destination, offset);
                ByteHelper.WriteUInt(OutputSize, destination, offset + 0x4);
                ByteHelper.WriteUInt(CompLoc, destination, offset + 0x8);
                ByteHelper.WriteUInt(RawLoc, destination, offset + 0xC);

                return true;
            }
        }

        public const uint MIO0_AS_UINT = 0x4D494F30;

        public static byte[] Encode(byte[] rawData)
        {
            throw new NotImplementedException();
        }

        public static byte[] EncodeAsRaw(byte[] rawData)
        {
            byte[] finalOutput;

            MIO0Header header;
            header.ID = MIO0_AS_UINT;
            header.OutputSize = (uint)rawData.Length;
            
            int layoutBytes = rawData.Length / 8;
            if(rawData.Length % 8 != 0)
                layoutBytes++;
            if (layoutBytes % 4 != 0)
                layoutBytes += 4 - (layoutBytes % 4);

            int startRawPos = 0x10 + layoutBytes;

            int totalLength = startRawPos + rawData.Length;
            if(totalLength % 16 != 0)
                totalLength += 16 - (totalLength % 16);

            header.CompLoc = (uint)startRawPos;
            header.RawLoc = (uint)startRawPos;

            //The idea here will be to have X bits filled with 1 after the header, where
            // X is the number of bytes in raw data. Then pad to a 4-byte location (0x0,
            // 0x4, 0x8 or 0xC) and start the raw data. Then, after the raw data, pad
            // it out to a 16-byte location (0x0).

            finalOutput = new byte[totalLength];

            header.WriteToBytes(finalOutput, 0);

            for (int i = 0; i < layoutBytes; i++)
            {
                finalOutput[i + 0x10] = 0xFF;
            }

            Array.Copy(rawData, 0, finalOutput, startRawPos, rawData.Length);

            return finalOutput;
        }

        public static byte[] Decode(byte[] encodedData)
        {
            MIO0Header Header;
            byte MapByte = 0x80, CurMapByte, Length;
            ushort SData, Dist;
            uint NumBytesOutput = 0;
            uint MapLoc = 0;	// current compression map position
            uint CompLoc = 0;	// current compressed data position
            uint RawLoc = 0;	// current raw data position
            uint OutLoc = 0;	// current output position

            int i;

            Header.ID = ByteHelper.ReadUInt(encodedData, 0);
            Header.OutputSize = ByteHelper.ReadUInt(encodedData, 4);
            Header.CompLoc = ByteHelper.ReadUInt(encodedData, 8);
            Header.RawLoc = ByteHelper.ReadUInt(encodedData, 12);

            // "MIO0"
            if (Header.ID != 0x4D494F30)
            {
                return null;
            }

            byte[] MIO0Buffer = new byte[Header.OutputSize];

            MapLoc = 0x10;
            CompLoc = Header.CompLoc;
            RawLoc = Header.RawLoc;

            CurMapByte = encodedData[MapLoc];

            while (NumBytesOutput < Header.OutputSize)
            {
                // raw
                if ((CurMapByte & MapByte) != 0x0)
                {
                    MIO0Buffer[OutLoc] = encodedData[RawLoc]; // copy a byte to output.
                    OutLoc++;
                    RawLoc++;
                    NumBytesOutput++;
                }

                // compressed
                else
                {
                    SData = ByteHelper.ReadUShort(encodedData, CompLoc); // get compressed data
                    Length = (byte)((SData >> 12) + 3);
                    Dist = (ushort)((SData & 0xFFF) + 1);

                    // sanity check: can't copy from before first byte
                    if (((int)OutLoc - Dist) < 0)
                    {
                        return null;
                    }

                    // copy from output
                    for (i = 0; i < Length; i++)
                    {
                        MIO0Buffer[OutLoc] = MIO0Buffer[OutLoc - Dist];
                        OutLoc++;
                        NumBytesOutput++;
                        if (NumBytesOutput >= Header.OutputSize) break;
                    }
                    CompLoc += 2;
                }

                MapByte >>= 1; // next map bit

                // if we did them all, get the next map byte
                if (MapByte == 0x0)
                {
                    MapByte = 0x80;
                    MapLoc++;
                    CurMapByte = encodedData[MapLoc];

                    // sanity check: map pointer should never reach this
                    int Check = (int)CompLoc;
                    if (RawLoc < CompLoc) Check = (int)RawLoc;

                    if (MapLoc > Check)
                    {
                        return null;
                    }
                }
            }

            return MIO0Buffer;
        }
    }
}
