/* 
 * Copyright (C) 1999-2007 John Källén.
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

namespace Decompiler.WindowsGui.Forms
{
	/// <summary>
	/// Handles interaction with the MainForm, in order to decouple platform-
	/// specific code from the user interaction code. This will make it easier to port
	/// to other GUI platforms.
	/// </summary>
	public class MainFormInteractor
	{
		private MainForm form;			//$REVIEW: in the future, this should be an interface.
		private DecompilerDriver decompiler;
		private DecompilerPhase initialPhase;
		private DecompilerPhase phase;
		
		public MainFormInteractor(MainForm form, DecompilerPhase initialPhase)
		{
			this.form = form;
			this.initialPhase = initialPhase;
			this.phase = initialPhase;
		}

		public void OpenBinary(string file, DecompilerHost host)
		{
			decompiler = new DecompilerDriver(file, host);
			try
			{
				decompiler.LoadProgram();
				form.ShowPhasePage(initialPhase.Page, decompiler);
			} 	
			catch (Exception e)
			{
				form.AddDiagnostic(Diagnostic.FatalError, "Fatal error: {0}", e.Message);
				form.SetStatus("Terminated due to fatal error.");
			}
		}

		public void BrowserItemSelected(object item)
		{
			phase.Page.BrowserItemSelected(item);
		}

		public void FinishDecompilation()
		{
			phase.Execute(decompiler);
			while (phase.NextPhase != null)
			{
				phase = phase.NextPhase;
				phase.Execute(decompiler);
			}
			form.ShowPhasePage(phase.Page, decompiler);

		}

		public void NextPhase()
		{
			phase.Execute(decompiler);
			if (phase.NextPhase != null)
			{
				phase = phase.NextPhase;
				form.ShowPhasePage(phase.Page, decompiler);
			}
		}
	}
}
