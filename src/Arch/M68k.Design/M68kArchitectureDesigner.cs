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

using Reko.Core.Services;
using Reko.Gui.Design;
using Reko.Gui.Services;

namespace Reko.Arch.M68k.Design
{
    public class M68kArchitectureDesigner : ArchitectureDesigner
    {
        public override void DoDefaultAction()
        {
            var shellUiSvc = Services.RequireService<IDecompilerShellUiService>();
            var windowFrame = shellUiSvc.FindDocumentWindow(GetType().FullName, Component);
            if (windowFrame is null)
            {
                var arch = (M68kArchitecture)Component;
                windowFrame = shellUiSvc.CreateDocumentWindow(
                    GetType().FullName,
                    Component,
                    arch.Description,
                    new M68kPropertiesInteractor(arch));
            }
            windowFrame.Show();
        }
    }
}
