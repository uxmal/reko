namespace Reko.Gui.Windows.Forms
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.treeBrowser = new System.Windows.Forms.TreeView();
            this.imlBrowser = new System.Windows.Forms.ImageList(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDiagnostics = new System.Windows.Forms.TabPage();
            this.listDiagnostics = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tabFindResults = new System.Windows.Forms.TabPage();
            this.listFindResults = new System.Windows.Forms.ListView();
            this.tabConsole = new System.Windows.Forms.TabPage();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabDocuments = new System.Windows.Forms.TabControl();
            this.statusStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabDiagnostics.SuspendLayout();
            this.tabFindResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 511);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel.Text = "Status";
            // 
            // treeBrowser
            // 
            this.treeBrowser.AllowDrop = true;
            this.treeBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeBrowser.ImageIndex = 0;
            this.treeBrowser.ImageList = this.imlBrowser;
            this.treeBrowser.Location = new System.Drawing.Point(0, 0);
            this.treeBrowser.Name = "treeBrowser";
            this.treeBrowser.SelectedImageIndex = 0;
            this.treeBrowser.Size = new System.Drawing.Size(179, 339);
            this.treeBrowser.TabIndex = 0;
            // 
            // imlBrowser
            // 
            this.imlBrowser.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlBrowser.ImageStream")));
            this.imlBrowser.TransparentColor = System.Drawing.Color.Transparent;
            this.imlBrowser.Images.SetKeyName(0, "Binary.ico");
            this.imlBrowser.Images.SetKeyName(1, "Userproc.ico");
            this.imlBrowser.Images.SetKeyName(2, "Data.ico");
            this.imlBrowser.Images.SetKeyName(3, "Procedure.ico");
            this.imlBrowser.Images.SetKeyName(4, "typelib.ico");
            this.imlBrowser.Images.SetKeyName(5, "RoSection.ico");
            this.imlBrowser.Images.SetKeyName(6, "RwSection.ico");
            this.imlBrowser.Images.SetKeyName(7, "RxSection.ico");
            this.imlBrowser.Images.SetKeyName(8, "WxSection.ico");
            this.imlBrowser.Images.SetKeyName(9, "DiscardableSection.ico");
            this.imlBrowser.Images.SetKeyName(10, "Platform.ico");
            this.imlBrowser.Images.SetKeyName(11, "Cpu.ico");
            this.imlBrowser.Images.SetKeyName(12, "EntryProcedure.ico");
            this.imlBrowser.Images.SetKeyName(13, "UserEntryProcedure.ico");
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabDiagnostics);
            this.tabControl1.Controls.Add(this.tabFindResults);
            this.tabControl1.Controls.Add(this.tabConsole);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(784, 168);
            this.tabControl1.TabIndex = 5;
            // 
            // tabDiagnostics
            // 
            this.tabDiagnostics.Controls.Add(this.listDiagnostics);
            this.tabDiagnostics.Location = new System.Drawing.Point(4, 22);
            this.tabDiagnostics.Name = "tabDiagnostics";
            this.tabDiagnostics.Padding = new System.Windows.Forms.Padding(3);
            this.tabDiagnostics.Size = new System.Drawing.Size(776, 142);
            this.tabDiagnostics.TabIndex = 1;
            this.tabDiagnostics.Text = "Diagostics";
            this.tabDiagnostics.UseVisualStyleBackColor = true;
            // 
            // listDiagnostics
            // 
            this.listDiagnostics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listDiagnostics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDiagnostics.FullRowSelect = true;
            this.listDiagnostics.Location = new System.Drawing.Point(3, 3);
            this.listDiagnostics.Name = "listDiagnostics";
            this.listDiagnostics.Size = new System.Drawing.Size(770, 136);
            this.listDiagnostics.SmallImageList = this.imageList;
            this.listDiagnostics.TabIndex = 2;
            this.listDiagnostics.UseCompatibleStateImageBehavior = false;
            this.listDiagnostics.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Address";
            this.columnHeader1.Width = 95;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Description";
            this.columnHeader2.Width = 624;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Open.ico");
            this.imageList.Images.SetKeyName(1, "Save.ico");
            this.imageList.Images.SetKeyName(2, "RestartDecompilation.ico");
            this.imageList.Images.SetKeyName(3, "NextPhase.ico");
            this.imageList.Images.SetKeyName(4, "FinishDecompilation.ico");
            this.imageList.Images.SetKeyName(5, "Error");
            this.imageList.Images.SetKeyName(6, "Warning");
            this.imageList.Images.SetKeyName(7, "CloseTab");
            this.imageList.Images.SetKeyName(8, "Collapse.ico");
            // 
            // tabFindResults
            // 
            this.tabFindResults.Controls.Add(this.listFindResults);
            this.tabFindResults.Location = new System.Drawing.Point(4, 22);
            this.tabFindResults.Name = "tabFindResults";
            this.tabFindResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabFindResults.Size = new System.Drawing.Size(776, 142);
            this.tabFindResults.TabIndex = 0;
            this.tabFindResults.Text = "Find results";
            this.tabFindResults.UseVisualStyleBackColor = true;
            // 
            // listFindResults
            // 
            this.listFindResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listFindResults.FullRowSelect = true;
            this.listFindResults.HideSelection = false;
            this.listFindResults.Location = new System.Drawing.Point(3, 3);
            this.listFindResults.Name = "listFindResults";
            this.listFindResults.Size = new System.Drawing.Size(770, 136);
            this.listFindResults.TabIndex = 0;
            this.listFindResults.UseCompatibleStateImageBehavior = false;
            // 
            // tabConsole
            // 
            this.tabConsole.Location = new System.Drawing.Point(4, 22);
            this.tabConsole.Name = "tabConsole";
            this.tabConsole.Size = new System.Drawing.Size(776, 142);
            this.tabConsole.TabIndex = 2;
            this.tabConsole.Text = "Console";
            this.tabConsole.UseVisualStyleBackColor = true;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.tabControl1);
            this.splitContainerMain.Size = new System.Drawing.Size(784, 511);
            this.splitContainerMain.SplitterDistance = 339;
            this.splitContainerMain.TabIndex = 9;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeBrowser);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabDocuments);
            this.splitContainer2.Size = new System.Drawing.Size(784, 339);
            this.splitContainer2.SplitterDistance = 179;
            this.splitContainer2.TabIndex = 0;
            // 
            // tabDocuments
            // 
            this.tabDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDocuments.ImageList = this.imageList;
            this.tabDocuments.Location = new System.Drawing.Point(0, 0);
            this.tabDocuments.Name = "tabDocuments";
            this.tabDocuments.SelectedIndex = 0;
            this.tabDocuments.Size = new System.Drawing.Size(601, 339);
            this.tabDocuments.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 533);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "MainForm";
            this.Text = "Reko Decompiler";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabDiagnostics.ResumeLayout(false);
            this.tabFindResults.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.TreeView treeBrowser;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabFindResults;
        private System.Windows.Forms.TabPage tabDiagnostics;
        private System.Windows.Forms.ListView listFindResults;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ListView listDiagnostics;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ImageList imlBrowser;
        private System.Windows.Forms.TabPage tabConsole;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabControl tabDocuments;
    }
}



