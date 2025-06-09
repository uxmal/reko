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
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.Gui.TextViewing;
using Reko.Gui.ViewModels;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class ImageSegmentPane : IWindowPane, ICommandTarget
    {
        private readonly ImageSegment segment;
        private readonly TextSpanFactory factory;
        private Program program;
        private ImageSegmentView segmentView;
        private IServiceProvider services;

        public ImageSegmentPane(ImageSegment segment, Program program, TextSpanFactory factory)
        {
            this.segment = segment;
            this.program = program;
            this.factory = factory;
        }

        public IWindowFrame Frame { get; set; }

        public object CreateControl()
        {
            this.segmentView = new ImageSegmentView();
            this.segmentView.TextView.Services = services;
            this.segmentView.TextView.Navigate += TextView_Navigate;
            DisplaySegment(segment, program);
            return this.segmentView;
        }

        void TextView_Navigate(object sender, EditorNavigationArgs e)
        {
            var addr = e.Destination as Address?;
            if (addr is not null)
            {
                var svc = services.RequireService<ILowLevelViewService>();
                svc.ShowMemoryAtAddress(program, addr.Value);
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
                if (segmentView is null ||
                    segment is null ||
                    segment.Designer is null)
                    return;
                this.program = program;
                var tsf = new TextSpanFormatter(factory);
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
                switch ((CmdIds) cmdId.ID)
                {
                case CmdIds.EditCopy:
                case CmdIds.EditSelectAll:
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                    return true;
                }
            }
            return false;
        }

        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            bool result = false;
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch ((CmdIds) cmdId.ID)
                {
                case CmdIds.EditCopy:
                    var ms = new MemoryStream();
                    this.segmentView.TextView.Selection.Save(ms, DataFormats.UnicodeText);
                    var text = new string(Encoding.Unicode.GetChars(ms.ToArray()));
                    Clipboard.SetData(DataFormats.UnicodeText, text);
                    result = true;
                    break;
                case CmdIds.EditSelectAll:
                    var tv = this.segmentView.TextView;
                    tv.SelectAll();
                    result = true;
                    break;
                }
            }
            return ValueTask.FromResult(result);
        }
    }
}
