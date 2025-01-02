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

using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;
using Dock.Model.ReactiveUI.Controls;
using System.Collections.Generic;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents
{
    //$TODO: this is a placeholder class for development use only. It will
    // be removed in the future.
    public class DocumentViewModel : Document
    {
        public DocumentViewModel()
        {
            this.Procedures = new VirtualList<ProcedureViewModel>(
                i => new ProcedureViewModel(i),
                1000);
        }

        public string? Content { get; set; }

        public IList<ProcedureViewModel> Procedures { get; set; }

    }
}
