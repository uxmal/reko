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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    [Obsolete("Use Arm32Instruction")]
    public class ThumbInstruction : MachineInstruction
    {
        public ThumbInstruction(Instruction<ArmInstruction, ArmRegister, ArmInstructionGroup, ArmInstructionDetail> instruction)
        {
            this.Internal = instruction;
            this.Address = Address.Ptr32((uint)instruction.Address);
        }

        public override int OpcodeAsInteger { get { return (int) Internal.Id; } }

        public Instruction<ArmInstruction, ArmRegister, ArmInstructionGroup, ArmInstructionDetail> Internal { get; private set; }
    }
}
