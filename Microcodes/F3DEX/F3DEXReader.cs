using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DEX.DataElements;
using Cereal64.Common.Utils;
using Cereal64.Microcodes.F3DEX.DataElements.Commands;
using Cereal64.Common.Rom;
using Cereal64.Common.DataElements;

namespace Cereal64.Microcodes.F3DEX
{
    //TO DO: REMOVE THIS, REPLACE WITH SOMETHING THAT ACTUALLY MAKES SENSE
    public class F3DEXReaderPackage
    {
        public Dictionary<RomFile, List<N64DataElement>> Elements = new Dictionary<RomFile,List<N64DataElement>>();
        //To consider: since this is going to be converted to VisObj64, think about how best to organize it so it will
        //              look good : )
    }

    public static class F3DEXReader
    {
        //Private variables to hold last-loaded settings (NECESSARY??)
        private static F3DEX_G_SetTile _lastSetTile;
        private static F3DEX_G_SetTImg _lastSetTImg;
        private static F3DEX_G_SetTileSize _lastSetTileSize;
        private static F3DEX_G_LoadBlock _lastLoadBlock;
        private static F3DEX_G_LoadTLut _lastLoadTLut;

        private static int DefaultTextureTile = 0;

        private static F3DEXImage _currentImage;

        private static TMem TMEM;

        private static Dictionary<RomFile, List<Palette>> _foundPalettes = new Dictionary<RomFile,List<Palette>>();
        private static Dictionary<RomFile, List<Texture>> _foundTextures = new Dictionary<RomFile,List<Texture>>();
        private static Dictionary<RomFile, List<Vertex>> _foundVertices = new Dictionary<RomFile,List<Vertex>>();
        private static Dictionary<RomFile, List<F3DEXCommand>> _foundCommands = new Dictionary<RomFile,List<F3DEXCommand>>();

        private static List<Vertex> _vertexBuffer = new List<Vertex>();

        public static void ParseF3DEXCommands(IList<F3DEXCommand> commands)
        {
            ResetReaderVariables();

            foreach (F3DEXCommand command in commands)
            {
                //ParseCommand(command);
            }
        }

        private static void ResetReaderVariables()
        {
            _foundCommands.Clear();
            _foundPalettes.Clear();
            _foundTextures.Clear();
            _foundVertices.Clear();

            _currentImage = null;

            _vertexBuffer.Clear();
            for (int i = 0; i < 32; i++)
                _vertexBuffer.Add(null);

            TMEM = new TMem();

            DefaultTextureTile = 0;

            _lastSetTile = null;
            _lastSetTImg = null;
            _lastSetTileSize = null;
            _lastLoadBlock = null;
            _lastLoadTLut = null;
        }

