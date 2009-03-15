namespace Decompiler.Gui.Windows.Forms
{
    partial class InitialPage
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
            this.SuspendLayout();
            this.txtInputFile = new System.Windows.Forms.TextBox();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.txtHeaderFile = new System.Windows.Forms.TextBox();
            this.txtLoadAddress = new System.Windows.Forms.TextBox();
            this.txtIntermediateFile = new System.Windows.Forms.TextBox();
            this.txtAssemblerFile = new System.Windows.Forms.TextBox();

            // 
            // txtInputFile
            // 
            this.txtInputFile.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputFile.Location = new System.Drawing.Point(24, 40);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.Size = new System.Drawing.Size(368, 20);
            this.txtInputFile.TabIndex = 3;
            this.dirtyManager.SetTrackChanges(this.txtInputFile, true);
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceFile.Location = new System.Drawing.Point(24, 80);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.Size = new System.Drawing.Size(368, 20);
            this.txtSourceFile.TabIndex = 11;
            this.dirtyManager.SetTrackChanges(this.txtSourceFile, true);
            // 
            // txtHeaderFile
            // 
            this.txtHeaderFile.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeaderFile.Location = new System.Drawing.Point(24, 40);
            this.txtHeaderFile.Name = "txtHeaderFile";
            this.txtHeaderFile.Size = new System.Drawing.Size(368, 20);
            this.txtHeaderFile.TabIndex = 8;
            this.dirtyManager.SetTrackChanges(this.txtHeaderFile, true);
            // 
            // txtLoadAddress
            // 
            this.txtLoadAddress.Location = new System.Drawing.Point(144, 70);
            this.txtLoadAddress.Name = "txtLoadAddress";
            this.txtLoadAddress.Size = new System.Drawing.Size(64, 20);
            this.txtLoadAddress.TabIndex = 6;
            this.dirtyManager.SetTrackChanges(this.txtLoadAddress, false);
            // 
            // txtIntermediateFile
            // 
            this.txtIntermediateFile.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIntermediateFile.Location = new System.Drawing.Point(24, 192);
            this.txtIntermediateFile.Name = "txtIntermediateFile";
            this.txtIntermediateFile.Size = new System.Drawing.Size(368, 20);
            this.txtIntermediateFile.TabIndex = 18;
            this.dirtyManager.SetTrackChanges(this.txtIntermediateFile, true);
            // 
            // txtAssemblerFile
            // 
            this.txtAssemblerFile.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAssemblerFile.Location = new System.Drawing.Point(24, 136);
            this.txtAssemblerFile.Name = "txtAssemblerFile";
            this.txtAssemblerFile.Size = new System.Drawing.Size(368, 20);
            this.txtAssemblerFile.TabIndex = 15;
            this.dirtyManager.SetTrackChanges(this.txtAssemblerFile, true);


            // 
            // InitialPage
            // 
            this.Name = "InitialPage";
            this.Size = new System.Drawing.Size(448, 360);
            this.ResumeLayout(false);
        }

        #endregion

        private Decompiler.Gui.Windows.Controls.DirtyManager dirtyManager;
        private Decompiler.Gui.Windows.Controls.RegexValidator addressValidator;
        private System.Windows.Forms.Button btnBrowseInputFile;
        private System.Windows.Forms.Button btnBrowseSourceFile;
        private System.Windows.Forms.TextBox txtSourceFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowseHeaderFile;
        private System.Windows.Forms.TextBox txtHeaderFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkAssemblerFile;
        private System.Windows.Forms.TextBox txtAssemblerFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkIntermediateFile;
        private System.Windows.Forms.TextBox txtInputFile;
        private System.Windows.Forms.Button btnBrowseAssemblerFile;
        private System.Windows.Forms.TextBox txtIntermediateFile;
        private System.Windows.Forms.Button btnBrowseIntermediateFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLoadAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog openFileDialog;

    }
}
