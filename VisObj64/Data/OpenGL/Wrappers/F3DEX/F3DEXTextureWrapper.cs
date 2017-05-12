using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DEX.DataElements;
using Cereal64.Microcodes.F3DEX.DataElements.Commands;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DEX
{
    public class F3DEXTextureWrapper : IVO64Texture
    {
        //NOTE: THIS WILL ALL BREAK IF TEXTURE/COMMANDS ARE NULL
        public F3DEXImage F3DImage;
        public F3DEX_G_SetTile SetTileCommand;
        public F3DEX_G_Texture TextureCommand;

        public Bitmap Image
        {
            get 
            { 
                if(F3DImage == null)
                    return null;
                return F3DImage.Image;
            } 
            set
            { //Finish this later
            //    byte[] rawData = TextureConversion.CI4ToBinary(value, F3DTexture.ImagePalette, F3DTexture.PaletteIndex, true);
            //    SelectedTexture.RawData = rawData;
            //    _updated = true; 
            } 
        }

        public TextureWrapMode WrapS { 
            get { if(SetTileCommand == null) return ConvertWrapMirrorSettings(F3DEX_G_SetTile.TextureWrapSetting.G_TX_CLAMP, F3DEX_G_SetTile.TextureMirrorSetting.G_TX_NOMIRROR); return ConvertWrapMirrorSettings(SetTileCommand.CMSWrap, SetTileCommand.CMSMirror); }
            set
            {
                F3DEX_G_SetTile.TextureWrapSetting wrap;
                F3DEX_G_SetTile.TextureMirrorSetting mirror;
                ConvertTextureWrapSettings(value, out mirror, out wrap);
                SetTileCommand.CMSWrap = wrap;
                SetTileCommand.CMSMirror = mirror;
                _updated = true;
            }
        }
        public TextureWrapMode WrapT
        {
            get { if (SetTileCommand == null) return ConvertWrapMirrorSettings(F3DEX_G_SetTile.TextureWrapSetting.G_TX_CLAMP, F3DEX_G_SetTile.TextureMirrorSetting.G_TX_NOMIRROR); return ConvertWrapMirrorSettings(SetTileCommand.CMTWrap, SetTileCommand.CMTMirror); }
            set
            {
                F3DEX_G_SetTile.TextureWrapSetting wrap;
                F3DEX_G_SetTile.TextureMirrorSetting mirror;
                ConvertTextureWrapSettings(value, out mirror, out wrap);
                SetTileCommand.CMTWrap = wrap;
                SetTileCommand.CMTMirror = mirror;
                _updated = true;
            }
        }

        private TextureWrapMode ConvertWrapMirrorSettings(F3DEX_G_SetTile.TextureWrapSetting wrap, F3DEX_G_SetTile.TextureMirrorSetting mirror)
        {
            if (wrap == F3DEX_G_SetTile.TextureWrapSetting.G_TX_WRAP)
            {
                if (mirror == F3DEX_G_SetTile.TextureMirrorSetting.G_TX_MIRROR)
                {
                    return TextureWrapMode.MirroredRepeat;
                }
                else
                {
                    return TextureWrapMode.Repeat;
                }
            }
            else
            {
                return TextureWrapMode.Clamp;
            }
        }

        private void ConvertTextureWrapSettings(TextureWrapMode mode,
            out F3DEX_G_SetTile.TextureMirrorSetting mirror, out F3DEX_G_SetTile.TextureWrapSetting wrap)
        {
            if (mode == TextureWrapMode.MirroredRepeat)
            {
                mirror = F3DEX_G_SetTile.TextureMirrorSetting.G_TX_MIRROR;
                wrap = F3DEX_G_SetTile.TextureWrapSetting.G_TX_WRAP;
            }
            else if (mode == TextureWrapMode.Repeat)
            {
                mirror = F3DEX_G_SetTile.TextureMirrorSetting.G_TX_NOMIRROR;
                wrap = F3DEX_G_SetTile.TextureWrapSetting.G_TX_WRAP;
            }
            else
            {
                mirror = F3DEX_G_SetTile.TextureMirrorSetting.G_TX_NOMIRROR;
                wrap = F3DEX_G_SetTile.TextureWrapSetting.G_TX_CLAMP;
            }
        }

        private bool _updated = true;
        private VO64SimpleTexture _simpleTexture;

        public F3DEXTextureWrapper(F3DEXImage image, F3DEX_G_SetTile command, F3DEX_G_Texture textureCommand)
        {
            F3DImage = image;
            SetTileCommand = command;
            TextureCommand = textureCommand;
        }

        public VO64SimpleTexture GetAsSimpleTexture()
        {
            if (_updated)
            {
                _simpleTexture = new VO64SimpleTexture(Image, WrapS, WrapT);
                _updated = false;
            }
            return _simpleTexture;
        }

        public float ShiftScaleS
        {
            get
            {
                if (SetTileCommand == null)
                    return 1.0f;

                if (SetTileCommand.ShiftS > 10)
                {
                    return (float)(1 << (int)(16 - SetTileCommand.ShiftS));
                }
                else if (SetTileCommand.ShiftS > 0)
                {
                    return 1.0f / (float)(1 << (int)(16 - SetTileCommand.ShiftS));
                }
                else
                    return 1.0f;
            }
        }

        public float ShiftScaleT
        {
            get
            {
                if (SetTileCommand == null)
                    return 1.0f;

                if (SetTileCommand.ShiftT > 10)
                {
                    return (float)(1 << (int)(16 - SetTileCommand.ShiftT));
                }
                else if (SetTileCommand.ShiftT > 0)
                {
                    return 1.0f / (float)(1 << (int)(16 - SetTileCommand.ShiftT));
                }
                else
                    return 1.0f;
            }
        }

        public float ScaleS
        {
            get { return (float)TextureCommand.ScaleS / (float)ushort.MaxValue; }
        }

        public float ScaleT
        {
            get { return (float)TextureCommand.ScaleT / (float)ushort.MaxValue; }
        }

        public int TexWidth { get { if(Image == null) return 32; return Image.Width; } }

        public int TexHeight { get { if(Image == null) return 32; return Image.Height; } }

    }
}
