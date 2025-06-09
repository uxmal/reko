#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.WE32100
{
    public partial class WE32100Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly WE32100Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<WE32100Instruction> dasm;
        private readonly RtlEmitter m;
        private readonly List<RtlInstruction> rtls;
        private WE32100Instruction instr;
        private InstrClass iclass;

        public WE32100Rewriter(WE32100Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new WE32100Disassembler(arch, rdr).GetEnumerator();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    host.Error(instr.Address, $"WE32100 instruction '{instr}' is not supported yet.");
                    EmitUnitTest();
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    m.Invalid(); iclass = InstrClass.Invalid; break;
                case Mnemonic.addb2: RewriteArithmetic2(m.IAdd, PrimitiveType.Byte); break;
                case Mnemonic.addh2: RewriteArithmetic2(m.IAdd, PrimitiveType.Word16); break;
                case Mnemonic.addw2: RewriteArithmetic2(m.IAdd, PrimitiveType.Word32); break;
                case Mnemonic.addb3: RewriteArithmetic3(m.IAdd, PrimitiveType.Byte); break;
                case Mnemonic.addh3: RewriteArithmetic3(m.IAdd, PrimitiveType.Word16); break;
                case Mnemonic.addw3: RewriteArithmetic3(m.IAdd, PrimitiveType.Word32); break;
                case Mnemonic.andb2: RewriteLogical2(m.And, PrimitiveType.Byte); break;
                case Mnemonic.andh2: RewriteLogical2(m.And, PrimitiveType.Word16); break;
                case Mnemonic.andw2: RewriteLogical2(m.And, PrimitiveType.Word32); break;
                case Mnemonic.andb3: RewriteLogical3(m.And, PrimitiveType.Byte); break;
                case Mnemonic.andh3: RewriteLogical3(m.And, PrimitiveType.Word16); break;
                case Mnemonic.andw3: RewriteLogical3(m.And, PrimitiveType.Word32); break;
                case Mnemonic.dech: RewriteUnary(e => m.ISub(e, 1), PrimitiveType.Word16, NZVC); break;
                case Mnemonic.movb: RewriteMov(PrimitiveType.Byte); break;
                case Mnemonic.subb2: RewriteArithmetic2(m.ISub, PrimitiveType.Byte); break;
                case Mnemonic.subh2: RewriteArithmetic2(m.ISub, PrimitiveType.Word16); break;
                case Mnemonic.subw2: RewriteArithmetic2(m.ISub, PrimitiveType.Word32); break;
                case Mnemonic.xorb2: RewriteLogical2(m.Xor, PrimitiveType.Byte); break;
                case Mnemonic.xorh2: RewriteLogical2(m.Xor, PrimitiveType.Word16); break;
                case Mnemonic.xorw2: RewriteLogical2(m.Xor, PrimitiveType.Word32); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("WE32100rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Expression ReadOp(int iop, PrimitiveType dt)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage rop:
                return binder.EnsureRegister(rop);
            case Constant imm:
                return imm;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                if (mem.Deferred)
                {
                    ea = m.Mem(PrimitiveType.Ptr32, ea);
                }
                return m.Mem(dt, ea);
            default:
                throw new NotImplementedException($"Unsupported operand type {op.GetType().Name}.");
            }
        }

        private Expression? WriteOp(int iop, PrimitiveType dt, Expression src)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage rop:
                var reg = binder.EnsureRegister(rop);
                m.Assign(reg, src);
                return reg;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                if (mem.Deferred)
                {
                    ea = m.Mem(PrimitiveType.Ptr32, ea);
                }
                if (!(src is Identifier || src is Constant))
                {
                    var tmp = binder.CreateTemporary(dt);
                    m.Assign(tmp, src);
                    src = tmp;
                }
                m.Assign(m.Mem(dt, ea), src);
                return src;
            case Constant _:
            case Address _:
                iclass = InstrClass.Invalid;
                m.Invalid();
                return null;
            default:
                throw new NotImplementedException($"Unsupported operand type {op.GetType().Name}.");
            }
        }

        private Expression? WriteBinaryOp(int iop, PrimitiveType dt, Expression src, Func<Expression, Expression, Expression> binary)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage rop:
                var reg = binder.EnsureRegister(rop);
                m.Assign(reg, binary(reg, src));
                return src;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                if (mem.Deferred)
                {
                    ea = m.Mem(PrimitiveType.Ptr32, ea);
                }
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, binary(m.Mem(dt, ea), src));
                m.Assign(m.Mem(dt, ea), tmp);
                return tmp;
            case Constant _:
            case Address _:
                iclass = InstrClass.Invalid;
                m.Invalid();
                return null;
            default:
                throw new NotImplementedException($"Unsupported operand type {op.GetType().Name}.");
            }
        }

        private Expression? WriteUnaryOp(int iop, PrimitiveType dt, Expression src, Func<Expression, Expression> unary)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage rop:
                var reg = binder.EnsureRegister(rop);
                m.Assign(reg, unary(src));
                return src;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                if (mem.Deferred)
                {
                    ea = m.Mem(PrimitiveType.Ptr32, ea);
                }
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, unary(src));
                m.Assign(m.Mem(dt, ea), tmp);
                return tmp;
            case Constant _:
            case Address _:
                iclass = InstrClass.Invalid;
                m.Invalid();
                return null;
            default:
                throw new NotImplementedException($"Unsupported operand type {op.GetType().Name}.");
            }
        }

        private Expression EffectiveAddress(MemoryOperand mem)
        {
            Expression ea;
            if (mem.Base is not null)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.Offset != 0)
                {
                    ea = m.AddSubSignedInt(ea, mem.Offset);
                }
            }
            else
            {
                ea = Address.Ptr32((uint) mem.Offset);
            }
            return ea;
        }

        private void NZV0(Expression? e)
        {
            if (e is not null)
            {
                m.Assign(binder.EnsureFlagGroup(Registers.NZV), m.Cond(e));
                m.Assign(binder.EnsureFlagGroup(Registers.C), Constant.False());
            }
        }

        private void NZVC(Expression? e)
        {
            if (e is not null)
            {
                m.Assign(binder.EnsureFlagGroup(Registers.NZVC), m.Cond(e));
            }
        }
    }
}