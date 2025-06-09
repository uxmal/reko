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

using Reko.Arch.X86;
using Reko.Gui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Arch.X86.Design
{
    public class X86PropertiesInteractor : IWindowPane
    {
        private IntelArchitecture arch;

        public X86PropertiesInteractor(IntelArchitecture arch)
        {
            this.arch = arch;
        }

        public X86ArchitecturePanel Control { get; private set; }
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
            Control = new X86ArchitecturePanel();
            Control.Emulate8087Checkbox.Checked = !arch.Options.ContainsKey("Emulate8087") ||
                arch.Options["Emulate8087"].ToString() == "true";
            Control.Emulate8087Checkbox.CheckedChanged += Emulate8087Checkbox_CheckedChanged;

            return Control;
        }

        private void Emulate8087Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            arch.Options["Emulate8087"] = Control.Emulate8087Checkbox.Checked.ToString().ToLowerInvariant();
            arch.LoadUserOptions(arch.Options);
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
