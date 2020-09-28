#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Assemblers;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Configuration;
using Reko.Gui;
using Reko.Loading;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.Gui.Forms
{
    public interface InitialPageInteractor : IPhasePageInteractor
    {
        bool OpenBinary(string file);
        bool OpenBinaryAs(string file, LoadDetails details);
        bool Assemble(string file, Assembler asm);
    }

    /// <summary>
    /// Handles interactions on InitialPage.
    /// </summary>
    public class InitialPageInteractorImpl : PhasePageInteractorImpl, InitialPageInteractor
    {
        public InitialPageInteractorImpl(IServiceProvider services) : base(services)
        {
        }

        protected virtual IDecompiler CreateDecompiler(ILoader ldr)
        {
            return new Decompiler(ldr, Services);
        }

        public override bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid== CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.ViewGoToAddress:
                case CmdIds.ViewShowAllFragments:
                case CmdIds.ViewShowUnscanned:
                case CmdIds.ActionMarkProcedure:
                case CmdIds.ActionRestartDecompilation:
                    status.Status = 0;
                    return true;
                case CmdIds.ActionFinishDecompilation:
                    status.Status = CanAdvance
                        ? MenuStatus.Visible | MenuStatus.Enabled
                        : MenuStatus.Visible; return true;
                case CmdIds.ActionNextPhase:
                    status.Status = CanAdvance 
                        ? MenuStatus.Visible | MenuStatus.Enabled
                        : MenuStatus.Visible;
                    text.Text = NeedsScanning() 
                        ? Resources.ScanBinaries 
                        : Resources.AnalyzeDataflow;
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

        /// <summary>
        /// Open the specified file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>True if the opened file was opened successfully.</returns>
        public bool OpenBinary(string file)
        {
            var ldr = Services.RequireService<ILoader>();
            this.Decompiler = CreateDecompiler(ldr);
            var svc = Services.RequireService<IWorkerDialogService>();
            bool successfullyLoaded = false;
            svc.StartBackgroundWork("Loading program", () =>
            {
                successfullyLoaded = Decompiler.Load(file);
            });
            if (!successfullyLoaded)
            {
                return false;
            }
            Decompiler.ExtractResources();

            var browserSvc = Services.RequireService<IProjectBrowserService>();
            browserSvc.Load(Decompiler.Project);
            browserSvc.Show();
            ShowLowLevelWindow();
            return true;
        }

        public bool OpenBinaryAs(string file, LoadDetails details)
        {
            var ldr = Services.RequireService<ILoader>();
            this.Decompiler = CreateDecompiler(ldr);
            IWorkerDialogService svc = Services.RequireService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", delegate()
            {
                Program program = Decompiler.LoadRawImage(file, details);
                Decompiler.ExtractResources();
            });
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            var procListSvc = Services.RequireService<IProcedureListService>();
            if (Decompiler.Project != null)
            {
                browserSvc.Load(Decompiler.Project);
                browserSvc.Show();
                procListSvc.Clear();
                ShowLowLevelWindow();
            }
            else
            {
                browserSvc.Clear();
                procListSvc.Clear();
            }
            return false;   // We never open projects this way.
        }

        private void ShowLowLevelWindow()
        {
            if (Decompiler.Project.Programs.Any(p => p.NeedsScanning))
            {
                var memSvc = Services.RequireService<ILowLevelViewService>();
                memSvc.ViewImage(Decompiler.Project.Programs.First(p => p.NeedsScanning));
            }
        }

        public bool Assemble(string file, Assembler asm)
        {
            var ldr = Services.RequireService<ILoader>();
            this.Decompiler = CreateDecompiler(ldr);
            var svc = Services.RequireService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", delegate()
            {
                Decompiler.Assemble(file, asm);
            });
            if (Decompiler.Project == null)
                return false;
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            browserSvc.Load(Decompiler.Project);
            browserSvc.Show();
            ShowLowLevelWindow();
            return false;
        }

        private bool NeedsScanning()
        {
            return
                Decompiler != null &&
                Decompiler.Project != null &&
                Decompiler.Project.Programs.Any(p => p.NeedsScanning);
        }
    }
}