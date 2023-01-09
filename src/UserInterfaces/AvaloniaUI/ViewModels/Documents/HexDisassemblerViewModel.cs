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
using Reko.Core.Configuration;
using Reko.Core.Output;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Services;
using System;
using System.IO;
using System.Linq;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents
{
    /// <summary>
    /// View model backing the <see cref="AvaloniaUI.Views.Documents.HexDisassemblerView"/>.
    /// </summary>
    public class HexDisassemblerViewModel : ReactiveObject, IWindowPane
    {
        private IServiceProvider services;
        private readonly IConfigurationService? configSvc;

        public HexDisassemblerViewModel(IServiceProvider services)
        {
            this.services = services;
            this.Address = "0";
            this.configSvc = services.RequireService<IConfigurationService>();
            if (this.configSvc is null)
            {
                Architectures = Array.Empty<ListOption>();
            }
            else
            {
                var archs = configSvc.GetArchitectures()
                    .Select(a => new ListOption(a.Description ?? a.Name!, a.Name));
                this.Architectures = archs.ToArray()!;
                this.SelectedArchitecture = Architectures[0];
            }
        }

        /// <summary>
        /// Address to use when disassembling.
        /// </summary>
        public string? Address
        {
            get { return sAddress; }
            set { this.RaiseAndSetIfChanged(ref sAddress, value, nameof(Address));
                Disassemble();
            }
        }
        private string? sAddress;

        /// <summary>
        /// The disassembled output.
        /// </summary>
        public string? Disassembly
        {
            get { return sDisassembly; }
            set
            {
                this.RaiseAndSetIfChanged(ref sDisassembly, value, nameof(Disassembly));
            }
        }
        private string? sDisassembly;

        /// <summary>
        /// The list of available CPU architectures.
        /// </summary>
        public ListOption[] Architectures { get; set; }

        /// <summary>
        /// The hex bytes to disassemble.
        /// </summary>
        public string? HexBytes
        {
            get { return sHexBytes; }
            set { this.RaiseAndSetIfChanged(ref sHexBytes, value, nameof(HexBytes));
                Disassemble();
            }
        }
        private string? sHexBytes;


        /// <summary>
        /// The CPU architecture chosen by the user.
        /// </summary>
        public ListOption? SelectedArchitecture
        {
            get { return selectedArchitecture; }
            set { 
                selectedArchitecture = value; 
                Disassemble();
            }
        }
        private ListOption? selectedArchitecture;

        public IWindowFrame? Frame { get; set; }

        /// <summary>
        /// Disassemble the machine code represented by the <see cref="HexBytes"/>
        /// and update the <see cref="Disassembly"/> property.
        /// </summary>
        private void Disassemble()
        {
            try
            {
                if (configSvc is null)
                    return;
                if (SelectedArchitecture is null)
                    return;
                if (SelectedArchitecture.Value is not string sArch)
                    return;
                var arch = configSvc.GetArchitecture(sArch);
                if (arch is null)
                    return;
                if (!arch.TryParseAddress(this.Address, out var addr))
                    return;
                if (HexBytes is null)
                    return;
                var bytes = BytePattern.FromHexBytes(HexBytes);
                if (bytes.Length > 0)
                {
                    var mem = arch.CreateCodeMemoryArea(addr, bytes);
                    var dumper = new Dumper(new Program())
                    {
                        ShowAddresses = true,
                        ShowCodeBytes = true,
                    };
                    var sw = new StringWriter();
                    var textFormatter = new TextFormatter(sw);
                    textFormatter.UseTabs = false;
                    dumper.DumpAssembler(arch, mem, mem.BaseAddress, bytes.Length, textFormatter);
                    Disassembly = sw.ToString();
                }
                else
                {
                    Disassembly = "";
                }
            }
            catch (Exception ex)
            {
                var diagSvc = services.RequireService<IDiagnosticsService>();
                diagSvc.Error(ex, "An error occurred during disassembly.");
            }
        }

        public object CreateControl()
        {
            throw new NotImplementedException();
        }

        public void SetSite(IServiceProvider services)
        {
            this.services = services;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
