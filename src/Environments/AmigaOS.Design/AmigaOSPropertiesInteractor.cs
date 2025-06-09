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

using Reko.Gui;
using Reko.Gui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.AmigaOS.Design
{
    public class AmigaOSPropertiesInteractor : IWindowPane
    {
        private AmigaOSPlatform platform;

        public AmigaOSPropertiesInteractor(AmigaOSPlatform platform)
        {
            this.platform = platform;
        }

        public AmigaOSProperties Control { get; private set; }
        public IWindowFrame Frame { get; set; }
        public IServiceProvider Services { get; private set; }

        public void SetSite(IServiceProvider sp)
        {
            this.Services = sp;
        }

        /// <summary>
        /// Creates the user interface control and connects all event handlers.
        /// </summary>
        /// <returns></returns>
        public object CreateControl()
        {
            Control = new AmigaOSProperties();
            var mapKickstartToListOfLibraries = platform.MapKickstartToListOfLibraries;
            if (mapKickstartToListOfLibraries is not null)
            {
                this.Control.KickstartVersionList.DataSource =
                    mapKickstartToListOfLibraries
                    .Select(kv => new ListOption($"Kickstart {kv.Key}", kv))
                    .ToList();
                this.Control.KickstartVersionList.SelectedIndex = 0;
            }
            PopulateLoadedLibraryList();

            this.Control.KickstartVersionList.SelectedIndexChanged += KickstartVersionList_SelectedIndexChanged;
            this.Control.ImportButton.Click += ImportButton_Click;
            return Control;
        }

        public void Close()
        {
            if (Control is not null)
            {
                Control.Dispose();
                Control = null;
            }
        }

        private void KickstartVersionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateLoadedLibraryList();
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            if (this.Control.KickstartVersionList.SelectedValue is ListOption opt &&
                opt.Value is KeyValuePair<string, object> kv &&
                int.TryParse(kv.Key, out int ksVersion))
            {
                platform.SetKickstartVersion(ksVersion);
            }
        }

        private void PopulateLoadedLibraryList()
        {
            if (Control.KickstartVersionList.SelectedValue is ListOption listOption)
            {
                var kv = (KeyValuePair<string, object>)listOption.Value;
                var libList = (List<object>)kv.Value;
                Control.LoadedLibraryList.Items.Clear();
                foreach (object lib in libList)
                {
                    Control.LoadedLibraryList.Items.Add(lib.ToString()); //$TODO: mark available library definition files ?
                }
            }
        }
    }
}
