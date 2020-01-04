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

using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.Gui.Forms
{
    public interface IScannedPageInteractor : IPhasePageInteractor
    {
    }

    public class ScannedPageInteractor : PhasePageInteractorImpl, IScannedPageInteractor
    {
        private Dictionary<int, MenuCommand> mpCmdidToCommand;
        private IDecompilerService decompilerSvc;
        private IStatusBarService sbSvc;
        private ILowLevelViewService memSvc;

        public ScannedPageInteractor(IServiceProvider services) : base(services)
        {
            decompilerSvc = services.RequireService<IDecompilerService>();
            sbSvc = services.RequireService<IStatusBarService>();
            memSvc = services.RequireService<ILowLevelViewService>();

            mpCmdidToCommand = new Dictionary<int, MenuCommand>();
            AddCommand(new CommandID(CmdSets.GuidReko, CmdIds.ViewShowAllFragments));
            AddCommand(new CommandID(CmdSets.GuidReko, CmdIds.ViewShowUnscanned));
        }

        protected MenuCommand AddCommand(CommandID cmdId)
        {
            MenuCommand mc = new MenuCommand(null, cmdId);
            mpCmdidToCommand.Add(mc.CommandID.ID, mc);
            return mc;
        }

        public override bool Execute(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.ViewShowUnscanned:
                    return ViewUnscannedBlocks();
                }
            }
            return base.Execute(cmdId);
        }

        public override void PerformWork(IWorkerDialogService workerDialogSvc)
        {
            workerDialogSvc.SetCaption("Scanning source program.");
            Decompiler.ScanPrograms();
        }

        public override void EnterPage()
        {
            if (Decompiler.Project.Programs.Any(p => p.NeedsScanning))
            {
                memSvc.ViewImage(Decompiler.Project.Programs.First());
            }
            Services.RequireService<IProjectBrowserService>().Reload();
            var procSvc = Services.RequireService<IProcedureListService>();
            procSvc.Load(Decompiler.Project);
            procSvc.Show();
        }

        public override bool LeavePage()
        {
            return true;
        }

        /// <summary>
        /// Shows a list of all blocks that have not yet been scanned.
        /// </summary>
        /// <returns></returns>
        public bool ViewUnscannedBlocks()
        {
            var srSvc = Services.RequireService<ISearchResultService>();
            var hits = Decompiler.Project.Programs
                .SelectMany(p => p.ImageMap.Items
                        .Where(i => i.Value.DataType is UnknownType)
                        .Select(i => new AddressSearchHit
                        {
                            Program = p,
                            Address = i.Key,
                            Length = 1
                        }));
            srSvc.ShowSearchResults(
                new AddressSearchResult(
                    Services,
                    hits,
                    new CodeSearchDetails()));
            return true;
        }

        public override bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.ActionFinishDecompilation:
                case CmdIds.ActionRestartDecompilation:
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                    return true;
                case CmdIds.ActionNextPhase:
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                    text.Text = Resources.AnalyzeDataflow;
                    return true;
                default:
                    MenuCommand cmd;
                    if (!mpCmdidToCommand.TryGetValue(cmdId.ID, out cmd))
                        return false;
                    status.Status = (MenuStatus)cmd.OleStatus;
                    return true;
                }
            }
            return false;
        }

        private void ShowMemoryControlRange(IStatusBarService sbSvc, AddressRange range)
        {
            if (range.Begin == null || range.End == null)
                return;
            if (range.Begin.ToLinear() == range.End.ToLinear())       //$REFACTOR: make bytespan a method of addressrange.
            {
                sbSvc.SetText(string.Format("[{0}]", range.Begin));
            }
            else
            {
                sbSvc.SetText(string.Format("[{0}-{1}]", range.Begin, range.End));
            }
        }
    }
}
