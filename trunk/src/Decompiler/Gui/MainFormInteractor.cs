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

using System;
using System.ComponentModel.Design;

namespace Decompiler.Gui
{
	/// <summary>
	/// GUI-neutral class that handles abstract command events, and dispatches UI requests to the main form or window
	/// of the Decompiler UI.
	/// </summary>
	//$REVIEW: when porting to a new platform, move all platform-independent code here.
	public class MainFormInteractorQ : ICommandTarget
	{
		private IMainForm mainForm;

		public MainFormInteractorQ(IMainForm mainForm, string [] arguments)
		{
			this.mainForm = mainForm;
		}

		public IMainForm MainForm
		{
			get { return mainForm; }
		}

		public virtual bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
		{
			return false;
		}

		public virtual bool Execute(ref Guid cmdSet, int cmdId)
		{
			return false;
		}
	}
}
