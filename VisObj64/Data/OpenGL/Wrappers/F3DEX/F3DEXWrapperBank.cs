using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DEX.DataElements;
using Cereal64.Microcodes.F3DEX.DataElements.Commands;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DEX
{
    /// <summary>
    /// Class to provide references to existing wrappers so that we don't create duplicates
    /// </summary>
    public static class F3DEXWrapperBank
    {
        /// <summary>
        /// Move these into the appropriate wrapper file for each, and remove the constructors???
        /// </summary>

        private static Dictionary<F3DEXImage, F3DEXTextureWrapper> _textureWrappers = new Dictionary<F3DEXImage, F3DEXTextureWrapper>();
        private static Dictionary<Vertex, F3DEXVertexWrapper> _vertexWrappers = new Dictionary<Vertex, F3DEXVertexWrapper>();
        private static Dictionary<F3DEX_G_Tri1, F3DEXTriangleWrapper> _triangleWrappers = new Dictionary<F3DEX_G_Tri1, F3DEXTriangleWrapper>();
        private static Dictionary<F3DEX_G_Tri2, F3DEXTriangleWrapper> _triangle2FirstWrappers = new Dictionary<F3DEX_G_Tri2, F3DEXTriangleWrapper>();
        private static Dictionary<F3DEX_G_Tri2, F3DEXTriangleWrapper> _triangle2SecondWrappers = new Dictionary<F3DEX_G_Tri2, F3DEXTriangleWrapper>();

        public static F3DEXTextureWrapper GetTextureWrapper(F3DEXImage image, F3DEX_G_SetTile command, F3DEX_G_Texture textureCommand)
        {
            if (_textureWrappers.ContainsKey(image))
                return _textureWrappers[image];

            F3DEXTextureWrapper wrapper = new F3DEXTextureWrapper(image, command, textureCommand);
            _textureWrappers.Add(image, wrapper);

            return wrapper;
        }

        public static F3DEXVertexWrapper GetVertexWrapper(Vertex vertex)
        {
            if (_vertexWrappers.ContainsKey(vertex))
                return _vertexWrappers[vertex];

            F3DEXVertexWrapper wrapper = new F3DEXVertexWrapper(vertex);
            _vertexWrappers.Add(vertex, wrapper);

            return wrapper;
        }

        public static F3DEXTriangleWrapper GetTriangleWrapper(F3DEX_G_Tri1 triangle)
        {
            if (_triangleWrappers.ContainsKey(triangle))
                return _triangleWrappers[triangle];

            F3DEXTriangleWrapper wrapper = new F3DEXTriangleWrapper(triangle);
            _triangleWrappers.Add(triangle, wrapper);

            return wrapper;
        }

        public static F3DEXTriangleWrapper GetTriangleWrapper(F3DEX_G_Tri2 triangle, int index)
        {
            Dictionary<F3DEX_G_Tri2, F3DEXTriangleWrapper> wrappers;
            if (index % 2 == 1)
                wrappers = _triangle2FirstWrappers;
            else
                wrappers = _triangle2SecondWrappers;

            if (wrappers.ContainsKey(triangle))
                return wrappers[triangle];

            F3DEXTriangleWrapper wrapper = new F3DEXTriangleWrapper(triangle, index);
            wrappers.Add(triangle, wrapper);

            return wrapper;
        }
    }
}
