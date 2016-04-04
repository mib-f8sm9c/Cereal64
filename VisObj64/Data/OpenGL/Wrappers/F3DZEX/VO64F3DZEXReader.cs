using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DZEX.DataElements;
using Cereal64.Microcodes.F3DZEX.DataElements.Commands;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DZEX
{
    /// <summary>
    /// Reads in a F3DZEXCommand block, creates a list of wrappers to be displayed
    /// </summary>
    public static class VO64F3DZEXReader
    {
        public static VO64GraphicsCollection ReadCommands(F3DZEXCommandCollection commands)
        {
            //Set up the VO64 graphics
            VO64GraphicsCollection collection = new VO64GraphicsCollection();
            VO64GraphicsElement element = VO64GraphicsElement.CreateNewElement();

            Texture lastTexture = null;
            F3DZEX_G_SetTile lastSetTile = null;

            List<uint> vertexOffsetList = new List<uint>(); //Keeps track of which vertices are loaded where
            List<Vertex> vertices = new List<Vertex>();

            foreach(F3DZEXCommand command in commands.Commands)
            {
                switch (command.CommandID)
                {
                    case F3DZEXCommandID.F3DZEX_G_DL: //ignore this one for now
                        break;
                    case F3DZEXCommandID.F3DZEX_G_SETTILE:
                        //keep track of this command when setting up the texture
                        lastSetTile = (F3DZEX_G_SetTile)command;
                        break;
                    case F3DZEXCommandID.F3DZEX_G_VTX:
                        //Record the vertex offset here, to keep track of vertex counts
                        F3DZEX_G_Vtx vtxCommand = (F3DZEX_G_Vtx)command;
                        break;
                    case F3DZEXCommandID.F3DZEX_G_TRI1:

                        if (((F3DZEX_G_Tri2)command).TextureReference != null &&
                            lastTexture != ((F3DZEX_G_Tri2)command).TextureReference)
                        {
                            //save the element
                            if (!element.IsEmpty)
                            {
                                collection.Add(element);
                                element = VO64GraphicsElement.CreateNewElement();
                            }
                            vertices.Clear();

                            //Set the texture here
                            lastTexture = ((F3DZEX_G_Tri2)command).TextureReference;
                            element.SetTexture(new F3DZEXTextureWrapper(lastTexture, lastSetTile));
                        }

                        F3DZEX_G_Tri1 tri = (F3DZEX_G_Tri1)command;
                        if (!vertices.Contains(tri.Vertex1Reference))
                        {
                            vertices.Add(tri.Vertex1Reference);
                        }
                            element.AddVertex(new F3DZEXVertexWrapper(tri.Vertex1Reference));
                        if (!vertices.Contains(tri.Vertex2Reference))
                        {
                            vertices.Add(tri.Vertex2Reference);
                        }
                            element.AddVertex(new F3DZEXVertexWrapper(tri.Vertex2Reference));
                        if (!vertices.Contains(tri.Vertex3Reference))
                        {
                            vertices.Add(tri.Vertex3Reference);
                        }
                            element.AddVertex(new F3DZEXVertexWrapper(tri.Vertex3Reference));

                        VO64SimpleTriangle triangle = new VO64SimpleTriangle((ushort)vertices.IndexOf(tri.Vertex1Reference),
                            (ushort)vertices.IndexOf(tri.Vertex2Reference),
                                (ushort)vertices.IndexOf(tri.Vertex3Reference));//new F3DZEXTriangleWrapper((F3DZEX_G_Tri1)command);

                        element.AddTriangle(triangle);

                        break;
                    case F3DZEXCommandID.F3DZEX_G_TRI2:

                        if (((F3DZEX_G_Tri2)command).TextureReference != null &&
                            lastTexture != ((F3DZEX_G_Tri2)command).TextureReference)
                        {
                            //save the element
                            if (!element.IsEmpty)
                            {
                                collection.Add(element);
                                element = VO64GraphicsElement.CreateNewElement();
                            }
                            vertices.Clear();

                            //Set the texture here
                            lastTexture = ((F3DZEX_G_Tri2)command).TextureReference;
                            element.SetTexture(new F3DZEXTextureWrapper(lastTexture, lastSetTile));
                        }

                        F3DZEX_G_Tri2 tri2 = (F3DZEX_G_Tri2)command;
                        if (!vertices.Contains(tri2.Vertex1Reference))
                        {
                            vertices.Add(tri2.Vertex1Reference);
                        }
                        element.AddVertex(new F3DZEXVertexWrapper(tri2.Vertex1Reference));
                        if (!vertices.Contains(tri2.Vertex2Reference))
                        {
                            vertices.Add(tri2.Vertex2Reference);
                        }
                        element.AddVertex(new F3DZEXVertexWrapper(tri2.Vertex2Reference));
                        if (!vertices.Contains(tri2.Vertex3Reference))
                        {
                            vertices.Add(tri2.Vertex3Reference);
                        }
                        element.AddVertex(new F3DZEXVertexWrapper(tri2.Vertex3Reference));

                        VO64SimpleTriangle triangle2 = new VO64SimpleTriangle((ushort)vertices.IndexOf(tri2.Vertex1Reference),
                            (ushort)vertices.IndexOf(tri2.Vertex2Reference),
                                (ushort)vertices.IndexOf(tri2.Vertex3Reference));//new F3DZEXTriangleWrapper((F3DZEX_G_Tri1)command);

                        element.AddTriangle(triangle2);

                        
                        if (!vertices.Contains(tri2.Vertex4Reference))
                        {
                            vertices.Add(tri2.Vertex4Reference);
                        }
                            element.AddVertex(new F3DZEXVertexWrapper(tri2.Vertex4Reference));
                        if (!vertices.Contains(tri2.Vertex5Reference))
                        {
                            vertices.Add(tri2.Vertex5Reference);
                        }
                            element.AddVertex(new F3DZEXVertexWrapper(tri2.Vertex5Reference));
                        if (!vertices.Contains(tri2.Vertex6Reference))
                        {
                            vertices.Add(tri2.Vertex6Reference);
                        }
                            element.AddVertex(new F3DZEXVertexWrapper(tri2.Vertex6Reference));

                        triangle2 = new VO64SimpleTriangle((ushort)vertices.IndexOf(tri2.Vertex4Reference),
                            (ushort)vertices.IndexOf(tri2.Vertex5Reference),
                                (ushort)vertices.IndexOf(tri2.Vertex6Reference));//new F3DZEXTriangleWrapper((F3DZEX_G_Tri1)command);

                        element.AddTriangle(triangle2);

                        break;
                }
            }

            collection.Add(element);

            return collection;
        }
    }
}
