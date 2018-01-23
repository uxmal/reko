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

using Gee.External.Capstone;
using Gee.External.Capstone.Arm64;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public class AArch64Disassembler : DisassemblerBase<AArch64Instruction>
    {
        private IEnumerator<Instruction<Arm64Instruction, Arm64Register, Arm64InstructionGroup, Arm64InstructionDetail>> stream;

        public AArch64Disassembler(EndianImageReader rdr)
        {
            var dasm = CapstoneDisassembler.CreateArm64Disassembler(DisassembleMode.Arm32);
            dasm.EnableDetails = true;
            this.stream = dasm.DisassembleStream(
                rdr.Bytes,
                (int)rdr.Offset,
                (long)rdr.Address.ToLinear() - rdr.Offset)
                .GetEnumerator();
        }

        public override AArch64Instruction DisassembleInstruction()
        {
            if (stream.MoveNext())
            {
                return new AArch64Instruction(stream.Current);
            }
            else
                return null;
        }
    }
}