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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Zilog.Z80
{
    public class Z80Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Z80Architecture arch;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Z80Instruction> dasm;
        private readonly RtlEmitter m;
        private readonly List<RtlInstruction> rtlInstructions;
        private InstrClass iclass;
        private Z80Instruction instr;

        public Z80Rewriter(Z80Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.host = host;
            this.dasm = new Z80Disassembler(arch, rdr).GetEnumerator();
            this.rtlInstructions = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtlInstructions);
            this.instr = default!;
        }

        public static PrimitiveType Nybble = PrimitiveType.CreateWord(4);

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instr = dasm.Current;
                var addr = instr.Address;
                var len = instr.Length;
                this.iclass = instr.InstructionClass;
                switch (dasm.Current.Mnemonic)
                {
                default:
                    host.Error(
                        dasm.Current.Address,
                        "Z80 instruction '{0}' is not supported yet.",
                        dasm.Current.Mnemonic);
                    goto case Mnemonic.illegal;
                case Mnemonic.illegal: m.Invalid(); break;
                case Mnemonic.adc: RewriteAdc(); break;
                case Mnemonic.add: RewriteAdd(); break;
                case Mnemonic.and: RewriteAnd(); break;
                case Mnemonic.bit: RewriteBit(); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.ccf: RewriteCcf(); break;
                case Mnemonic.cp: RewriteCp(); break;
                case Mnemonic.cpd: RewriteCp(m.ISub, false); break;
                case Mnemonic.cpdr: RewriteCp(m.ISub, true); break;
                case Mnemonic.cpi: RewriteCp(m.IAdd, false); break;
                case Mnemonic.cpir: RewriteCp(m.IAdd, true); break;
                case Mnemonic.cpl: RewriteCpl(); break;
                case Mnemonic.di: RewriteDi(); break;
                case Mnemonic.daa: RewriteDaa(); break;
                case Mnemonic.dec: RewriteDec(); break;
                case Mnemonic.djnz: RewriteDjnz(); break;
                case Mnemonic.ei: RewriteEi(); break;
                case Mnemonic.ex: RewriteEx(); break;
                case Mnemonic.ex_af: RewriteExAf(); break;
                case Mnemonic.exx: RewriteExx(); break;
                case Mnemonic.hlt: RewriteHlt(); break;
                case Mnemonic.@in: RewriteIn(); break;
                case Mnemonic.ind: RewriteIn(m.ISub, false); break;
                case Mnemonic.indr: RewriteIn(m.ISub, true); break;
                case Mnemonic.ini: RewriteIn(m.IAdd, false); break;
                case Mnemonic.inir: RewriteIn(m.IAdd, true); break;
                case Mnemonic.im:
                    m.SideEffect(m.Fn(im_intrinsic, RewriteOp(instr.Operands[0])));
                    break;
                case Mnemonic.inc: RewriteInc(); break;
                case Mnemonic.jp: RewriteJp(); break;
                case Mnemonic.jr: RewriteJr(); break;
                case Mnemonic.ld: RewriteLd(); break;
                case Mnemonic.rl: RewriteRotation(CommonOps.RolC, true); break;
                case Mnemonic.rla: RewriteRotation(CommonOps.RolC, true); break;
                case Mnemonic.rlc: RewriteRotation(CommonOps.Rol, false); break;
                case Mnemonic.rlca: RewriteRotation(CommonOps.Rol, false); break;
                case Mnemonic.rld: RewriteRld(); break;
                case Mnemonic.rrd: RewriteRrd(); break;
                case Mnemonic.rr: RewriteRotation(CommonOps.RorC, true); break;
                case Mnemonic.rra: RewriteRotation(CommonOps.RorC, true); break;
                case Mnemonic.rrc: RewriteRotation(CommonOps.Ror, false); break;
                case Mnemonic.rrca: RewriteRotation(CommonOps.Ror, false); break;
                case Mnemonic.ldd: RewriteBlockInstruction(m.ISub, false); break;
                case Mnemonic.lddr: RewriteBlockInstruction(m.ISub, true); break;
                case Mnemonic.ldi: RewriteBlockInstruction(m.IAdd, false); break;
                case Mnemonic.ldir: RewriteBlockInstruction(m.IAdd, true); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.@out: RewriteOut(); break;
                case Mnemonic.otdr: RewriteOutInstruction(-1, true); break;
                case Mnemonic.otir: RewriteOutInstruction(1, true); break;
                case Mnemonic.outd: RewriteOutInstruction(-1, false); break;
                case Mnemonic.outi: RewriteOutInstruction(1, false); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.res: RewriteResSet(res_intrinsic); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.rst: RewriteRst(); break;
                case Mnemonic.sbc: RewriteSbc(); break;
                case Mnemonic.scf: RewriteScf(); break;
                case Mnemonic.set: RewriteResSet(set_intrinsic); break;
                case Mnemonic.sla: RewriteShift(m.Shl); break;
                case Mnemonic.sra: RewriteShift(m.Sar); break;
                case Mnemonic.srl: RewriteShift(m.Shr); break;
                case Mnemonic.sll: RewriteShift(m.Shl); break;
                case Mnemonic.sub: RewriteSub(); break;
                case Mnemonic.xor: RewriteXor(); break;

                //$TODO: Not implemented yet; feel free to implement these!
                case Mnemonic.reti: goto default;
                case Mnemonic.retn: goto default;
                }
                if (len < 0)
                    throw new Exception($"addr: {instr.Address} len < 0");
                yield return m.MakeCluster(addr, len, iclass);
                rtlInstructions.Clear();
            }
        }

        private void RewriteAdc()
        {
            var dst = RewriteOp(dasm.Current.Operands[0]);
            var src = RewriteOp(dasm.Current.Operands[1]);
            m.Assign(dst, m.IAdd(m.IAdd(dst, src), FlagGroup(Registers.C)));
            AssignCond(Registers.SZPC, dst);
        }

        private void RewriteAdd()
        {
            var dst = RewriteOp(dasm.Current.Operands[0]);
            var src = RewriteOp(dasm.Current.Operands[1]);
            m.Assign(dst, m.IAdd(dst, src));
            AssignCond(Registers.SZPC, dst);
        }

        private void RewriteAnd()
        {
            var dst = RewriteOp(dasm.Current.Operands[0]);
            var src = RewriteOp(dasm.Current.Operands[1]);
            m.Assign(dst, m.And(dst, src));
            AssignCond(Registers.SZ, dst);
            m.Assign(FlagGroup(Registers.C), Constant.False());
        }

        private void RewriteBlockInstruction(Func<Expression, Expression, Expression> incdec, bool repeat)
        {
            var bc = binder.EnsureRegister(Registers.bc);
            var de = binder.EnsureRegister(Registers.de);
            var hl = binder.EnsureRegister(Registers.hl);
            var V =  FlagGroup(Registers.P);
            m.Assign(m.Mem8(de), m.Mem8(hl));
            m.Assign(hl, incdec(hl, Constant.Int16(1)));
            m.Assign(de, incdec(de, Constant.Int16(1)));
            m.Assign(bc, m.ISub(bc, 1));
            if (repeat)
            {
                m.BranchInMiddleOfInstruction(m.Ne0(bc), dasm.Current.Address, InstrClass.Transfer);
            }
            m.Assign(V, m.Const(PrimitiveType.Bool, 0));
        }

        private void RewriteNeg()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(a, m.Neg(a));
            AssignCond(Registers.SZPC, a);
        }

        private void RewriteOr()
        {
            var dst = RewriteOp(dasm.Current.Operands[0]);
            var src = RewriteOp(dasm.Current.Operands[1]);
            m.Assign(dst, m.Or(dst, src));
            AssignCond(Registers.SZ, dst);
            m.Assign(FlagGroup(Registers.C), Constant.False());
        }

        private void RewriteRld()
        {
            var lowA = binder.CreateTemporary(Nybble);
            var a = binder.EnsureRegister(Registers.a);
            var hl = binder.EnsureRegister(Registers.hl);
            var hlm = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(lowA, m.Slice(a, Nybble));
            m.Assign(hlm, m.Mem8(hl));
            m.Assign(a, m.Dpb(a, m.Slice(hlm, Nybble, 4), 0));
            m.Assign(hlm, m.Seq(m.Slice(hlm, Nybble, 0), lowA));
            m.Assign(m.Mem8(hl), hlm);
            AssignCond(Registers.SZ, a);
            //m.Assign(FlagGroup(Registers.H), 0);
        }

        private void RewriteRotation(IntrinsicProcedure intrinsic, bool useCarry)
        {
            Expression reg;
            if (dasm.Current.Operands.Length > 0)
            {
                reg = RewriteOp(dasm.Current.Operands[0]);
            }
            else
            {
                reg = binder.EnsureRegister(Registers.a);
            }
            var C = FlagGroup(Registers.C);
            var one = m.Byte(1);
            intrinsic = intrinsic.MakeInstance(reg.DataType, one.DataType);
            Expression src;
            if (useCarry)
            {
                src = m.Fn(intrinsic, reg, one, C);
            }
            else
            {
                src = m.Fn(intrinsic, reg, one);
            }
            m.Assign(reg, src);
            m.Assign(C, m.Cond(reg));
        }

        private void RewriteRrd()
        {
            var lowA = binder.CreateTemporary(Nybble);
            var a = binder.EnsureRegister(Registers.a);
            var hl = binder.EnsureRegister(Registers.hl);
            var hlm = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(lowA, m.Slice(a, Nybble));
            m.Assign(hlm, m.Mem8(hl));
            m.Assign(a, m.Dpb(a, m.Slice(hlm, Nybble, 0), 0));
            m.Assign(hlm, m.Seq(lowA, m.Slice(hlm, Nybble, 4)));
            m.Assign(m.Mem8(hl), hlm);
            AssignCond(Registers.SZ, a);
            //m.Assign(FlagGroup(Registers.H), 0);
        }


        private void RewriteSbc()
        {
            var dst = RewriteOp(dasm.Current.Operands[0]);
            var src = RewriteOp(dasm.Current.Operands[1]);
            m.Assign(dst, m.ISub(m.ISub(dst, src), FlagGroup(Registers.C)));
            AssignCond(Registers.SZPC, dst);
        }

        private void RewriteScf()
        {
            AssignCond(Registers.C, Constant.True());
        }

        private void RewriteSub()
        {
            var dst = RewriteOp(dasm.Current.Operands[0]);
            var src = RewriteOp(dasm.Current.Operands[1]);
            m.Assign(dst, m.ISub(dst, src));
            AssignCond(Registers.SZPC, dst);
        }

        private void RewriteXor()
        {
            var dst = RewriteOp(dasm.Current.Operands[0]);
            var src = RewriteOp(dasm.Current.Operands[1]);
            m.Assign(dst, m.Xor(dst, src));
            AssignCond(Registers.SZ, dst);
            m.Assign(FlagGroup(Registers.C), Constant.False());
        }

        private void AssignCond(FlagGroupStorage flags, Expression dst)
        {
            m.Assign(FlagGroup(flags), m.Cond(dst));
        }

        public Identifier FlagGroup(FlagGroupStorage flags)
        {
            return binder.EnsureFlagGroup(flags);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitBranch(ConditionOperand<CondCode> cOp, Address dst)
        {
            m.Branch(
                GenerateTestExpression(cOp, false),
                dst,
                InstrClass.ConditionalTransfer);
        }

        private TestCondition GenerateTestExpression(ConditionOperand<CondCode> cOp, bool invert)
        {
            ConditionCode cc = ConditionCode.ALWAYS;
            FlagGroupStorage? flags = null;
            switch (cOp.Condition)
            {
            case CondCode.nz:  cc = invert ? ConditionCode.EQ : ConditionCode.NE; flags = Registers.Z;  break;
            case CondCode.z: cc = invert ? ConditionCode.NE : ConditionCode.EQ; flags = Registers.Z;    break;
            case CondCode.nc: cc = invert ? ConditionCode.ULT : ConditionCode.UGE; flags = Registers.C; break;
            case CondCode.c: cc = invert ? ConditionCode.UGE : ConditionCode.ULT; flags = Registers.C;  break;
            case CondCode.po: cc = invert ? ConditionCode.PE : ConditionCode.PO; flags = Registers.P;  break;
            case CondCode.pe: cc = invert ? ConditionCode.PO : ConditionCode.PE; flags = Registers.P;    break;
            case CondCode.p: cc = invert ? ConditionCode.SG : ConditionCode.NS; flags =  Registers.S;    break;
            case CondCode.m: cc = invert ? ConditionCode.NS : ConditionCode.SG; flags =  Registers.S;    break;
            }
            return m.Test(
                cc,
                FlagGroup(flags!));
        }

        private void RewriteCall()
        {
            if (instr.Operands[0] is ConditionOperand<CondCode> cOp)
            {
                m.BranchInMiddleOfInstruction(
                    GenerateTestExpression(cOp, true),
                    instr.Address + instr.Length,
                    InstrClass.ConditionalTransfer);
                m.Call((Address)instr.Operands[1], 2);
            }
            else
            {
                m.Call((Address)instr.Operands[0], 2);
            }
        }

        private void RewriteCcf()
        {
            AssignCond(Registers.C, Constant.False());
        }

        private void RewriteCp()
        {
            var a = this.RewriteOp(dasm.Current.Operands[0]);
            var b = this.RewriteOp(dasm.Current.Operands[1]);
            m.Assign(
                FlagGroup(Registers.SZPC),
                m.Cond(m.ISub(a, b)));
        }

        private void RewriteCp(Func<Expression , Expression, Expression> incDec, bool repeat)
        {
            var addr = dasm.Current.Address;
            var a = binder.EnsureRegister(Registers.a);
            var bc = binder.EnsureRegister(Registers.bc);
            var hl = binder.EnsureRegister(Registers.hl);
            var z = FlagGroup(Registers.Z);
            m.Assign(z, m.Cond(m.ISub(a, m.Mem8(hl))));
            m.Assign(hl, incDec(hl, m.Int16(1)));
            m.Assign(bc, m.ISub(bc, 1));
            if (repeat)
            {
                m.BranchInMiddleOfInstruction(m.Eq0(bc), addr + dasm.Current.Length, InstrClass.ConditionalTransfer);
                m.Branch(m.Test(ConditionCode.NE, z), addr, InstrClass.ConditionalTransfer);
            }
        }

        private void RewriteCpl()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(a, m.Comp(a));
        }

        private void RewriteDaa()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(
                a,
                m.Fn(daa_intrinsic, a));
            AssignCond(Registers.SZPC, a);
        }

        private void RewriteDec()
        {
            var src = RewriteOp(dasm.Current.Operands[0]);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, m.ISub(src, 1));
            AssignCond(Registers.SZP, dst);
        }

        private void RewriteDjnz()
        {
            var dst = instr.Operands[0];
            var b = binder.EnsureRegister(Registers.b);
            m.Assign(b, m.ISub(b, 1));
            m.Branch(m.Ne0(b), (Address)dst, InstrClass.Transfer);
        }

        private void RewriteDi()
        {
            m.SideEffect(m.Fn(di_intrinsic));
        }

        private void RewriteEi()
        {
            m.SideEffect(m.Fn(ei_intrinsic));
        }

        private void RewriteEx()
        {
            var t = binder.CreateTemporary(dasm.Current.Operands[0].DataType);
            m.Assign(t, RewriteOp(dasm.Current.Operands[0]));
            m.Assign(RewriteOp(dasm.Current.Operands[0]), RewriteOp(dasm.Current.Operands[1]));
            m.Assign(RewriteOp(dasm.Current.Operands[1]), t);
        }

        private void RewriteExAf()
        {
            var t = binder.CreateTemporary(Registers.af.DataType);
            var af = binder.EnsureRegister(Registers.af);
            var af_ = binder.EnsureRegister(Registers.af_);
            m.Assign(t, af);
            m.Assign(af, af_);
            m.Assign(af_, t);
        }

        private void RewriteExx()
        {
            foreach (var r in new[] { "bc", "de", "hl" })
            {
                var t = binder.CreateTemporary(PrimitiveType.Word16);
                var reg = binder.EnsureRegister(arch.GetRegister(r)!);
                var reg_ = binder.EnsureRegister(arch.GetRegister(r + "'")!);
                m.Assign(t, reg);
                m.Assign(reg, reg_);
                m.Assign(reg_, t);
            }
        }

        private void RewriteHlt()
        {
            m.SideEffect(m.Fn(CommonOps.Halt), InstrClass.Terminates);
        }

        private void RewriteInc()
        {
            var src = RewriteOp(dasm.Current.Operands[0]);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, m.IAdd(src, 1));
            AssignCond(Registers.SZP, dst);
        }

        private void RewriteJp()
        {
            switch (instr.Operands[0])
            {
            case ConditionOperand<CondCode> cOp:
                EmitBranch(cOp, (Address)instr.Operands[1]);
                break;
            case Address target:
                m.Goto(target);
                break;
            case MemoryOperand mTarget:
                m.Goto(binder.EnsureRegister(mTarget.Base!));
                break;
            }
        }

        private void RewriteJr()
        {
            var op = dasm.Current.Operands[0];
            var cop = op as ConditionOperand<CondCode>;
            if (cop is not null)
            {
                op = dasm.Current.Operands[1];
            }
            var target = (Address)op;
            if (cop is not null)
            {
                ConditionCode cc;
                FlagGroupStorage cr;
                switch (cop.Condition)
                {
                case CondCode.c: cc = ConditionCode.ULT; cr = Registers.C; break;
                case CondCode.nz: cc = ConditionCode.NE; cr = Registers.Z; break;
                case CondCode.nc: cc = ConditionCode.UGE; cr = Registers.C; break;
                case CondCode.z: cc = ConditionCode.EQ; cr = Registers.Z; break;
                default: throw new NotImplementedException();
                }
                m.Branch(
                    m.Test(
                        cc,
                        binder.EnsureFlagGroup(cr)),
                    target,
                    iclass);
            }
            else
            {
                m.Goto(target);
            }
        }

        private void RewriteLd()
        {
            m.Assign(
                RewriteOp(dasm.Current.Operands[0]),
                RewriteOp(dasm.Current.Operands[1]));
        }

        private Expression RewriteOp(MachineOperand op)
        {
            switch (op)
            {
            case RegisterStorage rOp:
                return binder.EnsureRegister(rOp);
            case Constant immOp:
                return immOp;
            case MemoryOperand memOp:
                {
                    Identifier? bReg = null;
                    if (memOp.Base is not null)
                        bReg = binder.EnsureRegister(memOp.Base);
                    if (memOp.Offset is null)
                    {
                        return m.Mem(memOp.DataType, bReg!);
                    }
                    else if (bReg is null)
                    {
                        return m.Mem(memOp.DataType, memOp.Offset);
                    }
                    else
                    {
                        int s = memOp.Offset.ToInt32();
                        return m.Mem(memOp.DataType, m.AddSubSignedInt(bReg, s));
                        }
                        }
            default:
                throw new NotImplementedException(string.Format("Rewriting of Z80 operand type {0} is not implemented yet.", op.GetType().FullName));
            }
        }

        private void RewriteIn()
        {
            var dst = RewriteOp(dasm.Current.Operands[0]);
            var src = RewriteOp(dasm.Current.Operands[1]);
            m.Assign(dst, m.Fn(in_intrinsic, src));
        }

        private void RewriteIn(Func<Expression,Expression,Expression> incDec, bool repeat)
        {
            var hl = binder.EnsureRegister(Registers.hl);
            var c = binder.EnsureRegister(Registers.c);
            var b = binder.EnsureRegister(Registers.b);
            var Z = binder.EnsureFlagGroup(arch.GetFlagGroup("Z"));
            m.Assign(
                m.Mem8(hl),
                m.Fn(in_intrinsic, c));
            m.Assign(hl, incDec(hl, m.Int16(1)));
            m.Assign(b, m.ISub(b, 1));
            m.Assign(Z, m.Cond(b));
            if (repeat)
            {
                m.Branch(m.Ne0(b), dasm.Current.Address, InstrClass.ConditionalTransfer);
            }
        }

        private void RewriteOut()
        {
            var dst = RewriteOp(dasm.Current.Operands[0]);
            var src = RewriteOp(dasm.Current.Operands[1]);
            m.SideEffect(m.Fn(out_intrinsic, dst, src));
        }

        private void RewriteOutInstruction(int increment, bool repeat)
        {
            var hl = binder.EnsureRegister(Registers.hl);
            var c = binder.EnsureRegister(Registers.c);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, m.Mem8(hl));
            m.SideEffect(m.Fn(out_intrinsic, c, tmp));
            m.Assign(hl, m.AddSubSignedInt(hl, increment));
            if (repeat)
            {
                var b = binder.EnsureRegister(Registers.b);
                m.Assign(b, m.ISub(b, 1));
                m.Branch(m.Ne0(b), dasm.Current.Address);
            }
        }

        private void RewritePop()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var op = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(op, m.Mem(PrimitiveType.Word16, sp));
            m.Assign(sp, m.IAdd(sp, op.DataType.Size));
        }

        private void RewritePush()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var op = RewriteOp(instr.Operands[0]);
            m.Assign(sp, m.ISub(sp, op.DataType.Size));
            m.Assign(m.Mem(PrimitiveType.Word16, sp), op);
        }

        private void RewriteBit()
        {
            var bit = RewriteOp(dasm.Current.Operands[0]);
            var op = RewriteOp(dasm.Current.Operands[1]);
            AssignCond(Registers.Z, m.Fn(bit_intrinsic, op, bit));
        }

        private void RewriteResSet(IntrinsicProcedure intrinsic)
        {
            var bit = RewriteOp(dasm.Current.Operands[0]);
            var op = RewriteOp(dasm.Current.Operands[1]);
            Expression dst;
            if (op is MemoryAccess)
                dst = binder.CreateTemporary(op.DataType);
            else
                dst = op;
            m.Assign(dst, m.Fn(intrinsic, op, bit));
            if (dst != op)
            {
                m.Assign(op, dst);
            }
        }

        private void RewriteRet()
        {
            if (this.instr.Operands.Length != 0)
            {
                var cOp = (ConditionOperand<CondCode>) this.instr.Operands[0];
                m.BranchInMiddleOfInstruction(
                    GenerateTestExpression(cOp, true),
                    instr.Address + instr.Length,
                    InstrClass.ConditionalTransfer);
            }
            m.Return(2, 0);
        }

        private void RewriteRst()
        {
            m.Call(
                Address.Ptr16(
                    ((Constant)dasm.Current.Operands[0]).ToUInt16()),
                2);
        }

        private void RewriteShift(Func<Expression, Expression, Expression> op)
        {
            var reg = RewriteOp(instr.Operands[0]);
            var sh = m.Byte(1);
            m.Assign(reg, op(reg, sh));
            AssignCond(Registers.SZPC, reg);
        }

        private static readonly IntrinsicProcedure bit_intrinsic = new IntrinsicBuilder("__bit", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Bool);
        private static readonly IntrinsicProcedure daa_intrinsic = new IntrinsicBuilder("__daa", false)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure di_intrinsic = new IntrinsicBuilder("__di", true)
            .Void();
        private static readonly IntrinsicProcedure ei_intrinsic = new IntrinsicBuilder("__ei", true)
            .Void();
        private static readonly IntrinsicProcedure im_intrinsic = new IntrinsicBuilder("__im", true)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure in_intrinsic = new IntrinsicBuilder("__in", true)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure out_intrinsic = new IntrinsicBuilder("__out", true)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure res_intrinsic = new IntrinsicBuilder("__res", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure set_intrinsic = new IntrinsicBuilder("__set", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
    }
}
