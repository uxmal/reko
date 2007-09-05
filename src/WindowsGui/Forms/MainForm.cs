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
		private DecompilerDriver dec;
		private static string dirSettings;
		private MruList mru;
		private GuiHost host;

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
		private MemoryControl memctl;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox ddlType;
		private System.Windows.Forms.ImageList imglMapItems;
		private System.Windows.Forms.TreeView treeMapItems;
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
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mru = new MruList(MruListFile, MaxMruItems);
			host = new GuiHost();
			host.DiagnosticsControl = this.listDiagnostics;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Driver));
			this.menu = new System.Windows.Forms.MainMenu();
			this.miFile = new System.Windows.Forms.MenuItem();
			this.miFileOpen = new System.Windows.Forms.MenuItem();
			this.miFileMruSeparator = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.miFileExit = new System.Windows.Forms.MenuItem();
			this.ofd = new System.Windows.Forms.OpenFileDialog();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
			this.memctl = new Decompiler.WindowsGui.Controls.MemoryControl();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.ddlType = new System.Windows.Forms.ComboBox();
			this.imglMapItems = new System.Windows.Forms.ImageList(this.components);
			this.treeMapItems = new System.Windows.Forms.TreeView();
			this.tabsOutput = new System.Windows.Forms.TabControl();
			this.tabDiagnostics = new System.Windows.Forms.TabPage();
			this.tabLog = new System.Windows.Forms.TabPage();
			this.listDiagnostics = new System.Windows.Forms.ListView();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.miFileDecompile = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.miFileLoad = new System.Windows.Forms.MenuItem();
			this.miFileScan = new System.Windows.Forms.MenuItem();
			this.miFileRewrite = new System.Windows.Forms.MenuItem();
			this.miFileTypeInference = new System.Windows.Forms.MenuItem();
			this.miFileStructure = new System.Windows.Forms.MenuItem();
			this.miView = new System.Windows.Forms.MenuItem();
			this.miViewMemory = new System.Windows.Forms.MenuItem();
			this.miViewDisassembly = new System.Windows.Forms.MenuItem();
			this.splitter1 = new System.Windows.Forms.Splitter();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
			this.tabsOutput.SuspendLayout();
			this.tabDiagnostics.SuspendLayout();
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
			// memctl
			// 
			this.memctl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.memctl.BackColor = System.Drawing.SystemColors.Window;
			this.memctl.BytesPerRow = 16;
			this.memctl.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.memctl.Location = new System.Drawing.Point(288, 32);
			this.memctl.Name = "memctl";
			this.memctl.SelectedAddress = null;
			this.memctl.Size = new System.Drawing.Size(688, 312);
			this.memctl.TabIndex = 4;
			this.memctl.WordSize = 1;
			this.memctl.Click += new System.EventHandler(this.memctl_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 16);
			this.label2.TabIndex = 8;
			this.label2.Text = "&Items";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(288, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 16);
			this.label3.TabIndex = 9;
			this.label3.Text = "&View:";
			// 
			// ddlType
			// 
			this.ddlType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlType.Items.AddRange(new object[] {
														 "Disassembly",
														 "Code"});
			this.ddlType.Location = new System.Drawing.Point(320, 6);
			this.ddlType.Name = "ddlType";
			this.ddlType.Size = new System.Drawing.Size(152, 21);
			this.ddlType.TabIndex = 10;
			// 
			// imglMapItems
			// 
			this.imglMapItems.ImageSize = new System.Drawing.Size(16, 16);
			this.imglMapItems.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglMapItems.ImageStream")));
			this.imglMapItems.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// treeMapItems
			// 
			this.treeMapItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.treeMapItems.ImageList = this.imglMapItems;
			this.treeMapItems.Location = new System.Drawing.Point(0, 32);
			this.treeMapItems.Name = "treeMapItems";
			this.treeMapItems.Size = new System.Drawing.Size(280, 312);
			this.treeMapItems.TabIndex = 11;
			this.treeMapItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMapItems_AfterSelect);
			// 
			// tabsOutput
			// 
			this.tabsOutput.Controls.Add(this.tabDiagnostics);
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
			// tabLog
			// 
			this.tabLog.Controls.Add(this.txtLog);
			this.tabLog.Location = new System.Drawing.Point(4, 22);
			this.tabLog.Name = "tabLog";
			this.tabLog.Size = new System.Drawing.Size(976, 134);
			this.tabLog.TabIndex = 1;
			this.tabLog.Text = "Log";
			// 
			// listDiagnostics
			// 
			this.listDiagnostics.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listDiagnostics.Location = new System.Drawing.Point(0, 0);
			this.listDiagnostics.Name = "listDiagnostics";
			this.listDiagnostics.Size = new System.Drawing.Size(976, 110);
			this.listDiagnostics.TabIndex = 2;
			// 
			// txtLog
			// 
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtLog.Location = new System.Drawing.Point(0, -2);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.Size = new System.Drawing.Size(976, 136);
			this.txtLog.TabIndex = 2;
			this.txtLog.Text = "";
			// 
			// miFileDecompile
			// 
			this.miFileDecompile.Index = 2;
			this.miFileDecompile.Text = "&Decompile";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "-";
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
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(0, 350);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(984, 3);
			this.splitter1.TabIndex = 13;
			this.splitter1.TabStop = false;
			// 
			// Driver
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(984, 513);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.tabsOutput);
			this.Controls.Add(this.treeMapItems);
			this.Controls.Add(this.ddlType);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.memctl);
			this.Menu = this.menu;
			this.Name = "Driver";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Driver";
			this.Load += new System.EventHandler(this.Driver_Load);
			this.Closed += new System.EventHandler(this.Driver_Closed);
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
			this.tabsOutput.ResumeLayout(false);
			this.tabDiagnostics.ResumeLayout(false);
			this.tabLog.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private int ImageIndexOfMapItem(ImageMapItem mi)
		{
			if (dec.Program.Procedures[mi.Address] != null)
			{
				return MainForm.ImageIndexProcedureBlock;
			}
			if (mi is ImageMapBlock)
			{
				return MainForm.ImageIndexCodeBlock;
			}
			return MainForm.ImageIndexUnknown;
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

		private void LoadSegments(ImageMap map)
		{
			Cursor c = Cursor.Current;
			try
			{
				Cursor.Current = Cursors.WaitCursor;

				treeMapItems.Nodes.Clear();
				foreach (ImageMapSegment seg in map.Segments.Values)
				{
					TreeNode item = new TreeNode(seg.Address.ToString());
					item.ImageIndex = ImageIndexSegment;
					item.Tag = seg;
					treeMapItems.Nodes.Add(item);
					LoadMapItems(map, seg, item);
				}
			}
			finally
			{
				Cursor.Current = c;
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


		private void Open(string file)
		{
			this.dec = new DecompilerDriver();
			dec.Loaded += new ProgramLoadedEventHandler(decompiler_Loaded);
			dec.Finished += new EventHandler(decompiler_Finished);
			dec.Scanned += new EventHandler(decompiler_Scanned);
			dec.Rewritten += new EventHandler(decompiler_Rewritten);

			DecompilerProject project = new DecompilerProject();
			project.Input.Filename = file;

			try
			{
				SetStatus("Loading...");
				dec.Decompile(project, this.host);
			} 	
			catch (Exception e)
			{
				host.WriteDiagnostic(Diagnostic.FatalError, "Fatal error: {0}", e.Message);
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

		private void decompiler_Loaded(object sender, ProgramLoadedEventArgs e)
		{
			SetStatus("Scanning...");
			LoadSegments(e.Loader.ImageMap);
		}

		private void decompiler_Scanned(object sender, EventArgs e)
		{
			SetStatus("Rewriting...");
		}

		private void decompiler_Rewritten(object sender, EventArgs e)
		{
			SetStatus("Analyzing...");
		}

		private void decompiler_Finished(object sender, EventArgs e)
		{
			SetStatus("Ready");
		}


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
					Open(ofd.FileName);
				}
			} 
			finally 
			{
				Cursor.Current = Cursors.Arrow;
				SetStatus("");
			}
		}

		private void memctl_Click(object sender, System.EventArgs e)
		{
		
		}

		private void treeMapItems_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			TreeNode tn = treeMapItems.SelectedNode;
			ImageMapSegment seg = tn.Tag as ImageMapSegment;
			if (seg != null)
			{
				return;
			}

			ImageMapItem bl = tn.Tag as ImageMapItem;
			if (bl != null)
			{
				seg = (ImageMapSegment) tn.Parent.Tag; 
				memctl.ShowAddress(bl.Address);
			}
		}

		private void Driver_Load(object sender, System.EventArgs e)
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

		private void Driver_Closed(object sender, System.EventArgs e)
		{
			mru.Save(MruListFile);
		}

		private void miFileMru_Click(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem) sender;
			mru.Add(mi.Text);
			Open(mi.Text);
		}

		private void statusBar_PanelClick(object sender, System.Windows.Forms.StatusBarPanelClickEventArgs e)
		{
		
		}

		private void txtLog_TextChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
