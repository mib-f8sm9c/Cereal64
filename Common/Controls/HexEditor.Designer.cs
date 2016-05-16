namespace Cereal64.Common.Controls
{
    partial class HexEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.EditorView = new Cereal64.Common.Controls.HexEditorDataGridView();
            this.col00 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col01 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col02 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col03 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col04 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col05 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col06 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col07 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col08 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col09 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col0A = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col0B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col0C = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col0D = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col0E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col0F = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.EditorView)).BeginInit();
            this.SuspendLayout();
            // 
            // EditorView
            // 
            this.EditorView.AllowUserToAddRows = false;
            this.EditorView.AllowUserToDeleteRows = false;
            this.EditorView.AllowUserToResizeColumns = false;
            this.EditorView.AllowUserToResizeRows = false;
            this.EditorView.BackgroundColor = System.Drawing.Color.White;
            this.EditorView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.EditorView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.EditorView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.EditorView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.EditorView.ColumnHeadersHeight = 32;
            this.EditorView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.EditorView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col00,
            this.col01,
            this.col02,
            this.col03,
            this.col04,
            this.col05,
            this.col06,
            this.col07,
            this.col08,
            this.col09,
            this.col0A,
            this.col0B,
            this.col0C,
            this.col0D,
            this.col0E,
            this.col0F});
            this.EditorView.Cursor = System.Windows.Forms.Cursors.Arrow;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.EditorView.DefaultCellStyle = dataGridViewCellStyle2;
            this.EditorView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditorView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.EditorView.EnableHeadersVisualStyles = false;
            this.EditorView.GridColor = System.Drawing.SystemColors.Window;
            this.EditorView.Location = new System.Drawing.Point(0, 0);
            this.EditorView.MultiSelect = false;
            this.EditorView.Name = "EditorView";
            this.EditorView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.EditorView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.EditorView.RowHeadersWidth = 100;
            this.EditorView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.EditorView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.EditorView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.EditorView.ShowCellErrors = false;
            this.EditorView.ShowRowErrors = false;
            this.EditorView.Size = new System.Drawing.Size(647, 258);
            this.EditorView.TabIndex = 0;
            this.EditorView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.EditorView_CellEnter);
            this.EditorView.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.EditorView_CellMouseDown);
            this.EditorView.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.EditorView_CellMouseEnter);
            this.EditorView.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.EditorView_CellMouseLeave);
            this.EditorView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.EditorView_EditingControlShowing);
            this.EditorView.SelectionChanged += new System.EventHandler(this.EditorView_SelectionChanged);
            // 
            // col00
            // 
            this.col00.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col00.HeaderText = "00";
            this.col00.Name = "col00";
            this.col00.Width = 34;
            // 
            // col01
            // 
            this.col01.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col01.HeaderText = "01";
            this.col01.Name = "col01";
            this.col01.Width = 34;
            // 
            // col02
            // 
            this.col02.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col02.HeaderText = "02";
            this.col02.Name = "col02";
            this.col02.Width = 34;
            // 
            // col03
            // 
            this.col03.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col03.HeaderText = "03";
            this.col03.Name = "col03";
            this.col03.Width = 34;
            // 
            // col04
            // 
            this.col04.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col04.HeaderText = "04";
            this.col04.Name = "col04";
            this.col04.Width = 34;
            // 
            // col05
            // 
            this.col05.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col05.HeaderText = "05";
            this.col05.Name = "col05";
            this.col05.Width = 34;
            // 
            // col06
            // 
            this.col06.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col06.HeaderText = "06";
            this.col06.Name = "col06";
            this.col06.Width = 34;
            // 
            // col07
            // 
            this.col07.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col07.HeaderText = "07";
            this.col07.Name = "col07";
            this.col07.Width = 34;
            // 
            // col08
            // 
            this.col08.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col08.HeaderText = "08";
            this.col08.Name = "col08";
            this.col08.Width = 34;
            // 
            // col09
            // 
            this.col09.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col09.HeaderText = "09";
            this.col09.Name = "col09";
            this.col09.Width = 34;
            // 
            // col0A
            // 
            this.col0A.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col0A.HeaderText = "0A";
            this.col0A.Name = "col0A";
            this.col0A.Width = 34;
            // 
            // col0B
            // 
            this.col0B.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col0B.HeaderText = "0B";
            this.col0B.Name = "col0B";
            this.col0B.Width = 34;
            // 
            // col0C
            // 
            this.col0C.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col0C.HeaderText = "0C";
            this.col0C.Name = "col0C";
            this.col0C.Width = 34;
            // 
            // col0D
            // 
            this.col0D.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col0D.HeaderText = "0D";
            this.col0D.Name = "col0D";
            this.col0D.Width = 34;
            // 
            // col0E
            // 
            this.col0E.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col0E.HeaderText = "0E";
            this.col0E.Name = "col0E";
            this.col0E.Width = 34;
            // 
            // col0F
            // 
            this.col0F.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col0F.HeaderText = "0F";
            this.col0F.Name = "col0F";
            this.col0F.Width = 34;
            // 
            // HexEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.EditorView);
            this.Name = "HexEditor";
            this.Size = new System.Drawing.Size(647, 258);
            ((System.ComponentModel.ISupportInitialize)(this.EditorView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private HexEditorDataGridView EditorView;
        private System.Windows.Forms.DataGridViewTextBoxColumn col00;
        private System.Windows.Forms.DataGridViewTextBoxColumn col01;
        private System.Windows.Forms.DataGridViewTextBoxColumn col02;
        private System.Windows.Forms.DataGridViewTextBoxColumn col03;
        private System.Windows.Forms.DataGridViewTextBoxColumn col04;
        private System.Windows.Forms.DataGridViewTextBoxColumn col05;
        private System.Windows.Forms.DataGridViewTextBoxColumn col06;
        private System.Windows.Forms.DataGridViewTextBoxColumn col07;
        private System.Windows.Forms.DataGridViewTextBoxColumn col08;
        private System.Windows.Forms.DataGridViewTextBoxColumn col09;
        private System.Windows.Forms.DataGridViewTextBoxColumn col0A;
        private System.Windows.Forms.DataGridViewTextBoxColumn col0B;
        private System.Windows.Forms.DataGridViewTextBoxColumn col0C;
        private System.Windows.Forms.DataGridViewTextBoxColumn col0D;
        private System.Windows.Forms.DataGridViewTextBoxColumn col0E;
        private System.Windows.Forms.DataGridViewTextBoxColumn col0F;
    }
}
