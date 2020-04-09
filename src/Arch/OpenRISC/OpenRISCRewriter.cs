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
using System.Text;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.OpenRISC
{
    public class OpenRISCRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly OpenRISCArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<OpenRISCInstruction> dasm;
        private OpenRISCInstruction instrCur;
        private InstrClass iclass;
        private RtlEmitter m;

        public OpenRISCRewriter(OpenRISCArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new OpenRISCDisassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instrCur = dasm.Current;
                this.iclass = instrCur.InstructionClass;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                switch (instrCur.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    m.Invalid(); iclass = InstrClass.Invalid;
                    break;
                case Mnemonic.l_add: RewriteAluCV(m.IAdd); break;
                case Mnemonic.l_addc: RewriteAddc(); break;
                case Mnemonic.l_addi: RewriteAddi(); break;
                case Mnemonic.l_addic: RewriteAddic(); break;
                case Mnemonic.l_adrp: RewriteAdrp(); break;
                case Mnemonic.l_and: RewriteBinOp(m.And); break;
                case Mnemonic.l_andi: RewriteBinOpImm(m.And); break;
                case Mnemonic.l_bf: RewriteBranch(false); break;
                case Mnemonic.l_bnf: RewriteBranch(true); break;
                case Mnemonic.l_cmov: RewriteCmov(); break;
                case Mnemonic.l_csync: RewriteCsync(); break;
                case Mnemonic.l_j: RewriteJ(); break;
                case Mnemonic.l_jal: RewriteJal(); break;
                case Mnemonic.l_jalr: RewriteJalr(); break;
                case Mnemonic.l_jr: RewriteJr(); break;
                case Mnemonic.l_lbs: RewriteLoad(arch.SignedWordWidth); break;
                case Mnemonic.l_lbz: RewriteLoad(arch.WordWidth); break;
                case Mnemonic.l_lf: RewriteLoad(arch.WordWidth); break;
                case Mnemonic.l_lhs: RewriteLoad(arch.SignedWordWidth); break;
                case Mnemonic.l_lhz: RewriteLoad(arch.WordWidth); break;
                case Mnemonic.l_lwa: RewriteLwa(); break;
                case Mnemonic.l_lws: RewriteLoad(arch.SignedWordWidth); break;
                case Mnemonic.l_lwz: RewriteLoad(arch.WordWidth); break;
                case Mnemonic.l_ld: RewriteLoad(arch.WordWidth); break;
                case Mnemonic.l_maci: RewriteMaci(); break;
                case Mnemonic.l_macrc: RewriteMacrc(); break;
                case Mnemonic.l_movhi: RewriteMovhi(); break;
                case Mnemonic.l_mfspr: RewriteMfspr(); break;
                case Mnemonic.l_msync: RewriteMsync(); break;
                case Mnemonic.l_mtspr: RewriteMtspr(); break;
                case Mnemonic.l_mul: RewriteAluV(m.SMul); break;

                case Mnemonic.l_nop: m.Nop(); break;
                case Mnemonic.l_or: RewriteBinOp(m.Or); break;
                case Mnemonic.l_ori: RewriteBinOpImm(m.Or); break;
                case Mnemonic.l_psync: RewritePsync(); break;
                case Mnemonic.l_rfe: RewriteRfe(); break;
                case Mnemonic.l_sb: RewriteStore(); break;
                case Mnemonic.l_sh: RewriteStore(); break;
                case Mnemonic.l_sw: RewriteStore(); break;

                case Mnemonic.l_sfeq:   RewriteSf(m.Eq); break;
                case Mnemonic.l_sfeqi:  RewriteSfi(m.Eq); break;
                case Mnemonic.l_sfges:  RewriteSf(m.Ge); break;
                case Mnemonic.l_sfgesi: RewriteSfi(m.Ge); break;
                case Mnemonic.l_sfgeu:  RewriteSf(m.Uge); break;
                case Mnemonic.l_sfgtsi: RewriteSfi(m.Gt); break;
                case Mnemonic.l_sfgtu:  RewriteSf(m.Ugt); break;
                case Mnemonic.l_sfgtui: RewriteSfi(m.Ugt); break;
                case Mnemonic.l_sfles:  RewriteSf(m.Le); break;
                case Mnemonic.l_sflesi: RewriteSfi(m.Le); break;
                case Mnemonic.l_sfleu:  RewriteSf(m.Ule); break;
                case Mnemonic.l_sfleui: RewriteSfi(m.Ule); break;
                case Mnemonic.l_sflts:  RewriteSf(m.Lt); break;
                case Mnemonic.l_sfltsi: RewriteSfi(m.Lt); break;
                case Mnemonic.l_sfltu:  RewriteSf(m.Ult); break;
                case Mnemonic.l_sfltui: RewriteSfi(m.Ult); break;
                case Mnemonic.l_sfne:   RewriteSf(m.Ne); break;
                case Mnemonic.l_sfnei:  RewriteSfi(m.Ne); break;

                case Mnemonic.l_sll: RewriteBinOp(m.Shl); break;
                case Mnemonic.l_slli: RewriteBinOpImm(m.Shl); break;
                case Mnemonic.l_sra: RewriteBinOp(m.Sar); break;
                case Mnemonic.l_srai: RewriteBinOpImm(m.Sar); break;
                case Mnemonic.l_srl: RewriteBinOp(m.Shr); break;
                case Mnemonic.l_srli: RewriteBinOpImm(m.Shr); break;
                case Mnemonic.l_sub: RewriteAluCV(m.ISub); break;
                case Mnemonic.l_sys: RewriteSys(); break;
                case Mnemonic.l_xor: RewriteBinOp(m.Xor); break;
                case Mnemonic.l_xori: RewriteBinOpImm(m.Xor); break;
                }
                yield return m.MakeCluster(instrCur.Address, instrCur.Length, iclass);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for this rewriter.
        /// </summary>
        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);
            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadUInt32();
            Console.WriteLine("        [Test]");
            Console.WriteLine("        public void OpenRiscRw_" + dasm.Current.Mnemonic + "()");
            Console.WriteLine("        {");
            Console.WriteLine("            Given_HexString(\"{0:X8}\");\t// {1}", bytes, dasm.Current.ToString());
            Console.WriteLine("            AssertCode(");
            Console.WriteLine("                \"0|L--|00100000({0}): 1 instructions\",", dasm.Current.Length);
            Console.WriteLine("                \"1|L--|@@@\");");
            Console.WriteLine("        }");
            Console.WriteLine("");
        }

        private Address Addr(MachineOperand op)
        {
            return ((AddressOperand) op).Address;
        }

        private Constant Imm(MachineOperand op)
        {
            return ((ImmediateOperand) op).Value;
        }

        private MemoryAccess Mem(MachineOperand op)
        {
            var mop = (MemoryOperand) op;
            Expression ea = binder.EnsureRegister(mop.Base);
            ea = m.AddSubSignedInt(ea, mop.Offset);
            return m.Mem(op.Width, ea);
        }

        private Identifier Reg(MachineOperand op)
        {
            return binder.EnsureRegister(((RegisterOperand) op).Register);
        }

        private Expression Reg0(MachineOperand op)
        {
            var reg = ((RegisterOperand) op).Register;
            if (reg == Registers.GpRegs[0])
            {
                return Constant.Zero(arch.WordWidth);
            }
            else
            {
                return binder.EnsureRegister(reg);
            }
        }

        private Expression Spr(MachineOperand op)
        {
            if (op is RegisterOperand reg)
            {
                return binder.EnsureRegister(reg.Register);
            }
            return ((ImmediateOperand) op).Value;
        }


        private void CV(Identifier dst)
        {
            var cv = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint) (FlagM.CY | FlagM.OV)));
            m.Assign(cv, m.Cond(dst));
        }

        private void V(Identifier dst)
        {
            var v = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint) FlagM.OV));
            m.Assign(v, m.Cond(dst));
        }

        private void RewriteAluCV(Func<Expression, Expression, Expression> fn)
        {
            var dst = Reg(instrCur.Operands[0]);
            var src1 = Reg0(instrCur.Operands[1]);
            var src2 = Reg0(instrCur.Operands[2]);
            m.Assign(dst, fn(src1, src2));
            CV(dst);
        }

        private void RewriteAluV(Func<Expression, Expression, Expression> fn)
        {
            var dst = Reg(instrCur.Operands[0]);
            var src1 = Reg0(instrCur.Operands[1]);
            var src2 = Reg0(instrCur.Operands[2]);
            m.Assign(dst, fn(src1, src2));
            V(dst);
        }

        private void RewriteAddc()
        {
            var dst = Reg(instrCur.Operands[0]);
            var src1 = Reg0(instrCur.Operands[1]);
            var src2 = Reg0(instrCur.Operands[2]);
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint) FlagM.CY));
            m.Assign(dst, m.IAdd(m.IAdd(src1, src2), c));
            CV(dst);
        }

        private void RewriteAddi()
        {
            var dst = Reg(instrCur.Operands[0]);
            var src = Reg0(instrCur.Operands[1]);
            var imm = Imm(instrCur.Operands[2]);
            if (imm.IsNegative)
            {
                m.Assign(dst, m.ISub(src, imm.Negate()));
            }
            else
            {
                m.Assign(dst, m.IAdd(src, imm.Negate()));
            }
            CV(dst);
        }

        private void RewriteAddic()
        {
            var dst = Reg(instrCur.Operands[0]);
            var src = Reg0(instrCur.Operands[1]);
            var imm = Imm(instrCur.Operands[2]);
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint) FlagM.CY));
            m.Assign(dst, m.IAdd(m.IAdd(src, imm), c));
            CV(dst);
        }

        private void RewriteAdrp()
        {
            var dst = Reg(instrCur.Operands[0]);
            var src = Addr(instrCur.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteBinOp(Func<Expression, Expression, Expression> fn)
        {
            var dst = Reg(instrCur.Operands[0]);
            var src1 = Reg0(instrCur.Operands[1]);
            var src2 = Reg0(instrCur.Operands[2]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteBinOpImm(Func<Expression, Expression, Expression> fn)
        {
            var dst = Reg(instrCur.Operands[0]);
            var src1 = Reg0(instrCur.Operands[1]);
            var src2 = Imm(instrCur.Operands[2]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteBranch(bool v)
        {
            Expression f = binder.EnsureFlagGroup(Registers.F);
            if (v)
            {
                f = m.Not(f);
            }
            var target = (AddressOperand) instrCur.Operands[0];
            m.Branch(f, target.Address, instrCur.InstructionClass);
        }

        private void RewriteCmov()
        {
            var dst = Reg(instrCur.Operands[0]);
            var a = Reg0(instrCur.Operands[1]);
            var b = Reg0(instrCur.Operands[2]);
            var f = binder.EnsureFlagGroup(Registers.F);
            m.Assign(dst, m.Conditional(dst.DataType, f, a, b));
        }

        private void RewriteCsync()
        {
            m.SideEffect(host.PseudoProcedure("__csync", VoidType.Instance));
        }

        private void RewriteJ()
        {
            m.GotoD(Addr(instrCur.Operands[0]));
        }

        private void RewriteJal()
        {
            m.CallD(Addr(instrCur.Operands[0]), 0);
        }

        private void RewriteJalr()
        {
            m.CallD(Reg(instrCur.Operands[0]), 0);
        }

        private void RewriteJr()
        {
            var reg = (RegisterOperand) instrCur.Operands[0];
            //$TODO: Reko should have a 'paranoid mode' where we don't 
            // trust idioms like this.
            if (reg.Register == Registers.GpRegs[9])
                m.ReturnD(0, 0);
            else
                m.GotoD(binder.EnsureRegister(reg.Register));
        }

        private void RewriteLoad(PrimitiveType dtDst)
        {
            var dst = Reg(instrCur.Operands[0]);
            Expression mem = Mem(instrCur.Operands[1]);
            if (mem.DataType.BitSize < dtDst.BitSize)
            {
                mem = m.Cast(dtDst, mem);
            }
            m.Assign(dst, mem);
        }

        private void RewriteLwa()
        {
            var dtDst = arch.WordWidth;
            var dst = Reg(instrCur.Operands[0]);
            var mem = Mem(instrCur.Operands[1]);
            var ea = mem.EffectiveAddress;
            var fnType = new FunctionType(
                new Identifier("", PrimitiveType.Word32, null),
                new Identifier("ea", new Pointer(PrimitiveType.Word32, 32), null));
            var e = host.CallIntrinsic("__atomic_load_w32", fnType, ea);
            if (mem.DataType.BitSize < dtDst.BitSize)
            {
                e = m.Cast(dtDst, e);
            }
            m.Assign(dst, e);
        }

        private void RewriteMfspr()
        {
            var dst = Reg(instrCur.Operands[0]);
            var spr = Spr(instrCur.Operands[2]);
            m.Assign(dst, host.PseudoProcedure("__mfspr", PrimitiveType.Word32, spr));
        }

        private void RewriteMaci()
        {
            var src1 = Reg(instrCur.Operands[0]);
            var src2 = Imm(instrCur.Operands[1]);
            var mul = m.IMul(src1, src2);
            mul.DataType = PrimitiveType.Word64;
            var product = binder.CreateTemporary(mul.DataType);
            m.Assign(product, mul);
            var hi_lo = binder.EnsureSequence(mul.DataType, Registers.machi, Registers.maclo);
            m.Assign(hi_lo, m.IAdd(hi_lo, product));
        }

        private void RewriteMacrc()
        {
            var dst = Reg(instrCur.Operands[0]);
            var hi_lo = binder.EnsureSequence(PrimitiveType.Word64, Registers.machi, Registers.maclo);
            if (arch.WordWidth.BitSize == Registers.machi.DataType.BitSize)
            {
                m.Assign(dst, binder.EnsureRegister(Registers.maclo));
            }
            else
            {
                m.Assign(dst, hi_lo);
            }
            m.Assign(hi_lo, 0);
        }

        private void RewriteMovhi()
        {
            var dst = Reg(instrCur.Operands[0]);
            var imm = Imm(instrCur.Operands[1]);
            m.Assign(dst, Constant.Word32(imm.ToUInt32() << 16));
        }

        private void RewriteMsync()
        {
            m.SideEffect(host.PseudoProcedure("__msync", VoidType.Instance));
        }

        private void RewriteMtspr()
        {
            var src = Reg(instrCur.Operands[1]);
            var spr = Spr(instrCur.Operands[2]);
            m.SideEffect(host.PseudoProcedure("__mtspr", VoidType.Instance, spr, src));
        }

        private void RewritePsync()
        {
            m.SideEffect(host.PseudoProcedure("__psync", VoidType.Instance));
        }

        private void RewriteRfe()
        {
            m.ReturnD(0, 0);
        }

        private void RewriteSf(Func<Expression, Expression, Expression> fn)
        {
            var a = Reg0(instrCur.Operands[0]);
            var b = Reg0(instrCur.Operands[1]);
            var f = binder.EnsureFlagGroup(Registers.F);
            m.Assign(f, fn(a, b));
        }

        private void RewriteSfi(Func<Expression, Expression, Expression> fn)
        {
            var a = Reg0(instrCur.Operands[0]);
            var b = Imm(instrCur.Operands[1]);
            var f = binder.EnsureFlagGroup(Registers.F);
            m.Assign(f, fn(a, b));
        }

        private void RewriteStore()
        {
            Expression mem = Mem(instrCur.Operands[0]);
            Expression src = Reg(instrCur.Operands[1]);
            if (mem.DataType.BitSize < src.DataType.BitSize)
            {
                src = m.Slice(mem.DataType, src, 0);
            }
            m.Assign(mem, src);
        }

        private void RewriteSys()
        {
            var vector = Imm(instrCur.Operands[0]);
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, vector));
        }
    }
}
