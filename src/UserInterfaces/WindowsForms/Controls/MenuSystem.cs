#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Threading.Tasks;
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

		public MenuSystem(ICommandTarget target)
		{
			this.target = target;
            this.KeyBindings = new Dictionary<string, Dictionary<int, CommandID>>();
		}

        public Dictionary<string, Dictionary<int, CommandID>> KeyBindings { get; set; }



        public void AddBinding(string windowKey, Guid cmdSet, CmdIds id, int key, int modifiers)
        {
            AddBinding(windowKey, cmdSet, id, key | modifiers);
        }

        public void AddBinding(string windowKey, Guid cmdSet, CmdIds id, int key)
        {
            Dictionary<int, CommandID> bindingList;
            if (!KeyBindings.TryGetValue(windowKey, out bindingList))
            {
                bindingList = new Dictionary<int,CommandID>();
                KeyBindings.Add(windowKey, bindingList);
            }
            bindingList[key] = new CommandID(cmdSet, (int)id);
        }

        public void BuildMenu(SortedList menu, IList m)
        {
            bool separator = false;
            foreach (SortedList group in menu.Values)
            {
                if (group.Count == 0)
                    continue;
                if (separator)
                    m.Add(new ToolStripSeparator());
                separator = true;
                foreach (CommandMenuItem cmi in group.Values)
                {
                    CommandMenuItem cmiNew = (cmi.MenuCommand is not null)
                        ? new CommandMenuItem(cmi.Text, cmi.MenuCommand.CommandID.Guid, cmi.MenuCommand.CommandID.ID)
                        : cmi;
                    cmiNew.IsDynamic = cmi.IsDynamic;
                    cmiNew.DropDownOpening += new EventHandler(subMenu_Popup);
                    cmiNew.Click += new CommandMenuEventHandler(item_Click);
                    m.Add(cmiNew);
                }
            }
        }

        public void BuildToolMenu(SortedList menu, ToolStripItemCollection items)
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
                    if (!string.IsNullOrEmpty(cmi.ImageKey))
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

		public abstract ContextMenuStrip GetContextMenu(int menuId);

		public abstract MenuStrip GetMenu(int menuId);

        public abstract ToolStrip GetToolStrip(int menuId);

		public void SetItemVisibility(CommandMenuItem item, CommandStatus cmdStatus)
		{
			item.Visible = (cmdStatus.Status & MenuStatus.Visible) != 0;
			item.Enabled = (cmdStatus.Status & MenuStatus.Enabled) != 0;
			item.Checked = (cmdStatus.Status & MenuStatus.Checked) != 0;
		}

        public int SetStatusForMenuItems(IList menuItems)
        {
            var x = new ContextMenuStrip();
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
			SetStatusForMenuItems(((ToolStripDropDownItem) sender).DropDownItems);
		}

        protected void ctxMenu_Popup(object sender, EventArgs e)
        {
            SetStatusForMenuItems(((ContextMenuStrip) sender).Items);
        }

		private async void item_Click(object sender, CommandMenuEventArgs e)
		{
			MenuCommand cmd = e.Item.MenuCommand;
			if (cmd is not null)
			{
				await target.ExecuteAsync(cmd.CommandID);
			}
		}

        public ValueTask<bool> ProcessKey(string controlType, ICommandTarget ct, Keys keyData)
        {
            if (this.KeyBindings.TryGetValue(controlType, out var bindings))
            {
                if (ct is not null)
                {
                    if (KeyBindings.TryGetValue(ct.GetType().FullName, out bindings))
                    {
                        if (bindings.TryGetValue((int) keyData, out CommandID cmdID))
                        {
                            return ct.ExecuteAsync(cmdID);
                        }
                    }
                    return ValueTask.FromResult(false);
                }
            }
            
            if (KeyBindings.TryGetValue("", out bindings))
            {
                if (bindings.TryGetValue((int)keyData, out CommandID cmdID))
                {
                    return this.target.ExecuteAsync(cmdID);
                }
            }
            return ValueTask.FromResult(false);
        }
	}
}
