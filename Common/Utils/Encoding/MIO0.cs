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

            public MIO0Header(byte[] data, int offset)
            {
                ID = ByteHelper.ReadUInt(data, offset);
                OutputSize = ByteHelper.ReadUInt(data, offset + 0x4);
                CompLoc = ByteHelper.ReadUInt(data, offset + 0x8);
                RawLoc = ByteHelper.ReadUInt(data, offset + 0xC);
            }

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

            public byte[] GetAsBytes()
            {
                return ByteHelper.CombineIntoBytes(ID, OutputSize, CompLoc, RawLoc);
            }

            public static int Size { get { return 0x10; } }
        }

        public const uint MIO0_AS_UINT = 0x4D494F30;

        public static byte[] Encode(byte[] rawData, byte padToAddress = 4)
        {
            //To do: describe this algorithm
            // for now, just see here: http://wiki.origami64.net/sm64:mio0
            int minByteClump = 3;
            int maxByteClump = 18;
            int maxReadBack = 4096;

            ClumpInfo currentBestClump;

            List<ClumpInfo> clumpInfo = new List<ClumpInfo>();

            for (int offset = 1; offset < rawData.Length; )
            {
                currentBestClump = FindBestClump(rawData, offset, maxReadBack, maxByteClump, minByteClump);

                if (currentBestClump.Length >= minByteClump)
                {
                    //Set the clump, set back x bytes
                    clumpInfo.Add(currentBestClump);
                    offset += currentBestClump.Length;
                }
                else
                {
                    //Uncompressed, move on one byte
                    offset++;
                }
            }

            if (clumpInfo.Count == 0)
            {
                //Call the other one, huge waste of time
                return EncodeAsRaw(rawData);
            }

            //Optimization code
            for (int i = 0; i < clumpInfo.Count; i++)
            {
                if(FindBestAlternativeClumps(rawData, maxReadBack, maxByteClump, minByteClump,
                    clumpInfo, i))
                {
                    //to try to further optimize?
                }

            }

            ClumpInfo nextCompressedSection = clumpInfo[0];
            clumpInfo.RemoveAt(0);
            List<byte> UncompressedInfo = new List<byte>();
            List<byte> CompressedInfo = new List<byte>();
            List<byte> LayoutInfo = new List<byte>();
            byte layoutByte = 0;
            int layoutByteIndex = 7;

            for (int offset = 0; offset < rawData.Length; )
            {
                //If it's the offset from the next compressed section, do the compression. Else add an uncompressed.
                if (nextCompressedSection != null && offset == nextCompressedSection.Offset)
                {
                    //Write the compressed data
                    short compressedValues = (short)((((nextCompressedSection.Length - 3) & 0xF) << 12) | ((nextCompressedSection.CopyOffset - 1) & 0xFFF));
                    CompressedInfo.Add((byte)(compressedValues >> 8));
                    CompressedInfo.Add((byte)compressedValues);

                    //No writing to the byte, since it's a 0

                    offset += nextCompressedSection.Length;
                    if (clumpInfo.Count == 0)
                        nextCompressedSection = null;
                    else
                    {
                        nextCompressedSection = clumpInfo[0];
                        clumpInfo.RemoveAt(0);
                    }
                }
                else
                {
                    UncompressedInfo.Add(rawData[offset]);
                    layoutByte |= (byte)(1 << layoutByteIndex);
                    offset++;
                }

                layoutByteIndex--;
                if (layoutByteIndex < 0)
                {
                    LayoutInfo.Add(layoutByte);
                    layoutByte = 0;
                    layoutByteIndex = 7;
                }
            }

            //Add the proper end buffers to the lists, put together the header, throw it all to the user here
            if (layoutByteIndex != 7)
                LayoutInfo.Add(layoutByte);

            while (LayoutInfo.Count % 4 != 0)
                LayoutInfo.Add(0);

            MIO0Header header = new MIO0Header();
            header.ID = MIO0.MIO0_AS_UINT;
            header.OutputSize = (uint)rawData.Length;
            header.CompLoc = 0x10 + (uint)LayoutInfo.Count;
            header.RawLoc = header.CompLoc + (uint)CompressedInfo.Count;
            uint fullLength = header.RawLoc + (uint)UncompressedInfo.Count;

            //Pad if needed to hit a certain address value
            if (padToAddress != 0 && fullLength % padToAddress != 0)
                fullLength += padToAddress - (fullLength % padToAddress);

            byte[] finalData = new byte[fullLength];
            header.WriteToBytes(finalData, 0);
            Array.Copy(LayoutInfo.ToArray(), 0, finalData, 0x10, LayoutInfo.Count);
            Array.Copy(CompressedInfo.ToArray(), 0, finalData, header.CompLoc, CompressedInfo.Count);
            Array.Copy(UncompressedInfo.ToArray(), 0, finalData, header.RawLoc, UncompressedInfo.Count);

            return finalData;
        }

        private static ClumpInfo FindBestClump(byte[] rawData, int offset, int maxReadBack, int maxByteClump, int minByteClump)
        {
            ClumpInfo currentBestClump = new ClumpInfo(-1, -1, -1);

            int startRef = Math.Max(offset - maxReadBack, 0);

            for (int refOffset = startRef; refOffset < offset && refOffset < rawData.Length; refOffset++)
            {
                int matchCount = 0;
                while (matchCount < maxByteClump && offset + matchCount < rawData.Length && rawData[refOffset + matchCount] == rawData[offset + matchCount])
                    matchCount++;

                if (matchCount >= minByteClump && matchCount > currentBestClump.Length)
                {
                    currentBestClump = new ClumpInfo(offset, offset - refOffset, matchCount);
                    if (matchCount == maxByteClump)
                        break;
                }
            }

            return currentBestClump;
        }

        private static  bool FindBestAlternativeClumps(byte[] rawData, int maxReadBack, int maxByteClump, int minByteClump,
            List<ClumpInfo> origClumps, int clumpOffset)
        {
            //The idea is to shave off a byte at the beginning of the clump. If it improves the # of bytes used, then goodie goodie!

            //To do: look into shaving the end as well!
            int oldLayoutBitCount = 1;
            int oldByteCount = 2;
            bool isInOldClump = true;
            int oldClumpOffset = clumpOffset;

            int newLayoutBitCount = 1;
            int newByteCount = 1;
            bool isInNewClump = false;
            int newClumpOffset = 0;

            List<ClumpInfo> newClumps = new List<ClumpInfo>();
            
            int byteOffset = origClumps[clumpOffset].Offset + 1;

            while (byteOffset < rawData.Length)
            {
                if ((!isInNewClump || newClumps[newClumpOffset].FollowingByte == byteOffset) &&
                    (!isInOldClump || origClumps[oldClumpOffset].FollowingByte == byteOffset))
                {
                    break; //We are synced again, make sure to increment the oldClumpOffset so you don't overwrite a clump
                }

                //If we're not in a clump, try to generate a clump. If successful, add it in and jump in.
                if (isInNewClump)
                {
                    //Check if we need to exit
                    if (newClumps[newClumpOffset].FollowingByte == byteOffset)
                    {
                        newClumpOffset++;
                        //Try to generate new clump
                        ClumpInfo newClump = FindBestClump(rawData, byteOffset, maxReadBack, maxByteClump, minByteClump);
                        if (newClump.Offset != -1)
                        {
                            //jump into new clump
                            newClumps.Add(newClump);
                            newLayoutBitCount++;
                            newByteCount += 2;
                        }
                        else
                        {
                            isInNewClump = false;
                            newLayoutBitCount++;
                            newByteCount++;
                        }
                    }
                }
                else
                {
                    //Try to generate.
                    ClumpInfo newClump = FindBestClump(rawData, byteOffset, maxReadBack, maxByteClump, minByteClump);
                    if (newClump.Offset != -1)
                    {
                        //jump into new clump
                        newClumps.Add(newClump);
                        isInNewClump = true;
                        newLayoutBitCount++;
                        newByteCount += 2;
                    }
                    else
                    {
                        newLayoutBitCount++;
                        newByteCount++;
                    }
                }

                //Handle the old info
                if (isInOldClump)
                {
                    //Check if we need to exit
                    if (origClumps[oldClumpOffset].FollowingByte == byteOffset)
                    {
                        oldClumpOffset++;
                        if (oldClumpOffset < origClumps.Count && origClumps[oldClumpOffset].Offset == byteOffset)
                        {
                            //jump into new clump
                            oldLayoutBitCount++;
                            oldByteCount += 2;
                        }
                        else
                        {
                            isInOldClump = false;
                            oldLayoutBitCount++;
                            oldByteCount++;
                        }
                    }
                }
                else
                {
                    //Check if we enter
                    if (oldClumpOffset < origClumps.Count && origClumps[oldClumpOffset].Offset == byteOffset)
                    {
                        isInOldClump = true;
                        oldLayoutBitCount++;
                        oldByteCount += 2;
                    }
                    else
                    {
                        oldLayoutBitCount++;
                        oldByteCount++;
                    }
                }

                //if (isInNewClump && isInOldClump && origClumps[oldClumpOffset].LastByte == newClumps[newClumpOffset].LastByte)
                //    break; //We are synced again

                //if (!isInOldClump && !isInNewClump)
                //    break; //We are synced again

                    byteOffset++;
            }

            if (newByteCount > oldByteCount || (newByteCount == oldByteCount && newLayoutBitCount >= oldLayoutBitCount))
                return false;

            //make sure to go back an index if we're not in a clump
            if(!isInOldClump)
                oldClumpOffset--;

            //Apply
            int j;
            for (j = oldClumpOffset; j > clumpOffset; j--)
            {
                origClumps.RemoveAt(j);
            }
            origClumps.RemoveAt(j);

            while (newClumps.Count > 0)
            {
                origClumps.Insert(j, newClumps.Last());
                newClumps.RemoveAt(newClumps.Count - 1);
            }

            return true;

            //Idea: have an algorithm that'll take a given clump, bump one byte off the start and step through bytes, incrementing
            // the old and new clumps until they match up again. Remember to try to create clumps before ending the algorithm. If
            // you can't drop it, consider recursively running the alogritm over it again, allowing for one more extra raw byte to be
            // injected somewhere. If that doesn't work, failure!
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

        //To do: remove the debug code?
        public static byte[] Decode(byte[] encodedData, string fileName = null, bool verify = false)
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

            System.IO.StreamWriter writer = null;
            if (fileName != null)
                writer = new System.IO.StreamWriter(fileName);

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
                    if (writer != null)
                        writer.Write("{0:X2}", encodedData[RawLoc]);
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

                    if (writer != null)
                        writer.Write("[");

                    // copy from output
                    for (i = 0; i < Length; i++)
                    {
                        MIO0Buffer[OutLoc] = MIO0Buffer[OutLoc - Dist];
                        if (writer != null)
                            writer.Write("{0:X2}", MIO0Buffer[OutLoc]);
                        OutLoc++;
                        NumBytesOutput++;
                        if (NumBytesOutput >= Header.OutputSize) break;
                    }

                    if (writer != null)
                        writer.Write("]");
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

            if (writer != null)
                writer.Close();

            if (verify)
            {
                byte[] recoded = Encode(MIO0Buffer);

                if (recoded.Length > encodedData.Length)
                {
                    System.IO.File.WriteAllBytes("BadMIOData.bin",encodedData);
                    System.IO.File.WriteAllBytes("Fixed.bin", recoded);

                    Decode(encodedData, "diff1.txt");
                    Decode(recoded, "diff2.txt");

                    throw new Exception();
                }

                byte[] redecoded = Decode(recoded);

                if (redecoded.Length != MIO0Buffer.Length)
                    throw new Exception();

                for (int k = 0; k < redecoded.Length; k++)
                {
                    if (redecoded[k] != MIO0Buffer[k])
                        throw new Exception();
                }


            }

            return MIO0Buffer;
        }

        public static int FindLengthOfMIO0Block(byte[] data, int offsetOfMIO0)
        {
            //Load the header info
            MIO0Header header = new MIO0Header(data, offsetOfMIO0);

            //Count all the uncompressed bytes
            int uncompressedCount = 0;
            for (int i = 0x10; i < header.CompLoc; i++)
            {
                for (int j = 7; j >= 0; j--)
                {
                    //Check for one
                    if ((data[i + offsetOfMIO0] & (byte)(1 << j)) != 0)
                    {
                        uncompressedCount++;
                    }
                }
            }

            //Make it line up to 4-byte address
            int length = uncompressedCount + (int)header.RawLoc;
            if (length % 4 != 0)
                length += 4 - (length % 4);

            return length;
        }

        internal class ClumpInfo
        {
            public int Offset;
            public int CopyOffset;
            public int Length;

            public ClumpInfo(int offset, int copyOffset, int length)
            {
                Offset = offset;
                CopyOffset = copyOffset;
                Length = length;
            }

            public int FollowingByte
            { get { return Offset + Length; } }

            public int LastByte
            { get { return FollowingByte - 1; } }
        }
    }
}
