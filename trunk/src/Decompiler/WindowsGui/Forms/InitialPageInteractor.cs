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

namespace Decompiler.WindowsGui.Forms
{
	/// <summary>
	/// Handles interactions on InitialPage
	/// </summary>
	public class InitialPageInteractor : PhasePageInteractor
	{
		private InitialPage page;

		public InitialPageInteractor(InitialPage page, MainForm form)
			: base(page, form)
		{
			this.page = page;
		}

		public override void PopulateControls()
		{
			MainForm.BrowserFilter.Enabled = false;
			MainForm.BrowserList.Enabled = false;
			MainForm.BrowserTree.Enabled = false;
			page.BringToFront();
		}
	}
}
