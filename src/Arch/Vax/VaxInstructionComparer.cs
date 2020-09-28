#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.Vax
{
    internal class VaxInstructionComparer : InstructionComparer
    {
        public VaxInstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (VaxInstruction)x;
            var b = (VaxInstruction)y;
            if (a.Operands.Length != b.Operands.Length)
                return false;
            for (int i = 0; i < a.Operands.Length; ++i)
            {
                if (!Compare(a.Operands[i], b.Operands[i]))
                    return false;
            }
            return true;
        }

        private bool Compare(MachineOperand a, MachineOperand b)
        {
            if (a.GetType() != b.GetType())
                return false;
            var rA = a as RegisterOperand;
            if (rA != null)
            {
                if (NormalizeRegisters)
                    return true;
                var rB = b as RegisterOperand;
                return rA.Register == rB.Register;
            }
            throw new NotImplementedException();
        }

        public override int GetOperandsHash(MachineInstruction vInstr)
        {
            var instr = (VaxInstruction)vInstr;
            int h = 0;
            for (int i = 0; i<instr.Operands.Length;++i)
            {
                h = h * 17 ^ instr.Operands[i].GetType().GetHashCode();
            }
            return h;
        }
    }
}