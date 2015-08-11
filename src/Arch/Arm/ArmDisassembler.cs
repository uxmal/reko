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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ArmInstruction = Reko.Arch.Arm.ArmInstruction;

namespace Reko.Arch.Arm {

    public class ArmDisassembler : DisassemblerBase<ArmInstruction> {
        private ArmProcessorArchitecture arch;
        private CapstoneDisassembler<Gee.External.Capstone.Arm.ArmInstruction, ArmRegister, ArmInstructionGroup, ArmInstructionDetail> dasm;
        private IEnumerator<Instruction<Gee.External.Capstone.Arm.ArmInstruction, ArmRegister, ArmInstructionGroup, ArmInstructionDetail>> stream;

        public ArmDisassembler(ArmProcessorArchitecture arch, ImageReader rdr) {
            var dasm = CapstoneDisassembler.CreateArmDisassembler(
                DisassembleMode.Arm32 | DisassembleMode.LittleEndian);
            dasm.EnableDetails = true;
            this.stream = dasm.DisassembleStream(
                rdr.Bytes, 
                (int)rdr.Offset, 
                (long)(rdr.Address.ToLinear() - rdr.Offset))
                .GetEnumerator();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                stream.Dispose();
                dasm.Dispose();
            }
            base.Dispose(disposing);
        }

        public override ArmInstruction DisassembleInstruction() {
            if (stream.MoveNext())
            {
                return new ArmInstruction(stream.Current);
            }
            else
                return null;
        }
    }
}