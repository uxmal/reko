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
using Reko.Gui;
using Reko.Gui.ViewModels.Dialogs;
using Reko.Gui.ViewModels.Documents;
using Reko.Scanning;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Dialogs
{
    public partial class SearchAreaDialog : Window, IDialog<List<SearchArea>?>
    {
        public SearchAreaDialog()
        {
            InitializeComponent();
        }

        public SearchAreaViewModel? ViewModel => (SearchAreaViewModel?) DataContext;


        public string? Text {
            get { return Title; }
            set { Title = value; }
        }

        public List<SearchArea>? Value
        {
            get; set;
        }

        public void Dispose() { }

        private void segmentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel?.ChangeAreas(segmentList.SelectedItems);
        }

        private void segmentList_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox) sender;
            var itemVm = (SegmentListItemViewModel) checkBox.DataContext!;
            itemVm.IsSelected = checkBox.IsChecked ?? false;
            ViewModel?.ChangeAreas(segmentList.ItemsSource
                .OfType<SegmentListItemViewModel>().Where(s => s.IsSelected));
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Value = ViewModel!.Areas;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Value = null;
            this.Close();
        }
    }
}
