namespace Decompiler.Gui.Windows.Forms
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.listBrowser = new System.Windows.Forms.ListView();
            this.colBrowserHeader = new System.Windows.Forms.ColumnHeader();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabFindResults = new System.Windows.Forms.TabPage();
            this.listFindResults = new System.Windows.Forms.ListView();
            this.tabDiagnostics = new System.Windows.Forms.TabPage();
            this.listDiagnostics = new System.Windows.Forms.ListView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.statusStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabFindResults.SuspendLayout();
            this.tabDiagnostics.SuspendLayout();
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
            // listBrowser
            // 
            this.listBrowser.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colBrowserHeader});
            this.listBrowser.Dock = System.Windows.Forms.DockStyle.Left;
            this.listBrowser.Location = new System.Drawing.Point(3, 0);
            this.listBrowser.Name = "listBrowser";
            this.listBrowser.Size = new System.Drawing.Size(200, 328);
            this.listBrowser.TabIndex = 4;
            this.listBrowser.UseCompatibleStateImageBehavior = false;
            this.listBrowser.View = System.Windows.Forms.View.Details;
            // 
            // colBrowserHeader
            // 
            this.colBrowserHeader.Text = "";
            this.colBrowserHeader.Width = 196;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabDiagnostics);
            this.tabControl1.Controls.Add(this.tabFindResults);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Location = new System.Drawing.Point(0, 331);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(784, 180);
            this.tabControl1.TabIndex = 5;
            // 
            // tabFindResults
            // 
            this.tabFindResults.Controls.Add(this.listFindResults);
            this.tabFindResults.Location = new System.Drawing.Point(4, 22);
            this.tabFindResults.Name = "tabFindResults";
            this.tabFindResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabFindResults.Size = new System.Drawing.Size(776, 154);
            this.tabFindResults.TabIndex = 0;
            this.tabFindResults.Text = "Find results";
            this.tabFindResults.UseVisualStyleBackColor = true;
            // 
            // listFindResults
            // 
            this.listFindResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listFindResults.FullRowSelect = true;
            this.listFindResults.Location = new System.Drawing.Point(3, 3);
            this.listFindResults.Name = "listFindResults";
            this.listFindResults.Size = new System.Drawing.Size(770, 148);
            this.listFindResults.TabIndex = 0;
            this.listFindResults.UseCompatibleStateImageBehavior = false;
            // 
            // tabDiagnostics
            // 
            this.tabDiagnostics.Controls.Add(this.listDiagnostics);
            this.tabDiagnostics.Location = new System.Drawing.Point(4, 22);
            this.tabDiagnostics.Name = "tabDiagnostics";
            this.tabDiagnostics.Padding = new System.Windows.Forms.Padding(3);
            this.tabDiagnostics.Size = new System.Drawing.Size(776, 154);
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
            this.listDiagnostics.Size = new System.Drawing.Size(770, 148);
            this.listDiagnostics.SmallImageList = this.imageList;
            this.listDiagnostics.TabIndex = 2;
            this.listDiagnostics.UseCompatibleStateImageBehavior = false;
            this.listDiagnostics.View = System.Windows.Forms.View.Details;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 331);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(3, 328);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(781, 3);
            this.splitter2.TabIndex = 7;
            this.splitter2.TabStop = false;
            // 
            // ofd
            // 
            this.ofd.FileName = "openFileDialog1";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Open.ico");
            this.imageList.Images.SetKeyName(1, "Save.ico");
            this.imageList.Images.SetKeyName(2, "NextPhase.ico");
            this.imageList.Images.SetKeyName(3, "FinishDecompilation.ico");
            this.imageList.Images.SetKeyName(4, "Error");
            this.imageList.Images.SetKeyName(5, "Warning");
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 533);
            this.Controls.Add(this.listBrowser);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "MainForm";
            this.Text = "Decompiler";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabFindResults.ResumeLayout(false);
            this.tabDiagnostics.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ListView listBrowser;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabFindResults;
        private System.Windows.Forms.TabPage tabDiagnostics;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ListView listFindResults;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ListView listDiagnostics;
        private System.Windows.Forms.ColumnHeader colBrowserHeader;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}



