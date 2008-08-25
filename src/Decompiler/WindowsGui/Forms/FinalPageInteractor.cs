/* 
 * Copyright (C) 1999-2008 John Källén.
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
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
	public class FinalPageInteractor : PhasePageInteractor
	{
		private FinalPage finalPage;

		public FinalPageInteractor(FinalPage page, MainFormInteractor form) :
			base(page, form)
		{
			finalPage = page;
			finalPage.SourceFile.Click += new EventHandler(BrowseSourceFile_Click);
			finalPage.HeaderFile.Click += new EventHandler(BrowseHeaderFile_Click);
			finalPage.SaveButton.Click += new EventHandler(SaveButton_Click);
		}


		public override void EnterPage()
		{
			finalPage.SourceFile.Text = Decompiler.Project.Output.OutputFilename;
			finalPage.HeaderFile.Text = Decompiler.Project.Output.TypesFilename;

			SetTextBoxes(Decompiler.Project.Output);
			Decompiler.ReconstructTypes();
			Decompiler.StructureProgram();
		}

		public override bool LeavePage()
		{
			return true;
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			try
			{
				Decompiler.Project.Output.OutputFilename = finalPage.SourceFile.Text;
				Decompiler.Project.Output.TypesFilename = finalPage.HeaderFile.Text;
				Decompiler.WriteDecompilerProducts();
			} 
			catch (Exception ex)
			{
				MessageBox.Show(finalPage, string.Format("Couldn't save decompilation results. {0}", ex.Message), "Decompiler");
			}
		}

		public void SetTextBoxes(DecompilerOutput output)
		{
			if (output.TypesFilename == null || output.TypesFilename.Length == 0)
			{
			}

		}

		private void BrowseSourceFile_Click(object sender, EventArgs e)
		{
			MainForm.OpenFileDialog.FileName = finalPage.SourceFile.Text;
			ShowModalDialog(MainForm.OpenFileDialog);
		}

		private void BrowseHeaderFile_Click(object sender, EventArgs e)
		{
			MainForm.OpenFileDialog.FileName = finalPage.HeaderFile.Text;
			ShowModalDialog(MainForm.OpenFileDialog);

		}
	}
}
