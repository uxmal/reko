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

using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Gui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class EditProjectInteractor
    {
        private EditProjectDialog dlg;
        private IDecompilerUIService uiSvc;

        /// <summary>
        /// Shows UI and allows the user to edit the project properties.
        /// </summary>
        /// <param name="project"></param>
        public bool EditProjectProperties(
            IDecompilerShellUiService uiSvc,
            Project_v3 project,
            Action<Project_v3> updater)
        {
            using (var dlg = new EditProjectDialog())
            {
                Attach(dlg);
                LoadFieldsFromProject(project);
                if (uiSvc.ShowModalDialog(dlg) == Gui.DialogResult.OK)
                {
                    updater(CreateProjectFromFields());
                    return true;
                }
            }
            return false;
        }

        public void Attach(EditProjectDialog dlg)
        {
            this.dlg = dlg;
            uiSvc = CreateUIService(dlg, dlg.OpenFileDialog, dlg.SaveFileDialog);

            dlg.BrowseBinaryFileButton.Click += new EventHandler(BrowseBinaryFileButton_Click);
            dlg.BrowseAssemblerFileButton.Click += new EventHandler(BrowseAssemblerFileButton_Click);
            dlg.BrowseIntermediateFileButton.Click += new EventHandler(BrowseIntermediateFileButton_Click);
            dlg.BrowseTypesFileButton.Click += new EventHandler(BrowseTypesFileButton_Click);
            dlg.BrowseOutputFileButton.Click += new EventHandler(BrowseOutputFileButton_Click);
        }

        protected virtual IDecompilerUIService CreateUIService(Form dlg, OpenFileDialog ofd, SaveFileDialog sfd)
        {
            return new DecompilerUiService(dlg, ofd, sfd);
        }

        protected void BrowseBinaryFileButton_Click(object sender, EventArgs e)
        {
            TextBox txt = dlg.BinaryFilename;
            EditInputFile(txt);
        }

        protected void BrowseAssemblerFileButton_Click(object sender, EventArgs e)
        {
            EditOutputFile(dlg.Disassembly);
        }

        protected void BrowseIntermediateFileButton_Click(object sender, EventArgs e)
        {
            EditOutputFile(dlg.IntermediateFilename);
        }

        protected void BrowseTypesFileButton_Click(object sender, EventArgs e)
        {
            EditOutputFile(dlg.TypesFilename);
        }

        protected void BrowseOutputFileButton_Click(object sender, EventArgs e)
        {
            EditOutputFile(dlg.OutputFilename);
        }

        private void EditInputFile(TextBox txt)
        {
            string fileName = uiSvc.ShowOpenFileDialog(txt.Text);
            if (fileName != null)
            {
                txt.Text = fileName;
            }

        }


        private void EditOutputFile(TextBox txt)
        {
            string fileName = uiSvc.ShowSaveFileDialog(txt.Text);
            if (fileName != null)
            {
                txt.Text = fileName;
            }
        }

        private Project_v3 CreateProjectFromFields()
        {
            Project_v3 project = new Project_v3
            {
                Inputs =
                {
                    new DecompilerInput_v3
                    {
                        Filename = dlg.BinaryFilename.Text,
                        DisassemblyFilename = dlg.Disassembly.Text,
                        IntermediateFilename = dlg.IntermediateFilename.Text,
                        TypesFilename = dlg.TypesFilename.Text,
                        OutputFilename = dlg.OutputFilename.Text,
                        //project.Output.GloblalsFilename = dlg.OutputFilename.Text;
                    }
                }
            };
            return project;
        }

        private void LoadFieldsFromProject(Project_v3 project)
        {
            if (project == null || project.Inputs == null || project.Inputs.Count == 0)
                return;
            var input = project.Inputs[0] as DecompilerInput_v3;
            if (input == null)
                return;
            dlg.BinaryFilename.Text =       input.Filename;
            dlg.Disassembly.Text =          input.DisassemblyFilename;
            dlg.IntermediateFilename.Text = input.IntermediateFilename;
            dlg.TypesFilename.Text =        input.TypesFilename;
            dlg.OutputFilename.Text =       input.OutputFilename;
            // dlg.OutputFilename.Text = project.Output.GloblalsFilename 
        }
    }
}
