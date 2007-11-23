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

using Decompiler.Gui;
using System;

namespace Decompiler.WindowsGui.Forms
{
	/// <summary>
	/// Base class for all interactors in charge of phase pages. Provides common functionality
	/// such as command routing.
	/// </summary>
	public class PhasePageInteractor : ICommandTarget
	{
		private PhasePage page;
		private MainFormInteractor mainInteractor;

		public PhasePageInteractor(PhasePage page, MainFormInteractor mainInteractor)
		{
			this.page = page;
			this.mainInteractor = mainInteractor;
			page.Enter += new EventHandler(OnPageEntered);
			
		}

		public MainForm MainForm
		{
			get { return mainInteractor.MainForm; }
		}

		public MainFormInteractor MainInteractor
		{
			get { return mainInteractor; }
		}

		#region ICommandTarget Members

		public virtual bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
		{
			return false;
		}

		public virtual bool Execute(ref Guid cmdSet, int cmdId)
		{
			return false;
		}

		#endregion

		// Event handlers /////////////////////////////////

		public virtual void OnPageEntered(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("Entered {0}",  sender.GetType().Name));
			mainInteractor.CommandTarget = this;
		}
	}
}
