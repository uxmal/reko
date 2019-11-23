#region License
/* 
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>.
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
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Nro
{
    public enum NroSegmentType
    {
        Text,
        Ro,
        Data,
        ApiInfo,
        DynStr,
        DynSym
    }

    public struct NroStart
    {
        public UInt32 unused;
        public UInt32 mod0offset;
        public UInt64 padding;
    }

    public struct NroSegmentHeader
    {
        public UInt32 file_offset;
        public UInt32 size;
    }

    public struct NroHeader
    {
        public UInt32 magic;
        public UInt32 version;
        public UInt32 filesize;
        public UInt32 flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public NroSegmentHeader[] segments0;
        public UInt32 bss_size;
        private UInt32 reserved0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] build_id;
        private UInt32 reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public NroSegmentHeader[] segments1;
    }
}
