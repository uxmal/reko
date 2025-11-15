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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Arch.MN103
{
    public class MN103Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly MN103Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<MN103Instruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private MN103Instruction instr;
        private InstrClass iclass;

        public MN103Rewriter(MN103Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new MN103Disassembler(arch, rdr).GetEnumerator();
            this.instr = default!;
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
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
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.add: RewriteAdd(); break;
                case Mnemonic.addc: RewriteAddc(); break;
                case Mnemonic.and: RewriteAnd(); break;
                case Mnemonic.asl: RewriteShift(Operator.Shl); break;
                case Mnemonic.asl2: RewriteAsl2(); break;
                case Mnemonic.asr: RewriteShift(Operator.Sar); break;
                case Mnemonic.bcc: RewriteBcc(ConditionCode.UGE, Registers.C); break;
                case Mnemonic.bcs: RewriteBcc(ConditionCode.ULT, Registers.C); break;
                case Mnemonic.beq: RewriteBcc(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.bge: RewriteBcc(ConditionCode.GE, Registers.VN); break;
                case Mnemonic.bgt: RewriteBcc(ConditionCode.GT, Registers.VNZ); break;
                case Mnemonic.bhi: RewriteBcc(ConditionCode.UGT, Registers.CZ); break;
                case Mnemonic.ble: RewriteBcc(ConditionCode.LE, Registers.VNZ); break;
                case Mnemonic.bls: RewriteBcc(ConditionCode.ULE, Registers.CZ); break;
                case Mnemonic.blt: RewriteBcc(ConditionCode.LT, Registers.VN); break;
                case Mnemonic.bnc: RewriteBcc(ConditionCode.GE, Registers.N); break;
                case Mnemonic.bns: RewriteBcc(ConditionCode.LT, Registers.N); break;
                case Mnemonic.bne: RewriteBcc(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.bvc: RewriteBcc(ConditionCode.NO, Registers.V); break;
                case Mnemonic.bvs: RewriteBcc(ConditionCode.OV, Registers.V); break;
                case Mnemonic.bra: RewriteBra(); break;
                case Mnemonic.bclr: RewriteBclr(); break;
                case Mnemonic.bset: RewriteBset(); break;
                case Mnemonic.btst: RewriteBtst(); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.calls: RewriteCalls(); break;
                case Mnemonic.clr: RewriteClr(); break;
                case Mnemonic.cmp: RewriteCmp(); break;
                case Mnemonic.div: RewriteDiv(Operator.SDiv, Operator.SMod, PrimitiveType.Int32); break;
                case Mnemonic.divu: RewriteDiv(Operator.UDiv, Operator.UMod, PrimitiveType.UInt32); break;
                case Mnemonic.ext: RewriteExt(); break;
                case Mnemonic.extb: RewriteExtend(PrimitiveType.Byte, PrimitiveType.Int32); break;
                case Mnemonic.extbu: RewriteExtend(PrimitiveType.Byte, PrimitiveType.UInt32); break;
                case Mnemonic.exth: RewriteExtend(PrimitiveType.Word16, PrimitiveType.Int32); break;
                case Mnemonic.exthu: RewriteExtend(PrimitiveType.Word16, PrimitiveType.UInt32); break;
                case Mnemonic.inc: RewriteInc(); break;
                case Mnemonic.inc4: RewriteInc4(); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.lcc: RewriteLcc(ConditionCode.UGE, Registers.CZ); break;
                case Mnemonic.lcs: RewriteLcc(ConditionCode.ULT, Registers.C); break;
                case Mnemonic.leq: RewriteLcc(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.lge: RewriteLcc(ConditionCode.GE, Registers.VN); break;
                case Mnemonic.lgt: RewriteLcc(ConditionCode.GT, Registers.VNZ); break;
                case Mnemonic.lhi: RewriteLcc(ConditionCode.UGT, Registers.CZ); break;
                case Mnemonic.lle: RewriteLcc(ConditionCode.LE, Registers.VNZ); break;
                case Mnemonic.lls: RewriteLcc(ConditionCode.ULE, Registers.CZ); break;
                case Mnemonic.llt: RewriteLcc(ConditionCode.LT, Registers.VN); break;
                case Mnemonic.lne: RewriteLcc(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.lra: RewriteLra(); break;
                case Mnemonic.lsr: RewriteShift(Operator.Shr); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movbu: RewriteMovUnequal(PrimitiveType.Byte, PrimitiveType.UInt32); break;
                case Mnemonic.movhu: RewriteMovUnequal(PrimitiveType.Word16, PrimitiveType.UInt32); break;
                case Mnemonic.movm: RewriteMovm(); break;
                case Mnemonic.mul: RewriteMul(PrimitiveType.Int64); break;
                case Mnemonic.mulu: RewriteMul(PrimitiveType.UInt64); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.not: RewriteNot(); break;
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.retf: RewriteRetf(); break;
                case Mnemonic.rets: RewriteRets(); break;
                case Mnemonic.rol: RewriteRol(); break;
                case Mnemonic.rti: RewriteRti(); break;
                case Mnemonic.setlb: RewriteSetlb(); break;
                case Mnemonic.sub: RewriteSub(); break;
                case Mnemonic.subc: RewriteSubc(); break;
                case Mnemonic.syscall: RewriteSyscall(); break;
                case Mnemonic.trap: RewriteTrap(); break;
                case Mnemonic.xor: RewriteXor(); break;

                case Mnemonic.udf00: RewriteUdf(udf00_intrinsic); break;
                case Mnemonic.udf01: RewriteUdf(udf01_intrinsic); break;
                case Mnemonic.udf02: RewriteUdf(udf02_intrinsic); break;
                case Mnemonic.udf03: RewriteUdf(udf03_intrinsic); break;
                case Mnemonic.udf04: RewriteUdf(udf04_intrinsic); break;
                case Mnemonic.udf05: RewriteUdf(udf05_intrinsic); break;
                case Mnemonic.udf06: RewriteUdf(udf06_intrinsic); break;
                case Mnemonic.udf07: RewriteUdf(udf07_intrinsic); break;
                case Mnemonic.udf08: RewriteUdf(udf08_intrinsic); break;
                case Mnemonic.udf09: RewriteUdf(udf09_intrinsic); break;
                case Mnemonic.udf10: RewriteUdf(udf10_intrinsic); break;
                case Mnemonic.udf11: RewriteUdf(udf11_intrinsic); break;
                case Mnemonic.udf12: RewriteUdf(udf12_intrinsic); break;
                case Mnemonic.udf13: RewriteUdf(udf13_intrinsic); break;
                case Mnemonic.udf14: RewriteUdf(udf14_intrinsic); break;
                case Mnemonic.udf15: RewriteUdf(udf15_intrinsic); break;
                case Mnemonic.udf20: RewriteUdf(udf20_intrinsic); break;
                case Mnemonic.udf21: RewriteUdf(udf21_intrinsic); break;
                case Mnemonic.udf22: RewriteUdf(udf22_intrinsic); break;
                case Mnemonic.udf23: RewriteUdf(udf23_intrinsic); break;
                case Mnemonic.udf24: RewriteUdf(udf24_intrinsic); break;
                case Mnemonic.udf25: RewriteUdf(udf25_intrinsic); break;
                case Mnemonic.udf26: RewriteUdf(udf26_intrinsic); break;
                case Mnemonic.udf27: RewriteUdf(udf27_intrinsic); break;
                case Mnemonic.udf28: RewriteUdf(udf28_intrinsic); break;
                case Mnemonic.udf29: RewriteUdf(udf29_intrinsic); break;
                case Mnemonic.udf30: RewriteUdf(udf30_intrinsic); break;
                case Mnemonic.udf31: RewriteUdf(udf31_intrinsic); break;
                case Mnemonic.udf32: RewriteUdf(udf32_intrinsic); break;
                case Mnemonic.udf33: RewriteUdf(udf33_intrinsic); break;
                case Mnemonic.udf34: RewriteUdf(udf34_intrinsic); break;
                case Mnemonic.udf35: RewriteUdf(udf35_intrinsic); break;

                case Mnemonic.udfu00: RewriteUdf(udfu00_intrinsic); break;
                case Mnemonic.udfu01: RewriteUdf(udfu01_intrinsic); break;
                case Mnemonic.udfu02: RewriteUdf(udfu02_intrinsic); break;
                case Mnemonic.udfu03: RewriteUdf(udfu03_intrinsic); break;
                case Mnemonic.udfu04: RewriteUdf(udfu04_intrinsic); break;
                case Mnemonic.udfu05: RewriteUdf(udfu05_intrinsic); break;
                case Mnemonic.udfu06: RewriteUdf(udfu06_intrinsic); break;
                case Mnemonic.udfu07: RewriteUdf(udfu07_intrinsic); break;
                case Mnemonic.udfu08: RewriteUdf(udfu08_intrinsic); break;
                case Mnemonic.udfu09: RewriteUdf(udfu09_intrinsic); break;
                case Mnemonic.udfu10: RewriteUdf(udfu10_intrinsic); break;
                case Mnemonic.udfu11: RewriteUdf(udfu11_intrinsic); break;
                case Mnemonic.udfu12: RewriteUdf(udfu12_intrinsic); break;
                case Mnemonic.udfu13: RewriteUdf(udfu13_intrinsic); break;
                case Mnemonic.udfu14: RewriteUdf(udfu14_intrinsic); break;
                case Mnemonic.udfu15: RewriteUdf(udfu15_intrinsic); break;
                case Mnemonic.udfu20: RewriteUdf(udfu20_intrinsic); break;
                case Mnemonic.udfu21: RewriteUdf(udfu21_intrinsic); break;
                case Mnemonic.udfu22: RewriteUdf(udfu22_intrinsic); break;
                case Mnemonic.udfu23: RewriteUdf(udfu23_intrinsic); break;
                case Mnemonic.udfu24: RewriteUdf(udfu24_intrinsic); break;
                case Mnemonic.udfu25: RewriteUdf(udfu25_intrinsic); break;
                case Mnemonic.udfu26: RewriteUdf(udfu26_intrinsic); break;
                case Mnemonic.udfu27: RewriteUdf(udfu27_intrinsic); break;
                case Mnemonic.udfu28: RewriteUdf(udfu28_intrinsic); break;
                case Mnemonic.udfu29: RewriteUdf(udfu29_intrinsic); break;
                case Mnemonic.udfu30: RewriteUdf(udfu30_intrinsic); break;
                case Mnemonic.udfu31: RewriteUdf(udfu31_intrinsic); break;
                case Mnemonic.udfu32: RewriteUdf(udfu32_intrinsic); break;
                case Mnemonic.udfu33: RewriteUdf(udfu33_intrinsic); break;
                case Mnemonic.udfu34: RewriteUdf(udfu34_intrinsic); break;
                case Mnemonic.udfu35: RewriteUdf(udfu35_intrinsic); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EmitUnitTest()
        {
            host.Warn(
                instr.Address,
                "MN103 instruction '{0}' is not supported yet.",
                instr.Mnemonic);
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Mn103Rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private void Emit_0001()
        {
            m.Assign(binder.EnsureFlagGroup(Registers.VCN), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), (int) FlagM.ZF);
        }

        private void Emit_000Z(Expression exp)
        {
            m.Assign(binder.EnsureFlagGroup(Registers.Z), exp);
            m.Assign(binder.EnsureFlagGroup(Registers.VCN), 0);
        }

        private void Emit_00NZ(Expression exp)
        {
            var grf = binder.EnsureFlagGroup(Registers.NZ);
            m.Assign(grf, exp);
            m.Assign(binder.EnsureFlagGroup(Registers.VC), 0);
        }

        private void Emit__CNZ(Expression exp)
        {
            var grf = binder.EnsureFlagGroup(Registers.CNZ);
            m.Assign(grf, exp);
        }

        private void Emit_VCNZ(Expression exp)
        {
            var grf = binder.EnsureFlagGroup(Registers.VCNZ);
            m.Assign(grf, exp);
        }

        private Expression Op(int iop, DataType dtMem)
        {
            var op = instr.Operands[iop]; 
            switch (op)
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case Address addr:
                return addr;
            case Constant imm:
                var n = imm.ToInt32();    // sign extend.
                return m.Const(dtMem, n);
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                return m.Mem(dtMem, ea);
            default:
                throw new NotImplementedException($"OPerand type {op.GetType().Name}.");
            }
        }

        private Expression EffectiveAddress(MemoryOperand mem)
        {
            if (mem.Base is null)
                return Address.Ptr32((uint) mem.Displacement);
            Expression ea = binder.EnsureRegister(mem.Base);
            if (mem.Index is null)
            {
                if (mem.Displacement != 0)
                {
                    ea = m.IAddS(ea, mem.Displacement);
                }
            }
            else
            {
                ea = m.IAdd(ea, binder.EnsureRegister(mem.Index));
            }
            return ea;
        }

        private Identifier Reg(int iop)
        {
            return binder.EnsureRegister((RegisterStorage) instr.Operands[iop]);
        }

        private void RewriteAdd()
        {
            var right = Op(0, PrimitiveType.Word32);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.IAdd(left, right));
            Emit_VCNZ(m.Cond(Registers.VCNZ.DataType, left));
        }

        private void RewriteAddc()
        {
            var right = Op(0, PrimitiveType.Word32);
            var cy = binder.EnsureFlagGroup(Registers.C);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.IAddC(left, right, cy));
            Emit_VCNZ(m.Cond(Registers.VCNZ.DataType, left));
        }

        private void RewriteAnd()
        {
            var dt = instr.Operands[1].DataType;
            var right = Op(0, dt);
            var left = Op(1, dt);
            m.Assign(left, m.And(left, right));
            Emit_00NZ(m.Cond(Registers.VCNZ.DataType, left));
        }

        private void RewriteAsl2()
        {
            var id = Op(0, PrimitiveType.Word32);
            m.Assign(id, m.Shl(id, 2));
        }

        private void RewriteBcc(ConditionCode cc, FlagGroupStorage grf)
        {
            var target = (Address) instr.Operands[0];
            var flags = binder.EnsureFlagGroup(grf);
            m.Branch(m.Test(cc, flags), target);
        }

        private void RewriteBra()
        {
            var target = (Address) instr.Operands[0];
            m.Goto(target);
        }

        private void RewriteBclr()
        {
            var mask = Op(0, PrimitiveType.Word32);
            var e = Op(1, PrimitiveType.Byte);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, e);
            Emit_000Z(m.Cond(
                Registers.VCNZ.DataType, 
                m.And(
                    m.Convert(tmp, tmp.DataType, PrimitiveType.Word32),
                    mask)));
            m.Assign(e, m.Comp(tmp));
        }

        private void RewriteBset()
        {
            var mask = Op(0, PrimitiveType.Word32);
            var e = Op(1, PrimitiveType.Byte);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, e);
            Emit_000Z(m.Cond(
                Registers.VCNZ.DataType,
                m.And(
                    m.Convert(tmp, tmp.DataType, PrimitiveType.Word32),
                    mask)));
            m.Assign(e, m.Or(tmp, mask));
        }

        private void RewriteBtst()
        {
            var mask = Op(0, PrimitiveType.Word32);
            var e = Op(1, PrimitiveType.Byte);
            Emit_00NZ(m.And(e, mask));
        }

        private void RewriteCall()
        {
            var target = (Address) instr.Operands[0];
            var disp8 = (Constant) instr.Operands[2];

            SaveRegisters((MultipleRegistersOperand) instr.Operands[1], disp8.ToInt32());
            m.Call(target, 0);
        }

        private void RewriteCalls()
        {
            var mdr = binder.EnsureRegister(Registers.mdr);
            m.Assign(mdr, instr.Address + instr.Length);
            m.Call(Op(0, PrimitiveType.Word32), 4);
        }

        private void RestoreRegisters(MultipleRegistersOperand regs, int disp8)
        {
            var sp = binder.EnsureRegister(Registers.sp);
            if (disp8 != 0)
            {
                m.Assign(sp, m.IAdd(sp, (byte) disp8));
            }
            foreach (var reg in regs.GetRegisters().Reverse())
            {
                Expression e = binder.EnsureRegister(reg);
                m.Assign(e, m.Mem(e.DataType, sp));
                m.Assign(sp, m.IAddS(sp, 4));
            }
        }
        private void SaveRegisters(MultipleRegistersOperand regs, int disp8)
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, sp);
            foreach (var reg in regs.GetRegisters())
            {
                m.Assign(tmp, m.ISubS(tmp, 4));
                Expression e = binder.EnsureRegister(reg);
                if (e.DataType.BitSize < 32)
                {
                    e = m.Convert(e, e.DataType, PrimitiveType.UInt32);
                }
                m.Assign(m.Mem32(tmp), e);
            }
            if (disp8 != 0)
            {
                m.Assign(sp, m.ISub(sp, (byte) disp8));
            }
        }

        private void RewriteClr()
        {
            var reg = Op(0, PrimitiveType.Word32);
            m.Assign(reg, 0);
            Emit_0001();
        }

        private void RewriteCmp()
        {
            var right = Op(0, PrimitiveType.Word32);
            var left = Op(1, PrimitiveType.Word32);
            Emit_VCNZ(m.Cond(Registers.VCNZ.DataType, m.ISub(left, right)));
        }

        private void RewriteDiv(BinaryOperator div, BinaryOperator mod, PrimitiveType dtQuot)
        {
            var right = Reg(0);
            var left = Reg(1);
            var dividend = binder.EnsureSequence(PrimitiveType.Word64, Registers.mdr, left.Storage);
            m.Assign(left, m.Bin(div, dtQuot, dividend, right));
            m.Assign(right, m.Bin(mod, dtQuot, dividend, right));
            Emit_VCNZ(m.Cond(Registers.VCNZ.DataType, left));
        }

        private void RewriteExt()
        {
            var reg = (RegisterStorage) instr.Operands[0];
            var src = binder.EnsureRegister(reg);
            var dst = binder.EnsureSequence(PrimitiveType.Word64, Registers.mdr, reg);
            m.Assign(dst, m.Convert(src, src.DataType, PrimitiveType.Int64));
        }

        private void RewriteExtend(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var id = Op(0, dtFrom);
            var tmp = binder.CreateTemporary(dtFrom);
            m.Assign(tmp, m.Slice(id, dtFrom));
            m.Assign(id, m.Convert(tmp, tmp.DataType, dtTo));
        }

        private void RewriteInc()
        {
            var id = Op(0, PrimitiveType.Word32);
            m.Assign(id, m.IAdd(id, 1));
            Emit_VCNZ(m.Cond(Registers.VCNZ.DataType, id));
        }

        private void RewriteInc4()
        {
            var id = Op(0, PrimitiveType.Word32);
            m.Assign(id, m.IAdd(id, 4));
        }

        private void RewriteJmp()
        {
            m.Goto(Op(0, PrimitiveType.Word32));
        }

        private void RewriteLcc(ConditionCode cc, FlagGroupStorage grf)
        {
            var flags = binder.EnsureFlagGroup(grf);
            var lar = state.GetValue(Registers.lar);
            if (lar is Constant c && c.IsValid)
            {
                var addr = arch.MakeAddressFromConstant(c, true);
                m.Branch(m.Test(cc, flags), addr);
            }
            else
            {
                m.BranchInMiddleOfInstruction(m.Not(m.Test(cc, flags)), instr.Address + instr.Length, InstrClass.ConditionalTransfer);
                m.Goto(binder.EnsureRegister(Registers.lar));
            }
        }

        private void RewriteLra()
        {
            //$TODO: manual is hard to read.
            iclass = InstrClass.Invalid;
            m.Invalid();
        }

        private void RewriteShift(BinaryOperator op)
        {
            var right = Op(0, PrimitiveType.Word32);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.Bin(op, left, right));
            Emit__CNZ(m.Cond(Registers.VCNZ.DataType, left));
        }

        private void RewriteMov()
        {
            var right = Op(0, PrimitiveType.Word32);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, right);
            if (left is Identifier id && id.Storage is RegisterStorage reg &&
                reg == Registers.psw)
            {
                Emit_VCNZ(id);
            }
        }

        private void RewriteMovUnequal(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var right = Op(0, dtFrom);
            var tmp = binder.CreateTemporary(dtFrom);
            var left = Op(1, dtFrom);
            if (left is Identifier id)
            {
                m.Assign(tmp, right);
                m.Assign(id, m.Convert(tmp, dtFrom, dtTo));
            }
            else
            {
                m.Assign(tmp, m.Slice(right, dtFrom));
                m.Assign(left, tmp);
            }
        }

        private void RewriteMovm()
        {
            if (instr.Operands[0] is MultipleRegistersOperand regs)
            {
                SaveRegisters(regs, 0);
            }
            else
            {
                regs = (MultipleRegistersOperand) instr.Operands[1];
                RestoreRegisters(regs, 0);
            }
        }

        private void RewriteMul(PrimitiveType dtResult)
        {
            var right = Op(0, PrimitiveType.Word32);
            var left = Reg(1);
            var product = binder.EnsureSequence(dtResult,
                Registers.mdr,
                left.Storage);
            m.Assign(product, m.IMul(dtResult, left, right));
            Emit_VCNZ(product);
        }

        private void RewriteNot()
        {
            var id = Reg(0);
            m.Assign(id, m.Comp(id));
            Emit_00NZ(m.Cond(Registers.NZ.DataType, id));
        }

        private void RewriteOr()
        {
            var dt = instr.Operands[1].DataType;
            var right = Op(0, dt);
            var left = Op(1, dt);
            m.Assign(left, m.Or(left, right));
            Emit_00NZ(m.Cond(Registers.NZ.DataType, left));
        }

        private void RewriteRet()
        {
            int extra = ((Constant) instr.Operands[1]).ToInt32();
            RestoreRegisters(
                (MultipleRegistersOperand) instr.Operands[0],
                extra);
            m.Return(4, extra);
        }

        private void RewriteRetf()
        {
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, binder.EnsureRegister(Registers.mdr));
            RestoreRegisters(
                (MultipleRegistersOperand) instr.Operands[0],
                ((Constant) instr.Operands[1]).ToInt32());
            m.Goto(tmp);
        }

        private void RewriteRets()
        {
            m.Return(0, 0);
        }

        private void RewriteRol()
        {
            var reg = Op(0, PrimitiveType.Word32);
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(reg, m.Fn(CommonOps.RolC.MakeInstance(reg.DataType, PrimitiveType.Byte),
                reg, m.Byte(1), c));
            m.Assign(binder.EnsureFlagGroup(Registers.CNZ), m.Cond(Registers.CNZ.DataType, reg));
            m.Assign(binder.EnsureFlagGroup(Registers.V), 0);
        }

        private void RewriteRti()
        {
            m.SideEffect(m.Fn(rti_intrinsic));
            m.Return(0, 0);
        }

        private void RewriteSetlb()
        {
            m.Assign(binder.EnsureRegister(Registers.lir), m.Mem32(instr.Address + 1));
            m.Assign(binder.EnsureRegister(Registers.lar), instr.Address + instr.Length);
        }

        private void RewriteSub()
        {
            var right = Op(0, PrimitiveType.Word32);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.ISub(left, right));
            Emit_VCNZ(m.Cond(Registers.VCNZ.DataType, left));
        }

        private void RewriteSubc()
        {
            var right = Op(0, PrimitiveType.Word32);
            var cy = binder.EnsureFlagGroup(Registers.C);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.ISubC(left, right, cy));
            Emit_VCNZ(m.Cond(Registers.VCNZ.DataType, left));
        }

        private void RewriteSyscall()
        {
            if (instr.Operands.Length != 0)
            {
                m.SideEffect(m.Fn(CommonOps.Syscall_1));
            }
            else
            {
                m.SideEffect(m.Fn(CommonOps.Syscall_0));
            }
        }

        private void RewriteTrap()
        {
            m.SideEffect(m.Fn(CommonOps.Syscall));
        }

        private void RewriteUdf(IntrinsicProcedure intrinsic)
        {
            var src = Op(0, PrimitiveType.Word32);
            var dst = Op(1, PrimitiveType.Word32);
            m.Assign(dst, m.Fn(intrinsic, dst, src));
        }

        private void RewriteXor()
        {
            var right = Op(0, PrimitiveType.Word32);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.Xor(left, right));
            Emit_00NZ(left);
        }

        private static readonly IntrinsicProcedure movm_intrinsic = IntrinsicBuilder.SideEffect("** MOVM instruction decoding is not clear from documentation")
            .Void();

        private static readonly IntrinsicProcedure rti_intrinsic = IntrinsicBuilder.SideEffect("__return_from_interrupt")
            .Void();

        private static readonly IntrinsicProcedure udf00_intrinsic = new IntrinsicBuilder("udf00", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf01_intrinsic = new IntrinsicBuilder("udf01", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf02_intrinsic = new IntrinsicBuilder("udf02", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf03_intrinsic = new IntrinsicBuilder("udf03", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf04_intrinsic = new IntrinsicBuilder("udf04", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf05_intrinsic = new IntrinsicBuilder("udf05", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf06_intrinsic = new IntrinsicBuilder("udf06", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf07_intrinsic = new IntrinsicBuilder("udf07", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf08_intrinsic = new IntrinsicBuilder("udf08", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf09_intrinsic = new IntrinsicBuilder("udf09", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf10_intrinsic = new IntrinsicBuilder("udf10", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf11_intrinsic = new IntrinsicBuilder("udf11", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf12_intrinsic = new IntrinsicBuilder("udf12", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf13_intrinsic = new IntrinsicBuilder("udf13", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf14_intrinsic = new IntrinsicBuilder("udf14", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf15_intrinsic = new IntrinsicBuilder("udf15", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf20_intrinsic = new IntrinsicBuilder("udf20", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf21_intrinsic = new IntrinsicBuilder("udf21", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf22_intrinsic = new IntrinsicBuilder("udf22", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf23_intrinsic = new IntrinsicBuilder("udf23", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf24_intrinsic = new IntrinsicBuilder("udf24", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf25_intrinsic = new IntrinsicBuilder("udf25", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf26_intrinsic = new IntrinsicBuilder("udf26", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf27_intrinsic = new IntrinsicBuilder("udf27", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf28_intrinsic = new IntrinsicBuilder("udf28", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf29_intrinsic = new IntrinsicBuilder("udf29", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf30_intrinsic = new IntrinsicBuilder("udf30", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf31_intrinsic = new IntrinsicBuilder("udf31", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf32_intrinsic = new IntrinsicBuilder("udf32", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf33_intrinsic = new IntrinsicBuilder("udf33", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf34_intrinsic = new IntrinsicBuilder("udf34", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udf35_intrinsic = new IntrinsicBuilder("udf35", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);

        private static readonly IntrinsicProcedure udfu00_intrinsic = new IntrinsicBuilder("udfu00", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu01_intrinsic = new IntrinsicBuilder("udfu01", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu02_intrinsic = new IntrinsicBuilder("udfu02", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu03_intrinsic = new IntrinsicBuilder("udfu03", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu04_intrinsic = new IntrinsicBuilder("udfu04", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu05_intrinsic = new IntrinsicBuilder("udfu05", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu06_intrinsic = new IntrinsicBuilder("udfu06", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu07_intrinsic = new IntrinsicBuilder("udfu07", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu08_intrinsic = new IntrinsicBuilder("udfu08", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu09_intrinsic = new IntrinsicBuilder("udfu09", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu10_intrinsic = new IntrinsicBuilder("udfu10", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu11_intrinsic = new IntrinsicBuilder("udfu11", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu12_intrinsic = new IntrinsicBuilder("udfu12", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu13_intrinsic = new IntrinsicBuilder("udfu13", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu14_intrinsic = new IntrinsicBuilder("udfu14", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu15_intrinsic = new IntrinsicBuilder("udfu15", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu20_intrinsic = new IntrinsicBuilder("udfu20", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu21_intrinsic = new IntrinsicBuilder("udfu21", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu22_intrinsic = new IntrinsicBuilder("udfu22", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu23_intrinsic = new IntrinsicBuilder("udfu23", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu24_intrinsic = new IntrinsicBuilder("udfu24", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu25_intrinsic = new IntrinsicBuilder("udfu25", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu26_intrinsic = new IntrinsicBuilder("udfu26", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu27_intrinsic = new IntrinsicBuilder("udfu27", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu28_intrinsic = new IntrinsicBuilder("udfu28", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu29_intrinsic = new IntrinsicBuilder("udfu29", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu30_intrinsic = new IntrinsicBuilder("udfu30", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu31_intrinsic = new IntrinsicBuilder("udfu31", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu32_intrinsic = new IntrinsicBuilder("udfu32", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu33_intrinsic = new IntrinsicBuilder("udfu33", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu34_intrinsic = new IntrinsicBuilder("udfu34", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure udfu35_intrinsic = new IntrinsicBuilder("udfu35", true).Param(PrimitiveType.Word32).Param(PrimitiveType.Word32).Returns(PrimitiveType.Word32);

    }
}