        #region OLD BROKEN READING CODE
        /*
        //public static 
        public static F3DEXReaderPackage ReadF3DEXAt(RomFile file, int offset)
        {
            ResetReaderVariables();
            
            byte[] data = file.GetAsBytes();

            F3DEXReaderPackage package = new F3DEXReaderPackage();

            if (offset < 0 || offset >= data.Length || offset % 8 != 0)
                return package;

            //Reset/Initialize what needs to be initialized/reset

            byte[] command = new byte[8];

            while (offset < data.Length)
            {
                //read the command
                Array.Copy(data, offset, command, 0, 8);

                F3DEXCommand f3Command = F3DEXCommandFactory.ReadCommand(offset, command);
                if (f3Command == null)
                    break;

                if (!_foundCommands.ContainsKey(file))
                    _foundCommands.Add(file, new List<F3DEXCommand>());

                _foundCommands[file].Add(f3Command);

                ParseCommand(f3Command);

                offset += 8;

                if (f3Command is F3DEX_G_EndDL)
                    break;
            }

            //Sort what needs to be sorted
            foreach(RomFile rom in _foundTextures.Keys)
            {
                _foundTextures[rom].Sort((s1, s2) => s1.FileOffset.CompareTo(s2.FileOffset));
                for (int i = 0; i < _foundTextures[rom].Count - 1; i++)
                {
                    if (_foundTextures[rom][i].FileOffset == _foundTextures[rom][i + 1].FileOffset)
                    {
                        _foundTextures[rom].RemoveAt(i);
                        i--;
                    }
                }
            }

            foreach (RomFile rom in _foundPalettes.Keys)
            {
                _foundPalettes[rom].Sort((s1, s2) => s1.FileOffset.CompareTo(s2.FileOffset));
                for (int i = 0; i < _foundPalettes[rom].Count - 1; i++)
                {
                    if (_foundPalettes[rom][i].FileOffset == _foundPalettes[rom][i + 1].FileOffset)
                    {
                        _foundPalettes[rom].RemoveAt(i);
                        i--;
                    }
                }
            }

            foreach (RomFile rom in _foundVertices.Keys)
            {
                _foundVertices[rom].Sort((s1, s2) => s1.FileOffset.CompareTo(s2.FileOffset));
                for (int i = 0; i < _foundVertices[rom].Count - 1; i++)
                {
                    if (_foundVertices[rom][i].FileOffset == _foundVertices[rom][i + 1].FileOffset)
                    {
                        _foundVertices[rom].RemoveAt(i);
                        i--;
                    }
                }
            }

            foreach (RomFile rom in _foundCommands.Keys)
            {
                _foundCommands[rom].Sort((s1, s2) => s1.FileOffset.CompareTo(s2.FileOffset));
                for (int i = 0; i < _foundCommands[rom].Count - 1; i++)
                {
                    if (_foundCommands[rom][i].FileOffset == _foundCommands[rom][i + 1].FileOffset)
                    {
                        _foundCommands[rom].RemoveAt(i);
                        i--;
                    }
                }
            }

            //Combine into the collections
            Dictionary<RomFile, List<F3DEXCommandCollection>> commandColls = new Dictionary<RomFile, List<F3DEXCommandCollection>>();
            Dictionary<RomFile, List<VertexCollection>> vertexColls = new Dictionary<RomFile, List<VertexCollection>>();

            //F3DEX Commands
            foreach (RomFile rom in _foundCommands.Keys)
            {
                commandColls.Add(rom, new List<F3DEXCommandCollection>());
                int startColl = 0;
                for (int endColl = 0; endColl < _foundCommands[rom].Count; endColl++)
                {
                    if (endColl == _foundCommands[rom].Count - 1 ||
                        _foundCommands[rom][endColl + 1].FileOffset != _foundCommands[rom][endColl].FileOffset + _foundCommands[rom][endColl].RawDataSize)
                    {
                        //Cut off the collection here
                        F3DEXCommandCollection newColl = new F3DEXCommandCollection(_foundCommands[rom][startColl].FileOffset,
                            _foundCommands[rom].GetRange(startColl, endColl - startColl + 1)); //NOTE: Shallow copying is done here

                        commandColls[rom].Add(newColl);

                        startColl = endColl + 1;
                    }
                }
            }

            //Vertices
            foreach (RomFile rom in _foundVertices.Keys)
            {
                vertexColls.Add(rom, new List<VertexCollection>());
                int startColl = 0;
                for (int endColl = 0; endColl < _foundVertices[rom].Count; endColl++)
                {
                    if (endColl == _foundVertices[rom].Count - 1 ||
                        _foundVertices[rom][endColl + 1].FileOffset != _foundVertices[rom][endColl].FileOffset + _foundVertices[rom][endColl].RawDataSize)
                    {
                        //Cut off the collection here
                        VertexCollection newVert = new VertexCollection(_foundVertices[rom][startColl].FileOffset,
                            _foundVertices[rom].GetRange(startColl, endColl - startColl + 1)); //NOTE: Shallow copying is done here

                        vertexColls[rom].Add(newVert);

                        startColl = endColl + 1;
                    }
                }
            }

            //double check that the package has all the files
            foreach (RomFile rom in _foundTextures.Keys.Union(_foundPalettes.Keys.Union(
                commandColls.Keys.Union(vertexColls.Keys))))
            {
                if (!package.Elements.ContainsKey(rom))
                    package.Elements.Add(rom, new List<N64DataElement>());
            }
            foreach (RomFile rom in _foundTextures.Keys)
                package.Elements[rom].AddRange(_foundTextures[rom]);
            foreach (RomFile rom in _foundPalettes.Keys)
                package.Elements[rom].AddRange(_foundPalettes[rom]);
            foreach (RomFile rom in commandColls.Keys)
                package.Elements[rom].AddRange(commandColls[rom]);
            foreach (RomFile rom in vertexColls.Keys)
                package.Elements[rom].AddRange(vertexColls[rom]);

            return package;
        }

        private static void ParseCommand(F3DEXCommand command)
        {
            TileDescriptor tile;
            //Here handle extra command things (like if it's referencing textures and such)
            switch (command.CommandID)
            {
                case F3DEXCommandID.F3DEX_G_DL:
                    //Need to put another dl on the stack and keep reading. That forceJump option if it's true should purge the stack.

                    break;
                case F3DEXCommandID.F3DEX_G_MK64_ENDDL:
                    //Need to end the current level. Pop off the next on the stack. If it's empty, then end for good : )

                    break;

                case F3DEXCommandID.F3DEX_G_TEXTURE:
                    F3DEX_G_Texture textureCmd = (F3DEX_G_Texture)command;
                    tile = TMEM.TileDescriptors[textureCmd.Tile];
                    tile.On = (textureCmd.TurnOn & 1) == 0;
                    tile.ScaleS = textureCmd.ScaleS;
                    tile.ScaleT = textureCmd.ScaleT;
                    tile.Level = textureCmd.Level;

                    break;
                case F3DEXCommandID.F3DEX_G_SETTILE:
                    _lastSetTile = (F3DEX_G_SetTile)command;

                    tile = TMEM.TileDescriptors[_lastSetTile.Tile];
                    tile.CMSMirror = _lastSetTile.CMSMirror;
                    tile.CMTMirror = _lastSetTile.CMTMirror;
                    tile.CMSWrap = _lastSetTile.CMSWrap;
                    tile.CMTWrap = _lastSetTile.CMTWrap;
                    tile.ImageFormat = _lastSetTile.Format;
                    tile.Line = _lastSetTile.Line;
                    tile.MaskS = _lastSetTile.MaskS;
                    tile.MaskT = _lastSetTile.MaskT;
                    tile.Palette = _lastSetTile.Palette;
                    tile.PixelSize = _lastSetTile.PixelSize;
                    tile.ShiftS = _lastSetTile.ShiftS;
                    tile.ShiftT = _lastSetTile.ShiftT;
                    tile.TMem = _lastSetTile.TMem;

                    break;
                case F3DEXCommandID.F3DEX_G_SETTILESIZE:
                    _lastSetTileSize = (F3DEX_G_SetTileSize)command;

                    tile = TMEM.TileDescriptors[_lastSetTileSize.Tile];
                    tile.LRS = _lastSetTileSize.LRS;
                    tile.LRT = _lastSetTileSize.LRT;
                    tile.ULS = _lastSetTileSize.ULS;
                    tile.ULT = _lastSetTileSize.ULT;

                    break;
                case F3DEXCommandID.F3DEX_G_SETTIMG:
                    _lastSetTImg = (F3DEX_G_SetTImg)command;
                    TMEM.LastTImgCommand = _lastSetTImg;
                    //TMEM.RAMTexturePixelSize = _lastSetTImg.PixelSize;
                    //TMEM.RAMTextureAddress = _lastSetTImg.ImageAddress;
                    //TMEM.RAMTextureImageFormat = _lastSetTImg.Format;
                    //TMEM.RAMTextureWidth = _lastSetTImg.Width;

                    break;
                case F3DEXCommandID.F3DEX_G_LOADBLOCK:
                    _lastLoadBlock = (F3DEX_G_LoadBlock)command;

                    TMEM.LoadBlockIntoTMem(_lastLoadBlock);
                    break;
                case F3DEXCommandID.F3DEX_G_LOADTLUT:
                    _lastLoadTLut = (F3DEX_G_LoadTLut)command;

                    TMEM.LoadTLutIntoTMem(_lastLoadTLut);
                    break;
                case F3DEXCommandID.F3DEX_G_VTX:
                    F3DEX_G_Vtx vtx = (F3DEX_G_Vtx)command;

                    RomFile file;
                    int offset;
                    if(RomProject.Instance.FindRamOffset(vtx.VertexSourceAddress, out file, out offset))
                    {
                        if (file.GetElementAt(offset) is VertexCollection)
                        {
                            //Just update the reference buffer
                            VertexCollection collection = (VertexCollection)file.GetElementAt(offset);

                            if (vtx.VertexCount > 0)
                            {
                                int startIndex = 0;
                                bool foundVertices = false;
                                for (int i = 0; i < collection.Vertices.Count; i++)
                                {
                                    if (collection.Vertices[i].FileOffset == offset)
                                    {
                                        startIndex = i;
                                        foundVertices = true;
                                    }

                                    if (foundVertices)
                                    {
                                        _vertexBuffer[vtx.TargetBufferIndex + (i - startIndex)] = collection.Vertices[i];
                                        if (i - startIndex + 1 >= vtx.VertexCount)
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Actually load in the bytes as new Vertices
                            byte[] bytes = file.GetAsBytes();

                            byte[] vtxBytes = new byte[0x10];

                            List<Vertex> vertices = new List<Vertex>();

                            for (int i = 0; i < vtx.VertexCount; i++)
                            {
                                Array.Copy(bytes, offset, vtxBytes, 0, 0x10);
                                Vertex vertex = new Vertex(offset, vtxBytes);
                                //Add to collection somewhere
                                vertices.Add(vertex);

                                offset += 0x10;

                                _vertexBuffer[i + vtx.TargetBufferIndex] = vertex;
                            }

                            if (!_foundVertices.ContainsKey(file))
                                _foundVertices.Add(file, new List<Vertex>());

                            _foundVertices[file].AddRange(vertices);
                        }
                    }
                    break;
                case F3DEXCommandID.F3DEX_G_TRI1:
                case F3DEXCommandID.F3DEX_G_TRI2:
                    if (TMEM.TileDescriptors[DefaultTextureTile].SettingsChanged)
                    {
                        //Try seeing if the texture has been loaded before
                        if (!TryLoadExistingTexture(TMEM.TileDescriptors[DefaultTextureTile], out _currentImage) &&
                            TMEM.LoadedData.ContainsKey(TMEM.TileDescriptors[DefaultTextureTile].TMem))
                        {
                            LoadedTMemData tmemInfo = TMEM.LoadedData[TMEM.TileDescriptors[DefaultTextureTile].TMem];

                            //First try loading up the palette
                            Palette palette = null;
                            if (!TryLoadExistingPalette(out palette) &&
                                TMEM.LoadedData.ContainsKey(PALETTE_TMEM_OFFSET))
                            {
                                palette = ReadPaletteFromTMem();

                                if (palette != null)
                                {
                                    tmemInfo = TMEM.LoadedData[PALETTE_TMEM_OFFSET];

                                    if (!_foundPalettes.ContainsKey(tmemInfo.SourceFile))
                                        _foundPalettes.Add(tmemInfo.SourceFile, new List<Palette>());

                                    _foundPalettes[tmemInfo.SourceFile].Add(palette);
                                }
                            }

                            //Load up new Texture
                            _currentImage = ReadTextureFromTMem(TMEM.TileDescriptors[DefaultTextureTile], palette);

                            if (_currentImage != null)
                            {
                                //Store in the found variables
                                if (!_foundTextures.ContainsKey(tmemInfo.SourceFile))
                                    _foundTextures.Add(tmemInfo.SourceFile, new List<Texture>());

                                _foundTextures[tmemInfo.SourceFile].Add(_currentImage);

                            }
                        }
                        TMEM.TileDescriptors[0].SettingsChanged = false;
                    }
                    if (command.CommandID == F3DEXCommandID.F3DEX_G_TRI1)
                    {
                        F3DEX_G_Tri1 tri = (F3DEX_G_Tri1)command;
                        tri.ImageReference = _currentImage;

                        tri.Vertex1Reference = _vertexBuffer[tri.Vertex1];
                        tri.Vertex2Reference = _vertexBuffer[tri.Vertex2];
                        tri.Vertex3Reference = _vertexBuffer[tri.Vertex3];
                    }
                    else if (command.CommandID == F3DEXCommandID.F3DEX_G_TRI2)
                    {
                        F3DEX_G_Tri2 tri = (F3DEX_G_Tri2)command;
                        tri.ImageReference = _currentImage;


                        tri.Vertex1Reference = _vertexBuffer[tri.Vertex1];
                        tri.Vertex2Reference = _vertexBuffer[tri.Vertex2];
                        tri.Vertex3Reference = _vertexBuffer[tri.Vertex3];
                        tri.Vertex4Reference = _vertexBuffer[tri.Vertex4];
                        tri.Vertex5Reference = _vertexBuffer[tri.Vertex5];
                        tri.Vertex6Reference = _vertexBuffer[tri.Vertex6];

                    }

                    break;
            }
        }

        private static bool TryLoadExistingTexture(TileDescriptor tile, out Texture texture)
        {
            texture = null;

            if (TMEM.LoadedElements.ContainsKey(TMEM.TileDescriptors[DefaultTextureTile].TMem))
            {
                texture = (TMEM.LoadedElements[TMEM.TileDescriptors[DefaultTextureTile].TMem] as Texture);
                Palette newPalette;
              //  if (TryLoadExistingPalette(out newPalette))
             //       texture.ImagePalette = newPalette;
                return true;
            }

            if (!TMEM.LoadedData.ContainsKey(TMEM.TileDescriptors[DefaultTextureTile].TMem))
                return false;

            LoadedTMemData tmemInfo = TMEM.LoadedData[TMEM.TileDescriptors[DefaultTextureTile].TMem];

            if(tmemInfo.SourceFile == null || !_foundTextures.ContainsKey(tmemInfo.SourceFile))
                return false;

            for (int i = 0; i < _foundTextures[tmemInfo.SourceFile].Count; i++)
            {
                if (_foundTextures[tmemInfo.SourceFile][i].FileOffset == tmemInfo.FileOffset)
                {
                    texture = _foundTextures[tmemInfo.SourceFile][i];
                    return true;
                }
            }

            return false;
        }

        private static bool TryLoadExistingPalette(out Palette palette)
        {
            palette = null;

            if (TMEM.LoadedElements.ContainsKey(PALETTE_TMEM_OFFSET))
            {
                palette = (TMEM.LoadedElements[PALETTE_TMEM_OFFSET] as Palette);
                return true;
            }

            if (!TMEM.LoadedData.ContainsKey(PALETTE_TMEM_OFFSET))
                return false;

            LoadedTMemData tmemInfo = TMEM.LoadedData[PALETTE_TMEM_OFFSET];

            if (tmemInfo.SourceFile == null || !_foundPalettes.ContainsKey(tmemInfo.SourceFile))
                return false;

            for (int i = 0; i < _foundPalettes[tmemInfo.SourceFile].Count; i++)
            {
                if (_foundPalettes[tmemInfo.SourceFile][i].FileOffset == tmemInfo.FileOffset)
                {
                    palette = _foundPalettes[tmemInfo.SourceFile][i];
                    return true;
                }
            }

            return false;
        }

        private const int PALETTE_TMEM_OFFSET = 0x100;
        private const int PALETTE_TMEM_OFFSET_IN_BYTES = 0x800; //offset * 8

        private static Palette ReadPaletteFromTMem()
        {
            Palette newPalette;

            if (!TMEM.LoadedData.ContainsKey(PALETTE_TMEM_OFFSET))
                return null;

            int offset = TMEM.LoadedData[PALETTE_TMEM_OFFSET].FileOffset;
            int sizeOfData = TMEM.LoadedData[PALETTE_TMEM_OFFSET].Size;

            byte[] data = new byte[sizeOfData];
            Array.Copy(TMEM.Data, PALETTE_TMEM_OFFSET_IN_BYTES, data, 0, sizeOfData);

            newPalette = new Palette(offset, data);

            return newPalette;
        }

        private static Texture ReadTextureFromTMem(TileDescriptor tile, Palette palette = null)
        {
            Texture newTexture;

            if (!TMEM.LoadedData.ContainsKey(tile.TMem))
                return null;

            //if (!tile.On)   //Not sure if used??
            //    return null;

            int offset = TMEM.LoadedData[tile.TMem].FileOffset;
            int sizeOfData = TMEM.LoadedData[tile.TMem].Size;

            int widthInBytes = 8 * tile.Line;

            double texelSize = 1;
            switch (tile.PixelSize)
            {
                case Texture.PixelInfo.Size_4b:
                    texelSize = 0.5;
                    break;
                case Texture.PixelInfo.Size_16b:
                    texelSize = 2;
                    break;
                case Texture.PixelInfo.Size_32b:
                    texelSize = 4;
                    break;
            }

            int widthInTexels = (int)Math.Round(widthInBytes / texelSize);
            int heightInTexels = sizeOfData / widthInBytes;

            byte[] data = new byte[sizeOfData];
            Array.Copy(TMEM.Data, tile.TMemInBytes, data, 0, sizeOfData);
            
            if(tile.ImageFormat == Texture.ImageFormat.CI)
                newTexture = new Texture(offset, data, tile.ImageFormat, tile.PixelSize,
                    widthInTexels, heightInTexels, palette, tile.Palette);
            else
                newTexture = new Texture(offset, data, tile.ImageFormat, tile.PixelSize,
                    widthInTexels, heightInTexels);
            
            tile.SettingsChanged = false;

            return newTexture;
        }

*/
    #endregion

    }

