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
using System.Text;

namespace Reko.Arch.Altera.Nios2
{
    public class Nios2Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Nios2Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Nios2Instruction> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private Nios2Instruction instr;
        private InstrClass iclass;

        public Nios2Rewriter(Nios2Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Nios2Disassembler(arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
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
                    EmitUnitTest();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    iclass = InstrClass.Invalid; m.Invalid();
                    break;
                case Mnemonic.add: RewriteAddSub(m.IAdd); break;
                case Mnemonic.addi: RewriteAddi(); break;
                case Mnemonic.and: RewriteAnd(); break;
                case Mnemonic.andhi: RewriteAndi(16); break;
                case Mnemonic.andi: RewriteAndi(0); break;
                case Mnemonic.beq: RewriteBranch(m.Eq); break;
                case Mnemonic.bge: RewriteBranch(m.Ge); break;
                case Mnemonic.bgeu: RewriteBranch(m.Uge); break;
                case Mnemonic.blt: RewriteBranch(m.Lt); break;
                case Mnemonic.bltu: RewriteBranch(m.Ult); break;
                case Mnemonic.bne: RewriteBranch(m.Ne); break;
                case Mnemonic.br: RewriteJump(Addr(0)); break;
                case Mnemonic.call: RewriteCall(Addr(0)); break;
                case Mnemonic.cmpltu: RewriteCmp(m.Ult); break;
                case Mnemonic.flushd: RewriteFlushd(); break;
                case Mnemonic.flushi: RewriteFlushi(); break;
                case Mnemonic.flushp: RewriteFlushp(); break;
                case Mnemonic.initd: RewriteInitd(); break;
                case Mnemonic.initi: RewriteIniti(); break;
                case Mnemonic.jmp: RewriteJump(Reg0(0)); break;
                case Mnemonic.jmpi: RewriteJump(Addr(0)); break;
                case Mnemonic.ldb: RewriteLoad(PrimitiveType.Int8, PrimitiveType.Int32); break;
                case Mnemonic.ldbu: RewriteLoad(PrimitiveType.Byte, PrimitiveType.Word32); break;
                case Mnemonic.ldhu: RewriteLoad(PrimitiveType.Word16, PrimitiveType.Word32); break;
                case Mnemonic.ldw: RewriteLoad(PrimitiveType.Word32, PrimitiveType.Word32); break;
                case Mnemonic.nextpc: RewriteNextpc(); break;
                case Mnemonic.nor: RewriteNor(); break;
                case Mnemonic.or: RewriteOrXor(m.Or); break;
                case Mnemonic.orhi: RewriteOrXori(m.Or, 16); break;
                case Mnemonic.ori: RewriteOrXori(m.Or, 0); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.sll: RewriteShift(m.Shl); break;
                case Mnemonic.slli: RewriteShiftI(m.Shl); break;
                case Mnemonic.sra: RewriteShift(m.Sar); break;
                case Mnemonic.srai: RewriteShiftI(m.Sar); break;
                case Mnemonic.srl: RewriteShift(m.Shr); break;
                case Mnemonic.srli: RewriteShiftI(m.Shr); break;
                case Mnemonic.stb: RewriteStore(PrimitiveType.Byte); break;
                case Mnemonic.sth: RewriteStore(PrimitiveType.Word16); break;
                case Mnemonic.stw: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.sub: RewriteAddSub(m.ISub); break;
                case Mnemonic.wrctl: RewriteWrctl(); break;
                case Mnemonic.xor: RewriteOrXor(m.Xor); break;
                case Mnemonic.xorhi: RewriteOrXori(m.Xor, 16); break;
                case Mnemonic.xori: RewriteOrXori(m.Xor, 0); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                instrs.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private void EmitUnitTest()
        {
            arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("Nios2Rw", dasm.Current, dasm.Current.Mnemonic.ToString(), rdr, "");
        }

        private void EmitNop()
        {
            iclass = InstrClass.Linear | InstrClass.Padding;
            m.Nop();
        }

        private Address Addr(int iop) => ((AddressOperand) instr.Operands[iop]).Address;

        private Constant Imm(int iop) => ((ImmediateOperand)instr.Operands[iop]).Value;

        private MemoryAccess Mem(int iop, DataType dt)
        {
            var mem = (MemoryOperand) instr.Operands[iop];
            Expression ea = binder.EnsureRegister(mem.Base);
            ea = m.AddSubSignedInt(ea, mem.Offset);
            return m.Mem(dt, ea);
        }

        private RegisterStorage Reg(int iop) => ((RegisterOperand)instr.Operands[iop]).Register;

        private Expression Reg0(int iop)
        {
            var reg = ((RegisterOperand) instr.Operands[iop]).Register;
            if (reg.Number == 0)
                return Constant.Word32(0);
            else
                return binder.EnsureRegister(reg);
        }

        private Expression MaybeConvert(Expression e, DataType dtFrom, DataType dtTo)
        {
            if (dtFrom.BitSize < dtTo.BitSize)
            {
                return m.Convert(e, dtFrom, dtTo);
            }
            else
            {
                return e;
            }
        }
        private void RewriteAddSub(Func<Expression,Expression,Expression> fn)
        {
            var rc = Reg(0);
            if (rc.Number == 0)
            {
                EmitNop();
                return;
            }
            var dst = binder.EnsureRegister(rc);
            var ra = Reg(1);
            var rb = Reg(2);
            if (ra.Number == 0)
            {
                if (rb.Number == 0)
                {
                    m.Assign(dst, 0);
                }
                else
                {
                    m.Assign(dst, binder.EnsureRegister(rb));
                }
            }
            else if (rb.Number == 0)
            {
                m.Assign(dst, binder.EnsureRegister(ra));
            }
            else
            {
                m.Assign(dst, fn(binder.EnsureRegister(ra), binder.EnsureRegister(rb)));
            }
        }

        private void RewriteAddi()
        {
            var rb = Reg(0);
            if (rb.Number == 0)
            {
                EmitNop();
                return;
            }
            var dst = binder.EnsureRegister(rb);
            var ra = Reg(1);
            var imm = Imm(2).ToInt32();
            if (ra.Number == 0)
            {
                m.Assign(dst, m.Word32(imm));
            }
            else
            {
                var src = binder.EnsureRegister(ra);
                m.Assign(dst, m.AddSubSignedInt(src, imm));
            }
        }

        private void RewriteAnd()
        {
            var d = Reg(0);
            if (d.Number == 0)
            {
                EmitNop();
                return;
            }
            var a = Reg0(1);
            var b = Reg0(2);
            m.Assign(binder.EnsureRegister(d), m.And(a, b));
        }

        private void RewriteAndi(int shift)
        {
            var rb = Reg(0);
            if (rb.Number == 0)
            {
                EmitNop();
                return;
            }
            var dst = binder.EnsureRegister(rb);
            var ra = Reg(1);
            var imm = Imm(2).ToUInt32() << shift;
            if (ra.Number == 0)
            {
                m.Assign(dst, m.Word32(0));
            }
            else
            {
                var src = binder.EnsureRegister(ra);
                m.Assign(dst, m.And(src, m.Word32(imm)));
            }
        }

        private void RewriteBranch(Func<Expression,Expression,Expression> fn)
        {
            var a = Reg0(0);
            var b = Reg0(1);
            var dest = Addr(2);
            m.Branch(fn(a, b), dest);
        }

        private void RewriteCall(Expression dst)
        {
            m.Call(dst, 0);
        }

        private void RewriteCmp(Func<Expression,Expression,Expression> fn)
        {
            var d = Reg(0);
            if (d.Number == 0)
            {
                EmitNop();
                return;
            }
            var a = Reg0(1);
            var b = Reg0(2);
            m.Assign(
                binder.EnsureRegister(d),
                m.Convert(fn(a, b), PrimitiveType.Bool, d.DataType));
        }

        private void RewriteFlushd()
        {
            m.SideEffect(host.Intrinsic("__flushd", true, VoidType.Instance, m.AddrOf(PrimitiveType.Ptr32, Mem(0, PrimitiveType.Word32))));
        }

        private void RewriteFlushi()
        {
            m.SideEffect(host.Intrinsic("__flushi", true, VoidType.Instance, m.AddrOf(PrimitiveType.Ptr32, Mem(0, PrimitiveType.Word32))));
        }

        private void RewriteFlushp()
        {
            m.SideEffect(host.Intrinsic("__flushp", true, VoidType.Instance));
        }

        private void RewriteInitd()
        {
            m.SideEffect(host.Intrinsic("__initd", true, VoidType.Instance, m.AddrOf(PrimitiveType.Ptr32, Mem(0, PrimitiveType.Word32))));
        }

        private void RewriteIniti()
        {
            m.SideEffect(host.Intrinsic("__initi", true, VoidType.Instance, Reg0(0)));
        }

        private void RewriteJump(Expression dst)
        {
            m.Goto(dst);
        }

        private void RewriteLoad(DataType dtMem, DataType dtReg)
        {
            var src = Mem(1, dtMem);
            var dst = binder.EnsureRegister(Reg(0));
            m.Assign(dst, MaybeConvert(src, dtMem, dtReg));
        }

        private void RewriteNextpc()
        {
            var d = Reg(0);
            if (d.Number == 0)
            {
                EmitNop();
                return;
            }
            m.Assign(binder.EnsureRegister(d), instr.Address + 4);
        }

        private void RewriteNor()
        {
            var d = Reg(0);
            if (d.Number == 0)
            {
                EmitNop();
                return;
            }
            var a = Reg0(1);
            var b = Reg0(2);
            Expression src;
            if (a.IsZero)
                src = b;
            else if (b.IsZero)
                src = a;
            else
                src = m.Or(a, b);
            m.Assign(binder.EnsureRegister(d), m.Comp(src));
        }

        private void RewriteOrXor(Func<Expression, Expression,Expression> fn)
        {
            var d = Reg(0);
            if (d.Number == 0)
            {
                EmitNop();
                return;
            }
            var a = Reg0(1);
            var b = Reg0(2);
            Expression src;
            if (a.IsZero)
                src = b;
            else if (b.IsZero)
                src = a;
            else
                src = fn(a, b);
            m.Assign(binder.EnsureRegister(d), src);
        }

        private void RewriteOrXori(Func<Expression,Expression,Expression> fn, int shift)
        {
            var rb = Reg(0);
            if (rb.Number == 0)
            {
                EmitNop();
                return;
            }
            var dst = binder.EnsureRegister(rb);
            var ra = Reg(1);
            var imm = Imm(2).ToUInt32() << shift;
            if (ra.Number == 0)
            {
                m.Assign(dst, m.Word32(imm));
            }
            else
            {
                var src = binder.EnsureRegister(ra);
                m.Assign(dst, fn(src, m.Word32(imm)));
            }
        }

        private void RewriteRet()
        {
            m.Return(0, 0);
        }

        private void RewriteShift(Func<Expression,Expression,Expression> shift)
        {
            var d = Reg(0);
            if (d.Number == 0)
            {
                EmitNop();
                return;
            }
            var a = Reg0(1);
            var b = Reg0(2);
            Expression src;
            if (b is Constant)
            {
                src = a;
            }
            else if (a is Constant)
            {
                src = b;
            }
            else
            {
                src = shift(a, b);
            }
            m.Assign(binder.EnsureRegister(d), src);
        }

        private void RewriteShiftI(Func<Expression, Expression, Expression> shift)
        {
            var d = Reg(0);
            if (d.Number == 0)
            {
                EmitNop();
                return;
            }
            var a = Reg0(1);
            var b = Imm(2);
            Expression src;
            if (b.IsZero)
            {
                src = a;
            }
            else if (a.IsZero)
            {
                src = b;
            }
            else
            {
                src = shift(a, b);
            }
            m.Assign(binder.EnsureRegister(d), src);
        }
        private void RewriteStore(DataType dt)
        {
            Expression src;
            var reg = Reg(0);
            if (reg.Number == 0)
            {
                src = Constant.Create(dt, 0);
            }
            else
            {
                src = binder.EnsureRegister(reg);
                if (dt.BitSize < instr.Operands[0].Width.BitSize)
                {
                    var tmp = binder.CreateTemporary(dt);
                    m.Assign(tmp, m.Slice(src, dt, 0));
                    src = tmp;
                }
            }
            m.Assign(Mem(1, dt), src);
        }

        private void RewriteWrctl()
        {
            m.SideEffect(host.Intrinsic("__wrctl", true, VoidType.Instance, Reg0(0), Reg0(1)));
        }
    }
}
