using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DZEX.DataElements;
using Cereal64.Microcodes.F3DZEX.DataElements.Commands;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DZEX
{
    public class F3DZEXTextureWrapper : IVO64Texture
    {
        //NOTE: THIS WILL ALL BREAK IF TEXTURE/COMMANDS ARE NULL
        public Texture F3DTexture;
        public F3DZEX_G_SetTile SetTileCommand;
        public F3DZEX_G_Texture TextureCommand;

        public Bitmap Image {
            get { return (F3DTexture == null ? null : F3DTexture.Image); } 
            set
            { //Finish this later
            //    byte[] rawData = TextureConversion.CI4ToBinary(value, F3DTexture.ImagePalette, F3DTexture.PaletteIndex, true);
            //    SelectedTexture.RawData = rawData;
            //    _updated = true; 
            } 
        }

        public TextureWrapMode WrapS { 
            get { return ConvertWrapMirrorSettings(SetTileCommand.CMSWrap, SetTileCommand.CMSMirror); }
            set
            {
                F3DZEX_G_SetTile.TextureWrapSetting wrap;
                F3DZEX_G_SetTile.TextureMirrorSetting mirror;
                ConvertTextureWrapSettings(value, out mirror, out wrap);
                SetTileCommand.CMSWrap = wrap;
                SetTileCommand.CMSMirror = mirror;
                _updated = true;
            }
        }
        public TextureWrapMode WrapT
        {
            get { return ConvertWrapMirrorSettings(SetTileCommand.CMTWrap, SetTileCommand.CMTMirror); }
            set
            {
                F3DZEX_G_SetTile.TextureWrapSetting wrap;
                F3DZEX_G_SetTile.TextureMirrorSetting mirror;
                ConvertTextureWrapSettings(value, out mirror, out wrap);
                SetTileCommand.CMTWrap = wrap;
                SetTileCommand.CMTMirror = mirror;
                _updated = true;
            }
        }

        private TextureWrapMode ConvertWrapMirrorSettings(F3DZEX_G_SetTile.TextureWrapSetting wrap, F3DZEX_G_SetTile.TextureMirrorSetting mirror)
        {
            if (wrap == F3DZEX_G_SetTile.TextureWrapSetting.G_TX_WRAP)
            {
                if (mirror == F3DZEX_G_SetTile.TextureMirrorSetting.G_TX_MIRROR)
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
            out F3DZEX_G_SetTile.TextureMirrorSetting mirror, out F3DZEX_G_SetTile.TextureWrapSetting wrap)
        {
            if (mode == TextureWrapMode.MirroredRepeat)
            {
                mirror = F3DZEX_G_SetTile.TextureMirrorSetting.G_TX_MIRROR;
                wrap = F3DZEX_G_SetTile.TextureWrapSetting.G_TX_WRAP;
            }
            else if (mode == TextureWrapMode.Repeat)
            {
                mirror = F3DZEX_G_SetTile.TextureMirrorSetting.G_TX_NOMIRROR;
                wrap = F3DZEX_G_SetTile.TextureWrapSetting.G_TX_WRAP;
            }
            else
            {
                mirror = F3DZEX_G_SetTile.TextureMirrorSetting.G_TX_NOMIRROR;
                wrap = F3DZEX_G_SetTile.TextureWrapSetting.G_TX_CLAMP;
            }
        }

        private bool _updated = true;
        private VO64SimpleTexture _simpleTexture;

        public F3DZEXTextureWrapper(Texture texture, F3DZEX_G_SetTile command, F3DZEX_G_Texture textureCommand)
        {
            F3DTexture = texture;
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
            get { if (TextureCommand == null) return 1.0f; return (float)TextureCommand.ScaleS / (float)ushort.MaxValue; }
        }

        public float ScaleT
        {
            get { if (TextureCommand == null) return 1.0f; return (float)TextureCommand.ScaleT / (float)ushort.MaxValue; }
        }

        public int TexWidth { get { if (Image == null) return 1; return Image.Width; } }

        public int TexHeight { get { if (Image == null) return 1;  return Image.Height; } }

    }
}
