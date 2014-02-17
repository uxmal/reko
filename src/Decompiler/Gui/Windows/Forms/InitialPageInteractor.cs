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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Core.Configuration;
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
        void OpenBinaryAs(string file, IProcessorArchitecture arch, Platform platform, Address addrBase, DecompilerHost host);
    }

    /// <summary>
    /// Handles interactions on InitialPage.
    /// </summary>
    public class InitialPageInteractorImpl : PhasePageInteractorImpl, InitialPageInteractor
    {
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
                Site.GetService<IDecompilerConfigurationService>(),
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
                    browserSvc = Site.RequireService<IProgramImageBrowserService>();
                }
                else
                {
                    browserSvc = null;
                }
            }
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
            var browserSvc = Site.RequireService<IProgramImageBrowserService>();
            browserSvc.SelectionChanged -= BrowserItemSelected;
            return (Decompiler != null);
        }

        public void OpenBinary(string file, DecompilerHost host)
        {
            var sc = Site.RequireService<IServiceContainer>();
            var ldr = CreateLoader(sc);
            this.Decompiler = CreateDecompiler(ldr, host, sc);
            IWorkerDialogService svc = Site.RequireService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", delegate()
            {
                Decompiler.LoadProgram(file);
            });
            if (Decompiler.Program != null)
            {
                var browserSvc = Site.RequireService<IProjectBrowserService>();
                browserSvc.Load(Decompiler.Project);
                var memSvc = Site.RequireService<IMemoryViewService>();
                memSvc.ViewImage(Decompiler.Program);
                PopulateBrowserServiceWithSegments();
            }
        }

        public void OpenBinaryAs(
            string file, 
            IProcessorArchitecture arch,
            Platform platform, 
            Address addrBase, 
            DecompilerHost host)
        {
            var sc = Site.RequireService<IServiceContainer>();
            var ldr = CreateLoader(sc);
            this.Decompiler = CreateDecompiler(ldr, host, sc);
            IWorkerDialogService svc = Site.RequireService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", delegate()
            {
                Decompiler.LoadRawImage(file, arch, platform, addrBase);
                svc.SetCaption("Scanning source program.");
                Decompiler.ScanProgram();
            });
            if (Decompiler.Program != null)
            {
                var browserSvc = Site.RequireService<IProjectBrowserService>();
                browserSvc.Load(Decompiler.Project);
                var memSvc = Site.RequireService<IMemoryViewService>();
                memSvc.ViewImage(Decompiler.Program);
                PopulateBrowserServiceWithSegments();
            }
        }

        [Obsolete]
        private void PopulateBrowserServiceWithSegments()
        {
            var oldBrowserSvc = Site.RequireService<IProgramImageBrowserService>();
            oldBrowserSvc.Populate(Decompiler.Program.ImageMap.Segments.Values, delegate(object item, IListViewItem listItem)
            {
                var ims = (ImageMapSegment)item;
                listItem.Text = ims.Name;
                listItem.AddSubItem(ims.Address.ToString());
            });
            oldBrowserSvc.Enabled = true;
            oldBrowserSvc.Caption = "Segments";
            oldBrowserSvc.AddColumn("Address");
            oldBrowserSvc.SelectionChanged += BrowserItemSelected;
        }

        public void BrowserItemSelected(object sender, EventArgs e)
        {
            ImageMapSegment segment = (ImageMapSegment)browserSvc.FocusedItem;
            var memSvc = Site.RequireService<IMemoryViewService>();
            memSvc.ShowMemoryAtAddress(segment.Address);
            memSvc.ShowWindow();
        }
    }
}