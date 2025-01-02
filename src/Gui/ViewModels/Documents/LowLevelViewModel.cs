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

using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Gui.Reactive;
using Reko.Gui.Services;
using Reko.Gui.TextViewing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Gui.ViewModels.Documents
{
    public class LowLevelViewModel : ChangeNotifyingObject
    {
        private readonly ISelectedAddressService selAddrSvc;
        private readonly TextSpanFactory factory;
        private bool raisingSelectionChanged;   // Avoids stack overflow

        public LowLevelViewModel(IServiceProvider services, Program program, TextSpanFactory factory)
        {
            this.Services = services;
            this.selAddrSvc = services.RequireService<ISelectedAddressService>();
            this.selAddrSvc.SelectedAddressChanged += SelAddrSvc_SelectedAddressChanged;
            this.Program = program;
            this.factory = factory;
            this.SegmentMap = program.SegmentMap;
            this.ImageMap = program.ImageMap;
            this.SelectedArchitecture = program.Architecture;
            this.Architectures = BuildArchitectureViewModel();
            this.disassemblyTextModel = new EmptyEditorModel();
            UpdateSelection();
        }

        public ListOption[] Architectures { get; set; }

        public Program? Program { get; set; }

        public Address? SelectedAddress
        {
            get => addrSelected;
            set
            {
                if (this.addrSelected == value)
                    return;
                this.RaiseAndSetIfChanged(ref this.addrSelected, value);
                RaiseSelectionChanged();
            }
        }
        private Address? addrSelected;

        public Address? AnchorAddress
        {
            get => addrAnchor;
            set
            {
                if (this.addrAnchor == value)
                    return;
                this.RaiseAndSetIfChanged(ref this.addrAnchor, value);
                RaiseSelectionChanged();
            }
        }
        private Address? addrAnchor;

        public ImageMap ImageMap { get; set; }

        public IServiceProvider Services { get; }

        public MemoryArea? MemoryArea
        {
            get => this.memoryArea;
            set
            {
                if (this.memoryArea == value)
                    return;
                this.RaiseAndSetIfChanged(ref this.memoryArea, value);
                this.SelectedAddress = value?.BaseAddress;
                this.AnchorAddress = value?.BaseAddress;
            }
        }
        private MemoryArea? memoryArea;

        public SegmentMap SegmentMap { get; set; }

        public ITextViewModel DisassemblyTextModel
        {
            get => this.disassemblyTextModel;
            set => this.RaiseAndSetIfChanged(ref this.disassemblyTextModel, value);
        }
        private ITextViewModel disassemblyTextModel;

        /// <summary>
        /// The CPU architecture chosen by the user.
        /// </summary>
        public ListOption? SelectedArchitectureOption
        {
            get { return selectedArchitectureOption; }
            set
            {
                if (selectedArchitectureOption == value)
                    return;
                selectedArchitectureOption = value;
                this.SelectedArchitecture = LoadArchitecture();
            }
        }
        private ListOption? selectedArchitectureOption;

        public IProcessorArchitecture? SelectedArchitecture
        {
            get { return this.archSelected; }
            set
            {
                if (archSelected == value)
                    return;
                this.RaiseAndSetIfChanged(ref this.archSelected, value);
            }
        }
        private IProcessorArchitecture? archSelected;

        private ListOption[] BuildArchitectureViewModel()
        {
            var result = new List<ListOption>();
            result.Add(new ListOption("(Default)", null));
            var cfgSvc = Services.GetService<IConfigurationService>();
            if (cfgSvc is not null)
            {
                foreach (var arch in cfgSvc.GetArchitectures().OrderBy(a => a.Description))
                {
                    var choice = new ListOption(arch.Description ?? arch.Name!, arch.Name);
                    result.Add(choice);
                }
            }
            return result.ToArray();
        }

        private IProcessorArchitecture? LoadArchitecture()
        {
            IProcessorArchitecture? arch = null;
            var archName = this.SelectedArchitectureOption?.Value as string;
            if (archName is not null)
            {
                arch = Services.GetService<IConfigurationService>()?.GetArchitecture(archName);
            }
            return arch;
        }

        private void RaiseSelectionChanged()
        {
            if (raisingSelectionChanged || this.Program is null || this.AnchorAddress is null || this.SelectedAddress is null)
                return;
            this.raisingSelectionChanged = true;
            var par = ProgramAddressRange.HalfOpenRange(this.Program, AnchorAddress, SelectedAddress);
            this.selAddrSvc.SelectedAddressRange = par;
            this.raisingSelectionChanged = false;
        }


        private void UpdateSelection()
        {
            var addrRange = selAddrSvc.SelectedAddressRange;
            if (addrRange is null)
                return;

            // Changes to other programs don't affect us.
            if (addrRange.Program != this.Program)
                return;
            if (!this.Program.SegmentMap.TryFindSegment(addrRange.Address, out var segment))
                return;
            this.MemoryArea = segment.MemoryArea;
            this.AnchorAddress = addrRange.Address;
            this.SelectedAddress = addrRange.Address + addrRange.Length;
            this.DisassemblyTextModel = new DisassemblyTextModel(factory, Program, this.SelectedArchitecture ?? Program.Architecture, segment);
        }

        private void SelAddrSvc_SelectedAddressChanged(object? sender, EventArgs e)
        {
            UpdateSelection();
        }
    }
}
