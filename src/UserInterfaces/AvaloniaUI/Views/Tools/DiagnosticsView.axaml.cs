#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;
using System.Diagnostics;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Tools
{
    public partial class DiagnosticsView : UserControl
    {
        public DiagnosticsView()
        {
            InitializeComponent();
            this.diagnosticsView.AddHandler(DataGrid.GotFocusEvent, diagnosticsView_GotFocus);
        }

        public DiagnosticsViewModel? ViewModel => (DiagnosticsViewModel?) DataContext;

        private void diagnosticsView_GotFocus(object? sender, RoutedEventArgs e)
        {
            ViewModel?.OnGotFocus();
        }
    }
}
