using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cereal64.Common.Controls
{
    public partial class HexNumericUpDown : UserControl
    {
        public enum BaseState
        {
            hex,
            dec
        }

        private BaseState _state;

        public HexNumericUpDown()
        {
            InitializeComponent();

            _state = BaseState.hex;

        }

        public decimal Value
        {
            get
            {
                return numericUpDown.Value;
            }
            set
            {
                numericUpDown.Value = value;
            }
        }

        public decimal Minimum
        {
            get
            {
                return numericUpDown.Minimum;
            }
            set
            {
                numericUpDown.Minimum = value;
            }
        }

        public decimal Maximum
        {
            get
            {
                return numericUpDown.Maximum;
            }
            set
            {
                numericUpDown.Maximum = value;
            }
        }

        public BaseState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state == value)
                    return;

                switch (value)
                {
                    case BaseState.hex:
                        lbl0x.Visible = true;
                        numericUpDown.Hexadecimal = true;
                        optionDec.Checked = false;
                        optionHex.Checked = true;
                        btnBaseSystem.Text = "hex";
                        break;
                    case BaseState.dec:
                        lbl0x.Visible = false;
                        numericUpDown.Hexadecimal = false;
                        optionHex.Checked = false;
                        optionDec.Checked = true;
                        btnBaseSystem.Text = "dec";
                        break;
                }

                _state = value;
            }
        }

        private void btnBaseSystem_Click(object sender, EventArgs e)
        {
            baseMenu.Show(btnBaseSystem.Parent, btnBaseSystem.Left, btnBaseSystem.Bottom);
        }

        private void optionHex_Click(object sender, EventArgs e)
        {
            State = BaseState.hex;
        }

        private void optionDec_Click(object sender, EventArgs e)
        {
            State = BaseState.dec;
        }
    }
}
