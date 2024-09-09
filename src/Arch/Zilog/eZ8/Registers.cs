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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Zilog.eZ8
{
    public static class Registers
    {
        public static RegisterStorage Flags { get; }
        public static RegisterStorage RP { get; }   // Register pointer
        public static RegisterStorage SP { get; }   // Stack pointer
        public static RegisterStorage SPL { get; }   // Stack pointer low part
        public static RegisterStorage SPH { get; }   // Stack pointer high part


        public static FlagGroupStorage C { get; }   // Carry flag


        static Registers()
        {
            Flags = new RegisterStorage("Flags", 0xFFC, 0, PrimitiveType.Byte);
            RP = new RegisterStorage("Flags", 0xFFD, 0, PrimitiveType.Byte);
            SP = new RegisterStorage("sp", 0xFFE, 0, PrimitiveType.Word16);
            SPL = new RegisterStorage("spl", 0xFFE, 0, PrimitiveType.Byte);
            SPH = new RegisterStorage("spl", 0xFFE, 8, PrimitiveType.Byte);

            C = new FlagGroupStorage(Flags, (uint) FlagM.CF, nameof(C));
        }
    }

    [Flags]
    public enum FlagM
    {
        CF = 0x80,
        ZF = 0x40,
        SF = 0x20,
        VF = 0x10,
        DF = 0x08,
        HF = 0x04,
        F2 = 0x02,
        F1 = 0x01,
    }
}
