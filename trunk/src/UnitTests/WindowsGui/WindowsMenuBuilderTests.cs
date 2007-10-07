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
using Decompiler.WindowsGui;
using NUnit.Framework;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Decompiler.UnitTests.WindowsGui
{
	[TestFixture]
	public class WindowsMenuBuilderTests
	{
		[Test]
		public void CreateMainMenu()
		{
			MainMenu menu = new MainMenu();
			WindowsMenuBuilder bldr = new WindowsMenuBuilder(menu, new MenuCommandHandler(test_Click));
			bldr.BeginSubMenu();
			bldr.AddMenuItem("&File", new CommandID(CommandGroup.File, CmdID.File));
			bldr.EndSubMenu();

			Assert.AreEqual(1, menu.MenuItems.Count);
			MenuItem mi = menu.MenuItems[0];
			Assert.AreEqual("&File", mi.Text);
		}

		public void test_Click(object sender, MenuCommandArgs args)
		{
		}
	}
}
