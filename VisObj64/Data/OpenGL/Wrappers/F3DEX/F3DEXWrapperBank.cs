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

        private static Dictionary<Guid, F3DEXTextureWrapper> _textureWrappers = new Dictionary<Guid, F3DEXTextureWrapper>();
        private static Dictionary<Guid, F3DEXVertexWrapper> _vertexWrappers = new Dictionary<Guid, F3DEXVertexWrapper>();
        private static Dictionary<Guid, F3DEXTriangleWrapper> _triangleWrappers = new Dictionary<Guid, F3DEXTriangleWrapper>();
        private static Dictionary<Guid, F3DEXTriangleWrapper> _triangle2Wrappers = new Dictionary<Guid, F3DEXTriangleWrapper>();

        public static F3DEXTextureWrapper GetTextureWrapper(Texture texture, F3DEX_G_SetTile command, F3DEX_G_Texture textureCommand)
        {
            if (_textureWrappers.ContainsKey(texture.GUID))
                return _textureWrappers[texture.GUID];

            F3DEXTextureWrapper wrapper = new F3DEXTextureWrapper(texture, command, textureCommand);
            _textureWrappers.Add(texture.GUID, wrapper);

            return wrapper;
        }

        public static F3DEXVertexWrapper GetVertexWrapper(Vertex vertex)
        {
            if (_vertexWrappers.ContainsKey(vertex.GUID))
                return _vertexWrappers[vertex.GUID];

            F3DEXVertexWrapper wrapper = new F3DEXVertexWrapper(vertex);
            _vertexWrappers.Add(vertex.GUID, wrapper);

            return wrapper;
        }

        public static F3DEXTriangleWrapper GetTriangleWrapper(F3DEX_G_Tri1 triangle)
        {
            if (_triangleWrappers.ContainsKey(triangle.GUID))
                return _triangleWrappers[triangle.GUID];

            F3DEXTriangleWrapper wrapper = new F3DEXTriangleWrapper(triangle);
            _triangleWrappers.Add(triangle.GUID, wrapper);

            return wrapper;
        }

        public static F3DEXTriangleWrapper GetTriangleWrapper(F3DEX_G_Tri2 triangle, int index)
        {
            Dictionary<Guid, F3DEXTriangleWrapper> wrappers;
            if (index % 2 == 1)
                wrappers = _triangleWrappers;
            else
                wrappers = _triangle2Wrappers;

            if (wrappers.ContainsKey(triangle.GUID))
                return wrappers[triangle.GUID];

            F3DEXTriangleWrapper wrapper = new F3DEXTriangleWrapper(triangle, index);
            wrappers.Add(triangle.GUID, wrapper);

            return wrapper;
        }
    }
}
