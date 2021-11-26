#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Gui.Services;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;

namespace Reko.Gui.Forms
{
    public interface InitialPageInteractor : IPhasePageInteractor
    {
        bool OpenBinary(string file);
        bool OpenBinaryAs(string file, LoadDetails details);
        bool Assemble(string file, IAssembler asm, IPlatform platform);
    }

    /// <summary>
    /// Handles interactions on InitialPage.
    /// </summary>
    public class InitialPageInteractorImpl : PhasePageInteractorImpl, InitialPageInteractor
    {
        public InitialPageInteractorImpl(IServiceProvider services) : base(services)
        {
        }

        protected virtual IDecompiler CreateDecompiler(Project project)
        {
            return new Decompiler(project, Services);
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
        /// <param name="file">The file system path to the file.</param>
        public bool OpenBinary(string file)
        {
            var ldr = Services.RequireService<ILoader>();
            var svc = Services.RequireService<IWorkerDialogService>();
            var uiSvc = Services.RequireService<IDecompilerShellUiService>();
            ILoadedImage loadedImage = null;
            var imageUri = ImageLocation.FromUri(file);
            bool exceptionThrown = !svc.StartBackgroundWork("Opening file", () =>
            {
                var eventListener = Services.RequireService<DecompilerEventListener>();
                eventListener.ShowStatus("Loading file.");
                loadedImage = ldr.Load(imageUri);
                eventListener.ShowStatus("Loaded file.");
            });
            if (exceptionThrown)
            {
                // We've already reported the exception, so we should stop in our tracks.
                return false;
            }
            if (loadedImage is IArchive archive)
            {
                loadedImage = LoadFromArchive(archive, ldr);
            }
            if (loadedImage is Blob blob)
            {
                var dlgFactory = Services.RequireService<IDialogFactory>();
                using IOpenAsDialog dlg = dlgFactory.CreateOpenAsDialog(file);
                if (uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
                {
                    return OpenBinaryAs(file, dlg.GetLoadDetails());
                }
            }
            if (loadedImage is not Project project)
            {
                return false;
            }

            this.Decompiler = CreateDecompiler(project);
            Decompiler.ExtractResources();
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            browserSvc.Load(project);
            browserSvc.Show();
            ShowLowLevelWindow();
            return true;
        }

        public bool OpenBinaryAs(string file, LoadDetails details)
        {
            var ldr = Services.RequireService<ILoader>();
            IWorkerDialogService svc = Services.RequireService<IWorkerDialogService>();
            if (!svc.StartBackgroundWork("Loading program", delegate ()
            {
                var eventListener = Services.RequireService<DecompilerEventListener>();
                eventListener.ShowStatus("Loading source program.");
                var imageUri = ImageLocation.FromUri(file);
                Program program = ldr.LoadRawImage(imageUri, details);
                var project = new Project();
                project.AddProgram(imageUri, program);
                this.Decompiler = CreateDecompiler(project);
                Decompiler.ExtractResources();
                eventListener.ShowStatus("Source program loaded.");
            }))
                return false;
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            var procListSvc = Services.RequireService<IProcedureListService>();
            if (Decompiler.Project is not null)
            {
                browserSvc.Load(Decompiler.Project);
                browserSvc.Show();
                procListSvc.Clear();
                ShowLowLevelWindow();
                return true;
            }
            else
            {
                browserSvc.Clear();
                procListSvc.Clear();
                return false;
            }
        }

        private Project LoadFromArchive(IArchive archive, ILoader loader)
        {
            var abSvc = Services.RequireService<IArchiveBrowserService>();
            if (abSvc.SelectFileFromArchive(archive) is not ArchivedFile archiveFile)
                return null;
            switch (archiveFile.LoadImage(Services, null))
            {
            case Program program:
                var project = new Project();
                Debug.Assert(program.Uri is not null);
                project.AddProgram(program.Uri, program);
                return project;
            case null:
                return null;
            default:
                throw new NotImplementedException();
            }
        }

        private void ShowLowLevelWindow()
        {
            if (Decompiler.Project is null)
                return;
            if (Decompiler.Project.Programs.Any(p => p.NeedsScanning))
            {
                var memSvc = Services.RequireService<ILowLevelViewService>();
                memSvc.ViewImage(Decompiler.Project.Programs.First(p => p.NeedsScanning));
            }
        }

        public bool Assemble(string file, IAssembler asm, IPlatform platform)
        {
            var ldr = Services.RequireService<ILoader>();
            var svc = Services.RequireService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", delegate()
            {
                var eventListener = Services.RequireService<DecompilerEventListener>();
                eventListener.ShowStatus("Assembling program.");
                var asmFileUri = ImageLocation.FromUri(file);
                var program = ldr.AssembleExecutable(asmFileUri, asm, platform, null!);
                var project = new Project();
                project.AddProgram(asmFileUri, program);
                this.Decompiler = CreateDecompiler(project);
                this.Decompiler.ExtractResources();
                eventListener.ShowStatus("Assembled program.");
            });
            if (Decompiler.Project is null)
                return false;
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            browserSvc.Load(Decompiler.Project);
            browserSvc.Show();
            ShowLowLevelWindow();
            return true;
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