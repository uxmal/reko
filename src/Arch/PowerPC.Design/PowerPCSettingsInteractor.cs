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

using Reko.Core.Configuration;
using Reko.Core.Machine;
using Reko.Core.Services;
using Reko.Gui.Controls;
using Reko.Gui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.PowerPC.Design
{
    public class PowerPCSettingsInteractor : IWindowPane
    {
        private readonly PowerPcArchitecture arch;
        private IServiceProvider services;
        private PowerPCSettingsControl control;
        private IConfigurationService cfgSvc;

        public PowerPCSettingsInteractor(PowerPcArchitecture arch)
        {
            this.arch = arch;
        }

        public IWindowFrame Frame { get; set; }

        public object CreateControl()
        {
            this.control = new PowerPCSettingsControl();
            this.Attach(control);
            return control;
        }

        public void Close()
        {
            if (control is not null)
            {
                control.Dispose();
            }
            control = null;
        }

        private void Attach(PowerPCSettingsControl control)
        {
            control.Load += Control_Load;
        }

        private void Control_Load(object sender, EventArgs e)
        {
            if (services is not null)
            {
                this.cfgSvc = services.RequireService<IConfigurationService>();
                var archOptions = cfgSvc.GetArchitectures().Single(a => a.Name == arch.Name);
                control.ProcessorModel.DataSource = archOptions.Options.Single(o => o.Name == "Model").Choices;
                SelectModel(control.ProcessorModel);
                control.ProcessorModel.SelectedIndexChanged += ProcessorModel_SelectedIndexChanged;
            }
            else
            {
                this.cfgSvc = null;
            }
        }

        private void ProcessorModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (control.ProcessorModel.SelectedItem is null)
                return;
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { ProcessorOption.Model, ((ListOption_v1) control.ProcessorModel.SelectedItem).Value }
            });
        }

        private void SelectModel(IComboBox ddlProcessorModel)
        {
            int index = 0;
            if (arch.Options.TryGetValue(ProcessorOption.Model, out var model))
            {
                var sModel = (string)model;
                if (!string.IsNullOrEmpty(sModel))
                {
                    for (int i = 0; i < ddlProcessorModel.Items.Count; ++i)
                    {
                        var choice = ((ListOption_v1)ddlProcessorModel.Items[i]);
                        if (choice.Value == sModel)
                        {
                            index = i;
                            break;
                        }
                    }
                }
            }
            ddlProcessorModel.SelectedIndex = index;
        }

        public void SetSite(IServiceProvider services)
        {
            this.services = services;
         
        }
    }
}
