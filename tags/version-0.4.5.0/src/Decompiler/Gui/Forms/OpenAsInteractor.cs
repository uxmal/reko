#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Gui.Forms
{
    public class OpenAsInteractor
    {
        private IOpenAsDialog dlg;

        public void Attach(IOpenAsDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += dlg_Load;
            dlg.BrowseButton.Click += BrowseButton_Click;
            dlg.AddressTextBox.TextChanged += AddressTextBox_TextChanged;
        }

        private void dlg_Load(object sender, EventArgs e)
        {
            var dcCfg = dlg.Services.RequireService<IConfigurationService>();
            PopulateArchitectures(dcCfg);
            PopulatePlatforms(dcCfg);
            dlg.AddressTextBox.Text = "0";
        }

        private void EnableControls()
        {
            dlg.OkButton.Enabled = dlg.FileName.Text.Length > 0;
        }

        private void PopulatePlatforms(IConfigurationService dcCfg)
        {
            var noneOption = new ListOption
            {
                Text = "(None)",
                Value = typeof(DefaultPlatform).AssemblyQualifiedName
            };
            var platforms = new ListOption[] { noneOption }
                .Concat(
                    dcCfg.GetEnvironments()
                    .OfType<OperatingEnvironment>()
                    .OrderBy(p => p.Description)
                    .Where(p => !string.IsNullOrEmpty(p.TypeName))
                    .Select(p => new ListOption { Text = p.Description, Value = p.TypeName }));
            dlg.Platforms.DataSource = new ArrayList(platforms.ToArray());
        }

        private void PopulateArchitectures(IConfigurationService dcCfg)
        {
            var archs = dcCfg.GetArchitectures()
                .OfType<Architecture>()
                .OrderBy(a => a.Description)
                .Select(a => new ListOption { Text = a.Description, Value = a.TypeName });
            dlg.Architectures.DataSource = new ArrayList(archs.ToArray());
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
           var uiSvc =  dlg.Services.RequireService<IDecompilerShellUiService>();
           var fileName = uiSvc.ShowOpenFileDialog("");
           if (fileName != null)
               dlg.FileName.Text = fileName;
        }

        void AddressTextBox_TextChanged(object sender, EventArgs e)
        {
            EnableControls();
        }
    }
}

