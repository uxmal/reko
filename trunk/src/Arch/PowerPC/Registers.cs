#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public static class Registers
    {
        public static MachineRegister r0 = new MachineRegister("r0", 0, PrimitiveType.Word32);
        public static MachineRegister r1 = new MachineRegister("r1", 1, PrimitiveType.Word32);
        public static MachineRegister r2 = new MachineRegister("r2", 2, PrimitiveType.Word32);
        public static MachineRegister r3 = new MachineRegister("r3", 3, PrimitiveType.Word32);
        public static MachineRegister r4 = new MachineRegister("r4", 4, PrimitiveType.Word32);
        public static MachineRegister r5 = new MachineRegister("r5", 5, PrimitiveType.Word32);
        public static MachineRegister r6 = new MachineRegister("r6", 6, PrimitiveType.Word32);
        public static MachineRegister r7 = new MachineRegister("r7", 7, PrimitiveType.Word32);
        public static MachineRegister r8 = new MachineRegister("r8", 8, PrimitiveType.Word32);
        public static MachineRegister r9 = new MachineRegister("r9", 9, PrimitiveType.Word32);
        public static MachineRegister r10 = new MachineRegister("r10", 10, PrimitiveType.Word32);
        public static MachineRegister r11 = new MachineRegister("r11", 11, PrimitiveType.Word32);
        public static MachineRegister r12 = new MachineRegister("r12", 12, PrimitiveType.Word32);
        public static MachineRegister r13 = new MachineRegister("r13", 13, PrimitiveType.Word32);
        public static MachineRegister r14 = new MachineRegister("r14", 14, PrimitiveType.Word32);
        public static MachineRegister r15 = new MachineRegister("r15", 15, PrimitiveType.Word32);
        public static MachineRegister r16 = new MachineRegister("r16", 16, PrimitiveType.Word32);
        public static MachineRegister r17 = new MachineRegister("r17", 17, PrimitiveType.Word32);
        public static MachineRegister r18 = new MachineRegister("r18", 18, PrimitiveType.Word32);
        public static MachineRegister r19 = new MachineRegister("r19", 19, PrimitiveType.Word32);
        public static MachineRegister r20 = new MachineRegister("r20", 20, PrimitiveType.Word32);
        public static MachineRegister r21 = new MachineRegister("r21", 21, PrimitiveType.Word32);
        public static MachineRegister r22 = new MachineRegister("r22", 22, PrimitiveType.Word32);
        public static MachineRegister r23 = new MachineRegister("r23", 23, PrimitiveType.Word32);
        public static MachineRegister r24 = new MachineRegister("r24", 24, PrimitiveType.Word32);
        public static MachineRegister r25 = new MachineRegister("r25", 25, PrimitiveType.Word32);
        public static MachineRegister r26 = new MachineRegister("r26", 26, PrimitiveType.Word32);
        public static MachineRegister r27 = new MachineRegister("r27", 27, PrimitiveType.Word32);
        public static MachineRegister r28 = new MachineRegister("r28", 28, PrimitiveType.Word32);
        public static MachineRegister r29 = new MachineRegister("r29", 29, PrimitiveType.Word32);
        public static MachineRegister r30 = new MachineRegister("r30", 30, PrimitiveType.Word32);
        public static MachineRegister r31 = new MachineRegister("r31", 31, PrimitiveType.Word32);
    }
}
