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
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Mos6502
{
    public class Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private ProcessorState state;
        private Frame frame;
        private IRewriterHost host;
        private Mos6502ProcessorArchitecture arch;
        private IEnumerable<Instruction> instrs;
        private Instruction instrCur;
        private RtlInstructionCluster ric;
        private RtlEmitter emitter;

        public Rewriter(Mos6502ProcessorArchitecture arch, ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.frame = frame;
            this.host = host;
            this.instrs = new Disassembler(rdr.CreateLeReader());
        }

        private AddressCorrelatedException NYI()
        {
            return new AddressCorrelatedException(
                instrCur.Address,
                "Rewriting 6502 opcode '{0}' is not supported yet.",
                instrCur.Code);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            var dasm = this.instrs.GetEnumerator();
            while (dasm.MoveNext())
            {
                this.instrCur = dasm.Current;
                this.ric = new RtlInstructionCluster(instrCur.Address, instrCur.Length);
                this.emitter = new RtlEmitter(ric.Instructions);
                switch (instrCur.Code)
                {
                default: throw NYI();
                case Opcode.adc: Adc(); break;
                case Opcode.and: And(); break;
                case Opcode.asl: Asl(); break;
                case Opcode.bcc: Branch(ConditionCode.UGE, FlagM.CF); break;
                case Opcode.bcs: Branch(ConditionCode.ULT, FlagM.CF); break;
                case Opcode.beq: Branch(ConditionCode.EQ, FlagM.ZF); break;
                case Opcode.bit: Bit(); break;
                case Opcode.bmi: Branch(ConditionCode.SG, FlagM.NF); break;
                case Opcode.bne: Branch(ConditionCode.NE, FlagM.ZF); break;
                case Opcode.bpl: Branch(ConditionCode.NS, FlagM.NF); break;
                case Opcode.brk: Brk(); break;
                case Opcode.bvc: Branch(ConditionCode.NO, FlagM.VF); break;
                case Opcode.bvs: Branch(ConditionCode.OV, FlagM.VF); break;
                case Opcode.clc: SetFlag(FlagM.CF, false); break;
                case Opcode.cld: SetFlag(FlagM.DF, false); break;
                case Opcode.cli: SetFlag(FlagM.IF, false); break;
                case Opcode.clv: SetFlag(FlagM.VF, false); break;
                case Opcode.cmp: Cmp(Registers.a); break;
                case Opcode.cpx: Cmp(Registers.x); break;
                case Opcode.cpy: Cmp(Registers.y); break;
                case Opcode.dec: Dec(); break;
                case Opcode.dex: Dec(Registers.x); break;
                case Opcode.dey: Dec(Registers.y); break;
                case Opcode.eor: Eor(); break;
                case Opcode.inc: Inc(); break;
                case Opcode.inx: Inc(Registers.x); break;
                case Opcode.iny: Inc(Registers.y); break;
                case Opcode.jmp: Jmp(); break;
                case Opcode.jsr: Jsr(); break;
                case Opcode.lda: Ld(Registers.a); break;
                case Opcode.ldx: Ld(Registers.x); break;
                case Opcode.ldy: Ld(Registers.y); break;
                case Opcode.lsr: Lsr(); break;
                case Opcode.nop: continue; 
                case Opcode.ora: Ora(); break;
                case Opcode.pha: Push(Registers.a); break;
                case Opcode.php: Push(AllRegs()); break;
                case Opcode.pla: Pull(Registers.a); break;
                case Opcode.rol: Rotate(PseudoProcedure.Rol); break;
                case Opcode.ror: Rotate(PseudoProcedure.Ror); break;
                case Opcode.rti: Rti(); break;
                case Opcode.rts: Rts(); break;
                case Opcode.sbc: Sbc(); break;
                case Opcode.sec: SetFlag(FlagM.CF, true); break;
                case Opcode.sed: SetFlag(FlagM.DF, true); break;
                case Opcode.sei: SetFlag(FlagM.IF, true); break;
                case Opcode.sta: St(Registers.a); break;
                case Opcode.stx: St(Registers.x); break;
                case Opcode.sty: St(Registers.y); break;
                case Opcode.tax: Copy(Registers.x, Registers.a); break;
                case Opcode.tay: Copy(Registers.y, Registers.a); break;
                case Opcode.tsx: Copy(Registers.x, Registers.s); break;
                case Opcode.txa: Copy(Registers.a, Registers.x); break;
                case Opcode.txs: Copy(Registers.s, Registers.x); break;
                case Opcode.tya: Copy(Registers.a, Registers.y); break;
                }
                yield return ric;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Identifier AllRegs()
        {
            return FlagGroupStorage(FlagM.NF | FlagM.VF | FlagM.BF | FlagM.DF | FlagM.IF | FlagM.ZF | FlagM.CF);
        }

        private void Copy(RegisterStorage regDst, RegisterStorage regSrc)
        {
            var dst = frame.EnsureRegister(regDst);
            var src = frame.EnsureRegister(regSrc);
            emitter.Assign(dst, src);
            emitter.Assign(
                frame.EnsureFlagGroup(
                    (uint) (FlagM.NF | FlagM.ZF),
                    "NZ",
                    PrimitiveType.Byte),
                emitter.Cond(dst));
        }


        private void Branch(ConditionCode cc, FlagM flags)
        {
            var f = FlagGroupStorage(flags);
            emitter.Branch(
                emitter.Test(cc, f),
                Address.Ptr16(instrCur.Operand.Offset.ToUInt16()),
                RtlClass.ConditionalTransfer);
        }

        private Identifier FlagGroupStorage(FlagM flags)
        {
            uint f = (uint) flags;
            var sb = new StringBuilder();
            for (int iReg = Registers.N.Number; f != 0; ++iReg, f >>= 1)
            {
                if ((f & 1) != 0)
                    sb.Append(Registers.GetRegister(iReg));
            }
            return frame.EnsureFlagGroup((uint)flags, sb.ToString(), PrimitiveType.Byte);
        }

        private void Asl()
        {
            var mem = RewriteOperand(instrCur.Operand);
            var tmp = frame.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            emitter.Assign(tmp, emitter.Shl(mem, 1));
            emitter.Assign(mem, tmp);
            emitter.Assign(c, emitter.Cond(tmp));
        }

        private void Lsr()
        {
            var mem = RewriteOperand(instrCur.Operand);
            var tmp = frame.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            emitter.Assign(tmp, emitter.Shr(mem, 1));
            emitter.Assign(mem, tmp);
            emitter.Assign(c, emitter.Cond(tmp));
        }

        private void Bit()
        {
            var a = frame.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operand);
            var tmp = frame.CreateTemporary(PrimitiveType.Byte);
            var flags = FlagGroupStorage(FlagM.NF | FlagM.VF | FlagM.CF);
            emitter.Assign(tmp, emitter.And(a, mem));
            emitter.Assign(flags, emitter.Cond(tmp));
        }

        private void Brk()
        {
            emitter.Call(
                new ProcedureConstant(
                    new Pointer(new CodeType(), 2),
                    host.EnsurePseudoProcedure("__brk", VoidType.Instance, 0)),
                2);
        }

        private void Cmp(RegisterStorage r)
        {
            var a = frame.EnsureRegister(r);
            var mem = RewriteOperand(instrCur.Operand);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            emitter.Assign(c, emitter.Cond(emitter.ISub(a, mem)));
        }

        private void Dec()
        {
            var mem = RewriteOperand(instrCur.Operand);
            var tmp = frame.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF|FlagM.ZF);
            emitter.Assign(tmp, emitter.ISub(mem, 1));
            emitter.Assign(RewriteOperand(instrCur.Operand), tmp);
            emitter.Assign(c, emitter.Cond(tmp));
        }

        private void Dec(RegisterStorage reg)
        {
            var id = frame.EnsureRegister(reg);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            emitter.Assign(id, emitter.ISub(id, 1));
            emitter.Assign(c, emitter.Cond(id));
        }

        private void Inc()
        {
            var mem = RewriteOperand(instrCur.Operand);
            var tmp = frame.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            emitter.Assign(tmp, emitter.IAdd(mem, 1));
            emitter.Assign(RewriteOperand(instrCur.Operand), tmp);
            emitter.Assign(c, emitter.Cond(tmp));
        }

        private void Inc(RegisterStorage reg)
        {
            var id = frame.EnsureRegister(reg);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            emitter.Assign(id, emitter.IAdd(id, 1));
            emitter.Assign(c, emitter.Cond(id));
        }

        private void Jmp()
        {
            var mem = (MemoryAccess) RewriteOperand(instrCur.Operand);
            emitter.Goto(mem.EffectiveAddress);
        }

        private void Jsr()
        {
            var mem  = (MemoryAccess) RewriteOperand(instrCur.Operand);
            emitter.Call(mem.EffectiveAddress, 2);
        }

        private void Ld(RegisterStorage reg)
        {
            var r = frame.EnsureRegister(reg);
            var mem = RewriteOperand(instrCur.Operand);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            emitter.Assign(r, mem);
            emitter.Assign(c, emitter.Cond(r));
        }


        private void And()
        {
            var a = frame.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operand);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            emitter.Assign(
                a,
                emitter.And(a, mem));
            emitter.Assign(c, emitter.Cond(a));
        }

        private void Eor()
        {
            var a = frame.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operand);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            emitter.Assign(
                a,
                emitter.Xor(a, mem));
            emitter.Assign(c, emitter.Cond(a));
        }

        private void Ora()
        {
            var a = frame.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operand);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            emitter.Assign(
                a,
                emitter.Or(a, mem));
            emitter.Assign(c, emitter.Cond(a));
        }

        private void Push(RegisterStorage reg)
        {
            Push(frame.EnsureRegister(reg));
        }

        private void Push(Identifier reg)
        {
            var s = frame.EnsureRegister(arch.StackRegister);
            emitter.Assign(s, emitter.ISub(s, 1));
            emitter.Assign(emitter.LoadB(s), reg);
        }

        private void Pull(RegisterStorage reg)
        {
            var s = frame.EnsureRegister(arch.StackRegister);
            var c = FlagGroupStorage(FlagM.NF|FlagM.ZF);
            var r = frame.EnsureRegister(reg);
            emitter.Assign(r, emitter.LoadB(s));
            emitter.Assign(s, emitter.IAdd(s, 1));
            emitter.Assign(c, emitter.Cond(r));
        }

        private void Plp()
        {
            var s = frame.EnsureRegister(arch.StackRegister);
            var c = AllRegs();
            emitter.Assign(c, emitter.LoadB(s));
            emitter.Assign(s, emitter.IAdd(s, 1));
        }

        private void Rotate(string rot)
        {
            var fn = new ProcedureConstant(
                new Pointer(new CodeType(), 2),
                host.EnsurePseudoProcedure(rot, PrimitiveType.Byte, 2));
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            var arg = RewriteOperand(instrCur.Operand);
            emitter.Assign(arg, emitter.Fn(fn, arg, Constant.Byte(1)));
            emitter.Assign(c, emitter.Cond(arg));
        }

        private void Rti()
        {
            Plp();
            Rts();
        }

        private void Rts()
        {
            emitter.Return(2, 0);
        }

        private void Adc()
        {
            var mem = RewriteOperand(instrCur.Operand);
            var a = frame.EnsureRegister(Registers.a);
            var c = frame.EnsureFlagGroup((uint) FlagM.CF, "C", PrimitiveType.Bool);
            emitter.Assign(
                a,
                emitter.IAdd(
                    emitter.IAdd(a, mem),
                    c));
            emitter.Assign(
                frame.EnsureFlagGroup((uint) Instruction.DefCc(instrCur.Code), "NVZC", PrimitiveType.Byte),
                emitter.Cond(a));
        }

        private void Sbc()
        {
            var mem = RewriteOperand(instrCur.Operand);
            var a = frame.EnsureRegister(Registers.a);
            var c = frame.EnsureFlagGroup((uint) FlagM.CF, "C", PrimitiveType.Bool);
            emitter.Assign(
                a,
                emitter.ISub(
                    emitter.ISub(a, mem),
                    emitter.Not(c)));
            emitter.Assign(
                frame.EnsureFlagGroup((uint) Instruction.DefCc(instrCur.Code), "NVZC", PrimitiveType.Byte),
                emitter.Cond(a));
        }

        private void SetFlag(FlagM flag, bool value)
        {
            var reg = FlagGroupStorage(flag);
            var v = Constant.Bool(value);
            emitter.Assign(reg, v);
        }
        private void St(RegisterStorage reg)
        {
            var mem = RewriteOperand(instrCur.Operand);
            var id = frame.EnsureRegister(reg);
            emitter.Assign(mem, id);
        }

        private Expression RewriteOperand(Operand op)
        {
            Constant offset;
            switch (op.Mode)
            {
            default: throw new NotImplementedException("Unimplemented address mode " + op.Mode);
            case AddressMode.Accumulator:
                return frame.EnsureRegister(Registers.a);
            case AddressMode.Immediate:
                return op.Offset;
            case AddressMode.IndirectIndexed:
                var y = frame.EnsureRegister(Registers.y);
                offset = Constant.Word16((ushort) op.Offset.ToByte());
                return emitter.LoadB(
                    emitter.IAdd(
                        emitter.Load(PrimitiveType.Ptr16, offset),
                        emitter.Cast(PrimitiveType.UInt16, y)));
            case AddressMode.IndexedIndirect:
                var x = frame.EnsureRegister(Registers.x);
                offset = Constant.Word16((ushort) op.Offset.ToByte());
                return emitter.LoadB(
                    emitter.Load(
                        PrimitiveType.Ptr16,
                        emitter.IAdd(
                            offset,
                            emitter.Cast(PrimitiveType.UInt16, x))));
            case AddressMode.Absolute:
                return emitter.LoadB(op.Offset);
            case AddressMode.AbsoluteX:
                return emitter.LoadB(emitter.IAdd(op.Offset, frame.EnsureRegister(Registers.x)));
            case AddressMode.ZeroPage:
                if (op.Register != null)
                {
                    return emitter.LoadB(
                        emitter.IAdd(
                            Constant.Create(PrimitiveType.Ptr16, op.Offset.ToUInt16()),
                            frame.EnsureRegister(op.Register)));
                }
                else
                {
                    return emitter.LoadB(
                        Constant.Create(PrimitiveType.Ptr16, op.Offset.ToUInt16()));
                }
            }
        }
    }
}
