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
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.Etrax
{
    public class EtraxRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly EtraxArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<EtraxInstruction> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private EtraxInstruction instr;
        private InstrClass iclass;

        public EtraxRewriter(EtraxArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new EtraxDisassembler(arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instr = dasm.Current;
                iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.add: RewriteBinaryOp(m.IAdd, instr.DataWidth!, Registers.NZVC); break;
                case Mnemonic.addq: RewriteBinaryOp(m.IAdd, PrimitiveType.Word32, Registers.NZVC); break;
                case Mnemonic.and: RewriteLogicalOp(m.And, PrimitiveType.Word32, Registers.NZ); break;
                case Mnemonic.ba: RewriteBranchAlways(); break;
                case Mnemonic.beq: RewriteBranch(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.bne: RewriteBranch(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.bpl: RewriteBranch(ConditionCode.GE, Registers.N); break;
                case Mnemonic.btstq: RewriteBtst(); break;
                case Mnemonic.clearf: RewriteClearf(); break;
                case Mnemonic.cmp: RewriteCmp(); break;
                case Mnemonic.jsr: RewriteJsr(); break;
                case Mnemonic.jump: RewriteJump(); break;
                case Mnemonic.move: RewriteMove(); break;
                case Mnemonic.moveq: RewriteMove(); break;
                case Mnemonic.movu: RewriteMovu(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.or: RewriteLogicalOp(m.Or, PrimitiveType.Word32, Registers.NZ); break;
                case Mnemonic.sub: RewriteBinaryOp(m.ISub, instr.DataWidth!, Registers.NZVC); break;
                case Mnemonic.subq: RewriteBinaryOp(m.ISub, PrimitiveType.Word32, Registers.NZVC); break;
                case Mnemonic.swap: RewriteSwap(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                instrs.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("EtraxRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private void EmitCc(FlagGroupStorage flags, Expression src)
        {
            var dst = binder.EnsureFlagGroup(flags);
            m.Assign(dst, m.Cond(src));
        }

        private void Clear(FlagGroupStorage flags)
        {
            var dst = binder.EnsureFlagGroup(flags);
            m.Assign(dst, 0);
        }


        private Expression DstOp(
            int iop, 
            DataType dt, 
            Expression src,
            Func<Expression, Expression, Expression>? func)
        {
            Expression dst;
            switch (instr.Operands[iop])
            {
            case RegisterStorage reg:
                dst = binder.EnsureRegister(reg);
                if (func is not null)
                {
                    src = func(dst, src);
                }
                m.Assign(dst, src);
                break;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                dst = m.Mem(dt, ea);
                if (func is not null)
                {
                    var tmp = binder.CreateTemporary(dt);
                    m.Assign(tmp, func(dst, src));
                    src = tmp;
                }
                m.Assign(dst, src);
                if (mem.PostIncrement)
                {
                    m.Assign(ea, m.IAddS(ea, dt.Size));
                }
                dst = src;
                break;
            default: throw new NotImplementedException($"Rewriting of Etrax operand {instr.Operands[iop].GetType()} is not implemented yet.");
            }
            return dst;
        }

        private Expression SrcOp(int iop, DataType dt)
        {
            return SrcOp(instr.Operands[iop], dt);
        }

        private Expression SrcOp(MachineOperand op, DataType dt)
        {
            switch (op)
            {
            case RegisterStorage r: return binder.EnsureRegister(r);
            case Constant imm: return imm;
            case Address addr: return addr;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                if (mem.PostIncrement)
                {
                    var tmp = binder.CreateTemporary(dt);
                    m.Assign(tmp, m.Mem(dt, ea));
                    m.Assign(ea, m.IAddS(ea, dt.Size));
                    return tmp;
                }
                else
                {
                    return m.Mem(dt, ea);
                }
            default: throw new NotImplementedException($"Rewriting of Etrax operand {op.GetType()} is not implemented yet.");
            }
        }

        Expression EffectiveAddress(MemoryOperand mem)
        {
            Expression ea = SrcOp(mem.Base!, PrimitiveType.Word32);
            return ea;
        }

        private void RewriteBinaryOp(
            Func<Expression, Expression, Expression> fn,
            PrimitiveType dt,
            FlagGroupStorage grf)
        {
            var src = SrcOp(0, dt);
            var dst = DstOp(1, dt, src, fn);
            EmitCc(grf, dst);
        }

        private void RewriteBranch(ConditionCode cc, FlagGroupStorage grf)
        {
            var test = m.Test(cc, binder.EnsureFlagGroup(grf));
            var dst = (Address)instr.Operands[0];
            m.Branch(test, dst, instr.InstructionClass);
        }
        
        private void RewriteBranchAlways()
        {
            var dst = (Address)instr.Operands[0];
            m.GotoD(dst);
        }

        private void RewriteBtst()
        {
            var bit = SrcOp(0, PrimitiveType.Word32);
            var src = SrcOp(1, PrimitiveType.Word32);
            var n = binder.EnsureFlagGroup(Registers.N);
            var z = binder.EnsureFlagGroup(Registers.Z);
            m.Assign(n, m.And(src, m.Shl(Constant.UInt32(1), bit)));
            m.Assign(z, m.And(src, m.ISub(
                m.Shl(Constant.UInt32(1), m.IAdd(bit, 1)),
                1)));
        }

        private void RewriteClearf()
        {
            var grfOp = (FlagGroupStorage) instr.Operands[0];
            foreach (var grfBit in arch.GetSubFlags(grfOp))
            {
                var id = binder.EnsureFlagGroup(grfBit);
                m.Assign(id, 0);
            }
        }

        private void RewriteCmp()
        {
            var right = SrcOp(0, instr.DataWidth!);
            var left = SrcOp(1, instr.DataWidth!);
            var flags = binder.EnsureFlagGroup(Registers.NZVC);
            m.Assign(flags, m.Cond(m.ISub(left, right)));
        }

        private void RewriteJsr()
        {
            var dst = SrcOp(0, PrimitiveType.Ptr32);
            m.Call(dst, 0);
        }

        private void RewriteJump()
        {
            var dst = SrcOp(0, PrimitiveType.Ptr32);
            m.Goto(dst);
        }

        private void RewriteLogicalOp(
            Func<Expression, Expression, Expression> fn,
            PrimitiveType dt,
            FlagGroupStorage grf)
        {
            var src = SrcOp(0, dt);
            var dst = DstOp(1, dt, src, fn);
            EmitCc(grf, dst);
            Clear(Registers.V);
            Clear(Registers.C);
        }

        private void RewriteMove()
        {
            var src = SrcOp(0, instr.DataWidth!);
            var dst = DstOp(1, instr.DataWidth!, src, null);
            EmitCc(Registers.NZ, dst);
            Clear(Registers.V);
            Clear(Registers.C);
        }

        private void RewriteMovu()
        {
            var src = SrcOp(0, PrimitiveType.UInt16);
            var conv = m.Convert(src, src.DataType, PrimitiveType.UInt32);
            var dst = DstOp(1, instr.DataWidth!, conv, null);
            EmitCc(Registers.Z, dst);
            Clear(Registers.N);
            Clear(Registers.V);
            Clear(Registers.C);
        }

        private void RewriteSwap()
        {
            if (instr.SwapBits == 0x8)
            {
                var src = SrcOp(0, PrimitiveType.Word32);
                m.Assign(src, m.Not(src));
                EmitCc(Registers.NZ, src);
                Clear(Registers.V);
                Clear(Registers.C);
            }
            else
            {
                throw new NotImplementedException();
                //if ((SwapBits & 0x8) != 0) sb.Append('n');
                //if ((SwapBits & 0x4) != 0) sb.Append('w');
                //if ((SwapBits & 0x2) != 0) sb.Append('b');
                //if ((SwapBits & 0x1) != 0) sb.Append('r');
            }
        }
    }
}
