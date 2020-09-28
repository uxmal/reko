#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.MicroBlaze
{
    public class MicroBlazeRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly MicroBlazeArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private IEnumerator<MicroBlazeInstruction> dasm;
        private MicroBlazeInstruction instrCur;
        private InstrClass iclass;
        private RtlEmitter m;
        private Address addrInstr;
        private int immHiBits;

        public MicroBlazeRewriter(MicroBlazeArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new MicroBlazeDisassembler(arch, rdr).GetEnumerator();
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
                    NotImplementedInstruction();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    this.iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.add: RewriteAdd(true); break;
                case Mnemonic.addc: RewriteAddc(true); break;
                case Mnemonic.addk: RewriteAdd(false); break;
                case Mnemonic.addi: RewriteAddi(true); break;
                case Mnemonic.addik: RewriteAddi(false); break;
                case Mnemonic.and: RewriteLogical(m.And); break;
                case Mnemonic.andi: RewriteLogicalImm(m.And); break;
                case Mnemonic.beqi: RewriteBranch(ConditionCode.EQ); break;
                case Mnemonic.beqid: RewriteBranch(ConditionCode.EQ); break;
                case Mnemonic.bgei: RewriteBranch(ConditionCode.GE); break;
                case Mnemonic.bgeid: RewriteBranch(ConditionCode.GE); break;
                case Mnemonic.bgti: RewriteBranch(ConditionCode.GT); break;
                case Mnemonic.bgtid: RewriteBranch(ConditionCode.GT); break;
                case Mnemonic.blei: RewriteBranch(ConditionCode.LE); break;
                case Mnemonic.bleid: RewriteBranch(ConditionCode.LE); break;
                case Mnemonic.blti: RewriteBranch(ConditionCode.LT); break;
                case Mnemonic.bltid: RewriteBranch(ConditionCode.LT); break;
                case Mnemonic.bnei: RewriteBranch(ConditionCode.NE); break;
                case Mnemonic.bneid: RewriteBranch(ConditionCode.NE); break;
                case Mnemonic.br: RewriteJump(false, false); break;
                case Mnemonic.bra: RewriteJump(false, true); break;
                case Mnemonic.brad: RewriteJump(false, true); break;
                case Mnemonic.brai: RewriteJumpAddr(false); break;
                case Mnemonic.brald: RewriteJump(true, false); break;
                case Mnemonic.bri: RewriteJumpAddr(false); break;
                case Mnemonic.brid: RewriteJumpAddr(false); break;
                case Mnemonic.brlid: RewriteJumpAddr(true); break;
                case Mnemonic.cmp: RewriteCmp(m.ISub); break;
                case Mnemonic.cmpu: RewriteCmp(m.USub); break;
                case Mnemonic.imm: CaptureImmState(); continue; // <- don't emit code!
                case Mnemonic.lbu: RewriteLoadIdx(PrimitiveType.Byte); break;
                case Mnemonic.lbui: RewriteLoadOffset(PrimitiveType.Byte); break;
                case Mnemonic.lhu: RewriteLoadIdx(PrimitiveType.Word16); break;
                case Mnemonic.lhui: RewriteLoadOffset(PrimitiveType.Word16); break;
                case Mnemonic.lw: RewriteLoadIdx(PrimitiveType.Word32); break;
                case Mnemonic.lwi: RewriteLoadOffset(PrimitiveType.Word32); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.ori: RewriteOri(); break;
                case Mnemonic.rsub: RewriteRsub(true); break;
                case Mnemonic.rsubi: RewriteRsubi(true); break;
                case Mnemonic.rsubk: RewriteRsub(false); break;
                case Mnemonic.rsubik: RewriteRsubi(false); break;
                case Mnemonic.rtsd: RewriteRtsd(); break;
                case Mnemonic.sb: RewriteStoreIdx(PrimitiveType.Byte); break;
                case Mnemonic.sbi: RewriteStoreOffset(PrimitiveType.Byte); break;
                case Mnemonic.sext8: RewriteSext(PrimitiveType.SByte); break;
                case Mnemonic.sext16: RewriteSext(PrimitiveType.Int16); break;
                case Mnemonic.sh: RewriteStoreIdx(PrimitiveType.Word16); break;
                case Mnemonic.shi: RewriteStoreOffset(PrimitiveType.Word16); break;
                case Mnemonic.sra: RewriteShift1(m.Sar); break;
                case Mnemonic.src: RewriteShift1(RorC); break;
                case Mnemonic.srl: RewriteShift1(m.Shr); break;
                case Mnemonic.sw: RewriteStoreIdx(PrimitiveType.Word32); break;
                case Mnemonic.swi: RewriteStoreOffset(PrimitiveType.Word32); break;
                case Mnemonic.xor: RewriteLogical(m.Xor); break;
                case Mnemonic.xori: RewriteLogicalImm(m.Xor); break;
                }
                var addr = addrInstr ?? instrCur.Address;
                var length = (int) (rdr.Address - addr);
                this.addrInstr = null;
                yield return new RtlInstructionCluster(addr, length, instrs.ToArray())
                {
                    Class = iclass,
                };
                this.immHiBits = 0;
                addrInstr = null;
            }
        }

        private void NotImplementedInstruction()
        {
            host.Warn(
                instrCur.Address,
                "MicroBlaze instruction '{0}' is not supported yet.",
                instrCur.Mnemonic);
            EmitUnitTest();


        }

