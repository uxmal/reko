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

using Decompiler.Gui;
using System;

namespace Decompiler.WindowsGui.Forms
{
	/// <summary>
	/// Base class for all interactors in charge of phase pages. Provides common functionality
	/// such as command routing.
	/// </summary>
	public abstract class PhasePageInteractor : ICommandTarget
	{
		private PhasePage page;
		private PhasePageInteractor nextPage;
		private MainForm form;
		private DecompilerDriver decompiler;

		public PhasePageInteractor(PhasePage page, MainForm form)
		{
			this.page = page; 
			this.form = form;
		}

		public DecompilerDriver Decompiler
		{
			get { return decompiler; }
			set { decompiler = value; }
		}

		public MainForm MainForm
		{
			get { return form; }
		}

		public PhasePageInteractor NextPage
		{
			get { return nextPage; }
			set { nextPage = value; }
		}

		/// <summary>
		/// Displays and populates all the controls for this phase page.
		/// </summary>
		public abstract void PopulateControls();

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
	}
}
