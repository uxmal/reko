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

using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Services;
using System;
using System.Data;
using System.Linq;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class AssembleFileInteractor 
    {
        private AssembleFileDialog dlg;

        public void Attach(AssembleFileDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += dlg_Load;
            dlg.BrowseButton.Click += BrowseButton_Click;
        }


        void dlg_Load(object sender, EventArgs e)
        {
            var asms = dlg.Services.RequireService<IConfigurationService>()
                .GetArchitectures()
                .Select(elem => new ListOption(elem.Description, elem))
                .ToList();
            dlg.ArchitectureList.DataSource = asms;
        }

        private async void BrowseButton_Click(object sender, EventArgs e)
        {
            var uiSvc = dlg.Services.RequireService<IDecompilerShellUiService>();
            var fileName = await uiSvc.ShowOpenFileDialog(dlg.FileName.Text);
            if (!string.IsNullOrEmpty(fileName))
            {
                dlg.FileName.Text = fileName;
            }
        }
    }
}
