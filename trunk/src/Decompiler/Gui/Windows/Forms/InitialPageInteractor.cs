/* 
 * Copyright (C) 1999-2009 John Källén.
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

		public InitialPageInteractorImpl(IStartPage page)
		{
			this.page = page;
			page.BrowseInputFile.Click += new EventHandler(BrowseInputFile_Click);
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
                    browserSvc = (IProgramImageBrowserService) value.GetService(typeof(IProgramImageBrowserService));
                }
                else
                {
                    browserSvc = null;
                }
            }
        }

		public override void EnterPage()
		{
			if (Decompiler != null && Decompiler.Project != null)
			{
                LoadFieldsFromProject();
			}
			EnableControls();
		}

        public override bool LeavePage()
        {
            if (page.IsDirty)
            {
                SaveFieldsToProject();
            }
            return true;
        }

        public void OpenBinary(string file, DecompilerHost host)
        {
            var sc = GetService<IServiceContainer>();
            LoaderBase ldr = CreateLoader(file, sc);
            Decompiler = new DecompilerDriver(ldr, host, sc) ;
            IWorkerDialogService svc = GetService<IWorkerDialogService>();
            svc.StartBackgroundWork("Loading program", Decompiler.LoadProgram);
            LoadFieldsFromProject();
        }

        private void LoadFieldsFromProject()
        {
            DecompilerProject project = Decompiler.Project;
            page.InputFile.Text = project.Input.Filename;
            page.LoadAddress.Text = project.Input.BaseAddress.ToString();
            page.SourceFile.Text = project.Output.OutputFilename;
            page.HeaderFile.Text = project.Output.TypesFilename;
            page.IntermediateFile.Text = project.Output.IntermediateFilename;
            page.AssemblerFile.Text = project.Output.DisassemblyFilename;
        }


        private void SaveFieldsToProject()
        {
            DecompilerProject project = Decompiler.Project;
            project.Input.Filename = page.InputFile.Text;
            project.Input.BaseAddress = Address.ToAddress(page.LoadAddress.Text, 16);
            project.Output.OutputFilename = page.SourceFile.Text;
            project.Output.TypesFilename = page.HeaderFile.Text;
            project.Output.IntermediateFilename = page.IntermediateFile.Text;
            project.Output.DisassemblyFilename = page.AssemblerFile.Text;
        }

        public override object Page
        {
            get { return page; }
        }

        protected virtual LoaderBase CreateLoader(string filename, IServiceContainer sc)
        {
            return new Loader(
                filename, 
                GetService<IDecompilerConfigurationService>(),
                sc);
        }

        // Event handlers. 
		public void BrowseInputFile_Click(object sender, EventArgs e)
		{
			string sNew = UIService.ShowOpenFileDialog(page.InputFile.Text);
			if (sNew != null)
			{
				page.InputFile.Text = sNew;
			}
		}
	}
}