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
using Avalonia.Markup.Xaml;
using Reko.Core;
using Reko.Gui.Forms;
using Reko.Gui.ViewModels;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.Views
{
    public partial class ProgramPropertiesDialog : Window, IProgramPropertiesDialog
    {
        public ProgramPropertiesDialog()
        {
            InitializeComponent();
            this.Services = default!;
            this.Program = default!;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public IServiceProvider Services { get; set; }
        public Program Program { get; set; }
        public string? Text { get; set; }

        public void Dispose()
        {
        }

        private void ScanningHeuristics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = (ProgramPropertiesViewModel?) DataContext;
            if (dc is null)
                return;
            var item = ((ListBox) sender).SelectedItem;
            if (item is not Control ctrl || ctrl.DataContext is not ScanHeuristic sch)
                return;
            dc.ScanHeuristicDescription = sch.Description;
        }
    }
}
