namespace Reko.Gui.Windows.Forms
{
    partial class EditProjectDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditProjectDialog));
            this.dirtyManager = new Reko.Gui.Windows.Controls.DirtyManager();
            this.txtIntermediateFile = new System.Windows.Forms.TextBox();
            this.txtAssemblerFile = new System.Windows.Forms.TextBox();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.txtHeaderFile = new System.Windows.Forms.TextBox();
            this.txtLoadAddress = new System.Windows.Forms.TextBox();
            this.txtInputFile = new System.Windows.Forms.TextBox();
            this.btnBrowseIntermediateFile = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBrowseAssemblerFile = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBrowseSourceFile = new System.Windows.Forms.Button();
            this.btnBrowseHeaderFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkAssemblerFile = new System.Windows.Forms.CheckBox();
            this.chkIntermediateFile = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBrowseInputFile = new System.Windows.Forms.Button();
            this.addressValidator = new Reko.Gui.Windows.Controls.RegexValidator();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtIntermediateFile
            // 
            this.txtIntermediateFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIntermediateFile.Location = new System.Drawing.Point(24, 192);
            this.txtIntermediateFile.Name = "txtIntermediateFile";
            this.txtIntermediateFile.Size = new System.Drawing.Size(326, 20);
            this.txtIntermediateFile.TabIndex = 18;
            this.dirtyManager.SetTrackChanges(this.txtIntermediateFile, true);
            // 
            // txtAssemblerFile
            // 
            this.txtAssemblerFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAssemblerFile.Location = new System.Drawing.Point(24, 136);
            this.txtAssemblerFile.Name = "txtAssemblerFile";
            this.txtAssemblerFile.Size = new System.Drawing.Size(326, 20);
            this.txtAssemblerFile.TabIndex = 15;
            this.dirtyManager.SetTrackChanges(this.txtAssemblerFile, true);
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceFile.Location = new System.Drawing.Point(24, 80);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.Size = new System.Drawing.Size(326, 20);
            this.txtSourceFile.TabIndex = 11;
            this.dirtyManager.SetTrackChanges(this.txtSourceFile, true);
            // 
            // txtHeaderFile
            // 
            this.txtHeaderFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeaderFile.Location = new System.Drawing.Point(11, 40);
            this.txtHeaderFile.Name = "txtHeaderFile";
            this.txtHeaderFile.Size = new System.Drawing.Size(339, 20);
            this.txtHeaderFile.TabIndex = 8;
            this.dirtyManager.SetTrackChanges(this.txtHeaderFile, true);
            // 
            // txtLoadAddress
            // 
            this.txtLoadAddress.Location = new System.Drawing.Point(125, 69);
            this.txtLoadAddress.Name = "txtLoadAddress";
            this.txtLoadAddress.Size = new System.Drawing.Size(75, 20);
            this.txtLoadAddress.TabIndex = 6;
            this.dirtyManager.SetTrackChanges(this.txtLoadAddress, false);
            // 
            // txtInputFile
            // 
            this.txtInputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputFile.Location = new System.Drawing.Point(11, 40);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.Size = new System.Drawing.Size(339, 20);
            this.txtInputFile.TabIndex = 3;
            this.dirtyManager.SetTrackChanges(this.txtInputFile, true);
            // 
            // btnBrowseIntermediateFile
            // 
            this.btnBrowseIntermediateFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseIntermediateFile.Location = new System.Drawing.Point(358, 192);
            this.btnBrowseIntermediateFile.Name = "btnBrowseIntermediateFile";
            this.btnBrowseIntermediateFile.Size = new System.Drawing.Size(24, 21);
            this.btnBrowseIntermediateFile.TabIndex = 19;
            this.btnBrowseIntermediateFile.Text = "...";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Binary file to be decompiled:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "Load at virtual &address:";
            // 
            // btnBrowseAssemblerFile
            // 
            this.btnBrowseAssemblerFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseAssemblerFile.Location = new System.Drawing.Point(358, 136);
            this.btnBrowseAssemblerFile.Name = "btnBrowseAssemblerFile";
            this.btnBrowseAssemblerFile.Size = new System.Drawing.Size(24, 21);
            this.btnBrowseAssemblerFile.TabIndex = 16;
            this.btnBrowseAssemblerFile.Text = "...";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnBrowseIntermediateFile);
            this.groupBox2.Controls.Add(this.txtIntermediateFile);
            this.groupBox2.Controls.Add(this.btnBrowseAssemblerFile);
            this.groupBox2.Controls.Add(this.txtAssemblerFile);
            this.groupBox2.Controls.Add(this.txtSourceFile);
            this.groupBox2.Controls.Add(this.txtHeaderFile);
            this.groupBox2.Controls.Add(this.btnBrowseSourceFile);
            this.groupBox2.Controls.Add(this.btnBrowseHeaderFile);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.chkAssemblerFile);
            this.groupBox2.Controls.Add(this.chkIntermediateFile);
            this.groupBox2.Location = new System.Drawing.Point(12, 124);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(390, 224);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Outputs";
            // 
            // btnBrowseSourceFile
            // 
            this.btnBrowseSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSourceFile.Location = new System.Drawing.Point(358, 80);
            this.btnBrowseSourceFile.Name = "btnBrowseSourceFile";
            this.btnBrowseSourceFile.Size = new System.Drawing.Size(24, 21);
            this.btnBrowseSourceFile.TabIndex = 12;
            this.btnBrowseSourceFile.Text = "...";
            // 
            // btnBrowseHeaderFile
            // 
            this.btnBrowseHeaderFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseHeaderFile.Location = new System.Drawing.Point(358, 40);
            this.btnBrowseHeaderFile.Name = "btnBrowseHeaderFile";
            this.btnBrowseHeaderFile.Size = new System.Drawing.Size(24, 21);
            this.btnBrowseHeaderFile.TabIndex = 9;
            this.btnBrowseHeaderFile.Text = "...";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Data types header file:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Source code file:";
            // 
            // chkAssemblerFile
            // 
            this.chkAssemblerFile.Location = new System.Drawing.Point(8, 112);
            this.chkAssemblerFile.Name = "chkAssemblerFile";
            this.chkAssemblerFile.Size = new System.Drawing.Size(136, 16);
            this.chkAssemblerFile.TabIndex = 13;
            this.chkAssemblerFile.Text = "Assembler dump file:";
            // 
            // chkIntermediateFile
            // 
            this.chkIntermediateFile.Location = new System.Drawing.Point(8, 168);
            this.chkIntermediateFile.Name = "chkIntermediateFile";
            this.chkIntermediateFile.Size = new System.Drawing.Size(136, 16);
            this.chkIntermediateFile.TabIndex = 17;
            this.chkIntermediateFile.Text = "Intermediate code file:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtLoadAddress);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtInputFile);
            this.groupBox1.Controls.Add(this.btnBrowseInputFile);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(390, 104);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Inputs";
            // 
            // btnBrowseInputFile
            // 
            this.btnBrowseInputFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseInputFile.Location = new System.Drawing.Point(358, 40);
            this.btnBrowseInputFile.Name = "btnBrowseInputFile";
            this.btnBrowseInputFile.Size = new System.Drawing.Size(24, 21);
            this.btnBrowseInputFile.TabIndex = 2;
            this.btnBrowseInputFile.Text = "...";
            // 
            // addressValidator
            // 
            this.addressValidator.ControlToValidate = this.txtLoadAddress;
            this.addressValidator.ErrorMessage = "Invalid address specified.";
            this.addressValidator.IsValid = true;
            this.addressValidator.ValidationExpression = "[0-9A-Fa-f]+(\\:[0-9A-Fa-f]+)?";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(326, 375);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(245, 375);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 20;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // EditProjectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 410);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditProjectDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Project Properties";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Reko.Gui.Windows.Controls.DirtyManager dirtyManager;
        private System.Windows.Forms.TextBox txtIntermediateFile;
        private System.Windows.Forms.TextBox txtAssemblerFile;
        private System.Windows.Forms.TextBox txtSourceFile;
        private System.Windows.Forms.TextBox txtHeaderFile;
        private System.Windows.Forms.TextBox txtLoadAddress;
        private System.Windows.Forms.TextBox txtInputFile;
        private System.Windows.Forms.Button btnBrowseIntermediateFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBrowseAssemblerFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnBrowseSourceFile;
        private System.Windows.Forms.Button btnBrowseHeaderFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkAssemblerFile;
        private System.Windows.Forms.CheckBox chkIntermediateFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnBrowseInputFile;
        private Reko.Gui.Windows.Controls.RegexValidator addressValidator;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}