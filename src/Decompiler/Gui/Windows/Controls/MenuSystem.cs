/* 
 * Copyright (C) 1999-2009 John Källén.
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

namespace Decompiler.Gui.Windows.Controls
{
	/// <summary>
	/// Base class for obtaining menus for this application. 
	/// </summary>
	/// <remarks>
	/// Menus are defined in an XML file called command-definitions. Menus are then generated from that file by generating 
	/// C# code from the XML file via XSLT. 
	/// </remarks>
	public abstract class MenuSystem
	{
		private ICommandTarget target;
		private CommandStatus cmdStatus;
		private CommandText cmdText;

		public MenuSystem(ICommandTarget target)
		{
			this.target = target;
			this.cmdStatus = new CommandStatus();
			this.cmdText = new CommandText();
		}

		public void BuildMenu(SortedList menu, IList m)
		{
			bool separator = false;
			foreach (SortedList group in menu.Values)
			{
				if (group.Count == 0)
					continue;
				if (separator)
					m.Add(new CommandMenuItem("-"));
				separator = true;
				foreach (CommandMenuItem cmi in group.Values)
				{
					CommandMenuItem cmiNew = (cmi.MenuCommand != null)
						? new CommandMenuItem(cmi.Text, cmi.MenuCommand.CommandID.Guid, cmi.MenuCommand.CommandID.ID)
						: cmi;
					cmiNew.IsDynamic = cmi.IsDynamic;
					cmiNew.Popup += new EventHandler(subMenu_Popup);
					cmiNew.Click += new CommandMenuEventHandler(item_Click);
					m.Add(cmiNew);
				}
			}
		}

        public void BuildMenu(SortedList menu, ToolStripItemCollection items)
        {
            foreach (SortedList group in menu.Values)
            {
                if (group.Count == 0)
                    continue;
//                if (separator)
//                    m.Add(new CommandMenuItem("-"));
                foreach (CommandMenuItem cmi in group.Values)
                {
                    ToolStripButton btnNew = new ToolStripButton();
                    btnNew.Text = cmi.Text;
                    btnNew.Tag = cmi.MenuCommand;
                    btnNew.ImageIndex = cmi.ImageIndex;
                    items.Add(btnNew);
                }
            }
        }

		public SortedList CreatePriorityList()
		{
			return new SortedList(new PriorityComparer());
		}

		public abstract ContextMenu GetContextMenu(int menuId);

		public abstract Menu GetMenu(int menuId);

        public abstract ToolStrip GetToolStrip(int menuId);

		public void SetItemVisibility(CommandMenuItem item, CommandStatus cmdStatus)
		{
			item.Visible = (cmdStatus.Status & MenuStatus.Visible) != 0;
			item.Enabled = (cmdStatus.Status & MenuStatus.Enabled) != 0;
			item.Checked = (cmdStatus.Status & MenuStatus.Checked) != 0;
		}

        public void SetItemVisibility(ToolStripItem item, CommandStatus cmdStatus)
        {
            item.Visible = (cmdStatus.Status & MenuStatus.Visible) != 0;
            item.Enabled = (cmdStatus.Status & MenuStatus.Enabled) != 0;
        }

		public void SetStatusForMenuItems(IList menuItems)
		{
			for (int i = 0; i < menuItems.Count; ++i)
			{
				CommandMenuItem item = (CommandMenuItem) menuItems[i];
				MenuCommand cmd = item.MenuCommand;
				if (item.IsTemporary)
				{
					item.Popup -= new EventHandler(subMenu_Popup);
					item.Click -= new CommandMenuEventHandler(item_Click);

					menuItems.RemoveAt(i);
					--i;
				}
				else if (cmd != null)
				{
					Guid guid = cmd.CommandID.Guid;
					if (target.QueryStatus(ref guid, cmd.CommandID.ID, cmdStatus, cmdText))
					{
						if (item.IsDynamic)
						{
							item.Text = cmdText.Text;

							int cmdId = cmd.CommandID.ID;
							while (target.QueryStatus(ref guid, ++cmdId, cmdStatus, cmdText))
							{
								CommandMenuItem itemNew = new CommandMenuItem(cmdText.Text, guid, cmdId);
								itemNew.IsTemporary = true;
								itemNew.Popup += new EventHandler(subMenu_Popup);
								itemNew.Click += new CommandMenuEventHandler(item_Click);

								menuItems.Insert(++i, itemNew);
							}
						}
						else
						{
							SetItemVisibility(item, cmdStatus);
						}	
					}
				}
			}
		}

        public void SetStatusForToolStripItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem i in items)
            {
                CommandMenuItem item = (CommandMenuItem) i.Tag;
                if (item.MenuCommand != null)
                {
                    Guid guid = item.MenuCommand.CommandID.Guid;
                    if (target.QueryStatus(ref guid, item.MenuCommand.CommandID.ID, cmdStatus, cmdText))
                    {
                        SetItemVisibility(i, cmdStatus);
                    }
                }
            }
        }
		/// <summary>
		/// Comparer is used to fold items into order according to priority.
		/// </summary>
		public class PriorityComparer : IComparer
		{
			public int Compare(object a, object b)
			{
				int aa = (int) a;
				int bb = (int) b;
				if (aa <= bb)
					return -1;
				else
					return 1;
			}
		}

		private void subMenu_Popup(object sender, EventArgs e)
		{
			SetStatusForMenuItems(((MenuItem) sender).MenuItems);
		}

		private void item_Click(object sender, CommandMenuEventArgs e)
		{
			MenuCommand cmd = e.Item.MenuCommand;
			if (cmd != null)
			{
				Guid guid = cmd.CommandID.Guid;
				target.Execute(ref guid, cmd.CommandID.ID);
			}
		}
	}

}
