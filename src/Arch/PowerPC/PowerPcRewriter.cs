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
        private PowerPcInstruction instr;

        public PowerPcRewriter(PowerPcArchitecture arch, IEnumerable<PowerPcInstruction> instrs, Frame frame)
        {
            this.arch = arch;
            this.dasm = instrs.GetEnumerator();
            this.frame = frame;
        }

        public PowerPcRewriter(PowerPcArchitecture arch, ImageReader rdr, Frame frame)
        {
            this.arch = arch;
            //this.state = ppcState;
            this.frame = frame;
            //this.host = host;
            this.dasm = arch.CreateDisassembler(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var cluster = new RtlInstructionCluster(instr.Address, 4);
                this.emitter = new RtlEmitter(cluster.Instructions);
                Expression op1;
                Expression op2;
                Expression op3;
                Expression ea;
                Expression src;
                Expression dst;
                switch (dasm.Current.Opcode)
                {
                default: throw new AddressCorrelatedException(
                    instr.Address,
                    "PowerPC opcode {0} is not supported yet.",
                    instr.Opcode);
                case Opcode.addi: RewriteAddi(); break;
                case Opcode.addis: RewriteAddis(); break;
                case Opcode.add: RewriteAdd(); break;
                case Opcode.b: RewriteB(); break;
                case Opcode.bl: RewriteBl(); break;
                case Opcode.bcctr: RewriteBcctr(false); break;
                case Opcode.bctrl: RewriteBcctr(true); break;
                case Opcode.fmr: RewriteFmr(); break;
                case Opcode.fmul: RewriteFmul(); break;
                case Opcode.fsub: RewriteFsub(); break;
                case Opcode.lwz: RewriteLwz(); break;
                case Opcode.lwzx: RewriteLwzx(); break;

                case Opcode.mfcr:
                    dst = RewriteOperand(instr.op1);
                    src = frame.EnsureRegister(Registers.cr);
                    emitter.Assign(dst, src);
                    break;
                case Opcode.mflr: RewriteMflr(); break;
                case Opcode.mtctr: RewriteMtctr(); break;
                case Opcode.mtlr: RewriteMtlr(); break;
                case Opcode.neg: RewriteNeg(); break;
                case Opcode.or: RewriteOr(); break;
                case Opcode.oris:
                    emitter.Assign(
                        RewriteOperand(dasm.Current.op1),
                        emitter.Or(
                            RewriteOperand(dasm.Current.op2),
                            Shift16(dasm.Current.op3)));
                    break;
                case Opcode.lwzu:
                    op1 = RewriteOperand(dasm.Current.op1);
                    ea = EffectiveAddress(dasm.Current.op2, emitter);
                    emitter.Assign(op1, emitter.LoadDw(ea));
                    emitter.Assign(UpdatedRegister(ea), ea);
                    break;
                case Opcode.rlwinm: RewriteRlwinm(); break;
                case Opcode.slw: RewriteSlw(); break;
                case Opcode.srawi: RewriteSrawi(); break;
                case Opcode.stbu:
                    op1 = RewriteOperand(dasm.Current.op1);
                    ea = EffectiveAddress(dasm.Current.op2, emitter);
                    emitter.Assign(emitter.LoadB(ea), emitter.Cast(PrimitiveType.Byte, op1));
                    emitter.Assign(UpdatedRegister(ea), ea);
                    break;
                case Opcode.stbux:
                    op1 = RewriteOperand(dasm.Current.op1);
                    op2 = RewriteOperand(dasm.Current.op2);
                    op3 = RewriteOperand(dasm.Current.op3);
                    ea = emitter.IAdd(op2, op3);
                    emitter.Assign(emitter.LoadB(ea), emitter.Cast(PrimitiveType.Byte, op1));
                    emitter.Assign(op2, emitter.IAdd(op2, op3));
                    break;
                case Opcode.stw: RewriteStw(); break;
                case Opcode.stwu: RewriteStwu(); break;
                case Opcode.stwux: RewriteStwux(); break;
                case Opcode.stwx: RewriteStwx(); break;
                case Opcode.subf: RewriteSubf(); break;
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
                Constant c;
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

        private Expression UpdatedRegister(Expression effectiveAddress)
        {
            var bin = (BinaryExpression) effectiveAddress;
            return bin.Left;
        }
    }
}
