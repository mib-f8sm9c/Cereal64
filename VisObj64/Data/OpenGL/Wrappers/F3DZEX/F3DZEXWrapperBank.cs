using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DZEX.DataElements;
using Cereal64.Microcodes.F3DZEX.DataElements.Commands;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DZEX
{
    /// <summary>
    /// Class to provide references to existing wrappers so that we don't create duplicates
    /// </summary>
    public static class F3DZEXWrapperBank
    {
        /// <summary>
        /// Move these into the appropriate wrapper file for each, and remove the constructors???
        /// </summary>

        private static Dictionary<Texture, F3DZEXTextureWrapper> _textureWrappers = new Dictionary<Texture, F3DZEXTextureWrapper>();
        private static Dictionary<Vertex, F3DZEXVertexWrapper> _vertexWrappers = new Dictionary<Vertex, F3DZEXVertexWrapper>();
        private static Dictionary<F3DZEX_G_Tri1, F3DZEXTriangleWrapper> _triangleWrappers = new Dictionary<F3DZEX_G_Tri1, F3DZEXTriangleWrapper>();
        private static Dictionary<F3DZEX_G_Tri2, F3DZEXTriangleWrapper> _triangle2FirstWrappers = new Dictionary<F3DZEX_G_Tri2, F3DZEXTriangleWrapper>();
        private static Dictionary<F3DZEX_G_Tri2, F3DZEXTriangleWrapper> _triangle2SecondWrappers = new Dictionary<F3DZEX_G_Tri2, F3DZEXTriangleWrapper>();

        public static F3DZEXTextureWrapper GetTextureWrapper(Texture texture, F3DZEX_G_SetTile command, F3DZEX_G_Texture textureCommand)
        {
            if (_textureWrappers.ContainsKey(texture))
                return _textureWrappers[texture];

            F3DZEXTextureWrapper wrapper = new F3DZEXTextureWrapper(texture, command, textureCommand);
            _textureWrappers.Add(texture, wrapper);

            return wrapper;
        }

        public static F3DZEXVertexWrapper GetVertexWrapper(Vertex vertex)
        {
            if (_vertexWrappers.ContainsKey(vertex))
                return _vertexWrappers[vertex];

            F3DZEXVertexWrapper wrapper = new F3DZEXVertexWrapper(vertex);
            _vertexWrappers.Add(vertex, wrapper);

            return wrapper;
        }

        public static F3DZEXTriangleWrapper GetTriangleWrapper(F3DZEX_G_Tri1 triangle)
        {
            if (_triangleWrappers.ContainsKey(triangle))
                return _triangleWrappers[triangle];

            F3DZEXTriangleWrapper wrapper = new F3DZEXTriangleWrapper(triangle);
            _triangleWrappers.Add(triangle, wrapper);

            return wrapper;
        }

        public static F3DZEXTriangleWrapper GetTriangleWrapper(F3DZEX_G_Tri2 triangle, int index)
        {
            Dictionary<F3DZEX_G_Tri2, F3DZEXTriangleWrapper> wrappers;
            if (index % 2 == 1)
                wrappers = _triangle2FirstWrappers;
            else
                wrappers = _triangle2SecondWrappers;

            if (wrappers.ContainsKey(triangle))
                return wrappers[triangle];

            F3DZEXTriangleWrapper wrapper = new F3DZEXTriangleWrapper(triangle, index);
            wrappers.Add(triangle, wrapper);

            return wrapper;
        }
    }
}
