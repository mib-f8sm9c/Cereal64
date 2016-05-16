using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DEX.DataElements;
using Cereal64.Microcodes.F3DEX.DataElements.Commands;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DEX
{
    /// <summary>
    /// Reads in a F3DEXCommand block, creates a list of wrappers to be displayed
    /// </summary>
    public static class VO64F3DEXReader
    {
        public static VO64GraphicsCollection ReadCommands(F3DEXCommandCollection commands)
        {
            //Set up the VO64 graphics
            VO64GraphicsCollection collection = new VO64GraphicsCollection();
            VO64GraphicsElement element = VO64GraphicsElement.CreateNewElement();

            Texture lastTexture = null;
            F3DEX_G_SetTile lastSetTile = null;
            F3DEX_G_Texture lastTextureCommand = null;

            List<uint> vertexOffsetList = new List<uint>(); //Keeps track of which vertices are loaded where
            List<Vertex> vertices = new List<Vertex>();
            F3DEXVertexWrapper newVertex = null;
            F3DEXTextureWrapper newTexture = null;
            bool recordTileCommands = true; //Use RDPLoadSync and RDPTileSync to determine when it's okay to pick up SetTile commands
            //Note: Not guaranteed for all ways of using F3DEX!!

            foreach(F3DEXCommand command in commands.Commands)
            {
                switch (command.CommandID)
                {
                    case F3DEXCommandID.F3DEX_G_RDPLOADSYNC:
                        recordTileCommands = true;
                        break;
                    case F3DEXCommandID.F3DEX_G_RDPTILESYNC:
                        recordTileCommands = true;
                        break;
                    case F3DEXCommandID.F3DEX_G_DL: //ignore this one for now
                        break;
                    case F3DEXCommandID.F3DEX_G_SETTILE:
                        if (((F3DEX_G_SetTile)command).Line != 0)//recordTileCommands)
                        {
                            //keep track of this command when setting up the texture
                            lastSetTile = (F3DEX_G_SetTile)command;
                            recordTileCommands = true;
                        }
                        break;
                    case F3DEXCommandID.F3DEX_G_TEXTURE:
                        lastTextureCommand = (F3DEX_G_Texture)command;
                        break;
                    case F3DEXCommandID.F3DEX_G_VTX:
                        //Record the vertex offset here, to keep track of vertex counts
                        F3DEX_G_Vtx vtxCommand = (F3DEX_G_Vtx)command;
                        break;
                    case F3DEXCommandID.F3DEX_G_TRI1:

                        if (((F3DEX_G_Tri1)command).TextureReference != null &&
                            lastTexture != ((F3DEX_G_Tri1)command).TextureReference)
                        {
                            //save the element
                            if (!element.IsEmpty)
                            {
                                collection.Add(element);
                                element = VO64GraphicsElement.CreateNewElement();
                            }
                            vertices.Clear();

                            //Set the texture here
                            lastTexture = ((F3DEX_G_Tri1)command).TextureReference;
                            newTexture = new F3DEXTextureWrapper(lastTexture, lastSetTile, lastTextureCommand);
                            element.SetTexture(newTexture);
                        }

                        F3DEX_G_Tri1 tri = (F3DEX_G_Tri1)command;
                        if (tri.Vertex1Reference != null && tri.Vertex2Reference != null && tri.Vertex3Reference != null)
                        {
                            if (!vertices.Contains(tri.Vertex1Reference))
                            {
                                vertices.Add(tri.Vertex1Reference);
                                newVertex = new F3DEXVertexWrapper(tri.Vertex1Reference);
                                newVertex.SetTextureProperties(newTexture);
                                element.AddVertex(newVertex);
                            }
                            if (!vertices.Contains(tri.Vertex2Reference))
                            {
                                vertices.Add(tri.Vertex2Reference);
                                newVertex = new F3DEXVertexWrapper(tri.Vertex2Reference);
                                newVertex.SetTextureProperties(newTexture);
                                element.AddVertex(newVertex);
                            }
                            if (!vertices.Contains(tri.Vertex3Reference))
                            {
                                vertices.Add(tri.Vertex3Reference);
                                newVertex = new F3DEXVertexWrapper(tri.Vertex3Reference);
                                newVertex.SetTextureProperties(newTexture);
                                element.AddVertex(newVertex);
                            }

                            VO64SimpleTriangle triangle = new VO64SimpleTriangle((ushort)vertices.IndexOf(tri.Vertex1Reference),
                                (ushort)vertices.IndexOf(tri.Vertex2Reference),
                                    (ushort)vertices.IndexOf(tri.Vertex3Reference));//new F3DEXTriangleWrapper((F3DEX_G_Tri1)command);

                            element.AddTriangle(triangle);
                        }

                        break;
                    case F3DEXCommandID.F3DEX_G_TRI2:

                        if (((F3DEX_G_Tri2)command).TextureReference != null &&
                            lastTexture != ((F3DEX_G_Tri2)command).TextureReference)
                        {
                            //save the element
                            if (!element.IsEmpty)
                            {
                                collection.Add(element);
                                element = VO64GraphicsElement.CreateNewElement();
                            }
                            vertices.Clear();

                            //Set the texture here
                            lastTexture = ((F3DEX_G_Tri2)command).TextureReference;
                            newTexture = F3DEXWrapperBank.GetTextureWrapper(lastTexture, lastSetTile, lastTextureCommand);
                            element.SetTexture(newTexture);
                        }

                        F3DEX_G_Tri2 tri2 = (F3DEX_G_Tri2)command;

                        if (tri2.Vertex1Reference != null && tri2.Vertex2Reference != null && tri2.Vertex3Reference != null)
                        {
                            if (!vertices.Contains(tri2.Vertex1Reference))
                            {
                                vertices.Add(tri2.Vertex1Reference);
                                newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex1Reference);
                                newVertex.SetTextureProperties(newTexture);
                                element.AddVertex(newVertex);
                            }
                            if (!vertices.Contains(tri2.Vertex2Reference))
                            {
                                vertices.Add(tri2.Vertex2Reference);
                                newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex2Reference);
                                newVertex.SetTextureProperties(newTexture);
                                element.AddVertex(newVertex);
                            }
                            if (!vertices.Contains(tri2.Vertex3Reference))
                            {
                                vertices.Add(tri2.Vertex3Reference);
                                newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex3Reference);
                                newVertex.SetTextureProperties(newTexture);
                                element.AddVertex(newVertex);
                            }

                            VO64SimpleTriangle triangle2 = new VO64SimpleTriangle((ushort)vertices.IndexOf(tri2.Vertex1Reference),
                                (ushort)vertices.IndexOf(tri2.Vertex2Reference),
                                    (ushort)vertices.IndexOf(tri2.Vertex3Reference));//new F3DEXTriangleWrapper((F3DEX_G_Tri1)command);

                            element.AddTriangle(triangle2);
                        }

                        if (tri2.Vertex4Reference != null && tri2.Vertex5Reference != null && tri2.Vertex6Reference != null)
                        {
                            if (!vertices.Contains(tri2.Vertex4Reference))
                            {
                                vertices.Add(tri2.Vertex4Reference);
                                newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex4Reference);
                                newVertex.SetTextureProperties(newTexture);
                                element.AddVertex(newVertex);
                            }
                            if (!vertices.Contains(tri2.Vertex5Reference))
                            {
                                vertices.Add(tri2.Vertex5Reference);
                                newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex5Reference);
                                newVertex.SetTextureProperties(newTexture);
                                element.AddVertex(newVertex);
                            }
                            if (!vertices.Contains(tri2.Vertex6Reference))
                            {
                                vertices.Add(tri2.Vertex6Reference);
                                newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex6Reference);
                                newVertex.SetTextureProperties(newTexture);
                                element.AddVertex(newVertex);
                            }

                            VO64SimpleTriangle triangle2 = new VO64SimpleTriangle((ushort)vertices.IndexOf(tri2.Vertex4Reference),
                                (ushort)vertices.IndexOf(tri2.Vertex5Reference),
                                    (ushort)vertices.IndexOf(tri2.Vertex6Reference));//new F3DEXTriangleWrapper((F3DEX_G_Tri1)command);

                            element.AddTriangle(triangle2);
                        }

                        break;
                }
            }

            collection.Add(element);

            return collection;
        }
    }
}
