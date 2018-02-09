#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        private static OpRec [] CreateGroupOprecs()
        {
            return new OpRec[] 
			{
				// group 1
				new SingleByteOpRec(Opcode.add),
				new SingleByteOpRec(Opcode.or),
				new SingleByteOpRec(Opcode.adc),
				new SingleByteOpRec(Opcode.sbb),
				new SingleByteOpRec(Opcode.and),
				new SingleByteOpRec(Opcode.sub),
				new SingleByteOpRec(Opcode.xor),
				new SingleByteOpRec(Opcode.cmp),

				// group 2
				new SingleByteOpRec(Opcode.rol),
				new SingleByteOpRec(Opcode.ror),
				new SingleByteOpRec(Opcode.rcl),
				new SingleByteOpRec(Opcode.rcr),
				new SingleByteOpRec(Opcode.shl),
				new SingleByteOpRec(Opcode.shr),
				new SingleByteOpRec(Opcode.shl),
				new SingleByteOpRec(Opcode.sar),

				// group 3
				new SingleByteOpRec(Opcode.test, ",Ix"),
				new SingleByteOpRec(Opcode.test, ",Ix"),
				new SingleByteOpRec(Opcode.not),
				new SingleByteOpRec(Opcode.neg),
				new SingleByteOpRec(Opcode.mul),
				new SingleByteOpRec(Opcode.imul),
				new SingleByteOpRec(Opcode.div),
				new SingleByteOpRec(Opcode.idiv),
				
				// group 4
				new SingleByteOpRec(Opcode.inc, "Eb"),
				new SingleByteOpRec(Opcode.dec, "Eb"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal), 

				// group 5
				new SingleByteOpRec(Opcode.inc, "Ev"),
				new SingleByteOpRec(Opcode.dec, "Ev"),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.call, "Ev"),
                    new SingleByteOpRec(Opcode.call, "Eq")),
                new SingleByteOpRec(Opcode.call, "Ep"),
                new Alternative64OpRec(
				    new SingleByteOpRec(Opcode.jmp, "Ev"),
				    new SingleByteOpRec(Opcode.jmp, "Eq")),
				new SingleByteOpRec(Opcode.jmp, "Ep"),
                new Alternative64OpRec(
				    new SingleByteOpRec(Opcode.push, "Ev"),
				    new SingleByteOpRec(Opcode.push, "Eq")),
                new SingleByteOpRec(Opcode.illegal),

				// group 6
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 7
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new Group7OpRec(
                    new SingleByteOpRec(Opcode.illegal),

                    new SingleByteOpRec(Opcode.xgetbv),
                    new SingleByteOpRec(Opcode.xsetbv),
                    new SingleByteOpRec(Opcode.illegal),
                    new SingleByteOpRec(Opcode.illegal),

                    new SingleByteOpRec(Opcode.vmfunc),
                    new SingleByteOpRec(Opcode.xend),
                    new SingleByteOpRec(Opcode.xtest),
                    new SingleByteOpRec(Opcode.illegal)),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 8
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.bt),
				new SingleByteOpRec(Opcode.bts),
				new SingleByteOpRec(Opcode.btr),
				new SingleByteOpRec(Opcode.btc),

				// group 9
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 10
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 11
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 12
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 13
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 14
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 15
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.ldmxcsr, "Md"),
				new SingleByteOpRec(Opcode.stmxcsr, "Md"),
				new SingleByteOpRec(Opcode.illegal),
				new Group7OpRec(
                    new SingleByteOpRec(Opcode.xrstor, "Md"),

                    new SingleByteOpRec(Opcode.lfence, ""),
                    new SingleByteOpRec(Opcode.lfence, ""),
                    new SingleByteOpRec(Opcode.lfence, ""),
                    new SingleByteOpRec(Opcode.lfence, ""),

                    new SingleByteOpRec(Opcode.lfence, ""),
                    new SingleByteOpRec(Opcode.lfence, ""),
                    new SingleByteOpRec(Opcode.lfence, ""),
                    new SingleByteOpRec(Opcode.lfence, "")),
                new Group7OpRec(
                    new SingleByteOpRec(Opcode.xsaveopt, "Md"),

                    new SingleByteOpRec(Opcode.mfence, ""),
                    new SingleByteOpRec(Opcode.mfence, ""),
                    new SingleByteOpRec(Opcode.mfence, ""),
                    new SingleByteOpRec(Opcode.mfence, ""),

                    new SingleByteOpRec(Opcode.mfence, ""),
                    new SingleByteOpRec(Opcode.mfence, ""),
                    new SingleByteOpRec(Opcode.mfence, ""),
                    new SingleByteOpRec(Opcode.mfence, "")),

                new Group7OpRec(
                    new SingleByteOpRec(Opcode.clflush, "Md"),

                    new SingleByteOpRec(Opcode.sfence, ""),
                    new SingleByteOpRec(Opcode.sfence, ""),
                    new SingleByteOpRec(Opcode.sfence, ""),
                    new SingleByteOpRec(Opcode.sfence, ""),

                    new SingleByteOpRec(Opcode.sfence, ""),
                    new SingleByteOpRec(Opcode.sfence, ""),
                    new SingleByteOpRec(Opcode.sfence, ""),
                    new SingleByteOpRec(Opcode.sfence, "")),

				// group 16
				new SingleByteOpRec(Opcode.prefetchnta, "Mb"),
				new SingleByteOpRec(Opcode.prefetcht0, "Mb"),
				new SingleByteOpRec(Opcode.prefetcht1, "Mb"),
				new SingleByteOpRec(Opcode.prefetcht2, "Mb"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 17
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
			};
        }
    }
}
