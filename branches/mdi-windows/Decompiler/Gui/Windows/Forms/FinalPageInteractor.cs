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

using Decompiler.Core.Serialization;
using System;
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
		private FinalPage finalPage;

		public FinalPageInteractor()
		{
            // finalPage.DataTypeDefinitionLink.LinkClicked += new LinkLabelLinkClickedEventHandler(DataTypeDefinitionLink_LinkClicked);
            // finalPage.ProgramCodeLink.LinkClicked += new LinkLabelLinkClickedEventHandler(ProgramCodeLink_LinkClicked);
		}

        void DataTypeDefinitionLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowExplorerWindow(Decompiler.Project.Output.TypesFilename);
        }

        void ProgramCodeLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowExplorerWindow(Decompiler.Project.Output.OutputFilename);
        }

        private void ShowExplorerWindow(string filePath)
        {
            System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));

        }


		public override void EnterPage()
		{
            try
            {
                WorkerDialogService.StartBackgroundWork("Reconstructing datatypes.", delegate()
                {
                    Decompiler.ReconstructTypes();
                });
                WorkerDialogService.StartBackgroundWork("Structuring program.", delegate()
                {
                    Decompiler.StructureProgram();
                });
            }
            catch (Exception ex)
            {
                UIService.ShowError(ex, "An error occurred while reconstructing types.");
            }
		}

		public override bool LeavePage()
		{
			return true;
		}

	}
}
