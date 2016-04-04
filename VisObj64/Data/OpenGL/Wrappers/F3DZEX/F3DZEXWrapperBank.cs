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

        private static Dictionary<Guid, F3DZEXTextureWrapper> _textureWrappers = new Dictionary<Guid, F3DZEXTextureWrapper>();
        private static Dictionary<Guid, F3DZEXVertexWrapper> _vertexWrappers = new Dictionary<Guid, F3DZEXVertexWrapper>();
        private static Dictionary<Guid, F3DZEXTriangleWrapper> _triangleWrappers = new Dictionary<Guid, F3DZEXTriangleWrapper>();
        private static Dictionary<Guid, F3DZEXTriangleWrapper> _triangle2Wrappers = new Dictionary<Guid, F3DZEXTriangleWrapper>();

        public static F3DZEXTextureWrapper GetTextureWrapper(Texture texture, F3DZEX_G_SetTile command)
        {
            if (_textureWrappers.ContainsKey(texture.GUID))
                return _textureWrappers[texture.GUID];

            F3DZEXTextureWrapper wrapper = new F3DZEXTextureWrapper(texture, command);
            _textureWrappers.Add(texture.GUID, wrapper);

            return wrapper;
        }

        public static F3DZEXVertexWrapper GetVertexWrapper(Vertex vertex)
        {
            if (_vertexWrappers.ContainsKey(vertex.GUID))
                return _vertexWrappers[vertex.GUID];

            F3DZEXVertexWrapper wrapper = new F3DZEXVertexWrapper(vertex);
            _vertexWrappers.Add(vertex.GUID, wrapper);

            return wrapper;
        }

        public static F3DZEXTriangleWrapper GetTriangleWrapper(F3DZEX_G_Tri1 triangle)
        {
            if (_triangleWrappers.ContainsKey(triangle.GUID))
                return _triangleWrappers[triangle.GUID];

            F3DZEXTriangleWrapper wrapper = new F3DZEXTriangleWrapper(triangle);
            _triangleWrappers.Add(triangle.GUID, wrapper);

            return wrapper;
        }

        public static F3DZEXTriangleWrapper GetTriangleWrapper(F3DZEX_G_Tri2 triangle, int index)
        {
            Dictionary<Guid, F3DZEXTriangleWrapper> wrappers;
            if (index % 2 == 1)
                wrappers = _triangleWrappers;
            else
                wrappers = _triangle2Wrappers;

            if (wrappers.ContainsKey(triangle.GUID))
                return wrappers[triangle.GUID];

            F3DZEXTriangleWrapper wrapper = new F3DZEXTriangleWrapper(triangle, index);
            wrappers.Add(triangle.GUID, wrapper);

            return wrapper;
        }
    }
}
