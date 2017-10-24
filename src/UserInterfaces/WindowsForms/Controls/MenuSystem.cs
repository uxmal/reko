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

using Reko.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
	/// <summary>
	/// Base class for obtaining menus for this application. 
	/// </summary>
	/// <remarks>
	/// Menus are defined in an XML file called command-definitions. Menus
    /// are then generated from that file by generating C# code from the XML
    /// file via XSLT. This is done to avoid locking to a particular GUI
    /// platform.
	/// </remarks>
	public abstract class MenuSystem
	{
		private ICommandTarget target;
        private Dictionary<string, Dictionary<Keys, CommandID>> bindingLists;

		public MenuSystem(ICommandTarget target)
		{
			this.target = target;
            this.bindingLists = new Dictionary<string, Dictionary<Keys, CommandID>>();
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

        public void AddBinding(string windowKey, Guid cmdSet, int id, Keys key, Keys modifiers)
        {
            AddBinding(windowKey, cmdSet, id, key | modifiers);
        }

        public void AddBinding(string windowKey, Guid cmdSet, int id, Keys key)
        {
            Dictionary<Keys, CommandID> bindingList;
            if (!bindingLists.TryGetValue(windowKey, out bindingList))
            {
                bindingList = new Dictionary<Keys,CommandID>();
                bindingLists.Add(windowKey, bindingList);
            }
            bindingList[key] = new CommandID(cmdSet, id);
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
                    if (cmi.ImageKey != null)
                        btnNew.ImageKey = cmi.ImageKey;
                    else 
                        btnNew.ImageIndex = cmi.ImageIndex;
                    btnNew.ToolTipText = cmi.ToolTipText;
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

        public int SetStatusForMenuItems(IList menuItems)
        {
            var ms = new MenuStatusSetter(subMenu_Popup, item_Click);
            return ms.SetStatus(new MenuItemAdapter(menuItems), target);
        }

        public int SetStatusForToolStripItems(ToolStripItemCollection items)
        {
            var ms = new MenuStatusSetter(subMenu_Popup, item_Click);
            return ms.SetStatus(new ToolStripItemAdapter(items), target);
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

		protected void subMenu_Popup(object sender, EventArgs e)
		{
			SetStatusForMenuItems(((Menu) sender).MenuItems);
		}

		private void item_Click(object sender, CommandMenuEventArgs e)
		{
			MenuCommand cmd = e.Item.MenuCommand;
			if (cmd != null)
			{
				target.Execute(cmd.CommandID);
			}
		}

        public void ProcessKey(IDecompilerShellUiService uiSvc, KeyEventArgs e)
        {
            Dictionary<Keys, CommandID> bindings;
            var frame = uiSvc.ActiveFrame;
            if (frame != null)
            {
                var ct = frame.Pane as ICommandTarget;
                if (ct != null)
                {
                    if (bindingLists.TryGetValue(ct.GetType().FullName, out bindings))
                    {
                        CommandID cmdID;
                        if (bindings.TryGetValue(e.KeyData, out cmdID))
                        {
                            if (ct.Execute(cmdID))
                            {
                                e.Handled = true;
                                return;
                            }
                        }
                    }
                }
            }
            if (bindingLists.TryGetValue("", out bindings))
            {
                CommandID cmdID;
                if (bindings.TryGetValue(e.KeyData, out cmdID))
                {
                    if (this.target.Execute(cmdID))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            e.Handled = false;
        }
	}
}
