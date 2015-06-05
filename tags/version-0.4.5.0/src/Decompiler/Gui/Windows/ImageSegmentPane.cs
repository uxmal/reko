#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class ImageSegmentPane : IWindowPane
    {
        private ImageSegmentView segmentView;
        private IServiceProvider services;
        private Program program;

        public Control CreateControl()
        {
            this.segmentView = new ImageSegmentView();

            this.segmentView.TextView.Styles.Add("link", new EditorStyle
            {
                Foreground = new SolidBrush(Color.FromArgb(0x00, 0x80, 0x80)),
                Cursor = Cursors.Hand,
            });

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

        public void DisplaySegment(ImageMapSegment segment, Program program)
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
    }
}
