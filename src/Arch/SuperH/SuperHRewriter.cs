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

using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Reko.Core;
using System.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.SuperH
{
    public class SuperHRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly SuperHArchitecture arch;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly SuperHState state;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<SuperHInstruction> dasm;
        private SuperHInstruction instr;
        private RtlEmitter m;
        private InstrClass rtlc;

        public SuperHRewriter(SuperHArchitecture arch, EndianImageReader rdr, SuperHState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new SuperHDisassembler(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.rtlc = InstrClass.Linear;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                switch (instr.Mnemonic)
                {
                default:
                    host.Error(
                        dasm.Current.Address,
                        string.Format(
                            "SuperH instruction {0} not supported yet.",
                        dasm.Current.Mnemonic));
                    EmitUnitTest();
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    Invalid();
                    break;
                case Mnemonic.add: RewriteBinOp(m.IAdd, n => (sbyte)n); break;
                case Mnemonic.addc: RewriteAddcSubc(m.IAdd); break;
                case Mnemonic.addv: RewriteAddv(m.IAdd); break;
                case Mnemonic.and: RewriteBinOp(m.And, n => (byte)n); break;
                case Mnemonic.and_b: RewriteBinOp(m.And, n => (byte)n); break;
                case Mnemonic.bf: RewriteBranch(false, false); break;
                case Mnemonic.bf_s: RewriteBranch(false, true); break;
                case Mnemonic.bra: RewriteGoto(); break;
                case Mnemonic.braf: RewriteBraf(); break;
                case Mnemonic.brk: RewriteBrk(); break;
                case Mnemonic.bsr: RewriteBsr(); break;
                case Mnemonic.bsrf: RewriteBsrf(); break;
                case Mnemonic.bt: RewriteBranch(true, false); break;
                case Mnemonic.bt_s: RewriteBranch(true, true); break;
                case Mnemonic.clrmac: RewriteClr(Registers.mac); break;
                case Mnemonic.clrs: RewriteClrtSet(Registers.S, Constant.False()); break;
                case Mnemonic.clrt: RewriteClrtSet(Registers.T, Constant.False()); break;
                case Mnemonic.cmp_eq: RewriteCmp(m.Eq); break;
                case Mnemonic.cmp_ge: RewriteCmp(m.Ge); break;
                case Mnemonic.cmp_gt: RewriteCmp(m.Gt); break;
                case Mnemonic.cmp_hs: RewriteCmp(m.Uge); break;
                case Mnemonic.cmp_hi: RewriteCmp(m.Ugt); break;
                case Mnemonic.cmp_pl: RewriteCmp0(m.Gt0); break;
                case Mnemonic.cmp_pz: RewriteCmp0(m.Ge0); break;
                case Mnemonic.cmp_str: RewriteCmpStr(); break;
                case Mnemonic.div0s: RewriteDiv0s(); break;
                case Mnemonic.div0u: RewriteDiv0u(); break;
                case Mnemonic.div1: RewriteDiv1(); break;
                case Mnemonic.dmuls_l: RewriteDmul(m.SMul); break;
                case Mnemonic.dmulu_l: RewriteDmul(m.UMul); break;
                case Mnemonic.dt: RewriteDt(); break;
                case Mnemonic.exts_b: RewriteExt(PrimitiveType.SByte); break;
                case Mnemonic.exts_w: RewriteExt(PrimitiveType.Int16); break;
                case Mnemonic.extu_b: RewriteExt(PrimitiveType.Byte); break;
                case Mnemonic.extu_w: RewriteExt(PrimitiveType.UInt16); break;
                case Mnemonic.fabs: RewriteFabs(); break;
                case Mnemonic.fadd: RewriteBinOp(m.FAdd, null); break;
                case Mnemonic.fcmp_eq: RewriteCmp(m.FEq); break;
                case Mnemonic.fcmp_gt: RewriteCmp(m.FGt); break;
                case Mnemonic.fcnvds: RewriteUnary(d => m.Cast(PrimitiveType.Real32, d)); break;
                case Mnemonic.fcnvsd: RewriteUnary(d => m.Cast(PrimitiveType.Real64, d)); break;
                case Mnemonic.fdiv: RewriteBinOp(m.FDiv, null); break;
                case Mnemonic.fldi0: RewriteFldi(0.0F); break;
                case Mnemonic.fldi1: RewriteFldi(1.0F); break;
                case Mnemonic.flds: RewriteMov(); break;
                case Mnemonic.fmac: RewriteFmac(); break;
                case Mnemonic.fmov_d: RewriteMov(); break;
                case Mnemonic.fmov_s: RewriteMov(); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jsr: RewriteJsr(); break;
                case Mnemonic.lds: RewriteMov(); break;
                case Mnemonic.lds_l: RewriteMov(); break;
                case Mnemonic.mac_l: RewriteMac(PrimitiveType.Int64); break;
                case Mnemonic.mac_w: RewriteMac(PrimitiveType.Int32); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.mova: RewriteMova(); break;
                case Mnemonic.mov_b: RewriteMov(); break;
                case Mnemonic.mov_w: RewriteMov(); break;
                case Mnemonic.mov_l: RewriteMov(); break;
                case Mnemonic.movt: RewriteMovt(); break;
                case Mnemonic.mul_l: RewriteMul_l(); break;
                case Mnemonic.muls_w: RewriteMul_w(PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.mulu_w: RewriteMul_w(PrimitiveType.UInt16, m.UMul); break;
                case Mnemonic.neg: RewriteUnary(m.Neg); break;
                case Mnemonic.negc: RewriteNegc(); break;
                case Mnemonic.not: RewriteUnary(m.Comp); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.ocbi: RewriteOcbi(); break;
                case Mnemonic.or: RewriteBinOp(m.Or, u => (byte)u); break;
                case Mnemonic.rotcl: RewriteRotc(PseudoProcedure.RolC); break;
                case Mnemonic.rotcr: RewriteRotc(PseudoProcedure.RorC); break;
                case Mnemonic.rotl: RewriteRot(PseudoProcedure.Rol); break;
                case Mnemonic.rotr: RewriteRot(PseudoProcedure.Ror); break;
                case Mnemonic.rts: RewriteRts(); break;
                case Mnemonic.sett: RewriteClrtSet(Registers.T, Constant.True()); break;
                case Mnemonic.shad: RewriteShd(m.Shl, m.Sar); break;
                case Mnemonic.shar: RewriteShift(m.Sar, 1); break;
                case Mnemonic.shld: RewriteShd(m.Shl, m.Shr); break;
                case Mnemonic.shll: RewriteShift(m.Shl, 1); break;
                case Mnemonic.shll2: RewriteShift(m.Shl, 2); break;
                case Mnemonic.shll8: RewriteShift(m.Shl, 8); break;
                case Mnemonic.shll16: RewriteShift(m.Shl, 16); break;
                case Mnemonic.shlr: RewriteShift(m.Shr, 1); break;
                case Mnemonic.shlr2: RewriteShift(m.Shr, 2); break;
                case Mnemonic.shlr8: RewriteShift(m.Shr, 8); break;
                case Mnemonic.shlr16: RewriteShift(m.Shr, 16); break;
                case Mnemonic.stc: RewriteMov(); break;
                case Mnemonic.sts: RewriteMov(); break;
                case Mnemonic.sts_l: RewriteMov(); break;
                case Mnemonic.sub: RewriteBinOp(m.ISub, null); break;
                case Mnemonic.subc: RewriteAddcSubc(m.ISub); break;
                case Mnemonic.swap_w: RewriteSwapW(); break;
                case Mnemonic.tst: RewriteTst(); break;
                case Mnemonic.xor: RewriteBinOp(m.Xor, n => (byte)n); break;
                case Mnemonic.xtrct: RewriteXtrct(); break;
                }
                var rtlc = new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = this.rtlc,
                };
                yield return rtlc;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Invalid()
        {
            this.rtlc = InstrClass.Invalid;
            m.Invalid();
        }

        private static HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void SHRw_" + dasm.Current.Mnemonic + "()");
            Debug.WriteLine("        {");
            Debug.Write("            RewriteCode(\"");
            Debug.Write(string.Join(
                "",
                bytes.Select(b => string.Format("{0:X2}", (int)b))));
            Debug.WriteLine("\");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|00100000(2): 1 instructions\",");
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }

        private Expression SrcOp(MachineOperand op, Func<int, int> immediateFn=null)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                var id = binder.EnsureRegister(regOp.Register);
                return id;
            }
            var immOp = op as ImmediateOperand;
            if (immOp != null)
            {
                return Constant.Word32(immediateFn(immOp.Value.ToInt32()));
            }
            var addrOp = op as AddressOperand;
            if (addrOp != null)
            {
                return addrOp.Address;
            }
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                Identifier reg;
                switch (mem.mode)
                {
                default:
                    throw new NotImplementedException(mem.mode.ToString());
                case AddressingMode.Indirect:
                    return m.Mem(mem.Width, binder.EnsureRegister(mem.reg));
                case AddressingMode.IndirectPreDecr:
                    reg = binder.EnsureRegister(mem.reg);
                    m.Assign(reg, m.ISubS(reg, mem.Width.Size));
                    return m.Mem(mem.Width, reg);
                case AddressingMode.IndirectPostIncr:
                    var t = binder.CreateTemporary(mem.Width);
                    reg = binder.EnsureRegister(mem.reg);
                    m.Assign(t, m.Mem(mem.Width, reg));
                    m.Assign(reg, m.IAddS(reg, t.DataType.Size));
                    return t;
                case AddressingMode.IndirectDisplacement:
                    reg = binder.EnsureRegister(mem.reg);
                    return m.Mem(
                        mem.Width,
                        m.IAddS(reg, mem.disp));
                case AddressingMode.IndexedIndirect:
                    return m.Mem(mem.Width, m.IAdd(
                        binder.EnsureRegister(Registers.r0),
                        binder.EnsureRegister(mem.reg)));
                case AddressingMode.PcRelativeDisplacement:
                    var addr = instr.Address.ToUInt32();
                    if (mem.Width.Size == 4)
                    {
                        addr &= ~3u;
                    }
                    addr += (uint)(mem.disp + 4);
                    return m.Mem(mem.Width, Address.Ptr32(addr));
                }
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        private Expression DstOp(MachineOperand op, Expression src, Func<Expression, Expression, Expression> fn)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                var id = binder.EnsureRegister(regOp.Register);
                m.Assign(id, fn(id, src));
                return id;
            }
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                Identifier r0;
                Identifier gbr;
                var tmp = binder.CreateTemporary(op.Width);
                switch (mem.mode)
                {
                case AddressingMode.GbrIndexedIndirect:
                    r0 = binder.EnsureRegister(Registers.r0);
                    gbr = binder.EnsureRegister(Registers.gbr);
                    m.Assign(tmp, m.Mem(tmp.DataType, m.IAdd(r0, gbr)));
                    m.Assign(
                        m.Mem(tmp.DataType, m.IAdd(r0, gbr)),
                        fn(tmp, src));
                    return tmp;
                default:throw new NotImplementedException();
                }
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        private Expression DstOp(MachineOperand op, Expression src, Func<Expression, Expression> fn)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                var id = binder.EnsureRegister(regOp.Register);
                m.Assign(id, fn(src));
                return id;
            }
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                Identifier r0;
                Identifier gbr;
                Identifier reg;
                var tmp = binder.CreateTemporary(op.Width);
                switch (mem.mode)
                {
                case AddressingMode.Indirect:
                    reg = binder.EnsureRegister(mem.reg);
                    m.Assign(
                        m.Mem(mem.Width, reg),
                        fn(src));
                    return null;
                case AddressingMode.IndirectDisplacement:
                    reg = binder.EnsureRegister(mem.reg);
                    m.Assign(
                        m.Mem(mem.Width, m.IAddS(reg, mem.disp)),
                        fn(src));
                    return null;
                case AddressingMode.IndirectPreDecr:
                    reg = binder.EnsureRegister(mem.reg);
                    m.Assign(reg, m.ISubS(reg, mem.Width.Size));
                    m.Assign(
                        m.Mem(tmp.DataType, reg),
                        fn(src));
                    return null;
                case AddressingMode.IndexedIndirect:
                    m.Assign(
                        m.Mem(mem.Width, m.IAdd(
                            binder.EnsureRegister(Registers.r0),
                            binder.EnsureRegister(mem.reg))),
                        fn(src));
                    return null;
                case AddressingMode.GbrIndexedIndirect:
                    r0 = binder.EnsureRegister(Registers.r0);
                    gbr = binder.EnsureRegister(Registers.gbr);
                    m.Assign(tmp, m.Mem(tmp.DataType, m.IAdd(r0, gbr)));
                    m.Assign(
                        m.Mem(tmp.DataType, m.IAdd(r0, gbr)),
                        fn(src));
                    return tmp;
                default: throw new NotImplementedException(mem.mode.ToString());
                }
            }
            throw new NotImplementedException(op.GetType().Name);
        }


        private void RewriteAddcSubc(Func<Expression,Expression,Expression> fn)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var src = SrcOp(instr.Operands[0], null);
            var dst = DstOp(instr.Operands[1], src, (a, b) =>
                fn(fn(a, b), t));
        }

        private void RewriteNegc()
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var src = SrcOp(instr.Operands[0], null);
            var dst = DstOp(instr.Operands[1], src, (d, s) =>
                m.ISub(m.Neg(s), t));
        }

        private void RewriteAddv(Func<Expression, Expression, Expression> fn)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var src = SrcOp(instr.Operands[0], null);
            var dst = DstOp(instr.Operands[1], src, fn);
            m.Assign(t, m.Test(ConditionCode.OV, dst));
        }

        private void RewriteBinOp(
            Func<Expression, Expression, Expression> fn,
            Func<int, int> immediateFn)
        {
            var src = SrcOp(instr.Operands[0], immediateFn);
            var dst = DstOp(instr.Operands[1], src, fn);
        }

        private void RewriteBranch(bool takenOnTset, bool delaySlot)
        {
            this.rtlc = delaySlot
                ? InstrClass.ConditionalTransfer | InstrClass.Delay
                : InstrClass.ConditionalTransfer;
            Expression cond = binder.EnsureFlagGroup(Registers.T);
            var addr = ((AddressOperand)instr.Operands[0]).Address;
            if (!takenOnTset)
                cond = m.Not(cond);
            m.Branch(cond, addr, this.rtlc);
        }

        private void RewriteBraf()
        {
            this.rtlc = InstrClass.Delay | InstrClass.Transfer;
            var reg = binder.EnsureRegister(((RegisterOperand)instr.Operands[0]).Register);
            m.GotoD(m.IAdd(instr.Address + 4, reg));
        }

        private void RewriteBrk()
        {
            m.SideEffect(host.PseudoProcedure("__brk", VoidType.Instance));
        }

        private void RewriteBsr()
        {
            this.rtlc = InstrClass.Transfer | InstrClass.Call | InstrClass.Delay;
            var dst = SrcOp(instr.Operands[0], null);
            m.CallD(dst, 0);
        }

        private void RewriteBsrf()
        {
            this.rtlc = InstrClass.Transfer | InstrClass.Delay;
            var src = SrcOp(instr.Operands[0], null);
            var reg = binder.EnsureRegister(((RegisterOperand)instr.Operands[0]).Register);
            m.CallD(m.IAdd(instr.Address + 4, src), 0);
        }

        private void RewriteGoto()
        {
            this.rtlc = InstrClass.Transfer | InstrClass.Delay;
            var addr = ((AddressOperand)instr.Operands[0]).Address;
            m.GotoD(addr);
        }

        private void RewriteCmp(Func<Expression,Expression,Expression> fn)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var op1 = SrcOp(instr.Operands[0], n => (sbyte)n);
            var op2 = SrcOp(instr.Operands[1], null);
            m.Assign(t, fn(op2, op1));
        }


        private void RewriteClr(RegisterStorage reg)
        {
            rtlc = InstrClass.Linear;
            var dst = binder.EnsureRegister(reg);
            var z = Constant.Zero(dst.DataType);
            m.Assign(dst, z);
        }

        private void RewriteClrtSet(FlagGroupStorage f, Expression e)
        {
            var t = binder.EnsureFlagGroup(f);
            m.Assign(t, e);
        }


        private void RewriteCmp0(Func<Expression, Expression> fn)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var op1 = SrcOp(instr.Operands[0], n => (sbyte)n);
            m.Assign(t, fn(op1));
        }

        private void RewriteCmpStr()
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var left = SrcOp(instr.Operands[1]);
            var right = SrcOp(instr.Operands[0]);
            m.Assign(t, host.PseudoProcedure("__cmp_str", PrimitiveType.Bool, left, right));
        }


        private void RewriteDiv0s()
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = SrcOp(instr.Operands[1]);
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(t, host.PseudoProcedure("__div0s", PrimitiveType.Bool, dst, src));
        }

        private void RewriteDiv0u()
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(t, host.PseudoProcedure("__div0u", PrimitiveType.Bool));
        }


        private void RewriteDiv1()
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = SrcOp(instr.Operands[1]);
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(dst, host.PseudoProcedure("__div1", PrimitiveType.Bool, dst, src));
        }

        private void RewriteDmul(Func<Expression, Expression, Expression> fn)
        {
            var op1 = SrcOp(instr.Operands[0]);
            var op2 = SrcOp(instr.Operands[1]);
            var mac = binder.EnsureRegister(Registers.mac);
            m.Assign(mac, fn(op2, op1));
        }

        private void RewriteDt()
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var r = DstOp(instr.Operands[0], Constant.Word32(1), m.ISub);
            m.Assign(t, m.Eq0(r));
        }

        private void RewriteExt(PrimitiveType width)
        {
            var src = SrcOp(instr.Operands[0], null);
            var dst = DstOp(instr.Operands[1], src, (a, b) => m.Cast(width, b));
        }

        private void RewriteFabs()
        {
            var src = SrcOp(instr.Operands[0], null);
            var dst = DstOp(instr.Operands[0], src, (a, b) => host.PseudoProcedure("fabs", src.DataType, b));
        }

        private void RewriteFldi(float f)
        {
            DstOp(instr.Operands[0], Constant.Real32(f), a => a);
        }

        private void RewriteFmac()
        {
            var f0 = binder.EnsureRegister(Registers.fr0);
            var mul = SrcOp(instr.Operands[1]);
            var dst = SrcOp(instr.Operands[2]);
            m.Assign(dst, m.FAdd(m.FMul(f0, mul), dst));
        }

        private void RewriteJmp()
        {
            this.rtlc = InstrClass.Transfer | InstrClass.Delay;
            var src = SrcOp(instr.Operands[0]);
            m.GotoD(((MemoryAccess)src).EffectiveAddress);
        }

        private void RewriteJsr()
        {
            this.rtlc = InstrClass.Transfer | InstrClass.Delay;
            var dst = SrcOp(instr.Operands[0], null);
            m.CallD(dst, 0);
        }

        private void RewriteMac(PrimitiveType dt)
        {
            var left = SrcOp(instr.Operands[0]);
            var right = SrcOp(instr.Operands[1]);
            var mac = binder.EnsureRegister(Registers.mac);
            var mul = m.SMul(left, right);
            mul.DataType = dt;
            m.Assign(mac, m.IAdd(mul, mac));
        }

        private void RewriteMov()
        {
            var src = SrcOp(instr.Operands[0], a => (sbyte)a);
            var dst = DstOp(instr.Operands[1], src, a => a);
        }

        private void RewriteMova()
        {
            var src = (MemoryAccess)SrcOp(instr.Operands[0], a => (sbyte)a);
            var dst = DstOp(instr.Operands[1], src.EffectiveAddress, a => a);
        }

        private void RewriteMovt()
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var dst = DstOp(instr.Operands[0], t, a => m.Cast(PrimitiveType.Int32, a));
        }

        private void RewriteMul_l()
        {
            var macl = binder.EnsureRegister(Registers.macl);
            var op1 = SrcOp(instr.Operands[0]);
            var op2 = SrcOp(instr.Operands[1]);
            m.Assign(macl, m.IMul(op2, op1));
        }

        private void RewriteMul_w(DataType dt, Func<Expression, Expression, Expression> fn)
        {
            var macl = binder.EnsureRegister(Registers.macl);
            var op1 = m.Cast(dt, SrcOp(instr.Operands[0]));
            var op2 = m.Cast(dt, SrcOp(instr.Operands[1]));
            m.Assign(macl, fn(op2, op1));
        }

        private void RewriteOcbi()
        {
            var mem = (MemoryAccess)SrcOp(instr.Operands[0]);
            var intrinsic = host.PseudoProcedure("__ocbi", VoidType.Instance, mem.EffectiveAddress);
            m.SideEffect(intrinsic);
        }

        private void RewriteRot(string intrinsic)
        {
            var op1 = SrcOp(instr.Operands[0]);
            m.Assign(op1, host.PseudoProcedure(intrinsic, op1.DataType, op1, m.Int32(1)));
        }

        private void RewriteRotc(string intrinsic)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var op1 = SrcOp(instr.Operands[0]);
            m.Assign(op1, host.PseudoProcedure(intrinsic, op1.DataType, op1, m.Int32(1), t));
        }

        private void RewriteRts()
        {
            this.rtlc = InstrClass.Transfer | InstrClass.Delay;
            m.Return(0, 0);
        }

        private void RewriteShd(Func<Expression, Expression, Expression> fnLeft, Func<Expression, Expression, Expression> fnRight)
        {
            var sh = SrcOp(instr.Operands[0]);
            var dst = DstOp(instr.Operands[1], sh, (d, s) =>
                m.Conditional(d.DataType, m.Ge0(s), fnLeft(d, s), fnRight(d, s)));
        }

        private void RewriteShift(Func<Expression, Expression, Expression> fn, int c)
        {
            var src = Constant.Int32(c);
            var dst = DstOp(instr.Operands[0], src, fn);
        }

        private void RewriteSwapW()
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = SrcOp(instr.Operands[1]);
            m.Assign(dst, host.PseudoProcedure("__swap_w", dst.DataType, src));
        }

        private void RewriteTst()
        {
            var op1 = SrcOp(instr.Operands[0], u => (byte)u);
            var op2 = SrcOp(instr.Operands[1]);
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(t, m.Eq0(m.And(op2, op1)));
        }

        private void RewriteUnary(Func<Expression, Expression> fn)
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = DstOp(instr.Operands[1], src, fn);
        }

        private void RewriteXtrct()
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = SrcOp(instr.Operands[1]);
            m.Assign(dst, host.PseudoProcedure("__xtrct", dst.DataType, dst, src));
        }
    }
}
