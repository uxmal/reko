#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Gui;
using Reko.Gui.Forms;
using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using Reko.UserInterfaces.WindowsForms.Controls;

namespace Reko.UserInterfaces.WindowsForms
{
    public class ResourceEditorInteractor : IWindowPane
    {
        private Program program;
        private IResourceEditor dlg;
        private ProgramResourceInstance resource;
        private IServiceProvider services;

        public ResourceEditorInteractor(Program program, ProgramResourceInstance resource)
        {
            this.program = program;
            this.resource = resource;
        }

        public IWindowFrame Frame { get; set; }

        public void Close()
        {
            if (dlg != null)
                dlg.Dispose();
        }

        public object CreateControl()
        {
            try
            {
                dlg = services.RequireService<IDialogFactory>().CreateResourceEditor();
                switch (resource.Type)
                {
                case "Windows.BMP": GenerateWindowsBitmap(); break;
                case "Windows.ICO": GenerateWindowsIcon(); break;
                default: return DumpBytes();
                }
            }
            catch
            {
                return DumpBytes();
            }
            return (Control)dlg;
        }

        private void GenerateWindowsBitmap()
        {
            var stm = new MemoryStream(resource.Bytes);
            dlg.Image = Image.FromStream(stm);
        }

        private void GenerateWindowsIcon()
        {
            var icon = new Icon(new MemoryStream(resource.Bytes));
            dlg.Image = Bitmap.FromHicon(icon.Handle);
            icon.Dispose();
        }

        Control DumpBytes()
        {
            var mem = new Reko.Core.MemoryArea(Address.Ptr32(0), resource.Bytes);

            var memCtrl = new MemoryControl();
            memCtrl.Services = services;
            memCtrl.SegmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment("resource", mem, AccessMode.Read));
            memCtrl.Architecture = program.Architecture;
            memCtrl.Font = new Font("Lucida Console", 10F); //$TODO: use user preference
            return memCtrl;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }
    }
}