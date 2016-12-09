using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Cereal64.VisObj64.Data.OpenGL
{
    public interface IVO64Texture
    {
        Bitmap Image { get; set; }

        TextureWrapMode WrapS { get; set; }
        TextureWrapMode WrapT { get; set; }
        

        VO64SimpleTexture GetAsSimpleTexture();
    }
}
