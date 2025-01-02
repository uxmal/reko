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

using Reko.Gui.Components;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Windows.Input;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    /// <summary>
    /// View model class for menu items, toolbar buttons, etc.
    /// </summary>
    public class CommandItem : ReactingObject
    {
        public CommandItem()
        {
            this.Items = new ObservableCollection<CommandItem>();
        }

        public string? Text
        {
            get { return text; }
            set { this.RaiseAndSetIfChanged(ref text, value); }
        }
        private string? text;

        public bool IsChecked
        {
            get { return isChecked; }
            set { this.RaiseAndSetIfChanged(ref isChecked, value); }
        }
        private bool isChecked;

        public bool IsDynamic
        {
            get { return isDynamic; }
            set { this.RaiseAndSetIfChanged(ref isDynamic, value); }
        }
        private bool isDynamic;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { this.RaiseAndSetIfChanged(ref isEnabled, value); }
        }
        private bool isEnabled;

        public bool IsTemporary
        {
            get { return isTemp; }
            set { this.RaiseAndSetIfChanged(ref isTemp, value); }
        }
        private bool isTemp;

        public bool IsVisible
        {
            get { return isVisible; }
            set { this.RaiseAndSetIfChanged(ref isVisible, value); }
        }
        private bool isVisible;

        public CommandID? CommandID { get; set; }
        public ICommand? Command { get; set; }
        public string? ImageKey { get; set; }
        public int ImageIndex { get; set; }
        public string? ToolTipText { get; set; }

        public ObservableCollection<CommandItem> Items { get; }
    }
}
