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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.PaRisc
{
    public partial class PaRiscRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly PaRiscArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<PaRiscInstruction> dasm;
        private RtlEmitter m;
        private PaRiscInstruction instr;
        private InstrClass iclass;

        public PaRiscRewriter(PaRiscArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new PaRiscDisassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                var instrs = new List<RtlInstruction>();
                m = new RtlEmitter(instrs);
                switch (instr.Opcode)
                {
                default:
                    EmitUnitTest();
                    goto case Opcode.invalid;
                case Opcode.invalid: m.Invalid(); break;
                case Opcode.add: RewriteAdd(true); break;
                case Opcode.add_c: RewriteAdd_c(); break;
                case Opcode.add_l: RewriteAdd(false); break;
                case Opcode.addb: RewriteAddb(); break;
                case Opcode.addi: RewriteAddi(false); break;
                case Opcode.addi_tc: RewriteAddi(true); break;
                case Opcode.addib: RewriteAddb(); break;
                case Opcode.addil: RewriteAddi(false); break;
                case Opcode.and: RewriteLogical(m.And); break;
                case Opcode.andcm: RewriteLogical((a, b) => m.And(a, m.Comp(b))); break;
                case Opcode.b_l: RewriteBranch(); break;
                case Opcode.be: RewriteBe(); break;
                case Opcode.be_l: RewriteBe(); break;
                case Opcode.bv: RewriteBv(); break;
                case Opcode.cmpb: RewriteCmpb(0, 1); break;
                case Opcode.cmpib: RewriteCmpb(1, 0); break;
                case Opcode.@break: RewriteBreak(); break;
                case Opcode.depwi: RewriteDepwi(); break;
                case Opcode.diag: RewriteDiag(); break;
                case Opcode.extrw: RewriteExtrw(); break;
                case Opcode.fadd: RewriteFpArithmetic(m.FAdd); break;
                case Opcode.fcpy: RewriteFcpy(); break;
                case Opcode.fid: RewriteFid(); break;
                case Opcode.fldd: RewriteFld(PrimitiveType.Real64); break;
                case Opcode.fldw: RewriteFld(PrimitiveType.Real32); break;
                case Opcode.fmpy: RewriteFpArithmetic(m.FMul); break;
                case Opcode.fstd: RewriteFst(PrimitiveType.Real64); break;
                case Opcode.fstw: RewriteFst(PrimitiveType.Real32); break;
                case Opcode.fsub: RewriteFpArithmetic(m.FSub); break;
                case Opcode.ldb: RewriteLd(PrimitiveType.Byte); break;
                case Opcode.ldd: RewriteLd(PrimitiveType.Word64); break;
                case Opcode.ldh: RewriteLd(PrimitiveType.Word16); break;
                case Opcode.ldil: RewriteLdil(); break;
                case Opcode.ldo: RewriteLdo(); break;
                case Opcode.ldsid: RewriteLdsid(); break;
                case Opcode.ldw: RewriteLd(PrimitiveType.Word32); break;
                case Opcode.ldwa: RewriteLd(PrimitiveType.Word32); break;
                case Opcode.mfctl: RewriteMfctl(); break;
                case Opcode.mfctl_w: RewriteMfctl(); break;
                case Opcode.mtctl: RewriteMtctl(); break;
                case Opcode.mtsm: RewriteMtsm(); break;
                case Opcode.mtsp: RewriteMtsp(); break;
                case Opcode.or: RewriteOr(); break;
                case Opcode.rfi: RewriteRfi("__rfi"); break;
                case Opcode.rfi_r: RewriteRfi("__rfi_r"); break;
                case Opcode.shladd: RewriteShladd(); break;
                case Opcode.shrpd: RewriteShrp(PrimitiveType.Word64, PrimitiveType.Word128); break;
                case Opcode.shrpw: RewriteShrp(PrimitiveType.Word32, PrimitiveType.Word64); break;
                case Opcode.stb: RewriteSt(PrimitiveType.Byte); break;
                case Opcode.std: RewriteSt(PrimitiveType.Word64); break;
                case Opcode.stda: RewriteSt(PrimitiveType.Word64); break;
                case Opcode.sth: RewriteSt(PrimitiveType.Word16); break;
                case Opcode.stw: RewriteSt(PrimitiveType.Word32); break;
                case Opcode.sub: RewriteSub(); break;
                case Opcode.subi: RewriteSubi(); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = iclass,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static HashSet<Opcode> seen = new HashSet<Opcode>();

        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for this rewriter.
        /// </summary>
        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.Opcode))
                return;
            seen.Add(dasm.Current.Opcode);

            var bytes = rdr.PeekBeUInt32(-dasm.Current.Length);
            Console.WriteLine("        [Test]");
            Console.WriteLine("        public void PaRiscRw_" + dasm.Current.Opcode + "()");
            Console.WriteLine("        {");
            Console.WriteLine("            BuildTest(\"{0:X8}\");\t// {1}", bytes, dasm.Current.ToString());
            Console.WriteLine("            AssertCode(");
            Console.WriteLine("                \"0|L--|00100000({0}): 1 instructions\",", dasm.Current.Length);
            Console.WriteLine("                \"1|L--|@@@\");");
            Console.WriteLine("        }");
            Console.WriteLine("");
        }

        private void MaybeAnnulNextInstruction(InstrClass iclass, Expression e)
        {
            var addrNext = instr.Address + 8;
            MaybeConditionalJump(InstrClass.ConditionalTransfer, addrNext, false, e);
        }

        private bool MaybeSkipNextInstruction(InstrClass iclass, bool invert, Expression left, Expression right = null)
        {
            var addrNext = instr.Address + 8;
            if (MaybeConditionalJump(iclass, addrNext, invert, left, right))
            {
                this.iclass = InstrClass.ConditionalTransfer;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool MaybeConditionalJump(InstrClass iclass, Address addrTaken, bool invert, Expression left, Expression right = null)
        {
            if (instr.Condition == null)
                return false;

            right = right ?? Constant.Word(left.DataType.BitSize, 0);
            Expression e = RewriteCondition(left, right);
            if (e is Constant c)
            {
                if (!c.IsZero)
                {
                    m.Goto(addrTaken);
                    return true;
                }
            }
            if (invert)
                e = e.Invert();
            m.Branch(e, addrTaken, iclass);
            return true;
        }

        private Expression RewriteOp(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand r:
                if (r.Register == Registers.GpRegs[0])
                    return Constant.Zero(r.Register.DataType);
                else
                    return binder.EnsureRegister(r.Register);
            case ImmediateOperand i:
                return i.Value;
            case LeftImmediateOperand l:
                return l.Value;
            case AddressOperand a:
                return a.Address;
            case MemoryOperand mem:
                Identifier rb = binder.EnsureRegister(mem.Base);
                Expression ea = rb;
                if (mem.Index != null)
                {
                    if (mem.Index != Registers.GpRegs[0])
                    {
                        var idx = binder.EnsureRegister(mem.Index);
                        ea = m.IAdd(ea, idx);
                    }
                }
                else if (mem.Offset != 0)
                {
                    ea = m.IAddS(ea, mem.Offset);
                }
                if (instr.BaseReg == AddrRegMod.mb)
                {
                    m.Assign(rb, ea);
                    ea = rb;
                }
                else if (instr.BaseReg == AddrRegMod.ma)
                {
                    var tmp = binder.CreateTemporary(rb.DataType);
                    m.Assign(tmp, ea);
                    m.Assign(rb, tmp);
                    ea = tmp;
                }
                return m.Mem(mem.Width, ea);
            }
            throw new NotImplementedException($"Unimplemented PA-RISC operand type {op.GetType()}.");
        }

        private Expression RewriteCondition(Expression left, Expression right)
        {
            Expression e;
            switch (instr.Condition.Type)
            {
            case ConditionType.Tr: e = Constant.True(); break;
            case ConditionType.Never: e = Constant.False(); break;
            case ConditionType.Eq:
            case ConditionType.Eq64:
                e = m.Eq(left, right); break;
            case ConditionType.Ne:
            case ConditionType.Ne64:
                e = m.Ne(left, right); break;
            case ConditionType.Lt: e = m.Lt(left, right); break;
            case ConditionType.Le: e = m.Le(left, right); break;
            case ConditionType.Ge:
            case ConditionType.Ge64: e = m.Ge(left, right); break;
            case ConditionType.Gt: e = m.Gt(left, right); break;
            case ConditionType.Ult: e = m.Ult(left, right); break;
            case ConditionType.Ule: e = m.Ule(left, right); break;
            case ConditionType.Uge:
            case ConditionType.Uge64:
                e = m.Uge(left, right); break;
            case ConditionType.Ugt: e = m.Ugt(left, right); break;
            case ConditionType.Nuv:
            case ConditionType.Nuv64:
                e = m.Test(ConditionCode.OV, m.ISub(left, right)); break;
            case ConditionType.Nsv:
                //$TODO: need signed minus/unsigned minus.
                e = m.Test(ConditionCode.OV, m.ISub(left, right)); break;
            case ConditionType.Even:
            case ConditionType.Even64:
                e = m.Eq0(m.And(left, 1)); break;
            default:
                throw new NotImplementedException(instr.Condition.ToString());
            }
            return e;

        }
    }
}
