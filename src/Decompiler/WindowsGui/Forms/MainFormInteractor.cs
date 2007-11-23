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

using Decompiler.Core.Serialization;
using Decompiler.Gui;
using Decompiler.WindowsGui.Controls;
using System;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
	/// <summary>
	/// Handles interaction with the MainForm, in order to decouple platform-
	/// specific code from the user interaction code. This will make it easier to port
	/// to other GUI platforms.
	/// </summary>
	public class MainFormInteractor : ICommandTarget
	{
		private GuiHost host;
		private MainForm form;			//$REVIEW: in the future, this should be an interface.
		private DecompilerDriver decompiler;
		private DecompilerPhase initialPhase;
		private DecompilerPhase phase;
		private MruList mru;

		private static string dirSettings;
		
		private const int MaxMruItems = 10;

		public MainFormInteractor(MainForm form, DecompilerPhase initialPhase)
		{
			this.form = form;
			this.initialPhase = initialPhase;
			this.phase = initialPhase;

			host = new GuiHost(form);
			mru = new MruList(MaxMruItems);
			mru.Load(MruListFile);
			DecompilerMenus dm = new DecompilerMenus(this);
			form.Menu = dm.MainMenu;


			form.Load += new System.EventHandler(this.MainForm_Load);
			form.Closed += new System.EventHandler(this.MainForm_Closed);

		}

		public void OpenBinary(string file, DecompilerHost host)
		{
			decompiler = new DecompilerDriver(file, host);
			try
			{
				decompiler.LoadProgram();
				form.ShowPhasePage(initialPhase.Page, decompiler);
			} 	
			catch (Exception e)
			{
				form.AddDiagnostic(Diagnostic.FatalError, "Fatal error: {0}", e.Message);
				form.SetStatus("Terminated due to fatal error.");
			}
		}

		public void BrowserItemSelected(object item)
		{
			phase.Page.BrowserItemSelected(item);
		}

		public void FinishDecompilation()
		{
			phase.Execute(decompiler);
			while (phase.NextPhase != null)
			{
				phase = phase.NextPhase;
				phase.Execute(decompiler);
			}
			form.ShowPhasePage(phase.Page, decompiler);

		}

		private static string MruListFile
		{
			get { return SettingsDirectory + "\\mru.txt"; }
		}


		public void NextPhase()
		{
			phase.Execute(decompiler);
			if (phase.NextPhase != null)
			{
				phase = phase.NextPhase;
				form.ShowPhasePage(phase.Page, decompiler);
			}
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

		#region ICommandTarget members 
		public bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus cmdStatus, CommandText cmdText)
		{
			if (cmdSet == CmdSets.GuidDecompiler)
			{
				int iMru = cmdId - CmdIds.FileMru;
				if (0 <= iMru && iMru < mru.Items.Count)
				{
					cmdStatus.Status = MenuStatus.Visible|MenuStatus.Enabled;
					cmdText.Text = (string) mru.Items[iMru];
					return true;
				}

				switch (cmdId)
				{
				case CmdIds.FileOpen:
				case CmdIds.FileExit:
					cmdStatus.Status = MenuStatus.Enabled|MenuStatus.Visible;
					return true;
				case CmdIds.FileMru:
					cmdStatus.Status = MenuStatus.Visible;
					return true;
				}
			}
			return false;
		}

		public bool Execute(ref Guid cmdSet, int cmdId)
		{
			if (cmdSet == CmdSets.GuidDecompiler)
			{
				int iMru = cmdId - CmdIds.FileMru;
				if (0 <= iMru && iMru < mru.Items.Count)
				{
					string file = (string) mru.Items[iMru];
					OpenBinary(file, host);
					mru.Use(file);
					return true;
				}
				switch (cmdId)
				{
				case CmdIds.FileOpen: OpenBinary(); return true;
				case CmdIds.FileExit: form.Close(); return true;
				}
			}
			return false;
		}

		#endregion

		// Event handlers //////////////////////////////

		private void miFileExit_Click(object sender, System.EventArgs e)
		{
			form.Close();
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			/*
			// Populate menu with stuff.

			if (MruList.Items.Count == 0)
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
			pageInitial.BringToFront();
*/
		}

		private void MainForm_Closed(object sender, System.EventArgs e)
		{
			mru.Save(MruListFile);
		}


		private void statusBar_PanelClick(object sender, System.Windows.Forms.StatusBarPanelClickEventArgs e)
		{
		
		}

		private void txtLog_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void treeBrowser_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			BrowserItemSelected(e.Node.Tag);
		}

		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			/*
			//$REVIEW: this hard-wiring should use command routing instead. Store the menu commands in the tags of the 
			// toolbar.
			if (e.Button == tbtnNextPhase)
			{
				NextPhase();
			} 
			else if (e.Button == tbtnFinishDecompilation)
			{
				FinishDecompilation();
			}
			*/
		}

		private void OpenBinary()
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				if (form.OpenFileDialog.ShowDialog(form) == DialogResult.OK)
				{
					OpenBinary(form.OpenFileDialog.FileName, host);
					mru.Use(form.OpenFileDialog.FileName);
				}
			} 
			finally 
			{
				Cursor.Current = Cursors.Arrow;
				form.SetStatus("");
			}
		}
	}
}
