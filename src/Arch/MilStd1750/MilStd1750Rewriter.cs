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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.MilStd1750
{
    public class MilStd1750Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private static readonly FlagGroupStorage C = Registers.C;
        private static readonly FlagGroupStorage CPZN = new FlagGroupStorage(Registers.sw, (uint) (FlagM.CF |FlagM.PF | FlagM.ZF | FlagM.NF), "CPZN", PrimitiveType.Byte);
        private static readonly FlagGroupStorage N = Registers.N;
        private static readonly FlagGroupStorage P = Registers.P;
        private static readonly FlagGroupStorage PZ = new FlagGroupStorage(Registers.sw, (uint) (FlagM.PF | FlagM.ZF), "PZ", PrimitiveType.Byte);
        private static readonly FlagGroupStorage PZN = new FlagGroupStorage(Registers.sw, (uint) (FlagM.PF | FlagM.ZF | FlagM.NF), "PZN", PrimitiveType.Byte);
        private static readonly FlagGroupStorage Z = Registers.Z;
        private static readonly FlagGroupStorage ZN = new FlagGroupStorage(Registers.sw, (uint) (FlagM.ZF | FlagM.NF), "ZN", PrimitiveType.Byte);

        private readonly MilStd1750Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Instruction> dasm;
        private readonly RtlEmitter m;
        private readonly List<RtlInstruction> rtls;
        private InstrClass iclass;
#nullable disable
        private Instruction instr;
#nullable enable

        public MilStd1750Rewriter(MilStd1750Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new MilStd1750Disassembler(this.arch, rdr).GetEnumerator();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                rtls.Clear();
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest(instr);
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    m.Invalid();
                    iclass = InstrClass.Invalid;
                    break;
                case Mnemonic.a:
                case Mnemonic.aim:
                case Mnemonic.aisp:
                case Mnemonic.ar:
                    RewriteArithmetic(m.IAdd);
                    break;
                case Mnemonic.ab: RewriteAb(); break;
                case Mnemonic.andm:
                    RewriteLogical(m.And);
                    break;
                case Mnemonic.bez: RewriteBranch(ConditionCode.EQ, Z); break;
                case Mnemonic.bge: RewriteBranch(ConditionCode.GE, PZ); break;
                case Mnemonic.bgt: RewriteBranch(ConditionCode.GE, P); break;
                case Mnemonic.ble: RewriteBranch(ConditionCode.LE, ZN); break;
                case Mnemonic.blt: RewriteBranch(ConditionCode.LT, N); break;
                case Mnemonic.bnz: RewriteBranch(ConditionCode.NE, Z); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.c:
                case Mnemonic.cim:
                case Mnemonic.cisp:
                    RewriteCmp();
                    break;
                case Mnemonic.cbl: RewriteCbl(); break;
                case Mnemonic.d:
                case Mnemonic.dim:
                case Mnemonic.dr:
                    RewriteD();
                    break;
                case Mnemonic.da:
                    RewriteDa();
                    break;
                case Mnemonic.db:
                    RewriteDb();
                    break;
                case Mnemonic.dcr: RewriteDcr(); break;
                case Mnemonic.ddr: RewriteDdr(); break;

                case Mnemonic.disp:
                case Mnemonic.dv:
                case Mnemonic.dvr:
                case Mnemonic.dvim:
                    RewriteDv();
                    break;
                case Mnemonic.dlb: RewriteDlb(); break;
                case Mnemonic.dlr: RewriteDlr(); break;
                case Mnemonic.dmr: RewriteDmr(); break;
                case Mnemonic.dneg: RewriteDneg(); break;
                case Mnemonic.dsra: RewriteDsra(); break;
                case Mnemonic.dsr: RewriteDsr(); break;
                case Mnemonic.dst: RewriteDst(); break;
                case Mnemonic.efa: RewriteEf(m.FAdd); break;
                case Mnemonic.efar: RewriteEfr(m.FAdd); break;
                case Mnemonic.efc: RewriteEfc(); break;
                case Mnemonic.efd: RewriteEf(m.FDiv); break;
                case Mnemonic.efdr: RewriteEfr(m.FDiv); break;
                case Mnemonic.efix: RewriteEfix(); break;
                case Mnemonic.efl:
                    RewriteEfl();
                    break;
                case Mnemonic.eflt: RewriteEflt(); break;
                case Mnemonic.efm: RewriteEf(m.FMul); break;
                case Mnemonic.efmr: RewriteEfr(m.FMul); break;
                case Mnemonic.efs: RewriteEf(m.FSub); break;
                case Mnemonic.efsr: RewriteEfr(m.FSub); break;
                case Mnemonic.efst: RewriteEfst(); break;
                case Mnemonic.fab: RewriteFab(); break;
                case Mnemonic.fdb: RewriteFdb(); break;
                case Mnemonic.fmb: RewriteFmb(); break;
                case Mnemonic.incm: RewriteIncm(); break;
                case Mnemonic.jc: RewriteJc(); break;
                case Mnemonic.lb: RewriteLb(); break;
                case Mnemonic.l:
                case Mnemonic.lim:
                case Mnemonic.lisp:
                case Mnemonic.lr:
                    RewriteLoad(); break;
                case Mnemonic.llb: RewriteLlb(); break;
                case Mnemonic.lub: RewriteLub(); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.orb: RewriteOrb(); break;
                case Mnemonic.orim:
                case Mnemonic.orr:
                    RewriteLogical(m.Or);
                    break;
                case Mnemonic.popm: RewritePopm(); break;
                case Mnemonic.pshm: RewritePshm(); break;
                case Mnemonic.sar: RewriteSar(); break;
                case Mnemonic.sisp:
                case Mnemonic.sr:
                    RewriteArithmetic(m.ISub);
                    break;
                case Mnemonic.sjs: RewriteSjs(); break;
                case Mnemonic.sll: RewriteSll(); break;
                case Mnemonic.slr: RewriteSlr(); break;
                case Mnemonic.soj: RewriteSoj(); break;
                case Mnemonic.sra: RewriteSra(); break;
                case Mnemonic.srl: RewriteSrl(); break;
                case Mnemonic.st:
                case Mnemonic.stc:
                    RewriteStore(); 
                    break;

                case Mnemonic.stlb: RewriteStlb(); break;
                case Mnemonic.stub: RewriteStub(); break;
                case Mnemonic.tbr: RewriteTbr(); break;
                case Mnemonic.urs: RewriteUrs(); break;
                case Mnemonic.xbr: RewriteXbr(); break;
                case Mnemonic.xorr:
                    RewriteLogical(m.Xor);
                    break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            }
        }



        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EmitUnitTest(Instruction instr, string message = "")
        {
            var testgenSvc = arch.Services.GetService<ITestGenerationService>();
            testgenSvc?.ReportMissingRewriter("MS1750Rw", instr, instr.Mnemonic.ToString(), rdr, message);
        }

        private void AssignFlags(FlagGroupStorage flags, Expression e) => m.Assign(binder.EnsureFlagGroup(flags), e);
        private Address Addr(int iOp) => ((AddressOperand) instr.Operands[iOp]).Address;


        private Expression AltMem(DataType dt)
        {
            Expression ea = Reg(0);
            var displacement = Imm(1);
            if (!displacement.IsZero)
            {
                ea = m.IAdd(ea, displacement);
            }
            return m.Mem(dt, ea);
        }

        /// <summary>
        /// Double register.
        /// </summary>
        private Identifier DReg(int iOp)
        {
            var regHi = ((RegisterOperand) instr.Operands[iOp]).Register;
            var regLo = Registers.GpRegs[(regHi.Number + 1) & 0xF];
            return binder.EnsureSequence(PrimitiveType.Word32, regHi, regLo);
        }

        /// <summary>
        /// Extended Float register triple.
        /// </summary>
        private Identifier Ef(int iOp)
        {
            var regHi = ((RegisterOperand) instr.Operands[iOp]).Register;
            var regMi = Registers.GpRegs[(regHi.Number + 1) & 0xF];
            var regLo = Registers.GpRegs[(regHi.Number + 2) & 0xF];
            return binder.EnsureSequence(MilStd1750Architecture.Real48, regHi, regMi, regLo);
        }

        private Constant Imm(int iOp) => ((ImmediateOperand) instr.Operands[iOp]).Value;
        private Identifier Reg(int iOp) => binder.EnsureRegister(((RegisterOperand) instr.Operands[iOp]).Register);

        private Expression Op(int iOp)
        {
            switch (instr.Operands[iOp])
            {
            case RegisterOperand reg:
                return binder.EnsureRegister(reg.Register);
            case ImmediateOperand imm:
                return imm.Value;
            case AddressOperand addr:
                return addr.Address;
            case MemoryOperand mem:
                Expression ea;
                if (mem.Index != null)
                {
                    ea = binder.EnsureRegister(mem.Index);
                    if (mem.Displacement != 0)
                    {
                        ea = m.IAdd(ea, mem.Displacement);
                    }
                }
                else
                {
                    ea = Address.Ptr16(mem.Displacement);
                }
                return m.Mem(mem.Width, ea);
            default:
                throw new NotImplementedException($"Rewriting {instr.Operands[iOp].GetType().Name} is not supported.");
            }
        }

        private void RewriteAb()
        {
            var src = AltMem(PrimitiveType.Word16);
            var dst = binder.EnsureRegister(Registers.GpRegs[2]);
            m.Assign(dst, m.IAdd(dst, src));
            AssignFlags(CPZN, m.Cond(dst));
        }

        private void RewriteArithmetic(Func<Expression, Expression, Expression> fn)
        {
            var src = Op(1);
            var dst = Op(0);
            m.Assign(dst, fn(dst, src));
            AssignFlags(CPZN, m.Cond(dst));
        }

        private void RewriteBr()
        {
            m.Goto(Addr(0));
        }

        private void RewriteBranch(ConditionCode cc, FlagGroupStorage grf)
        {
            m.Branch(m.Test(cc, binder.EnsureFlagGroup(grf)), Addr(0));
        }

        private void RewriteCbl()
        {
            var ea = binder.CreateTemporary(PrimitiveType.Ptr16);
            var lo = binder.CreateTemporary(PrimitiveType.Word16);
            var hi = binder.CreateTemporary(PrimitiveType.Word16);
            var reg = Reg(0);
            m.Assign(ea, ((MemoryAccess) Op(1)).EffectiveAddress);
            m.Assign(lo, m.Mem16(ea));
            m.Assign(hi, m.Mem16(m.AddSubSignedInt(ea, 1)));
            AssignFlags(C, m.Gt(lo, hi));
            AssignFlags(N, m.Lt(reg, lo));
            AssignFlags(Z, m.Cand(m.Le(lo, reg), m.Le(reg, hi)));
            AssignFlags(P, m.Lt(hi, reg));
        }
    
        private void RewriteCmp()
        {
            var left = Op(0);
            var right = Op(1);
            AssignFlags(PZN, m.Cond(m.ISub(left, right)));
        }

        private void RewriteD()
        {
            var ra0 = ((RegisterOperand) instr.Operands[0]).Register;
            var ra1 = Registers.GpRegs[(ra0.Number + 1) & 0xF];
            var dividend = binder.EnsureSequence(PrimitiveType.Int32, ra0, ra1);
            var divisor = binder.CreateTemporary(instr.Operands[1].Width);
            var quo = binder.EnsureRegister(ra0);
            var rem = binder.EnsureRegister(ra1);
            m.Assign(divisor, Op(1));
            m.Assign(quo, m.SDiv(dividend, divisor));
            m.Assign(rem, m.Remainder(dividend, divisor));
            AssignFlags(PZN, m.Cond(quo));
        }

        private void RewriteDa()
        {
            var src = Op(1);
            var dst = DReg(0);
            m.Assign(dst, m.IAdd(dst, src));
            AssignFlags(CPZN, m.Cond(dst));
        }

        private void RewriteDb()
        {
            var ra0 = ((RegisterOperand) instr.Operands[0]).Register;
            var ra1 = Registers.GpRegs[(ra0.Number + 1) & 0xF];
            var dividend = binder.EnsureSequence(PrimitiveType.Int32, Registers.GpRegs[0], Registers.GpRegs[1]);
            var divisor = binder.CreateTemporary(instr.Operands[1].Width);
            var quo = binder.EnsureRegister(ra0);
            var rem = binder.EnsureRegister(ra1);
            m.Assign(divisor, Op(1));
            m.Assign(quo, m.SDiv(dividend, divisor));
            m.Assign(rem, m.Remainder(dividend, divisor));
            AssignFlags(PZN, m.Cond(quo));
        }

        private void RewriteDv()
        {
            var ra0 = ((RegisterOperand) instr.Operands[0]).Register;
            var ra1 = Registers.GpRegs[(ra0.Number + 1) & 0xF];
            var dividend = binder.EnsureRegister(ra0);
            var divisor = binder.CreateTemporary(instr.Operands[1].Width);
            m.Assign(divisor, Op(1));
            var quo = binder.EnsureRegister(ra0);
            var rem = binder.EnsureRegister(ra1);
            m.Assign(quo, m.SDiv(dividend, divisor));
            m.Assign(rem, m.Remainder(dividend, divisor));
            AssignFlags(PZN, m.Cond(quo));
        }

        private void RewriteDcr()
        {
            var left = DReg(0);
            var right = DReg(1);
            AssignFlags(PZN, m.Cond(m.ISub(left, right)));
        }

        private void RewriteDdr()
        {
            var divisor = DReg(1);
            var dividend = DReg(0);
            m.Assign(dividend, m.SDiv(dividend, divisor));
            AssignFlags(PZN, m.Cond(dividend));
        }

        private void RewriteDlb()
        {
            var baseReg = Reg(0);
            var disp = Imm(1);
            var dst = binder.EnsureSequence(PrimitiveType.Word32, Registers.GpRegs[0], Registers.GpRegs[2]);
            m.Assign(dst, m.Mem32(m.IAdd(baseReg, disp)));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteDlr()
        {
            var src = DReg(1);
            var dst = DReg(0);
            m.Assign(dst, src);
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteDmr()
        {
            var src = DReg(1);
            var dst = DReg(0);
            m.Assign(dst, m.IMul(dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteDneg()
        {
            var src = DReg(1);
            var dst = DReg(0);
            m.Assign(dst, m.Neg(src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteDsra()
        {
            var dst = DReg(0);
            var shift = Imm(1);
            m.Assign(dst, m.Sar(dst, shift));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteDsr()
        {
            var src = DReg(1);
            var dst = DReg(0);
            m.Assign(dst, m.ISub(dst, src));
            AssignFlags(CPZN, m.Cond(dst));
        }

        private void RewriteDst()
        {
            var src = DReg(0);
            var dst = Op(1);
            m.Assign(dst, src);
        }

        private void RewriteEfc()
        {
            var src = Op(1);
            var dst = Ef(0);
            AssignFlags(PZN, m.Cond(m.FSub(dst, src)));
        }

        private void RewriteEfix()
        {
            var src = Ef(1);
            var dst = DReg(0);
            m.Assign(dst, m.Convert(src, MilStd1750Architecture.Real48, PrimitiveType.Int32));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteEfl()
        {
            var src = Op(1);
            var dst = Ef(0);
            m.Assign(dst, src);
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteEflt()
        {
            var src = DReg(1);
            var dst = Ef(0);
            m.Assign(dst, m.Convert(src, PrimitiveType.Int32, MilStd1750Architecture.Real48));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteEf(Func<Expression, Expression, Expression> fn)
        {
            var src = Op(1);
            var dst = Ef(0);
            m.Assign(dst, fn(dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteEfr(Func<Expression, Expression, Expression> fn)
        {
            var src = Ef(1);
            var dst = Ef(0);
            m.Assign(dst, fn(dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteEfst()
        {
            var src = Ef(0);
            var dst = Op(1);
            m.Assign(dst, src);
        }

        private void RewriteFab()
        {
            var src = AltMem(PrimitiveType.Real32);
            var dst = binder.EnsureSequence(src.DataType, Registers.GpRegs[0], Registers.GpRegs[1]);
            m.Assign(dst, m.FAdd(dst, src));
            AssignFlags(CPZN, m.Cond(dst));
        }

        private void RewriteFdb()
        {
            var src = AltMem(PrimitiveType.Real32);
            var dst = binder.EnsureSequence(src.DataType, Registers.GpRegs[0], Registers.GpRegs[1]);
            m.Assign(dst, m.FDiv(dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteFmb()
        {
            var src = AltMem(PrimitiveType.Real32);
            var dst = binder.EnsureSequence(src.DataType, Registers.GpRegs[0], Registers.GpRegs[1]);
            m.Assign(dst, m.FMul(dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteIncm()
        {
            var src = Imm(0);
            var mem = Op(1);
            var tmp = binder.CreateTemporary(mem.DataType);
            m.Assign(tmp, m.IAdd(mem, src));
            m.Assign(mem, tmp);
            AssignFlags(CPZN, m.Cond(tmp));
        }

        private void RewriteJc()
        {
            var c = Imm(0);
            var target = Addr(1);
            switch (c.ToUInt32())
            {
            case 0:
                m.Nop();
                break;
            case 1:
                m.Branch(m.Test(ConditionCode.LT, binder.EnsureFlagGroup(N)), target);
                break;
            case 2:
                m.Branch(m.Test(ConditionCode.EQ, binder.EnsureFlagGroup(Z)), target);
                break;
            case 3:
                m.Branch(m.Test(ConditionCode.LT, binder.EnsureFlagGroup(ZN)), target);
                break;
            case 4:
                m.Branch(m.Test(ConditionCode.GT, binder.EnsureFlagGroup(P)), target);
                break;
            case 5:
                m.Branch(m.Test(ConditionCode.NE, binder.EnsureFlagGroup(PZN)), target);
                break;
            case 6:
                m.Branch(m.Test(ConditionCode.GE, binder.EnsureFlagGroup(PZ)), target);
                break;
            case 7:
            case 0xF:
                m.Goto(target);
                break;
            default:
                EmitUnitTest(this.instr, $"C = 0x{c.ToUInt32():X2}");
                break;
            }
        }

        private void RewriteLb()
        {
            var baseReg = Reg(0);
            var disp = Imm(1);
            var dst = binder.EnsureRegister(Registers.GpRegs[2]);
            m.Assign(dst, m.Mem16(m.IAdd(baseReg, disp)));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteLoad()
        {
            var src = Op(1);
            var dst = Op(0);
            m.Assign(dst, src);
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteLogical(Func<Expression,Expression,Expression> fn)
        {
            var src = Op(1);
            var dst = Op(0);
            m.Assign(dst, fn(dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteLlb()
        {
            var src = Op(1);
            var dst = Op(0);
            m.Assign(dst, m.Dpb(dst, m.Slice(PrimitiveType.Byte, src, 0), 0));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteLub()
        {
            var src = Op(1);
            var dst = Op(0);
            m.Assign(dst, m.Dpb(dst, m.Slice(PrimitiveType.Byte, src, 0), 8));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteMov()
        {
            var ra = Reg(0);
            var rb = Reg(1);
            var tmpA = binder.CreateTemporary(PrimitiveType.Ptr16);
            var tmpB = binder.CreateTemporary(PrimitiveType.Ptr16);
            m.Assign(tmpA, ra);
            m.Assign(tmpB, rb);
            m.SideEffect(host.PseudoProcedure("__mov", VoidType.Instance, tmpA, tmpB));
        }

        private void RewriteNeg()
        {
            var src = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, m.Neg(src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteOrb()
        {
            var src = AltMem(PrimitiveType.Word16);
            var dst = binder.EnsureRegister(Registers.GpRegs[2]);
            m.Assign(dst, m.Or(dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }


        private void RewritePopm()
        {
            var ra = ((RegisterOperand) instr.Operands[0]).Register;
            var rb = ((RegisterOperand) instr.Operands[1]).Register;
            var r15 = binder.EnsureRegister(Registers.GpRegs[15]);
            if (ra.Number == rb.Number)
            {
                m.Nop();
            }
            else if (ra.Number < rb.Number)
            {
                for (int i = 0; i < rb.Number - ra.Number; ++i)
                {
                    if (ra.Number + i != 15)
                        m.Assign(binder.EnsureRegister(Registers.GpRegs[ra.Number + i]), m.Mem16(r15));
                    m.Assign(r15, m.AddSubSignedInt(r15, 1));
                }
            }
            else
            {
                for (int i = 0; i < 15 - ra.Number; ++i)
                {
                    if (ra.Number + i != 15)
                        m.Assign(binder.EnsureRegister(Registers.GpRegs[ra.Number + i]), m.Mem16(r15));
                    m.Assign(r15, m.AddSubSignedInt(r15, 1));
                }
                for (int i = 0; i < rb.Number; ++i)
                {
                    m.Assign(binder.EnsureRegister(Registers.GpRegs[ra.Number + i]), m.Mem16(r15));
                    m.Assign(r15, m.AddSubSignedInt(r15, 1));
                }
            }
        }

        private void RewritePshm()
        {
            var ra = ((RegisterOperand) instr.Operands[0]).Register;
            var rb = ((RegisterOperand) instr.Operands[1]).Register;
            var r15 = binder.EnsureRegister(Registers.GpRegs[15]);
            if (ra.Number == rb.Number)
            {
                m.Nop();
            }
            else if (ra.Number < rb.Number)
            {
                for (int i = 0; i < rb.Number - ra.Number; ++i)
                {
                    m.Assign(r15, m.ISub(r15, 1));
                    m.Assign(m.Mem16(r15), binder.EnsureRegister(Registers.GpRegs[rb.Number - i]));
                }
            }
            else
            {
                for (int i = 0; i < rb.Number; ++i)
                {
                    m.Assign(r15, m.ISub(r15, 1));
                    m.Assign(m.Mem16(r15), binder.EnsureRegister(Registers.GpRegs[rb.Number - i]));
                }
                for (int i = 0; i < 15 - ra.Number; ++i)
                {
                    m.Assign(r15, m.ISub(r15, 1));
                    m.Assign(m.Mem16(r15), binder.EnsureRegister(Registers.GpRegs[15 - i]));
                }
            }
        }

        private void RewriteSar()
        {
            var src = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, host.PseudoProcedure("__shift_arithmetic", dst.DataType, dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteSjs()
        {
            var dst = Addr(1);
            m.Call(dst, 2);
        }

        private void RewriteSll()
        {
            var src = Op(1);
            var dst = Reg(0);
            m.Assign(dst, m.Shl(dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteSlr()
        {
            var src = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, host.PseudoProcedure("__shift_logical", dst.DataType, dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteSoj()
        {
            var reg = Reg(0);
            var target = Op(1);
            AssignFlags(PZN, m.Cond(reg));
            if (target is Address addr)
            {
                m.Assign(reg, m.ISub(reg, 1));
                m.Branch(m.Ne0(reg), addr);
            }
            else
            {
                var tmp = binder.CreateTemporary(PrimitiveType.Word32);
                var idx = Reg(2);
                m.Assign(tmp, m.IAdd(idx, target));
                m.Assign(reg, m.ISub(reg, 1));
                m.Goto(tmp);
            }
        }

        private void RewriteSra()
        {
            var src = Op(1);
            var dst = Reg(0);
            m.Assign(dst, m.Sar(dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteSrl()
        {
            var src = Op(1);
            var dst = Reg(0);
            m.Assign(dst, m.Shr(dst, src));
            AssignFlags(PZN, m.Cond(dst));
        }

        private void RewriteStore()
        {
            var src = Op(0);
            var dst = Op(1);
            m.Assign(dst, src);
        }

        private void RewriteStlb()
        {
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, m.Slice(PrimitiveType.Byte, Reg(0), 0));
            var tmp2 = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(tmp2, Op(1));
            m.Assign(Op(1), m.Dpb(tmp2, tmp, 0));
        }

        private void RewriteStub()
        {
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, m.Slice(PrimitiveType.Byte, Reg(0), 0));
            var tmp2 = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(tmp2, Op(1));
            m.Assign(Op(1), m.Dpb(tmp2, tmp, 8));
        }

        private void RewriteTbr()
        {
            var bit = Imm(0);
            var src = Op(1);
            var tmp = binder.CreateTemporary(src.DataType);
            m.Assign(tmp, m.And(src, m.Shl(Constant.UInt16(1), bit)));
            AssignFlags(Z, m.Eq0(tmp));
        }

        private void RewriteUrs()
        {
            m.Return(2, 0);
        }

        private void RewriteXbr()
        {
            var dst = Reg(0);
            m.Assign(dst, host.PseudoProcedure("__xbr", PrimitiveType.Word32, dst));
            AssignFlags(PZN, m.Cond(dst));
        }
    }
}