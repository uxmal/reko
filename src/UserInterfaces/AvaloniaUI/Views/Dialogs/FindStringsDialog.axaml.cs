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

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Reko.Gui;
using Reko.Gui.ViewModels.Dialogs;
using Reko.Scanning;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Dialogs
{
    public partial class FindStringsDialog : Window, IDialog<StringFinderCriteria?>
    {
        public FindStringsDialog()
        {
            InitializeComponent();
        }

        public FindStringsViewModel? ViewModel => (FindStringsViewModel?) DataContext;

        public StringFinderCriteria? Value { get; private set; }

        public string? Text { get => this.Title; set => this.Title = value; }

        public void Dispose()
        {
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Value = ViewModel?.GetCriteria();
            ViewModel?.SaveMruToSettings();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Value = null;
            this.Close();
        }

        private async void btnSearchArea_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel!.SelectSearchArea();
        }
    }
}
