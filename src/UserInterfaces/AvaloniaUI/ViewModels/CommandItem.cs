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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    /// <summary>
    /// View model class for menu items, toolbar buttons, etc.
    /// </summary>
    public class CommandItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        public CommandItem()
        {
            this.Items = new ObservableCollection<CommandItem>();
        }

        public string? Text
        {
            get { return text; }
            set { text = value; Notify(nameof(Text)); }
        }
        private string? text;

        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked= value; }
        }
        private bool isChecked;

        public bool IsDynamic
        {
            get { return isDynamic; }
            set { isDynamic = value; }
        }
        private bool isDynamic;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }
        private bool isEnabled;


        public bool IsTemporary
        {
            get { return isTemp; }
            set { isTemp = value; }
        }
        private bool isTemp;

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
        private bool isVisible;

        public CommandID? CommandId { get; set; }
        public string? ImageKey { get; set; }
        public int ImageIndex { get; set; }
        public string? ToolTipText { get; set; }

        public ObservableCollection<CommandItem> Items { get; }
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
