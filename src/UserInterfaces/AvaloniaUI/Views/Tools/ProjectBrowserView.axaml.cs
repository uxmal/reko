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
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Tools
{
    public class ProjectBrowserView : UserControl
    {
        private TreeView tree;

        public ProjectBrowserView()
        {
            InitializeComponent();
            this.tree = this.FindControl<TreeView>("projectItems");
            //$TODO: these should start working in Avalonia 11.0
            tree.AddHandler(DragDrop.DragEnterEvent, projectItems_DragEnter);
            tree.AddHandler(DragDrop.DragEnterEvent, projectItems_DragOver);
            tree.AddHandler(DragDrop.DragEnterEvent, projectItems_DragLeave);
            tree.AddHandler(DragDrop.DragEnterEvent, projectItems_Drop);
            tree.AddHandler(Control.GotFocusEvent, tree_GotFocus);
            tree.AddHandler(Control.LostFocusEvent, tree_LostFocus);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected void tree_GotFocus(object? sender, EventArgs e)
        {
            if (DataContext is ProjectBrowserViewModel viewModel)
            {
                viewModel.TreeView.Focused = true;
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
