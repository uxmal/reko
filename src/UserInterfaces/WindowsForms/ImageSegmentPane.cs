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
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class ImageSegmentPane : IWindowPane, ICommandTarget
    {
        private ImageSegmentView segmentView;
        private IServiceProvider services;
        private Program program;

        public IWindowFrame Frame { get; set; }

        public object CreateControl()
        {
            this.segmentView = new ImageSegmentView();
            this.segmentView.TextView.Services = services;
            this.segmentView.TextView.Navigate += TextView_Navigate;
            return this.segmentView;
        }

        void TextView_Navigate(object sender, EditorNavigationArgs e)
        {
            var addr = e.Destination as Address;
            if (addr !=null)
            {
                var svc = services.RequireService<ILowLevelViewService>();
                svc.ShowMemoryAtAddress(program, addr);
            }
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }

        public void Close()
        {
        }

        public void DisplaySegment(ImageSegment segment, Program program)
        {
            try
            {
                if (segmentView == null ||
                    segment == null ||
                    segment.Designer == null)
                    return;
                this.program = program;
                var tsf = new TextSpanFormatter();
                segment.Designer.Render(
                    segment,
                    program,
                    tsf);
                this.segmentView.TextView.Model = tsf.GetModel();
            }
            catch
            {

            }
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditCopy:
                case CmdIds.EditSelectAll:
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                    return true;
                }
            }
            return false;
        }

        public bool Execute(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditCopy:
                    var ms = new MemoryStream();
                    this.segmentView.TextView.Selection.Save(ms, DataFormats.UnicodeText);
                    var text = new string(Encoding.Unicode.GetChars(ms.ToArray()));
                    Clipboard.SetData(DataFormats.UnicodeText, text);
                    return true;
                case CmdIds.EditSelectAll:
                    var tv = this.segmentView.TextView;
                    tv.SelectAll();
                    return true;
                }
            }
            return false;
        }
    }
}
