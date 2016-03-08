#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Environments.AmigaOS.Design
{
    public class AmigaOSPropertiesInteractor : IWindowPane
    {
        private AmigaOSPlatform platform;

        public AmigaOSPropertiesInteractor(AmigaOSPlatform platform)
        {
            this.platform = platform;
        }

        public Control Control { get; private set; }
        public IServiceProvider Services { get; private set; }

        public Control CreateControl()
        {
            Control = new AmigaOSProperties(this);
            return Control;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.Services = sp;
        }

        public void Close()
        {
            if (Control != null)
            {
                Control.Dispose();
                Control = null;
            }
        }

        internal List<int> getAvailableKickstarts()
        {
            return AmigaOSPlatform.MapKickstartToListOfLibraries.Keys.ToList();
        }

        internal List<string> GetLibrariesForKickstart(int v)
        {
            return platform.GetLibrarySetForKickstartVersion(v);
        }
    }
}
