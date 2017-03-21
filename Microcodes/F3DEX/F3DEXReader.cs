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

        private static List<F3DEXImage> _foundImages = new List<F3DEXImage>();
        private static Dictionary<Texture, List<F3DEXImage>> _imagesByTexture = new Dictionary<Texture, List<F3DEXImage>>();
        private static Dictionary<Palette, List<F3DEXImage>> _imagesByPalette = new Dictionary<Palette, List<F3DEXImage>>();

        private static List<Vertex> _vertexBuffer = new List<Vertex>();

        private static Stack<int> _dlPointerStack = new Stack<int>();
        //Can this work without the RomProject backing? Probably best not to use this
        //public static void ParseF3DEXCommands(IList<F3DEXCommand> commands)
        //{
        //    ResetReaderVariables();

        //    foreach (F3DEXCommand command in commands)
        //    {
        //        //ParseCommand(command);
        //    }
        //}

        private static void ResetReaderVariables()
        {
            _foundCommands.Clear();
            _foundPalettes.Clear();
            _foundTextures.Clear();
            _foundVertices.Clear();

            _foundImages.Clear();
            _imagesByTexture.Clear();
            _imagesByPalette.Clear();

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

            _dlPointerStack.Clear();
        }

        public static F3DEXReaderPackage ReadF3DEXAt(RomFile file, int offset)
        {
            ResetReaderVariables();
            
            byte[] data = file.GetAsBytes();

            F3DEXReaderPackage package = new F3DEXReaderPackage();

            if (offset < 0 || offset >= data.Length || offset % 8 != 0)
                return package;

            //Reset/Initialize what needs to be initialized/reset

            byte[] command = new byte[8];
            N64DataElement el;
            F3DEXCommand f3Command;

            //A bit backwards to add it in here and then immediately pop it off, but it'll help structurally later
            _dlPointerStack.Push(offset);

            //OPTIMIZE THIS BY FINDING THE F3DEXCOMMANDCOLLECTION ONCE ONLY

            while (_dlPointerStack.Count > 0)
            {
                offset = _dlPointerStack.Pop();

                while (offset < data.Length && offset >= 0)
                {
                    f3Command = null;

                    if (file.HasElementAt(offset, out el))
                    {
                        if (el is F3DEXCommand)
                        {
                            f3Command = (F3DEXCommand)el;
                        }
                        else if (el is F3DEXCommandCollection)
                        {
                            int index = (offset - el.FileOffset) / 0x8;
                            f3Command = ((F3DEXCommandCollection)el).Commands[index];
                        }
                    }
                    
                    if(f3Command == null)
                    {
                        //read the command
                        Array.Copy(data, offset, command, 0, 8);

                        f3Command = F3DEXCommandFactory.ReadCommand(offset, command);
                        if (f3Command == null)
                            break;

                        if (!_foundCommands.ContainsKey(file))
                            _foundCommands.Add(file, new List<F3DEXCommand>());

                        _foundCommands[file].Add(f3Command);
                    }
                    
                    //Increment before so the DL jump code doesn't get confusing
                    offset += 8;

                    ParseCommand(f3Command, ref offset, file);

                    if (f3Command is F3DEX_G_EndDL || f3Command is F3DEX_G_MK64_EndDL) //Need to handle other end types too, like MK64EndType?
                        break;
                }
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

        private static void ParseCommand(F3DEXCommand command, ref int dlOffset, RomFile refFile)
        {
            TileDescriptor tile;
            N64DataElement el;
            RomFile file;
            int offset;

            //Here handle extra command things (like if it's referencing textures and such)
            switch (command.CommandID)
            {
                case F3DEXCommandID.F3DEX_G_DL:
                    //Need to put another dl on the stack and keep reading. That forceJump option if it's true should purge the stack.

                    //For now, disable the chance to jump to a different file. We'll need to set that up soon
                    F3DEX_G_DL dlCommand = (F3DEX_G_DL)command;

                    RomProject.Instance.FindRamOffset(dlCommand.DLAddress, out file, out offset);

                    if (file != refFile)
                        offset = -1;

                    _dlPointerStack.Push(dlOffset);

                    dlOffset = offset;

                    if (dlCommand.ForceJump)
                        _dlPointerStack.Clear();
                    break;
                case F3DEXCommandID.F3DEX_G_ENDDL:
                case F3DEXCommandID.F3DEX_G_MK64_ENDDL:
                    //Need to stop reading the current dl, jump back up the stack
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

                    if(RomProject.Instance.FindRamOffset(vtx.VertexSourceAddress, out file, out offset))
                    {
                        if (file.HasElementAt(offset, out el) && el is VertexCollection)
                        {
                            //Just update the reference buffer
                            VertexCollection collection = (VertexCollection)el;

                            if (vtx.VertexCount > 0)
                            {
                                int startIndex = (offset - el.FileOffset) / Vertex.Vertex_Length;
                                if (collection.Vertices.Count >= startIndex + vtx.VertexCount)
                                {
                                    for (int i = 0; i < vtx.VertexCount; i++)
                                    {
                                        _vertexBuffer[vtx.TargetBufferIndex + (i)] = collection.Vertices[i + startIndex];
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
                        LoadCurrentImage();

                        //////////

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

        private static void LoadCurrentImage()
        {
            //Try to load the F3DEXImage if it already has been loaded
            if (!TryLoadExistingImage(TMEM.TileDescriptors[DefaultTextureTile], out _currentImage))
            {
                //If not, find or convert stuff to Texture & Palette, & make a new F3DEXImage
                Texture newTexture = ReadTextureFromTMem(TMEM.TileDescriptors[DefaultTextureTile]);

                if (newTexture != null && TMEM.LoadedData.ContainsKey(TMEM.TileDescriptors[DefaultTextureTile].TMem))
                {
                    LoadedTMemData tmemInfo = TMEM.LoadedData[TMEM.TileDescriptors[DefaultTextureTile].TMem];

                    //Store in the found variables
                    if (!_foundTextures.ContainsKey(tmemInfo.SourceFile))
                        _foundTextures.Add(tmemInfo.SourceFile, new List<Texture>());

                    if (!_foundTextures[tmemInfo.SourceFile].Contains(newTexture))
                        _foundTextures[tmemInfo.SourceFile].Add(newTexture);

                    if (newTexture.Format == Texture.ImageFormat.CI)
                    {
                        Palette newPalette = ReadPaletteFromTMem();

                        if (newPalette != null &&
                            TMEM.LoadedData.ContainsKey(PALETTE_TMEM_OFFSET))
                        {
                            tmemInfo = TMEM.LoadedData[PALETTE_TMEM_OFFSET];

                            if (!_foundPalettes.ContainsKey(tmemInfo.SourceFile))
                                _foundPalettes.Add(tmemInfo.SourceFile, new List<Palette>());

                            if (!_foundPalettes[tmemInfo.SourceFile].Contains(newPalette))
                                _foundPalettes[tmemInfo.SourceFile].Add(newPalette);

                            //Make new F3DEXImage here!
                            _currentImage = new F3DEXImage(newTexture, newPalette);
                        }
                        else
                            _currentImage = null;
                    }
                    else
                    {
                        //Make new F3DEXImage for the texture
                        _currentImage = new F3DEXImage(newTexture);
                    }
                }
                else
                    _currentImage = null;
            }

            //Check if image needs to be added to the list
            if (_currentImage != null && !_foundImages.Contains(_currentImage))
            {
                _foundImages.Add(_currentImage);

                if (!_imagesByTexture.ContainsKey(_currentImage.Texture))
                    _imagesByTexture.Add(_currentImage.Texture, new List<F3DEXImage>());
                _imagesByTexture[_currentImage.Texture].Add(_currentImage);
                foreach (Palette p in _currentImage.BasePalettes)
                {
                    if (!_imagesByPalette.ContainsKey(p))
                        _imagesByPalette.Add(p, new List<F3DEXImage>());
                    _imagesByPalette[p].Add(_currentImage);
                }
            }
        }

        private static bool TryLoadExistingImage(TileDescriptor tile, out F3DEXImage image)
        {
            image = null;

            N64DataElement el;
            Texture texture;

            if(TMEM.LoadedElements.ContainsKey(tile.TMem) && TMEM.LoadedElements[tile.TMem] is Texture)
                texture = (Texture)TMEM.LoadedElements[tile.TMem];
            else if (TMEM.LoadedData.ContainsKey(tile.TMem))
            {
                LoadedTMemData data = TMEM.LoadedData[tile.TMem];
                if (data.SourceFile.HasElementExactlyAt(data.FileOffset, out el) && el is Texture)
                    texture = (Texture)el;
                else
                    return false;
            }
            else
                return false;

            if (!_imagesByTexture.ContainsKey(texture) || _imagesByTexture[texture].Count == 0)
                return false;

            if (texture.Format == Texture.ImageFormat.CI)
            {
                Palette palette;

                //Handle palette stuff here, just in case same texture but different palettes are available
                if (TMEM.LoadedElements.ContainsKey(PALETTE_TMEM_OFFSET) && TMEM.LoadedElements[PALETTE_TMEM_OFFSET] is Palette)
                    palette = (Palette)TMEM.LoadedElements[PALETTE_TMEM_OFFSET];
                else if (TMEM.LoadedData.ContainsKey(PALETTE_TMEM_OFFSET))
                {
                    LoadedTMemData paletteData = TMEM.LoadedData[PALETTE_TMEM_OFFSET];
                    if (paletteData.SourceFile.HasElementExactlyAt(paletteData.FileOffset, out el) && el is Palette)
                        palette = (Palette)el;
                    else
                        return false;
                }
                else
                    return false;

                if (!_imagesByPalette.ContainsKey(palette) || _imagesByPalette[palette].Count == 0)
                    return false;

                List<F3DEXImage> mixedImages = _imagesByPalette[palette].Intersect(_imagesByTexture[texture]).ToList();

                //in the future, can there be more than 1 F3DEXImage??

                if (mixedImages.Count == 0)
                    return false;

                image = mixedImages[0];
                return true;
            }
            else
            {
                //in the future, can there be more than 1 F3DEXImage on a texture without palettes??

                image = _imagesByTexture[texture][0];
                return true;
            }
        }

        private const int PALETTE_TMEM_OFFSET = 0x100;
        private const int PALETTE_TMEM_OFFSET_IN_BYTES = 0x800; //offset * 8

        private static Palette ReadPaletteFromTMem()
        {
            Palette newPalette;

            if (!TMEM.LoadedData.ContainsKey(PALETTE_TMEM_OFFSET))
                return null;

            //If it's already loaded, return it
            if (TMEM.LoadedElements.ContainsKey(PALETTE_TMEM_OFFSET) &&
                TMEM.LoadedElements[PALETTE_TMEM_OFFSET] is Palette)
                return (Palette)TMEM.LoadedElements[PALETTE_TMEM_OFFSET];

            //Else create it
            int offset = TMEM.LoadedData[PALETTE_TMEM_OFFSET].FileOffset;
            int sizeOfData = TMEM.LoadedData[PALETTE_TMEM_OFFSET].Size;

            byte[] data = new byte[sizeOfData];
            Array.Copy(TMEM.Data, PALETTE_TMEM_OFFSET_IN_BYTES, data, 0, sizeOfData);

            newPalette = new Palette(offset, data);

            return newPalette;
        }
        
        private static Texture ReadTextureFromTMem(TileDescriptor tile)
        {
            //NOTE: THIS WON'T WORK IF ALL TEXTURES/PALETTES AREN'T PRELOADED. SINCE NEW TEXTURES/PALETTES ARE STORED IN
            //       _FOUNDTEXTURES INSTEAD OF THE ROMFILE, A SECOND TIME ITS LOADED THE TMEM WILL NOT FIND THE FIRST ONE!!
            Texture newTexture;

            if (!TMEM.LoadedData.ContainsKey(tile.TMem))
                return null;

            //If it's already loaded, return it
            if (TMEM.LoadedElements.ContainsKey(tile.TMem) &&
                TMEM.LoadedElements[tile.TMem] is Texture)
                return (Texture)TMEM.LoadedElements[tile.TMem];

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
            
            newTexture = new Texture(offset, data, tile.ImageFormat, tile.PixelSize, widthInTexels, heightInTexels);
            
            tile.SettingsChanged = false;

            return newTexture;
        }

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
