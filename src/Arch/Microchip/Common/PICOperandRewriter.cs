#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Core;
using System;

namespace Reko.Arch.Microchip.Common
{
    public abstract class PICOperandRewriter
    {
        protected readonly PICArchitecture arch;
        private readonly ExpressionEmitter m;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;

        public PICOperandRewriter(PICArchitecture arch, ExpressionEmitter emitter, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.m = emitter;
            this.binder = binder;
            this.host = host;
        }

        public Expression Transform(PICInstruction instr, MachineOperand op, PrimitiveType opWidth, PICProcessorState state)
        {
            switch (op)
            {
                case PICOperandRegister reg:
                    return binder.EnsureRegister(reg.Register);

                case PICOperandImmediate imm:
                    return CreateConstant(imm, imm.Width);

                case PICOperandProgMemoryAddress prog:
                    return prog.CodeTarget;

                case PICOperandOffsetBankedMemory data:
                    return CreateMemoryAccess(instr, data, opWidth, state);


            }
            throw new NotImplementedException($"Operand {op}");
        }

        public Constant CreateConstant(PICOperandImmediate imm, PrimitiveType dataWidth)
        {
            if (dataWidth.BitSize > imm.Width.BitSize)
                return Constant.Create(dataWidth, imm.ImmediateValue.ToInt64());
            else
                return Constant.Create(imm.Width, imm.ImmediateValue.ToUInt32());
        }

        public Expression CreateMemoryAccess(PICInstruction instr, PICOperandOffsetBankedMemory mem, DataType dt, PICProcessorState state)
        {
            //            Expression expr = EffectiveAddressExpression(instr, mem, state);
            //            if (IsSegmentedAccessRequired ||
            //                (mem.DefaultSegment != Registers.ds && mem.DefaultSegment != Registers.ss))
            //            {
            //                Expression seg = ReplaceCodeSegment(mem.DefaultSegment, state);
            //                if (seg == null)
            //                    seg = AluRegister(mem.DefaultSegment);
            //                return new BankMemoryAccess(MemoryIdentifier.GlobalMemory, seg, expr, dt);
            //            }
            //            else
            //            {
            //                return new MemoryAccess(MemoryIdentifier.GlobalMemory, expr, dt);
            //            }
            return null; 
        }

        private Expression ImportedGlobal(Address addrInstruction, PICOperandOffsetBankedMemory data)
        {
            if (data != null && data.IsAbsolute)
                return host.GetImport(Address.Ptr32(data.DataTarget.ToUInt32()), addrInstruction);
            return null;
        }

        private ExternalProcedure ImportedProcedure(Address addrInstruction, PICOperandProgMemoryAddress prog)
        {
            if (prog != null)
                return host.GetImportedProcedure(Address.Ptr32(prog.CodeTarget.ToUInt32()), addrInstruction);
            return null;
        }

    }

}
