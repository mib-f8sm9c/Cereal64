using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using System.ComponentModel;
using Cereal64.Common.DataElements;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    // To do: add a thanks to CloudModding for the F3DZEX help info

    public abstract class F3DZEXCommand : N64DataElement
    {
        public delegate void UpdateEvent();

        public UpdateEvent Updated = delegate { };

        public abstract F3DZEXCommandID CommandID { get; }

        public abstract string CommandName { get; }

        public abstract string CommandDesc { get; }

        public abstract bool IsValid { get; protected set; }

        public F3DZEXCommand(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
        }

        public F3DZEXCommand(int fileOffset, byte[] data)
            : base(fileOffset, data)
        {

        }

        public override TreeNode GetAsTreeNode()
        {
            TreeNode node = new TreeNode();
            node.Tag = this;
            node.Text =  string.Format("{0}: {1}", string.Format("{0:X2}", (int)this.CommandID), this.CommandID);

            return node;
        }

        public void CallUpdate()
        {
            Updated();
        }
    }
}
