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

using System;
using System.ComponentModel.Design;

namespace Reko.Gui
{
	/// <summary>
	/// An ICommandTarget handles the issuing of commands. It is inspired by IOleCommandTarget.
	/// </summary>
	public interface ICommandTarget
	{
		/// <summary>
		/// Sets or rests the visibility flags of the command. 
		/// </summary>
		/// <remarks>
		/// If the command target knows about the command, but doesn't want it visible or enabled, set the 
		/// appropriate bits on <paramref>cmd</paramref> and return true; higher level command targets will
		/// then respect this selection. If the command target doesn't know what the 
		/// menu command is, return false from this method. This allows higher-level command targets to set
		/// set command statues.
		/// </remarks>
		/// <param name="cmd"></param>
		/// <param name="info">collecting parameter. If not null, asks for the text of the command item (for
		/// displaying in menus, buttons, etc).</param>
		/// <returns>false if the command is not supported, true if it is.</returns>
		bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text);

		/// <summary>
		/// Executes the specified command.
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns>false if the specified command is unknown, true otherwise.</returns>
		bool Execute(CommandID cmdId);
	}

	public class CommandStatus
	{
		public MenuStatus Status;
	}

	public class CommandText
	{
		public string Text;
	}

	[Flags]
	public enum MenuStatus
	{
		Visible = 0x0001,
		Enabled = 0x0002,
		Checked = 0x0004,
	}
}
