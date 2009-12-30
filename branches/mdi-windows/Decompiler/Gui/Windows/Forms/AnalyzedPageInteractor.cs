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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public interface IAnalyzedPageInteractor : IPhasePageInteractor
    {
    }

	public class AnalyzedPageInteractorImpl : PhasePageInteractorImpl, IAnalyzedPageInteractor
	{
        private IProgramImageBrowserService browserSvc;
        private ICodeViewerService codeViewerSvc;

		public AnalyzedPageInteractorImpl()
		{
		}

        private void DisplayProcedure(Procedure proc)
        {
            codeViewerSvc.DisplayProcedure(proc);
        }


        public override void EnterPage()
        {
            browserSvc.Enabled = true;

            WorkerDialogService.StartBackgroundWork(
                "Generating intermediate code",
                delegate()
                {
                    Decompiler.RewriteMachineCode();
                    Decompiler.AnalyzeDataFlow();
                });

            PopulateBrowserListWithProcedures();
            browserSvc.SelectionChanged += new EventHandler(BrowserList_SelectedIndexChanged);
        }


		public override bool LeavePage()
		{
            browserSvc.SelectionChanged -= new EventHandler(BrowserList_SelectedIndexChanged);
			return true;
        }


        private void PopulateBrowserListWithProcedures()
        {
            browserSvc.Populate(Decompiler.Program.Procedures, delegate(object item, IListViewItem listItem)
            {
                KeyValuePair<Address, Procedure> entry = (KeyValuePair<Address, Procedure>) item;
                listItem.Text = entry.Value.Name;
            });
        }

        public KeyValuePair<Address, Procedure> SelectedProcedureEntry
        {
            get
            {
                if (!browserSvc.IsItemSelected)
                    return new KeyValuePair<Address,Procedure>(null, null);
                KeyValuePair<Address, Procedure> entry = (KeyValuePair<Address, Procedure>) browserSvc.SelectedItem;
                return entry;
            }
        }

        public override System.ComponentModel.ISite Site
        {
            get { return base.Site; }
            set 
            {
                base.Site = value;
                browserSvc = EnsureService<IProgramImageBrowserService>();
                codeViewerSvc = EnsureService<ICodeViewerService>();
            }
        }

        #region ICommandTarget interface 
        public override bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
        {
            if (cmdSet == CmdSets.GuidDecompiler)
            {
                switch (cmdId)
                {
                case CmdIds.ActionEditSignature:
                    status.Status = MenuStatus.Visible;
                    if (SelectedProcedureEntry.Key != null)
                        status.Status |= MenuStatus.Enabled;
                    return true;
                }
            }
            return base.QueryStatus(ref cmdSet, cmdId, status, text);
        }

        public override bool Execute(ref Guid cmdSet, int cmdId)
        {
            if (cmdSet == CmdSets.GuidDecompiler)
            {
                switch (cmdId)
                {
                case CmdIds.ActionEditSignature:
                    {
                        ProcedureSerializer ser = new ProcedureSerializer(Decompiler.Program.Architecture, "stdapi");
                        SerializedProcedure proc = ser.Serialize(SelectedProcedureEntry.Value, SelectedProcedureEntry.Key);
                        ProcedureDialogInteractor i = new ProcedureDialogInteractor(proc);
                        using (ProcedureDialog dlg = i.CreateDialog())
                        {
                            if (DialogResult.OK == UIService.ShowModalDialog(dlg))
                            {
                                i.ApplyChangesToProcedure(SelectedProcedureEntry.Value);
                                //$TODO: prohibit stepping forward, only go back to previous steps.
                            }
                        }
                    }
                    return true;
                }
            }
            return base.Execute(ref cmdSet, cmdId);
        }

        #endregion

        public void BrowserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayProcedure(SelectedProcedureEntry.Value);
        }


	}
}
