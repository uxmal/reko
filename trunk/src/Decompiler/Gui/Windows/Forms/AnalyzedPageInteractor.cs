#region License
/* 
* Copyright (C) 1999-2014 John Källén.
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
#endregion

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public interface IAnalyzedPageInteractor : IPhasePageInteractor
    {
    }

	public class AnalyzedPageInteractorImpl : PhasePageInteractorImpl, IAnalyzedPageInteractor
	{
        private IDecompilerService decompilerSvc;
        private ICodeViewerService codeViewerSvc;
        private IMemoryViewService memViewerSvc;
        private IDisassemblyViewService disasmViewerSvc;
        private bool canAdvance;

		public AnalyzedPageInteractorImpl()
		{
            this.canAdvance = true;
		}

        private void DisplayProcedure(Address addr, Procedure proc)
        {
            if (addr != null && proc != null)
            {
                memViewerSvc.ShowMemoryAtAddress(decompilerSvc.Decompiler.Program, addr);
                codeViewerSvc.DisplayProcedure(proc);
            }
        }

        public override void PerformWork(IWorkerDialogService workerDlgSvc)
        {
            workerDlgSvc.SetCaption("Generating intermediate code");
            Decompiler.AnalyzeDataFlow();
        }

        public override bool CanAdvance
        {
            get { return canAdvance; }
        }

        public override void EnterPage()
        {
            PopulateBrowserListWithProcedures();
        }


		public override bool LeavePage()
		{
			return true;
        }


        private void PopulateBrowserListWithProcedures()
        {
            //$TODO!
        }

        private void EditSignature()
        {
            var arch = Decompiler.Program.Architecture;
            ProcedureSerializer ser = new ProcedureSerializer(arch, "stdapi");
            SerializedProcedure proc = ser.Serialize(SelectedProcedureEntry.Value, SelectedProcedureEntry.Key);
            ProcedureDialogInteractor i = new ProcedureDialogInteractor(arch, proc);
            using (ProcedureDialog dlg = i.CreateDialog())
            {
                if (DialogResult.OK == UIService.ShowModalDialog(dlg))
                {
                    //$REVIEW: Need to pass InputFile into the SelectedProcedureEntry piece.
                    Decompiler.Project.InputFiles[0].UserProcedures[SelectedProcedureEntry.Key] =
                        i.SerializedProcedure;
                    ser = new ProcedureSerializer(arch, "stdapi");
                    SelectedProcedureEntry.Value.Signature =
                        ser.Deserialize(i.SerializedProcedure.Signature, SelectedProcedureEntry.Value.Frame);

                    canAdvance = false;
                }
            }
        }

        public KeyValuePair<Address, Procedure> SelectedProcedureEntry
        {
            get
            {
                return new KeyValuePair<Address, Procedure>(null, null);
            }
        }

        public override ISite Site
        {
            get { return base.Site; }
            set 
            {
                base.Site = value;
                decompilerSvc = Site.RequireService<IDecompilerService>();
                codeViewerSvc = Site.RequireService<ICodeViewerService>();
                memViewerSvc = Site.RequireService<IMemoryViewService>();
                disasmViewerSvc = Site.RequireService<IDisassemblyViewService>();
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
                    EditSignature();
                    return true;
                }
            }
            return base.Execute(ref cmdSet, cmdId);
        }


        #endregion

        public void BrowserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayProcedure(SelectedProcedureEntry.Key, SelectedProcedureEntry.Value);
        }
	}
}
