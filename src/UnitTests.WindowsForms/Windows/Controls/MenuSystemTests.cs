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

using Reko.Gui;
using NUnit.Framework;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Reko.UserInterfaces.WindowsForms.Controls;

namespace Reko.UnitTests.Gui.Windows.Controls
{
	[TestFixture]
	public class MenuSystemTests : ICommandTarget
	{
		private Guid cmdSet;
		private ArrayList menu;
		private Hashtable mpCmdIdToStatus;

		const int mruFirst = 100;
		const int mruMax = 103;

		public MenuSystemTests()
		{
			cmdSet = new Guid("00001111-2222-3333-4444-555566667777");
		}

		[SetUp]
		public void Setup()
		{
			mpCmdIdToStatus = new Hashtable();
			menu = new ArrayList();
		}

		private CommandMenuItem Item(int i)
		{
			return (CommandMenuItem) menu[i];
		}

		[Test]
		public void SetStatusForMenuItems()
		{
			CommandMenuItem item = new CommandMenuItem("Test1", cmdSet, 1);
			menu.Add(item);
			item = new CommandMenuItem("Test2", cmdSet, 2);
			menu.Add(item);

			MenuSystem sys = new TestMenuSystem(this);
			mpCmdIdToStatus[1] = (MenuStatus.Visible|MenuStatus.Enabled);
			mpCmdIdToStatus[2] = (MenuStatus.Visible);
			sys.SetStatusForMenuItems(menu);
			Assert.AreEqual("Test1", Item(0).Text);
			Assert.AreEqual(1, Item(0).MenuCommand.CommandID.ID);
			Assert.IsTrue(Item(0).Visible);
			Assert.IsTrue(Item(0).Enabled);

			Assert.AreEqual("Test2", Item(1).Text);
			Assert.IsTrue(Item(1).Visible);
			Assert.IsFalse(Item(1).Enabled);
		}

		[Test]
		public void BuildMruList()
		{
			MenuSystem sys = new TestMenuSystem(this);
			CommandMenuItem item = new CommandMenuItem("MRU", cmdSet, mruFirst);
			item.IsDynamic = true;
			menu.Add(item);
			sys.SetStatusForMenuItems(menu);
			Assert.AreEqual(3, menu.Count);
			Assert.AreEqual("MRU 0", Item(0).Text);
			Assert.AreEqual("MRU 1", Item(1).Text);
			Assert.AreEqual("MRU 2", Item(2).Text);
			Assert.AreEqual(mruFirst, Item(0).MenuCommand.CommandID.ID);
			Assert.AreEqual(mruFirst+1, Item(1).MenuCommand.CommandID.ID);
			Assert.AreEqual(mruFirst+2, Item(2).MenuCommand.CommandID.ID);

			sys.SetStatusForMenuItems(menu);		// Drop the menu again
			Assert.AreEqual(3, menu.Count);
			Assert.AreEqual("MRU 0", Item(0).Text);
			Assert.AreEqual("MRU 1", Item(1).Text);
			Assert.AreEqual("MRU 2", Item(2).Text);
			Assert.AreEqual(mruFirst, Item(0).MenuCommand.CommandID.ID);
			Assert.AreEqual(mruFirst+1, Item(1).MenuCommand.CommandID.ID);
			Assert.AreEqual(mruFirst+2, Item(2).MenuCommand.CommandID.ID);
		}

		private void DumpMenu()
		{
			foreach (CommandMenuItem item in menu)
			{
				Console.WriteLine("{0}: {1} {2}", item.Text, item.MenuCommand.CommandID.ID, item.Visible?"Visible":""); 
			}
		}

		public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
		{
			if (mruFirst <= cmdId.ID && cmdId.ID < mruMax)
			{
				status.Status = MenuStatus.Visible|MenuStatus.Enabled;
				text.Text = string.Format("MRU {0}", cmdId.ID - mruFirst);
				return true;
			}

			if (mpCmdIdToStatus.Contains(cmdId.ID))
			{
				status.Status = (MenuStatus) mpCmdIdToStatus[cmdId.ID];
				//Console.WriteLine("Testing cmd {0}: status {1}", cmdId.ID, status.Status);
				return true;
			}
			return false;
		}

		public bool Execute(CommandID cmdId)
		{
			return false;
		}

		public class TestMenuSystem : MenuSystem
		{
			public TestMenuSystem(ICommandTarget target) : base(target)
			{
			}

			public override ContextMenu GetContextMenu(int menuId)
			{
				throw new NotImplementedException();
			}

			public override Menu GetMenu(int menuId)
			{
				throw new NotImplementedException();
			}

            public override ToolStrip GetToolStrip(int menuId)
            {
                throw new NotImplementedException();
            }
		}
	}
}
