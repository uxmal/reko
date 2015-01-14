namespace Decompiler.Gui.Windows.Forms
{
    partial class UserPreferencesDialog
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
            Decompiler.Gui.Windows.Controls.EmptyEditorModel emptyEditorModel1 = new Decompiler.Gui.Windows.Controls.EmptyEditorModel();
            Decompiler.Gui.Windows.Controls.EmptyEditorModel emptyEditorModel2 = new Decompiler.Gui.Windows.Controls.EmptyEditorModel();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Code");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Data");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Memory Window", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Address");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Disassembly Window", new System.Windows.Forms.TreeNode[] {
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Keywords");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Comments");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Code Window", new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7});
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabWindows = new System.Windows.Forms.TabPage();
            this.dasmCtl = new Decompiler.Gui.Windows.Controls.DisassemblyControl();
            this.codeCtl = new Decompiler.Gui.Windows.Controls.TextView();
            this.memCtl = new Decompiler.Gui.Windows.Controls.MemoryControl();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btnWindowBgColor = new System.Windows.Forms.Button();
            this.btnWindowFgColor = new System.Windows.Forms.Button();
            this.btnWindowFont = new System.Windows.Forms.Button();
            this.tabColors = new System.Windows.Forms.TabPage();
            this.btnElementBgColor = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnElementFgColor = new System.Windows.Forms.Button();
            this.lbxUiElements = new System.Windows.Forms.ListBox();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            this.fontPicker = new System.Windows.Forms.FontDialog();
            this.tabControl1.SuspendLayout();
            this.tabWindows.SuspendLayout();
            this.tabColors.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(396, 226);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(477, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabWindows);
            this.tabControl1.Controls.Add(this.tabColors);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(565, 220);
            this.tabControl1.TabIndex = 2;
            // 
            // tabWindows
            // 
            this.tabWindows.Controls.Add(this.dasmCtl);
            this.tabWindows.Controls.Add(this.codeCtl);
            this.tabWindows.Controls.Add(this.memCtl);
            this.tabWindows.Controls.Add(this.treeView1);
            this.tabWindows.Controls.Add(this.btnWindowBgColor);
            this.tabWindows.Controls.Add(this.btnWindowFgColor);
            this.tabWindows.Controls.Add(this.btnWindowFont);
            this.tabWindows.Location = new System.Drawing.Point(4, 22);
            this.tabWindows.Name = "tabWindows";
            this.tabWindows.Padding = new System.Windows.Forms.Padding(3);
            this.tabWindows.Size = new System.Drawing.Size(557, 194);
            this.tabWindows.TabIndex = 0;
            this.tabWindows.Text = "Windows";
            this.tabWindows.UseVisualStyleBackColor = true;
            // 
            // dasmCtl
            // 
            this.dasmCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dasmCtl.Location = new System.Drawing.Point(180, 37);
            this.dasmCtl.Model = emptyEditorModel1;
            this.dasmCtl.Name = "dasmCtl";
            this.dasmCtl.SelectedObject = null;
            this.dasmCtl.Services = null;
            this.dasmCtl.Size = new System.Drawing.Size(374, 138);
            this.dasmCtl.StartAddress = null;
            this.dasmCtl.TabIndex = 8;
            this.dasmCtl.TopAddress = null;
            // 
            // codeCtl
            // 
            this.codeCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.codeCtl.Location = new System.Drawing.Point(183, 37);
            this.codeCtl.Model = emptyEditorModel2;
            this.codeCtl.Name = "codeCtl";
            this.codeCtl.Services = null;
            this.codeCtl.Size = new System.Drawing.Size(371, 142);
            this.codeCtl.TabIndex = 7;
            this.codeCtl.Text = "textView1";
            // 
            // memCtl
            // 
            this.memCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memCtl.Architecture = null;
            this.memCtl.BytesPerRow = ((uint)(16u));
            this.memCtl.ImageMap = null;
            this.memCtl.Location = new System.Drawing.Point(183, 37);
            this.memCtl.Name = "memCtl";
            this.memCtl.ProgramImage = null;
            this.memCtl.SelectedAddress = null;
            this.memCtl.Services = null;
            this.memCtl.Size = new System.Drawing.Size(371, 142);
            this.memCtl.TabIndex = 6;
            this.memCtl.Text = "memoryControl1";
            this.memCtl.TopAddress = null;
            this.memCtl.WordSize = ((uint)(1u));
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView1.Location = new System.Drawing.Point(7, 7);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "nMemCode";
            treeNode1.Tag = "code";
            treeNode1.Text = "Code";
            treeNode2.Name = "nMemData";
            treeNode2.Tag = "memData";
            treeNode2.Text = "Data";
            treeNode3.Name = "nMemory";
            treeNode3.Tag = "mem";
            treeNode3.Text = "Memory Window";
            treeNode4.Name = "nDasmAddr";
            treeNode4.Tag = "dasmAddr";
            treeNode4.Text = "Address";
            treeNode5.Name = "nDasm";
            treeNode5.Tag = "dasm";
            treeNode5.Text = "Disassembly Window";
            treeNode6.Name = "nCodeKeyword";
            treeNode6.Tag = "kw";
            treeNode6.Text = "Keywords";
            treeNode7.Name = "nCodeComment";
            treeNode7.Tag = "comment";
            treeNode7.Text = "Comments";
            treeNode8.Name = "nCode";
            treeNode8.Tag = "code";
            treeNode8.Text = "Code Window";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode5,
            treeNode8});
            this.treeView1.ShowLines = false;
            this.treeView1.ShowPlusMinus = false;
            this.treeView1.Size = new System.Drawing.Size(164, 172);
            this.treeView1.TabIndex = 5;
            // 
            // btnWindowBgColor
            // 
            this.btnWindowBgColor.Location = new System.Drawing.Point(342, 6);
            this.btnWindowBgColor.Name = "btnWindowBgColor";
            this.btnWindowBgColor.Size = new System.Drawing.Size(80, 23);
            this.btnWindowBgColor.TabIndex = 3;
            this.btnWindowBgColor.Text = "Background...";
            this.btnWindowBgColor.UseVisualStyleBackColor = true;
            // 
            // btnWindowFgColor
            // 
            this.btnWindowFgColor.Location = new System.Drawing.Point(261, 7);
            this.btnWindowFgColor.Name = "btnWindowFgColor";
            this.btnWindowFgColor.Size = new System.Drawing.Size(75, 23);
            this.btnWindowFgColor.TabIndex = 2;
            this.btnWindowFgColor.Text = "Foreground...";
            this.btnWindowFgColor.UseVisualStyleBackColor = true;
            // 
            // btnWindowFont
            // 
            this.btnWindowFont.Location = new System.Drawing.Point(180, 7);
            this.btnWindowFont.Name = "btnWindowFont";
            this.btnWindowFont.Size = new System.Drawing.Size(75, 23);
            this.btnWindowFont.TabIndex = 1;
            this.btnWindowFont.Text = "Font...";
            this.btnWindowFont.UseVisualStyleBackColor = true;
            // 
            // tabColors
            // 
            this.tabColors.Controls.Add(this.btnElementBgColor);
            this.tabColors.Controls.Add(this.label2);
            this.tabColors.Controls.Add(this.btnElementFgColor);
            this.tabColors.Controls.Add(this.lbxUiElements);
            this.tabColors.Location = new System.Drawing.Point(4, 22);
            this.tabColors.Name = "tabColors";
            this.tabColors.Padding = new System.Windows.Forms.Padding(3);
            this.tabColors.Size = new System.Drawing.Size(557, 194);
            this.tabColors.TabIndex = 1;
            this.tabColors.Text = "Image Bar";
            this.tabColors.UseVisualStyleBackColor = true;
            // 
            // btnElementBgColor
            // 
            this.btnElementBgColor.Location = new System.Drawing.Point(265, 6);
            this.btnElementBgColor.Name = "btnElementBgColor";
            this.btnElementBgColor.Size = new System.Drawing.Size(85, 23);
            this.btnElementBgColor.TabIndex = 4;
            this.btnElementBgColor.Text = "&Background...";
            this.btnElementBgColor.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(177, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(371, 146);
            this.label2.TabIndex = 3;
            this.label2.Text = "lblImageBarPreview";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnElementFgColor
            // 
            this.btnElementFgColor.Location = new System.Drawing.Point(177, 6);
            this.btnElementFgColor.Name = "btnElementFgColor";
            this.btnElementFgColor.Size = new System.Drawing.Size(82, 23);
            this.btnElementFgColor.TabIndex = 2;
            this.btnElementFgColor.Text = "&Foreground...";
            this.btnElementFgColor.UseVisualStyleBackColor = true;
            // 
            // lbxUiElements
            // 
            this.lbxUiElements.FormattingEnabled = true;
            this.lbxUiElements.IntegralHeight = false;
            this.lbxUiElements.Items.AddRange(new object[] {
            "Code",
            "Data",
            "Address",
            "Unknown"});
            this.lbxUiElements.Location = new System.Drawing.Point(7, 7);
            this.lbxUiElements.Name = "lbxUiElements";
            this.lbxUiElements.Size = new System.Drawing.Size(164, 172);
            this.lbxUiElements.TabIndex = 1;
            // 
            // UserPreferencesDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 261);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserPreferencesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User Preferences";
            this.tabControl1.ResumeLayout(false);
            this.tabWindows.ResumeLayout(false);
            this.tabColors.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabWindows;
        private System.Windows.Forms.TabPage tabColors;
        private System.Windows.Forms.Button btnWindowBgColor;
        private System.Windows.Forms.Button btnWindowFgColor;
        private System.Windows.Forms.Button btnWindowFont;
        private System.Windows.Forms.ListBox lbxUiElements;
        private System.Windows.Forms.Button btnElementBgColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnElementFgColor;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ColorDialog colorPicker;
        private System.Windows.Forms.FontDialog fontPicker;
        private Controls.DisassemblyControl dasmCtl;
        private Controls.TextView codeCtl;
        private Controls.MemoryControl memCtl;
    }
}