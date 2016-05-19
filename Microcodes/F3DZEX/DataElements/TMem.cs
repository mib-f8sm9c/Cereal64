using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common.Utils;
using Cereal64.Microcodes.F3DZEX.DataElements.Commands;
using Cereal64.Common.Rom;
using Cereal64.Common.DataElements;

namespace Cereal64.Microcodes.F3DZEX.DataElements
{
    public class LoadedTMemData
    {
        public RomFile SourceFile;
        public int FileOffset;
        public int Size;
        public byte[] Data;

        public LoadedTMemData(RomFile file, int offset, int size, byte[] data)
        {
            SourceFile = file;
            FileOffset = offset;
            Size = size;
            Data = data;
        }
    }

    public class TMem
    {
        public byte[] Data;
        public TileDescriptor[] TileDescriptors;

        public F3DZEX_G_SetTImg LastTImgCommand;

        public Dictionary<int, LoadedTMemData> LoadedData; //Info of blocks of data loaded into TMEM
        public Dictionary<int, N64DataElement> LoadedElements; //Info of existing elements loaded into TMEM (used when parsing previously parsed data)

        public TMem()
        {
            Data = new byte[4096];
            TileDescriptors = new TileDescriptor[8];
            for (int i = 0; i < TileDescriptors.Length; i++)
                TileDescriptors[i] = new TileDescriptor((byte)i);
            LoadedData = new Dictionary<int, LoadedTMemData>();
            LoadedElements = new Dictionary<int, N64DataElement>();
        }

        public void LoadBlockIntoTMem(F3DZEX_G_LoadBlock loadBlock)
        {
            RomFile file;
            int offset;
            TileDescriptor tile = TileDescriptors[loadBlock.Tile];

            if (RomProject.Instance.FindRamOffset(LastTImgCommand.ImageAddress, out file, out offset))
            {
                if (file.GetElementAt(offset) is Palette)
                {
                    Palette palette = (Palette)file.GetElementAt(offset);
                    LoadedElements[tile.TMem] = palette;
                }
                else if (file.GetElementAt(offset) is Texture)
                {
                    Texture texture = (Texture)file.GetElementAt(offset);
                    LoadedElements[tile.TMem] = texture;
                }
                else
                {
                    byte[] data = file.GetAsBytes();

                    double texelSizeInBytes = 1;
                    switch (tile.PixelSize)
                    {
                        case Texture.PixelInfo.Size_4b:
                            texelSizeInBytes = 0.5;
                            break;
                        case Texture.PixelInfo.Size_16b:
                            texelSizeInBytes = 2;
                            break;
                        case Texture.PixelInfo.Size_32b:
                            texelSizeInBytes = 4;
                            break;
                    }

                    int size = (int)Math.Round(loadBlock.Texels * texelSizeInBytes);
                    byte[] newData = new byte[size];

                    Array.Copy(data, offset, newData, 0, size);
                    Array.Copy(newData, 0, this.Data, tile.TMemInBytes, size);

                    LoadedData[tile.TMem] = new LoadedTMemData(file,
                        offset, size, newData);
                }
            }
        }

        public void LoadTLutIntoTMem(F3DZEX_G_LoadTLut loadTLut)
        {
            RomFile file;
            int offset;
            TileDescriptor tile = TileDescriptors[loadTLut.Tile];

            if (RomProject.Instance.FindRamOffset(LastTImgCommand.ImageAddress, out file, out offset))
            {
                if (file.GetElementAt(offset) is Palette)
                {
                    Palette palette = (Palette)file.GetElementAt(offset);
                    LoadedElements[tile.TMem] = palette;
                }
                else if (file.GetElementAt(offset) is Texture)
                {
                    Texture texture = (Texture)file.GetElementAt(offset);
                    LoadedElements[tile.TMem] = texture;
                }
                else
                {
                    byte[] data = file.GetAsBytes();

                    double texelSizeInBytes = 2; //Hardcoded?

                    int size = (int)Math.Round(loadTLut.Count * texelSizeInBytes);
                    byte[] newData = new byte[size];

                    Array.Copy(data, offset, newData, 0, size);
                    Array.Copy(newData, 0, this.Data, tile.TMemInBytes, size);

                    LoadedData[tile.TMem] = new LoadedTMemData(file,
                        offset, size, newData);
                }
            }
        }

