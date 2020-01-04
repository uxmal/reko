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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.VmsExe
{
    class Header
    {
        public ushort HdrSize { get; internal set; }
        public ushort RvaTaa { get; internal set; }
        public ushort RvaSymbols { get; internal set; }
        public ushort RvaIdent { get; internal set; }
        public ushort RvaPatchData { get; internal set; }
        public ushort Spare0A { get; set; }
        public ushort IdMajor { get; internal set; }
        public ushort IdMinor { get; internal set; }

        public byte HeaderBlocks { get; internal set; }
        public byte ImageType { get; internal set; }
        public ushort Spare12 { get; set; }
        public uint ImageFlags { get; internal set; }
        public ushort IoChannels { get; internal set; }
        public ushort IoSegPages { get; internal set; }
        public ulong RequestedPrivilegeMask { get; internal set; }
        public uint GlobalSectionID { get; internal set; }
        public uint SystemVersionNumber { get; internal set; }
    }
}
