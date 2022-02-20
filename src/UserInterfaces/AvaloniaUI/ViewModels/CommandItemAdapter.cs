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

using Reko.Gui;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    public class CommandItemAdapter : IMenuAdapter
    {
        private IList<CommandItem> menuItems;

        public CommandItemAdapter(IList<CommandItem> menuItems)
        {
            this.menuItems = menuItems;
        }

        public int Count => menuItems.Count;

        public CommandID? GetCommandID(int i)
        {
            return menuItems[i].CommandID;
        }

        public void RemoveAt(int i)
        {
            menuItems.RemoveAt(i);
        }

        public bool IsDynamic(int i)
        {
            var item = menuItems[i];
            return item.IsDynamic;
        }

        public bool IsTemporary(int i)
        {
            return menuItems[i].IsTemporary;
        }

        public bool IsSeparator(int i)
        {
            return menuItems[i].Text == "-";
        }

        public void InsertAt(int i, object item)
        {
            menuItems.Insert(i, (CommandItem) item);
        }

        public void SetText(int i, string text)
        {
            var item = menuItems[i];
            item.Text = text;
        }

        public void SetStatus(int i, MenuStatus s)
        {
            var item = menuItems[i];
            item.IsVisible = (s & MenuStatus.Visible) != 0;
            item.IsEnabled = (s & MenuStatus.Enabled) != 0;
            item.IsChecked = (s & MenuStatus.Checked) != 0;
        }
    }
}
