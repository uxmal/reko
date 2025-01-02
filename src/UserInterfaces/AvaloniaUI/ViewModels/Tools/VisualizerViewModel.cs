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

using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Gui;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools
{
    public class VisualizerViewModel : Tool
    {
        private readonly ISelectedAddressService selAddrSvc;
        private readonly ISelectionService selSvc;
        private Program? program;

        public VisualizerViewModel(ISelectedAddressService selAddrSvc, ISelectionService selSvc)
        {
            this.selAddrSvc = selAddrSvc;
            this.selSvc = selSvc;
            this.selAddrSvc.SelectedAddressChanged += SelAddrSvc_SelectedAddressChanged;
            this.selAddrSvc.SelectedProcedureChanged += SelAddrSvc_SelectedProcedureChanged;
            this.selAddrSvc.SelectedProgramChanged += SelAddrSvc_SelectedProgramChanged;
            this.selSvc.SelectionChanged += SelSvc_SelectionChanged;
            this.selectedVisualizer = Visualizers[0];
        }


        public List<ListOption> Visualizers { get; } = new List<ListOption>()
        {
            new ListOption("V1", "vis-1"),
            new ListOption("V2", "vis-2"),
            new ListOption("V3", "vis-3"),
        };


        public Address? Address
        {
            get => this.address;
            set => this.RaiseAndSetIfChanged(ref this.address, value);
        }
        private Address? address;

        public MemoryArea? MemoryArea
        {
            get => this.memoryArea;
            set => this.RaiseAndSetIfChanged(ref this.memoryArea, value);
        }
        private MemoryArea? memoryArea;

        public ListOption SelectedVisualizer
        {
            get => selectedVisualizer;
            set { this.RaiseAndSetIfChanged(ref selectedVisualizer, value); }
        }
        private ListOption selectedVisualizer;

        private void SelAddrSvc_SelectedProgramChanged(object? sender, EventArgs e)
        {
            var program = this.selAddrSvc.SelectedProgram;
            if (program is null)
            {
                this.MemoryArea = null;
                this.Address = null;
            }
            else
            {
                if (!program.SegmentMap.TryFindSegment(program.SegmentMap.BaseAddress, out var segment))
                    return;
                this.MemoryArea = segment.MemoryArea;
                this.Address = segment.MemoryArea.BaseAddress;
            }
        }

        private void SelAddrSvc_SelectedProcedureChanged(object? sender, EventArgs e)
        {
            var proc = this.selAddrSvc.SelectedProcedure;
            this.program = this.selAddrSvc.SelectedProgram;
            if (proc is null || program is null || proc.EntryAddress == this.Address)
                return;
            Debug.Assert(program is not null);
            if (!program.SegmentMap.TryFindSegment(proc.EntryAddress, out var segment))
                return;
            this.MemoryArea = segment.MemoryArea;
            this.Address = proc.EntryAddress;
        }

        private void SelAddrSvc_SelectedAddressChanged(object? sender, EventArgs e)
        {
            var addrRange = this.selAddrSvc.SelectedAddressRange;
            this.program = this.selAddrSvc.SelectedProgram;
            if (addrRange is null || program is null || addrRange.Address == this.Address)
                return;
            if (!program.SegmentMap.TryFindSegment(addrRange.Address, out var segment))
                return;
            this.MemoryArea = segment.MemoryArea;
            this.Address = addrRange.Address;
        }

        //$TODO: this should happen in VisualizerViewModel.
        private void SelSvc_SelectionChanged(object? sender, EventArgs e)
        {
            if (program is null)
                return;
            var ar = selSvc.GetSelectedComponents()
               .OfType<AddressRange>()
               .FirstOrDefault();
            if (ar is null)
                return;
            if (!program.SegmentMap.TryFindSegment(ar.Begin, out var seg))
            {
                return;
            }
            this.MemoryArea = seg.MemoryArea;
        }
    }
}
