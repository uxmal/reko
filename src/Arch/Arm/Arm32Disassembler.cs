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
using System.Collections.Generic;

namespace Reko.Arch.Arm
{
    public class Arm32Disassembler : DisassemblerBase<Arm32Instruction> 
    {
        private IEnumerator<Instruction<Gee.External.Capstone.Arm.ArmInstruction, ArmRegister, ArmInstructionGroup, ArmInstructionDetail>> stream;
        private static int disposes;

        public Arm32Disassembler(Arm32ProcessorArchitecture arch, EndianImageReader rdr) {
            var dasm = CapstoneDisassembler.CreateArmDisassembler(
                DisassembleMode.Arm32 | DisassembleMode.LittleEndian);
            dasm.EnableDetails = true;
            this.stream = dasm.DisassembleStream(
                rdr.Bytes, 
                (int)rdr.Offset, 
                (long)rdr.Address.ToLinear())
                .GetEnumerator();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                ++disposes;
                stream.Dispose();
            }
            base.Dispose(disposing);
        }

        public override Arm32Instruction DisassembleInstruction() {
            if (stream.MoveNext())
            {
                return new Arm32Instruction(stream.Current);
            }
            else
                return null;
        }
    }
}