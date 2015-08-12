#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
    public class ThumbDisassembler : DisassemblerBase<Arm32Instruction>
    {
        private CapstoneDisassembler<ArmInstruction, ArmRegister, ArmInstructionGroup, ArmInstructionDetail> dasm;
        private IEnumerator<Instruction<ArmInstruction, ArmRegister, ArmInstructionGroup, ArmInstructionDetail>> stream;

        public ThumbDisassembler(ImageReader rdr)
        {
            var dasm = CapstoneDisassembler.CreateArmDisassembler(DisassembleMode.ArmThumb);
            dasm.EnableDetails = true;
            this.stream = dasm.DisassembleStream(
                rdr.Bytes,
                (int)rdr.Offset,
                (long)(rdr.Address.ToLinear() - rdr.Offset))
                .GetEnumerator();
        }

        public override Arm32Instruction DisassembleInstruction()
        {
            if (stream.MoveNext())
            {
                return new Arm32Instruction(stream.Current);
            }
            else
                return null;
        }
    }
}
