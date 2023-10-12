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

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Reko.Gui;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;
using System;
using System.Collections.Generic;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Tools
{
    public partial class SearchResultsToolView : UserControl
    {
        public SearchResultsToolView()
        {
            InitializeComponent();
            DataContextChanged += SearchResultsToolView_DataContextChanged;
            gridResults.AddHandler(DataGrid.GotFocusEvent, gridResults_GotFocus);
        }

        private void SearchResultsToolView_DataContextChanged(object? sender, EventArgs e)
        {
            if (DataContext is SearchResultsToolViewModel viewModel)
            {
                viewModel.SearchResultsView = CreateSearchResultView();
            }
        }

        public ISearchResultView CreateSearchResultView()
        {
            return new DataGridSearchResultView(gridResults);
        }

        private void gridResults_GotFocus(object? sender, RoutedEventArgs e)
        {
            if (DataContext is SearchResultsToolViewModel viewModel)
            {
                viewModel.OnGotFocus();
            }
        }

        private class DataGridSearchResultView : ISearchResultView
        {
            private DataGrid dataGrid;

            public DataGridSearchResultView(DataGrid dataGrid)
            {
                this.dataGrid = dataGrid;
            }

            public bool IsFocused => dataGrid.IsFocused;

            //$BUG: need to change the interface to return IEnumerable<T>
            public IEnumerable<int> SelectedIndices => (IEnumerable<int>) (object) dataGrid.SelectedItems;

            public void AddColumn(string columnTitle, int width)
            {
                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = columnTitle,
                    Width = new DataGridLength(width),
                });
            }

            public void Invalidate()
            {
                throw new NotImplementedException();
            }

            public void ShowTypeMarker(Action<string> action)
            {
                throw new NotImplementedException();
            }
        }
    }
}
