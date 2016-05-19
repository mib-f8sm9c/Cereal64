using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Cereal64.Microcodes.F3DEX.DataElements.Commands;
using Cereal64.Common;
using System.Windows.Forms;
using Cereal64.Common.DataElements;
using System.Xml.Linq;

namespace Cereal64.Microcodes.F3DEX.DataElements
{
    public class F3DEXCommandCollection : N64DataElement
    {
        private List<F3DEXCommand> _commands = new List<F3DEXCommand>();

        public ReadOnlyCollection<F3DEXCommand> Commands { get { return _commands.AsReadOnly(); } }

        public F3DEXCommandCollection(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
        }

        public F3DEXCommandCollection(int index, byte[] rawBytes)
            : base(index, rawBytes)
        {

        }

        public F3DEXCommandCollection(int index, List<F3DEXCommand> commands)
            : base (index, null)
        {
            _commands = commands;
        }

        public override byte[] RawData 
        { 
            get
            {
                byte[] rawBytes = new byte[RawDataSize];
                for (int i = 0; i < _commands.Count; i++)
                {
                    Array.Copy(_commands[i].RawData, 0, rawBytes, i * 0x8, 0x8);
                }
                return rawBytes;
            }
            set
            {
                if (value == null) //Use null to allow for different constructor
                    return;

                byte[] bytes = new byte[0x8];
                for (int i = 0; i < value.Length - 0x7; i += 0x8)
                {
                    Array.Copy(value, i, bytes, 0, 0x8);

                    F3DEXCommand command = F3DEXCommandFactory.ReadCommand(FileOffset + i, bytes);
                    if (command != null)
                        _commands.Add(command);
                }
            }
        }

        public override int RawDataSize { get { return _commands.Count * 0x8; } }

        public override System.Windows.Forms.TreeNode GetAsTreeNode()
        {
            //Include all the previous things
            TreeNode node = new TreeNode();
            node.Text = "Command Collection";
            node.Tag = this;

            foreach (F3DEXCommand command in _commands)
            {
                node.Nodes.Add(command.GetAsTreeNode());
            }

            return node;
        }

        public override void PostXMLLoad()
        {
            F3DEXReader.ParseF3DEXCommands(_commands);
        }
    }
}
