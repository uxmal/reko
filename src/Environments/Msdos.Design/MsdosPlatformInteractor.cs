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

using Reko.Gui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Environments.Msdos.Design
{
    public class MsdosPropertiesInteractor : IWindowPane
    {
        private MsdosPlatform platform;

        public MsdosPropertiesInteractor(MsdosPlatform platform)
        {
            this.platform = platform;
        }

        public MsdosPropertiesPanel Control { get; private set; }
        public IServiceProvider Services { get; private set; }
        public IWindowFrame Frame { get; set; }

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
            Control = new MsdosPropertiesPanel();

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

    }
}
