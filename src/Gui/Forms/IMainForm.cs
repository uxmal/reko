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

using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Reko.Gui.Forms
{
	/// <summary>
	/// Interface that abstracts the functionality of the main form.
	/// </summary>
	public interface IMainForm : IDisposable
	{
		/// <summary>
		/// The text of the window's title bar.
		/// </summary>
		string TitleText { get; set; }
        System.Drawing.Size Size { get; set; }
        FormWindowState WindowState { get; set; }

        TabPage FindResultsPage { get; }
        TabPage DiagnosticsPage { get; }

        event EventHandler Closed;
        event EventHandler Load;

        void LayoutMdi(DocumentWindowLayout layout);
        
        void Show();

        void Close();

        object Invoke(Delegate action, params object [] args);

        void UpdateToolbarState();
    }

    public enum DocumentWindowLayout
    {
        None,
        TiledHorizontal,
        TiledVertical,
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
