#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public partial class PowerPcRewriter : IEnumerable<RtlInstructionCluster>
    {
        private Frame frame;
        private RtlEmitter emitter;
        private PowerPcArchitecture arch;
        private IEnumerator<PowerPcInstruction> dasm;
        private IRewriterHost host;
        private PowerPcInstruction instr;

        public PowerPcRewriter(PowerPcArchitecture arch, IEnumerable<PowerPcInstruction> instrs, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.frame = frame;
            this.host = host;
            this.dasm = instrs.GetEnumerator();
        }

        public PowerPcRewriter(PowerPcArchitecture arch, ImageReader rdr, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            //this.state = ppcState;
            this.frame = frame;
            this.host = host;
            this.dasm = arch.CreateDisassembler(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var cluster = new RtlInstructionCluster(instr.Address, 4);
                this.emitter = new RtlEmitter(cluster.Instructions);
                Expression src;
                Expression dst;
                switch (dasm.Current.Opcode)
                {
                default: throw new AddressCorrelatedException(
                    instr.Address,
                    "PowerPC instruction '{0}' is not supported yet.",
                    instr);
                case Opcode.addi: RewriteAddi(); break;
                case Opcode.addc: RewriteAddc(); break;
                case Opcode.addic: RewriteAddic(); break;
                case Opcode.addis: RewriteAddis(); break;
                case Opcode.add: RewriteAdd(); break;
                case Opcode.adde: RewriteAdde(); break;
                case Opcode.addze: RewriteAddze(); break;
                case Opcode.and: RewriteAnd(false); break;
                case Opcode.andi: RewriteAnd(false); break;
                case Opcode.andis: RewriteAndis(); break;
                case Opcode.b: RewriteB(); break;
                case Opcode.bc: RewriteBc(false); break;
                case Opcode.bcctr: RewriteBcctr(false); break;
                case Opcode.bctrl: RewriteBcctr(true); break;
                case Opcode.beq: RewriteBranch(false, ConditionCode.EQ); break;
                case Opcode.beql: RewriteBranch(true, ConditionCode.EQ); break;
                case Opcode.bge: RewriteBranch(false, ConditionCode.GE); break;
                case Opcode.bgel: RewriteBranch(true, ConditionCode.GE); break;
                case Opcode.bgt: RewriteBranch(false, ConditionCode.GT); break;
                case Opcode.bgtl: RewriteBranch(true, ConditionCode.GT); break;
                case Opcode.bl: RewriteBl(); break;
                case Opcode.blr: RewriteBlr(); break;
                case Opcode.ble: RewriteBranch(false, ConditionCode.LE); break;
                case Opcode.blel: RewriteBranch(true, ConditionCode.LE); break;
                case Opcode.blt: RewriteBranch(false, ConditionCode.LT); break;
                case Opcode.bltl: RewriteBranch(true, ConditionCode.LT); break;
                case Opcode.bne: RewriteBranch(false, ConditionCode.NE); break;
                case Opcode.bnel: RewriteBranch(true, ConditionCode.NE); break;
                case Opcode.bns: RewriteBranch(false, ConditionCode.NO); break;
                case Opcode.bnsl: RewriteBranch(true, ConditionCode.NO); break;
                case Opcode.bso: RewriteBranch(false, ConditionCode.OV); break;
                case Opcode.bsol: RewriteBranch(true, ConditionCode.OV); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.cmpli: RewriteCmpli(); break;
                case Opcode.cmplw: RewriteCmplw(); break;
                case Opcode.cmpwi: RewriteCmpwi(); break;
                case Opcode.cntlzw: RewriteCntlzw(); break;
                case Opcode.creqv: RewriteCreqv(); break;
                case Opcode.cror: RewriteCror(); break;
                case Opcode.crxor: RewriteCrxor(); break;
                case Opcode.divw: RewriteDivw(); break;
                case Opcode.divwu: RewriteDivwu(); break;
                case Opcode.extsb: RewriteExtsb(); break;
                case Opcode.fctiwz: RewriteFctiwz(); break;
                case Opcode.fdiv: RewriteFdiv(); break;
                case Opcode.fmr: RewriteFmr(); break;
                case Opcode.fcmpu: RewriteFcmpu(); break;
                case Opcode.fadd: RewriteFadd(); break;
                case Opcode.fmadd: RewriteFmadd(); break;
                case Opcode.fmul: RewriteFmul(); break;
                case Opcode.fneg: RewriteFneg(); break;
                case Opcode.fsub: RewriteFsub(); break;
                case Opcode.lbz: RewriteLz(PrimitiveType.Byte); break;
                case Opcode.lbzx: RewriteLzx(PrimitiveType.Byte); break;
                case Opcode.lbzu: RewriteLzu(PrimitiveType.Byte); break;
                case Opcode.lbzux: RewriteLzux(PrimitiveType.Byte); break;
                case Opcode.lfd: RewriteLfd(); break;
                case Opcode.lfs: RewriteLfs(); break;
                case Opcode.lha: RewriteLha(); break;
                case Opcode.lhz: RewriteLz(PrimitiveType.Word16); break;
                case Opcode.lhzx: RewriteLzx(PrimitiveType.Word16); break;
                case Opcode.lwbrx: RewriteLwbrx(); break;
                case Opcode.lwz: RewriteLz(PrimitiveType.Word32); break;
                case Opcode.lwzu: RewriteLzu(PrimitiveType.Word32); break;
                case Opcode.lwzx: RewriteLzx(PrimitiveType.Word32); break;
                case Opcode.mcrf: RewriteMcrf(); break;
                case Opcode.mfcr:
                    dst = RewriteOperand(instr.op1);
                    src = frame.EnsureRegister(Registers.cr);
                    emitter.Assign(dst, src);
                    break;
                case Opcode.mflr: RewriteMflr(); break;
                case Opcode.mtcrf: RewriteMtcrf(); break;
                case Opcode.mtctr: RewriteMtctr(); break;
                case Opcode.mtlr: RewriteMtlr(); break;
                case Opcode.mulhw: RewriteMulhw(); break;
                case Opcode.mulhwu: RewriteMulhwu(); break;
                case Opcode.mulli: RewriteMullw(); break;
                case Opcode.mullw: RewriteMullw(); break;
                case Opcode.neg: RewriteNeg(); break;
                case Opcode.nand: RewriteAnd(true); break;
                case Opcode.nor: RewriteOr(true); break;
                case Opcode.or: RewriteOr(false); break;
                case Opcode.orc: RewriteOrc(false); break;
                case Opcode.ori: RewriteOr(false); break;
                case Opcode.oris: RewriteOris(); break;
                case Opcode.rlwinm: RewriteRlwinm(); break;
                case Opcode.rlwimi: RewriteRlwimi(); break;
                case Opcode.slw: RewriteSlw(); break;
                case Opcode.sraw: RewriteSraw(); break;
                case Opcode.srawi: RewriteSraw(); break;
                case Opcode.srw: RewriteSrw(); break;
                case Opcode.stb: RewriteSt(PrimitiveType.Byte); break;
                case Opcode.stbu: RewriteStu(PrimitiveType.Byte); break;
                case Opcode.stbux: RewriteStux(PrimitiveType.Byte); break;
                case Opcode.stbx: RewriteStx(PrimitiveType.Byte); break;
                case Opcode.stfd: RewriteStfd(); break;
                case Opcode.sth: RewriteSt(PrimitiveType.Word16); break;
                case Opcode.sthu: RewriteStu(PrimitiveType.Word16); break;
                case Opcode.sthx: RewriteStx(PrimitiveType.Word16); break;
                case Opcode.stw: RewriteSt(PrimitiveType.Word32); break;
                case Opcode.stwbrx: RewriteStwbrx(); break;
                case Opcode.stwu: RewriteStu(PrimitiveType.Word32); break;
                case Opcode.stwux: RewriteStux(PrimitiveType.Word32); break;
                case Opcode.stwx: RewriteStx(PrimitiveType.Word32); break;
                case Opcode.subf: RewriteSubf(); break;
                case Opcode.subfc: RewriteSubfc(); break;
                case Opcode.subfe: RewriteSubfe(); break;
                case Opcode.subfic: RewriteSubfic(); break;
                case Opcode.subfze: RewriteSubfze(); break;
                case Opcode.xor: RewriteXor(); break;
                case Opcode.xori: RewriteXor(); break;
                case Opcode.xoris: RewriteXoris(); break;
                }
                yield return cluster;
            }
        }

        private Expression Shift16(MachineOperand machineOperand)
        {
            var imm = (ImmediateOperand)machineOperand;
            return Constant.Word32(imm.Value.ToInt32() << 16);
        }

        private Expression RewriteOperand(MachineOperand op, bool maybe0 = false)
        {
            var rOp = op as RegisterOperand;
            if (rOp != null)
            {
                if (maybe0 && rOp.Register.Number == 0)
                    return Constant.Zero(rOp.Register.DataType);
                return frame.EnsureRegister(rOp.Register);
            }
            var iOp = op as ImmediateOperand;
            if (iOp != null)
            {
                // Sign-extend the bastard.
                PrimitiveType iType = (PrimitiveType)iOp.Value.DataType;
                if (arch.WordWidth.BitSize == 64)
                {
                    return (iType.Domain == Domain.SignedInt)
                        ? Constant.Int64(iOp.Value.ToInt64())
                        : Constant.Word64(iOp.Value.ToUInt64());
                }
                else
                {
                    return (iType.Domain == Domain.SignedInt)
                        ? Constant.Int32(iOp.Value.ToInt32())
                        : Constant.Word32(iOp.Value.ToUInt32());

                }
            }
            var aOp = op as AddressOperand;
            if (aOp != null)
                return aOp.Address;
            
            throw new NotImplementedException(
                string.Format("RewriteOperand:{0} ({1}}}", op, op.GetType()));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression EffectiveAddress(MachineOperand operand, RtlEmitter emitter)
        {
            var mop = (MemoryOperand) operand;
            var reg = frame.EnsureRegister(mop.BaseRegister);
            var offset = mop.Offset;
            return emitter.IAdd(reg, offset);
        }

        private Expression EffectiveAddress_r0(MachineOperand operand, RtlEmitter emitter)
        {
            var mop = (MemoryOperand) operand;
            if (mop.BaseRegister.Number == 0)
            {
                return Constant.Word32((int) mop.Offset.ToInt16());
            }
            else
            {
                var reg = frame.EnsureRegister(mop.BaseRegister);
                var offset = mop.Offset;
                return emitter.IAdd(reg, offset);
            }
        }

        //$REVIEW: push PseudoProc into the RewriterHost interface"
        public Expression PseudoProc(string name, DataType retType, params Expression[] args)
        {
            var ppp = host.EnsurePseudoProcedure(name, retType, args.Length);
            return PseudoProc(ppp, retType, args);
        }

        public Expression PseudoProc(PseudoProcedure ppp, DataType retType, params Expression[] args)
        {
            if (args.Length != ppp.Arity)
                throw new ArgumentOutOfRangeException(
                    string.Format("Pseudoprocedure {0} expected {1} arguments, but was passed {2}.",
                    ppp.Name,
                    ppp.Arity,
                    args.Length));

            return emitter.Fn(new ProcedureConstant(arch.PointerType, ppp), retType, args);
        }

        private Expression UpdatedRegister(Expression effectiveAddress)
        {
            var bin = (BinaryExpression) effectiveAddress;
            return bin.Left;
        }
    }
}
