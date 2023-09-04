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

using ReactiveUI;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Gui;
using Reko.UserInterfaces.AvaloniaUI.Views.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents
{
    public class SegmentListViewModel : ReactiveObject, IWindowPane
    {
        private readonly Program program;
        private IServiceProvider? services;

        public SegmentListViewModel(Program program)
        {
            this.program = program;
            this.Segments = new ObservableCollection<SegmentListItemViewModel>(program.SegmentMap.Segments.Values
                .Select(MakeItemViewModel));
        }

        public ObservableCollection<SegmentListItemViewModel> Segments { get; }

        public IWindowFrame? Frame { get; set; }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public object CreateControl()
        {
            var view = new SegmentListView
            {
                DataContext = this
            };
            return view;
        }

        private SegmentListItemViewModel MakeItemViewModel(ImageSegment seg)
        {
            return new SegmentListItemViewModel
            {
                Name = seg.Name,
                Address = seg.Address.ToString(),
                End = seg.EndAddress.ToString(),
                //$TODO: PDP-10 and PDP-11 prefer octal
                Length = $"0x{seg.Size:X}",
                Read = seg.Access.HasFlag(AccessMode.Read) ? "R" : "-",
                Write = seg.Access.HasFlag(AccessMode.Write) ? "W" : "-",
                Execute = seg.Access.HasFlag(AccessMode.Execute) ? "X" : "-",
            };
        }

        public void SetSite(IServiceProvider services)
        {
            this.services = services;
        }
    }

    public class SegmentListItemViewModel : ReactiveObject
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? End { get; set; }
        public string? Length { get; set; }
        public string? Read { get; set; }
        public string? Write { get; set; }
        public string? Execute { get; set; }
    }
}
