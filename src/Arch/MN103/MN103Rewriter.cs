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
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
                case Mnemonic.setlb: RewriteSetlb(); break;
                case Mnemonic.sub: RewriteSub(); break;
                case Mnemonic.subc: RewriteSubc(); break;
                case Mnemonic.xor: RewriteXor(); break;
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

        private Expression Op(int iop, PrimitiveType dtMem)
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
                return Constant.Word32(n);
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
            Emit_VCNZ(m.Cond(left));
        }

        private void RewriteAddc()
        {
            var right = Op(0, PrimitiveType.Word32);
            var cy = binder.EnsureFlagGroup(Registers.C);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.IAddC(left, right, cy));
            Emit_VCNZ(m.Cond(left));
        }

        private void RewriteAnd()
        {
            var right = Op(0, PrimitiveType.Word32);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.And(left, right));
            Emit_00NZ(m.Cond(left));
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
            foreach (var reg in regs.GetRegisters())
            {
                m.Assign(sp, m.ISubS(sp, 4));
                Expression e = binder.EnsureRegister(reg);
                if (e.DataType.BitSize < 32)
                {
                    e = m.Convert(e, e.DataType, PrimitiveType.UInt32);
                }
                m.Assign(m.Mem32(sp), e);
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
            Emit_VCNZ(m.Cond(m.ISub(left, right)));
        }

        private void RewriteDiv(BinaryOperator div, BinaryOperator mod, PrimitiveType dtQuot)
        {
            var right = Reg(0);
            var left = Reg(1);
            var dividend = binder.EnsureSequence(PrimitiveType.Word64, Registers.mdr, left.Storage);
            m.Assign(left, m.Bin(div, dtQuot, dividend, right));
            m.Assign(right, m.Bin(mod, dtQuot, dividend, right));
            Emit_VCNZ(m.Cond(left));
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
            Emit_VCNZ(m.Cond(id));
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
            //$TODO: manual is hard to read.
            //var target = (Address) instr.Operands[0];
            //var flags = binder.EnsureFlagGroup(grf);
            //m.Branch(m.Test(cc, flags), target);
            iclass = InstrClass.Invalid;
            m.Invalid();
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
            Emit__CNZ(m.Cond(left));
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
                RestoreRegisters(regs, 0);
            }
            else
            {
                regs = (MultipleRegistersOperand) instr.Operands[1];
                SaveRegisters(regs, 0);
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
            Emit_00NZ(m.Cond(id));
        }

        private void RewriteOr()
        {
            var right = Op(0, PrimitiveType.Word32);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.Or(left, right));
            Emit_00NZ(m.Cond(left));
        }

        private void RewriteRet()
        {
            RestoreRegisters(
                (MultipleRegistersOperand) instr.Operands[0],
                ((Constant) instr.Operands[1]).ToInt32());
            m.Return(4, 0);
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
            Emit_VCNZ(m.Cond(left));
        }

        private void RewriteSubc()
        {
            var right = Op(0, PrimitiveType.Word32);
            var cy = binder.EnsureFlagGroup(Registers.C);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.ISubC(left, right, cy));
            Emit_VCNZ(m.Cond(left));
        }

        private void RewriteXor()
        {
            var right = Op(0, PrimitiveType.Word32);
            var left = Op(1, PrimitiveType.Word32);
            m.Assign(left, m.Xor(left, right));
            Emit_00NZ(left);
        }
    }
}