using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using System.Collections.ObjectModel;
using Cereal64.Microcodes.F3DZEX.DataElements;
using System.Windows.Forms;

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

        public void AddElement(N64DataElement element)
        {
            if (element is Palette)
                _palettes.Add((Palette)element);
            else if (element is Texture)
                _textures.Add((Texture)element);
            else if (element is F3DZEXCommandCollection)
                _commandCollections.Add((F3DZEXCommandCollection)element);
            else if (element is VertexCollection)
                _vertices.Add((VertexCollection)element);
        }

        public void RemoveElement(N64DataElement element)
        {
            if (element is Palette)
                _palettes.Remove((Palette)element);
            else if (element is Texture)
                _textures.Remove((Texture)element);
            else if (element is F3DZEXCommandCollection)
                _commandCollections.Remove((F3DZEXCommandCollection)element);
            else if (element is VertexCollection)
                _vertices.Remove((VertexCollection)element);
        }

        public TreeNode GetAsTreeNode()
        {
            TreeNode F3DZEXOverallNode = new TreeNode();
            F3DZEXOverallNode.Text = "F3DZEX Related Elements";

            //Palettes

            //Textures

            //Commands

            //Vertices

            return F3DZEXOverallNode;
        }
    }
}
