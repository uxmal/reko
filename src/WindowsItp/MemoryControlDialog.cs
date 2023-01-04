#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
                var mem = new ByteMemoryArea(Address.Ptr32(0x00100000), new byte[2560]);
                var segMap = new SegmentMap(mem.BaseAddress,
                    new ImageSegment("", mem, AccessMode.ReadWriteExecute));
                var imgMap = segMap.CreateImageMap();
                imgMap.AddItemWithSize(Address.Ptr32(0x00100000), new ImageMapBlock(Address.Ptr32(0x00100000)) { Size = 30 });
                imgMap.AddItemWithSize(Address.Ptr32(0x00100100), new ImageMapBlock(Address.Ptr32(0x00100100)) { Size = 300 });
                imgMap.AddItemWithSize(Address.Ptr32(0x00100500), new ImageMapBlock(Address.Ptr32(0x00100500)) { Size = 600 });
                memoryControl1.Architecture = new X86ArchitectureFlat32(
                    new ServiceContainer(), 
                    "x86-protected-32",
                    new Dictionary<string, object>());

                imageMapView1.ImageMap = imgMap;
            }
            else
            {
                memoryControl1.Architecture = null;
            }
        }
    }
}
