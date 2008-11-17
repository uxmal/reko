namespace Decompiler.WindowsGui.Forms
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBrowser = new System.Windows.Forms.ListView();
            this.listBrowserItem = new System.Windows.Forms.ColumnHeader();
            this.toolbar = new System.Windows.Forms.ToolStrip();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.progressStatusBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDiagnostics = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colDiagnosticDescription = new System.Windows.Forms.ColumnHeader();
            this.colDiagnosticAddress = new System.Windows.Forms.ColumnHeader();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabDiagnostics.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(53, 27);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBrowser);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Size = new System.Drawing.Size(641, 223);
            this.splitContainer1.SplitterDistance = 213;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBrowser
            // 
            this.listBrowser.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listBrowserItem});
            this.listBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBrowser.Location = new System.Drawing.Point(0, 0);
            this.listBrowser.Name = "listBrowser";
            this.listBrowser.Size = new System.Drawing.Size(213, 223);
            this.listBrowser.TabIndex = 0;
            this.listBrowser.UseCompatibleStateImageBehavior = false;
            this.listBrowser.View = System.Windows.Forms.View.Details;
            // 
            // listBrowserItem
            // 
            this.listBrowserItem.Text = "Item";
            this.listBrowserItem.Width = 113;
            // 
            // toolbar
            // 
            this.toolbar.Location = new System.Drawing.Point(0, 0);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(721, 25);
            this.toolbar.TabIndex = 1;
            this.toolbar.Text = "toolStrip1";
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.progressStatusBar});
            this.statusBar.Location = new System.Drawing.Point(0, 485);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(721, 22);
            this.statusBar.TabIndex = 2;
            this.statusBar.Text = "statusStrip1";
            // 
            // progressStatusBar
            // 
            this.progressStatusBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.progressStatusBar.Name = "progressStatusBar";
            this.progressStatusBar.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // ofd
            // 
            this.ofd.FileName = "openFileDialog1";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Location = new System.Drawing.Point(12, 44);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(697, 420);
            this.splitContainer2.SplitterDistance = 210;
            this.splitContainer2.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabDiagnostics);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 14);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(688, 192);
            this.tabControl1.TabIndex = 0;
            // 
            // tabDiagnostics
            // 
            this.tabDiagnostics.Controls.Add(this.listView1);
            this.tabDiagnostics.Location = new System.Drawing.Point(4, 22);
            this.tabDiagnostics.Name = "tabDiagnostics";
            this.tabDiagnostics.Padding = new System.Windows.Forms.Padding(3);
            this.tabDiagnostics.Size = new System.Drawing.Size(680, 166);
            this.tabDiagnostics.TabIndex = 0;
            this.tabDiagnostics.Text = "Diagnostics";
            this.tabDiagnostics.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(551, 100);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDiagnosticDescription,
            this.colDiagnosticAddress});
            this.listView1.Location = new System.Drawing.Point(15, 7);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(354, 97);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // colDiagnosticDescription
            // 
            this.colDiagnosticDescription.Text = "Description";
            this.colDiagnosticDescription.Width = 279;
            // 
            // colDiagnosticAddress
            // 
            this.colDiagnosticAddress.Text = "Address";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 507);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.toolbar);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabDiagnostics.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listBrowser;
        private System.Windows.Forms.ColumnHeader listBrowserItem;
        private System.Windows.Forms.ToolStrip toolbar;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripProgressBar progressStatusBar;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabDiagnostics;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colDiagnosticDescription;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ColumnHeader colDiagnosticAddress;
    }
}