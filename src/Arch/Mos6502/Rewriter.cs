#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Rtl;
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
        private readonly Mos6502ProcessorArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<Instruction> dasm;
        private Instruction instrCur;
        private InstrClass rtlc;
        private RtlEmitter m;

        public Rewriter(Mos6502ProcessorArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.dasm = new Disassembler(rdr).GetEnumerator();
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
            while (dasm.MoveNext())
            {
                this.instrCur = dasm.Current;
                var instrs = new List<RtlInstruction>();
                this.rtlc = instrCur.InstructionClass;
                this.m = new RtlEmitter(instrs);
                switch (instrCur.Code)
                {
                default:
                    EmitUnitTest();
                    rtlc = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Opcode.illegal: m.Invalid(); break;
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
                case Opcode.nop: m.Nop(); break;
                case Opcode.ora: Ora(); break;
                case Opcode.pha: Push(Registers.a); break;
                case Opcode.php: Push(AllFlags()); break;
                case Opcode.pla: Pull(Registers.a); break;
                case Opcode.plp: Plp(); break;
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
                yield return new RtlInstructionCluster(
                    instrCur.Address,
                    instrCur.Length,
                    instrs.ToArray())
                {
                    Class = rtlc
                };
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
                Address.Ptr16(instrCur.Operand.Offset.ToUInt16()),
                rtlc);
        }

        private Identifier FlagGroupStorage(FlagM flags)
        {
            var grf = arch.GetFlagGroup((uint)flags);
            return binder.EnsureFlagGroup(grf);
        }

        private void Asl()
        {
            var mem = RewriteOperand(instrCur.Operand);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            m.Assign(tmp, m.Shl(mem, 1));
            m.Assign(mem, tmp);
            m.Assign(c, m.Cond(tmp));
        }

        private void Lsr()
        {
            var mem = RewriteOperand(instrCur.Operand);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            m.Assign(tmp, m.Shr(mem, 1));
            m.Assign(mem, tmp);
            m.Assign(c, m.Cond(tmp));
        }

        private void Bit()
        {
            var a = binder.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operand);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var flags = FlagGroupStorage(FlagM.NF | FlagM.VF | FlagM.CF);
            m.Assign(tmp, m.And(a, mem));
            m.Assign(flags, m.Cond(tmp));
        }

        private void Brk()
        {
            m.SideEffect(host.PseudoProcedure("__brk", VoidType.Instance));
        }

        private void Cmp(RegisterStorage r)
        {
            var a = binder.EnsureRegister(r);
            var mem = RewriteOperand(instrCur.Operand);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF | FlagM.CF);
            m.Assign(c, m.Cond(m.ISub(a, mem)));
        }

        private void Dec()
        {
            var mem = RewriteOperand(instrCur.Operand);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF|FlagM.ZF);
            m.Assign(tmp, m.ISub(mem, 1));
            m.Assign(RewriteOperand(instrCur.Operand), tmp);
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
            var mem = RewriteOperand(instrCur.Operand);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(tmp, m.IAdd(mem, 1));
            m.Assign(RewriteOperand(instrCur.Operand), tmp);
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
            var mem = (MemoryAccess)RewriteOperand(instrCur.Operand);
            m.Goto(mem.EffectiveAddress);
        }

        private void Jsr()
        {
            var mem  = (MemoryAccess) RewriteOperand(instrCur.Operand);
            m.Call(mem.EffectiveAddress, 2);
        }

        private void Ld(RegisterStorage reg)
        {
            var r = binder.EnsureRegister(reg);
            var mem = RewriteOperand(instrCur.Operand);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(r, mem);
            m.Assign(c, m.Cond(r));
        }

        private void And()
        {
            var a = binder.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operand);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(
                a,
                m.And(a, mem));
            m.Assign(c, m.Cond(a));
        }

        private void Eor()
        {
            var a = binder.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operand);
            var c = FlagGroupStorage(FlagM.NF | FlagM.ZF);
            m.Assign(
                a,
                m.Xor(a, mem));
            m.Assign(c, m.Cond(a));
        }

        private void Ora()
        {
            var a = binder.EnsureRegister(Registers.a);
            var mem = RewriteOperand(instrCur.Operand);
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
            var arg = RewriteOperand(instrCur.Operand);
            m.Assign(arg, host.PseudoProcedure(rot, arg.DataType, arg, Constant.Byte(1)));
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
            var mem = RewriteOperand(instrCur.Operand);
            var a = binder.EnsureRegister(Registers.a);
            var c = binder.EnsureFlagGroup(Registers.p, (uint) FlagM.CF, "C", PrimitiveType.Bool);
            m.Assign(
                a,
                m.IAdd(
                    m.IAdd(a, mem),
                    c));
            m.Assign(
                binder.EnsureFlagGroup(Registers.p, (uint) Instruction.DefCc(instrCur.Code), "NVZC", PrimitiveType.Byte),
                m.Cond(a));
        }

        private void Sbc()
        {
            var mem = RewriteOperand(instrCur.Operand);
            var a = binder.EnsureRegister(Registers.a);
            var c = binder.EnsureFlagGroup(Registers.p, (uint) FlagM.CF, "C", PrimitiveType.Bool);
            m.Assign(
                a,
                m.ISub(
                    m.ISub(a, mem),
                    m.Not(c)));
            m.Assign(
                binder.EnsureFlagGroup(Registers.p, (uint) Instruction.DefCc(instrCur.Code), "NVZC", PrimitiveType.Byte),
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
            var mem = RewriteOperand(instrCur.Operand);
            var id = binder.EnsureRegister(reg);
            m.Assign(mem, id);
        }

        private Expression RewriteOperand(Operand op)
        {
            Constant offset;
            switch (op.Mode)
            {
            default: throw new NotImplementedException("Unimplemented address mode " + op.Mode);
            case AddressMode.Accumulator:
                return binder.EnsureRegister(Registers.a);
            case AddressMode.Immediate:
                return op.Offset;
            case AddressMode.IndirectIndexed:
                var y = binder.EnsureRegister(Registers.y);
                offset = Constant.Word16((ushort) op.Offset.ToByte());
                return m.Mem8(
                    m.IAdd(
                        m.Mem(PrimitiveType.Ptr16, offset),
                        m.Cast(PrimitiveType.UInt16, y)));
            case AddressMode.IndexedIndirect:
                var x = binder.EnsureRegister(Registers.x);
                offset = Constant.Word16(op.Offset.ToByte());
                return m.Mem8(
                    m.Mem(
                        PrimitiveType.Ptr16,
                        m.IAdd(
                            offset,
                            m.Cast(PrimitiveType.UInt16, x))));
            case AddressMode.Absolute:
                return m.Mem8(arch.MakeAddressFromConstant(op.Offset));
            case AddressMode.AbsoluteX:
            case AddressMode.AbsoluteY:
                return m.Mem8(m.IAdd(
                    arch.MakeAddressFromConstant(op.Offset),
                    binder.EnsureRegister(op.Register)));
            case AddressMode.ZeroPage:
                if (op.Register != null)
                {
                    return m.Mem8(
                        m.IAdd(
                            arch.MakeAddressFromConstant(op.Offset),
                            binder.EnsureRegister(op.Register)));
                }
                else
                {
                    return m.Mem8(arch.MakeAddressFromConstant(op.Offset));
                }
            case AddressMode.Indirect:
                return m.Mem16(m.Mem16(arch.MakeAddressFromConstant(op.Offset)));
            }
        }

        private static HashSet<Opcode> seen = new HashSet<Opcode>();

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.Code))
                return;
            seen.Add(dasm.Current.Code);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void Rw6502_" + dasm.Current.Code + "()");
            Debug.WriteLine("        {");
            Debug.Write("            BuildTest(");
            Debug.Write(string.Join(
                ", ",
                bytes.Select(b => string.Format("0x{0:X2}", (int) b))));
            Debug.WriteLine(");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|{0}({1}): 1 instructions\",", dasm.Current.Address, dasm.Current.Length);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }

    }
}
