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


		private const int ImageIndexSegment = 0;
		private const int ImageIndexProcedureBlock = 1;
		private const int ImageIndexCodeBlock = 2;
		private const int ImageIndexData = 3;
		private const int ImageIndexVector = 4;
		private const int ImageIndexUnknown = 5;

		private const int MaxMruItems = 10;

		private System.Windows.Forms.MenuItem miFileOpen;
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
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem miFileDecompile;
		private System.Windows.Forms.MenuItem miFileLoad;
		private System.Windows.Forms.MenuItem miFileScan;
		private System.Windows.Forms.MenuItem miFileRewrite;
		private System.Windows.Forms.MenuItem miFileTypeInference;
		private System.Windows.Forms.MenuItem miFileStructure;
		private System.Windows.Forms.MenuItem miView;
		private System.Windows.Forms.MenuItem miViewMemory;
		private System.Windows.Forms.MenuItem miViewDisassembly;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.TabPage tabDiscoveries;
		private System.Windows.Forms.ListView listDiscoveries;
		private System.Windows.Forms.ColumnHeader colDiscoveryType;
		private System.Windows.Forms.ColumnHeader colDiscoveryDescription;
		private Decompiler.WindowsGui.Forms.LoadPage loadPage;
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mru = new MruList(MruListFile, MaxMruItems);
			interactor = new MainFormInteractor(this);
			host = new GuiHost(this);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			this.miFileOpen = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.miFileDecompile = new System.Windows.Forms.MenuItem();
			this.miFileLoad = new System.Windows.Forms.MenuItem();
			this.miFileScan = new System.Windows.Forms.MenuItem();
			this.miFileRewrite = new System.Windows.Forms.MenuItem();
			this.miFileTypeInference = new System.Windows.Forms.MenuItem();
			this.miFileStructure = new System.Windows.Forms.MenuItem();
			this.miFileMruSeparator = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.miFileExit = new System.Windows.Forms.MenuItem();
			this.miView = new System.Windows.Forms.MenuItem();
			this.miViewMemory = new System.Windows.Forms.MenuItem();
			this.miViewDisassembly = new System.Windows.Forms.MenuItem();
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
			this.loadPage = new Decompiler.WindowsGui.Forms.LoadPage();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
			this.tabsOutput.SuspendLayout();
			this.tabDiagnostics.SuspendLayout();
			this.tabDiscoveries.SuspendLayout();
			this.tabLog.SuspendLayout();
			this.SuspendLayout();
			// 
			// menu
			// 
			this.menu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				 this.miFile,
																				 this.miView});
			// 
			// miFile
			// 
			this.miFile.Index = 0;
			this.miFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miFileOpen,
																				   this.menuItem2,
																				   this.miFileDecompile,
																				   this.miFileLoad,
																				   this.miFileScan,
																				   this.miFileRewrite,
																				   this.miFileTypeInference,
																				   this.miFileStructure,
																				   this.miFileMruSeparator,
																				   this.menuItem3,
																				   this.miFileExit});
			this.miFile.Text = "&File";
			// 
			// miFileOpen
			// 
			this.miFileOpen.Index = 0;
			this.miFileOpen.Text = "&Open...";
			this.miFileOpen.Click += new System.EventHandler(this.miFileOpen_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "-";
			// 
			// miFileDecompile
			// 
			this.miFileDecompile.Index = 2;
			this.miFileDecompile.Text = "&Decompile";
			// 
			// miFileLoad
			// 
			this.miFileLoad.Index = 3;
			this.miFileLoad.Text = "&Load";
			// 
			// miFileScan
			// 
			this.miFileScan.Index = 4;
			this.miFileScan.Text = "&Scan";
			// 
			// miFileRewrite
			// 
			this.miFileRewrite.Index = 5;
			this.miFileRewrite.Text = "&Rewrite";
			// 
			// miFileTypeInference
			// 
			this.miFileTypeInference.Index = 6;
			this.miFileTypeInference.Text = "&Type Inference";
			// 
			// miFileStructure
			// 
			this.miFileStructure.Index = 7;
			this.miFileStructure.Text = "Stru&cture";
			// 
			// miFileMruSeparator
			// 
			this.miFileMruSeparator.Index = 8;
			this.miFileMruSeparator.Text = "-";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 9;
			this.menuItem3.Text = "-";
			// 
			// miFileExit
			// 
			this.miFileExit.Index = 10;
			this.miFileExit.Text = "E&xit";
			this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
			// 
			// miView
			// 
			this.miView.Index = 1;
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
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 489);
			this.statusBar.Name = "statusBar";
			this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						 this.statusBarPanel1,
																						 this.statusBarPanel2,
																						 this.statusBarPanel3});
			this.statusBar.ShowPanels = true;
			this.statusBar.Size = new System.Drawing.Size(984, 24);
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
			this.statusBarPanel3.Width = 468;
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
			this.tabsOutput.Location = new System.Drawing.Point(0, 353);
			this.tabsOutput.Name = "tabsOutput";
			this.tabsOutput.SelectedIndex = 0;
			this.tabsOutput.Size = new System.Drawing.Size(984, 136);
			this.tabsOutput.TabIndex = 12;
			// 
			// tabDiagnostics
			// 
			this.tabDiagnostics.Controls.Add(this.listDiagnostics);
			this.tabDiagnostics.Location = new System.Drawing.Point(4, 22);
			this.tabDiagnostics.Name = "tabDiagnostics";
			this.tabDiagnostics.Size = new System.Drawing.Size(976, 110);
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
			this.listDiagnostics.Size = new System.Drawing.Size(976, 110);
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
			this.tabDiscoveries.Size = new System.Drawing.Size(976, 110);
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
			this.listDiscoveries.Size = new System.Drawing.Size(976, 110);
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
			this.tabLog.Size = new System.Drawing.Size(976, 110);
			this.tabLog.TabIndex = 1;
			this.tabLog.Text = "Log";
			// 
			// txtLog
			// 
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtLog.Location = new System.Drawing.Point(0, -26);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.Size = new System.Drawing.Size(976, 136);
			this.txtLog.TabIndex = 2;
			this.txtLog.Text = "";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(0, 350);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(984, 3);
			this.splitter1.TabIndex = 13;
			this.splitter1.TabStop = false;
			// 
			// loadPage
			// 
			this.loadPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.loadPage.Location = new System.Drawing.Point(0, 0);
			this.loadPage.Name = "loadPage";
			this.loadPage.ProgramImage = null;
			this.loadPage.Size = new System.Drawing.Size(984, 350);
			this.loadPage.TabIndex = 14;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(984, 513);
			this.Controls.Add(this.loadPage);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.tabsOutput);
			this.Controls.Add(this.statusBar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.menu;
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Driver";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Closed += new System.EventHandler(this.MainForm_Closed);
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
			this.tabsOutput.ResumeLayout(false);
			this.tabDiagnostics.ResumeLayout(false);
			this.tabDiscoveries.ResumeLayout(false);
			this.tabLog.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

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

		public void ShowLoadPage(Program program)
		{
			loadPage.BringToFront();
			loadPage.Architecture = program.Architecture;
			loadPage.ProgramImage = program.Image;
			loadPage.ImageMap = program.ImageMap;
		}

		// Event handlers /////////////////////////////////////

		private void miFileExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void miFileOpen_Click(object sender, System.EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				if (this.ofd.ShowDialog(this) == DialogResult.OK)
				{
					mru.Add(ofd.FileName);
					interactor.Open(ofd.FileName, host);
				}
			} 
			finally 
			{
				Cursor.Current = Cursors.Arrow;
				SetStatus("");
			}
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
		}

		private void MainForm_Closed(object sender, System.EventArgs e)
		{
			mru.Save(MruListFile);
		}

		private void miFileMru_Click(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem) sender;
			mru.Add(mi.Text);
			interactor.Open(mi.Text, host);
		}

		private void statusBar_PanelClick(object sender, System.Windows.Forms.StatusBarPanelClickEventArgs e)
		{
		
		}

		private void txtLog_TextChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
