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

using System.ComponentModel;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using System;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
	/// <summary>
	/// Handles interactions on InitialPage
	/// </summary>
	public class InitialPageInteractor : PhasePageInteractor
	{
		private IStartPage page;
        private IProgramImageBrowserService browserSvc;

		public InitialPageInteractor(IStartPage page)
		{
			this.page = page;
			page.BrowseInputFile.Click += new EventHandler(BrowseInputFile_Click);
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