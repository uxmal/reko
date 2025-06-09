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
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    /// <summary>
    /// Generator class that takes platform-agnostic <see cref="CommandDefinitions"/>
    /// and generates Windows Forms specific controls.
    /// </summary>
    public class DecompilerMenus : MenuSystem
    {
        private readonly Dictionary<int, MenuStrip> menusById = new();
        private readonly Dictionary<int, ContextMenuStrip> ctxMenusById = new();
        private readonly Dictionary<int, ToolStrip> toolstripsById = new();

        public DecompilerMenus(CommandDefinitions commandDefs, ICommandTarget target) : base(target)
        {
            var menuSorters = new Dictionary<int, SortedList>();
            foreach (var menuDef in commandDefs.Menus)
            {
                menuSorters.Add(menuDef.id, CreatePriorityList());
            }

            // Create groups

            var groupSorters = new Dictionary<int, SortedList>();
            foreach (var groupDef in commandDefs.Groups)
            {
                var grSorter = CreatePriorityList();
                groupSorters.Add(groupDef.id, grSorter);
                if (groupDef.container != 0)
                {
                    menuSorters[groupDef.container].Add(groupDef.priority, grSorter);
                }
            }

            // Create commands

            var cmdsById = new Dictionary<int, CommandMenuItem>();
            foreach (var cmdDef in commandDefs.Commands)
            {
                var cmi = new CommandMenuItem(cmdDef.text, cmdDef.cmdSet, (int)cmdDef.id);
                cmi.IsDynamic = cmdDef.dynamicItemId != 0;
                cmi.ImageIndex = cmdDef.image;
                if (cmdDef.imageKey is not null) cmi.ImageKey = cmdDef.imageKey;
                if (cmdDef.tip is not null) cmi.ToolTipText = cmdDef.tip;
                cmdsById.Add((int)cmdDef.id, cmi);
                if (cmdDef.container != 0)
                {
                    groupSorters[cmdDef.container].Add(cmdDef.priority, cmi);
                }
            }

            // Create submenus

            var subMenuItems = new Dictionary<int, CommandMenuItem>();
            foreach (var menuDef in commandDefs.Menus)
            {
                if (menuDef.container != 0)
                {
                    var mi = new CommandMenuItem(menuDef.Text);
                    groupSorters[menuDef.container].Add(menuDef.priority, mi);
                    subMenuItems.Add(menuDef.id, mi);
                }
            }

            // Place commands.
            foreach (var placement in commandDefs.Placements)
            {
                if (groupSorters.ContainsKey(placement.container))
                {
                    groupSorters[placement.container].Add(placement.priority, cmdsById[placement.item]);
                }
                else if (menuSorters.ContainsKey(placement.container))
                {
                    menuSorters[placement.container].Add(placement.priority, groupSorters[placement.item]);
                }
            }

            // Build accelerators.
            foreach (var binding in commandDefs.KeyBindings)
            {
                AddBinding(
                  binding.editor,
                  binding.cmdSet,
                  binding.id,
                  (int) Keys.Parse<Keys>(binding.key1),
                  (int) ModifiersToWindowsForms(binding.alt1));
            }

            // Make controls.
            foreach (var menuDef in commandDefs.Menus)
            {
                switch (menuDef.type)
                {
                case MenuType.Submenu:
                    BuildMenu(menuSorters[menuDef.id], subMenuItems[menuDef.id].DropDownItems);
                    break;
                case MenuType.ContextMenu:
                    {
                        var ctxMenu = new ContextMenuStrip();
                        ctxMenusById[menuDef.id] = ctxMenu;
                        BuildMenu(menuSorters[menuDef.id], ctxMenu.Items);
                        ctxMenu.Opening += ctxMenu_Popup;
                        break;
                    }
                case MenuType.MainMenu:
                    {
                        var menu = new MenuStrip();
                        menusById[menuDef.id] = menu;
                        BuildMenu(menuSorters[menuDef.id], menu.Items);
                        break;
                    }
                case MenuType.Toolstrip:
                    {
                        var toolstrip = new ToolStrip();
                        toolstripsById[menuDef.id] = toolstrip;
                        BuildToolMenu(menuSorters[menuDef.id], toolstrip.Items);
                        break;
                    }
                }
            }
        }

        private Keys ModifiersToWindowsForms(KeyModifiers mod)
        {
            Keys result = 0;
            if (mod.HasFlag(KeyModifiers.Shift))
                result |= Keys.Shift;
            if (mod.HasFlag(KeyModifiers.Control))
                result |= Keys.Control;
            if (mod.HasFlag(KeyModifiers.Alt))
                result |= Keys.Alt;
            return result;
        }

        public override MenuStrip GetMenu(int menuId)
        {
            if (menusById.TryGetValue(menuId, out var menu))
                return menu;
            throw new ArgumentException(string.Format("There is no menu with id {0}.", menuId));

        }

        public override ContextMenuStrip GetContextMenu(int menuId)
        {
            if (ctxMenusById.TryGetValue(menuId, out var ctxMenu))
                return ctxMenu;
            throw new ArgumentException($"There is no context menu with id {menuId}.");
        }

        public override ToolStrip GetToolStrip(int menuId)
        {
            if (toolstripsById.TryGetValue(menuId, out var toolstrip))
                return toolstrip;
            throw new ArgumentException($"There is no tool strip with id {menuId}.");
        }
    }
}
  