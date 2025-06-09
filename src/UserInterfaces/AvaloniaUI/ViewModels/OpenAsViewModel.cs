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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using ReactiveUI;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Gui;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    public class OpenAsViewModel : ReactiveObject
    {
        private readonly IConfigurationService configSvc;
        private readonly IFileSystemService fs;
        private bool fileExists;

        public OpenAsViewModel(IServiceProvider services)
        {
            this.filename = "";
            this.address = "";
            this.loadAddressChecked = true;
            this.configSvc = services.RequireService<IConfigurationService>();
            this.fs = services.RequireService<IFileSystemService>();

            this.FileFormats = configSvc.GetRawFiles()
                .Select(ff => new ListOption(ff.Description ?? ff.Name!, ff))
                .OrderBy(o => o.Text)
                .ToArray();
            this.selectedFileFormat = this.FileFormats[0];

            this.Environments = WithNone(configSvc.GetEnvironments()
                .Select(e => new ListOption(e.Description ?? e.Name!, e)))
                .OrderBy(o => o.Text)
                .ToArray();
            this.selectedEnvironment = this.Environments[0];

            this.Architectures = WithNone(configSvc.GetArchitectures()
                .Select(a => new ListOption(a.Description ?? a.Name!, a)))
                .OrderBy(o => o.Text)
                .ToArray();
            this.selectedArchitecture = this.Architectures[0];

            this.ProcessorModels = new ObservableCollection<ListOption>();
            PopulateProcessorModels();
        }

        private IEnumerable<ListOption> WithNone(IEnumerable<ListOption> options)
        {
            var none = new[] { new ListOption("(None)", null) };
            return none.Concat(options);
        }

        public string FileName
        {
            get => filename;
            set 
            {
                this.RaiseAndSetIfChanged(ref filename, value);
                this.fileExists = fs.FileExists(value);
                SetEnableProperties();
            }
        }
        private string filename;

        public ListOption[] FileFormats { get; set; }
        
        public ListOption SelectedFileFormat
        {
            get => selectedFileFormat;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedFileFormat, value);
                SetEnableProperties();
            }
        }
        private ListOption selectedFileFormat;

        public ListOption[] Environments { get;  }

        public ListOption SelectedEnvironment
        {
            get => selectedEnvironment;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedEnvironment, value);
                SetEnableProperties();
            }
        }
        private ListOption selectedEnvironment;

        public bool EnvironmentsEnabled
        {
            get => environmentsEnabled;
            set => this.RaiseAndSetIfChanged(ref environmentsEnabled, value);
        }
        private bool environmentsEnabled;


        public ListOption[] Architectures { get; }

        public ListOption SelectedArchitecture
        {
            get => selectedArchitecture;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedArchitecture, value);
                SetEnableProperties();
            }
        }
        private ListOption selectedArchitecture;

        public bool ArchitecturesEnabled
        {
            get => architecturesEnabled;
            set => this.RaiseAndSetIfChanged(ref architecturesEnabled, value);
        }
        private bool architecturesEnabled;


        public ObservableCollection<ListOption> ProcessorModels { get; }

        public bool ProcessorModelsEnabled
        {
            get => processorModelsEnabled;
            set => this.RaiseAndSetIfChanged(ref processorModelsEnabled, value);
        }
        private bool processorModelsEnabled;

        public ListOption SelectedProcessorModel
        {
            get => selectedArchitecture;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedProcessorModel, value);
                SetEnableProperties();
            }
        }
        private ListOption? selectedProcessorModel;


        public string Address
        {
            get => address;
            set
            {
                this.RaiseAndSetIfChanged(ref address, value);
                this.SetEnableProperties();
            }
        }
        private string address;

        public bool AddressEnabled
        {
            get => addressEnabled;
            set => this.RaiseAndSetIfChanged(ref addressEnabled, value);
        }
        private bool addressEnabled;

        public bool OKButtonEnabled
        {
            get => okButtonEnabled;
            set => this.RaiseAndSetIfChanged(ref okButtonEnabled, value);
        }
        private bool okButtonEnabled;

        public bool LoadAddressChecked
        {
            get => loadAddressChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref loadAddressChecked, value);
                SetEnableProperties();
            }
        }
        private bool loadAddressChecked;


        private void PopulateProcessorModels()
        {
            var archModels = Array.Empty<ModelDefinition>();
            var arch = (ArchitectureDefinition?) SelectedArchitecture.Value;
            if (arch is { })
            {
                archModels = arch.Models.Values.ToArray();
            }
            ProcessorModels.Clear();
            ProcessorModels.AddRange(WithNone(
                archModels
                .OrderBy(m => m.Name)
                .Select(m => new ListOption(m.Name ?? "?", m))
                .OrderBy(o => o.Text)));
            ProcessorModelsEnabled = archModels.Length > 0;
        }

        private void SetEnableProperties()
        {
            var rawfile = (RawFileDefinition?) SelectedFileFormat.Value;
            var unknownRawFileFormat = rawfile is null;
            bool platformRequired = unknownRawFileFormat;
            bool archRequired = unknownRawFileFormat;
            bool addrRequired = unknownRawFileFormat;
            if (!unknownRawFileFormat)
            {
                platformRequired = string.IsNullOrEmpty(rawfile?.Environment);
                archRequired = string.IsNullOrEmpty(rawfile?.Architecture);
                addrRequired = string.IsNullOrEmpty(rawfile?.BaseAddress);
            }
            EnvironmentsEnabled = platformRequired;
            ArchitecturesEnabled = archRequired;
            AddressEnabled = addrRequired && LoadAddressChecked;
            //dlg.PropertyGrid.Enabled = dlg.PropertyGrid.SelectedObject is not null;
            OKButtonEnabled = 
                fileExists &&
                !unknownRawFileFormat;
        }

        public LoadDetails? CreateLoadDetails()
        {
            var rawFileOption = SelectedFileFormat;
            string? archName = null;
            string? envName = null;
            string? sAddr = null;
            string? loader = null;
            EntryPointDefinition? entry = null;
            if (rawFileOption is not null && rawFileOption.Value is not null)
            {
                var raw = (RawFileDefinition) rawFileOption.Value;
                loader = raw.Loader;
                archName = raw.Architecture;
                envName = raw.Environment;
                sAddr = raw.BaseAddress;
                entry = raw.EntryPoint;
            }
            var archOption = (ArchitectureDefinition?) SelectedArchitecture.Value;
            var envOption = (PlatformDefinition?) SelectedEnvironment.Value;
            archName ??= archOption?.Name!;
            envName ??= envOption?.Name;
            sAddr ??= this.Address.Trim();

            var arch = configSvc.GetArchitecture(archName);
            if (arch is null)
                throw new InvalidOperationException($"Unable to load {archName} architecture.");
            //$NYI arch.LoadUserOptions(this.ArchitectureOptions);
            if (!arch.TryParseAddress(sAddr, out var _))
                sAddr = null;
            return new LoadDetails
            {
                Location = ImageLocation.FromUri(this.FileName),
                LoaderName = loader,
                ArchitectureName = archName,
                //$NYI: ArchitectureOptions = this.ArchitectureOptions,
                PlatformName = envName,
                LoadAddress = sAddr,
                EntryPoint = entry,
            };
        }
    }
}
