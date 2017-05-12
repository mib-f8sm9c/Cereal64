using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using Cereal64.Microcodes.F3DEX.DataElements.Commands;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DEX
{
    public class F3DEXGraphicsElement : VO64GraphicsElement
    {
        public List<F3DEXCommand> Commands { get; private set; }

        protected F3DEXGraphicsElement(int vaIndex, int vbIndex, int ibIndex, string name)
            : base(vaIndex, vbIndex, ibIndex)
        {
            Name = name;
            Commands = new List<F3DEXCommand>();
        }

        public static F3DEXGraphicsElement CreateNewElement(string name = "Element")
        {
            int newvaIndex, newvbIndex, newibIndex;
            GL.GenVertexArrays(1, out newvaIndex);
            GL.GenBuffers(1, out newvbIndex);
            GL.GenBuffers(1, out newibIndex);

            return new F3DEXGraphicsElement(newvaIndex, newvbIndex, newibIndex, name);
        }

    }
}
