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

using Reko.Core.Diagnostics;
using Reko.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    /// <summary>
    /// Base class for obtaining menus for this application. 
    /// </summary>
    /// <remarks>
    /// Menus are defined in <see cref="Reko.Gui.CommandDefinitions"/>.
    /// View Models suitable for binding to an Avalonia &lt;Menu&gt; control
    /// in this class.
    /// </remarks>
    public abstract class MenuSystem
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(MenuSystem), "Trace the MenuSystem class");


        public MenuSystem(ICommandTarget target)
        {
            this.Target = target;
            this.KeyBindings = new Dictionary<string, Dictionary<(string,KeyModifiers), CommandID>>();
        }

        public Dictionary<string, Dictionary<(string,KeyModifiers), CommandID>> KeyBindings { get; set; }

        public ICommandTarget Target { get; }


        public void AddBinding(string windowKey, Guid cmdSet, int id, string key, KeyModifiers modifiers)
        {
            if (!KeyBindings.TryGetValue(windowKey, out var bindingList))
            {
                bindingList = new Dictionary<(string,KeyModifiers), CommandID>();
                KeyBindings.Add(windowKey, bindingList);
            }
            bindingList[(key,modifiers)] = new CommandID(cmdSet, id);
        }

        public void BuildMenu(SortedList<int, SortedList<int, CommandItem>> menu, IList m)
        {
            DumpMenu(menu);

            bool separator = false;
            foreach (var group in menu.Values)
            {
                if (group.Count == 0)
                    continue;
                if (separator)
                    m.Add(new CommandItem { Text = "-" });
                separator = true;
                foreach (CommandItem cmi in group.Values)
                {
                    CommandItem cmiNew = (cmi.CommandID is not null)
                        ? new CommandItem 
                          {
                            Text = cmi.Text,
                            CommandID = cmi.CommandID,
                            Command = cmi.Command 
                          }
                        : cmi;
                    cmiNew.IsDynamic = cmi.IsDynamic;
                    //cmiNew.DropDownOpening += new EventHandler(subMenu_Popup);
                    //cmiNew.Click += new CommandMenuEventHandler(item_Click);
                    m.Add(cmiNew);
                }
            }
        }

        [Conditional("DEBUG")]
        private static void DumpMenu(SortedList<int, SortedList<int, CommandItem>> menu)
        {
            if (!trace.TraceVerbose)
                return;
            trace.Verbose("Building menu from {0} groups", menu.Count);
            bool needsSep = false;
            foreach (var group in menu.Values)
            {
                if (needsSep)
                    trace.Verbose("  -----");
                foreach (var cmd in group.Values)
                {
                    trace.Verbose("  {0}", cmd.Text ?? "(null)");
                }
            }
        }

        public void BuildToolMenu(SortedSet<SortedSet<CmdDefinition>> menu, ObservableCollection<CommandItem> items)
        {
            foreach (SortedSet<CmdDefinition> group in menu)
            {
                if (group.Count == 0)
                    continue;
                //                if (separator)
                //                    m.Add(new CommandMenuItem("-"));
                foreach (CmdDefinition cmi in group)
                {
                    var btnNew = new CommandItem { Text = cmi.text };
                    btnNew.Command = null; //$BUG: command target needed.
                    btnNew.CommandID = new CommandID(cmi.cmdSet, (int)cmi.id);
#if NYI
                    if (!string.IsNullOrEmpty(cmi.imageKey))
                        btnNew.ImageKey = cmi.ImageKey;
                    else
                        btnNew.ImageIndex = cmi.ImageIndex;
                    btnNew.ToolTipText = cmi.Text;
#endif
                    items.Add(btnNew);
                }
            }
        }

        public SortedList<int, T> CreatePriorityList<T>()
        {
            return new SortedList<int, T>(new PriorityComparer());
        }

        public abstract ObservableCollection<CommandItem> GetContextMenu(int menuId);

        public abstract ObservableCollection<CommandItem> GetMenu(int menuId);

        public abstract ObservableCollection<CommandItem> GetToolStrip(int menuId);

        public static void SetItemVisibility(CommandItem item, CommandStatus cmdStatus)
        {
            item.IsVisible = (cmdStatus.Status & MenuStatus.Visible) != 0;
            item.IsEnabled = (cmdStatus.Status & MenuStatus.Enabled) != 0;
            item.IsChecked = (cmdStatus.Status & MenuStatus.Checked) != 0;
        }

        public int SetStatusForMenuItems(IList<CommandItem> menuItems)
        {
            var ms = new MenuStatusSetter();
            return ms.SetStatus(new CommandItemAdapter(menuItems), Target);
        }

        public int SetStatusForToolStripItems(IList<CommandItem> items)
        {
            var ms = new MenuStatusSetter();
            return ms.SetStatus(new CommandItemAdapter(items), Target);
        }

        /// <summary>
        /// Comparer is used to fold items into order according to priority.
        /// </summary>
        public class PriorityComparer : IComparer<int>
        {
            public int Compare(int a, int b)
            {
                if (a <= b)
                    return -1;
                else
                    return 1;
            }
        }

        //protected void subMenu_Popup(object sender, EventArgs e)
        //{
        //    SetStatusForMenuItems(((ToolStripDropDownItem) sender).DropDownItems);
        //}

        //protected void ctxMenu_Popup(object sender, EventArgs e)
        //{
        //    SetStatusForMenuItems(((ContextMenuStrip) sender).Items);
        //}

        //private void item_Click(object sender, CommandMenuEventArgs e)
        //{
        //    MenuCommand cmd = e.Item.MenuCommand;
        //    if (cmd is not null)
        //    {
        //        target.Execute(cmd.CommandID);
        //    }
        //}

        public ValueTask<bool> ProcessKey(string controlType, ICommandTarget ct, string key, KeyModifiers modifiers)
        {
            if (this.KeyBindings.TryGetValue(controlType, out var bindings))
            {
                if (ct is not null)
                {
                    if (KeyBindings.TryGetValue(ct.GetType().FullName!, out bindings))
                    {
                        if (bindings.TryGetValue((key, modifiers), out CommandID? cmdID))
                        {
                            return ct.ExecuteAsync(cmdID);
                        }
                    }
                    return ValueTask.FromResult(false);
                }
            }

            if (KeyBindings.TryGetValue("", out bindings))
            {
                if (bindings.TryGetValue((key, modifiers), out CommandID? cmdID))
                {
                    return this.Target.ExecuteAsync(cmdID);
                }
            }
            return ValueTask.FromResult(false);
        }
    }
}
