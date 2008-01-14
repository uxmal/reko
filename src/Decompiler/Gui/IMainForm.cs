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
	/// Interface that abstracts the functionality of the main form.
	/// </summary>
	public interface IMainForm
	{
		/// <summary>
		/// The text of the windows's title bar.
		/// </summary>
		string TitleText { get; set; }

		/// <summary>
		/// Fired when the user picks a command.
		/// </summary>
		event MenuCommandHandler CommandInvoked;
	}

	public delegate void MenuCommandHandler(object sender, MenuCommandArgs arg);
	
	public class MenuCommandArgs : EventArgs
	{
		private MenuCommand cmd; 
		private bool isSupported;

		public MenuCommandArgs(MenuCommand cmd)
		{
			this.cmd = cmd;
		}

		public bool IsSupported
		{
			get { return isSupported; }
			set { isSupported = value; }
		}
		
		public MenuCommand MenuCommand
		{
			get { return cmd; }
		}
	}


}
