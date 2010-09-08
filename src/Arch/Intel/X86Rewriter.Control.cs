#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Intel
{
    public partial class X86Rewriter
    {
        private void RewriteJmp()
        {
            if (IsRealModeReboot(di.Instruction))
			{
                throw new NotImplementedException();
                //PseudoProcedure reboot = host.EnsurePseudoProcedure("__bios_reboot", PrimitiveType.Void, 0);
                //reboot.Characteristics = new Decompiler.Core.Serialization.ProcedureCharacteristics();
                //reboot.Characteristics.Terminates = true;
                //emitter.SideEffect(reboot);
				return;
			}

				
			if (di.Instruction.op1 is ImmediateOperand)
			{
				Address addr = OperandAsCodeAddress(di.Instruction.op1);
                emitter.Goto(addr);
				return;
			}
            throw new NotImplementedException();
        }

        private bool IsRealModeReboot(IntelInstruction instrCur)
        {
            // A jumps to 0xFFFF:0x0000 in real mode is a reboot.
            AddressOperand addrOp = instrCur.op1 as AddressOperand;
            bool isRealModeReboot = addrOp != null && addrOp.Address.Linear == 0xFFFF0;
            return isRealModeReboot;
        }

        public Address OperandAsCodeAddress(MachineOperand op)
        {
            AddressOperand ado = op as AddressOperand;
            if (ado != null)
                return ado.Address;
            ImmediateOperand imm = op as ImmediateOperand;
            if (imm != null)
            {
                if (arch.ProcessorMode == ProcessorMode.ProtectedFlat)
                {
                    return new Address(imm.Value.ToUInt32());
                }
                else
                    return new Address(di.Address.Selector, imm.Value.ToUInt32());
            }
            return null;
        }

    }
}
