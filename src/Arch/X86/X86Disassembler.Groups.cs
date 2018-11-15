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

using Reko.Core;

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
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid, 

				// group 5
				new SingleByteOpRec(Opcode.inc, "Ev"),
				new SingleByteOpRec(Opcode.dec, "Ev"),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.call, InstrClass.Transfer|InstrClass.Call, "Ev"),
                    new SingleByteOpRec(Opcode.call, InstrClass.Transfer|InstrClass.Call, "Eq")),
                new SingleByteOpRec(Opcode.call, InstrClass.Transfer|InstrClass.Call, "Ep"),
                new Alternative64OpRec(
				    new SingleByteOpRec(Opcode.jmp, InstrClass.Transfer, "Ev"),
				    new SingleByteOpRec(Opcode.jmp, InstrClass.Transfer, "Eq")),
				new SingleByteOpRec(Opcode.jmp, InstrClass.Transfer, "Ep"),
                new Alternative64OpRec(
				    new SingleByteOpRec(Opcode.push, "Ev"),
				    new SingleByteOpRec(Opcode.push, "Eq")),
                s_invalid,

				// group 6
				new SingleByteOpRec(Opcode.sldt, "Ew"),
				new SingleByteOpRec(Opcode.str, "Ew"),
				new SingleByteOpRec(Opcode.lldt, "Ew"),
				new SingleByteOpRec(Opcode.ltr, "Ew"),
				new SingleByteOpRec(Opcode.verr, "Ew"),
				new SingleByteOpRec(Opcode.verw, "Ew"),
				s_invalid,
				s_invalid,

				// group 7
				s_invalid,
				s_invalid,
				new Group7OpRec(
                    s_invalid,

                    new SingleByteOpRec(Opcode.xgetbv),
                    new SingleByteOpRec(Opcode.xsetbv),
                    s_invalid,
                    s_invalid,

                    new SingleByteOpRec(Opcode.vmfunc),
                    new SingleByteOpRec(Opcode.xend),
                    new SingleByteOpRec(Opcode.xtest),
                    s_invalid),
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// group 8
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				new SingleByteOpRec(Opcode.bt),
				new SingleByteOpRec(Opcode.bts),
				new SingleByteOpRec(Opcode.btr),
				new SingleByteOpRec(Opcode.btc),

				// group 9
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// group 10
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// group 11
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// group 12
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// group 13
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// group 14
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// group 15
				new Group7OpRec(new SingleByteOpRec(Opcode.fxsave)),
				new Group7OpRec(new SingleByteOpRec(Opcode.fxrstor)),
				new SingleByteOpRec(Opcode.ldmxcsr, "Md"),
				new SingleByteOpRec(Opcode.stmxcsr, "Md"),
				s_invalid,
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
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// group 17
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
			};
        }
    }
}
