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
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public interface IFinalPageInteractor : IPhasePageInteractor
    {
    }

    public class FinalPageInteractor : PhasePageInteractorImpl, IFinalPageInteractor
    {
        IProgramImageBrowserService browserService;

        public FinalPageInteractor()
        {
        }

        void DataTypeDefinitionLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowExplorerWindow(Decompiler.Project.InputFiles[0].TypesFilename);
        }

        void ProgramCodeLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowExplorerWindow(Decompiler.Project.InputFiles[0].OutputFilename);
        }

        private void ShowExplorerWindow(string filePath)
        {
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
        }


        public void ConnectToBrowserService()
        {
            browserService = Site.GetService<IProgramImageBrowserService>();
            browserService.Enabled = true;
            browserService.SelectionChanged += browserService_SelectionChanged;
            browserService.Caption = "Procedures";
            browserService.SelectionChanged += new EventHandler(browserService_SelectionChanged);
        }

        public void DisconnectFromBrowserService()
        {
            browserService.SelectionChanged -= browserService_SelectionChanged;
        }

        public override void PerformWork(IWorkerDialogService workerDialogSvc)
        {
            try
            {
                workerDialogSvc.SetCaption("Reconstructing datatypes.");
                Decompiler.ReconstructTypes();
                workerDialogSvc.SetCaption("Structuring program.");
                Decompiler.StructureProgram();
            }
            catch (Exception ex)
            {
                //$REVIEW: need a new exception type which when thrown contains the activity we were doing.
                workerDialogSvc.ShowError("An error occurred while reconstructing types.", ex);
            }
        }

        public override void EnterPage()
        {
            ConnectToBrowserService();
            PopulateBrowserService();
        }

        private void PopulateBrowserService()
        {
            browserService.Populate(Decompiler.Program.Procedures.Values, delegate(object o, IListViewItem item)
            {
                item.Text = o.ToString();
            });
        }


        public override bool LeavePage()
        {
            DisconnectFromBrowserService();
            return true;
        }

        void browserService_SelectionChanged(object sender, EventArgs e)
        {
            var proc = (Procedure) browserService.SelectedItem;
            var codeSvc = Site.RequireService<ICodeViewerService>();
            codeSvc.DisplayProcedure(proc);
        }
    }
}
