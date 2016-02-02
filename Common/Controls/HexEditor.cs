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
    //To do: Tie this with a RomFile? Allow for editing to specifically target certain areas

    //To do: Copy/Paste with right click menu & keyboard shortcuts

    public partial class HexEditor : UserControl
    {
        public byte[] Data { get; private set; }

        public HexEditor()
        {
            InitializeComponent();

            InitEditor();
        }

        public void SetData(byte[] data)
        {
            ClearEditor();

            Data = data;

            InitEditor();
        }

        public void ClearEditor()
        {
            EditorView.Rows.Clear();
        }

        public void InitEditor()
        {
            if (Data == null)
            {
                int rowIndex = EditorView.Rows.Add();

                EditorView.Rows[rowIndex].HeaderCell.Value = "00000000";
            }
            else
            {
                for (int i = 0; i < Data.Length;)
                {
                    int rowIndex = EditorView.Rows.Add();

                    EditorView.Rows[rowIndex].HeaderCell.Value = i.ToString("X8");
                    
                    int j;
                    for (j = 0; i + j < Data.Length && j < 16; j++)
                    {
                        EditorView.Rows[rowIndex].Cells[j].Value = Data[i + j].ToString("X");
                    }
                    i += j;
                }
            }
        }

        private void EditorView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            //Skip the Column and Row headers
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            EditorView.Cursor = Cursors.IBeam;
        }

        private void EditorView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            EditorView.Cursor = Cursors.Default;
        }

        //Copied from https://social.msdn.microsoft.com/Forums/windows/en-US/fe5d5cfb-63b6-4a69-a01c-b7bbd18ae84a/how-can-you-achieve-single-click-editing-in-a-datagridview?forum=winformsdatacontrols
        private void EditorView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                base.OnMouseDown(e);
                return;
            }

            //Skip the Column and Row headers
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                base.OnMouseDown(e);
                return;
            }

            // If column isn't text or it's read only, do standard processing
            if (!(EditorView.Columns[e.ColumnIndex] is DataGridViewTextBoxColumn) ||
                EditorView.Columns[e.ColumnIndex].ReadOnly)
            {
                base.OnMouseDown(e);
                return;
            }


            //Enter into the cell, put cursor closest to mouse
            DataGridViewTextBoxCell textBoxCell = (DataGridViewTextBoxCell)EditorView.Rows[e.RowIndex].Cells[e.ColumnIndex];


            // If cell not current, try and make it so
            if (EditorView.CurrentCell != textBoxCell)
            {
                // Allow standard processing make clicked cell current
                base.OnMouseDown(e);

                // If this didn't happen (validation failed etc), abort
                if (EditorView.CurrentCell != textBoxCell)
                {
                    return;
                }
            }
            
            // If already in edit mode, do standard processing (will position caret)
            if (EditorView.CurrentCell.IsInEditMode)
            {
                base.OnMouseDown(e);
                return;
            }

            EditorView.BeginEdit(false);
            TextBoxBase textBox = (TextBoxBase)EditorView.EditingControl;
            textBox.SelectionStart = 0;
            textBox.ScrollToCaret();

            Int32 lParam = e.X << 16;
            SendMessage(EditorView.EditingControl.Handle, WM_LBUTTONDOWN, 0, lParam);
            SendMessage(EditorView.EditingControl.Handle, WM_LBUTTONUP, 0, lParam);
        }

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        private void EditorView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //Skip the Column and Row headers
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            //Select all text in cell
        }

        private Keys[] validKeys = new Keys[] { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
            Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
            Keys.F, Keys.Left, Keys.Right, Keys.Enter };

        private void EditorView_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;

            //Check if it's in editing mode
            if (EditorView.CurrentCell == null || !EditorView.CurrentCell.IsInEditMode)
            {
                return;
            }

            //Needs to be a valid key
            if (!validKeys.Contains(e.KeyCode))
            {
                return;
            }

            TextBoxBase textBox = (TextBoxBase)EditorView.EditingControl;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    //Move caret left
                    textBox.SelectionStart = textBox.SelectionStart + textBox.SelectionLength - 1;
                    textBox.SelectionLength = 0;
                    textBox.ScrollToCaret();
                    //MoveCaretLeft();
                    break;
                case Keys.Right:
                    //Move caret right
                    textBox.SelectionStart = textBox.SelectionStart + textBox.SelectionLength + 1;
                    textBox.SelectionLength = 0;
                    textBox.ScrollToCaret();
                    break;
                case Keys.Enter:
                    //Deselect the cell, exit editing
                    break;
                default:
                    //Overwrite
                    if(textBox.SelectionLength == 0)
                    {
                        //holy cow this is terrible
                        if (textBox.SelectionStart < textBox.Text.Length)
                        {
                            char[] text = textBox.Text.ToArray();
                            int selectionStart = textBox.SelectionStart;
                            text[selectionStart] = (new KeysConverter()).ConvertToString(e.KeyCode)[0];
                            textBox.Text = new string(text);
                            textBox.SelectionStart = selectionStart + 1;
                        }
                    }
                    break;
            }

        }

        private void MoveCaretLeft()
        {
            if(EditorView.CurrentCell == null)
                return;

            TextBoxBase textBox = (TextBoxBase)EditorView.EditingControl;

            int selectionStart = textBox.SelectionStart;

            if (selectionStart == 0)
            {
                //Move to previous cell
                if (EditorView.CurrentCell.ColumnIndex == 0)
                {
                    if (EditorView.CurrentCell.RowIndex == 0) //Upper left most cell
                        return;
                    else
                    {
                        EditorView.CurrentCell = EditorView.Rows[EditorView.CurrentCell.RowIndex - 1].Cells[15];
                    }
                }
                else
                {
                    EditorView.CurrentCell = EditorView.Rows[EditorView.CurrentCell.RowIndex].Cells[EditorView.CurrentCell.ColumnIndex - 1];
                }

                EditorView.BeginEdit(false);
                textBox = (TextBoxBase)EditorView.EditingControl;
                textBox.SelectionLength = 0;
                textBox.SelectionStart = 2;
                textBox.ScrollToCaret();
            }
            else
            {
                textBox.SelectionStart--;
                textBox.ScrollToCaret();
            }
        }

        private void MoveCaretRight()
        {

        }

        private void EditorView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
            tb.KeyDown -= EditorView_KeyDown;
            tb.KeyDown += EditorView_KeyDown;
        }

        private void EditorView_CurrentCellChanged(object sender, EventArgs e)
        {
            if (EditorView.CurrentCell == null)
                return;

            EditorView.BeginEdit(false);
            TextBoxBase textBox = (TextBoxBase)EditorView.EditingControl;
            textBox.SelectionStart = 0;
            textBox.ScrollToCaret();
        }
    }
}
