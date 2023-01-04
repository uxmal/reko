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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Output;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Services;
using System;
using System.IO;
using System.Linq;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class HexDisassemblerController : IWindowPane
    {
        private IServiceProvider services;
        private HexDisassemblerView control;
        private IProcessorArchitecture arch;

        public IWindowFrame Frame { get; set; }

        public void Close()
        {
        }

        public object CreateControl()
        {
            this.control = new HexDisassemblerView();
            this.Attach(control);
            return control;
        }

        private void Attach(HexDisassemblerView control)
        {
            control.Load += Control_Load;
            control.Architectures.SelectedIndexChanged += Architecture_SelectedIndexChanged;
            control.Address.TextChanged += Address_TextChanged;
            control.HexBytes.TextChanged += HexBytes_TextChanged;
        }

        private void Control_Load(object sender, EventArgs e)
        {
            var configSvc = services.GetService<IConfigurationService>();
            if (configSvc is null)
                return;
            var archs = configSvc.GetArchitectures()
                .Select(a => new ListOption(a.Description, a.Name));
            control.Architectures.DataSource = archs.ToList();
            control.Address.Text = "0";
        }

        private void HexBytes_TextChanged(object sender, EventArgs e)
        {
            Disassemble();
        }

        private void Disassemble()
        {
            try
            {
                if (this.arch is null)
                    return;
                if (!arch.TryParseAddress(control.Address.Text, out var addr))
                    return;
                var bytes = BytePattern.FromHexBytes(control.HexBytes.Text);
                if (bytes.Length > 0)
                {
                    var mem = arch.CreateMemoryArea(addr, bytes);
                    var dumper = new Dumper(new Program())
                    {
                        ShowAddresses = true,
                        ShowCodeBytes = true,
                    };
                    var sw = new StringWriter();
                    dumper.DumpAssembler(arch, mem, mem.BaseAddress, bytes.Length, new Core.Output.TextFormatter(sw));
                    control.Disassembly.Text = sw.ToString();
                }
                else
                {
                    control.Disassembly.Text = "";
                }
            }
            catch (Exception ex)
            {
                var diagSvc = services.RequireService<IDiagnosticsService>();
                diagSvc.Error(ex, "An error occurred during disassembly.");
            }
        }

        private void Architecture_SelectedIndexChanged(object sender, EventArgs e)
        {
            var configSvc = services.GetService<IConfigurationService>();
            if (configSvc is null)
                return;
            var sArch = (string) ((ListOption) control.Architectures.SelectedValue).Value;
            this.arch = configSvc.GetArchitecture(sArch);

            Disassemble();
        }

        private void Address_TextChanged(object sender, EventArgs e)
        {
            Disassemble();
        }


        public void SetSite(IServiceProvider services)
        {
            this.services = services;
        }
    }
}
