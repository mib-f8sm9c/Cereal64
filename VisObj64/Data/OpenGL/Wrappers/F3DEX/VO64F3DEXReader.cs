using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DEX.DataElements;
using Cereal64.Microcodes.F3DEX.DataElements.Commands;
using Cereal64.Common.Rom;
using Cereal64.Common.DataElements;
using Cereal64.Common.Utils;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DEX
{
    /// <summary>
    /// Reads in a F3DEXCommand block, creates a list of wrappers to be displayed
    /// </summary>
    public static class VO64F3DEXReader
    {
        private static Stack<F3DEXCommandCollection> _commandStack = new Stack<F3DEXCommandCollection>();
        private static Stack<VO64GraphicsCollection> _collectionStack = new Stack<VO64GraphicsCollection>();
        private static Stack<int> _indexStack = new Stack<int>();

        private static F3DEXGraphicsElement CurrentElement { get { if (_element == null) _element = F3DEXGraphicsElement.CreateNewElement(); return _element; } }
        private static F3DEXGraphicsElement _element;
        private static bool CurrentElementExists() { return _element != null; }
        private static void ResetCurrentElement() { _element = null; }

        private static Dictionary<uint, VO64GraphicsCollection> _loadedCollections = new Dictionary<uint,VO64GraphicsCollection>();

        public static VO64GraphicsCollection ReadCommands(F3DEXCommandCollection commands, int index, DmaAddress address = null)
        {
            //Set up the VO64 graphics
            VO64GraphicsCollection collection = null;// = new VO64GraphicsCollection();

            F3DEXImage lastImage = null;
            F3DEX_G_SetTile lastSetTile = null;
            F3DEX_G_Texture lastTextureCommand = null;

            List<uint> vertexOffsetList = new List<uint>(); //Keeps track of which vertices are loaded where
            List<Vertex> vertices = new List<Vertex>();
            F3DEXVertexWrapper newVertex = null;
            F3DEXTextureWrapper newTexture = null;
            bool recordTileCommands = true; //Use RDPLoadSync and RDPTileSync to determine when it's okay to pick up SetTile commands
            //Note: Not guaranteed for all ways of using F3DEX!!

            _commandStack.Clear();
            _indexStack.Clear();
            _collectionStack.Clear();

            _commandStack.Push(commands);
            _indexStack.Push(index);
            if(address == null)
                _collectionStack.Push(new VO64GraphicsCollection());
            else
                _collectionStack.Push(new VO64GraphicsCollection(ByteHelper.DisplayValue(address.GetAsUInt(), true, true)));

            bool finished = false;

            while (_commandStack.Count > 0)
            {
                commands = _commandStack.Pop();
                index = _indexStack.Pop();

                //save the element
                if (CurrentElementExists() && collection != null)
                {
                    //if (CurrentElement.Commands.Count > 1)// !CurrentElement.IsEmpty || CurrentElement.Texture != null)
                    {
                        collection.Add(CurrentElement);
                    }
                    ResetCurrentElement();
                    if (newTexture != null)
                        CurrentElement.SetTexture(newTexture);
                }
                vertices.Clear();

                VO64GraphicsCollection oldCollection = _collectionStack.Pop();
                if (collection != null)
                    oldCollection.Add(collection);
                collection = oldCollection;

                finished = false;

                for (; index < commands.Commands.Count; index++)
                {
                    if (finished)
                        break;

                    F3DEXCommand command = commands.Commands[index];

                    if (CurrentElementExists())
                        CurrentElement.Commands.Add(command);

                    switch (command.CommandID)
                    {
                        case F3DEXCommandID.F3DEX_G_RDPLOADSYNC:
                            recordTileCommands = true;
                            break;
                        case F3DEXCommandID.F3DEX_G_RDPTILESYNC:
                            recordTileCommands = true;
                            break;
                        case F3DEXCommandID.F3DEX_G_LOADBLOCK:
                            if (((F3DEX_G_LoadBlock)command).ImageReference != null &&
                                lastImage != ((F3DEX_G_LoadBlock)command).ImageReference)
                            {
                                //save the element

                                if (CurrentElementExists())
                                {
                                    //if (CurrentElement.Commands.Count > 1)// !CurrentElement.IsEmpty || CurrentElement.Texture != null)
                                    {
                                        collection.Add(CurrentElement);
                                    }
                                    ResetCurrentElement();
                                    if (newTexture != null)
                                        CurrentElement.SetTexture(newTexture);
                                }
                                vertices.Clear();

                                //Set the texture here
                                lastImage = ((F3DEX_G_LoadBlock)command).ImageReference;
                                newTexture = new F3DEXTextureWrapper(lastImage, lastSetTile, lastTextureCommand);
                                CurrentElement.SetTexture(newTexture);
                            }
                            break;
                        case F3DEXCommandID.F3DEX_G_DL: //ignore this one for now
                            F3DEX_G_DL dlCommand = (F3DEX_G_DL)command;

                            int offset;
                            RomFile file;

                            RomProject.Instance.FindRamOffset(dlCommand.DLAddress, out file, out offset);

                            N64DataElement el;

                            if (_loadedCollections.ContainsKey(dlCommand.DLAddress.GetAsUInt()))
                            {
                                collection.Add(_loadedCollections[dlCommand.DLAddress.GetAsUInt()]);
                                if (CurrentElementExists())
                                {
                                    //if(CurrentElement.Commands.Count > 1)
                                        collection.Add(CurrentElement);
                                    ResetCurrentElement();

                                    F3DEXTextureWrapper lastUsedTextureWrapper = GetLastTextureWrapperFor(
                                        _loadedCollections[dlCommand.DLAddress.GetAsUInt()]);
                                    if (lastUsedTextureWrapper != null)
                                    {
                                        CurrentElement.SetTexture(lastUsedTextureWrapper);
                                        newTexture = lastUsedTextureWrapper;
                                    }
                                    else if (newTexture != null)
                                        CurrentElement.SetTexture(newTexture);
                                    else
                                        System.Windows.Forms.MessageBox.Show("Error");
                                }
                            }
                            else if (file.HasElementAt(offset, out el) && el is F3DEXCommandCollection)
                            {
                                int fileOffset = (offset - el.FileOffset - 1) / 0x8; //-1 so it increments correctly at the for loop
                                _commandStack.Push(commands);
                                _indexStack.Push(index + 1);
                                commands = (F3DEXCommandCollection)el;
                                index = fileOffset;
                                //collection.Add(ReadCommands((F3DEXCommandCollection)el, fileOffset));

                                _collectionStack.Push(collection);
                                if (CurrentElementExists())
                                {
                                    //if(CurrentElement.Commands.Count > 1)
                                        collection.Add(CurrentElement);
                                    ResetCurrentElement();
                                    if (newTexture != null)
                                        CurrentElement.SetTexture(newTexture);
                                }
                                collection = new VO64GraphicsCollection(ByteHelper.DisplayValue(dlCommand.DLAddress.GetAsUInt(), true, true));
                                _loadedCollections.Add(dlCommand.DLAddress.GetAsUInt(), collection);
                            }

                            if (dlCommand.ForceJump)
                            {
                                _commandStack.Clear();
                                _indexStack.Clear();
                                //RIGHT NOW THIS BREAKS THE READER. AVOID!!!
                            }
                            break;
                        case F3DEXCommandID.F3DEX_G_SETTILE:
                            if (((F3DEX_G_SetTile)command).Line != 0)//recordTileCommands)
                            {
                                //keep track of this command when setting up the texture
                                lastSetTile = (F3DEX_G_SetTile)command;
                                recordTileCommands = true;
                                if (newTexture != null && newTexture.SetTileCommand == null)
                                    newTexture.SetTileCommand = lastSetTile;
                            }
                            break;
                        case F3DEXCommandID.F3DEX_G_TEXTURE:
                            lastTextureCommand = (F3DEX_G_Texture)command;
                            if (newTexture != null && newTexture.TextureCommand == null)
                                newTexture.TextureCommand = lastTextureCommand;
                            if (lastTextureCommand.TurnOn == 0 && lastTextureCommand.Tile == 0)
                                newTexture = null;
                            break;
                        case F3DEXCommandID.F3DEX_G_VTX:
                            //Record the vertex offset here, to keep track of vertex counts
                            F3DEX_G_Vtx vtxCommand = (F3DEX_G_Vtx)command;
                            break;
                        case F3DEXCommandID.F3DEX_G_TRI1:
                            F3DEX_G_Tri1 tri = (F3DEX_G_Tri1)command;
                            if (tri.Vertex1Reference != null && tri.Vertex2Reference != null && tri.Vertex3Reference != null)
                            {
                                if (!vertices.Contains(tri.Vertex1Reference))
                                {
                                    vertices.Add(tri.Vertex1Reference);
                                    newVertex = new F3DEXVertexWrapper(tri.Vertex1Reference);
                                    newVertex.SetTextureProperties(newTexture);
                                    CurrentElement.AddVertex(newVertex);
                                }
                                if (!vertices.Contains(tri.Vertex2Reference))
                                {
                                    vertices.Add(tri.Vertex2Reference);
                                    newVertex = new F3DEXVertexWrapper(tri.Vertex2Reference);
                                    newVertex.SetTextureProperties(newTexture);
                                    CurrentElement.AddVertex(newVertex);
                                }
                                if (!vertices.Contains(tri.Vertex3Reference))
                                {
                                    vertices.Add(tri.Vertex3Reference);
                                    newVertex = new F3DEXVertexWrapper(tri.Vertex3Reference);
                                    newVertex.SetTextureProperties(newTexture);
                                    CurrentElement.AddVertex(newVertex);
                                }

                                VO64SimpleTriangle triangle = new VO64SimpleTriangle((ushort)vertices.IndexOf(tri.Vertex1Reference),
                                    (ushort)vertices.IndexOf(tri.Vertex2Reference),
                                        (ushort)vertices.IndexOf(tri.Vertex3Reference));//new F3DEXTriangleWrapper((F3DEX_G_Tri1)command);

                                CurrentElement.AddTriangle(triangle);
                            }

                            break;
                        case F3DEXCommandID.F3DEX_G_TRI2:

                            F3DEX_G_Tri2 tri2 = (F3DEX_G_Tri2)command;

                            if (tri2.Vertex1Reference != null && tri2.Vertex2Reference != null && tri2.Vertex3Reference != null)
                            {
                                if (!vertices.Contains(tri2.Vertex1Reference))
                                {
                                    vertices.Add(tri2.Vertex1Reference);
                                    newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex1Reference);
                                    newVertex.SetTextureProperties(newTexture);
                                    CurrentElement.AddVertex(newVertex);
                                }
                                if (!vertices.Contains(tri2.Vertex2Reference))
                                {
                                    vertices.Add(tri2.Vertex2Reference);
                                    newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex2Reference);
                                    newVertex.SetTextureProperties(newTexture);
                                    CurrentElement.AddVertex(newVertex);
                                }
                                if (!vertices.Contains(tri2.Vertex3Reference))
                                {
                                    vertices.Add(tri2.Vertex3Reference);
                                    newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex3Reference);
                                    newVertex.SetTextureProperties(newTexture);
                                    CurrentElement.AddVertex(newVertex);
                                }

                                VO64SimpleTriangle triangle2 = new VO64SimpleTriangle((ushort)vertices.IndexOf(tri2.Vertex1Reference),
                                    (ushort)vertices.IndexOf(tri2.Vertex2Reference),
                                        (ushort)vertices.IndexOf(tri2.Vertex3Reference));//new F3DEXTriangleWrapper((F3DEX_G_Tri1)command);

                                CurrentElement.AddTriangle(triangle2);
                            }

                            if (tri2.Vertex4Reference != null && tri2.Vertex5Reference != null && tri2.Vertex6Reference != null)
                            {
                                if (!vertices.Contains(tri2.Vertex4Reference))
                                {
                                    vertices.Add(tri2.Vertex4Reference);
                                    newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex4Reference);
                                    newVertex.SetTextureProperties(newTexture);
                                    CurrentElement.AddVertex(newVertex);
                                }
                                if (!vertices.Contains(tri2.Vertex5Reference))
                                {
                                    vertices.Add(tri2.Vertex5Reference);
                                    newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex5Reference);
                                    newVertex.SetTextureProperties(newTexture);
                                    CurrentElement.AddVertex(newVertex);
                                }
                                if (!vertices.Contains(tri2.Vertex6Reference))
                                {
                                    vertices.Add(tri2.Vertex6Reference);
                                    newVertex = F3DEXWrapperBank.GetVertexWrapper(tri2.Vertex6Reference);
                                    newVertex.SetTextureProperties(newTexture);
                                    CurrentElement.AddVertex(newVertex);
                                }

                                VO64SimpleTriangle triangle2 = new VO64SimpleTriangle((ushort)vertices.IndexOf(tri2.Vertex4Reference),
                                    (ushort)vertices.IndexOf(tri2.Vertex5Reference),
                                        (ushort)vertices.IndexOf(tri2.Vertex6Reference));//new F3DEXTriangleWrapper((F3DEX_G_Tri1)command);

                                CurrentElement.AddTriangle(triangle2);
                            }

                            break;
                        case F3DEXCommandID.F3DEX_G_ENDDL:
                        case F3DEXCommandID.F3DEX_G_MK64_ENDDL:
                            finished = true;
                            break;
                    }
                }
            }

            if (CurrentElementExists())
            {
                //if (CurrentElement.IsEmpty)
                //    VO64GraphicsElement.ReleaseElement(CurrentElement);
                //else
                //if (CurrentElement.Commands.Count > 1)
                    collection.Add(CurrentElement);
            }

            return collection;
        }

        private static F3DEXTextureWrapper GetLastTextureWrapperFor(VO64GraphicsCollection coll)
        {
            //Emphasize elements over collections
            for (int i = coll.Elements.Count - 1; i >= 0; i--)
            {
                if (coll.Elements[i].Texture != null && coll.Elements[i].Texture is F3DEXTextureWrapper)
                {
                    return (F3DEXTextureWrapper)coll.Elements[i].Texture;
                }
            }

            for (int i = coll.Collections.Count - 1; i >= 0; i--)
            {
                F3DEXTextureWrapper texture = GetLastTextureWrapperFor(coll.Collections[i]);
                if (texture != null)
                    return texture;
            }
            return null;
        }
    }
}
