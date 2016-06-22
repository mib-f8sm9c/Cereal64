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

        public static byte[] Encode(byte[] rawData)
        {
            //Pretty simple idea here, though it will be slow: basically filter backwards through the data, trying to find the best
            // matches for the current string of bytes (3-18 long). Match from the rear first, to try to allow for dynamic size testing.
            // Use the biggest match you can find. If you max out size early on, stop the current search early.

            //If a match is found, save the info to a compressed data info table(offset & length). Push on.

            //After you're done, you can go forwards and split the data into the compressed and uncompressed sections. Finalize it.

            int minByteClump = 3;
            int maxByteClump = 18;
            int maxReadBack = 4096;

            Tuple<int, int, int> currentBestClump;

            List<Tuple<int, int, int>> clumpInfo = new List<Tuple<int, int, int>>(); //output offset, input offset, length

            for (int offset = 1; offset < rawData.Length; )
            {
                currentBestClump = new Tuple<int, int, int>(-1, -1, -1);
                for (int refOffset = offset - 1; offset - refOffset <= maxReadBack && refOffset >= 0; refOffset--)
                {
                    int matchCount = 0;
                    while (matchCount < maxByteClump && offset + matchCount < rawData.Length && rawData[refOffset + matchCount] == rawData[offset + matchCount])
                        matchCount++;

                    if (matchCount >= minByteClump && matchCount > currentBestClump.Item3)
                    {
                        currentBestClump = new Tuple<int, int, int>(offset, offset - refOffset, matchCount);
                        if (matchCount == maxByteClump)
                            break;
                    }
                }

                if (currentBestClump.Item3 >= minByteClump)
                {
                    //Set the clump, set back x bytes
                    clumpInfo.Add(currentBestClump);
                    offset += currentBestClump.Item3;
                }
                else
                {
                    //Uncompressed, move on one byte
                    offset++;
                }
            }

            //THOUGHTS ON IMPROVING THE ALGORITHM: TRY A NORMAL AND A REVERSE PASS, THEN COMBINE THEM AS BEST YOU CAN?

            //Thoughts on combining: just measure the bytes along each section as you go down it, and when they match up in index,
            //       compare the two and take the one that matches best.

            if (clumpInfo.Count == 0)
            {
                //Call the other one, huge waste of time
                return EncodeAsRaw(rawData);
            }

            //OPTIMIZATION: SEE IF PARTING WITH THE FIRST BYTE OF A SMALLER CLUMP CAN MAKE IT EXPAND!
            for (int i = 0; i < clumpInfo.Count; i++)
            {
                //First test case: it's right next to a clump && combined are less than or equal to 19
                if (i < clumpInfo.Count - 1 && clumpInfo[i].Item3 + clumpInfo[i + 1].Item3 <= maxByteClump + 1 &&
                    clumpInfo[i].Item1 + clumpInfo[i].Item3 == clumpInfo[i + 1].Item1)
                {
                    //Test if removing the first byte combines the two clumps
                    int startIndex = clumpInfo[i].Item1 + 1;
                    int idealClumpLength = clumpInfo[i].Item3 + clumpInfo[i + 1].Item3 - 1;
                    for (int refOffset = startIndex - 1; startIndex - refOffset <= maxReadBack && refOffset >= 0; refOffset--)
                    {
                        int matchCount = 0;
                        while (matchCount < maxByteClump && startIndex + matchCount < rawData.Length && rawData[refOffset + matchCount] == rawData[startIndex + matchCount])
                            matchCount++;

                        if (matchCount == idealClumpLength)
                        {
                            clumpInfo[i] = new Tuple<int, int, int>(startIndex, startIndex - refOffset, matchCount);
                            clumpInfo.RemoveAt(i + 1);
                            break;
                        }
                    }
                }
                else if ((i == clumpInfo.Count - 1 && clumpInfo[i].Item1 + clumpInfo[i].Item2 < rawData.Length - 2) ||
                    (i < clumpInfo.Count - 1 && clumpInfo[i].Item1 + clumpInfo[i].Item2 < clumpInfo[i + 1].Item1))
                {
                    //Test if removing the first byte merges two or more of the following unclumped bytes into a single clump
                    //Test if removing the first byte combines the two clumps
                    int startIndex = clumpInfo[i].Item1 + 1;
                    int idealClumpLength;
                    if (i == clumpInfo.Count - 1)
                        idealClumpLength = rawData.Length - clumpInfo[i].Item1 - 1;
                    else
                        idealClumpLength = clumpInfo[i + 1].Item1 - clumpInfo[i].Item1 - 1;
                    for (int refOffset = startIndex - 1; startIndex - refOffset <= maxReadBack && refOffset >= 0; refOffset--)
                    {
                        int matchCount = 0;
                        while (matchCount < maxByteClump && startIndex + matchCount < rawData.Length && rawData[refOffset + matchCount] == rawData[startIndex + matchCount])
                            matchCount++;

                        if (matchCount > clumpInfo[i].Item3 && matchCount <= idealClumpLength)
                        {
                            clumpInfo[i] = new Tuple<int, int, int>(startIndex, startIndex - refOffset, matchCount);
                            clumpInfo.RemoveAt(i + 1);
                            break;
                        }
                    }
                }
            }

            Tuple<int, int, int> nextCompressedSection = clumpInfo[0];
            clumpInfo.RemoveAt(0);
            List<byte> UncompressedInfo = new List<byte>();
            List<byte> CompressedInfo = new List<byte>();
            List<byte> LayoutInfo = new List<byte>();
            byte layoutByte = 0;
            int layoutByteIndex = 7;

            for (int offset = 0; offset < rawData.Length; )
            {
                //If it's the offset from the next compressed section, do the compression. Else add an uncompressed.
                if (nextCompressedSection != null && offset == nextCompressedSection.Item1)
                {
                    //Write the compressed data
                    short compressedValues = (short)((((nextCompressedSection.Item3 - 3) & 0xF) << 12) | ((nextCompressedSection.Item2 - 1) & 0xFFF));
                    CompressedInfo.Add((byte)(compressedValues >> 8));
                    CompressedInfo.Add((byte)compressedValues);

                    //No writing to the byte, since it's a 0

                    offset += nextCompressedSection.Item3;
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
            if (fullLength % 4 != 0)
                fullLength += 4 - (fullLength % 4);

            byte[] finalData = new byte[fullLength];
            header.WriteToBytes(finalData, 0);
            Array.Copy(LayoutInfo.ToArray(), 0, finalData, 0x10, LayoutInfo.Count);
            Array.Copy(CompressedInfo.ToArray(), 0, finalData, header.CompLoc, CompressedInfo.Count);
            Array.Copy(UncompressedInfo.ToArray(), 0, finalData, header.RawLoc, UncompressedInfo.Count);

            return finalData;
        }

        public static byte[] EncodeDoublePass(byte[] rawData)
        {
            //Pretty simple idea here, though it will be slow: basically filter backwards through the data, trying to find the best
            // matches for the current string of bytes (3-18 long). Match from the rear first, to try to allow for dynamic size testing.
            // Use the biggest match you can find. If you max out size early on, stop the current search early.

            //If a match is found, save the info to a compressed data info table(offset & length). Push on.

            //After you're done, you can go forwards and split the data into the compressed and uncompressed sections. Finalize it.

            int minByteClump = 3;
            int maxByteClump = 18;
            int maxReadBack = 4096;

            Stack<Tuple<int, int, int>> reverseClumpInfo = new Stack<Tuple<int, int, int>>(); //output offset, input offset, length

            Tuple<int, int, int> currentBestClump;

            for (int offset = rawData.Length - 1; offset > 0; )
            {
                currentBestClump = new Tuple<int, int, int>(-1, -1, -1);
                for (int refOffset = offset - 1; offset - refOffset <= maxReadBack && refOffset >= 0; refOffset--)
                {
                    int matchCount = 0;
                    while (matchCount < maxByteClump && refOffset - matchCount >= 0 && rawData[refOffset - matchCount] == rawData[offset - matchCount])
                        matchCount++;

                    if (matchCount >= minByteClump && matchCount > currentBestClump.Item3)
                    {
                        currentBestClump = new Tuple<int, int, int>(offset - matchCount + 1, offset - refOffset, matchCount);
                        if (matchCount == maxByteClump)
                            break;
                    }
                }

                if (currentBestClump.Item3 >= minByteClump)
                {
                    //Set the clump, set back x bytes
                    reverseClumpInfo.Push(currentBestClump);
                    offset -= currentBestClump.Item3;
                }
                else
                {
                    //Uncompressed, move on one byte
                    offset--;
                }
            }

            Queue<Tuple<int, int, int>> forwardClumpInfo = new Queue<Tuple<int, int, int>>(); //output offset, input offset, length

            for (int offset = 1; offset < rawData.Length; )
            {
                currentBestClump = new Tuple<int, int, int>(-1, -1, -1);
                for (int refOffset = offset - 1; offset - refOffset <= maxReadBack && refOffset >= 0; refOffset--)
                {
                    int matchCount = 0;
                    while (matchCount < maxByteClump && offset + matchCount < rawData.Length && rawData[refOffset + matchCount] == rawData[offset + matchCount])
                        matchCount++;

                    if (matchCount >= minByteClump && matchCount > currentBestClump.Item3)
                    {
                        currentBestClump = new Tuple<int, int, int>(offset, offset - refOffset, matchCount);
                        if (matchCount == maxByteClump)
                            break;
                    }
                }

                if (currentBestClump.Item3 >= minByteClump)
                {
                    //Set the clump, set back x bytes
                    forwardClumpInfo.Enqueue(currentBestClump);
                    offset += currentBestClump.Item3;
                }
                else
                {
                    //Uncompressed, move on one byte
                    offset++;
                }
            }

            //THOUGHTS ON IMPROVING THE ALGORITHM: TRY A NORMAL AND A REVERSE PASS, THEN COMBINE THEM AS BEST YOU CAN?

            //Thoughts on combining: just measure the bytes along each section as you go down it, and when they match up in index,
            //       compare the two and take the one that matches best.

            List<Tuple<int, int, int>> combinedClumpInfo = new List<Tuple<int, int, int>>();

            Queue<Tuple<int, int, int>> builtUpReverseInfos = new Queue<Tuple<int, int, int>>();
            Queue<Tuple<int, int, int>> builtUpForwardInfos = new Queue<Tuple<int, int, int>>();

            int lastMatchingIndex = 0;
            int currentForwardIndex = 0;
            int currentReverseIndex = 0;
            int forwardBytes = 0;
            int reverseBytes = 0;
            for (int i = 1; i < rawData.Length; i++)
            {
                if (currentForwardIndex < i)
                {
                    //Move it forward
                    if (forwardClumpInfo.Peek().Item1 == i)
                    {
                        currentForwardIndex = forwardClumpInfo.Peek().Item1 + forwardClumpInfo.Peek().Item3 - 1;
                        forwardBytes += 2;
                        builtUpForwardInfos.Enqueue(forwardClumpInfo.Dequeue());
                    }
                    else
                    {
                        currentForwardIndex = i;
                        forwardBytes++;
                    }
                }

                if (currentReverseIndex < i)
                {
                    //Move it forward
                    if (reverseClumpInfo.Peek().Item1 == i)
                    {
                        currentReverseIndex = reverseClumpInfo.Peek().Item1 + reverseClumpInfo.Peek().Item3 - 1;
                        reverseBytes += 2;
                        builtUpReverseInfos.Enqueue(reverseClumpInfo.Pop());
                    }
                    else
                    {
                        currentReverseIndex = i;
                        reverseBytes++;
                    }
                }

                if (i == currentForwardIndex && i == currentReverseIndex)
                {
                    //Caught up matching!
                    if (false)//reverseBytes < forwardBytes)
                    {
                        while (builtUpReverseInfos.Count > 0)
                            combinedClumpInfo.Add(builtUpReverseInfos.Dequeue());

                        builtUpForwardInfos.Clear();
                    }
                    else
                    {
                        while (builtUpForwardInfos.Count > 0)
                            combinedClumpInfo.Add(builtUpForwardInfos.Dequeue());

                        builtUpReverseInfos.Clear();
                    }

                    forwardBytes = 0;
                    reverseBytes = 0;
                }
            }

            //reverseClumpInfo.Clear();
            //while (forwardClumpInfo.Count > 0)
            //    reverseClumpInfo.Push(forwardClumpInfo.Pop());
            //So now we should have the list all good and working. Now put everything in its proper place!

            if (combinedClumpInfo.Count == 0)
            {
                //Call the other one, huge waste of time
                return EncodeAsRaw(rawData);
            }

            //OPTIMIZATION: SEE IF PARTING WITH THE FIRST BYTE OF A SMALLER CLUMP CAN MAKE IT EXPAND!
            for (int i = 0; i < combinedClumpInfo.Count; i++)
            {
                //First test case: it's right next to a clump && combined are less than or equal to 19
                if (i < combinedClumpInfo.Count - 1 && combinedClumpInfo[i].Item3 + combinedClumpInfo[i + 1].Item3 <= maxByteClump + 1 &&
                    combinedClumpInfo[i].Item1 + combinedClumpInfo[i].Item3 == combinedClumpInfo[i + 1].Item1)
                {
                    //Test if removing the first byte combines the two clumps
                    int startIndex = combinedClumpInfo[i].Item1 + 1;
                    int idealClumpLength = combinedClumpInfo[i].Item3 + combinedClumpInfo[i + 1].Item3 - 1;
                    for (int refOffset = startIndex - 1; startIndex - refOffset <= maxReadBack && refOffset >= 0; refOffset--)
                    {
                        int matchCount = 0;
                        while (matchCount < maxByteClump && startIndex + matchCount < rawData.Length && rawData[refOffset + matchCount] == rawData[startIndex + matchCount])
                            matchCount++;

                        if (matchCount == idealClumpLength)
                        {
                            combinedClumpInfo[i] = new Tuple<int, int, int>(startIndex, startIndex - refOffset, matchCount);
                            combinedClumpInfo.RemoveAt(i + 1);
                            break;
                        }
                    }
                }
                else if ((i == combinedClumpInfo.Count - 1 && combinedClumpInfo[i].Item1 + combinedClumpInfo[i].Item2 < rawData.Length - 2) ||
                    (i < combinedClumpInfo.Count - 1 && combinedClumpInfo[i].Item1 + combinedClumpInfo[i].Item2 < combinedClumpInfo[i + 1].Item1))
                {
                    //Test if removing the first byte merges two or more of the following unclumped bytes into a single clump
                    //Test if removing the first byte combines the two clumps
                    int startIndex = combinedClumpInfo[i].Item1 + 1;
                    int idealClumpLength;
                    if (i == combinedClumpInfo.Count - 1)
                        idealClumpLength = rawData.Length - combinedClumpInfo[i].Item1 - 1;
                    else
                        idealClumpLength = combinedClumpInfo[i + 1].Item1 - combinedClumpInfo[i].Item1 - 1;
                    for (int refOffset = startIndex - 1; startIndex - refOffset <= maxReadBack && refOffset >= 0; refOffset--)
                    {
                        int matchCount = 0;
                        while (matchCount < maxByteClump && startIndex + matchCount < rawData.Length && rawData[refOffset + matchCount] == rawData[startIndex + matchCount])
                            matchCount++;

                        if (matchCount > combinedClumpInfo[i].Item3 && matchCount <= idealClumpLength)
                        {
                            combinedClumpInfo[i] = new Tuple<int, int, int>(startIndex, startIndex - refOffset, matchCount);
                            combinedClumpInfo.RemoveAt(i + 1);
                            break;
                        }
                    }
                }
            }


            Tuple<int, int, int> nextCompressedSection = combinedClumpInfo[0];
            combinedClumpInfo.RemoveAt(0);
            List<byte> UncompressedInfo = new List<byte>();
            List<byte> CompressedInfo = new List<byte>();
            List<byte> LayoutInfo = new List<byte>();
            byte layoutByte = 0;
            int layoutByteIndex = 7;

            for (int offset = 0; offset < rawData.Length; )
            {
                //If it's the offset from the next compressed section, do the compression. Else add an uncompressed.
                if (nextCompressedSection != null && offset == nextCompressedSection.Item1)
                {
                    //Write the compressed data
                    short compressedValues = (short)((((nextCompressedSection.Item3 - 3) & 0xF) << 12) | ((nextCompressedSection.Item2 - 1) & 0xFFF));
                    CompressedInfo.Add((byte)(compressedValues >> 8));
                    CompressedInfo.Add((byte)compressedValues);

                    //No writing to the byte, since it's a 0

                    offset += nextCompressedSection.Item3;
                    if (combinedClumpInfo.Count == 0)
                        nextCompressedSection = null;
                    else
                    {
                        nextCompressedSection = combinedClumpInfo[0];
                        combinedClumpInfo.RemoveAt(0);
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
            if (fullLength % 4 != 0)
                fullLength += 4 - (fullLength % 4);

            byte[] finalData = new byte[fullLength];
            header.WriteToBytes(finalData, 0);
            Array.Copy(LayoutInfo.ToArray(), 0, finalData, 0x10, LayoutInfo.Count);
            Array.Copy(CompressedInfo.ToArray(), 0, finalData, header.CompLoc, CompressedInfo.Count);
            Array.Copy(UncompressedInfo.ToArray(), 0, finalData, header.RawLoc, UncompressedInfo.Count);

            return finalData;
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
    }
}
