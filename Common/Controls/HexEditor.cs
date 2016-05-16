using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Cereal64.Common.Controls
{
    //To do: Copy/Paste with right click menu & keyboard shortcuts

    /// <summary>
    /// Custom Hex Editor to be used in Cereal64 applications for browsing/editing raw binary data.
    /// </summary>
    public partial class HexEditor : UserControl
    {
        /// <summary>
        /// The raw data displayed in the Hex Editor. Set through SetData.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Event that fires whenever the first change is made, or the changes are saved. For external
        ///  use to write out the raw data when changes are saved.
        /// </summary>
        public ChangedStateChangedEvent ChangedStateChanged = delegate { };
        public delegate void ChangedStateChangedEvent();

        /// <summary>
        /// Caret position within the selected textbox. Can be 0, 1 or 2
        /// </summary>
        private int _caretPosition = 0;

        /// <summary>
        /// Reference to the last cell with valid data
        /// </summary>
        private DataGridViewCell _lastCell;

        /// <summary>
        /// Special text pixel width used for clicking navigation on textboxes
        /// </summary>
        private const int ENDTEXTPOSITION = 18;

        /// <summary>
        /// Min caret position value
        /// </summary>
        private const int CARET_CELL_START = 0;

        /// <summary>
        /// Max caret position value
        /// </summary>
        private const int CARET_CELL_END = 2;

        /// <summary>
        /// Cell font used for the data cells
        /// </summary>
        private Font DefaultCellFont = new Font("Courier New", 9.75f);

        /// <summary>
        /// True if unsaved changes exist, false if not.
        /// </summary>
        public bool UnsavedChanges { get { return _unsavedChanges; } set { if (_unsavedChanges != value) { _unsavedChanges = value; UpdateChanged(); } } }
        private bool _unsavedChanges;
        
        /// <summary>
        /// Keys that will be accepted by the HexEditor for navigation/editing.
        /// </summary>
        private Keys[] validKeys = new Keys[] { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
            Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
            Keys.F, Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Enter, Keys.Tab, Keys.Delete,
            Keys.Back, Keys.Space };

        /// <summary>
        /// True if caret is at the start of the current cell
        /// </summary>
        private bool CaretAtCellStart { get { return _caretPosition == CARET_CELL_START; } }

        /// <summary>
        /// True if caret is at the end of the current cell
        /// </summary>
        private bool CaretAtCellEnd { get { return _caretPosition == CARET_CELL_END; } }

        /// <summary>
        /// True if caret is in the first cell
        /// </summary>
        private bool FirstCellSelected { get { return EditorView.CurrentCell.ColumnIndex == 0 && EditorView.CurrentCell.RowIndex == 0; } }

        /// <summary>
        /// True if caret is at the start of the first cell
        /// </summary>
        private bool FirstPositionSelected { get { return FirstCellSelected && CaretAtCellStart; } }

        /// <summary>
        /// True if caret is in the last cell with data in it
        /// </summary>
        private bool LastCellSelected { get { return EditorView.CurrentCell == _lastCell; } }

        /// <summary>
        /// True if caret is at the end of the last cell with data in it
        /// </summary>
        private bool LastPositionSelected { get { return LastCellSelected && CaretAtCellEnd; } }

        /// <summary>
        /// Constructor for the HexEditor
        /// </summary>
        public HexEditor()
        {
            InitializeComponent();

            EditorView.DoubleBuffered(true);

            InitEditor();
        }

        /// <summary>
        /// Sets up the HexEditor for use. Called every time Data is changed
        /// </summary>
        public void InitEditor()
        {
            EditorView.Rows.Clear();
            _lastCell = null;

            InitHeaders();

            if (Data == null)
            {
                //Add a single empty 
                int rowIndex = EditorView.Rows.Add();
                EditorView.Rows[rowIndex].HeaderCell.Value = "00000000";
            }
            else
            {
                //Create a row for each group of 16 bytes
                for (int i = 0; i < Data.Length; )
                {
                    int rowIndex = EditorView.Rows.Add();
                    
                    EditorView.Rows[rowIndex].HeaderCell.Value = i.ToString("X8");

                    //Go through all 16 cells and fill in the data (if they have it)
                    int j;
                    for (j = 0; i + j < Data.Length && j < 16; j++)
                    {
                        EditorView.Rows[rowIndex].Cells[j].Value = Data[i + j].ToString("X");
                        _lastCell = EditorView.Rows[rowIndex].Cells[j];
                    }
                    for (; j < 16; j++)
                    {
                        EditorView.Rows[rowIndex].Cells[j].Value = null;
                    }
                    i += j;
                }

                _caretPosition = CARET_CELL_START;
                EditorView.CurrentCell = null;
            }
        }

        /// <summary>
        /// Reset the column headers. Not really necessary, but if we ever use a custom
        ///  DataGridViewColumn type, we'll need to define it in here.
        /// </summary>
        private void InitHeaders()
        {
            EditorView.Columns.Clear();
            for (int i = 0; i < 16; i++)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.Resizable = DataGridViewTriState.False;
                col.Width = 34;
                col.HeaderText = string.Format("{0:X2}", i);
                col.MaxInputLength = CARET_CELL_END;
                col.DefaultCellStyle.Font = DefaultCellFont;
                EditorView.Columns.Add(col);
            }
        }

        /// <summary>
        /// Set the data to fill in the HexEditor with.
        /// </summary>
        /// <param name="data">Binary data to display.</param>
        public void SetData(byte[] data)
        {
            ClearEditor();

            Data = data;

            InitEditor();
        }

        /// <summary>
        /// Clear out all rows from the HexEditor
        /// </summary>
        public void ClearEditor()
        {
            EditorView.Rows.Clear();
        }

        /// <summary>
        /// Derive the DataGridViewCell referenced in the DataGridViewCellEventArgs. Useful function when
        ///  subscribing to DataGridViewCell events.
        /// </summary>
        private DataGridViewCell GetCellFromArgs(DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= EditorView.Columns.Count || e.RowIndex >= EditorView.Rows.Count ||
                e.ColumnIndex < 0 || e.RowIndex < 0)
                return null;

            return EditorView.Rows[e.RowIndex].Cells[e.ColumnIndex];
        }

        /// <summary>
        /// Derive the DataGridViewCell referenced in the DataGridViewCellMouseEventArgs. Useful function when
        ///  subscribing to DataGridViewCell events.
        /// </summary>
        private DataGridViewCell GetCellFromArgs(DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= EditorView.Columns.Count || e.RowIndex >= EditorView.Rows.Count ||
                e.ColumnIndex < 0 || e.RowIndex < 0)
                return null;

            return EditorView.Rows[e.RowIndex].Cells[e.ColumnIndex];
        }

        /// <summary>
        /// Automatically called when UnsavedChanges changes values. Currently unfinished.
        /// </summary>
        private void UpdateChanged()
        {
            if (_unsavedChanges)
            {
                //Going from unsaved to saved
            }
            else
            {
                //Going from saved to unsaved

                //Clear out the bolds
                this.SuspendLayout();

                for (int i = 0; i < EditorView.Rows.Count; i++)
                {
                    for (int j = 0; j < EditorView.Columns.Count; j++)
                    {
                        EditorView.Rows[i].Cells[j].Style.ForeColor = Color.Black;
                        EditorView.CurrentCell.Style.SelectionForeColor = Color.Black;
                    }
                }

                TextBox tb = (TextBox)EditorView.EditingControl;
                if (tb != null)
                {
                    tb.ForeColor = Color.Black;
                }

                this.ResumeLayout();
            }

            ChangedStateChanged();
        }

        /// <summary>
        /// Move the caret to the left if possible, switching cells if need be.
        /// </summary>
        private void MoveCaretLeft()
        {
            if(EditorView.CurrentCell == null)
                return;

            if (CaretAtCellStart)
            {
                DataGridViewCell prevCell;

                //Move to previous cell
                if (EditorView.CurrentCell.ColumnIndex == 0)
                {
                    if (EditorView.CurrentCell.RowIndex == 0) //Upper left most cell
                        return;
                    else
                    {
                        prevCell = EditorView.Rows[EditorView.CurrentCell.RowIndex - 1].Cells[15];
                    }
                }
                else
                {
                    prevCell = EditorView.Rows[EditorView.CurrentCell.RowIndex].Cells[EditorView.CurrentCell.ColumnIndex - 1];
                }

                if (prevCell != null && prevCell.Value != null)
                {
                    _caretPosition = CARET_CELL_END;
                    EditorView.CurrentCell = prevCell;
                    EditorView.BeginEdit(false);
                }
            }
            else
            {
                _caretPosition--;
            }

            UpdateCaret((TextBoxBase)EditorView.EditingControl);
        }

        /// <summary>
        /// Move the caret to the left if possible, switching cells if need be.
        /// </summary>
        /// <param name="tab">If tabbing, the caret position is maintained and the cell to the right is selected</param>
        private void MoveCaretRight(bool tab = false)
        {
            if (EditorView.CurrentCell == null)
                return;

            if (_caretPosition == CARET_CELL_END || tab)
            {
                DataGridViewCell nextCell;

                //Move to previous cell
                if (EditorView.CurrentCell.ColumnIndex == 15)
                {
                    if (EditorView.CurrentCell.RowIndex == EditorView.Rows.Count - 1) //Lower right most cell
                        return;
                    else
                    {
                        nextCell = EditorView.Rows[EditorView.CurrentCell.RowIndex + 1].Cells[0];
                    }
                }
                else
                {
                    nextCell = EditorView.Rows[EditorView.CurrentCell.RowIndex].Cells[EditorView.CurrentCell.ColumnIndex + 1];
                }

                if (nextCell != null && nextCell.Value != null)
                {
                    _caretPosition = CARET_CELL_START;
                    EditorView.CurrentCell = nextCell;
                    EditorView.BeginEdit(false);
                }
            }
            else
            {
                _caretPosition++;
            }

            UpdateCaret((TextBoxBase)EditorView.EditingControl);
        }

        /// <summary>
        /// Select the cell above the currently selected one if possible, maintaining caret position.
        /// </summary>
        private void MoveCaretUp()
        {
            if (EditorView.CurrentCell == null)
                return;

                //Move to previous cell
            if (EditorView.CurrentCell.RowIndex != 0)
            {
                DataGridViewCell prevCell = EditorView.Rows[EditorView.CurrentCell.RowIndex - 1].Cells[EditorView.CurrentCell.ColumnIndex];

                if (prevCell != null && prevCell.Value != null)
                {
                    EditorView.CurrentCell = prevCell;
                    EditorView.BeginEdit(false);
                    UpdateCaret((TextBoxBase)EditorView.EditingControl);
                }
            }
        }

        /// <summary>
        /// Select the cell above the currently selected one if possible, maintaining caret position.
        /// </summary>
        private void MoveCaretDown()
        {
            if (EditorView.CurrentCell == null)
                return;

            //Move to previous cell
            if (EditorView.CurrentCell.RowIndex != EditorView.Rows.Count - 1)
            {
                DataGridViewCell nextCell = EditorView.Rows[EditorView.CurrentCell.RowIndex + 1].Cells[EditorView.CurrentCell.ColumnIndex];

                if (nextCell != null && nextCell.Value != null)
                {
                    EditorView.CurrentCell = nextCell;
                    EditorView.BeginEdit(false);
                    UpdateCaret((TextBoxBase)EditorView.EditingControl);
                }
            }
        }

        /// <summary>
        /// Repositions the caret in the defined TextBox to position _caretPosition
        /// </summary>
        /// <param name="textBox">Textbox to update the caret position in</param>
        private void UpdateCaret(TextBoxBase textBox)
        {
            textBox.SelectionLength = 0;
            textBox.SelectionStart = _caretPosition;
            textBox.ScrollToCaret();
        }
        
        /// <summary>
        /// Overwrites the character in front of the caret, and moves the caret one position forward. Will
        ///  jump to the next cell if need be.
        /// </summary>
        /// <param name="newChar">The character to overwrite. Must be 0-9 or A-F.</param>
        private void OverwriteHexValue(char newChar)
        {
            if (EditorView.CurrentCell == null)
                return;

            TextBoxBase textBox = (TextBoxBase)EditorView.EditingControl;

            if (textBox.SelectionLength == 0)
            {
                if (LastPositionSelected)
                    return;

                if (CaretAtCellEnd)
                    MoveCaretRight();

                ChangeHexValue(textBox, newChar, _caretPosition);

                _caretPosition++;
                UpdateCaret(textBox);
            }
        }

        /// <summary>
        /// Moves the caret one position back and set the traversed character to 0. Will
        ///  jump to the previous cell if need be.
        /// </summary>
        private void ClearPreviousChar()
        {
            if (EditorView.CurrentCell == null)
                return;

            TextBoxBase textBox = (TextBoxBase)EditorView.EditingControl;

            if (textBox.SelectionLength == 0)
            {
                if (LastPositionSelected)
                    return;

                if (CaretAtCellStart)
                    MoveCaretLeft();

                MoveCaretLeft();

                ChangeHexValue(textBox, '0', _caretPosition);

                UpdateCaret(textBox);
            }
        }

        /// <summary>
        /// Sets the character in front of the caret to 0 and moves the caret one position forward. Will
        ///  jump to the next cell if need be.
        /// </summary>
        private void ClearNextChar()
        {
            if (EditorView.CurrentCell == null)
                return;

            TextBoxBase textBox = (TextBoxBase)EditorView.EditingControl;

            if (textBox.SelectionLength == 0)
            {
                if (LastPositionSelected)
                    return;

                if (CaretAtCellEnd)
                    MoveCaretRight();

                ChangeHexValue(textBox, '0', _caretPosition);

                _caretPosition++;
                UpdateCaret(textBox);
            }
        }

        /// <summary>
        /// Changes the defined character in the textbox with the given character.
        /// </summary>
        /// <param name="textbox">Textbox to edit</param>
        /// <param name="newChar">Character to overwrite with</param>
        /// <param name="overwriteCharPosition">Position of the character to overwrite. 0 or 1.</param>
        public void ChangeHexValue(TextBoxBase textbox, char newChar, int overwriteCharPosition)
        {
            char[] text = textbox.Text.ToArray();
            if (text[overwriteCharPosition] != newChar)
            {
                text[_caretPosition] = newChar;
                textbox.Text = new string(text);
                textbox.ForeColor = Color.Red;
                EditorView.CurrentCell.Style.ForeColor = Color.Red;
                EditorView.CurrentCell.Style.SelectionForeColor = Color.Red;

                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// Subscribes the editing control to a KeyDown event, to capture the desired key events.
        /// </summary>
        private void EditorView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
            tb.KeyDown -= EditorView_KeyDown;
            tb.KeyDown += EditorView_KeyDown;
        }

        /// <summary>
        /// Forces entered cells into edit mode, and subscribes to their KeyDown events to suppress
        ///  their innate key press code. Finally resets the caret.
        /// </summary>
        private void EditorView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (EditorView.CurrentCell == null)
                return;

            if (EditorView.CurrentCell.Value != null)
            {
                if (!EditorView.CurrentCell.IsInEditMode)
                    EditorView.BeginEdit(false);

                TextBoxBase textBox = (TextBoxBase)EditorView.EditingControl;

                if (textBox != null)
                {
                    //Disable the arrow keys for our own implementation
                    textBox.KeyDown -= textBox_KeyDown;
                    textBox.KeyDown += textBox_KeyDown;

                    UpdateCaret(textBox);
                }
            }
        }

        /// <summary>
        /// Supresses key events for the keys defined in validKeys. Others will hit the 
        ///  readonly status of the DataGridView and will ping the error noise.
        /// </summary>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (validKeys.Contains(e.KeyCode))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Handle the mouse click, to select the new cell and reposition the caret closest
        ///  to the mouse.
        /// </summary>
        private void EditorView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewCell cell = GetCellFromArgs(e);
            if (cell == null || !(cell is DataGridViewTextBoxCell) || cell.Value == null)
            {
                return;
            }

            //Enter into the cell, put cursor closest to mouse
            DataGridViewTextBoxCell textBoxCell = (DataGridViewTextBoxCell)EditorView.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (EditorView.CurrentCell != textBoxCell)
                EditorView.CurrentCell = textBoxCell;

            TextBox editor = (TextBox)EditorView.EditingControl;

            // insert checks for null here if needed!!

            //Here we check by hand if the mouse position is far enough right to put the caret at
            // the end. If not, we let the computer decide with GetCharIndexFromPosition.
            if (e.Location.X > ENDTEXTPOSITION)
                _caretPosition = CARET_CELL_END;
            else
                _caretPosition = editor.GetCharIndexFromPosition(e.Location);

            UpdateCaret(editor);
        }

        /// <summary>
        /// Checks if the selected cell is one of the invalid cells at the end. If
        ///  it is, it switches back to the last cell with valid data and puts
        ///  the caret at the end position.
        /// </summary>
        private void EditorView_SelectionChanged(object sender, EventArgs e)
        {
            if (EditorView.CurrentCell != null)
            {
                if (EditorView.CurrentCell.Value == null)
                {
                    //Switch back to the last cell
                    if (_lastCell != null)
                    {
                        _caretPosition = CARET_CELL_END;
                        EditorView.CurrentCell = _lastCell;
                        EditorView.BeginEdit(false);
                    }
                }
            }
        }
        
        /// <summary>
        /// Replaces the cursor with an I-Beam cursor over cells.
        /// </summary>
        private void EditorView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = GetCellFromArgs(e);

            if (cell == null || cell.Value == null) return;

            EditorView.Cursor = Cursors.IBeam;
        }

        /// <summary>
        /// Restores the cursor when leaving cells.
        /// </summary>
        private void EditorView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            EditorView.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Takes the valid key presses and performs the code that corresponds to each one.
        /// </summary>
        private void EditorView_KeyDown(object sender, KeyEventArgs e)
        {
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
                    MoveCaretLeft();
                    break;
                case Keys.Right:
                    MoveCaretRight();
                    break;
                case Keys.Up:
                    MoveCaretUp();
                    break;
                case Keys.Down:
                case Keys.Enter:
                    MoveCaretDown();
                    UnsavedChanges = false;
                    break;
                case Keys.Tab:
                    MoveCaretRight(true);
                    break;
                case Keys.Delete:
                case Keys.Space:
                    ClearNextChar();
                    break;
                case Keys.Back:
                    ClearPreviousChar();
                    break;
                default:
                    //Overwrite hex values
                    OverwriteHexValue((new KeysConverter()).ConvertToString(e.KeyCode)[0]);
                    break;
            }

        }

    }

    /// <summary>
    /// Custom DataGridView class to disable the automatic key-handling for certain keys. Doing this will
    ///  allow us to use our own custom functions for certain keypresses rather than piggy-backing off of
    ///  what the DataGridView/DataGridViewCell already does. We can also throw in a double-buffering
    ///  fix.
    /// </summary>
    internal class HexEditorDataGridView : DataGridView
    {
        /// <summary>
        /// Disable delete, back, space and arrow keys.
        /// </summary>
        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Space)
            {
                return false;
            }
            return base.ProcessDataGridViewKey(e);
        }

        /// <summary>
        /// Disable enter and tab keys.
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter || keyData == Keys.Tab)
            {
                return false;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Enable double buffering, to help reduce flickering in the form when painting
        /// </summary>
        /// <param name="setting">True to enable, false to disable</param>
        public new void DoubleBuffered(bool setting)
        {
            Type dgvType = GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this, setting, null);
        }
    }
}
