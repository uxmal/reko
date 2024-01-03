#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Collections;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Tools
{
    public partial class ProjectBrowserView : UserControl
    {
        private TreeView tree;

        public ProjectBrowserView()
        {
            InitializeComponent();
            this.tree = this.FindControl<TreeView>("projectItems")!;
            //$TODO: these should start working in Avalonia 11.0
            tree.AddHandler(DragDrop.DragEnterEvent, projectItems_DragEnter);
            tree.AddHandler(DragDrop.DragEnterEvent, projectItems_DragOver);
            tree.AddHandler(DragDrop.DragEnterEvent, projectItems_DragLeave);
            tree.AddHandler(DragDrop.DragEnterEvent, projectItems_Drop);
            tree.AddHandler(Control.GotFocusEvent, tree_GotFocus);
            tree.AddHandler(Control.LostFocusEvent, tree_LostFocus);
            tree.ContextMenu?.AddHandler(ContextMenu.OpenedEvent, contextMenu_Opening);
        }

        //$REFACTOR: this will need to be done for every context menu. Find a way
        // to do this in one place. Perhaps context menus' Opened event can be 
        // captured as a bubbled event in MainView?
        private void contextMenu_Opening(object? sender, RoutedEventArgs e)
        {
            if (e.Source is not ContextMenu ctxMenu)
                return;
            if (ctxMenu.ItemsSource is not IList items)
                return;

            if (this.DataContext is ProjectBrowserViewModel vm)
            {
                vm.SetMenuStatus(items);
            }
        }

        protected void tree_GotFocus(object? sender, EventArgs e)
        {
            if (DataContext is ProjectBrowserViewModel viewModel)
            {
                viewModel.TreeView.Focused = true;
                viewModel.OnGotFocus();
            }
        }

        protected void tree_LostFocus(object? sender, EventArgs e)
        {
            if (DataContext is ProjectBrowserViewModel viewModel)
            {
                viewModel.TreeView.Focused = false;
            }
        }

        protected void projectItems_DragEnter(object? sender, DragEventArgs e)
        {
        }

        protected void projectItems_DragOver(object? sender, DragEventArgs e)
        {
        }

        protected void projectItems_DragLeave(object? sender, DragEventArgs e)
        {
        }

        protected void projectItems_Drop(object? sender, DragEventArgs e)
        {
            //$TODO:
            // await interactor.OpenBinary(filenames[0]);
        }
    }
}
