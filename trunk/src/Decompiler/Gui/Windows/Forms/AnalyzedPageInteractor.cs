#region License
/* 
* Copyright (C) 1999-2015 John Källén.
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
using System.ComponentModel.Design;
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
        private ILowLevelViewService memViewerSvc;
        private IDisassemblyViewService disasmViewerSvc;
        private IProjectBrowserService projectSvc;
        private bool canAdvance;

		public AnalyzedPageInteractorImpl(IServiceProvider services) : base(services)
		{
            decompilerSvc = services.RequireService<IDecompilerService>();
            codeViewerSvc = services.RequireService<ICodeViewerService>();
            memViewerSvc = services.RequireService<ILowLevelViewService>();
            disasmViewerSvc = services.RequireService<IDisassemblyViewService>();
            projectSvc = services.RequireService<IProjectBrowserService>();

            this.canAdvance = true;
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
            projectSvc.Reload();
        }


		public override bool LeavePage()
		{
			return true;
        }

        private void EditSignature()
        { 
            //$TODO: need "current program"
            IProcessorArchitecture arch = null; // Decompiler.Program.Architecture;
            var ser = arch.CreateProcedureSerializer(new TypeLibraryLoader(arch, true), "stdapi");
            var proc = ser.Serialize(SelectedProcedureEntry.Value, SelectedProcedureEntry.Key);
            var i = new ProcedureDialogInteractor(arch, proc);
            using (ProcedureDialog dlg = i.CreateDialog())
            {
                if (DialogResult.OK == UIService.ShowModalDialog(dlg))
                {
                    //$REVIEW: Need to pass InputFile into the SelectedProcedureEntry piece.
                    var program =  Decompiler.Project.Programs[0]; 
                    program.UserProcedures[SelectedProcedureEntry.Key] =
                        i.SerializedProcedure;
                    ser = arch.CreateProcedureSerializer(new TypeLibraryLoader(arch, true), "stdapi");
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

        #region ICommandTarget interface 
        public override bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                switch (cmdId.ID)
                {
                case CmdIds.ActionEditSignature:
                    status.Status = MenuStatus.Visible;
                    if (SelectedProcedureEntry.Key != null)
                        status.Status |= MenuStatus.Enabled;
                    return true;
                }
            }
            return base.QueryStatus(cmdId, status, text);
        }

        public override bool Execute(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                switch (cmdId.ID)
                {
                case CmdIds.ActionEditSignature:
                    EditSignature();
                    return true;
                }
            }
            return base.Execute(cmdId);
        }

        #endregion

	}
}
