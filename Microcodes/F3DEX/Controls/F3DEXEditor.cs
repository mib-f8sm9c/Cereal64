using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cereal64.Microcodes.F3DEX.DataElements;
using Cereal64.Microcodes.F3DEX.DataElements.Commands;

namespace Cereal64.Microcodes.F3DEX.Controls
{
    public partial class F3DEXEditor : UserControl
    {
        public bool FixedSize { get; set; }

        private F3DEXCommandCollection _commands;
        public F3DEXCommandCollection Commands
        {
            get
            {
                return _commands;
            }
            set
            {
                _commands = value;

                lbCommands.Items.Clear();

                PopulateCommands();
            }
        }

        public F3DEXEditor()
        {
            InitializeComponent();
        }

        private void PopulateCommands()
        {
            if (_commands == null)
                return;

            foreach (F3DEXCommand command in _commands.Commands)
            {
                lbCommands.Items.Add(command);
            }
        }

        private void lbCommands_Format(object sender, ListControlConvertEventArgs e)
        {
            F3DEXCommand command = e.ListItem as F3DEXCommand;

            e.Value = string.Format("{0}: {1}", string.Format("{0:X2}", (int)(e.ListItem as F3DEXCommand).CommandID), command.CommandID);
        }

        private void lbCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            commandProperties.SelectedObject = lbCommands.SelectedItem;
        }
    }
}
