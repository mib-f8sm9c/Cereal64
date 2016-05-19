﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using System.Collections.ObjectModel;
using Cereal64.Microcodes.F3DZEX.DataElements;
using System.Windows.Forms;
using Cereal64.Common.DataElements;
using Cereal64.Common.Rom;

namespace Cereal64.Microcodes.F3DZEX
{
    public class F3DZEXContainer : IN64ElementContainer
    {
        public ReadOnlyCollection<Palette> Palettes { get { return _palettes.AsReadOnly(); } }
        private List<Palette> _palettes;

        public ReadOnlyCollection<Texture> Textures { get { return _textures.AsReadOnly(); } }
        private List<Texture> _textures;

        public ReadOnlyCollection<F3DZEXCommandCollection> CommandCollections { get { return _commandCollections.AsReadOnly(); } }
        private List<F3DZEXCommandCollection> _commandCollections;
        
        public ReadOnlyCollection<VertexCollection> Vertices { get { return _vertices.AsReadOnly(); } }
        private List<VertexCollection> _vertices;

        public F3DZEXContainer()
        {
            _palettes = new List<Palette>();
            _textures = new List<Texture>();
            _commandCollections = new List<F3DZEXCommandCollection>();
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
            else if (element is F3DZEXCommandCollection)
            {
                if (_commandCollections.Contains(element))
                    return false;
                _commandCollections.Add((F3DZEXCommandCollection)element);
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
            else if (element is F3DZEXCommandCollection)
            {
                if (!_commandCollections.Contains(element))
                    return false;
                _commandCollections.Remove((F3DZEXCommandCollection)element);
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
            TreeNode F3DZEXOverallNode = new TreeNode();
            F3DZEXOverallNode.Text = "F3DZEX Related Elements";
            F3DZEXOverallNode.Tag = this;

            //Palettes
            TreeNode PalettesNode = new TreeNode();
            PalettesNode.Text = "Palettes";
            PalettesNode.Tag = _palettes;
            foreach (Palette palette in _palettes)
                PalettesNode.Nodes.Add(palette.GetAsTreeNode());
            F3DZEXOverallNode.Nodes.Add(PalettesNode);

            //Textures
            TreeNode TexturesNode = new TreeNode();
            TexturesNode.Text = "Textures";
            TexturesNode.Tag = _textures;
            foreach (Texture texture in _textures)
                TexturesNode.Nodes.Add(texture.GetAsTreeNode());
            F3DZEXOverallNode.Nodes.Add(TexturesNode);

            //Commands
            TreeNode CommandsNode = new TreeNode();
            CommandsNode.Text = "Commands";
            CommandsNode.Tag = _commandCollections;
            foreach (F3DZEXCommandCollection command in _commandCollections)
                CommandsNode.Nodes.Add(command.GetAsTreeNode());
            F3DZEXOverallNode.Nodes.Add(CommandsNode);

            //Vertices
            TreeNode VerticesNode = new TreeNode();
            VerticesNode.Text = "Vertices";
            VerticesNode.Tag = _vertices;
            foreach (VertexCollection vertex in _vertices)
                VerticesNode.Nodes.Add(vertex.GetAsTreeNode());
            F3DZEXOverallNode.Nodes.Add(VerticesNode);

            return F3DZEXOverallNode;
        }

    }
}
