using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using System.Collections.ObjectModel;
using Cereal64.Microcodes.F3DEX.DataElements;
using System.Windows.Forms;
using Cereal64.Common.DataElements;
using Cereal64.Common.Rom;

namespace Cereal64.Microcodes.F3DEX
{
    public class F3DEXContainer : IN64ElementContainer
    {
        public ReadOnlyCollection<Palette> Palettes { get { return _palettes.AsReadOnly(); } }
        private List<Palette> _palettes;

        public ReadOnlyCollection<Texture> Textures { get { return _textures.AsReadOnly(); } }
        private List<Texture> _textures;

        public ReadOnlyCollection<F3DEXCommandCollection> CommandCollections { get { return _commandCollections.AsReadOnly(); } }
        private List<F3DEXCommandCollection> _commandCollections;
        
        public ReadOnlyCollection<VertexCollection> Vertices { get { return _vertices.AsReadOnly(); } }
        private List<VertexCollection> _vertices;

        public F3DEXContainer()
        {
            _palettes = new List<Palette>();
            _textures = new List<Texture>();
            _commandCollections = new List<F3DEXCommandCollection>();
            _vertices = new List<VertexCollection>();
        }

        public bool AddElement(N64DataElement element)
        {
            if (element is Palette)
            {
                if (_palettes.Contains(element))
                    return false;
                _palettes.Add((Palette)element);
                _palettes.Sort((e1, e2) => e1.FileOffset.CompareTo(e2.FileOffset));
            }
            else if (element is Texture)
            {
                if (_textures.Contains(element))
                    return false;
                _textures.Add((Texture)element);
                _textures.Sort((e1, e2) => e1.FileOffset.CompareTo(e2.FileOffset));
            }
            else if (element is F3DEXCommandCollection)
            {
                if (_commandCollections.Contains(element))
                    return false;
                _commandCollections.Add((F3DEXCommandCollection)element);
                _commandCollections.Sort((e1, e2) => e1.FileOffset.CompareTo(e2.FileOffset));
            }
            else if (element is VertexCollection)
            {
                if (_vertices.Contains(element))
                    return false;
                _vertices.Add((VertexCollection)element);
                _vertices.Sort((e1, e2) => e1.FileOffset.CompareTo(e2.FileOffset));
            }
            else
                return false;

            ReferencesNeedUpdating = true;
            return true;
        }

        public bool RemoveElement(N64DataElement element)
        {
            if (element is Palette)
            {
                if (!_palettes.Contains(element))
                    return false;
                _palettes.Remove((Palette)element);
            }
            else if (element is Texture)
            {
                if (!_textures.Contains(element))
                    return false;
                _textures.Remove((Texture)element);
            }
            else if (element is F3DEXCommandCollection)
            {
                if (!_commandCollections.Contains(element))
                    return false;
                _commandCollections.Remove((F3DEXCommandCollection)element);
            }
            else if (element is VertexCollection)
            {
                if (!_vertices.Contains(element))
                    return false;
                _vertices.Remove((VertexCollection)element);
            }
            else
                return false;

            return true;
        }

        public TreeNode GetAsTreeNode()
        {
            TreeNode F3DEXOverallNode = new TreeNode();
            F3DEXOverallNode.Text = "F3DEX Related Elements";
            F3DEXOverallNode.Tag = this;

            //Palettes
            TreeNode PalettesNode = new TreeNode();
            PalettesNode.Text = "Palettes";
            PalettesNode.Tag = _palettes;
            foreach (Palette palette in _palettes)
                PalettesNode.Nodes.Add(palette.GetAsTreeNode());
            F3DEXOverallNode.Nodes.Add(PalettesNode);

            //Textures
            TreeNode TexturesNode = new TreeNode();
            TexturesNode.Text = "Textures";
            TexturesNode.Tag = _textures;
            foreach (Texture texture in _textures)
                TexturesNode.Nodes.Add(texture.GetAsTreeNode());
            F3DEXOverallNode.Nodes.Add(TexturesNode);

            //Commands
            TreeNode CommandsNode = new TreeNode();
            CommandsNode.Text = "Commands";
            CommandsNode.Tag = _commandCollections;
            foreach (F3DEXCommandCollection command in _commandCollections)
                CommandsNode.Nodes.Add(command.GetAsTreeNode());
            F3DEXOverallNode.Nodes.Add(CommandsNode);

            //Vertices
            TreeNode VerticesNode = new TreeNode();
            VerticesNode.Text = "Vertices";
            VerticesNode.Tag = _vertices;
            foreach (VertexCollection vertex in _vertices)
                VerticesNode.Nodes.Add(vertex.GetAsTreeNode());
            F3DEXOverallNode.Nodes.Add(VerticesNode);

            return F3DEXOverallNode;
        }

        private static bool ReferencesNeedUpdating = false;

        public void LoadReferencesFromGUID()
        {
            //Since references can be done across RomFiles, we need to handle all F3DEXContainers in the project at once
            if (!ReferencesNeedUpdating)
                return;

            List<Texture> allTextures = new List<Texture>();
            List<Palette> allPalettes = new List<Palette>();

            foreach (RomFile file in RomProject.Instance.Files)
            {
                if (file.ElementContainers.Count(c => c is F3DEXContainer) > 0)
                {
                    F3DEXContainer container = (F3DEXContainer)file.ElementContainers.First(c => c is F3DEXContainer);

                    allTextures.AddRange(container.Textures);
                    allPalettes.AddRange(container.Palettes);
                }
            }

            //Texture -> Palette
            foreach (Texture texture in allTextures)
            {
                if (texture.Format == Texture.ImageFormat.CI && texture.ImagePalette == null && texture.MatchedPaletteGUID != Guid.Empty)
                {
                    //Find the actual palette
                    if (allPalettes.Exists(p => p.GUID == texture.MatchedPaletteGUID))
                    {
                        texture.ImagePalette = allPalettes.First(p => p.GUID == texture.MatchedPaletteGUID);
                        texture.MatchedPaletteGUID = Guid.Empty;
                    }
                }
            }

            ReferencesNeedUpdating = false;
        }
    }
}
