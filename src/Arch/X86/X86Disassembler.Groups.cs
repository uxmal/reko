#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
        private static Decoder [] CreateGroupOprecs()
        {
            return new Decoder[] 
			{
				// group 1
				new InstructionDecoder(Opcode.add),
				new InstructionDecoder(Opcode.or),
				new InstructionDecoder(Opcode.adc),
				new InstructionDecoder(Opcode.sbb),
				new InstructionDecoder(Opcode.and),
				new InstructionDecoder(Opcode.sub),
				new InstructionDecoder(Opcode.xor),
				new InstructionDecoder(Opcode.cmp),

				// group 2
				new InstructionDecoder(Opcode.rol),
				new InstructionDecoder(Opcode.ror),
				new InstructionDecoder(Opcode.rcl),
				new InstructionDecoder(Opcode.rcr),
				new InstructionDecoder(Opcode.shl),
				new InstructionDecoder(Opcode.shr),
				new InstructionDecoder(Opcode.shl),
				new InstructionDecoder(Opcode.sar),

				// group 3
				new InstructionDecoder(Opcode.test, ",Ix"),
				new InstructionDecoder(Opcode.test, ",Ix"),
				new InstructionDecoder(Opcode.not),
				new InstructionDecoder(Opcode.neg),
				new InstructionDecoder(Opcode.mul),
				new InstructionDecoder(Opcode.imul),
				new InstructionDecoder(Opcode.div),
				new InstructionDecoder(Opcode.idiv),
				
				// group 4
				new InstructionDecoder(Opcode.inc, "Eb"),
				new InstructionDecoder(Opcode.dec, "Eb"),
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi, 

				// group 5
				new InstructionDecoder(Opcode.inc, "Ev"),
				new InstructionDecoder(Opcode.dec, "Ev"),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.call, InstrClass.Transfer|InstrClass.Call, "Ev"),
                    new InstructionDecoder(Opcode.call, InstrClass.Transfer|InstrClass.Call, "Eq")),
                new InstructionDecoder(Opcode.call, InstrClass.Transfer|InstrClass.Call, "Ep"),
                new Alternative64Decoder(
				    new InstructionDecoder(Opcode.jmp, InstrClass.Transfer, "Ev"),
				    new InstructionDecoder(Opcode.jmp, InstrClass.Transfer, "Eq")),
				new InstructionDecoder(Opcode.jmp, InstrClass.Transfer, "Ep"),
                new Alternative64Decoder(
				    new InstructionDecoder(Opcode.push, "Ev"),
				    new InstructionDecoder(Opcode.push, "Eq")),
                s_nyi,

				// group 6
				new Group7OpRec(
					new SingleByteOpRec(Opcode.sldt, "Ew"),
					new SingleByteOpRec(Opcode.sldt, "Rv"),
					new SingleByteOpRec(Opcode.sldt, "Rv"),
					new SingleByteOpRec(Opcode.sldt, "Rv"),
					new SingleByteOpRec(Opcode.sldt, "Rv"),
					new SingleByteOpRec(Opcode.sldt, "Rv"),
					new SingleByteOpRec(Opcode.sldt, "Rv"),
					new SingleByteOpRec(Opcode.sldt, "Rv")),
				new SingleByteOpRec(Opcode.str, "Ew"),
				new SingleByteOpRec(Opcode.lldt, "Ew"),
				new SingleByteOpRec(Opcode.ltr, "Ew"),
				new SingleByteOpRec(Opcode.verr, "Ew"),
				new SingleByteOpRec(Opcode.verw, "Ew"),
				s_nyi,
				s_nyi,

				// group 7
				s_nyi,
				s_nyi,
				new Group7Decoder(
                    s_nyi,

                    new InstructionDecoder(Opcode.xgetbv),
                    new InstructionDecoder(Opcode.xsetbv),
                    s_nyi,
                    s_nyi,

                    new InstructionDecoder(Opcode.vmfunc),
                    new InstructionDecoder(Opcode.xend),
                    new InstructionDecoder(Opcode.xtest),
                    s_nyi),
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

				// group 8
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				new InstructionDecoder(Opcode.bt),
				new InstructionDecoder(Opcode.bts),
				new InstructionDecoder(Opcode.btr),
				new InstructionDecoder(Opcode.btc),

				// group 9
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

				// group 10
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

				// group 11
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

				// group 12
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

				// group 13
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

				// group 14
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

				// group 15
				new Group7Decoder(new InstructionDecoder(Opcode.fxsave)),
				new Group7Decoder(new InstructionDecoder(Opcode.fxrstor)),
				new InstructionDecoder(Opcode.ldmxcsr, "Md"),
				new InstructionDecoder(Opcode.stmxcsr, "Md"),
				s_nyi,
				new Group7Decoder(
                    new InstructionDecoder(Opcode.xrstor, "Md"),

                    new InstructionDecoder(Opcode.lfence, ""),
                    new InstructionDecoder(Opcode.lfence, ""),
                    new InstructionDecoder(Opcode.lfence, ""),
                    new InstructionDecoder(Opcode.lfence, ""),

                    new InstructionDecoder(Opcode.lfence, ""),
                    new InstructionDecoder(Opcode.lfence, ""),
                    new InstructionDecoder(Opcode.lfence, ""),
                    new InstructionDecoder(Opcode.lfence, "")),
                new Group7Decoder(
                    new InstructionDecoder(Opcode.xsaveopt, "Md"),

                    new InstructionDecoder(Opcode.mfence, ""),
                    new InstructionDecoder(Opcode.mfence, ""),
                    new InstructionDecoder(Opcode.mfence, ""),
                    new InstructionDecoder(Opcode.mfence, ""),

                    new InstructionDecoder(Opcode.mfence, ""),
                    new InstructionDecoder(Opcode.mfence, ""),
                    new InstructionDecoder(Opcode.mfence, ""),
                    new InstructionDecoder(Opcode.mfence, "")),

                new Group7Decoder(
                    new InstructionDecoder(Opcode.clflush, "Md"),

                    new InstructionDecoder(Opcode.sfence, ""),
                    new InstructionDecoder(Opcode.sfence, ""),
                    new InstructionDecoder(Opcode.sfence, ""),
                    new InstructionDecoder(Opcode.sfence, ""),

                    new InstructionDecoder(Opcode.sfence, ""),
                    new InstructionDecoder(Opcode.sfence, ""),
                    new InstructionDecoder(Opcode.sfence, ""),
                    new InstructionDecoder(Opcode.sfence, "")),

				// group 16
				new InstructionDecoder(Opcode.prefetchnta, "Mb"),
				new InstructionDecoder(Opcode.prefetcht0, "Mb"),
				new InstructionDecoder(Opcode.prefetcht1, "Mb"),
				new InstructionDecoder(Opcode.prefetcht2, "Mb"),
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

				// group 17
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
			};
        }
    }
}
