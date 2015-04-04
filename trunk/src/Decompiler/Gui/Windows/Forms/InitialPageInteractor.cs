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

using Decompiler.Core.Assemblers;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Core.Configuration;
using Decompiler.Gui;
using Decompiler.Loading;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public interface InitialPageInteractor : IPhasePageInteractor
    {
        bool OpenBinary(string file, DecompilerHost host);
        bool OpenBinaryAs(string file, IProcessorArchitecture arch, Platform platform, Address addrBase, DecompilerHost host);
        bool Assemble(string file, Assembler asm, DecompilerHost host);
    }

    /// <summary>
    /// Handles interactions on InitialPage.
    /// </summary>
    public class InitialPageInteractorImpl : PhasePageInteractorImpl, InitialPageInteractor
    {
        public InitialPageInteractorImpl(IServiceProvider services) : base(services)
        {
        }

        protected virtual IDecompiler CreateDecompiler(ILoader ldr, DecompilerHost host)
        {
            return new DecompilerDriver(ldr, host, Services);
        }

        public override bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid== CmdSets.GuidDecompiler)
            {
                switch (cmdId.ID)
                {
                case CmdIds.ViewGoToAddress:
                case CmdIds.ViewShowAllFragments:
                case CmdIds.ViewShowUnscanned:
                case CmdIds.ActionEditSignature:
                case CmdIds.ActionMarkProcedure:
                    status.Status = 0;
                    return true;
                }
            }
            return base.QueryStatus(cmdId, status, text);
        }

        public void EnableControls()
        {
        }

        public override void PerformWork(IWorkerDialogService workerDlgSvc)
        {
        }

        public override void EnterPage()
        {
            EnableControls();
        }

        public override bool LeavePage()
        {
            return (Decompiler != null);
        }

        public bool OpenBinary(string file, DecompilerHost host)
        {
            var ldr = Services.RequireService<ILoader>();
            this.Decompiler = CreateDecompiler(ldr, host);
            IWorkerDialogService svc = Services.RequireService<IWorkerDialogService>();
            bool isOldProject = false;
            svc.StartBackgroundWork("Loading program", delegate()
            {
                isOldProject = Decompiler.Load(file);
            });
            if (Decompiler.Project == null)
                return false;
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            browserSvc.Load(Decompiler.Project);
            if (Decompiler.Project.Programs.Count > 0)
            {
                var memSvc = Services.RequireService<ILowLevelViewService>();
                memSvc.ViewImage(Decompiler.Project.Programs.First());
            }
            return isOldProject;
        }

        public bool OpenBinaryAs(
            string file, 
            IProcessorArchitecture arch,
            Platform platform, 
            Address addrBase, 
            DecompilerHost host)
        {
            var ldr = Services.RequireService<ILoader>();
            this.Decompiler = CreateDecompiler(ldr, host);
            IWorkerDialogService svc = Services.RequireService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", delegate()
            {
                Decompiler.LoadRawImage(file, arch, platform, addrBase);
                svc.SetCaption("Scanning source program.");
                Decompiler.ScanPrograms();
            });
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            browserSvc.Load(Decompiler.Project);
            if (Decompiler.Project.Programs.Count > 0)
            {
                var memSvc = Services.RequireService<ILowLevelViewService>();
                memSvc.ViewImage(Decompiler.Project.Programs.First());
            }
            return false;   // We never open projects this way.
        }

        public bool Assemble(string file, Assembler asm, DecompilerHost host)
        {
            var ldr = Services.RequireService<ILoader>();
            this.Decompiler = CreateDecompiler(ldr, host);
            var svc = Services.RequireService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", delegate()
            {
                Decompiler.Assemble(file, asm);
            });
            if (Decompiler.Project == null)
                return false;
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            browserSvc.Load(Decompiler.Project);
            if (Decompiler.Project.Programs.Count > 0)
            {
                var memSvc = Services.RequireService<ILowLevelViewService>();
                memSvc.ViewImage(Decompiler.Project.Programs.First());
            }
            return false;
        }

    }
}