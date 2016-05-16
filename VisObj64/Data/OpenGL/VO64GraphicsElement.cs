using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace Cereal64.VisObj64.Data.OpenGL
{
    public class VO64GraphicsElement
    {
        private int _vaoIndex; //vertex array object (handy handle for everything)
        private int _vboIndex; //vertex buffer object (vertex information)
        private int _iboIndex; //index buffer object (references to indexes for the triangles

        private IVO64Texture _texture;
        private uint _textureID;

        //Texture information goes here too

        private List<IVO64Triangle> _indices;
        private List<IVO64Vertex> _vertices;

        private bool _updatedVertices = true;
        private bool _updatedIndices = true;
        private bool _updatedTexture = true;

        public bool IsEmpty { get { return _vertices.Count == 0 && _indices.Count == 0; } }

        public bool Selected;

        private VO64GraphicsElement(int vaIndex, int vbIndex, int ibIndex)
        {
            _vertices = new List<IVO64Vertex>();
            _indices = new List<IVO64Triangle>();

            _vaoIndex = vaIndex;
            _vboIndex = vbIndex;
            _iboIndex = ibIndex;

            _texture = null;

            Selected = false;
        }

        public static VO64GraphicsElement CreateNewElement()
        {
            int newvaIndex, newvbIndex, newibIndex;
            newvaIndex = GL.GenVertexArray();
            GL.GenBuffers(1, out newvbIndex);
            GL.GenBuffers(1, out newibIndex);

            return new VO64GraphicsElement(newvaIndex, newvbIndex, newibIndex);
        }

        public void AddVertex(IVO64Vertex vertex)
        {
            _vertices.Add(vertex);
            _updatedVertices = true;
        }

        public void AddTriangle(IVO64Triangle triangle)
        {
            _indices.Add(triangle);
            _updatedIndices = true;
        }

        public void SetTexture(IVO64Texture texture)
        {
            _texture = texture;
            _updatedTexture = true;
        }

        public void UpdateBuffers()
        {
            if (_updatedTexture)
            {
                if (_texture != null && _texture.Texture != null)
                {
                    BitmapData bmp_data = _texture.Texture.LockBits(new Rectangle(0, 0, _texture.Texture.Width, _texture.Texture.Height),
                        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.GenTextures(1, out _textureID);
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, _textureID);
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
                    //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)_texture.MinFilter);
                    //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)_texture.MagFilter);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)_texture.WrapS);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)_texture.WrapT);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                    _texture.Texture.UnlockBits(bmp_data);

                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.Disable(EnableCap.Texture2D);
                }
                else
                    _textureID = 0;
                _updatedTexture = false;
            }

            if (_updatedVertices || _updatedIndices)
            {
                if (_updatedVertices)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIndex);

                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertices.Count * VO64SimpleVertex.Size), _vertices.ConvertAll<VO64SimpleVertex>(v => v.GetAsSimpleVertex()).ToArray(), BufferUsageHint.StaticDraw);

                    VO64SimpleVertex.SetOpenGLVertexFormat();

                    int size;
                    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
                    if (_vertices.Count * VO64SimpleVertex.Size != size)
                        throw new ApplicationException("Index array not uploaded correctly");
                    
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                    for (int i = 0; i < _vertices.Count; i++)
                    {
                        _vertices[i].SetVBOReference((uint)_vboIndex, VO64SimpleVertex.Size * i);
                    }

                    _updatedVertices = false;
                }
                if (_updatedIndices)
                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _iboIndex);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_indices.Count * VO64SimpleTriangle.Size), _indices.ConvertAll<VO64SimpleTriangle>(v => v.GetAsSimpleTriangle()).ToArray(), BufferUsageHint.StaticDraw);

                    int size;
                    GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
                    if (_indices.Count * VO64SimpleTriangle.Size != size)
                        throw new ApplicationException("Index array not uploaded correctly");

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                    _updatedIndices = false;
                }

                //Update the vao
                GL.BindVertexArray(_vaoIndex);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIndex);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _iboIndex);

                VO64SimpleVertex.SetOpenGLVertexFormat();

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

        }

        public void Draw()
        {
            //Double check the buffers are up to date
            UpdateBuffers();

            //Draw
            GL.BindVertexArray(_vaoIndex);

            GL.EnableVertexAttribArray(0);

            if (_textureID != 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, _textureID);

                GL.Enable(EnableCap.Texture2D);
            }

            GL.DrawElements(PrimitiveType.Triangles, _indices.Count * 3, DrawElementsType.UnsignedShort, IntPtr.Zero);

            if (_textureID != 0)
                GL.Disable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.DisableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            //Remove from OpenGL
            GL.DeleteBuffer(_vboIndex);
            GL.DeleteBuffer(_iboIndex);

            //TO DO: RESET ALL THE VERTEX VBO REFERENCES
        }
    }
}
