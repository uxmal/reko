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

using Decompiler.Arch.PowerPC;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompiler.Environments.Ps3
{
    /// <summary>
    /// Platform support for Sony Playstation 3 (PS3).
    /// </summary>
    public class Ps3Platform : Platform
    {
        public Ps3Platform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch) 
        {
        }

        public override string DefaultCallingConvention { get { return ""; } }

        public override PrimitiveType PointerType { get { return PrimitiveType.Pointer32; } }

        public override BitSet CreateImplicitArgumentRegisters()
        {
            return Architecture.CreateRegisterBitset();
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override ProcedureBase GetTrampolineDestination(ImageReader rdr, IRewriterHost host)
        {
            var dasm = new PowerPcDisassembler(
                (PowerPcArchitecture64) Architecture,
                rdr,
                PrimitiveType.Word64);
            PowerPcInstruction instr;
            ImmediateOperand immOp;
            MemoryOperand memOp;

            //addi r12,r0,0000
            instr = dasm.DisassembleInstruction();
            if (instr.Opcode != Opcode.addi)
                return null;

            //oris r12,r12,0006
            instr = dasm.DisassembleInstruction();
            if (instr.Opcode != Opcode.oris)
                return null;
            immOp = (ImmediateOperand) instr.op3;
            uint aFuncDesc = immOp.Value.ToUInt32() << 16;

            //lwz r12,nnnn(r12)
            instr = dasm.DisassembleInstruction();
            if (instr.Opcode != Opcode.lwz)
                return null;
            memOp = (MemoryOperand)instr.op2;
            int offset = memOp.Offset.ToInt32();
            aFuncDesc = (uint)(aFuncDesc + offset);

            //std r2,40(r1)
            instr = dasm.DisassembleInstruction();
            if (instr.Opcode != Opcode.std)
                return null;

            //lwz r0,0(r12)
            // Have a pointer to a trampoline
            instr = dasm.DisassembleInstruction();
            if (instr.Opcode != Opcode.lwz)
                return null;

            //lwz r2,4(r12)
            instr = dasm.DisassembleInstruction();
            if (instr.Opcode != Opcode.lwz)
                return null;

            // mtctr r0
            instr = dasm.DisassembleInstruction();
            if (instr.Opcode != Opcode.mtctr)
                return null;

            // bcctr 14,00
            instr = dasm.DisassembleInstruction();
            if (instr.Opcode != Opcode.bcctr)
                return null;

            // Read the function pointer from the function descriptor.

            offset = (int)aFuncDesc - (int)rdr.Address.ToUInt32();
            rdr.Offset = (ulong) (((long)rdr.Offset) + offset);
            var aFn = rdr.ReadUInt32();
            return null;
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            // Bizarrely, pointers are 32-bit on this 64-bit platform.
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address MakeAddressFromLinear(ulong uAddr)
        {
            return Address.Ptr32((uint)uAddr);
        }

    }
}
