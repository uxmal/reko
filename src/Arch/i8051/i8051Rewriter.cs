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
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.i8051
{
    // http://www.keil.com/support/man/docs/is51/is51_instructions.htm
    public class i8051Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly i8051Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<i8051Instruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private i8051Instruction instr;
        private InstrClass iclass;

        public i8051Rewriter(i8051Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new i8051Disassembler(arch, rdr).GetEnumerator();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = InstrClass.Linear;
                switch (instr.Mnemonic)
                {
                default:
                    host.Warn(
                       instr.Address,
                       string.Format(
                           "i8051 instruction '{0}' not supported yet.",
                           instr.Mnemonic));
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                case Mnemonic.reserved:
                    Invalid();
                    break;
                case Mnemonic.acall: RewriteCall(); break;
                case Mnemonic.add: RewriteBinop(m.IAdd, Registers.CAOP); break;
                case Mnemonic.addc: RewriteAddcSubb(m.IAdd); break;
                case Mnemonic.ajmp: RewriteJump(); break;
                case Mnemonic.anl: RewriteLogical(m.And); break;
                case Mnemonic.cjne: RewriteCjne(); break;
                case Mnemonic.clr: RewriteClr(); break;
                case Mnemonic.cpl: RewriteCpl(); break;
                case Mnemonic.da: RewriteDa(da_intrinsic); break;
                case Mnemonic.dec: RewriteIncDec(m.ISub); break;
                case Mnemonic.div: RewriteDiv(); break;
                case Mnemonic.djnz: RewriteDjnz(); break;
                case Mnemonic.inc: RewriteIncDec(m.IAdd); break;
                case Mnemonic.jb: RewriteJb(m.Ne0); break;
                case Mnemonic.jbc: RewriteJbc(); break;
                case Mnemonic.jc: RewriteJc(ConditionCode.ULT); break;
                case Mnemonic.jmp: RewriteJump(); break;
                case Mnemonic.jnb: RewriteJb(m.Eq0); break;
                case Mnemonic.jnc: RewriteJc(ConditionCode.UGE); break;
                case Mnemonic.jnz: RewriteJz(m.Ne0); break;
                case Mnemonic.jz: RewriteJz(m.Eq0); break;
                case Mnemonic.lcall: RewriteCall(); break;
                case Mnemonic.ljmp: RewriteJump(); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movc: RewriteMovc(); break;
                case Mnemonic.movx: RewriteMovx(); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.orl: RewriteLogical(m.Or); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.reti: RewriteRet(); break;
                case Mnemonic.rl: RewriteRotate(CommonOps.Rol); break;
                case Mnemonic.rlc: RewriteRotateC(CommonOps.RolC, 0x80); break;
                case Mnemonic.rr: RewriteRotate(CommonOps.Ror); break;
                case Mnemonic.rrc: RewriteRotateC(CommonOps.RorC, 1); break;
                case Mnemonic.setb: RewriteSetb(); break;
                case Mnemonic.sjmp: RewriteJump(); break;
                case Mnemonic.subb: RewriteAddcSubb(m.ISub); break;
                case Mnemonic.swap: RewriteSwap(); break;
                case Mnemonic.xrl: RewriteLogical(m.Xor); break;
                case Mnemonic.xch: RewriteXch(); break;
                case Mnemonic.xchd: RewriteXchd(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        private void RewriteJump()
        {
            iclass = InstrClass.Transfer;
            var dst = OpSrc(instr.Operands[0], null);
            if (dst is MemoryAccess mem)
            {
                dst = mem.EffectiveAddress;
            }
            dst.DataType = PrimitiveType.Ptr16;
            m.Goto(dst);
        }

        private void RewriteBinop(Func<Expression, Expression, BinaryExpression> fn, FlagGroupStorage? grf = null)
        {
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            var src = OpSrc(instr.Operands[1], arch.DataMemory);
            m.Assign(dst, fn(dst, src));
            if (grf is not null)
            {
                m.Assign(binder.EnsureFlagGroup(grf), m.Cond(dst));
            }
        }

        public void RewriteAddcSubb(Func<Expression, Expression, Expression> opr)
        {
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.PSW, (uint)FlagM.C));
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            var src = OpSrc(instr.Operands[1], arch.DataMemory);
            m.Assign(
                dst,
                opr(
                    opr(
                        dst,
                        src),
                    c));
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.PSW, (uint)(FlagM.C | FlagM.AC | FlagM.OV | FlagM.P)));
            m.Assign(grf, m.Cond(dst));
        }

        private void RewriteCall()
        {
            iclass = InstrClass.Transfer | InstrClass.Call;
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            dst.DataType = PrimitiveType.Ptr16;
            m.Call(dst, 2);
        }

        private void RewriteCjne()
        {
            iclass = InstrClass.ConditionalTransfer;
            var a = OpSrc(instr.Operands[0], arch.DataMemory);
            var b = OpSrc(instr.Operands[1], arch.DataMemory);
            var C = binder.EnsureFlagGroup(Registers.CFlag);
            var addr = (Address) instr.Operands[2];
            m.Assign(C, m.Cond(m.ISub(a, b)));
            m.Branch(m.Ne(a, b), addr, iclass);
        }

        private void RewriteClr()
        {
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            WriteDst(instr.Operands[0], Constant.Zero(dst.DataType));
            if (dst is Identifier id && id.Storage is RegisterStorage)
            {
                m.Assign(binder.EnsureFlagGroup(Registers.PFlag), m.Cond(dst));
            }
        }

        private void RewriteCpl()
        {
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            switch (instr.Operands[0])
            {
            case RegisterStorage r:
                var reg = binder.EnsureRegister(r);
                m.Assign(reg, m.Comp(reg));
                break;
            case FlagGroupStorage f:
                var grf = binder.EnsureFlagGroup(f);
                m.Assign(grf, m.Comp(grf));
                break;
            case BitOperand b:
                var breg = binder.EnsureRegister(b.Register);
                m.Assign(breg, m.Xor(breg, 1u << b.Bit));
                break;
            default:
                throw new NotSupportedException(
                    $"Impossible operand type {instr.Operands[0].GetType().Name}.");
            }
        }

        private void RewriteDa(IntrinsicProcedure intrinsic)
        {
            var acc = binder.EnsureRegister(Registers.A);
            m.Assign(
                binder.EnsureFlagGroup(Registers.CFlag), 
                m.Fn(
                intrinsic,
                    acc,
                    m.Out(PrimitiveType.Byte, acc)));
        }

        private void RewriteDiv()
        {
            var a = binder.EnsureRegister(Registers.A);
            var b = binder.EnsureRegister(Registers.B);
            var q = binder.CreateTemporary(PrimitiveType.UInt8);
            var r = binder.CreateTemporary(PrimitiveType.UInt8);
            m.Assign(q, m.UDiv(a, b));
            m.Assign(r, m.UMod(a, b));
            m.Assign(a, q);
            m.Assign(b, r);
            m.Assign(binder.EnsureFlagGroup(Registers.CFlag), Constant.False());
            m.Assign(binder.EnsureFlagGroup(Registers.OFlag), Constant.False());
        }

        private void RewriteDjnz()
        {
            iclass = InstrClass.ConditionalTransfer;
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            var addr = (Address) instr.Operands[1];
            m.Assign(dst, m.ISub(dst, 1));
            m.Branch(m.Ne0(dst), addr, InstrClass.ConditionalTransfer);
        }

        private void RewriteIncDec(Func<Expression, Expression, Expression> fn)
        {
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            m.Assign(dst, fn(dst, m.Const(dst.DataType, 1)));
            if (dst is Identifier id && id.Storage is RegisterStorage)
            {
                var flg = arch.GetFlagGroup(Registers.PSW, (uint)FlagM.P);
                m.Assign(binder.EnsureFlagGroup(flg), m.Cond(dst));
            }
        }

        private void RewriteJb(Func<Expression, Expression> cmp)
        {
            iclass = InstrClass.ConditionalTransfer;
            var src = OpSrc(instr.Operands[0], arch.DataMemory);
            var addr = (Address)instr.Operands[1];
            m.Branch(cmp(src), addr, InstrClass.ConditionalTransfer);
        }

        private void RewriteJc(ConditionCode cc)
        {
            iclass = InstrClass.ConditionalTransfer;
            var src = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.PSW, (uint)FlagM.C));
            var addr = (Address)instr.Operands[0];
            m.Branch(m.Test(cc, src), addr, InstrClass.ConditionalTransfer);
        }

        private void RewriteJbc()
        {
            iclass = InstrClass.ConditionalTransfer;
            var src = OpSrc(instr.Operands[0], arch.DataMemory);
            m.BranchInMiddleOfInstruction(m.Eq0(src), instr.Address + instr.Length, iclass);
            WriteDst(instr.Operands[0], Constant.Zero(src.DataType));
            m.Goto(OpSrc(instr.Operands[1], null));
        }

        private void RewriteJz(Func<Expression, Expression> cmp)
        {
            iclass = InstrClass.ConditionalTransfer;
            var src = binder.EnsureRegister(Registers.A);
            var addr = (Address)instr.Operands[0];
            m.Branch(cmp(src), addr, InstrClass.ConditionalTransfer);
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> fn)
        {
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            var src = OpSrc(instr.Operands[1], arch.DataMemory);
            m.Assign(dst, fn(dst, src));
            if (dst is Identifier id && id.Storage is RegisterStorage)
            {
                var flg = arch.GetFlagGroup(Registers.PSW, (uint)FlagM.P);
                m.Assign(binder.EnsureFlagGroup(flg), m.Cond(dst));
            }
        }

        private void RewriteMov()
        {
            var src = OpSrc(instr.Operands[1], arch.DataMemory);
            if (instr.Operands[0] is BitOperand bit)
            {
                var r = binder.EnsureRegister(bit.Register);
                m.Assign(r, m.Fn(
                    CommonOps.WriteBit.MakeInstance(r.DataType, PrimitiveType.Byte),
                    r, m.Byte((byte)bit.Bit), src));
            }
            else
            {
                var dst = OpSrc(instr.Operands[0], arch.DataMemory);
                m.Assign(dst, src);
            }
        }

        private void RewriteMovc()
        {
            var dst = binder.EnsureRegister(Registers.A);
            var src = OpSrc(instr.Operands[0], null);
            m.Assign(dst, src);
        }

        private void RewriteMovx()
        {
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            var src = OpSrc(instr.Operands[1], arch.DataMemory);
            m.Assign(dst, src);
        }

        private void RewriteMul()
        {
            var a = binder.EnsureRegister(Registers.A);
            var b = binder.EnsureRegister(Registers.B);
            var ab = binder.EnsureSequence(PrimitiveType.Word16, Registers.B, Registers.A);
            m.Assign(ab, m.UMul(PrimitiveType.Word16, a, b));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.PSW, (uint)FlagM.P)), m.Cond(ab));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.PSW, (uint)FlagM.OV)), m.Ugt(ab, m.Word16(0xFF)));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.PSW, (uint)FlagM.C)), Constant.False());
        }

        private void RewritePop()
        {
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            var sp = binder.EnsureRegister(Registers.SP);
            var dataMemory = binder.EnsureRegister(arch.DataMemory);
            m.Assign(dst, m.SegMem(dst.DataType, dataMemory, sp));
            m.Assign(sp, m.ISub(sp, 1));
        }

        private void RewritePush() {
            var src = OpSrc(instr.Operands[0], arch.DataMemory);
            var sp = binder.EnsureRegister(Registers.SP);
            m.Assign(sp, m.IAdd(sp, 1));
            var dataMemory = binder.EnsureRegister(arch.DataMemory);
            m.Assign(m.SegMem8(dataMemory, sp), src);
        }

        private void RewriteRotate(IntrinsicProcedure rot)
        {
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            m.Assign(dst, m.Fn(rot, dst, m.Byte(1)));
        }

        private void RewriteRotateC(IntrinsicProcedure rot, uint maskOut)
        {
            var dst = OpSrc(instr.Operands[0], arch.DataMemory);
            var c = binder.EnsureFlagGroup(Registers.CFlag);
            var t = binder.CreateTemporary(PrimitiveType.Bool);
            m.Assign(t, m.Ne0(m.And(dst, maskOut)));
            m.Assign(dst, m.Fn(rot.MakeInstance(dst.DataType, dst.DataType), dst, m.Byte(1), c));
            m.Assign(c, t);
        }

        private void RewriteRet()
        {
            iclass = InstrClass.Transfer|InstrClass.Return;
            m.Return(2, 0);
        }

        private void RewriteSetb()
        {
            WriteDst(instr.Operands[0], Constant.True());
        }

        private void RewriteSwap()
        {
            var tmp = binder.CreateTemporary(Registers.A.DataType);
            var a = binder.EnsureRegister(Registers.A);
            m.Assign(tmp, m.Shl(a, 4));
            m.Assign(a, m.Shr(a, 4));
            m.Assign(a, m.Or(a, tmp));
        }

        private void RewriteXch()
        {
            var tmp = binder.CreateTemporary(Registers.A.DataType);
            var a = OpSrc(instr.Operands[0], arch.DataMemory);
            var b = OpSrc(instr.Operands[1], arch.DataMemory);
            m.Assign(tmp, a);
            m.Assign(a, b);
            m.Assign(b, tmp);
        }

        private void RewriteXchd()
        {
            var tmpA = binder.CreateTemporary(PrimitiveType.CreateWord(4));
            var tmpB = binder.CreateTemporary(PrimitiveType.Byte);
            var a = OpSrc(instr.Operands[0], arch.DataMemory);
            var b = OpSrc(instr.Operands[1], arch.DataMemory);
            m.Assign(tmpA, m.Slice(a, tmpA.DataType));
            m.Assign(tmpB, b);
            m.Assign(a, m.Dpb(a, m.Slice(tmpB, tmpA.DataType), 0));
            m.Assign(tmpB, m.Dpb(tmpB, tmpA, 0));
            m.Assign(b, tmpB);
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("i8051_rw", this.instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private void Invalid()
        {
            m.Invalid();
            iclass = InstrClass.Invalid;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression OpSrc(MachineOperand op, RegisterStorage? dataMemory)
        {
            switch (op)
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case FlagGroupStorage flg:
                return binder.EnsureFlagGroup(flg);
            case Constant imm:
                return imm;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                var (ea, alias) = EffectiveAddress(mem);
                if (alias is not null)
                    return alias;
                Debug.Assert(ea is not null);
                if (dataMemory is not null)
                {
                    return m.SegMem(mem.DataType, binder.EnsureRegister(dataMemory), ea);
                }
                else
                {
                    return m.Mem(mem.DataType, ea);
                }
            case BitOperand bit:
                Expression e = binder.EnsureRegister(bit.Register);
                if (bit.Bit > 0)
                {
                    e = m.Shr(e, (byte)bit.Bit);
                }
                e = m.And(e, 1);
                if (bit.Negated)
                {
                    e = m.Not(e);
                }
                return e;
            case SequenceStorage seq:
                return binder.EnsureSequence(seq);
            default:
                throw new NotImplementedException($"Not implemented {op.GetType().Name}.");
            }
        }

        private (Expression?, Expression?) EffectiveAddress(MemoryOperand mem)
        {
            Expression ea;
            if (mem.DirectAddress is not null)
            {
                if (mem.Index is not null)
                {
                    ea = m.IAdd(mem.DirectAddress, binder.EnsureRegister(mem.Index));
                }
                else if (mem.DirectAddress is Constant c)
                {
                    var alias = AliasedSpecialFunctionRegister(c.ToUInt16());
                    if (alias is not null)
                        return (null, alias);
                    ea = c;
                }
                else
                {
                    ea = mem.DirectAddress;
                }
            }
            else if (mem.Register is not null)
            {
                if (mem.Index is not null)
                {
                    var idx = binder.EnsureRegister(mem.Index);
                    if (mem.Register == Registers.PC)
                    {
                        ea = m.IAdd(
                            instr.Address + instr.Length, idx);
                    }
                    else
                    {
                        ea = binder.EnsureIdentifier(mem.Register);
                        ea = m.IAdd(ea, idx);
                    }
                }
                else
                {
                    ea = binder.EnsureIdentifier(mem.Register);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            if (ea.DataType.BitSize < 16)
            {
                ea = m.Convert(ea, PrimitiveType.Byte, PrimitiveType.Word16);
            }
            return (ea, null);
        }

        /// <summary>
        /// Some 8051 registers are aliased to memory addresses.
        /// </summary>
        private Expression? AliasedSpecialFunctionRegister(ushort uAddress)
        {
            switch (uAddress)
            {
            case 0x81: return binder.EnsureIdentifier(Registers.SP);
            case 0x82: return binder.EnsureIdentifier(Registers.DPL);
            case 0x83: return binder.EnsureIdentifier(Registers.DPH);
            case 0xE0: return binder.EnsureIdentifier(Registers.A);
            case 0xF0: return binder.EnsureIdentifier(Registers.B);
            }
            return null;
        }

        private void WriteDst(MachineOperand op, Expression src)
        {
            switch (op)
            {
            case RegisterStorage reg:
                m.Assign(binder.EnsureRegister(reg), src);
                break;
            case FlagGroupStorage flg:
                m.Assign(binder.EnsureFlagGroup(flg), src);
                break;
            case MemoryOperand mem:
                Expression ea;
                if (mem.DirectAddress is not null)
                {
                    if (mem.Index is not null)
                    {
                        ea = m.IAdd(mem.DirectAddress, binder.EnsureRegister(mem.Index));
                    }
                    else
                    {
                        ea = mem.DirectAddress;
                    }
                }
                else if (mem.Register is not null)
                {
                    if (mem.Index is not null)
                    {
                        var idx = binder.EnsureRegister(mem.Index);
                        if (mem.Register == Registers.PC)
                        {
                            ea = m.IAdd(
                                instr.Address + instr.Length, idx);
                        }
                        else
                        {
                            ea = binder.EnsureIdentifier(mem.Register);
                            ea = m.IAdd(ea, idx);
                        }
                    }
                    else
                    {
                        ea = binder.EnsureIdentifier(mem.Register);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                m.Assign(m.Mem(mem.DataType, ea), src);
                break;
            case BitOperand bit:
                if (bit.Bit > 0)
                {
                    src = m.Shl(src, (byte)bit.Bit);
                }
                var mask = (byte)~(1 << bit.Bit);
                var e = binder.EnsureRegister(bit.Register);
                m.Assign(e, m.Or(m.And(e, mask), src));
                break;
            default:
                throw new NotImplementedException($"Not implemented {op.GetType().Name}.");
            }
        }

        private static readonly IntrinsicProcedure da_intrinsic = new IntrinsicBuilder("__decimal_adjust_addition", false)
                .Param(PrimitiveType.Byte)
                .OutParam(PrimitiveType.Byte)
                .Returns(PrimitiveType.Bool);
    }
}
