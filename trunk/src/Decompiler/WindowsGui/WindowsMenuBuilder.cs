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
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Decompiler.WindowsGui
{
	/// <summary>
	/// Builds a Windows menu.
	/// </summary>
	public class WindowsMenuBuilder : MenuBuilder
	{
		private Menu menu;
		private Stack incompleteMenus;
		private ArrayList items;
		private MenuCommandHandler clickHandler;

		public WindowsMenuBuilder(Menu menu, MenuCommandHandler clickHandler)
		{
			this.Menu = menu;
			this.incompleteMenus = new Stack();
			this.clickHandler = clickHandler;
		}

		public Menu Menu
		{
			get { return menu; }
			set { menu = value; }
		}

		public override void AddMenuItem(string text, CommandID cmd)
		{
			WindowsMenuItem mi = new WindowsMenuItem();
			mi.Text = text;
			mi.CommandID = cmd;
			mi.Index = items.Count;
			mi.Click += clickHandler;
		}

		public override void AddSeparator()
		{

		}

		public override void BeginSubMenu()
		{
			items = new ArrayList();
			incompleteMenus.Push(items);
		}

		public override void EndSubMenu()
		{
			
		}

	}

	public class WindowsMenuItem : MenuItem
	{
		public event MenuCommandHandler Click;

		private MenuCommand cmd;

		public WindowsMenuItem()
		{
		}

		protected override void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, new MenuCommandArgs(cmd));
		}

		public CommandID CommandID
		{ 
			get { return cmd.CommandID; }
			set { cmd = new MenuCommand(null, value); }
		}
	}
}
