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
using Reko.Core.Loading;
using Reko.Gui.Forms;
using Reko.Gui.ViewModels.Dialogs;
using System.Collections.Generic;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Dialogs
{
    public partial class ArchiveBrowserDialog : Window, IArchiveBrowserDialog
    {
        public ArchiveBrowserDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public ICollection<ArchiveDirectoryEntry> ArchiveEntries { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Core.Loading.ArchivedFile? Value { get; private set; }

        public string? Text { get => this.Text; set => this.Text = value; }

        public void Dispose()
        {
        }

        protected void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Value = ((ArchiveBrowserViewModel?) DataContext)?.SelectedArchiveEntry.Entry as Core.Loading.ArchivedFile;
            this.Close();
        }

        protected void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Value = null;
            this.Close();
        }
    }
}
