#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Gee.External.Capstone;
using Gee.External.Capstone.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public class ThumbDisassembler : DisassemblerBase<Arm32InstructionOld>
    {
        private EndianImageReader rdr;
        private IEnumerator<Instruction<ArmInstruction, ArmRegister, ArmInstructionGroup, ArmInstructionDetail>> stream;
        private CapstoneDisassembler<ArmInstruction, ArmRegister, ArmInstructionGroup, ArmInstructionDetail> dasm;

        public ThumbDisassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
            this.dasm = CapstoneDisassembler.CreateArmDisassembler(DisassembleMode.ArmThumb);
            dasm.EnableDetails = true;
            this.stream = dasm.DisassembleStream(
                rdr.Bytes,
                (int)rdr.Offset,
                (long)(rdr.Address.ToLinear()))
                .GetEnumerator();
        }

        public override Arm32InstructionOld DisassembleInstruction()
        {
            // Check to see if we've hit the end of the address space, and return
            // null if we are. We have to do this because the underlying Capstone
            // disassembler doesn't distinguish between invalid opcodes and
            // reaching the end of the image.
            if (!rdr.IsValid)
            {
                return null;
            }
            if (stream.MoveNext())
            {
                // Capstone doesn't actually use the imageReader, but apparently
                // reko components peek at the reader, so we have to simulate motion.
                rdr.Offset += stream.Current.Bytes.Length;
                return new Arm32InstructionOld(stream.Current);
            }
            else
            {
                // We got an invalid instruction. Create a placeholder and then
                // advance one opcode (which is 2 bytes for Thumb).
                var instr = Arm32InstructionOld.CreateInvalid(rdr.Address);
                rdr.Offset += 2;
                this.stream.Dispose();

                // Skip over the offending instruction and resume.
                this.stream = dasm.DisassembleStream(
                    rdr.Bytes,
                    (int)rdr.Offset,
                    (long)rdr.Address.ToLinear() - rdr.Offset)
                    .GetEnumerator();
                return instr;
            }
        }
    }
}
