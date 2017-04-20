#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Gui.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Windows.Forms
{
    public class SymbolSourceInteractor
    {
        private SymbolSourceDialog dlg;

        public void Attach(SymbolSourceDialog dlg)
        {
            this.dlg = dlg;
            this.dlg.Load += Dlg_Load;
            this.dlg.SymbolFileUrl.LostFocus += SymbolFileUrl_LostFocus;
            this.dlg.CustomSourceCheckbox.CheckedChanged += CustomSourceCheckbox_CheckedChanged;

        }

 
        private void Dlg_Load(object sender, EventArgs e)
        {
            PopulateSymbolSources();
            EnableControls();
        }

        private void SymbolFileUrl_LostFocus(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void CustomSourceCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            EnableControls();
        }


        private void PopulateSymbolSources()
        {
            var cfgSvc = dlg.Services.RequireService<IConfigurationService>();
            var items = cfgSvc.GetSymbolSources();
            //dlg.SymbolSourceList.DataSource = 
            dlg.SymbolSourceList.DataSource = (object)items
                .Select(ss => new string[] { ss.Name, ss.Description });
        }

        private void EnableControls()
        {
            dlg.SymbolSourceList.Enabled =
                dlg.SymbolFileUrl.Text.Length > 0 &&
                !dlg.CustomSourceCheckbox.Checked;
            dlg.SymbolSourceClasses.Enabled = 
                dlg.CustomSourceCheckbox.Checked;
            dlg.AssemblyFile.Enabled = 
                dlg.CustomSourceCheckbox.Checked;
            dlg.BrowseAssemblyFile.Enabled = 
                dlg.CustomSourceCheckbox.Checked;
            dlg.OkButton.Enabled = false;
            dlg.SymbolSourceClasses.Enabled = false;
        }
    }
}
