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

namespace Reko.Environments.Msdos.Design
{
    /// <summary>
    /// TreeNode designer that plugs into the project browser and which opens a 
    /// tabbed document window to show the AmigaOS properties window pane.
    /// </summary>
    public class MsdosPlatformDesigner : PlatformDesigner
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
                var platform = (MsdosPlatform)Component;
                windowFrame = shellUiSvc.CreateDocumentWindow(
                    GetType().FullName,
                    Component,
                    platform.Description,
                    new MsdosPropertiesInteractor(platform));
            }
            windowFrame.Show();
        }
    }
}
