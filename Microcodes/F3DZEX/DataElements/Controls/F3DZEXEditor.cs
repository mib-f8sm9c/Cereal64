using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cereal64.Microcodes.F3DZEX.DataElements;
using Cereal64.Microcodes.F3DZEX.DataElements.Commands;

namespace Cereal64.Microcodes.F3DZEX.Controls
{
    public partial class F3DZEXEditor : UserControl
    {
        public bool FixedSize { get; set; }

        private F3DZEXCommandCollection _commands;
        public F3DZEXCommandCollection Commands
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

        public F3DZEXEditor()
        {
            InitializeComponent();
        }

        private void PopulateCommands()
        {
            if (_commands == null)
                return;

            foreach (F3DZEXCommand command in _commands.Commands)
            {
                lbCommands.Items.Add(command);
            }
        }

        private void lbCommands_Format(object sender, ListControlConvertEventArgs e)
        {
            F3DZEXCommand command = e.ListItem as F3DZEXCommand;

            e.Value = string.Format("{0}: {1}", string.Format("{0:X2}", (int)(e.ListItem as F3DZEXCommand).CommandID), command.CommandID);
        }

        private void lbCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            commandProperties.SelectedObject = lbCommands.SelectedItem;
        }
    }
}
