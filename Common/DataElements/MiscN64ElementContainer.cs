using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cereal64.Common.DataElements
{
    public class MiscN64ElementContainer : IN64ElementContainer
    {
        List<N64DataElement> Elements = new List<N64DataElement>();

        public bool AddElement(N64DataElement element)
        {
            if (Elements.Contains(element))
                return false;

            Elements.Add(element);

            Elements.Sort((e1, e2) => e1.FileOffset.CompareTo(e2.FileOffset));

            return true;
        }

        public bool RemoveElement(N64DataElement element)
        {
            return Elements.Remove(element);
        }

        public TreeNode GetAsTreeNode()
        {
            TreeNode elementsNode = new TreeNode();
            elementsNode.Text = "Misc. Elements";
            elementsNode.Tag = this;

            foreach (N64DataElement element in Elements)
            {
                elementsNode.Nodes.Add(element.GetAsTreeNode());
            }

            return elementsNode;
        }
    }
}
