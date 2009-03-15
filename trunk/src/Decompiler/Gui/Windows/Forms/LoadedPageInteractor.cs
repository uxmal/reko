/* 
 * Copyright (C) 1999-2009 John Källén.
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
using Decompiler.WindowsGui.Controls;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
	public class LoadedPageInteractor : PhasePageInteractor
	{
		private ILoadedPage pageLoaded;
		private Hashtable mpCmdidToCommand;
        private IProgramImageBrowserService browserSvc;

		public LoadedPageInteractor(ILoadedPage page) 
		{
			this.pageLoaded = page;
            page.MemoryControl.SelectionChanged += new System.EventHandler(this.memctl_SelectionChanged);
            page.Disassembly.Resize += new System.EventHandler(this.txtDisassembly_Resize);

			mpCmdidToCommand = new Hashtable();
            AddCommand(ref CmdSets.GuidDecompiler, CmdIds.EditFind);
			AddCommand(ref CmdSets.GuidDecompiler, CmdIds.ViewShowAllFragments);
			AddCommand(ref CmdSets.GuidDecompiler, CmdIds.ViewShowUnscanned);
			AddCommand(ref CmdSets.GuidDecompiler, CmdIds.ViewFindFragments);
		}

		protected MenuCommand AddCommand(ref Guid cmdSet, int cmdId)
		{
			MenuCommand mc = new MenuCommand(null, new CommandID(cmdSet, cmdId));
			mpCmdidToCommand.Add(mc.CommandID.ID, mc);
			return mc;
		}


		public override bool Execute(ref Guid cmdSet, int cmdId)
		{
			if (cmdSet == CmdSets.GuidDecompiler)
			{
				switch (cmdId)
				{
                case CmdIds.EditFind:
                    EditFindBytes(); return true;
				case CmdIds.ViewGoToAddress:
					GotoAddress(); return true;
				case CmdIds.ActionMarkProcedure:
					MarkAndScanProcedure(); return true;
				}
			}
			return base.Execute(ref cmdSet, cmdId);
		}


        public void EditFindBytes()
        {
            FindDialogInteractor i = new FindDialogInteractor();
            using (FindDialog dlg = i.CreateDialog())
            {
                if (UIService.ShowModalDialog(dlg) == DialogResult.OK)
                {
                    FindMatchingBytes(i.ToHexadecimal(""));
                }
            }
        }

        private void FindMatchingBytes(byte [] pattern)
        {
            throw new Exception("The method or operation is not implemented.");
        }

		public void GotoAddress()
		{
			using (GotoDialog dlg = new GotoDialog())
			{
				GotoDialogInteractor i = new GotoDialogInteractor(dlg);
				if (UIService.ShowModalDialog(dlg) == DialogResult.OK)
				{
					pageLoaded.MemoryControl.SelectedAddress = i.Address;
					pageLoaded.MemoryControl.TopAddress = i.Address;
				}
			}
		}




        public void DumpAssembler()
        {
            if (Decompiler.Program.Architecture == null || Decompiler.Program.Image == null || pageLoaded.MemoryControl.SelectedAddress == null)
            {
                pageLoaded.Disassembly.Text = "";
                return;
            }
            int lines = (pageLoaded.Disassembly.Height + pageLoaded.Disassembly.Font.Height - 1) / pageLoaded.Disassembly.Font.Height;
            if (lines < 1)
                lines = 1;
            StringWriter writer = new StringWriter();
            Dumper dumper = Decompiler.Program.Architecture.CreateDumper();
            Disassembler dasm = Decompiler.Program.Architecture.CreateDisassembler(Decompiler.Program.Image, pageLoaded.MemoryControl.SelectedAddress);
            while (lines != 0)
            {
                dumper.DumpAssemblerLine(Decompiler.Program.Image, dasm, true, true, writer);
                --lines;
            }
            pageLoaded.Disassembly.Text = writer.ToString();
        }

		public override void EnterPage()
		{
            browserSvc.Enabled = true;
            browserSvc.SelectionChanged += BrowserItemSelected;

			Decompiler.ScanProgram();

			pageLoaded.MemoryControl.ProgramImage = Decompiler.Program.Image;
			pageLoaded.Disassembly.Text = "";

			PopulateBrowser();
		}

		public override bool LeavePage()
		{
            browserSvc.SelectionChanged -= BrowserItemSelected;
			return true;
		}

        public void MarkAndScanProcedure()
        {
            Address addr = pageLoaded.MemoryControl.SelectedAddress;
            if (addr != null)
            {
                Procedure proc = Decompiler.ScanProcedure(addr);
                SerializedProcedure userp = new SerializedProcedure();
                userp.Address = addr.ToString();
                userp.Name = proc.Name;
                Decompiler.Project.UserProcedures.Add(userp);
                pageLoaded.MemoryControl.Invalidate();
            }
        }

        public override object Page
        {
            get { return pageLoaded; }
        }

		public void PopulateBrowser()
		{
            browserSvc.Populate(Decompiler.Program.Image.Map.Segments.Values, delegate(object item, IListViewItem listItem)
            {
                listItem.Text = ((ImageMapSegment) item).Name;
            });
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

        public override System.ComponentModel.ISite Site
        {
            set
            {
                base.Site = value;
                if (value != null)
                {
                    browserSvc = (IProgramImageBrowserService) EnsureService(typeof(IProgramImageBrowserService));
                    pageLoaded.MemoryControl.ContextMenu  = base.UIService.GetContextMenu(MenuIds.CtxMemoryControl);

                }
                else
                {
                    browserSvc = null;
                }
            }
        }
        // Event handlers /////////////////////////

        private void memctl_SelectionChanged(object sender, System.EventArgs e)
        {
            DumpAssembler();
        }

        private void txtDisassembly_Resize(object sender, System.EventArgs e)
        {
            DumpAssembler();
        }

        public void BrowserItemSelected(object sender, EventArgs e)
        {
            ImageMapSegment segment = (ImageMapSegment) browserSvc.FocusedItem;
            pageLoaded.MemoryControl.TopAddress = segment.Address;
            pageLoaded.MemoryControl.SelectedAddress = segment.Address;
        }

	}
}
