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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.Views
{
    public partial class OpenAsDialog : Window, IDialog<LoadDetails?>
    {
        public OpenAsDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Services = default!;
        }

        public IServiceProvider Services { get; set; }
        public LoadDetails? Value { get; set; }
        private OpenAsViewModel ViewModel => (OpenAsViewModel) this.DataContext!;

        public string? Text 
        {
            get => base.Title;
            set => base.Title = value;
        }

        public void Dispose()
        {
            base.Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void btnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            var uiSvc = Services.RequireService<IDecompilerShellUiService>();
            var fname = await uiSvc.ShowOpenFileDialog(ViewModel?.FileName);
            if (fname is {})
            {
                ViewModel!.FileName = fname;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = ViewModel;
            this.Value = viewModel?.CreateLoadDetails();
            this.Close();
        }
    }
}
