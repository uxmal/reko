using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
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
            IDecompilerUIService uiSvc,
            DecompilerProject project,
            Action<DecompilerProject> updater)
        {
            using (var dlg = new EditProjectDialog())
            {
                Attach(dlg);
                LoadFieldsFromProject(project);
                if (uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
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

        private DecompilerProject CreateProjectFromFields()
        {
            DecompilerProject project = new DecompilerProject();

            project.Input.Filename = dlg.BinaryFilename.Text;
            project.Input.Address = dlg.BaseAddress.Text;
            project.Output.DisassemblyFilename = dlg.Disassembly.Text;
            project.Output.IntermediateFilename = dlg.IntermediateFilename.Text;
            project.Output.TypesFilename = dlg.TypesFilename.Text;
            project.Output.OutputFilename = dlg.OutputFilename.Text;

            return project;
        }

        private void LoadFieldsFromProject(DecompilerProject project)
        {
            dlg.BinaryFilename.Text = project.Input.Filename;
            dlg.BaseAddress.Text = project.Input.Address;
            dlg.Disassembly.Text = project.Output.DisassemblyFilename;
            dlg.IntermediateFilename.Text = project.Output.IntermediateFilename;
            dlg.TypesFilename.Text = project.Output.TypesFilename;
            dlg.OutputFilename.Text = project.Output.OutputFilename;
        }

    }
}