#if DEBUG
        private static readonly HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        private void EmitUnitTest()
        {
            if (rdr == null || seen.Contains(instrCur.Mnemonic))
                return;
            seen.Add(instrCur.Mnemonic);

            var r2 = rdr.Clone();
            int cbInstr = dasm.Current.Length;
            r2.Offset -= cbInstr;
            var uInstr = r2.ReadUInt32();
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void MicroBlazeRw_{0}()", dasm.Current.Mnemonic);
            Debug.WriteLine("        {");
            Debug.WriteLine("            RewriteCode(\"{0:X8}\");   // {1}", uInstr, dasm.Current);
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|00100000({0}): 1 instructions\",", cbInstr);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }
#else
        [Conditional("DEBUG")]
        private void EmitUnitTest() { }
#endif

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Address Addr(int iOp)
        {
            var op = instrCur.Operands[iOp];
            var addr = ((AddressOperand) op).Address;
            if (immHiBits != 0)
            {
                addr += immHiBits;
            }
            return addr;
        }

        private void C(Expression e)
        {
            var carry = binder.EnsureFlagGroup(Registers.C);
            m.Assign(carry, m.Cond(e));
        }

        /// <summary>
        /// An imm instruction will affect a subsequent instruction with 
        /// an immediate operand / address.
        /// </summary>
        void CaptureImmState()
        {
            this.immHiBits = ((ImmediateOperand) instrCur.Operands[0]).Value.ToInt32() << 16;
            this.addrInstr = instrCur.Address;
        }

        private void EmitLoad(Identifier dst, MemoryAccess src)
        {
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                m.Assign(dst, m.Cast(dst.DataType, tmp));
            }
            else
            {
                m.Assign(dst, src);
            }
        }

        private void EmitStore(MemoryAccess dst, Identifier src)
        {
            if (src.DataType.BitSize > dst.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(dst.DataType);
                m.Assign(tmp, m.Slice(dst.DataType, src, 0));
                m.Assign(dst, tmp);
            }
            else
            {
                m.Assign(dst, src);
            }
        }

        private Constant Imm(int iOp)
        {
            var op = instrCur.Operands[iOp];
            var imm = ((ImmediateOperand) op).Value;
            if (immHiBits != 0)
            {
                imm = Constant.Create(imm.DataType, imm.ToInt32() + immHiBits);
            }
            return imm;
        }

        private Identifier Reg(int iOp)
        {
            var op = instrCur.Operands[iOp];
            var id = binder.EnsureRegister(((RegisterOperand) op).Register);
            return id;
        }

        private Expression Reg0(int iOp)
        {
            var op = instrCur.Operands[iOp];
            var reg = ((RegisterOperand) op).Register;
            if (reg == Registers.GpRegs[0])
                return Constant.Zero(reg.DataType);
            var id = binder.EnsureRegister(reg);
            return id;
        }

        private Expression RorC(Expression a, Expression shift)
        {
            var cy = binder.EnsureFlagGroup(Registers.C);
            var rorc = host.PseudoProcedure(PseudoProcedure.RorC, a.DataType, a, shift, cy);
            return rorc;
        }

        private void RewriteAdd(bool setCy)
        {
            var dst = Reg(0);
            var regA = ((RegisterOperand) instrCur.Operands[1]).Register;
            var regB = ((RegisterOperand) instrCur.Operands[2]).Register;
            Expression src;
            if (regA == Registers.GpRegs[0])
            {
                setCy = false;
                if (regB == Registers.GpRegs[0])
                {
                    src = Constant.Zero(dst.DataType);
                }
                else
                {
                    src = binder.EnsureRegister(regB);
                }
            }
            else
            {
                if (regB == Registers.GpRegs[0])
                {
                    setCy = false;
                    src = binder.EnsureRegister(regA);
                }
                else
                {
                    var a = binder.EnsureRegister(regA);
                    var b = binder.EnsureRegister(regB);
                    src = m.IAdd(a, b);
                }
            }
            m.Assign(dst, src);
            if (setCy)
            {
                C(dst);
            }
        }

        private void RewriteAddc(bool setCy)
        {
            var dst = Reg(0);
            var regA = ((RegisterOperand) instrCur.Operands[1]).Register;
            var regB = ((RegisterOperand) instrCur.Operands[2]).Register;
            var cy = binder.EnsureFlagGroup(Registers.C);
            Expression src;
            if (regA == Registers.GpRegs[0])
            {
                if (regB == Registers.GpRegs[0])
                {
                    setCy = false;
                    src = m.Cast(dst.DataType, cy);
                }
                else
                {
                    src = binder.EnsureRegister(regB);
                    src = m.IAdd(src, m.Cast(dst.DataType, cy));
                }
            }
            else
            {
                if (regB == Registers.GpRegs[0])
                {
                    src = binder.EnsureRegister(regA);
                    src = m.IAdd(src, m.Cast(dst.DataType, cy));
                }
                else
                {
                    var a = binder.EnsureRegister(regA);
                    var b = binder.EnsureRegister(regB);
                    src = m.IAdd(m.IAdd(a, b), cy);
                }
            }
            m.Assign(dst, src);
            if (setCy)
            {
                C(dst);
            }
        }

        private void RewriteAddi(bool setCy)
        {
            var dst = Reg(0);
            var reg = ((RegisterOperand) instrCur.Operands[1]).Register;
            var imm = ((ImmediateOperand) instrCur.Operands[2]).Value;
            Expression src;
            if (reg == Registers.GpRegs[0])
            {
                setCy = false;
                src = imm;
            }
            else
            {
                src = binder.EnsureRegister(reg);
                if (imm.IsZero)
                {
                    setCy = false;
                }
                else
                {
                    src = m.IAdd(src, imm);
                }
            }
            m.Assign(dst, src);
            if (setCy)
            {
                C(dst);
            }
        }

        private void RewriteBranch(ConditionCode cc)
        {
            var reg = Reg(0);
            var cond = m.Test(cc, reg);
            if (instrCur.Operands[1] is AddressOperand addrDst)
            {
                m.Branch(cond, addrDst.Address, instrCur.InstructionClass);
            }
            else
            {
                var regDst = Reg(1);
                m.BranchInMiddleOfInstruction(
                    cond.Invert(),
                    instrCur.Address + instrCur.Length,
                    InstrClass.ConditionalTransfer);
                m.Goto(m.IAdd(instrCur.Address, regDst));
            }
        }

        private void RewriteCmp(Func<Expression, Expression, Expression> fn)
        {
            var dst = Reg(0);
            var src1 = Reg0(1);
            var src2 = Reg0(2);
            m.Assign(dst, m.Cond(fn(src2, src1)));
        }

        private void RewriteJump(bool link, bool absolute)
        {
            Identifier regIdx;
            if (link)
            {
                Debug.Assert(instrCur.Operands.Length == 2, "Expected a link register");
                m.Assign(Reg(0), instrCur.Address);
                regIdx = Reg(1);
            }
            else
            {
                regIdx = Reg(0);
            }
            Expression dst;
            if (absolute)
            {
                dst = regIdx;
            }
            else
            {
                dst = m.IAdd(instrCur.Address, regIdx);
            }
            if (link)
            {
                m.Call(dst, 0, instrCur.InstructionClass);
            }
            else
            {
                m.Goto(dst, instrCur.InstructionClass);
            }
        }

        private void RewriteJumpAddr(bool link)
        {
            Address addrDest;
            if (link)
            {
                Debug.Assert(instrCur.Operands.Length == 2, "Expected a link register");
                m.Assign(Reg(0), instrCur.Address);
                addrDest = Addr(1);
                m.Call(addrDest, 0, instrCur.InstructionClass);
            }
            else
            {
                addrDest = Addr(0);
                m.Goto(addrDest, instrCur.InstructionClass);
            }
        }

        private void RewriteLoadIdx(PrimitiveType dt)
        {
            var dst = Reg(0);
            var src1 = Reg0(1);
            var src2 = Reg0(2);
            EmitLoad(dst, m.Mem(dt, m.IAdd(src1, src2)));
        }

        private void RewriteLoadOffset(PrimitiveType dt)
        {
            var dst = Reg(0);
            var baseReg = Reg0(1);
            var offset = Imm(2);
            if (baseReg.IsZero)
            {
                EmitLoad(dst, m.Mem(dt, Address.Ptr32(offset.ToUInt32())));
            }
            else
            {
                EmitLoad(dst, m.Mem(dt, m.AddSubSignedInt(baseReg, offset.ToInt32())));
            }
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> fn)
        {
            var dst = Reg(0);
            var src1 = Reg0(1);
            var src2 = Reg0(2);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteLogicalImm(Func<Expression, Expression, Expression> fn)
        {
            var dst = Reg(0);
            var src1 = Reg0(1);
            var src2 = Imm(2);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteMul()
        {
            var dst = Reg(0);
            var src1 = Reg0(1);
            var src2 = Reg0(2);
            m.Assign(dst, m.IMul(src1, src2));
        }

        private void RewriteOr()
        {
            var dst = Reg(0);
            var regA = ((RegisterOperand) instrCur.Operands[1]).Register;
            var regB = ((RegisterOperand) instrCur.Operands[2]).Register;
            Expression src;
            if (regA == Registers.GpRegs[0])
            {
                if (regB == Registers.GpRegs[0])
                {
                    src = Constant.Zero(dst.DataType);
                }
                else
                {
                    src = binder.EnsureRegister(regB);
                }
            }
            else
            {
                if (regB == Registers.GpRegs[0])
                {
                    src = binder.EnsureRegister(regA);
                }
                else
                {
                    var a = binder.EnsureRegister(regA);
                    var b = binder.EnsureRegister(regB);
                    src = m.Or(a, b);
                }
            }
            m.Assign(dst, src);
        }

        private void RewriteOri()
        {
            var dst = Reg(0);
            var reg = ((RegisterOperand) instrCur.Operands[1]).Register;
            var imm = Imm(2);
            Expression src;
            if (reg == Registers.GpRegs[0])
            {
                if (imm.IsZero)
                {
                    src = Constant.Zero(dst.DataType);
                }
                else
                {
                    src = imm;
                }
            }
            else
            {
                if (imm.IsZero)
                {
                    src = binder.EnsureRegister(reg);
                }
                else
                {
                    var a = binder.EnsureRegister(reg);
                    src = m.Or(a, imm);
                }
            }
            m.Assign(dst, src);
        }

        private void RewriteRsub(bool setCy)
        {
            var dst = Reg(0);
            var regA = ((RegisterOperand) instrCur.Operands[2]).Register;
            var regB = ((RegisterOperand) instrCur.Operands[1]).Register;
            Expression src;
            if (regA == Registers.GpRegs[0])
            {
                setCy = false;
                if (regB == Registers.GpRegs[0])
                {
                    src = Constant.Zero(dst.DataType);
                }
                else
                {
                    src = m.Neg(binder.EnsureRegister(regB));
                }
            }
            else
            {
                if (regB == Registers.GpRegs[0])
                {
                    setCy = false;
                    src = binder.EnsureRegister(regA);
                }
                else
                {
                    var a = binder.EnsureRegister(regA);
                    var b = binder.EnsureRegister(regB);
                    src = m.ISub(a, b);
                }
            }
            m.Assign(dst, src);
            if (setCy)
            {
                C(dst);
            }
        }

        private void RewriteRsubi(bool setCy)
        {
            var dst = Reg(0);
            var imm = ((ImmediateOperand) instrCur.Operands[2]).Value;
            var reg = ((RegisterOperand) instrCur.Operands[1]).Register;
            Expression src;
            if (reg == Registers.GpRegs[0])
            {
                setCy = false;
                src = imm;
            }
            else
            {
                src = binder.EnsureRegister(reg);
                if (imm.IsZero)
                {
                    setCy = false;
                    src = m.Neg(src);
                }
                else
                {
                    src = m.ISub(imm, src);
                }
            }
            m.Assign(dst, src);
            if (setCy)
            {
                C(dst);
            }
        }

        private void RewriteRtsd()
        {
            var linkReg = Reg(0);
            var offset = Imm(1);
            if (linkReg.Storage == Registers.GpRegs[15] && 
                offset.ToInt32() == 8)
            {
                m.ReturnD(0, 0);
            }
            else
            {
                m.GotoD(m.IAdd(linkReg, offset));
            }
        }

        private void RewriteSext(PrimitiveType dt)
        {
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Slice(dt, Reg0(1), 0));
            m.Assign(Reg(0), m.Cast(PrimitiveType.Int32, tmp));
        }

        private void RewriteShift1(Func<Expression, Expression, Expression> fn)
        {
            var dst = Reg(0);
            var src = Reg(1);
            m.Assign(dst, fn(src, m.Int32(1)));
            C(dst);
        }

        private void RewriteStoreIdx(PrimitiveType dt)
        {
            var src = Reg(0);
            var baseReg = Reg0(1);
            var idxReg = Reg0(2);
            EmitStore(m.Mem(dt, m.IAdd(baseReg, idxReg)), src);
        }

        private void RewriteStoreOffset(PrimitiveType dt)
        {
            var src = Reg(0);
            var baseReg = Reg0(1);
            var offset = Imm(2);
            if (baseReg.IsZero)
            {
                EmitStore(m.Mem(dt, Address.Ptr32(offset.ToUInt32())), src);
            }
            else
            {
                EmitStore(m.Mem(dt, m.AddSubSignedInt(baseReg, offset.ToInt32())), src);
            }
        }
    }
}