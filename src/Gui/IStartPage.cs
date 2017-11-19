#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Windows.Forms;

namespace Reko.Gui
{
	/// <summary>
	/// Abstracts the user interface of the initial page.
	/// </summary>
	public interface IStartPage
	{
        System.Windows.Forms.TextBox AssemblerFile { get; }
        System.Windows.Forms.Button BrowseInputFile { get; }
        System.Windows.Forms.TextBox HeaderFile { get; }
        System.Windows.Forms.TextBox InputFile { get; }
        TextBox IntermediateFile { get; }
        bool IsDirty { get; set; }
        event EventHandler IsDirtyChanged;
        System.Windows.Forms.TextBox LoadAddress { get; }
        System.Windows.Forms.TextBox SourceFile { get; }
	}
}
