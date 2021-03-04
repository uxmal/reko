#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

namespace Reko.Arch.Blackfin
{
    public class BlackfinRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly BlackfinArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<BlackfinInstruction> dasm;
        private RtlEmitter m;
        private InstrClass iclass;
        private BlackfinInstruction instr;

        public BlackfinRewriter(BlackfinArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new BlackfinDisassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest(instr);
                    iclass = InstrClass.Invalid;
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    this.iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.CLI: RewriteCli(); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.mov_x: RewriteMovx(); break;
                case Mnemonic.mov_xb: RewriteMovxb(); break;
                case Mnemonic.mov_zb: RewriteMovzb(); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.JUMP_L: RewriteJump(); break;
                case Mnemonic.RTN: RewriteRtn(); break;
                case Mnemonic.RTS: RewriteRts(); break;
                case Mnemonic.sub3: RewriteBinop(m.ISub); break;
                case Mnemonic.xor3: RewriteBinop(m.Xor); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest(BlackfinInstruction instr)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("BlackfinRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Address Addr(int iOperand)
        {
            return ((AddressOperand) instr.Operands[iOperand]).Address;
        }

        private Expression SrcOperand(int iOperand)
        {
            switch (instr.Operands[iOperand])
            {
            case RegisterOperand rop:
                return binder.EnsureRegister(rop.Register);
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                return m.Mem(mem.Width, ea);
            case RegisterRange range:
                return ExtendedRegister(range);
            default:
                throw new NotImplementedException($"Operand type {instr.Operands[iOperand].GetType().Name}.");
            }
        }

        private Expression DstOperand(int iOperand, Expression src)
        {
            switch (instr.Operands[iOperand])
            {
            case RegisterOperand rop:
                var dst = binder.EnsureRegister(rop.Register);
                m.Assign(dst, src);
                return dst;
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                m.Assign(m.Mem(mem.Width, ea), src);
                return src;
            case RegisterRange range:
                var extReg = ExtendedRegister(range);
                m.Assign(extReg, src);
                return extReg;
            default:
                throw new NotImplementedException($"Operand type {instr.Operands[iOperand].GetType().Name}.");
            }
        }

        private Expression EffectiveAddress(MemoryOperand mem)
        {
            Expression ea;
            if (mem.Base != null)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.Index != null)
                    throw new NotImplementedException();
                if (mem.Offset != 0)
                {
                    ea = m.AddSubSignedInt(ea, mem.Offset);
                }
                if (mem.PostDecrement || mem.PostIncrement)
                {
                    var tmp = binder.CreateTemporary(ea.DataType);
                    m.Assign(tmp, ea);
                    var size = mem.Width.Size;
                    m.Assign(ea, m.AddSubSignedInt(ea, mem.PostIncrement
                        ? size
                        : -size));
                    ea = tmp;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            return ea;
        }

        private Identifier ExtendedRegister(RegisterRange range)
        {
            int nRegs = range.MaxRegister - range.MinRegister + 1;
            var regSequence = new Storage[nRegs];
            int bitsize = 0;
            for (int i = 0; i < nRegs; ++i)
            {
                var reg = range.Registers[range.MaxRegister - i];
                regSequence[i] = reg;
                bitsize += reg.DataType.BitSize; 
            }
            var dt = PrimitiveType.CreateWord(bitsize);
            return binder.EnsureSequence(dt, regSequence);
        }

        private Identifier Reg(int iOperand)
        {
            return binder.EnsureRegister(((RegisterOperand) instr.Operands[iOperand]).Register);
        }

        private void RewriteCli()
        {
            m.SideEffect(host.Intrinsic("__cli", false, VoidType.Instance));
        }

        private void RewriteJump()
        {
            var addrDst = Addr(0);
            m.Goto(addrDst);
        }

        private void RewriteRts()
        {
            // A more accurate rewriter would assign PC = RETS
            m.Return(0, 0);
        }

        private void RewriteMov()
        {
            var src = SrcOperand(1);
            DstOperand(0, src);
        }

        private void RewriteMovx()
        {
            var src = SrcOperand(1);
            var from = PrimitiveType.Create(Domain.SignedInt, src.DataType.BitSize);
            m.Assign(Reg(0), m.Convert(src, from, PrimitiveType.Int32));
        }

        private void RewriteMovxb()
        {
            var src = SrcOperand(1);
            m.Assign(Reg(0), m.Convert(m.Slice(PrimitiveType.SByte, src, 0), PrimitiveType.SByte, PrimitiveType.Int32));
        }

        private void RewriteMovzb()
        {
            var src = SrcOperand(1);
            m.Assign(Reg(0), m.Convert(m.Slice(PrimitiveType.Byte, src, 0), PrimitiveType.Byte, PrimitiveType.Word32));
        }

        private void RewriteMul()
        {
            Debug.Assert(instr.Operands.Length == 2);
            var dst = Reg(0);
            var src = Reg(1);
            m.Assign(dst, m.IMul(dst, src));
        }

        private void RewriteRtn()
        {
            // A more accurate rewriter would assign PC = RETN
            m.Return(0, 0);
        }

        private void RewriteBinop(Func<Expression,Expression,Expression> binop)
        {
            var src1 = Reg(1);
            var src2 = Reg(2);
            var dst = Reg(0);
            m.Assign(dst, binop(src1, src2));
        }
    }
}