        public LoadedTMemData GetTexturePaletteInfo(TileDescriptor tile)
        {
            if (LoadedData.ContainsKey(tile.TMem))
                return null;

            return LoadedData[tile.TMem];
        }

    }

    public class TileDescriptor
    {
        public Texture.PixelInfo PixelSize
        {
            get { return _pixelSize; }
            set { _pixelSize = value; SettingsChanged = true; }
        }
        private Texture.PixelInfo _pixelSize;

        public Texture.ImageFormat ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value; SettingsChanged = true; }
        }
        private Texture.ImageFormat _imageFormat;

        public ushort Line
        {
            get { return _line; }
            set { _line = value; SettingsChanged = true; }
        }
        private ushort _line;

        public ushort TMemInBytes
        {
            get { return (ushort)(_tMem * 8); }
        }

        public ushort TMem
        {
            get { return _tMem; }
            set { _tMem = value; SettingsChanged = true; }
        }
        private ushort _tMem;

        public byte Tile
        {
            get { return _tile; }
            set { _tile = value; SettingsChanged = true; }
        }
        private byte _tile;

        public byte Palette
        {
            get { return _palette; }
            set { _palette = value; SettingsChanged = true; }
        }
        private byte _palette;

        public Commands.F3DZEX_G_SetTile.TextureMirrorSetting CMTMirror
        {
            get { return _cmtMirror; }
            set { _cmtMirror = value; SettingsChanged = true; }
        }
        private Commands.F3DZEX_G_SetTile.TextureMirrorSetting _cmtMirror;

        public Commands.F3DZEX_G_SetTile.TextureMirrorSetting CMSMirror
        {
            get { return _cmsMirror; }
            set { _cmsMirror = value; SettingsChanged = true; }
        }
        private Commands.F3DZEX_G_SetTile.TextureMirrorSetting _cmsMirror;

        public Commands.F3DZEX_G_SetTile.TextureWrapSetting CMTWrap
        {
            get { return _cmtWrap; }
            set { _cmtWrap = value; SettingsChanged = true; }
        }
        private Commands.F3DZEX_G_SetTile.TextureWrapSetting _cmtWrap;

        public Commands.F3DZEX_G_SetTile.TextureWrapSetting CMSWrap
        {
            get { return _cmsWrap; }
            set { _cmsWrap = value; SettingsChanged = true; }
        }
        private Commands.F3DZEX_G_SetTile.TextureWrapSetting _cmsWrap;

        public byte MaskT
        {
            get { return _maskT; }
            set { _maskT = value; SettingsChanged = true; }
        }
        private byte _maskT;

        public byte ShiftT
        {
            get { return _shiftT; }
            set { _shiftT = value; SettingsChanged = true; }
        }
        private byte _shiftT;

        public byte MaskS
        {
            get { return _maskS; }
            set { _maskS = value; SettingsChanged = true; }
        }
        private byte _maskS;

        public byte ShiftS
        {
            get { return _shiftS; }
            set { _shiftS = value; SettingsChanged = true; }
        }
        private byte _shiftS;

        public qushort ULS
        {
            get { return _uls; }
            set { _uls = value; SettingsChanged = true; }
        }
        private qushort _uls;

        public qushort ULT
        {
            get { return _ult; }
            set { _ult = value; SettingsChanged = true; }
        }
        private qushort _ult;

        public qushort LRS
        {
            get { return _lrs; }
            set { _lrs = value; SettingsChanged = true; }
        }
        private qushort _lrs;

        public qushort LRT
        {
            get { return _lrt; }
            set { _lrt = value; SettingsChanged = true; }
        }
        private qushort _lrt;

        public bool On
        {
            get { return _on; }
            set { _on = value; SettingsChanged = true; }
        }
        private bool _on;

        public byte Level
        {
            get { return _level; }
            set { _level = value; SettingsChanged = true; }
        }
        private byte _level;

        public ushort ScaleS
        {
            get { return _scaleS; }
            set { _scaleS = value; SettingsChanged = true; }
        }
        private ushort _scaleS;

        public ushort ScaleT
        {
            get { return _scaleT; }
            set { _scaleT = value; SettingsChanged = true; }
        }
        private ushort _scaleT;

        public bool SettingsChanged { get; set; }

        public TileDescriptor(byte tileNum)
        {
            On = false;

            Tile = tileNum;

            ULS = new qushort("10.2", 0);
            ULT = new qushort("10.2", 0);
            LRS = new qushort("10.2", 0);
            LRT = new qushort("10.2", 0);
        }
    }
}
