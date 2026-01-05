#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.ImageLoaders.LLVM
{
    // "e-m:e-i64:64-f80:128-n8:16:32:64-S128"
    public class DataLayout
    {
        public EndianServices Endianness = EndianServices.Little;
        public int StackAlignment;  // bits
        public int ProgramAddressSpace;
        public int AllocaAddressSpace;
        public Dictionary<int, PointerLayout> PointerLayouts = [];
        public Dictionary<int, AlignmentLayout> IntegerLayouts = [];
        public Dictionary<int, AlignmentLayout> VectorLayouts = [];
        public Dictionary<int, AlignmentLayout> FloatLayouts = [];
    }

    public class PointerLayout
    {
        public int BitSize;
    }

    public class AlignmentLayout
    { }

}
