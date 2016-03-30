namespace VisObj64.Visualization.OpenGL
{
    partial class OpenGLControl
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
            this._glDisplay = new OpenTK.GLControl();
            this.SuspendLayout();
            // 
            // glDisplay
            // 
            this._glDisplay.BackColor = System.Drawing.Color.Black;
            this._glDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this._glDisplay.Location = new System.Drawing.Point(0, 0);
            this._glDisplay.Name = "glDisplay";
            this._glDisplay.Size = new System.Drawing.Size(444, 279);
            this._glDisplay.TabIndex = 1;
            this._glDisplay.VSync = true;
            this._glDisplay.Load += new System.EventHandler(this.glDisplay_Load);
            this._glDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.glDisplay_Paint);
            this._glDisplay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glDisplay_KeyDown);
            this._glDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glDisplay_MouseDown);
            this._glDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glDisplay_MouseMove);
            this._glDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glDisplay_MouseUp);
            // 
            // OpenGLControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._glDisplay);
            this.Name = "OpenGLControl";
            this.Size = new System.Drawing.Size(444, 279);
            this.Resize += new System.EventHandler(this.OpenGLControl_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl _glDisplay;
    }
}
