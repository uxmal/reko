/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.WindowsGui.Controls;
using Decompiler.WindowsGui.Forms;
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
	public class MainForm : System.Windows.Forms.Form
	{
		private PhasePage phasePage;

		private InitialPhase initialPhase;
		private LoadedPhase loadingPhase;

		private const int ImageIndexSegment = 0;
		private const int ImageIndexProcedureBlock = 1;
		private const int ImageIndexCodeBlock = 2;
		private const int ImageIndexData = 3;
		private const int ImageIndexVector = 4;
		private const int ImageIndexUnknown = 5;
		private System.Windows.Forms.OpenFileDialog ofd;
		private System.Windows.Forms.ImageList imglMapItems;
		private System.Windows.Forms.ImageList imagesToolbar;
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.ToolBarButton tbtnOpen;
		private System.Windows.Forms.ToolBarButton tbtnSave;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton tbtnNextPhase;
		private System.Windows.Forms.ToolBarButton tbtnFinishDecompilation;
		private System.Windows.Forms.TabControl tabsOutput;
		private System.Windows.Forms.TabPage tabDiagnostics;
		private System.Windows.Forms.ListView listDiagnostics;
		private System.Windows.Forms.ListView listDiscoveries;
		private System.Windows.Forms.TabPage tabLog;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader colDiscoveryType;
		private System.Windows.Forms.ColumnHeader colDiscoveryDescription;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Splitter splitterTop;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private System.Windows.Forms.StatusBarPanel statusBarPanel3;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panelRhs;
		private Decompiler.WindowsGui.Forms.LoadedPage pageLoaded;
		private Decompiler.WindowsGui.Forms.InitialPage pageInitial;
		private Decompiler.WindowsGui.Forms.FinalPage pageFinal;
		private System.Windows.Forms.Panel panelLhs;
		private System.Windows.Forms.ListView listBrowser;
		private System.Windows.Forms.ComboBox ddlBrowserFilter;
		private System.Windows.Forms.TreeView treeBrowser;
		private System.Windows.Forms.ColumnHeader listBrowserItemName;
		private System.Windows.Forms.ProgressBar progressBar1;
		private Decompiler.WindowsGui.Forms.AnalyzedPage pageAnalyzed;
		private System.Windows.Forms.TabPage tabWarnings;
        private SaveFileDialog sfd;
        private TabPage tabFindResults;
        private ListView listFindResults;
        private ColumnHeader columnHeader2;
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			BuildToolbarButtons();

			BuildPhases();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing )
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
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
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.imglMapItems = new System.Windows.Forms.ImageList(this.components);
            this.imagesToolbar = new System.Windows.Forms.ImageList(this.components);
            this.toolBar = new System.Windows.Forms.ToolBar();
            this.tbtnOpen = new System.Windows.Forms.ToolBarButton();
            this.tbtnSave = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.tbtnNextPhase = new System.Windows.Forms.ToolBarButton();
            this.tbtnFinishDecompilation = new System.Windows.Forms.ToolBarButton();
            this.tabsOutput = new System.Windows.Forms.TabControl();
            this.tabDiagnostics = new System.Windows.Forms.TabPage();
            this.listDiagnostics = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.tabWarnings = new System.Windows.Forms.TabPage();
            this.listDiscoveries = new System.Windows.Forms.ListView();
            this.colDiscoveryType = new System.Windows.Forms.ColumnHeader();
            this.colDiscoveryDescription = new System.Windows.Forms.ColumnHeader();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tabFindResults = new System.Windows.Forms.TabPage();
            this.listFindResults = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.panelTop = new System.Windows.Forms.Panel();
            this.splitterTop = new System.Windows.Forms.Splitter();
            this.panelRhs = new System.Windows.Forms.Panel();
            this.panelLhs = new System.Windows.Forms.Panel();
            this.listBrowser = new System.Windows.Forms.ListView();
            this.listBrowserItemName = new System.Windows.Forms.ColumnHeader();
            this.ddlBrowserFilter = new System.Windows.Forms.ComboBox();
            this.treeBrowser = new System.Windows.Forms.TreeView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.tabsOutput.SuspendLayout();
            this.tabDiagnostics.SuspendLayout();
            this.tabWarnings.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tabFindResults.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelRhs.SuspendLayout();
            this.panelLhs.SuspendLayout();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.statusBarPanel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.statusBarPanel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.statusBarPanel3)).BeginInit();
            this.tabFindResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // imglMapItems
            // 
            this.imglMapItems.ImageStream = ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("imglMapItems.ImageStream")));
            this.imglMapItems.TransparentColor = System.Drawing.Color.Transparent;
            this.imglMapItems.Images.SetKeyName(0, "");
            this.imglMapItems.Images.SetKeyName(1, "");
            this.imglMapItems.Images.SetKeyName(2, "");
            this.imglMapItems.Images.SetKeyName(3, "");
            this.imglMapItems.Images.SetKeyName(4, "");
            this.imglMapItems.Images.SetKeyName(5, "");
            // 
            // imagesToolbar
            // 
            this.imagesToolbar.ImageStream = ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("imagesToolbar.ImageStream")));
            this.imagesToolbar.TransparentColor = System.Drawing.Color.Transparent;
            this.imagesToolbar.Images.SetKeyName(0, "");
            this.imagesToolbar.Images.SetKeyName(1, "");
            this.imagesToolbar.Images.SetKeyName(2, "");
            this.imagesToolbar.Images.SetKeyName(3, "");
            // 
            // toolBar
            // 
            this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbtnOpen,
            this.tbtnSave,
            this.toolBarButton3,
            this.tbtnNextPhase,
            this.tbtnFinishDecompilation});
            this.toolBar.DropDownArrows = true;
            this.toolBar.ImageList = this.imagesToolbar;
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.ShowToolTips = true;
            this.toolBar.Size = new System.Drawing.Size(704, 28);
            this.toolBar.TabIndex = 19;
            // 
            // tbtnOpen
            // 
            this.tbtnOpen.ImageIndex = 0;
            this.tbtnOpen.Name = "tbtnOpen";
            // 
            // tbtnSave
            // 
            this.tbtnSave.ImageIndex = 1;
            this.tbtnSave.Name = "tbtnSave";
            // 
            // toolBarButton3
            // 
            this.toolBarButton3.Name = "toolBarButton3";
            this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // tbtnNextPhase
            // 
            this.tbtnNextPhase.ImageIndex = 2;
            this.tbtnNextPhase.Name = "tbtnNextPhase";
            this.tbtnNextPhase.ToolTipText = "Advance to next Decompiler Phase";
            // 
            // tbtnFinishDecompilation
            // 
            this.tbtnFinishDecompilation.ImageIndex = 3;
            this.tbtnFinishDecompilation.Name = "tbtnFinishDecompilation";
            this.tbtnFinishDecompilation.ToolTipText = "Finish decompilation";
            // 
            // tabsOutput
            // 
            this.tabsOutput.Controls.Add(this.tabDiagnostics);
            this.tabsOutput.Controls.Add(this.tabWarnings);
            this.tabsOutput.Controls.Add(this.tabLog);
            this.tabsOutput.Controls.Add(this.tabFindResults);
            this.tabsOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsOutput.Location = new System.Drawing.Point(0, 0);
            this.tabsOutput.Name = "tabsOutput";
            this.tabsOutput.SelectedIndex = 0;
            this.tabsOutput.Size = new System.Drawing.Size(704, 100);
            this.tabsOutput.TabIndex = 21;
            // 
            // tabDiagnostics
            // 
            this.tabDiagnostics.Controls.Add(this.listDiagnostics);
            this.tabDiagnostics.Location = new System.Drawing.Point(4, 22);
            this.tabDiagnostics.Name = "tabDiagnostics";
            this.tabDiagnostics.Size = new System.Drawing.Size(696, 74);
            this.tabDiagnostics.TabIndex = 0;
            this.tabDiagnostics.Text = "Diagnostics";
            this.tabDiagnostics.ToolTipText = "Displays errors and warnings incurred during decompilation";
            this.tabDiagnostics.UseVisualStyleBackColor = true;
            // 
            // listDiagnostics
            // 
            this.listDiagnostics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listDiagnostics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDiagnostics.Location = new System.Drawing.Point(0, 0);
            this.listDiagnostics.Name = "listDiagnostics";
            this.listDiagnostics.Size = new System.Drawing.Size(696, 74);
            this.listDiagnostics.TabIndex = 2;
            this.listDiagnostics.UseCompatibleStateImageBehavior = false;
            this.listDiagnostics.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 400;
            // 
            // tabWarnings
            // 
            this.tabWarnings.Controls.Add(this.listDiscoveries);
            this.tabWarnings.Location = new System.Drawing.Point(4, 22);
            this.tabWarnings.Name = "tabWarnings";
            this.tabWarnings.Size = new System.Drawing.Size(696, 74);
            this.tabWarnings.TabIndex = 2;
            this.tabWarnings.Text = "Warnings";
            this.tabWarnings.UseVisualStyleBackColor = true;
            this.tabWarnings.Visible = false;
            // 
            // listDiscoveries
            // 
            this.listDiscoveries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDiscoveryType,
            this.colDiscoveryDescription});
            this.listDiscoveries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDiscoveries.Location = new System.Drawing.Point(0, 0);
            this.listDiscoveries.Name = "listDiscoveries";
            this.listDiscoveries.Size = new System.Drawing.Size(696, 74);
            this.listDiscoveries.TabIndex = 0;
            this.listDiscoveries.UseCompatibleStateImageBehavior = false;
            this.listDiscoveries.View = System.Windows.Forms.View.Details;
            // 
            // colDiscoveryType
            // 
            this.colDiscoveryType.Text = "Type";
            // 
            // colDiscoveryDescription
            // 
            this.colDiscoveryDescription.Text = "Description";
            this.colDiscoveryDescription.Width = 396;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.txtLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Size = new System.Drawing.Size(696, 74);
            this.tabLog.TabIndex = 1;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            this.tabLog.Visible = false;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtLog.Location = new System.Drawing.Point(0, -62);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(696, 136);
            this.txtLog.TabIndex = 2;
            // 
            // tabFindResults
            // 
            this.tabFindResults.Controls.Add(this.listFindResults);
            this.tabFindResults.Location = new System.Drawing.Point(4, 22);
            this.tabFindResults.Name = "tabFindResults";
            this.tabFindResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabFindResults.Size = new System.Drawing.Size(696, 74);
            this.tabFindResults.TabIndex = 3;
            this.tabFindResults.Text = "Find Results";
            this.tabFindResults.UseVisualStyleBackColor = true;
            // 
            // listFindResults
            // 
            this.listFindResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listFindResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listFindResults.Location = new System.Drawing.Point(3, 3);
            this.listFindResults.Name = "listFindResults";
            this.listFindResults.Size = new System.Drawing.Size(690, 68);
            this.listFindResults.TabIndex = 0;
            this.listFindResults.UseCompatibleStateImageBehavior = false;
            this.listFindResults.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Item";
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.splitterTop);
            this.panelTop.Controls.Add(this.panelRhs);
            this.panelTop.Controls.Add(this.panelLhs);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTop.Location = new System.Drawing.Point(0, 28);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(704, 446);
            this.panelTop.TabIndex = 23;
            // 
            // splitterTop
            // 
            this.splitterTop.Location = new System.Drawing.Point(256, 0);
            this.splitterTop.Name = "splitterTop";
            this.splitterTop.Size = new System.Drawing.Size(3, 446);
            this.splitterTop.TabIndex = 18;
            this.splitterTop.TabStop = false;
            // 
            // panelRhs
            // 
            this.panelRhs.Controls.Add(this.pageLoaded);
            this.panelRhs.Controls.Add(this.pageInitial);
            this.panelRhs.Controls.Add(this.pageAnalyzed);
            this.panelRhs.Controls.Add(this.pageFinal);
            this.panelRhs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRhs.Location = new System.Drawing.Point(256, 0);
            this.panelRhs.Name = "panelRhs";
            this.panelRhs.Size = new System.Drawing.Size(448, 446);
            this.panelRhs.TabIndex = 22;
            // 
            // pageLoaded
            // 
            this.pageLoaded.BackColor = System.Drawing.SystemColors.Control;
            this.pageLoaded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageLoaded.Location = new System.Drawing.Point(0, 0);
            this.pageLoaded.Name = "pageLoaded";
            this.pageLoaded.ProgramImage = null;
            this.pageLoaded.Size = new System.Drawing.Size(448, 446);
            this.pageLoaded.TabIndex = 20;
            // 
            // pageInitial
            // 
            this.pageInitial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageInitial.IsDirty = false;
            this.pageInitial.Location = new System.Drawing.Point(0, 0);
            this.pageInitial.Name = "pageInitial";
            this.pageInitial.Size = new System.Drawing.Size(448, 446);
            this.pageInitial.TabIndex = 19;
            // 
            // pageAnalyzed
            // 
            this.pageAnalyzed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageAnalyzed.Location = new System.Drawing.Point(0, 0);
            this.pageAnalyzed.Name = "pageAnalyzed";
            this.pageAnalyzed.Size = new System.Drawing.Size(448, 446);
            this.pageAnalyzed.TabIndex = 21;
            // 
            // pageFinal
            // 
            this.pageFinal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageFinal.Location = new System.Drawing.Point(0, 0);
            this.pageFinal.Name = "pageFinal";
            this.pageFinal.Size = new System.Drawing.Size(448, 446);
            this.pageFinal.TabIndex = 0;
            // 
            // panelLhs
            // 
            this.panelLhs.Controls.Add(this.listBrowser);
            this.panelLhs.Controls.Add(this.ddlBrowserFilter);
            this.panelLhs.Controls.Add(this.treeBrowser);
            this.panelLhs.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLhs.Location = new System.Drawing.Point(0, 0);
            this.panelLhs.Name = "panelLhs";
            this.panelLhs.Padding = new System.Windows.Forms.Padding(4, 1, 1, 1);
            this.panelLhs.Size = new System.Drawing.Size(256, 446);
            this.panelLhs.TabIndex = 23;
            // 
            // listBrowser
            // 
            this.listBrowser.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBrowser.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listBrowserItemName});
            this.listBrowser.Location = new System.Drawing.Point(3, 32);
            this.listBrowser.Name = "listBrowser";
            this.listBrowser.Size = new System.Drawing.Size(253, 414);
            this.listBrowser.TabIndex = 21;
            this.listBrowser.UseCompatibleStateImageBehavior = false;
            this.listBrowser.View = System.Windows.Forms.View.Details;
            // 
            // listBrowserItemName
            // 
            this.listBrowserItemName.Text = "Item";
            this.listBrowserItemName.Width = 187;
            // 
            // ddlBrowserFilter
            // 
            this.ddlBrowserFilter.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlBrowserFilter.Location = new System.Drawing.Point(3, 8);
            this.ddlBrowserFilter.Name = "ddlBrowserFilter";
            this.ddlBrowserFilter.Size = new System.Drawing.Size(253, 21);
            this.ddlBrowserFilter.TabIndex = 20;
            this.ddlBrowserFilter.Text = "comboBox1";
            // 
            // treeBrowser
            // 
            this.treeBrowser.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeBrowser.Location = new System.Drawing.Point(3, 32);
            this.treeBrowser.Name = "treeBrowser";
            this.treeBrowser.Size = new System.Drawing.Size(253, 414);
            this.treeBrowser.TabIndex = 16;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.progressBar1);
            this.panelBottom.Controls.Add(this.statusBar);
            this.panelBottom.Controls.Add(this.tabsOutput);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 477);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(704, 100);
            this.panelBottom.TabIndex = 24;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(502, 78);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(176, 22);
            this.progressBar1.TabIndex = 23;
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 76);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2,
            this.statusBarPanel3});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(704, 24);
            this.statusBar.TabIndex = 22;
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Width = 400;
            // 
            // statusBarPanel2
            // 
            this.statusBarPanel2.Name = "statusBarPanel2";
            this.statusBarPanel2.Text = "Ready";
            // 
            // statusBarPanel3
            // 
            this.statusBarPanel3.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBarPanel3.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusBarPanel3.Name = "statusBarPanel3";
            this.statusBarPanel3.Text = "bar";
            this.statusBarPanel3.Width = 187;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 474);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(704, 3);
            this.splitter1.TabIndex = 25;
            this.splitter1.TabStop = false;
            // 
            // pageLoaded
            // 
            this.pageLoaded.BackColor = System.Drawing.SystemColors.Control;
            this.pageLoaded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageLoaded.Location = new System.Drawing.Point(0, 0);
            this.pageLoaded.Name = "pageLoaded";
            this.pageLoaded.ProgramImage = null;
            this.pageLoaded.Size = new System.Drawing.Size(448, 446);
            this.pageLoaded.TabIndex = 20;
            // 
            // pageInitial
            // 
            this.pageInitial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageInitial.IsDirty = false;
            this.pageInitial.Location = new System.Drawing.Point(0, 0);
            this.pageInitial.Name = "pageInitial";
            this.pageInitial.Size = new System.Drawing.Size(448, 446);
            this.pageInitial.TabIndex = 19;
            // 
            // pageAnalyzed
            // 
            this.pageAnalyzed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageAnalyzed.Location = new System.Drawing.Point(0, 0);
            this.pageAnalyzed.Name = "pageAnalyzed";
            this.pageAnalyzed.Size = new System.Drawing.Size(448, 446);
            this.pageAnalyzed.TabIndex = 21;
            // 
            // pageFinal
            // 
            this.pageFinal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageFinal.Location = new System.Drawing.Point(0, 0);
            this.pageFinal.Name = "pageFinal";
            this.pageFinal.Size = new System.Drawing.Size(448, 446);
            this.pageFinal.TabIndex = 0;
            // 
            // tabFindResults
            // 
            this.tabFindResults.Controls.Add(this.listView1);
            this.tabFindResults.Location = new System.Drawing.Point(4, 22);
            this.tabFindResults.Name = "tabFindResults";
            this.tabFindResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabFindResults.Size = new System.Drawing.Size(696, 74);
            this.tabFindResults.TabIndex = 3;
            this.tabFindResults.Text = "Find Results";
            this.tabFindResults.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(690, 68);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Item";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(704, 577);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.toolBar);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Decompiler";
            this.tabsOutput.ResumeLayout(false);
            this.tabDiagnostics.ResumeLayout(false);
            this.tabWarnings.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.tabFindResults.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelRhs.ResumeLayout(false);
            this.panelLhs.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.statusBarPanel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.statusBarPanel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.statusBarPanel3)).EndInit();
            this.tabFindResults.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public void BuildPhases()
		{
			initialPhase = new InitialPhase(pageInitial);
			loadingPhase = new LoadedPhase(pageLoaded);
			initialPhase.NextPhase = loadingPhase;
		}

		public void BuildToolbarButtons()
		{
			tbtnOpen.Tag = new CommandID(CmdSets.GuidDecompiler, CmdIds.FileOpen);
			tbtnSave.Tag = new CommandID(CmdSets.GuidDecompiler, CmdIds.FileSave);
			tbtnNextPhase.Tag = new CommandID(CmdSets.GuidDecompiler, CmdIds.ActionNextPhase);
			tbtnFinishDecompilation.Tag = new CommandID(CmdSets.GuidDecompiler, CmdIds.ActionFinishDecompilation);
		}

		public DecompilerPhase GetInitialPhase()
		{
			return initialPhase;
		}

		public int ImageIndexOfMapItem(ImageMapItem mi)
		{
			//$REFACTOR: figure out where this class belongs.

			/*			if (dec.Program.Procedures[mi.Address] != null)
						{
							return MainForm.ImageIndexProcedureBlock;
						}
						if (mi is ImageMapBlock)
						{
							return MainForm.ImageIndexCodeBlock;
						}
						*/
			return MainForm.ImageIndexUnknown;
		}

		public void AddDiagnostic(Diagnostic d, string format, params object[] args)
		{
			ListViewItem li = new ListViewItem();
			li.SubItems.Add(string.Format(format, args));
			this.listDiagnostics.Items.Add(li);
		}

		public ComboBox BrowserFilter
		{
			get {return ddlBrowserFilter; }
		}

		public ListView BrowserList
		{
			get { return listBrowser; }
		}

		public TreeView BrowserTree
		{
			get { return treeBrowser; }
		}

		public PhasePage CurrentPhasePage
		{
			get { return phasePage; }
			set 
			{ 
				this.phasePage = value;
				phasePage.BringToFront();
				phasePage.Visible = true;
			}
		}

		public FinalPage FinalPage
		{
			get { return pageFinal; }
		}

        public ListView FindResultsList
        {
            get { return listFindResults; }
        }

		public InitialPage InitialPage
		{
			get { return pageInitial; }
		}

		public LoadedPage LoadedPage
		{
			get { return pageLoaded; }
		}

		public AnalyzedPage AnalyzedPage
		{
			get { return pageAnalyzed; }
		}

		public OpenFileDialog OpenFileDialog
		{
			get { return ofd; }
		}

		public ProgressBar ProgressBar
		{
			get { return progressBar1; }
		}

        public SaveFileDialog SaveFileDialog
        {
            get { return sfd; }
        }

        public void SetStatus(string txt)
		{
			statusBar.Panels[1].Text = txt;
		}

		public void SetDetails(string txt)
		{
			statusBar.Panels[0].Text = txt;
		}

		public ToolBar ToolBar
		{
			get { return this.toolBar; }
		}

    }
}
