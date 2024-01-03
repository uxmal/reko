#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Gui.ViewModels;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class SegmentListViewInteractor : IWindowPane
    {
        private readonly Program program;

        public SegmentListViewInteractor(Program program)
        {
            this.program = program;
        }

        public object CreateControl()
        {
            var segmentListView = new SegmentListView();
            Attach(segmentListView);
            return segmentListView;
        }

        private void Attach(SegmentListView segmentListview)
        {
            var segViews = program.SegmentMap.Segments.Values
                .Select(MakeSegmentItemView)
                .ToArray();
            segmentListview.Segments.Items.AddRange(segViews);
        }

        private ListViewItem MakeSegmentItemView(ImageSegment seg)
        {
            var item = new ListViewItem
            {
                Text = seg.Name,
            };
            item.SubItems.Add(new ListViewItem.ListViewSubItem()
            {
                Text = seg.Address.ToString(),
            });
            item.SubItems.Add(new ListViewItem.ListViewSubItem()
            {
                Text = seg.EndAddress.ToString(),
            });
            item.SubItems.Add(new ListViewItem.ListViewSubItem()
            {
                //$TODO: PDP-10 and PDP-11 prefer octal
                Text = $"0x{seg.Size:X}",
            });
            item.SubItems.Add(new ListViewItem.ListViewSubItem()
            {
                Text = seg.Access.HasFlag(AccessMode.Read) ? "R" : "-"
            });
            item.SubItems.Add(new ListViewItem.ListViewSubItem()
            {
                Text = seg.Access.HasFlag(AccessMode.Write) ? "W" : "-"
            }); 
            item.SubItems.Add(new ListViewItem.ListViewSubItem()
            {
                Text = seg.Access.HasFlag(AccessMode.Execute) ? "X" : "-"
            });
            return item;
        }

        public IWindowFrame Frame { get; set; }

        public void Close()
        {
        }


        public void SetSite(IServiceProvider services)
        {
        }
    }
}