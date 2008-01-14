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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
	public class LoadedPageInteractor : PhasePageInteractor
	{
		private LoadedPage pageLoaded;
		private Hashtable mpCmdidToCommand;

		public LoadedPageInteractor(LoadedPage page, MainForm form, DecompilerMenus dm) : base(page, form)
		{
			this.pageLoaded = page;
			mpCmdidToCommand = new Hashtable();
			AddCommand(ref CmdSets.GuidDecompiler, CmdIds.ViewShowAllFragments);
			AddCommand(ref CmdSets.GuidDecompiler, CmdIds.ViewShowUnscanned);
			AddCommand(ref CmdSets.GuidDecompiler, CmdIds.ViewFindFragments);
			pageLoaded.MemoryControl.ContextMenu = dm.GetContextMenu(MenuIds.CtxMemoryControl);
		}

		protected MenuCommand AddCommand(ref Guid cmdSet, int cmdId)
		{
			MenuCommand mc = new MenuCommand(null, new CommandID(cmdSet, cmdId));
			mpCmdidToCommand.Add(mc.CommandID.ID, mc);
			return mc;
		}

		public void BrowserItemSelected()
		{
			ImageMapSegment segment = (ImageMapSegment) MainForm.BrowserList.FocusedItem.Tag;
			pageLoaded.MemoryControl.TopAddress = segment.Address;
			pageLoaded.MemoryControl.SelectedAddress = segment.Address;
		}

		public override bool Execute(ref Guid cmdSet, int cmdId)
		{
			if (cmdSet == CmdSets.GuidDecompiler)
			{
				switch (cmdId)
				{
				case CmdIds.BrowserItemSelected:
					BrowserItemSelected(); return true;
				case CmdIds.ViewGoToAddress:
					GotoAddress(); return true;
				case CmdIds.ActionMarkProcedure:
					MarkAndScanProcedure(); return true;
				}
			}
			return base.Execute(ref cmdSet, cmdId);
		}


		public void GotoAddress()
		{
			using (GotoDialog dlg = new GotoDialog())
			{
				GotoDialogInteractor i = new GotoDialogInteractor(dlg);
				if (dlg.ShowDialog(MainForm) == DialogResult.OK)
				{
					pageLoaded.MemoryControl.SelectedAddress = i.Address;
					pageLoaded.MemoryControl.TopAddress = i.Address;
				}
			}
		}

		public void MarkAndScanProcedure()
		{
			Address addr = pageLoaded.MemoryControl.SelectedAddress;
			if (addr != null)
			{
				Decompiler.ScanProcedure(addr);
				SerializedProcedure userp = new SerializedProcedure();
				userp.Address = addr.ToString();
				Decompiler.Project.UserProcedures.Add(userp);
				pageLoaded.MemoryControl.Invalidate();
			}
		}

		public override void PopulateControls()
		{
			pageLoaded.Architecture = Decompiler.Program.Architecture;
			pageLoaded.MemoryControl.ProgramImage = Decompiler.Program.Image;
			pageLoaded.Disassembly.Text = "";

			MainForm.BrowserList.Visible = true;
			MainForm.BrowserList.Enabled = true;
			MainForm.BrowserTree.Visible = false; 
			PopulateBrowser();
		}

		public void PopulateBrowser()
		{
			MainForm.BrowserList.Items.Clear();
			foreach (ImageMapSegment seg in Decompiler.Program.Image.Map.Segments.Values)
			{
				ListViewItem node = new ListViewItem(seg.Name);
				node.Tag = seg;
				MainForm.BrowserList.Items.Add(node);
			}
		}

		public override bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
		{
			if (cmdSet == CmdSets.GuidDecompiler)
			{
				MenuCommand cmd = (MenuCommand) mpCmdidToCommand[cmdId];
				if (cmd == null)
					return false;
				status.Status = (MenuStatus) cmd.OleStatus;
				return true;
			}
			return false;
		}
	}
}
