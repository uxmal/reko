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
using Reko.Arch.X86;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class MemoryControlDialog : Form
    {
        public MemoryControlDialog()
        {
            InitializeComponent();
        }

        private void chkShowData_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowData.Checked)
            {
                var mem = new MemoryArea(Address.Ptr32(0x00100000), new byte[2560]);
                var segMap = new SegmentMap(mem.BaseAddress,
                    new ImageSegment("", mem, AccessMode.ReadWriteExecute));
                var imgMap = segMap.CreateImageMap();
                imgMap.AddItemWithSize(Address.Ptr32(0x00100000), new ImageMapBlock { Size = 30 });
                imgMap.AddItemWithSize(Address.Ptr32(0x00100100), new ImageMapBlock { Size = 300 });
                imgMap.AddItemWithSize(Address.Ptr32(0x00100500), new ImageMapBlock { Size = 600 });
                memoryControl1.Architecture = new X86ArchitectureFlat32("x86-protected-32");

                imageMapView1.ImageMap = imgMap;
            }
            else
            {
                memoryControl1.Architecture = null;
            }
        }
    }
}
