#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Microsoft.Toolkit.Mvvm.Input;
using ReactiveUI;
using Reko.Gui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    public partial class DecompilerMenus : MenuSystem
    {
        private readonly Dictionary<int, ObservableCollection<CommandItem>> menus;

        public DecompilerMenus(CommandDefinitions cmdDefs, ICommandTarget target) : base(target)
        {
            this.menus = new();

            var menusById = new SortedList<int, SortedList<int, SortedList<int, CommandItem>>>();
            foreach (var menu in cmdDefs.Menus)
            {
                menusById.Add(menu.id, CreatePriorityList<SortedList<int, CommandItem>>());
            }

            var groupsById = new SortedList<int, SortedList<int, CommandItem>>();
            foreach (var group in cmdDefs.Groups)
            {
                var gr = CreatePriorityList<CommandItem>();
                groupsById.Add(group.id, gr);
                if (group.container != 0)
                {
                    menusById[group.container].Add(group.priority, gr);
                }
            }

            // Create commands in containers.
            var cmdsById = new SortedList<int, CommandItem>();
            foreach (var command in cmdDefs.Commands)
            {
                var cmdid = new CommandID(command.cmdSet, (int)command.id);
                var cmd = new CommandItem { CommandID = cmdid, Text = command.text };
                cmdsById.Add((int)command.id, cmd);
                cmd.Command = ExecuteCommand;
                cmd.IsDynamic = command.dynamicItemId != 0;
                cmd.ImageKey = command.imageKey;
                cmd.ImageIndex = command.image;
                cmd.ToolTipText = command.text; //$REVIEW: separate tooltip text?
                if (command.container != 0)
                {
                    groupsById[command.container].Add(command.priority, cmd);
                }
            }

            // Create submenus
            var submenuCommandItems = new Dictionary<int, CommandItem>();
            foreach (var menu in cmdDefs.Menus)
            {
                if (menu.container != 0)
                {
                    var mi = new CommandItem { Text = menu.Text };
                    submenuCommandItems.Add(menu.id, mi);
                    groupsById[menu.container].Add(menu.priority, mi);
                }
            }

            // Place commands.
            foreach (var placement in cmdDefs.Placements)
            {
                if (cmdsById.TryGetValue(placement.item, out var cmd))
                {
                    groupsById[placement.container].Add(placement.priority, cmd);
                }
                else if (groupsById.TryGetValue(placement.item, out var grp))
                {
                    menusById[placement.container].Add(placement.priority, grp);
                }
            }

            // Build accelerators.
            foreach (var binding in cmdDefs.KeyBindings)
            {
                AddBinding(binding.editor, binding.cmdSet, (int)binding.id, binding.key1, binding.alt1);
            }

            foreach (var menu in cmdDefs.Menus)
            {
                if (menu.container != 0)
                {
                    BuildMenu(menusById[menu.id], submenuCommandItems[menu.id].Items);
                    base.SetStatusForMenuItems(submenuCommandItems[menu.id].Items);
                }
                else
                {
                    var itemcollection = new ObservableCollection<CommandItem>();
                    BuildMenu(menusById[menu.id], itemcollection);
                    this.menus.Add(menu.id, itemcollection);
                    base.SetStatusForMenuItems(itemcollection);
                }
            }
        }

        [ICommand]
        public async Task Execute(CommandID cmdId)
        {
            await base.Target.ExecuteAsync(cmdId);
        }

        public override ObservableCollection<CommandItem> GetMenu(int menuId)
        {
            return menus[menuId];
        }

        public override ObservableCollection<CommandItem> GetContextMenu(int menuId)
        {
            return menus[menuId];
        }

        public override ObservableCollection<CommandItem> GetToolStrip(int menuId)
        {
            return menus[menuId];
        }
    }
}
