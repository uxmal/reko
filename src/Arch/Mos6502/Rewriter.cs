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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mos6502
{
    public class Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly Mos6502Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<Instruction> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private Instruction instrCur;
        private InstrClass iclass;

        public Rewriter(Mos6502Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.dasm = new Disassembler(arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.instrCur = default!;
        }

        private AddressCorrelatedException NYI()
        {
            return new AddressCorrelatedException(
                instrCur.Address,
                "Rewriting 6502 instruction '{0}' is not supported yet.",
                instrCur.Mnemonic);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instrCur = dasm.Current;
                this.iclass = instrCur.InstructionClass;
                switch (instrCur.Mnemonic)
                {
                default:
                    EmitUnitTest(instrCur);
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.illegal: m.Invalid(); break;
                case Mnemonic.adc: Adc(); break;
                case Mnemonic.and: And(); break;
                case Mnemonic.asl: Asl(); break;
                case Mnemonic.bcc: Branch(ConditionCode.UGE, FlagM.CF); break;
                case Mnemonic.bcs: Branch(ConditionCode.ULT, FlagM.CF); break;
                case Mnemonic.beq: Branch(ConditionCode.EQ, FlagM.ZF); break;
                case Mnemonic.bit: Bit(); break;
                case Mnemonic.bmi: Branch(ConditionCode.SG, FlagM.NF); break;
                case Mnemonic.bne: Branch(ConditionCode.NE, FlagM.ZF); break;
                case Mnemonic.bpl: Branch(ConditionCode.NS, FlagM.NF); break;
                case Mnemonic.brk: Brk(); break;
                case Mnemonic.bvc: Branch(ConditionCode.NO, FlagM.VF); break;
                case Mnemonic.bvs: Branch(ConditionCode.OV, FlagM.VF); break;
                case Mnemonic.clc: SetFlag(FlagM.CF, false); break;
                case Mnemonic.cld: SetFlag(FlagM.DF, false); break;
                case Mnemonic.cli: SetFlag(FlagM.IF, false); break;
                case Mnemonic.clv: SetFlag(FlagM.VF, false); break;
                case Mnemonic.cmp: Cmp(Registers.a); break;
                case Mnemonic.cpx: Cmp(Registers.x); break;
                case Mnemonic.cpy: Cmp(Registers.y); break;
                case Mnemonic.dec: Dec(); break;
                case Mnemonic.dex: Dec(Registers.x); break;
                case Mnemonic.dey: Dec(Registers.y); break;
                case Mnemonic.eor: Eor(); break;
                case Mnemonic.inc: Inc(); break;
                case Mnemonic.inx: Inc(Registers.x); break;
                case Mnemonic.iny: Inc(Registers.y); break;
                case Mnemonic.jmp: Jmp(); break;
                case Mnemonic.jsr: Jsr(); break;
                case Mnemonic.lda: Ld(Registers.a); break;
                case Mnemonic.ldx: Ld(Registers.x); break;
                case Mnemonic.ldy: Ld(Registers.y); break;
                case Mnemonic.lsr: Lsr(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.ora: Ora(); break;
                case Mnemonic.pha: Push(Registers.a); break;
                case Mnemonic.php: Push(AllFlags()); break;
                case Mnemonic.pla: Pull(Registers.a); break;
                case Mnemonic.plp: Plp(); break;
                case Mnemonic.rol: Rotate(IntrinsicProcedure.Rol); break;
                case Mnemonic.ror: Rotate(IntrinsicProcedure.Ror); break;
                case Mnemonic.rti: Rti(); break;
                case Mnemonic.rts: Rts(); break;
                case Mnemonic.sbc: Sbc(); break;
                case Mnemonic.sec: SetFlag(FlagM.CF, true); break;
                case Mnemonic.sed: SetFlag(FlagM.DF, true); break;
                case Mnemonic.sei: SetFlag(FlagM.IF, true); break;
                case Mnemonic.sta: St(Registers.a); break;
                case Mnemonic.stx: St(Registers.x); break;
                case Mnemonic.sty: St(Registers.y); break;
                case Mnemonic.tax: Copy(Registers.x, Registers.a); break;
                case Mnemonic.tay: Copy(Registers.y, Registers.a); break;
                case Mnemonic.tsx: Copy(Registers.x, Registers.s); break;
                case Mnemonic.txa: Copy(Registers.a, Registers.x); break;
                case Mnemonic.txs: Copy(Registers.s, Registers.x); break;
                case Mnemonic.tya: Copy(Registers.a, Registers.y); break;
                }
                yield return m.MakeCluster(instrCur.Address, instrCur.Length, iclass);
                instrs.Clear();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Identifier AllFlags()
        {
            return FlagGroupStorage(FlagM.NF | FlagM.VF | FlagM.BF | FlagM.DF | FlagM.IF | FlagM.ZF | FlagM.CF);
        }

        private void Copy(RegisterStorage regDst, RegisterStorage regSrc)
        {
            var dst = binder.EnsureRegister(regDst);
            var src = binder.EnsureRegister(regSrc);
            m.Assign(dst, src);
            m.Assign(
                binder.EnsureFlagGroup(
                    Registers.p,
                    (uint) (FlagM.NF | FlagM.ZF),
                    "NZ",
                    PrimitiveType.Byte),
                m.Cond(dst));
        }

        private void Branch(ConditionCode cc, FlagM flags)
        {
            var f = FlagGroupStorage(flags);
            m.Branch(
                m.Test(cc, f),
                Address.Ptr16(((Operand)instrCur.Operands[0]).Offset!.ToUInt16()),
                iclass);
        }

        private Identifier FlagGroupStorage(FlagM flags)
        {
            var grf = arch.GetFlagGroup(Registers.p, (uint)flags);
            return binder.EnsureFlagGroup(grf);
        }

        private void Asl()
        {
            var mem = RewriteOperand(instrCur.Operands[0]);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            m.Assign(tmp, m.Shl(mem, 1));
            m.Assign(mem, tmp);
            m.Assign(c, m.Cond(tmp));
        }

        private void Lsr()
        {
            var mem = RewriteOperand(instrCur.Operands[0]);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            m.Assign(tmp, m.Shr(mem, 1));
            m.Assign(mem, tmp);
            m.Assign(c, m.Cond(tmp));
        }

        private void Bit()
        {
            var a = binder.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operands[0]);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var flags = FlagGroupStorage(FlagM.NF | FlagM.VF | FlagM.CF);
            m.Assign(tmp, m.And(a, mem));
            m.Assign(flags, m.Cond(tmp));
        }

        private void Brk()
        {
            m.SideEffect(host.Intrinsic("__brk", false, VoidType.Instance));
        }

        private void Cmp(RegisterStorage r)
        {
            var a = binder.EnsureRegister(r);
            var mem = RewriteOperand(instrCur.Operands[0]);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            m.Assign(c, m.Cond(m.ISub(a, mem)));
        }

        private void Dec()
        {
            var mem = RewriteOperand(instrCur.Operands[0]);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF|FlagM.ZF);
            m.Assign(tmp, m.ISub(mem, 1));
            m.Assign(RewriteOperand(instrCur.Operands[0]), tmp);
            m.Assign(c, m.Cond(tmp));
        }

        private void Dec(RegisterStorage reg)
        {
            var id = binder.EnsureRegister(reg);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(id, m.ISub(id, 1));
            m.Assign(c, m.Cond(id));
        }

        private void Inc()
        {
            var mem = RewriteOperand(instrCur.Operands[0]);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(tmp, m.IAdd(mem, 1));
            m.Assign(RewriteOperand(instrCur.Operands[0]), tmp);
            m.Assign(c, m.Cond(tmp));
        }

        private void Inc(RegisterStorage reg)
        {
            var id = binder.EnsureRegister(reg);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(id, m.IAdd(id, 1));
            m.Assign(c, m.Cond(id));
        }

        private void Jmp()
        {
            var mem = (MemoryAccess)RewriteOperand(instrCur.Operands[0]);
            m.Goto(mem.EffectiveAddress);
        }

        private void Jsr()
        {
            var mem  = (MemoryAccess) RewriteOperand(instrCur.Operands[0]);
            m.Call(mem.EffectiveAddress, 2);
        }

        private void Ld(RegisterStorage reg)
        {
            var r = binder.EnsureRegister(reg);
            var mem = RewriteOperand(instrCur.Operands[0]);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(r, mem);
            m.Assign(c, m.Cond(r));
        }

        private void And()
        {
            var a = binder.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operands[0]);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(
                a,
                m.And(a, mem));
            m.Assign(c, m.Cond(a));
        }

        private void Eor()
        {
            var a = binder.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operands[0]);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(
                a,
                m.Xor(a, mem));
            m.Assign(c, m.Cond(a));
        }

        private void Ora()
        {
            var a = binder.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operands[0]);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(
                a,
                m.Or(a, mem));
            m.Assign(c, m.Cond(a));
        }

        private void Push(RegisterStorage reg)
        {
            Push(binder.EnsureRegister(reg));
        }

        private void Push(Identifier reg)
        {
            var s = binder.EnsureRegister(arch.StackRegister);
            m.Assign(s, m.ISubS(s, 1));
            m.Assign(m.Mem8(s), reg);
        }

        private void Pull(RegisterStorage reg)
        {
            var id = binder.EnsureRegister(reg);
            var s = binder.EnsureRegister(arch.StackRegister);
            var c = FlagGroupStorage(FlagM.NF|FlagM.ZF);
            m.Assign(id, m.Mem8(s));
            m.Assign(s, m.IAddS(s, 1));
            m.Assign(c, m.Cond(id));
        }

        private void Plp()
        {
            var s = binder.EnsureRegister(arch.StackRegister);
            var c = AllFlags();
            m.Assign(c, m.Mem8(s));
            m.Assign(s, m.IAddS(s, 1));
        }

        private void Rotate(string rot)
        {
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            var arg = RewriteOperand(instrCur.Operands[0]);
            m.Assign(arg, host.Intrinsic(rot, true, arg.DataType, arg, Constant.Byte(1)));
            m.Assign(c, m.Cond(arg));
        }

        private void Rti()
        {
            Plp();
            Rts();
        }

        private void Rts()
        {
            m.Return(2, 0);
        }

        private void Adc()
        {
            var mem = RewriteOperand(instrCur.Operands[0]);
            var a = binder.EnsureRegister(Registers.a);
            var c = binder.EnsureFlagGroup(Registers.p, (uint) FlagM.CF, "C", PrimitiveType.Bool);
            m.Assign(
                a,
                m.IAdd(
                    m.IAdd(a, mem),
                    c));
            m.Assign(
                binder.EnsureFlagGroup(Registers.p, (uint) Instruction.DefCc(instrCur.Mnemonic), "NVZC", PrimitiveType.Byte),
                m.Cond(a));
        }

        private void Sbc()
        {
            var mem = RewriteOperand(instrCur.Operands[0]);
            var a = binder.EnsureRegister(Registers.a);
            var c = binder.EnsureFlagGroup(Registers.p, (uint) FlagM.CF, "C", PrimitiveType.Bool);
            m.Assign(
                a,
                m.ISub(
                    m.ISub(a, mem),
                    m.Not(c)));
            m.Assign(
                binder.EnsureFlagGroup(Registers.p, (uint) Instruction.DefCc(instrCur.Mnemonic), "NVZC", PrimitiveType.Byte),
                m.Cond(a));
        }

        private void SetFlag(FlagM flag, bool value)
        {
            var reg = FlagGroupStorage(flag);
            var v = Constant.Bool(value);
            m.Assign(reg, v);
        }

        private void St(RegisterStorage reg)
        {
            var mem = RewriteOperand(instrCur.Operands[0]);
            var id = binder.EnsureRegister(reg);
            m.Assign(mem, id);
        }

        private Expression RewriteOperand(MachineOperand mop)
        {
            Address addrZeroPage;
            var op = (Operand) mop;
            switch (op.Mode)
            {
            default: throw new NotImplementedException("Unimplemented address mode " + op.Mode);
            case AddressMode.Accumulator:
                return binder.EnsureRegister(Registers.a);
            case AddressMode.Immediate:
                return op.Offset!;
            case AddressMode.IndirectIndexed:
                var y = binder.EnsureRegister(Registers.y);
                addrZeroPage = m.Ptr16(op.Offset!.ToByte());
                return m.Mem8(
                    m.IAdd(
                        m.Mem(PrimitiveType.Ptr16, addrZeroPage),
                        m.Convert(y, y.DataType, PrimitiveType.UInt16)));
            case AddressMode.IndexedIndirect:
                var x = binder.EnsureRegister(Registers.x);
                addrZeroPage = m.Ptr16(op.Offset!.ToByte());
                return m.Mem8(
                    m.Mem(
                        PrimitiveType.Ptr16,
                        m.IAdd(
                            addrZeroPage,
                            m.Convert(x, x.DataType, PrimitiveType.UInt16))));
            case AddressMode.Absolute:
                return m.Mem8(arch.MakeAddressFromConstant(op.Offset!, false));
            case AddressMode.AbsoluteX:
            case AddressMode.AbsoluteY:
                return m.Mem8(m.IAdd(
                    arch.MakeAddressFromConstant(op.Offset!, false),
                    binder.EnsureRegister(op.Register!)));
            case AddressMode.ZeroPage:
                 return m.Mem8(arch.MakeAddressFromConstant(op.Offset!, false));
            case AddressMode.ZeroPageX:
            case AddressMode.ZeroPageY:
                return m.Mem8(
                    m.IAdd(
                        arch.MakeAddressFromConstant(op.Offset!, false),
                        binder.EnsureRegister(op.Register!)));
            case AddressMode.Indirect:
                return m.Mem16(m.Mem16(arch.MakeAddressFromConstant(op.Offset!, false)));
            }
        }

        [Conditional("DEBUG")]
        private void EmitUnitTest(Instruction instr)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Rw6502", instr, instr.Mnemonic.ToString(), rdr, "");
        }
    }
}
