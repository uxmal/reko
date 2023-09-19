#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Dock.Avalonia;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;

namespace Reko.UserInterfaces.AvaloniaUI.Views
{
    /// <summary>
    /// The main view of the application, displaying the menu, the toolbar, 
    /// the dockable windows, and the status bar.
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            InitializeThemes();
            InitializeMenu();
        }

        private void InitializeThemes()
        {
            /*
            var themes = this.Find<ComboBox>("Themes");

            themes.SelectionChanged += (_, _) =>
            {
                Application.Current!.Styles[0] = themes.SelectedIndex switch
                {
                    0 => App.FluentLight,
                    1 => App.FluentDark,
                    2 => App.DefaultLight,
                    3 => App.DefaultDark,
                    _ => throw new Exception("Not support theme.")
                };
            };
            */
        }

        private void InitializeMenu()
        {
            mainMenu.AddHandler(MenuItem.SubmenuOpenedEvent, (_, e) =>
            {
                if (e.Source is not MenuItem mi)
                    return;
                if (mi.DataContext is not CommandItem item)
                    return;
                var n = mi.DataContext?.GetType()?.Name;
                if (this.DataContext is MainViewModel vm)
                {
                    vm.SetMenuStatus(item.Items);
                };
            });

        }
    }
}
