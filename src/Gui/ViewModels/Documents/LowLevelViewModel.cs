#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Gui.TextViewing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Gui.ViewModels.Documents
{
    public class LowLevelViewModel : ChangeNotifyingObject
    {
        private readonly IServiceProvider services;
        private readonly TextSpanFactory factory;

        public LowLevelViewModel(IServiceProvider services, Program program, TextSpanFactory factory)
        {
            this.services = services;
            this.Program = program;
            this.factory = factory;
            this.SegmentMap = program.SegmentMap;
            this.ImageMap = program.ImageMap;
            this.SelectedArchitecture = program.Architecture;
            this.Architectures = BuildArchitectureViewModel();
        }


        public ListOption[] Architectures { get; set; }

        public Program Program { get; set; }
        public Address? SelectedAddress { get; set; }

        public ImageMap ImageMap { get; set; }


        public MemoryArea? MemoryArea
        {
            get => this.memoryArea;
            set => this.RaiseAndSetIfChanged(ref this.memoryArea, value);
        }
        private MemoryArea? memoryArea;

        public SegmentMap SegmentMap { get; set; }

        public DisassemblyTextModel? DisassemblyTextModel
        {
            get => this.disassemblyTextModel;
            set => this.RaiseAndSetIfChanged(ref this.disassemblyTextModel, value);
        }
        private DisassemblyTextModel? disassemblyTextModel;

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
                this.archSelected = value;
            }
        }
        private IProcessorArchitecture? archSelected;

        private ListOption[] BuildArchitectureViewModel()
        {
            var result = new List<ListOption>();
            result.Add(new ListOption("(Default)", null));
            var cfgSvc = services?.GetService<IConfigurationService>();
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
                arch = services.GetService<IConfigurationService>()?.GetArchitecture(archName);
            }
            return arch;
        }
    }
}
