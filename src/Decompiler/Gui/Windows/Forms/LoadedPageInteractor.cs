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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using Decompiler.Gui.Windows.Controls;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public interface ILoadedPageInteractor : IPhasePageInteractor
    {
    }

	public class LoadedPageInteractor : PhasePageInteractorImpl, ILoadedPageInteractor
	{
		private ILoadedPage pageLoaded;
		private Hashtable mpCmdidToCommand;
        private IProgramImageBrowserService browserSvc;
        private IStatusBarService sbSvc;
        private IMemoryViewService memSvc;
        private IDisassemblyViewService disSvc;

		public LoadedPageInteractor() 
		{
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
            throw new NotImplementedException();
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

        public override void EnterPage()
        {
            browserSvc.Enabled = true;
            browserSvc.SelectionChanged += BrowserItemSelected;

            memSvc.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(memctl_SelectionChanged);
            memSvc.ViewImage(Decompiler.Program.Image);
            disSvc.ShowWindow();
            disSvc.Clear();

            PopulateBrowser();
        }

		public override bool LeavePage()
		{
            browserSvc.SelectionChanged -= BrowserItemSelected;
            memSvc.SelectionChanged -= memctl_SelectionChanged;

			return true;
		}

        public void MarkAndScanProcedure()
        {
            AddressRange addrRange = memSvc.GetSelectedAddressRange();
            if (addrRange.IsValid)
            {
                Procedure proc = Decompiler.ScanProcedure(addrRange.Begin);
                SerializedProcedure userp = new SerializedProcedure();
                userp.Address = addrRange.Begin.ToString();
                userp.Name = proc.Name;
                Decompiler.Project.UserProcedures.Add(userp);
                memSvc.InvalidateWindow();
            }
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

        public override ISite Site
        {
            set
            {
                base.Site = value;
                if (value != null)
                {
                    browserSvc = EnsureService<IProgramImageBrowserService>();
                    sbSvc = EnsureService<IStatusBarService>();
                    memSvc = EnsureService<IMemoryViewService>();
                    disSvc = EnsureService<IDisassemblyViewService>();
                }
                else
                {
                    browserSvc = null;
                    sbSvc = null;
                }
            }
        }

        private void ShowMemoryControlRange(IStatusBarService sbSvc, AddressRange range)
        {
            if (range.Begin == null || range.End == null)
                return;
            if (range.Begin.Linear == range.End.Linear)       //$REFACTOR: make bytespan a method of addressrange.
            {
                sbSvc.SetText(string.Format("[{0}]", range.Begin));
            }
            else
            {
                sbSvc.SetText(string.Format("[{0}-{1}]", range.Begin, range.End));
            }
        }


        // Event handlers /////////////////////////

        private void memctl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sbSvc != null)
            {
                ShowMemoryControlRange(sbSvc, e.AddressRange);
                disSvc.DisassembleStartingAtAddress(e.AddressRange.Begin);

            }
        }


        public void BrowserItemSelected(object sender, EventArgs e)
        {
            ImageMapSegment segment = (ImageMapSegment) browserSvc.FocusedItem;
            pageLoaded.MemoryControl.TopAddress = segment.Address;
            pageLoaded.MemoryControl.SelectedAddress = segment.Address;
        }
    }
}
