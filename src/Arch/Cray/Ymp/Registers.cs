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
using Reko.Core.Types;

namespace Reko.Arch.Cray.Ymp
{
    public class Registers
    {
        public static readonly RegisterStorage[] SRegs;
        public static readonly RegisterStorage[] TRegs;
        public static readonly RegisterStorage[] ARegs;
        public static readonly RegisterStorage[] BRegs;
        public static readonly RegisterStorage[] VRegs;
        public static readonly RegisterStorage[] STRegs;

        public static readonly RegisterStorage sb;
        public static readonly RegisterStorage sp;

        public static readonly RegisterStorage st;
        public static readonly RegisterStorage rt;  // Real time clock
        public static readonly RegisterStorage sm;  // Semaphore register
        public static readonly RegisterStorage vl;  // Vector length

        static Registers()
        {
            var factory = new StorageFactory();
            SRegs = factory.RangeOfReg64(8, "S{0}");
            TRegs = factory.RangeOfReg64(64, "T{0}");
            ARegs = factory.RangeOfReg32(8, "A{0}");
            BRegs = factory.RangeOfReg32(64, "B{0}");
            // The data type of a VReg is actually an array of 64-bit values,
            // but the Reko object model doesn't support it. This is cleaned
            // up in the rewriter.
            VRegs = factory.RangeOfReg(8, n => $"V{n}", PrimitiveType.Word64);

            // Pseudo-registers
            sb = factory.Reg64("SB");
            st = factory.Reg64("ST");
            sp = factory.Reg32("SP");   // There is no specific YMP stack register.

            // System registers
            var sysfactory = new StorageFactory(StorageDomain.SystemRegister);
            STRegs = factory.RangeOfReg64(8, "ST{0}");
            rt = sysfactory.Reg64("RT");
            sm = sysfactory.Reg64("SM");
            vl = sysfactory.Reg64("VL");
        }
    }
}