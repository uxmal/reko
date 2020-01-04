#region License
/* 
 * Copyright (C) 2018-2020 Stefano Moioli <smxdev4@gmail.com>.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Reko.ImageLoaders.Xex.Enums;
using static Reko.ImageLoaders.Xex.Structures;

namespace Reko.ImageLoaders.Xex
{
    public class PESection
    {
        public string Name { get; private set; }
        public uint VirtualOffset { get; private set; }
        public uint VirtualSize { get; private set; }
        public uint PhysicalOffset { get; private set; }
        public uint PhysicalSize { get; private set; }
        public AccessMode Access { get; private set; }

        public PESection(COFFSection section) {
            Name = Encoding.ASCII.GetString(section.Name).Trim('\0');

            AccessMode acc = AccessMode.Read;
            if (section.Flags.HasFlag(PESectionFlags.IMAGE_SCN_MEM_WRITE)) {
                acc |= AccessMode.Write;
            }
            if (section.Flags.HasFlag(PESectionFlags.IMAGE_SCN_MEM_EXECUTE)) {
                acc |= AccessMode.Execute;
            }
            Access = acc;

            VirtualOffset = section.VirtualAddress;
            VirtualSize = section.VirtualSize;
            PhysicalOffset = section.PointerToRawData;
            PhysicalSize = section.SizeOfRawData;
        }
    }
}
