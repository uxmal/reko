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
using Reko.Gui;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.WindowsForms
{
    public class ResourceEditorService : IResourceEditorService
    {
        private IServiceProvider services;

        public ResourceEditorService(IServiceProvider services)
        {
            this.services = services;
        }

        public void Show(Program program, ProgramResourceInstance resource)
        {
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            var rsrcToString = ResourceToString(resource);
            var wnd = uiSvc.FindDocumentWindow("resEdit", resource);
            if (wnd is null)
            {
                wnd = uiSvc.CreateDocumentWindow(
                    "resEdit",
                    resource,
                    resource.Name,
                    new ResourceEditorInteractor(program, resource));
            }
            wnd.Show();
        }

        private string ResourceToString(ProgramResourceInstance resource)
        {
            return string.Format("{0}:{1}", resource.Type, resource.Name);
        }
    }
}
