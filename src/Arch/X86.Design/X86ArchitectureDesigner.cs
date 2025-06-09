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

using Reko.Core;
using Reko.Core.Services;
using Reko.Gui.Design;
using Reko.Gui.Services;

namespace Reko.Arch.X86.Design
{
    /// <summary>
    /// TreeNode designer that plugs into the project browser and which opens a 
    /// tabbed document window to show the AmigaOS properties window pane.
    /// </summary>
    public class X86ArchitectureDesigner : ArchitectureDesigner
    {
        public override void Initialize(object obj)
        {
            base.Initialize(obj);
        }

        public override void DoDefaultAction()
        {
            var shellUiSvc = Services.RequireService<IDecompilerShellUiService>();
            var windowFrame = shellUiSvc.FindDocumentWindow(GetType().FullName, Component);
            if (windowFrame is null)
            {
                var arch = (IntelArchitecture)Component;
                windowFrame = shellUiSvc.CreateDocumentWindow(
                    GetType().FullName,
                    Component,
                    arch.Description,
                    new X86PropertiesInteractor(arch));
            }
            windowFrame.Show();
        }
    }
}
