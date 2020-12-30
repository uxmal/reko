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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Arch.zSeries
{
#pragma warning disable IDE1006

    public partial class zSeriesRewriter : IEnumerable<RtlInstructionCluster>
    {
        //$TODO: IBM defines a hexadecimal format; it's not IEEE 783
        private static readonly PrimitiveType ShortHexFloat = PrimitiveType.Real32;
        private static readonly PrimitiveType LongHexFloat = PrimitiveType.Real64;
        private static readonly PrimitiveType ExtendedHexFloat = PrimitiveType.Real128;

        private readonly zSeriesArchitecture arch;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<zSeriesInstruction> dasm;
        private readonly ExpressionValueComparer cmp;
        private zSeriesInstruction instr;
        private RtlEmitter m;
        private InstrClass iclass;

        public zSeriesRewriter(zSeriesArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new zSeriesDisassembler(arch, rdr).GetEnumerator();
            this.cmp = new ExpressionValueComparer();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                switch (instr.Mnemonic)
                {
                default:
                    host.Warn(instr.Address, "zSeries instruction {0} not implemented yet.", instr);
                    EmitUnitTest();
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.aghi: RewriteAhi2(PrimitiveType.Word64); break;
                case Mnemonic.aghik: RewriteAhi3(PrimitiveType.Word64); break;
                case Mnemonic.ahi: RewriteAhi2(PrimitiveType.Word32); break;
                case Mnemonic.ahik: RewriteAhi3(PrimitiveType.Word32); break;
                case Mnemonic.agr: RewriteAgr(); break;
                case Mnemonic.al: RewriteAl(PrimitiveType.Word32); break;
                case Mnemonic.ar: RewriteAr(); break;
                case Mnemonic.basr: RewriteBasr(); break;
                case Mnemonic.ber: RewriteBranch(ConditionCode.EQ); break;
                case Mnemonic.bler: RewriteBranch(ConditionCode.LE); break;
                case Mnemonic.bner: RewriteBranch(ConditionCode.NE); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.brasl: RewriteBrasl(); break;
                case Mnemonic.brctg: RewriteBrctg(); break;
                case Mnemonic.cghi: RewriteCghi(); break;
                case Mnemonic.cgij: RewriteCij(PrimitiveType.Int64); break;
                case Mnemonic.cgr: RewriteCgr(); break;
                case Mnemonic.cgrj: RewriteCrj(PrimitiveType.Int64); break;
                case Mnemonic.chi: RewriteChi(); break;
                case Mnemonic.cij: RewriteCij(PrimitiveType.Int32); break;
                case Mnemonic.clc: RewriteClc(); break;
                case Mnemonic.clg: RewriteClg(); break;
                case Mnemonic.clgij: RewriteClij(PrimitiveType.Word64); break;
                case Mnemonic.clgr: RewriteClgr(); break;
                case Mnemonic.clgrl: RewriteClgr(); break;
                case Mnemonic.clgrj: RewriteClrj(PrimitiveType.Word64); break;
                case Mnemonic.clfi: RewriteClfi(PrimitiveType.UInt32); break;
                case Mnemonic.cli: RewriteCli(); break;
                case Mnemonic.clij: RewriteClij(PrimitiveType.Word32); break;
                case Mnemonic.cs: RewriteCs(PrimitiveType.Word32); break;
                case Mnemonic.csg: RewriteCs(PrimitiveType.Word64); break;
                case Mnemonic.dp: RewriteDp(); break;
                case Mnemonic.ex: RewriteEx(); break;
                case Mnemonic.exrl: RewriteEx(); break;
                case Mnemonic.ic: RewriteIc(); break;
                case Mnemonic.j: RewriteJ(); break;
                case Mnemonic.je: RewriteJcc(ConditionCode.EQ); break;
                case Mnemonic.jg: RewriteJcc(ConditionCode.GT); break;
                case Mnemonic.jh: RewriteJcc(ConditionCode.UGT); break;
                case Mnemonic.jl: RewriteJcc(ConditionCode.LT); break;
                case Mnemonic.jle: RewriteJcc(ConditionCode.LE); break;
                case Mnemonic.jhe: RewriteJcc(ConditionCode.UGE); break;
                case Mnemonic.jne: RewriteJcc(ConditionCode.NE); break;
                case Mnemonic.la: RewriteLa(); break;
                case Mnemonic.larl: RewriteLarl(); break;
                case Mnemonic.l: RewriteL(PrimitiveType.Word32); break;
                case Mnemonic.lay: RewriteLay(); break;
                case Mnemonic.lcer: RewriteLcr(ShortHexFloat); break;
                case Mnemonic.ld: RewriteL(PrimitiveType.Word64); break;
                case Mnemonic.ldgr: RewriteLdgr(); break;
                case Mnemonic.lg: RewriteL(PrimitiveType.Word64); break;
                case Mnemonic.lgfrl: RewriteL(PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Mnemonic.lh: RewriteL(PrimitiveType.Word16); break;
                case Mnemonic.lhrl: RewriteL(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.lgdr: RewriteLgdr(); break;
                case Mnemonic.lgf: RewriteLgf(); break;
                case Mnemonic.lgfr: RewriteLgfr(); break;
                case Mnemonic.lghi: RewriteLghi(); break;
                case Mnemonic.lgr: RewriteLgr(); break;
                case Mnemonic.lgrl: RewriteLgrl(); break;
                case Mnemonic.lhi: RewriteLhi(); break;
                case Mnemonic.lmg: RewriteLmg(); break;
                case Mnemonic.lnr: RewriteLnr(PrimitiveType.Int32); break;
                case Mnemonic.locg: RewriteLoc(PrimitiveType.Word64, ConditionCode.ALWAYS); break;
                case Mnemonic.locgre: RewriteLoc(PrimitiveType.Word64, ConditionCode.EQ); break;
                case Mnemonic.locgrne: RewriteLoc(PrimitiveType.Word64, ConditionCode.NE); break;
                case Mnemonic.lpdr: RewriteLpr("fabs", LongHexFloat); break;
                case Mnemonic.lper: RewriteLpr("fabsf", ShortHexFloat); break;
                case Mnemonic.lpr: RewriteLpr("abs", PrimitiveType.Int32); break;
                case Mnemonic.lr: RewriteLr(); break;
                case Mnemonic.lrl: RewriteL(PrimitiveType.Word32); break;
                case Mnemonic.lt: RewriteLt(PrimitiveType.Word32); break;
                case Mnemonic.ltg: RewriteLt(PrimitiveType.Word64); break;
                case Mnemonic.ltgr: RewriteLtr(PrimitiveType.Word64); break;
                case Mnemonic.ltr: RewriteLtr(PrimitiveType.Word32); break;
                case Mnemonic.mvcle: RewriteMvcle(); break;
                case Mnemonic.mvhi: RewriteMvi(PrimitiveType.Word16); break;
                case Mnemonic.mvi: RewriteMvi(PrimitiveType.Byte); break;
                case Mnemonic.mvz: RewriteMvz(); break;
                case Mnemonic.nc: RewriteNc(); break;
                case Mnemonic.ngr: RewriteNgr(); break;
                case Mnemonic.ni: RewriteNi(); break;
                case Mnemonic.nopr: m.Nop(); break;
                case Mnemonic.oi: RewriteOi(); break;
                case Mnemonic.or: RewriteOr(PrimitiveType.Word32); break;
                case Mnemonic.risbg: RewriteRisbg("__risbg"); break;
                case Mnemonic.risbgn: RewriteRisbg("__risbgn"); break;
                case Mnemonic.s: RewriteS(PrimitiveType.Int32); break;
                case Mnemonic.sl: RewriteS(PrimitiveType.Word32); break;
                case Mnemonic.sdr: RewriteSdr(LongHexFloat); break;
                case Mnemonic.sgr: RewriteSub2(PrimitiveType.Int64); break;
                case Mnemonic.sla: RewriteShift2(PrimitiveType.Int32, m.Shl); break;
                case Mnemonic.sll: RewriteShift2(PrimitiveType.Word32, m.Shl); break;  //$TODO: CC's are handled unsigned.
                case Mnemonic.slr: RewriteSub2(PrimitiveType.Word32); break;
                case Mnemonic.sllg: RewriteShift3(PrimitiveType.Word64, m.Shl); break;
                case Mnemonic.sllk: RewriteShift3(PrimitiveType.Word32, m.Shl); break;
                case Mnemonic.srag: RewriteShift3(PrimitiveType.Int64, m.Sar); break;
                case Mnemonic.srl: RewriteShift2(PrimitiveType.Word32, m.Shr); break;
                case Mnemonic.srlg: RewriteShift3(PrimitiveType.Word64, m.Shr); break;
                case Mnemonic.srp: RewriteSrp(); break;
                case Mnemonic.std: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.ste: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.st: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.stg: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.sth: RewriteSt(PrimitiveType.Word16); break;
                case Mnemonic.stgrl: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.stmg: RewriteStmg(); break;
                case Mnemonic.sur: RewriteSur(ShortHexFloat); break;
                case Mnemonic.svc: RewriteSvc(); break;
                case Mnemonic.x: RewriteXor2(PrimitiveType.Word32); break;
                case Mnemonic.xc: RewriteXc(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void EmitUnitTest()
        {
            arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("zSeriesRw", dasm.Current, instr.Mnemonic.ToString(), rdr, "");
        }

        private Address Addr(int iop)
        {
            return ((AddressOperand)instr.Operands[iop]).Address;
        }

        private Expression Assign(Expression dst, Expression src)
        {
            var excessBits = dst.DataType.BitSize - src.DataType.BitSize;
            if (excessBits > 0)
            {
                if (!(src is Identifier || src is Constant))
                {
                    var tmp = binder.CreateTemporary(src.DataType);
                    m.Assign(tmp, src);
                    src = tmp;
                }
                var seq = m.Dpb(dst, src, 0);
                m.Assign(dst, seq);
                return seq.Expressions[1];
            }
            else
            {
                m.Assign(dst, src);
                return dst;
            }
        }

        private Constant Const(MachineOperand op)
        {
            return ((ImmediateOperand)op).Value;
        }

        private Expression EffectiveAddress(int iop)
        {
            switch (instr.Operands[iop])
            {
            case AddressOperand aOp:
                return aOp.Address;
            case MemoryOperand mem:
                return EffectiveAddress(mem);
            default:
                throw new InvalidOperationException($"Operand type {instr.Operands[iop].GetType().Name} cannot be an effective address.");
            }
        }

        private Expression EffectiveAddress(MemoryOperand mem)
        {
            if (mem.Base == null || mem.Base.Number == 0)
            {
                // Must be abs address.
                return Address.Create(arch.PointerType, (uint) mem.Offset);
            }
            Expression ea = binder.EnsureRegister(mem.Base);
            if (mem.Index != null && mem.Index.Number > 0)
            {
                var idx = binder.EnsureRegister(mem.Index);
                ea = m.IAdd(ea, idx);
            }
            if (mem.Offset != 0)
            {
                var off = Constant.Int(mem.Base.DataType, mem.Offset);
                ea = m.IAdd(ea, off);
            }
            return ea;
        }

        private Expression Op(int iop)
        {
            switch (instr.Operands[iop])
            {
            case RegisterOperand reg:
                return binder.EnsureRegister(reg.Register);
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mem:
                return new MemoryAccess(EffectiveAddress(mem), mem.Width);
            case AddressOperand addr:
                return addr.Address;
            default:
                throw new NotImplementedException($"{instr.Operands[iop].GetType().Name}");
            }
        }

        private Identifier FReg(int iOp)
        {
            var freg = Registers.FpRegisters[((RegisterOperand) instr.Operands[iOp]).Register.Number];
            return binder.EnsureRegister(freg);
        }

        private Constant Imm(int iop, DataType dt)
        {
            var c = ((ImmediateOperand) instr.Operands[iop]).Value;
            return Constant.Create(dt, c.ToUInt64());
        }

        private Address PcRel(int iOp)
        {
            var addr = ((AddressOperand) instr.Operands[iOp]).Address;
            return addr;
        }

        private Identifier Reg(int iOp)
        {
            return binder.EnsureRegister(((RegisterOperand)instr.Operands[iOp]).Register);
        }

        private Expression Rel(int iOp, PrimitiveType dt)
        {
            return m.Mem(dt, ((AddressOperand) instr.Operands[iOp]).Address);
        }

        private Identifier Reg(int iOp, DataType dt)
        {
            var reg = binder.EnsureRegister(((RegisterOperand) instr.Operands[iOp]).Register);
            if (reg.DataType.BitSize > dt.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(reg, dt, 0));
                return tmp;
            }
            else
            {
                return reg;
            }
        }

        private Identifier Seq(PrimitiveType dt, int iopHi, int iopLo)
        {
            var regHi = ((RegisterOperand) instr.Operands[iopHi]).Register;
            var regLo = ((RegisterOperand) instr.Operands[iopLo]).Register;
            return binder.EnsureSequence(dt, regHi, regLo);
        }
        private void SetCc(Expression e)
        {
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, e);
        }

        private Expression SignedCondition(int iop, Expression left, Expression right)
        {
            var mm = ((ImmediateOperand) instr.Operands[iop]).Value.ToInt32();
            return mm switch
            {
                2 => m.Gt(left, right),
                4 => m.Lt(left, right),
                6 => m.Ne(left, right),
                8 => m.Eq(left, right),
                0xA => m.Ge(left, right),
                0xC => m.Le(left, right),
                _ => Constant.False()
            };
        }

        private Expression UnsignedCondition(int iop, Expression left, Expression right)
        {
            var mm = ((ImmediateOperand) instr.Operands[iop]).Value.ToInt32();
            return mm switch
            {
                2 => m.Ugt(left, right),
                4 => m.Ult(left, right),
                6 => m.Ne(left, right),
                8 => m.Eq(left, right),
                0xA => m.Uge(left, right),
                0xC => m.Ule(left, right),
                _ => Constant.False()
            };
        }
    }
}