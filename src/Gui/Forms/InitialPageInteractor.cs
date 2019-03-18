#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Configuration;
using Reko.Gui;
using Reko.Loading;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Reko.Gui.Forms
{
    public interface InitialPageInteractor : IPhasePageInteractor
    {
        bool OpenBinary(string file, string outputDir);
        bool OpenBinaryAs(string file, string outputDir, LoadDetails details);
        bool Assemble(string file, string outputDir, Assembler asm);
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
            return new DecompilerDriver(ldr, Services);
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
        /// <returns>True if the opened file was a Reko project.</returns>
        public bool OpenBinary(string file, string outputDir)
        {
            var ldr = Services.RequireService<ILoader>();
            this.Decompiler = CreateDecompiler(ldr);
            var svc = Services.RequireService<IWorkerDialogService>();
            bool isOldProject = false;
            svc.StartBackgroundWork("Loading program", delegate ()
            {
                isOldProject = Decompiler.Load(file, outputDir);
            });
            if (Decompiler.Project == null)
            {
                return false;
            }

            ProgramResourceGroup prg = Decompiler.Project.Programs[0].Resources;

            // Process the resources adn save as files
            ProcessResources(prg, outputDir);

            var browserSvc = Services.RequireService<IProjectBrowserService>();
            browserSvc.Load(Decompiler.Project);
            ShowLowLevelWindow();
            return isOldProject;
        }

        public bool OpenBinaryAs(string file, string outputDir, LoadDetails details)
        {
            var ldr = Services.RequireService<ILoader>();
            this.Decompiler = CreateDecompiler(ldr);
            IWorkerDialogService svc = Services.RequireService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", delegate()
            {
                Program program = Decompiler.LoadRawImage(file, outputDir, details);
                ProgramResourceGroup prg = program.Resources;

                // Process the resources adn save as files
                ProcessResources(prg, outputDir);
            });
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            if (Decompiler.Project != null)
            {
                browserSvc.Load(Decompiler.Project);
                ShowLowLevelWindow();
            }
            else
            {
                browserSvc.Clear();
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

        public bool Assemble(string file, string outputDir, Assembler asm)
        {
            var ldr = Services.RequireService<ILoader>();
            this.Decompiler = CreateDecompiler(ldr);
            var svc = Services.RequireService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", delegate()
            {
                Decompiler.Assemble(file, outputDir, asm);
            });
            if (Decompiler.Project == null)
                return false;
            var browserSvc = Services.RequireService<IProjectBrowserService>();
            browserSvc.Load(Decompiler.Project);
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

        private void ProcessResources(ProgramResourceGroup prg, string outputDir)
        {
            try
            {
                if (prg.Name == "PE resources")
                {
                    // Create the dir
                    Directory.CreateDirectory(outputDir + "\\resources");
                    foreach (ProgramResourceGroup pr in prg.Resources)
                    {
                        switch (pr.Name)
                        {
                            case "CURSOR":
                            {
                                WriteResourceFile(outputDir + "\\resources", "Cursor", ".cur", pr);
                            }
                            break;
                            case "BITMAP":
                            {
                                WriteResourceFile(outputDir + "\\resources", "Bitmap", ".bmp", pr);
                            }
                            break;
                            case "ICON":
                            {
                                WriteResourceFile(outputDir + "\\resources", "Icon", ".ico", pr);
                            }
                            break;
                            case "FONT":
                            {
                                WriteResourceFile(outputDir + "\\resources", "Font", ".bin", pr);
                            }
                            break;
                            case "NEWBITMAP":
                            {
                                WriteResourceFile(outputDir + "\\resources", "NewBitmap", ".bmp", pr);
                            }
                            break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void WriteResourceFile(string outputDir, string ResourceType, string ext, ProgramResourceGroup pr)
        {
            Directory.CreateDirectory(outputDir + "\\" + ResourceType);
            foreach (ProgramResourceGroup pr1 in pr.Resources)
            {
                foreach (ProgramResourceInstance pr2 in pr1.Resources)
                {
                    // write the bytes to file
                    try
                    {
                        using (var fs = new FileStream(outputDir + "\\" + ResourceType + "\\" + pr1.Name + ext, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(pr2.Bytes, 0, pr2.Bytes.Length);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
    }
}