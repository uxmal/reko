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
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.LatticeMico
{
    public class LatticeMico32Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly LatticeMico32Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<LatticeMico32Instruction> dasm;
        private LatticeMico32Instruction instr;
        private InstrClass iclass;
        private RtlEmitter m;

        public LatticeMico32Rewriter(LatticeMico32Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new LatticeMico32Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    break;
                case Mnemonic.Invalid:
                case Mnemonic.reserved:
                    m.Invalid(); break;
                case Mnemonic.add: RewriteBinop(m.IAdd); break;
                case Mnemonic.addi: RewriteBinop(m.IAdd); break;
                case Mnemonic.and: RewriteBinop(m.And); break;
                case Mnemonic.andhi: RewriteHiImm(m.And); break;
                case Mnemonic.andi: RewriteBinop(m.And); break;
                case Mnemonic.b: RewriteIndirectGoto(); break;
                case Mnemonic.be: RewriteBranch(m.Eq); break;
                case Mnemonic.bg: RewriteBranch(m.Gt); break;
                case Mnemonic.bge: RewriteBranch(m.Ge); break;
                case Mnemonic.bgeu: RewriteBranch(m.Uge); break;
                case Mnemonic.bgu: RewriteBranch(m.Ugt); break;
                case Mnemonic.bi: RewriteGoto(); break;
                case Mnemonic.bne: RewriteBranch(m.Ne); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.calli: RewriteCall(); break;
                case Mnemonic.cmpe: RewriteCmp(m.Eq); break;
                case Mnemonic.cmpei: RewriteCmp(m.Eq); break;
                case Mnemonic.cmpg: RewriteCmp(m.Gt); break;
                case Mnemonic.cmpgi: RewriteCmp(m.Gt); break;
                case Mnemonic.cmpge: RewriteCmp(m.Ge); break;
                case Mnemonic.cmpgei: RewriteCmp(m.Ge); break;
                case Mnemonic.cmpgeu: RewriteCmp(m.Uge); break;
                case Mnemonic.cmpgeui: RewriteCmp(m.Uge); break;
                case Mnemonic.cmpgu: RewriteCmp(m.Ugt); break;
                case Mnemonic.cmpgui: RewriteCmp(m.Ugt); break;
                case Mnemonic.cmpne: RewriteCmp(m.Ne); break;
                case Mnemonic.cmpnei: RewriteCmp(m.Ne); break;
                case Mnemonic.div: RewriteBinop(m.SDiv); break;
                case Mnemonic.divu: RewriteBinop(m.UDiv); break;
                case Mnemonic.lb: RewriteLoad(PrimitiveType.Int32); break;
                case Mnemonic.lbu: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.lh: RewriteLoad(PrimitiveType.Int32); break;
                case Mnemonic.lhu: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.lw: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.mod: RewriteBinop(m.Mod); break;
                case Mnemonic.modu: RewriteBinop(m.Mod); break;
                case Mnemonic.mul: RewriteBinop(m.IMul); break;
                case Mnemonic.muli: RewriteBinop(m.IMul); break;
                case Mnemonic.nor: RewriteBinop(Nor); break;
                case Mnemonic.nori: RewriteBinop(Nor); break;
                case Mnemonic.or: RewriteBinop(m.Or); break;
                case Mnemonic.orhi: RewriteHiImm(m.Or); break;
                case Mnemonic.ori: RewriteBinop(m.Or); break;
                case Mnemonic.sb: RewriteStore(); break;
                case Mnemonic.sextb: RewriteSext(PrimitiveType.SByte); break;
                case Mnemonic.sexth: RewriteSext(PrimitiveType.Int16); break;
                case Mnemonic.sh: RewriteStore(); break;
                case Mnemonic.sl: RewriteBinop(m.Shl); break;
                case Mnemonic.sli: RewriteBinop(m.Shl); break;
                case Mnemonic.sr: RewriteBinop(m.Sar); break;
                case Mnemonic.sri: RewriteBinop(m.Sar); break;
                case Mnemonic.sru: RewriteBinop(m.Shr); break;
                case Mnemonic.srui: RewriteBinop(m.Shr); break;
                case Mnemonic.sub: RewriteBinop(m.ISub); break;
                case Mnemonic.sw: RewriteStore(); break;

                case Mnemonic.xnor: RewriteBinop(Xnor); break;
                case Mnemonic.xnori: RewriteBinop(Xnor); break;
                case Mnemonic.xor: RewriteBinop(m.Xor); break;
                case Mnemonic.xori: RewriteBinop(m.Xor); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, rtls.ToArray())
                {
                    Class = iclass
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static HashSet<Mnemonic> opcode_seen = new HashSet<Mnemonic>();

        private void EmitUnitTest()
        {
            m.Invalid();
            iclass = InstrClass.Invalid;

            if (opcode_seen.Contains(instr.Mnemonic))
                return;
            opcode_seen.Add(instr.Mnemonic);
            host.Warn(
                instr.Address,
                "LatticeMico32 instruction '{0}' is not supported yet.",
                instr.Mnemonic.ToString());

            var r2 = rdr.Clone();
            r2.Offset -= instr.Length;
            var hexBytes = string.Join("", r2.ReadBytes(instr.Length).Select(b => $"{b:X2}"));

            var sb = new StringBuilder();
            sb.AppendLine($"        [Test]");
            sb.AppendLine($"        public void Lm32Rw_{instr.Mnemonic}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            Given_HexString(\"{hexBytes}\"); // {instr}");
            sb.AppendLine($"            AssertCode(");
            sb.AppendLine($"                \"0|L--|00100000({instr.Length}): 1 instructions\",");
            sb.AppendLine($"                \"1|L--|@@@\");");
            sb.AppendLine("        }");
            Debug.WriteLine(sb.ToString());
            Console.WriteLine(sb.ToString());
        }

        private Expression Nor(Expression a, Expression b)
        {
            return m.Comp(m.Or(a, b));
        }

        private Expression Xnor(Expression a, Expression b)
        {
            return m.Comp(m.Xor(a, b));
        }

        private Expression Rewrite(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rop:
                if (rop.Register.Number == 0)
                    return Constant.Create(rop.Register.DataType, 0);
                else
                    return binder.EnsureRegister(rop.Register);
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mem:
                Expression ea;
                if (mem.Base.Number == 0)
                {
                    ea = Address.Ptr32((uint) mem.Offset);
                }
                else
                {
                    ea = binder.EnsureRegister(mem.Base);
                    ea = m.AddSubSignedInt(ea, mem.Offset);
                }
                return m.Mem(mem.Width, ea);
            case AddressOperand aop:
                return aop.Address;
            }
            throw new NotImplementedException($"{op.GetType().Name} not implemented.");
        }

        private void RewriteBinop(Func<Expression, Expression, Expression> fn)
        {
            var src1 = Rewrite(instr.Operands[1]);
            var src2 = Rewrite(instr.Operands[2]);
            var dst = Rewrite(instr.Operands[0]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteBranch(Func<Expression, Expression, Expression> fn)
        {
            var src1 = Rewrite(instr.Operands[0]);
            var src2 = Rewrite(instr.Operands[1]);
            var dst = ((AddressOperand)instr.Operands[2]).Address;
            m.Branch(fn(src1, src2), dst);
        }

        private void RewriteCall()
        {
            var dst = Rewrite(instr.Operands[0]);
            m.Call(dst, 0);
        }

        private void RewriteCmp(Func<Expression, Expression, Expression> fn)
        {
            var src1 = Rewrite(instr.Operands[1]);
            var src2 = Rewrite(instr.Operands[2]);
            var dst = Rewrite(instr.Operands[0]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteGoto()
        {
            m.Goto(Rewrite(instr.Operands[0]));
        }

        private void RewriteIndirectGoto()
        {
            var reg = ((RegisterOperand) instr.Operands[0]).Register;
            if (reg == Registers.ra)
            {
                m.Return(0, 0);
            }
            else
            {
                m.Goto(binder.EnsureRegister(reg));
            }
        }

        private void RewriteLoad(PrimitiveType dt)
        {
            var src = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[0]);
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                src = m.Cast(dt, src);
            }
            m.Assign(dst, src);
        }

        private void RewriteHiImm(Func<Expression, Expression, Expression> fn)
        {
            var src1 = Rewrite(instr.Operands[1]);
            var imm = ((ImmediateOperand)instr.Operands[2]).Value;
            var dst = Rewrite(instr.Operands[0]);
            m.Assign(dst, fn(src1, Constant.Word32(imm.ToInt32() << 16)));
        }

        private void RewriteSext(PrimitiveType dt)
        {
            var src = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[0]);
            src = m.Cast(dt, src);
            m.Assign(dst, m.Cast(PrimitiveType.Int32, src));
        }

        private void RewriteStore()
        {
            var src = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[0]);
            if (src.DataType.BitSize > dst.DataType.BitSize)
            {
                src = m.Slice(dst.DataType, src, 0);
            }
            m.Assign(dst, src);
        }
    }
}
