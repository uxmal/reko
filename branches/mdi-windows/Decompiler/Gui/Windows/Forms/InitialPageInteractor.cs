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
using Decompiler.Configuration;
using Decompiler.Gui;
using Decompiler.Loading;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public interface InitialPageInteractor : IPhasePageInteractor
    {
        void OpenBinary(string file, DecompilerHost host);
    }

    /// <summary>
    /// Handles interactions on InitialPage.
    /// </summary>
    public class InitialPageInteractorImpl : PhasePageInteractorImpl, InitialPageInteractor
    {
        private IStartPage page;
        private IProgramImageBrowserService browserSvc;

        public InitialPageInteractorImpl()
        {
        }

        protected virtual IDecompiler CreateDecompiler(LoaderBase ldr, DecompilerHost host, IServiceProvider sp)
        {
            return new DecompilerDriver(ldr, host, sp);
        }

        protected virtual LoaderBase CreateLoader(IServiceContainer sc)
        {
            return new Loader(
                GetService<IDecompilerConfigurationService>(),
                sc);
        }


        public override bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
        {
            if (cmdSet == CmdSets.GuidDecompiler)
            {
                switch (cmdId)
                {
                case CmdIds.EditFind:
                case CmdIds.ViewGoToAddress:
                case CmdIds.ViewShowAllFragments:
                case CmdIds.ViewShowUnscanned:
                case CmdIds.ActionEditSignature:
                case CmdIds.ActionMarkProcedure:
                    status.Status = 0;
                    return true;
                }
            }
            return base.QueryStatus(ref cmdSet, cmdId, status, text);
        }

        public void EnableControls()
        {
            browserSvc.Enabled = false;
        }

        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;
                if (value != null)
                {
                    browserSvc = GetService<IProgramImageBrowserService>();
                }
                else
                {
                    browserSvc = null;
                }
            }
        }

        public override void EnterPage()
        {
            EnableControls();
        }

        public override bool LeavePage()
        {
            if (Decompiler == null)
                return false;

            IWorkerDialogService svc = GetService<IWorkerDialogService>();
            return svc.StartBackgroundWork("Scanning source program.", Decompiler.ScanProgram);
        }

        public void OpenBinary(string file, DecompilerHost host)
        {
            var sc = GetService<IServiceContainer>();
            LoaderBase ldr = CreateLoader(sc);
            Decompiler = CreateDecompiler(ldr, host, sc);
            IWorkerDialogService svc = EnsureService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", delegate()
            {
                Decompiler.LoadProgram(file);
            });
            var memSvc = EnsureService<IMemoryViewService>();
            memSvc.ViewImage(Decompiler.Program.Image);
        }


    }
}