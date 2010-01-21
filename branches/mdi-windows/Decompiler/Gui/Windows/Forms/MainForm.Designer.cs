/* 
 * Copyright (C) 1999-2010 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */

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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatusDetails = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listBrowser = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.finalPage1 = new Decompiler.Gui.Windows.Forms.FinalPage();
            this.analyzedPage1 = new Decompiler.Gui.Windows.Forms.AnalyzedPage();
            this.startPage = new Decompiler.Gui.Windows.Forms.StartPage();
            this.loadedPage1 = new Decompiler.Gui.Windows.Forms.LoadedPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDiagnostics = new System.Windows.Forms.TabPage();
            this.listDiagnostics = new System.Windows.Forms.ListView();
            this.colDiagnosticType = new System.Windows.Forms.ColumnHeader();
            this.colDiagnosticAddress = new System.Windows.Forms.ColumnHeader();
            this.colDiagnosticDescription = new System.Windows.Forms.ColumnHeader();
            this.tabFindResults = new System.Windows.Forms.TabPage();
            this.listFindResults = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabDiagnostics.SuspendLayout();
            this.tabFindResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblStatusDetails});
            this.statusStrip1.Location = new System.Drawing.Point(0, 490);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(725, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // lblStatusDetails
            // 
            this.lblStatusDetails.Name = "lblStatusDetails";
            this.lblStatusDetails.Size = new System.Drawing.Size(0, 17);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(725, 490);
            this.splitContainer1.SplitterDistance = 345;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listBrowser);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.finalPage1);
            this.splitContainer2.Panel2.Controls.Add(this.analyzedPage1);
            this.splitContainer2.Panel2.Controls.Add(this.startPage);
            this.splitContainer2.Panel2.Controls.Add(this.loadedPage1);
            this.splitContainer2.Size = new System.Drawing.Size(725, 345);
            this.splitContainer2.SplitterDistance = 169;
            this.splitContainer2.TabIndex = 0;
            // 
            // listBrowser
            // 
            this.listBrowser.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBrowser.Location = new System.Drawing.Point(0, 0);
            this.listBrowser.Name = "listBrowser";
            this.listBrowser.Size = new System.Drawing.Size(169, 345);
            this.listBrowser.TabIndex = 0;
            this.listBrowser.UseCompatibleStateImageBehavior = false;
            this.listBrowser.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Item";
            // 
            // finalPage1
            // 
            this.finalPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.finalPage1.Location = new System.Drawing.Point(0, 0);
            this.finalPage1.Name = "finalPage1";
            this.finalPage1.Size = new System.Drawing.Size(552, 345);
            this.finalPage1.TabIndex = 3;
            // 
            // analyzedPage1
            // 
            this.analyzedPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analyzedPage1.Location = new System.Drawing.Point(0, 0);
            this.analyzedPage1.Name = "analyzedPage1";
            this.analyzedPage1.Size = new System.Drawing.Size(552, 345);
            this.analyzedPage1.TabIndex = 2;
            // 
            // startPage
            // 
            this.startPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startPage.IsDirty = false;
            this.startPage.Location = new System.Drawing.Point(0, 0);
            this.startPage.Name = "startPage";
            this.startPage.Size = new System.Drawing.Size(552, 345);
            this.startPage.TabIndex = 0;
            // 
            // loadedPage1
            // 
            this.loadedPage1.BackColor = System.Drawing.SystemColors.Control;
            this.loadedPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadedPage1.Location = new System.Drawing.Point(0, 0);
            this.loadedPage1.Name = "loadedPage1";
            this.loadedPage1.Size = new System.Drawing.Size(552, 345);
            this.loadedPage1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabDiagnostics);
            this.tabControl1.Controls.Add(this.tabFindResults);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(725, 141);
            this.tabControl1.TabIndex = 0;
            // 
            // tabDiagnostics
            // 
            this.tabDiagnostics.Controls.Add(this.listDiagnostics);
            this.tabDiagnostics.Location = new System.Drawing.Point(4, 22);
            this.tabDiagnostics.Name = "tabDiagnostics";
            this.tabDiagnostics.Padding = new System.Windows.Forms.Padding(3);
            this.tabDiagnostics.Size = new System.Drawing.Size(717, 115);
            this.tabDiagnostics.TabIndex = 0;
            this.tabDiagnostics.Text = "Diagnostics";
            this.tabDiagnostics.UseVisualStyleBackColor = true;
            // 
            // listDiagnostics
            // 
            this.listDiagnostics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDiagnosticType,
            this.colDiagnosticAddress,
            this.colDiagnosticDescription});
            this.listDiagnostics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDiagnostics.Location = new System.Drawing.Point(3, 3);
            this.listDiagnostics.Name = "listDiagnostics";
            this.listDiagnostics.Size = new System.Drawing.Size(711, 109);
            this.listDiagnostics.TabIndex = 0;
            this.listDiagnostics.UseCompatibleStateImageBehavior = false;
            this.listDiagnostics.View = System.Windows.Forms.View.Details;
            // 
            // colDiagnosticType
            // 
            this.colDiagnosticType.Text = "Type";
            // 
            // colDiagnosticAddress
            // 
            this.colDiagnosticAddress.Text = "Address";
            // 
            // colDiagnosticDescription
            // 
            this.colDiagnosticDescription.Text = "Description";
            // 
            // tabFindResults
            // 
            this.tabFindResults.Controls.Add(this.listFindResults);
            this.tabFindResults.Location = new System.Drawing.Point(4, 22);
            this.tabFindResults.Name = "tabFindResults";
            this.tabFindResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabFindResults.Size = new System.Drawing.Size(717, 115);
            this.tabFindResults.TabIndex = 1;
            this.tabFindResults.Text = "Search Results";
            this.tabFindResults.UseVisualStyleBackColor = true;
            // 
            // listFindResults
            // 
            this.listFindResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listFindResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listFindResults.Location = new System.Drawing.Point(3, 3);
            this.listFindResults.Name = "listFindResults";
            this.listFindResults.Size = new System.Drawing.Size(711, 109);
            this.listFindResults.TabIndex = 0;
            this.listFindResults.UseCompatibleStateImageBehavior = false;
            this.listFindResults.View = System.Windows.Forms.View.Details;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Open.ico");
            this.imageList.Images.SetKeyName(1, "Save.ico");
            this.imageList.Images.SetKeyName(2, "NextPhase.ico");
            this.imageList.Images.SetKeyName(3, "FinishDecompilation.ico");
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 512);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Decompiler";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabDiagnostics.ResumeLayout(false);
            this.tabFindResults.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private StartPage startPage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabDiagnostics;
        private System.Windows.Forms.TabPage tabFindResults;
        private System.Windows.Forms.ListView listFindResults;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView listBrowser;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private LoadedPage loadedPage1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusDetails;
        private AnalyzedPage analyzedPage1;
        private System.Windows.Forms.ListView listDiagnostics;
        private System.Windows.Forms.ColumnHeader colDiagnosticType;
        private System.Windows.Forms.ColumnHeader colDiagnosticAddress;
        private System.Windows.Forms.ColumnHeader colDiagnosticDescription;
        private FinalPage finalPage1;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.ImageList imageList;
    }
}