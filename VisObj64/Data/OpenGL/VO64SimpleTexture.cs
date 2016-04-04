using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Cereal64.VisObj64.Data.OpenGL
{
    public class VO64SimpleTexture : IVO64Texture
    {
        //NOTE: THIS WILL ALL BREAK IF TEXTURE IS NULL

        public Bitmap Texture { get; set; }

        public TextureWrapMode WrapS { get; set; }
        public TextureWrapMode WrapT { get; set; }

        public VO64SimpleTexture(Bitmap texture)
        {
            Texture = texture;

            WrapS = TextureWrapMode.Repeat;
            WrapT = TextureWrapMode.Repeat;
        }

        public VO64SimpleTexture(Bitmap texture, TextureWrapMode wrapS, TextureWrapMode wrapT)
        {
            Texture = texture;

            WrapS = wrapS;
            WrapT = wrapT;
        }

        public VO64SimpleTexture GetAsSimpleTexture()
        {
            return this;
        }
    }
}
