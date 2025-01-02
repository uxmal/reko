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
using Reko.Core;
using Reko.Core.Memory;
using Reko.Gui.Forms;
using Reko.Gui.ViewModels.Dialogs;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Dialogs
{
    public partial class SegmentEditorDialog : Window, ISegmentEditorDialog
    {
        public SegmentEditorDialog()
        {
            InitializeComponent();
        }

        public string? Text
        {
            get => this.Title;
            set => this.Title = value;
        }

        public SegmentEditorViewModel ViewModel
        { 
            get => (SegmentEditorViewModel)DataContext!; 
            set => this.DataContext = value;
        }

        public UserSegment? Value { get; set; }

        public UserSegment CreateUserSegment()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void LoadUserSegment(MemoryArea mem, UserSegment segment)
        {
            throw new NotImplementedException();
        }
    }
}
