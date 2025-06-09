namespace Reko.UiPrototype.WinForms
{
    partial class MainForm
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
            if (disposing && (components is not null))
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "fn00410120",
            "fn00410120",
            "int __cdecl fn00410120(char * szArg08)"}, -1);
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("fn00401000 (entry point)");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("fn00401400");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode(".text segment", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode(".data segment");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("PE Executable", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("fn05AC_0000 (entry point)");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("fn05AC_00C8");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("05AC - segment", new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("060B - segment");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("MS-DOS Executable Stub", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("mainprog.exe", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode10});
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("addin.dll");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.listSearchResults = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.callGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findExecutableCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.followReverseJumpsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncannedBlocksOfCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.timerSearchResults = new System.Windows.Forms.Timer(this.components);
            this.ctxmProc = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showDisassemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCallGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.ctxmProc.SuspendLayout();
            this.SuspendLayout();
            // 
            // listSearchResults
            // 
            this.listSearchResults.CheckBoxes = true;
            this.listSearchResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listSearchResults.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listSearchResults.FullRowSelect = true;
            this.listSearchResults.HideSelection = false;
            listViewItem1.StateImageIndex = 0;
            this.listSearchResults.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.listSearchResults.Location = new System.Drawing.Point(3, 421);
            this.listSearchResults.Name = "listSearchResults";
            this.listSearchResults.Size = new System.Drawing.Size(1156, 132);
            this.listSearchResults.TabIndex = 0;
            this.listSearchResults.UseCompatibleStateImageBehavior = false;
            this.listSearchResults.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Address";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 160;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Details";
            this.columnHeader3.Width = 617;
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList;
            this.treeView1.Location = new System.Drawing.Point(3, 24);
            this.treeView1.Name = "treeView1";
            treeNode1.ImageKey = "Code.ico";
            treeNode1.Name = "Node1";
            treeNode1.SelectedImageKey = "Code.ico";
            treeNode1.Text = "fn00401000 (entry point)";
            treeNode2.ImageKey = "Code.ico";
            treeNode2.Name = "Node2";
            treeNode2.SelectedImageKey = "Code.ico";
            treeNode2.Text = "fn00401400";
            treeNode3.ImageKey = "Code.ico";
            treeNode3.Name = "Node4";
            treeNode3.SelectedImageKey = "Code.ico";
            treeNode3.Text = ".text segment";
            treeNode4.ImageKey = "Data.ico";
            treeNode4.Name = "Node0";
            treeNode4.SelectedImageKey = "Data.ico";
            treeNode4.Text = ".data segment";
            treeNode5.ImageKey = "Header.ico";
            treeNode5.Name = "Node2";
            treeNode5.SelectedImageKey = "Header.ico";
            treeNode5.Text = "PE Executable";
            treeNode6.ImageKey = "Code.ico";
            treeNode6.Name = "Node5";
            treeNode6.SelectedImageKey = "Code.ico";
            treeNode6.Text = "fn05AC_0000 (entry point)";
            treeNode7.ImageKey = "Code.ico";
            treeNode7.Name = "Node6";
            treeNode7.SelectedImageKey = "Code.ico";
            treeNode7.Text = "fn05AC_00C8";
            treeNode8.Name = "Node3";
            treeNode8.Text = "05AC - segment";
            treeNode9.Name = "Node4";
            treeNode9.Text = "060B - segment";
            treeNode10.ImageKey = "Header.ico";
            treeNode10.Name = "Node3";
            treeNode10.SelectedImageKey = "Header.ico";
            treeNode10.Text = "MS-DOS Executable Stub";
            treeNode11.Name = "Node0";
            treeNode11.Text = "mainprog.exe";
            treeNode12.Name = "Node1";
            treeNode12.Text = "addin.dll";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode11,
            treeNode12});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(300, 397);
            this.treeView1.TabIndex = 1;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "binary.ico");
            this.imageList.Images.SetKeyName(1, "Header.ico");
            this.imageList.Images.SetKeyName(2, "Code.ico");
            this.imageList.Images.SetKeyName(3, "Data.ico");
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 24);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 529);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.actionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1159, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.openAsToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveasToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            // 
            // openAsToolStripMenuItem
            // 
            this.openAsToolStripMenuItem.Name = "openAsToolStripMenuItem";
            this.openAsToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.openAsToolStripMenuItem.Text = "Op&en as...";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveasToolStripMenuItem
            // 
            this.saveasToolStripMenuItem.Name = "saveasToolStripMenuItem";
            this.saveasToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.saveasToolStripMenuItem.Text = "Save &as...";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(123, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.toolStripMenuItem2,
            this.propertiesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.renameToolStripMenuItem.Text = "&Rename";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(133, 6);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.propertiesToolStripMenuItem.Text = "P&roperties...";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.callGraphToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // callGraphToolStripMenuItem
            // 
            this.callGraphToolStripMenuItem.Enabled = false;
            this.callGraphToolStripMenuItem.Name = "callGraphToolStripMenuItem";
            this.callGraphToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.callGraphToolStripMenuItem.Text = "Call graph";
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stringToolStripMenuItem,
            this.findExecutableCodeToolStripMenuItem,
            this.uncannedBlocksOfCodeToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.actionsToolStripMenuItem.Text = "&Search";
            // 
            // stringToolStripMenuItem
            // 
            this.stringToolStripMenuItem.Name = "stringToolStripMenuItem";
            this.stringToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.stringToolStripMenuItem.Text = "&Pattern...";
            this.stringToolStripMenuItem.Click += new System.EventHandler(this.stringToolStripMenuItem_Click);
            // 
            // findExecutableCodeToolStripMenuItem
            // 
            this.findExecutableCodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.followReverseJumpsToolStripMenuItem});
            this.findExecutableCodeToolStripMenuItem.Name = "findExecutableCodeToolStripMenuItem";
            this.findExecutableCodeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.findExecutableCodeToolStripMenuItem.Text = "&Executable code";
            // 
            // followReverseJumpsToolStripMenuItem
            // 
            this.followReverseJumpsToolStripMenuItem.Name = "followReverseJumpsToolStripMenuItem";
            this.followReverseJumpsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.followReverseJumpsToolStripMenuItem.Text = "Follow reverse jumps";
            // 
            // uncannedBlocksOfCodeToolStripMenuItem
            // 
            this.uncannedBlocksOfCodeToolStripMenuItem.Name = "uncannedBlocksOfCodeToolStripMenuItem";
            this.uncannedBlocksOfCodeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.uncannedBlocksOfCodeToolStripMenuItem.Text = "&Uncanned blocks of code";
            this.uncannedBlocksOfCodeToolStripMenuItem.Click += new System.EventHandler(this.uncannedBlocksOfCodeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(303, 418);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(856, 3);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // timerSearchResults
            // 
            this.timerSearchResults.Interval = 200;
            this.timerSearchResults.Tick += new System.EventHandler(this.timerSearchResults_Tick);
            // 
            // ctxmProc
            // 
            this.ctxmProc.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showDisassemblyToolStripMenuItem,
            this.showCallGraphToolStripMenuItem});
            this.ctxmProc.Name = "ctxmProc";
            this.ctxmProc.Size = new System.Drawing.Size(172, 48);
            // 
            // showDisassemblyToolStripMenuItem
            // 
            this.showDisassemblyToolStripMenuItem.Name = "showDisassemblyToolStripMenuItem";
            this.showDisassemblyToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.showDisassemblyToolStripMenuItem.Text = "Show &Disassembly";
            // 
            // showCallGraphToolStripMenuItem
            // 
            this.showCallGraphToolStripMenuItem.Name = "showCallGraphToolStripMenuItem";
            this.showCallGraphToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.showCallGraphToolStripMenuItem.Text = "&Show call graph";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1159, 553);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.listSearchResults);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Reko Decompiler";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ctxmProc.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listSearchResults;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findExecutableCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem followReverseJumpsToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stringToolStripMenuItem;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Timer timerSearchResults;
        private System.Windows.Forms.ToolStripMenuItem callGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncannedBlocksOfCodeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ctxmProc;
        private System.Windows.Forms.ToolStripMenuItem showDisassemblyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showCallGraphToolStripMenuItem;
    }
}