    ///Helper classes to allow duplicate data to be removed
    //public class TextureComparer : IEqualityComparer<Texture>
    //{
    //    public bool Equals(Texture texture1, Texture texture2)
    //    {
    //        return texture1.FileOffset == texture2.FileOffset;
    //    }
    //    public int GetHashCode(Texture texture)
    //    {
    //        return texture.FileOffset.GetHashCode();
    //    }
    //}
    //public class PaletteComparer : IEqualityComparer<Palette>
    //{
    //    public bool Equals(Palette palette1, Palette palette2)
    //    {
    //        return palette1.FileOffset == palette2.FileOffset;
    //    }
    //    public int GetHashCode(Palette palette)
    //    {
    //        return palette.FileOffset.GetHashCode();
    //    }
    //}
    //public class VertexComparer : IEqualityComparer<Vertex>
    //{
    //    public bool Equals(Vertex vertex1, Vertex vertex2)
    //    {
    //        return vertex1.FileOffset == vertex2.FileOffset;
    //    }
    //    public int GetHashCode(Vertex vertex)
    //    {
    //        return vertex.FileOffset.GetHashCode();
    //    }
    //}
    //public class CommandComparer : IEqualityComparer<F3DEXCommand>
    //{
    //    public bool Equals(F3DEXCommand command1, F3DEXCommand command2)
    //    {
    //        return command1.FileOffset == command2.FileOffset;
    //    }
    //    public int GetHashCode(F3DEXCommand command)
    //    {
    //        return command.FileOffset.GetHashCode();
    //    }
    //}
}
