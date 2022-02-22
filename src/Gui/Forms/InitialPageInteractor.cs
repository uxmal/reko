#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Threading.Tasks;

#nullable enable

namespace Reko.Gui.Forms
{
    public interface InitialPageInteractor : IPhasePageInteractor
    {
        ValueTask<bool> OpenBinary(string file);
        ValueTask<bool> OpenBinaryAs(string file, LoadDetails details);
        ValueTask<bool> Assemble(string file, IAssembler asm, IPlatform platform);
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
        public async ValueTask<bool> OpenBinary(string file)
        {
            var ldr = Services.RequireService<ILoader>();
            var svc = Services.RequireService<IWorkerDialogService>();
            ILoadedImage? loadedImage = null;
            var imageUri = ImageLocation.FromUri(file);
            bool exceptionThrown = !await svc.StartBackgroundWork("Opening file", () =>
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
            switch (loadedImage)
            {
            case Program program:
                loadedImage = Project.FromSingleProgram(program);
                break;
            case IArchive archive:
                loadedImage = await LoadFromArchive(archive, ldr);
                break;
            case Blob blob:
                var details = await ShowOpenBinaryAsDialog(file);
                if (details is null)
                    return false;
                var rawProgram = ldr.LoadRawImage(blob.Location, blob.Image, null, details);
                loadedImage = Project.FromSingleProgram(rawProgram);
                break;
            }
            if (loadedImage is not Project project)
            {
                return false;
            }

            this.Decompiler = CreateDecompiler(project);
            Decompiler.ExtractResources();
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            var procListSvc = Services.RequireService<IProcedureListService>();
            browserSvc.Load(project);
            browserSvc.Show();
            procListSvc.Clear();
            ShowLowLevelWindow();
            return true;
        }

        private async ValueTask<LoadDetails?> ShowOpenBinaryAsDialog(string file)
        {
            var uiSvc = Services.RequireService<IDecompilerShellUiService>();
            var dlgFactory = Services.RequireService<IDialogFactory>();
            using (IOpenAsDialog dlg = dlgFactory.CreateOpenAsDialog(file))
            {
                if (await uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
                {
                    return dlg.GetLoadDetails();
                }
                else
                {
                    return null;
                }
            }
        }

        public async ValueTask<bool> OpenBinaryAs(string file, LoadDetails details)
        {
            var ldr = Services.RequireService<ILoader>();
            IWorkerDialogService svc = Services.RequireService<IWorkerDialogService>();
            if (!await svc.StartBackgroundWork("Loading program", delegate ()
            {
                //$TODO: taskify this.
                var eventListener = Services.RequireService<DecompilerEventListener>();
                eventListener.ShowStatus("Loading source program.");
                var imageUri = ImageLocation.FromUri(file);
                Program program = ldr.LoadRawImage(imageUri, details);
                var project = Project.FromSingleProgram(program);
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

        private async ValueTask<Project?> LoadFromArchive(IArchive archive, ILoader loader)
        {
            var abSvc = Services.RequireService<IArchiveBrowserService>();
            while (await abSvc.SelectFileFromArchive(archive) is ArchivedFile archiveFile)
            {
                var image = archiveFile.LoadImage(Services, null);
                if (image is Blob blob)
                {
                    // The archive itself doesn't know the format of the blob,
                    // perhaps Reko does.
                    image = loader.LoadBinaryImage(blob.Location, blob.Image, null, null);
                }
                switch (image)
                {
                case Program program:
                    Debug.Assert(program.Location is not null);
                    return Project.FromSingleProgram(program);
                case Blob blob2:
                    //$TODO: make 'filename' textbox readonly when there are fragments in path.
                    var loadDetails = await ShowOpenBinaryAsDialog(blob2.Location.FilesystemPath);
                    if (loadDetails is null)
                        return null;
                    var rawProgram = loader.LoadRawImage(blob2.Location, blob2.Image, null, loadDetails);
                    return Project.FromSingleProgram(rawProgram);
                case IArchive nestedArchive:
                    archive = nestedArchive;
                    break;
                default:
                    throw new NotImplementedException();
                }
            }
            return null;
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

        public async ValueTask<bool> Assemble(string file, IAssembler asm, IPlatform platform)
        {
            var ldr = Services.RequireService<ILoader>();
            var svc = Services.RequireService<IWorkerDialogService>();
            await svc.StartBackgroundWork("Loading program", delegate()
            {
                var eventListener = Services.RequireService<DecompilerEventListener>();
                eventListener.ShowStatus("Assembling program.");
                var asmFileLocation = ImageLocation.FromUri(file);
                var program = ldr.AssembleExecutable(asmFileLocation, asm, platform, null!);
                var project = Project.FromSingleProgram(program);
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