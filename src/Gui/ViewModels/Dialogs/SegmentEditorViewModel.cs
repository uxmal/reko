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
using Reko.Core.Configuration;
using Reko.Gui.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Reko.Gui.ViewModels.Dialogs
{
    public class SegmentEditorViewModel : ChangeNotifyingObject
    {
        private readonly Program program;

        public SegmentEditorViewModel(Program program, IConfigurationService cfgSvc)
        {
            this.program = program;
            this.Architectures = WithNone(cfgSvc.GetArchitectures()
                .Select(a => new ListOption(a.Description ?? a.Name!, a)))
                .OrderBy(o => o.Text)
                .ToList();
        }

        [Required]
        public string? SegmentName
        {
            get { return segmentName; }
            set { this.RaiseAndSetIfChanged(ref this.segmentName, value); }
        }
        private string? segmentName;

        public string? FileOffset
        {
            get { return fileOffset; }
            set { this.RaiseAndSetIfChanged(ref this.fileOffset, value); }
        }
        private string? fileOffset;

        [Required]
        public string? Length
        {
            get { return length; }
            set { this.RaiseAndSetIfChanged(ref this.length, value); }
        }
        private string? length;

        [Required]
        public string? LoadAddress
        {
            get { return loadAddress; }
            set {
                if (!program.Architecture.TryParseAddress(value, out _))
                    throw new ArgumentException("Invalid address.");
                this.RaiseAndSetIfChanged(ref this.loadAddress, value);
            }
        }
        private string? loadAddress;


        public int SelectedArchitectureIndex
        {
            get { return selectedArchitectureIndex; }
            set { this.RaiseAndSetIfChanged(ref this.selectedArchitectureIndex, value); }
        }
        private int selectedArchitectureIndex;

        public List<ListOption> Architectures { get; }

        public bool ReadAccess
        {
            get { return readAccess; }
            set { this.RaiseAndSetIfChanged(ref this.readAccess, value); }
        }
        private bool readAccess;

        public bool WriteAccess
        {
            get { return writeAccess; }
            set { this.RaiseAndSetIfChanged(ref this.writeAccess, value); }
        }
        private bool writeAccess;

        public bool ExecuteAccess
        {
            get { return executeAccess; }
            set { this.RaiseAndSetIfChanged(ref this.executeAccess, value); }
        }
        private bool executeAccess;

        public object? DataView
        {
            get { return dataView; }
            set { this.RaiseAndSetIfChanged(ref this.dataView, value); }
        }
        private object? dataView;


        public object? Disassembly
        {
            get { return disassembly; }
            set { this.RaiseAndSetIfChanged(ref this.disassembly, value); }
        }
        private object? disassembly;


        public bool OkEnabled
        {
            get { return okEnabled; }
            set { this.RaiseAndSetIfChanged(ref this.okEnabled, value); }
        }
        private bool okEnabled;

        private IEnumerable<ListOption> WithNone(IEnumerable<ListOption> options)
        {
            var none = new[] { new ListOption("(None)", null) };
            return none.Concat(options);
        }
    }
}
