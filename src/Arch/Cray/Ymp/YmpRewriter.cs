#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Arch.Cray.Ymp
{
    public class YmpRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly CrayYmpArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<CrayInstruction> dasm;
        private CrayInstruction instrCur;
        private InstrClass iclass;
        private RtlEmitter m;

        public YmpRewriter(CrayYmpArchitecture arch, Decoder<YmpDisassembler, Mnemonic, CrayInstruction> decoder, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new YmpDisassembler(arch, decoder, rdr).GetEnumerator();
            this.instrCur = null!;
            this.m = null!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instrCur = dasm.Current;
                this.iclass = instrCur.InstructionClass;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                switch (instrCur.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    this.iclass = InstrClass.Invalid;
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    m.Invalid();
                    break;
                case Mnemonic.j: RewriteJ(); break;
                case Mnemonic.jan: RewriteJxx(Registers.ARegs[0], m.Ne0); break;
                case Mnemonic.jam: RewriteJxx(Registers.ARegs[0], m.Lt0); break;
                case Mnemonic.jap: RewriteJxx(Registers.ARegs[0], m.Ge0); break;
                case Mnemonic.jaz: RewriteJxx(Registers.ARegs[0], m.Eq0); break;
                case Mnemonic.jsn: RewriteJxx(Registers.SRegs[0], m.Ne0); break;
                case Mnemonic.jsm: RewriteJxx(Registers.SRegs[0], m.Lt0); break;
                case Mnemonic.jsp: RewriteJxx(Registers.SRegs[0], m.Ge0); break;
                case Mnemonic.jsz: RewriteJxx(Registers.SRegs[0], m.Eq0); break;
                case Mnemonic.r: RewriteR(); break;
                case Mnemonic._and: Rewrite3(m.And); break;
                case Mnemonic._clz: RewriteIntrinsic("__clz", false); break;
                case Mnemonic._popcnt: RewriteIntrinsic("__popcnt", false); break;
                case Mnemonic._fmul: Rewrite3(m.FMul); break;
                case Mnemonic._iadd: Rewrite3(m.IAdd); break;
                case Mnemonic._isub: Rewrite3(m.ISub); break;
                case Mnemonic._mov: RewriteMov(); break;
                case Mnemonic._movz: RewriteMovz(); break;
                case Mnemonic._lmask: RewriteLmask(); break;
                case Mnemonic._load: RewriteLoad(); break;
                case Mnemonic._lsl: RewriteShift(m.Shl); break;
                case Mnemonic._lsr: RewriteShift(m.Shr); break;
                case Mnemonic._vor: Rewrite3(m.Or); break;
                case Mnemonic._store: RewriteStore(); break;
                case Mnemonic._xor: Rewrite3(m.Xor); break;
                }
                yield return m.MakeCluster(instrCur.Address, instrCur.Length, iclass);
            }
        }

        private void RewriteIntrinsic(string intrinsicName, bool hasSideEffect)
        {
            var dst = Op(0);
            var args = Enumerable.Range(1, instrCur.Operands.Length - 1)
                .Select(i => Op(i))
                .ToArray();
            m.Assign(dst, host.Intrinsic(intrinsicName, hasSideEffect, dst.DataType, args));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("YmpRw", this.instrCur, instrCur.Mnemonic.ToString(), rdr, "", YmpDisassembler.Octize);
        }

        private Expression Op(int iop)
        {
            switch (instrCur.Operands[iop])
            {
            case RegisterStorage rop:
                var reg = this.binder.EnsureRegister(rop);
                return reg;
            case ImmediateOperand imm:
                return imm.Value;
            case AddressOperand addr:
                return addr.Address;
            default:
                throw new NotImplementedException($"Unimplemented Cray operand {instrCur.Operands[iop].GetType().Name}.");
            }
        }

        private void Rewrite3(Func<Expression, Expression, Expression> fn)
        {
            var dst = Op(0);
            var src1 = Op(1);
            var src2 = Op(2);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteJ()
        {
            var dst = Op(0);
            m.Goto(dst);
        }

        private void RewriteJxx(RegisterStorage reg, Func<Expression, BinaryExpression> test)
        {
            m.Branch(test(binder.EnsureRegister(reg)), (Address)Op(0));
        }

        private void RewriteLmask()
        {
            var bits = ((ImmediateOperand) instrCur.Operands[1]).Value;
            var mask = Bits.Mask(0, bits.ToInt32());
            var dst = Op(0);
            m.Assign(dst, Constant.Create(dst.DataType, mask));
        }

        private void RewriteLoad()
        {
            var ea = Op(1);
            if (instrCur.Operands.Length == 3)
            {
                ea = m.IAdd(ea, Op(2));
            }
            var dst = Op(0);
            m.Assign(dst, m.Mem(dst.DataType, ea));
        }

        private void RewriteShift(Func<Expression,Expression,Expression> shift)
        {
            Expression src;
            if (instrCur.Operands.Length == 2)
            {
                src = shift(Op(0), Op(1));
            }
            else
            {
                src = shift(Op(1), Op(2));
            }
            var dst = Op(0);
            m.Assign(dst, src);
        }

        private void RewriteMov()
        {
            var src1 = Op(1);
            var dst = Op(0);
            if (instrCur.Operands.Length == 2)
            {
                m.Assign(dst, src1);
            }
            else
            {
                var src2 = Op(2);
                m.Assign(dst, m.ARef(PrimitiveType.Word64, src1, src2));
            }
        }

        private void RewriteMovz()
        {
            var src = Op(1);
            var dst = Op(0);
            m.Assign(dst, m.Convert(src, src.DataType, dst.DataType));
        }

        private void RewriteR()
        {
            var dst = Op(0);
            m.Call(dst, 0);
        }

        private void RewriteStore()
        {
            Expression ea;
            Expression src;
            if (instrCur.Operands.Length == 2)
            {
                src = Op(1);
                ea = Op(0);
            }
            else
            {
                src = Op(2);
                ea = m.IAdd(Op(1), Op(0));
            }
            m.Assign(m.Mem(src.DataType, ea), src);
        }
    }
}
