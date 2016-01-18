namespace Cereal64.Common.Controls
{
    partial class HexNumericUpDown
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
            this.components = new System.ComponentModel.Container();
            this.numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.lbl0x = new System.Windows.Forms.Label();
            this.btnBaseSystem = new System.Windows.Forms.Button();
            this.baseMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.optionHex = new System.Windows.Forms.ToolStripMenuItem();
            this.optionDec = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
            this.baseMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericUpDown
            // 
            this.numericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown.Hexadecimal = true;
            this.numericUpDown.Location = new System.Drawing.Point(22, 2);
            this.numericUpDown.Name = "numericUpDown";
            this.numericUpDown.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown.TabIndex = 0;
            // 
            // lbl0x
            // 
            this.lbl0x.AutoSize = true;
            this.lbl0x.Location = new System.Drawing.Point(4, 4);
            this.lbl0x.Name = "lbl0x";
            this.lbl0x.Size = new System.Drawing.Size(18, 13);
            this.lbl0x.TabIndex = 1;
            this.lbl0x.Text = "0x";
            // 
            // btnBaseSystem
            // 
            this.btnBaseSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBaseSystem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBaseSystem.Location = new System.Drawing.Point(146, 1);
            this.btnBaseSystem.Name = "btnBaseSystem";
            this.btnBaseSystem.Size = new System.Drawing.Size(39, 21);
            this.btnBaseSystem.TabIndex = 2;
            this.btnBaseSystem.Text = "hex";
            this.btnBaseSystem.UseVisualStyleBackColor = true;
            this.btnBaseSystem.Click += new System.EventHandler(this.btnBaseSystem_Click);
            // 
            // baseMenu
            // 
            this.baseMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionHex,
            this.optionDec});
            this.baseMenu.Name = "baseMenu";
            this.baseMenu.Size = new System.Drawing.Size(94, 48);
            // 
            // optionHex
            // 
            this.optionHex.Checked = true;
            this.optionHex.CheckOnClick = true;
            this.optionHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionHex.Name = "optionHex";
            this.optionHex.Size = new System.Drawing.Size(93, 22);
            this.optionHex.Text = "hex";
            this.optionHex.Click += new System.EventHandler(this.optionHex_Click);
            // 
            // optionDec
            // 
            this.optionDec.CheckOnClick = true;
            this.optionDec.Name = "optionDec";
            this.optionDec.Size = new System.Drawing.Size(93, 22);
            this.optionDec.Text = "dec";
            this.optionDec.Click += new System.EventHandler(this.optionDec_Click);
            // 
            // HexNumericUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnBaseSystem);
            this.Controls.Add(this.lbl0x);
            this.Controls.Add(this.numericUpDown);
            this.Name = "HexNumericUpDown";
            this.Size = new System.Drawing.Size(185, 26);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
            this.baseMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown;
        private System.Windows.Forms.Label lbl0x;
        private System.Windows.Forms.Button btnBaseSystem;
        private System.Windows.Forms.ContextMenuStrip baseMenu;
        private System.Windows.Forms.ToolStripMenuItem optionHex;
        private System.Windows.Forms.ToolStripMenuItem optionDec;
    }
}
