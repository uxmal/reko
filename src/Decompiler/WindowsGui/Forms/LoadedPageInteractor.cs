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

using Decompiler.Core;
using System;
using System.Collections;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
	public class LoadedPageInteractor : PhasePageInteractor
	{
		private LoadedPage pageLoaded;

		public LoadedPageInteractor(LoadedPage page, MainFormInteractor mi) : base(page, mi)
		{
			pageLoaded = page;
		}

		public void BrowserItemSelected()
		{
			ImageMapSegment segment = (ImageMapSegment) MainForm.BrowserTree.SelectedNode.Tag;
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
				}
			}
			return base.Execute(ref cmdSet, cmdId);
		}


		public override void OnPageEntered(object sender, EventArgs e)
		{
			pageLoaded.Architecture = MainInteractor.Program.Architecture;
			pageLoaded.MemoryControl.ProgramImage = MainInteractor.Program.Image;
			pageLoaded.MemoryControl.ImageMap = MainInteractor.Program.ImageMap;
			pageLoaded.Disassembly.Text = "";

			MainForm.BrowserTree.Visible = true;
			MainForm.BrowserTree.Enabled = true;
			MainForm.BrowserList.Visible = false; 
			PopulateBrowser();
			base.OnPageEntered(sender, e);
		}

		public void PopulateBrowser()
		{
			MainForm.BrowserTree.Nodes.Clear();
			foreach (ImageMapSegment seg in MainInteractor.Program.ImageMap.Segments.Values)
			{
				TreeNode node = new TreeNode(seg.Name);
				node.Tag = seg;
				MainForm.BrowserTree.Nodes.Add(node);
			}
		}
	}
}
