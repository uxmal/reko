/* 
 * Copyright (C) 1999-2007 John Källén.
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
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
	public class MainForm : System.Windows.Forms.Form
	{
		private static string dirSettings;
		private MruList mru;
		private GuiHost host;
		private MainFormInteractor interactor;
		private PhasePage currentPage;

		private InitialPhase initialPhase;
		private LoadedPhase loadingPhase;
		private ScannedPhase scannedPhase;
		private MachineCodeRewrittenPhase rewritingPhase;
		private DataFlowPhase dataflowPhase;
		private TypeReconstructedPhase typeReconstructionPhase;
		private CodeStructuredPhase codeStructuringPhase;

		private const int ImageIndexSegment = 0;
		private const int ImageIndexProcedureBlock = 1;
		private const int ImageIndexCodeBlock = 2;
		private const int ImageIndexData = 3;
		private const int ImageIndexVector = 4;
		private const int ImageIndexUnknown = 5;

		private const int MaxMruItems = 10;
		private System.Windows.Forms.MenuItem miFileExit;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.OpenFileDialog ofd;
		private System.Windows.Forms.MainMenu menu;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private System.Windows.Forms.StatusBarPanel statusBarPanel3;
		private System.Windows.Forms.ImageList imglMapItems;
		private System.Windows.Forms.MenuItem miFileMruSeparator;
		private System.Windows.Forms.MenuItem miFile;
		private System.Windows.Forms.TabControl tabsOutput;
		private System.Windows.Forms.TabPage tabDiagnostics;
		private System.Windows.Forms.TabPage tabLog;
		private System.Windows.Forms.ListView listDiagnostics;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.MenuItem miView;
		private System.Windows.Forms.MenuItem miViewMemory;
		private System.Windows.Forms.MenuItem miViewDisassembly;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.TabPage tabDiscoveries;
		private System.Windows.Forms.ListView listDiscoveries;
		private System.Windows.Forms.ColumnHeader colDiscoveryType;
		private System.Windows.Forms.ColumnHeader colDiscoveryDescription;
		private System.Windows.Forms.Panel panel1;
		private Decompiler.WindowsGui.Forms.LoadPage loadPage;
		private System.Windows.Forms.Splitter splitterTop;
		private System.Windows.Forms.TreeView treeBrowser;
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.ToolBarButton tbtnNextPhase;
		private System.Windows.Forms.ToolBarButton tbtnOpen;
		private System.Windows.Forms.ToolBarButton tbtnSave;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ImageList imagesToolbar;
		private System.Windows.Forms.ToolBarButton tbtnFinishDecompilation;
		private Decompiler.WindowsGui.Forms.InitialPage initialPage;
		private System.Windows.Forms.MenuItem miFileOpenProject;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem miFileOpenBinary;
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mru = new MruList(MruListFile, MaxMruItems);
			BuildPhases();

			interactor = new MainFormInteractor(this, initialPhase);
			host = new GuiHost(this);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.menu = new System.Windows.Forms.MainMenu();
			this.miFile = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.miFileOpenProject = new System.Windows.Forms.MenuItem();
			this.miFileOpenBinary = new System.Windows.Forms.MenuItem();
			this.miFileMruSeparator = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.miFileExit = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.miView = new System.Windows.Forms.MenuItem();
			this.miViewMemory = new System.Windows.Forms.MenuItem();
			this.miViewDisassembly = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.ofd = new System.Windows.Forms.OpenFileDialog();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
			this.imglMapItems = new System.Windows.Forms.ImageList(this.components);
			this.tabsOutput = new System.Windows.Forms.TabControl();
			this.tabDiagnostics = new System.Windows.Forms.TabPage();
			this.listDiagnostics = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.tabDiscoveries = new System.Windows.Forms.TabPage();
			this.listDiscoveries = new System.Windows.Forms.ListView();
			this.colDiscoveryType = new System.Windows.Forms.ColumnHeader();
			this.colDiscoveryDescription = new System.Windows.Forms.ColumnHeader();
			this.tabLog = new System.Windows.Forms.TabPage();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitterTop = new System.Windows.Forms.Splitter();
			this.loadPage = new Decompiler.WindowsGui.Forms.LoadPage();
			this.treeBrowser = new System.Windows.Forms.TreeView();
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.tbtnOpen = new System.Windows.Forms.ToolBarButton();
			this.tbtnSave = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
			this.tbtnNextPhase = new System.Windows.Forms.ToolBarButton();
			this.tbtnFinishDecompilation = new System.Windows.Forms.ToolBarButton();
			this.imagesToolbar = new System.Windows.Forms.ImageList(this.components);
			this.initialPage = new Decompiler.WindowsGui.Forms.InitialPage();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
			this.tabsOutput.SuspendLayout();
			this.tabDiagnostics.SuspendLayout();
			this.tabDiscoveries.SuspendLayout();
			this.tabLog.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menu
			// 
			this.menu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				 this.miFile,
																				 this.menuItem1,
																				 this.miView,
																				 this.menuItem5});
			// 
			// miFile
			// 
			this.miFile.Index = 0;
			this.miFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.menuItem8,
																				   this.miFileOpenProject,
																				   this.miFileOpenBinary,
																				   this.miFileMruSeparator,
																				   this.menuItem3,
																				   this.miFileExit});
			this.miFile.Text = "&File";
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 0;
			this.menuItem8.Text = "&New Project";
			// 
			// miFileOpenProject
			// 
			this.miFileOpenProject.Index = 1;
			this.miFileOpenProject.Text = "&Open Project...";
			this.miFileOpenProject.Click += new System.EventHandler(this.miFileOpen_Click);
			// 
			// miFileOpenBinary
			// 
			this.miFileOpenBinary.Index = 2;
			this.miFileOpenBinary.Text = "Open &Binary File...";
			this.miFileOpenBinary.Click += new System.EventHandler(this.miFileOpenBinary_Click);
			// 
			// miFileMruSeparator
			// 
			this.miFileMruSeparator.Index = 3;
			this.miFileMruSeparator.Text = "-";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 4;
			this.menuItem3.Text = "-";
			// 
			// miFileExit
			// 
			this.miFileExit.Index = 5;
			this.miFileExit.Text = "E&xit";
			this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 1;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem4});
			this.menuItem1.Text = "&Edit";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 0;
			this.menuItem4.Text = "&Copy";
			// 
			// miView
			// 
			this.miView.Index = 2;
			this.miView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miViewMemory,
																				   this.miViewDisassembly});
			this.miView.Text = "&View";
			// 
			// miViewMemory
			// 
			this.miViewMemory.Index = 0;
			this.miViewMemory.Text = "&Memory";
			// 
			// miViewDisassembly
			// 
			this.miViewDisassembly.Index = 1;
			this.miViewDisassembly.Text = "&Disassembly";
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 3;
			this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem6});
			this.menuItem5.Text = "&Help";
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 0;
			this.menuItem6.Text = "&About...";
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 449);
			this.statusBar.Name = "statusBar";
			this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						 this.statusBarPanel1,
																						 this.statusBarPanel2,
																						 this.statusBarPanel3});
			this.statusBar.ShowPanels = true;
			this.statusBar.Size = new System.Drawing.Size(792, 24);
			this.statusBar.TabIndex = 3;
			this.statusBar.PanelClick += new System.Windows.Forms.StatusBarPanelClickEventHandler(this.statusBar_PanelClick);
			// 
			// statusBarPanel1
			// 
			this.statusBarPanel1.Width = 400;
			// 
			// statusBarPanel2
			// 
			this.statusBarPanel2.Text = "Ready";
			// 
			// statusBarPanel3
			// 
			this.statusBarPanel3.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.statusBarPanel3.Text = "bar";
			this.statusBarPanel3.Width = 276;
			// 
			// imglMapItems
			// 
			this.imglMapItems.ImageSize = new System.Drawing.Size(16, 16);
			this.imglMapItems.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglMapItems.ImageStream")));
			this.imglMapItems.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tabsOutput
			// 
			this.tabsOutput.Controls.Add(this.tabDiagnostics);
			this.tabsOutput.Controls.Add(this.tabDiscoveries);
			this.tabsOutput.Controls.Add(this.tabLog);
			this.tabsOutput.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tabsOutput.Location = new System.Drawing.Point(0, 313);
			this.tabsOutput.Name = "tabsOutput";
			this.tabsOutput.SelectedIndex = 0;
			this.tabsOutput.Size = new System.Drawing.Size(792, 136);
			this.tabsOutput.TabIndex = 12;
			// 
			// tabDiagnostics
			// 
			this.tabDiagnostics.Controls.Add(this.listDiagnostics);
			this.tabDiagnostics.Location = new System.Drawing.Point(4, 22);
			this.tabDiagnostics.Name = "tabDiagnostics";
			this.tabDiagnostics.Size = new System.Drawing.Size(784, 110);
			this.tabDiagnostics.TabIndex = 0;
			this.tabDiagnostics.Text = "Diagnostics";
			this.tabDiagnostics.ToolTipText = "Displays errors and warnings incurred during decompilation";
			// 
			// listDiagnostics
			// 
			this.listDiagnostics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							  this.columnHeader1});
			this.listDiagnostics.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listDiagnostics.Location = new System.Drawing.Point(0, 0);
			this.listDiagnostics.Name = "listDiagnostics";
			this.listDiagnostics.Size = new System.Drawing.Size(784, 110);
			this.listDiagnostics.TabIndex = 2;
			this.listDiagnostics.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 400;
			// 
			// tabDiscoveries
			// 
			this.tabDiscoveries.Controls.Add(this.listDiscoveries);
			this.tabDiscoveries.Location = new System.Drawing.Point(4, 22);
			this.tabDiscoveries.Name = "tabDiscoveries";
			this.tabDiscoveries.Size = new System.Drawing.Size(784, 110);
			this.tabDiscoveries.TabIndex = 2;
			this.tabDiscoveries.Text = "Discoveries";
			// 
			// listDiscoveries
			// 
			this.listDiscoveries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							  this.colDiscoveryType,
																							  this.colDiscoveryDescription});
			this.listDiscoveries.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listDiscoveries.Location = new System.Drawing.Point(0, 0);
			this.listDiscoveries.Name = "listDiscoveries";
			this.listDiscoveries.Size = new System.Drawing.Size(784, 110);
			this.listDiscoveries.TabIndex = 0;
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
			this.tabLog.Size = new System.Drawing.Size(784, 110);
			this.tabLog.TabIndex = 1;
			this.tabLog.Text = "Log";
			// 
			// txtLog
			// 
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtLog.Location = new System.Drawing.Point(0, -26);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.Size = new System.Drawing.Size(784, 136);
			this.txtLog.TabIndex = 2;
			this.txtLog.Text = "";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(0, 310);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(792, 3);
			this.splitter1.TabIndex = 13;
			this.splitter1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.splitterTop);
			this.panel1.Controls.Add(this.loadPage);
			this.panel1.Controls.Add(this.treeBrowser);
			this.panel1.Controls.Add(this.toolBar);
			this.panel1.Controls.Add(this.initialPage);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(792, 310);
			this.panel1.TabIndex = 15;
			// 
			// splitterTop
			// 
			this.splitterTop.Location = new System.Drawing.Point(200, 28);
			this.splitterTop.Name = "splitterTop";
			this.splitterTop.Size = new System.Drawing.Size(3, 282);
			this.splitterTop.TabIndex = 17;
			this.splitterTop.TabStop = false;
			// 
			// loadPage
			// 
			this.loadPage.Architecture = null;
			this.loadPage.BackColor = System.Drawing.SystemColors.Control;
			this.loadPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.loadPage.Location = new System.Drawing.Point(200, 28);
			this.loadPage.Name = "loadPage";
			this.loadPage.ProcessorArchitecture = null;
			this.loadPage.ProgramImage = null;
			this.loadPage.Size = new System.Drawing.Size(592, 282);
			this.loadPage.TabIndex = 15;
			// 
			// treeBrowser
			// 
			this.treeBrowser.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeBrowser.ImageIndex = -1;
			this.treeBrowser.Location = new System.Drawing.Point(0, 28);
			this.treeBrowser.Name = "treeBrowser";
			this.treeBrowser.SelectedImageIndex = -1;
			this.treeBrowser.Size = new System.Drawing.Size(200, 282);
			this.treeBrowser.TabIndex = 16;
			this.treeBrowser.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeBrowser_AfterSelect);
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
			this.toolBar.Size = new System.Drawing.Size(792, 28);
			this.toolBar.TabIndex = 18;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
			// 
			// tbtnOpen
			// 
			this.tbtnOpen.ImageIndex = 0;
			// 
			// tbtnSave
			// 
			this.tbtnSave.ImageIndex = 1;
			// 
			// toolBarButton3
			// 
			this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbtnNextPhase
			// 
			this.tbtnNextPhase.ImageIndex = 2;
			this.tbtnNextPhase.ToolTipText = "Advance to next Decompiler Phase";
			// 
			// tbtnFinishDecompilation
			// 
			this.tbtnFinishDecompilation.ImageIndex = 3;
			this.tbtnFinishDecompilation.ToolTipText = "Finish decompilation";
			// 
			// imagesToolbar
			// 
			this.imagesToolbar.ImageSize = new System.Drawing.Size(16, 16);
			this.imagesToolbar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagesToolbar.ImageStream")));
			this.imagesToolbar.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// initialPage
			// 
			this.initialPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.initialPage.Location = new System.Drawing.Point(0, 0);
			this.initialPage.Name = "initialPage";
			this.initialPage.Size = new System.Drawing.Size(792, 310);
			this.initialPage.TabIndex = 19;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 473);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.tabsOutput);
			this.Controls.Add(this.statusBar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.menu;
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Decompiler";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Closed += new System.EventHandler(this.MainForm_Closed);
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
			this.tabsOutput.ResumeLayout(false);
			this.tabDiagnostics.ResumeLayout(false);
			this.tabDiscoveries.ResumeLayout(false);
			this.tabLog.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void BuildPhases()
		{
			initialPhase = new InitialPhase(initialPage);
			loadingPhase = new LoadedPhase(loadPage);
//			scannedPhase = new ScannedPhase(ScanPage);
			//			rewritingPhase = new RewritingPhase();
			//			dataflowPhase = new DataFlowPhrase();
			//			typeReconstructionPhase = new TypeReconstructionPhase();
			//			codeStructuringPhase = new CodeStructuringPhase();
			initialPhase.NextPhase = loadingPhase;
			loadingPhase.NextPhase = scannedPhase;
//5			scannedPhase.NextPhase = null;
		}

		public DecompilerPhase GetInitialPhase()
		{
			return initialPhase;
		}

		private int ImageIndexOfMapItem(ImageMapItem mi)
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

		private void LoadMapItems(ImageMap map, ImageMapSegment seg, TreeNode node)
		{
			int maxAddr = (int) (seg.Address.Linear + seg.Size);
			IEnumerator e = map.GetItemEnumerator(seg.Address);
			while (e.MoveNext())
			{
				DictionaryEntry de = (DictionaryEntry) e.Current;
				ImageMapItem mi = (ImageMapItem) de.Value;
				if (mi.Address.Linear >= maxAddr)
					break;

				TreeNode item = new TreeNode(
					string.Format("{0}, size: 0x{1:X8}", mi.Address.ToString(), mi.Size));
				item.ImageIndex = ImageIndexOfMapItem(mi);
				item.SelectedImageIndex = item.ImageIndex;
				item.Tag = mi;
				node.Nodes.Add(item);
			}
		}

		private static string MruListFile
		{
			get { return SettingsDirectory + "\\mru.txt"; }
		}

		private static string SettingsDirectory
		{
			get 
			{ 
				if (dirSettings == null)
				{
					string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\jkl\\grovel"; 
					if (!Directory.Exists(dir))
						Directory.CreateDirectory(dir);
					dirSettings = dir;
				}
				return dirSettings;
			}
		}


		public void SetStatus(string txt)
		{
			statusBar.Panels[1].Text = txt;
		}

		public void SetDetails(string txt)
		{
			statusBar.Panels[0].Text = txt;
		}


		public void ShowLoadPage(DecompilerDriver decompiler)
		{
			loadPage.BringToFront();
			loadPage.Populate(decompiler, treeBrowser);
		}

		public void ShowPhasePage(PhasePage page, DecompilerDriver decompiler)
		{
			page.BringToFront();
			page.Populate(decompiler, treeBrowser);
		}

		// Event handlers /////////////////////////////////////

		private void miFileExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void miFileOpen_Click(object sender, System.EventArgs e)
		{
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			// Populate menu with stuff.

			if (mru.Items.Count == 0)
			{
				miFileMruSeparator.Visible = false;
			}
			else
			{
				miFileMruSeparator.Visible = true;
			}

			int idx = miFileMruSeparator.Index + 1;
			foreach (string item in mru.Items)
			{
				MenuItem mi = new MenuItem(item);
				mi.Click +=new EventHandler(miFileMru_Click);
				miFile.MenuItems.Add(idx, mi);
				++idx;
			}

			initialPage.BringToFront();
		}

		private void MainForm_Closed(object sender, System.EventArgs e)
		{
			mru.Save(MruListFile);
		}

		private void miFileMru_Click(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem) sender;
			mru.Add(mi.Text);
			interactor.OpenBinary(mi.Text, host);
		}

		private void statusBar_PanelClick(object sender, System.Windows.Forms.StatusBarPanelClickEventArgs e)
		{
		
		}

		private void txtLog_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void treeBrowser_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			interactor.BrowserItemSelected(e.Node.Tag);
		}

		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			//$REVIEW: this hard-wiring should use command routing instead. Store the menu commands in the tags of the 
			// toolbar.
			if (e.Button == tbtnNextPhase)
			{
				interactor.NextPhase();
			} 
			else if (e.Button == tbtnFinishDecompilation)
			{
				interactor.FinishDecompilation();
			}
		}

		private void miFileOpenBinary_Click(object sender, System.EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				if (this.ofd.ShowDialog(this) == DialogResult.OK)
				{
					mru.Add(ofd.FileName);
					interactor.OpenBinary(ofd.FileName, host);
				}
			} 
			finally 
			{
				Cursor.Current = Cursors.Arrow;
				SetStatus("");
			}
		}
	}
}
