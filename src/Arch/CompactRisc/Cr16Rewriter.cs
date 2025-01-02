#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.CompactRisc
{
    internal class Cr16Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Cr16Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Cr16Instruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private Cr16Instruction instr;
        private InstrClass iclass;

        public Cr16Rewriter(Cr16Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Cr16cDisassembler(arch, rdr).GetEnumerator();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    host.Warn(instr.Address, "Cr16 instruction {0} not supported yet.", this.instr);
                    goto case Mnemonic.Invalid;
                case Mnemonic.res:
                case Mnemonic.Invalid: m.Invalid(); iclass = InstrClass.Invalid; break;
                case Mnemonic.addb: RewriteAdd(PrimitiveType.Byte); break;
                case Mnemonic.addd: RewriteAdd(PrimitiveType.Word32); break;
                case Mnemonic.addw: RewriteAdd(PrimitiveType.Word16); break;
                case Mnemonic.addcb: RewriteAddc(PrimitiveType.Byte); break;
                case Mnemonic.addcw: RewriteAddc(PrimitiveType.Word16); break;
                case Mnemonic.addub: RewriteAddu(PrimitiveType.Byte); break;
                case Mnemonic.adduw: RewriteAddu(PrimitiveType.Word16); break;
                case Mnemonic.andb: RewriteAnd(PrimitiveType.Byte); break;
                case Mnemonic.andd: RewriteAnd(PrimitiveType.Word32); break;
                case Mnemonic.andw: RewriteAnd(PrimitiveType.Word16); break;
                case Mnemonic.ashub: RewriteAshu(PrimitiveType.Byte); break;
                case Mnemonic.ashud: RewriteAshu(PrimitiveType.Word32); break;
                case Mnemonic.ashuw: RewriteAshu(PrimitiveType.Word16); break;
                case Mnemonic.bal: RewriteBal(); break;
                case Mnemonic.bcc: RewriteBccClear(Registers.C); break;
                case Mnemonic.bcs: RewriteBccSet(Registers.C); break;
                case Mnemonic.beq: RewriteBcc(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.beq0b: RewriteBeq0(PrimitiveType.Byte); break;
                case Mnemonic.beq0w: RewriteBeq0(PrimitiveType.Word16); break;
                case Mnemonic.bfc: RewriteBccClear(Registers.F); break;
                case Mnemonic.bfs: RewriteBccSet(Registers.F); break;
                case Mnemonic.bge: RewriteBcc(ConditionCode.GE, Registers.NZ); break;
                case Mnemonic.bgt: RewriteBcc(ConditionCode.GT, Registers.N); break;
                case Mnemonic.bhi: RewriteBcc(ConditionCode.ULT, Registers.L); break;
                case Mnemonic.bhs: RewriteBcc(ConditionCode.ULE, Registers.LZ); break;
                case Mnemonic.ble: RewriteBcc(ConditionCode.LT, Registers.N); break;
                case Mnemonic.blo: RewriteBcc(ConditionCode.UGT, Registers.LZ); break;
                case Mnemonic.bls: RewriteBcc(ConditionCode.UGE, Registers.L); break;
                case Mnemonic.blt: RewriteBcc(ConditionCode.LT, Registers.NZ); break;
                case Mnemonic.bne: RewriteBcc(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.bne0b: RewriteBne0(PrimitiveType.Byte); break;
                case Mnemonic.bne0w: RewriteBne0(PrimitiveType.Word16); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.cbitb: RewriteCbit(PrimitiveType.Byte); break;
                case Mnemonic.cbitw: RewriteCbit(PrimitiveType.Word16); break;
                case Mnemonic.cinv: RewriteCinv(); break;
                case Mnemonic.cmpb: RewriteCmp(PrimitiveType.Byte); break;
                case Mnemonic.cmpd: RewriteCmp(PrimitiveType.Word32); break;
                case Mnemonic.cmpw: RewriteCmp(PrimitiveType.Word16); break;
                case Mnemonic.di: RewriteDi(); break;
                case Mnemonic.ei: RewriteEi(); break;
                case Mnemonic.eiwait: RewriteEiwait(); break;
                case Mnemonic.excp: RewriteExcp(); break;
                case Mnemonic.jal: RewriteJal(); break;
                case Mnemonic.jcc: RewriteJccClear(Registers.C); break;
                case Mnemonic.jcs: RewriteJccSet(Registers.C); break;
                case Mnemonic.jeq: RewriteJcc(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.jfc: RewriteJccClear(Registers.F); break;
                case Mnemonic.jfs: RewriteJccSet(Registers.F); break;
                case Mnemonic.jge: RewriteJcc(ConditionCode.LT, Registers.NZ); break;
                case Mnemonic.jgt: RewriteJcc(ConditionCode.LE, Registers.N); break;
                case Mnemonic.jhi: RewriteJcc(ConditionCode.ULE, Registers.L); break;
                case Mnemonic.jhs: RewriteJcc(ConditionCode.ULT, Registers.LZ); break;
                case Mnemonic.jle: RewriteJcc(ConditionCode.GE, Registers.N); break;
                case Mnemonic.jlo: RewriteJcc(ConditionCode.UGE, Registers.LZ); break;
                case Mnemonic.jls: RewriteJcc(ConditionCode.UGT, Registers.L); break;
                case Mnemonic.jlt: RewriteJcc(ConditionCode.GE, Registers.NZ); break;
                case Mnemonic.jne: RewriteJcc(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.jr: RewriteJr(); break;
                case Mnemonic.loadb: RewriteLoad(PrimitiveType.Byte); break;
                case Mnemonic.loadd: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.loadw: RewriteLoad(PrimitiveType.Word16); break;
                case Mnemonic.lpr: RewriteLpr(PrimitiveType.Word16); break;
                case Mnemonic.lprd: RewriteLpr(PrimitiveType.Word32); break;
                case Mnemonic.lshb: RewriteLsh(PrimitiveType.Byte); break;
                case Mnemonic.lshd: RewriteLsh(PrimitiveType.Word32); break;
                case Mnemonic.lshw: RewriteLsh(PrimitiveType.Word16); break;
                case Mnemonic.movb: RewriteMov(PrimitiveType.Byte); break;
                case Mnemonic.movd: RewriteMov(PrimitiveType.Word32); break;
                case Mnemonic.movw: RewriteMov(PrimitiveType.Word16); break;
                case Mnemonic.movxb: RewriteMovsx(PrimitiveType.Byte); break;
                case Mnemonic.movxw: RewriteMovsx(PrimitiveType.Word16); break;
                case Mnemonic.movzb: RewriteMovzx(PrimitiveType.Byte); break;
                case Mnemonic.movzw: RewriteMovzx(PrimitiveType.Word16); break;
                case Mnemonic.mulb: RewriteMul(PrimitiveType.Byte); break;
                case Mnemonic.mulw: RewriteMul(PrimitiveType.Word16); break;
                case Mnemonic.mulsb: RewriteMuls(PrimitiveType.Int8, PrimitiveType.Int16); break;
                case Mnemonic.mulsw: RewriteMuls(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.muluw: RewriteMulu(PrimitiveType.Word16); break;
                case Mnemonic.orb: RewriteOr(PrimitiveType.Byte); break;
                case Mnemonic.ord: RewriteOr(PrimitiveType.Word32); break;
                case Mnemonic.orw: RewriteOr(PrimitiveType.Word16); break;
                case Mnemonic.nop: RewriteNop(); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.popret: RewritePopret(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.retx: RewriteRetx(); break;
                case Mnemonic.sbitb: RewriteSbit(PrimitiveType.Byte); break;
                case Mnemonic.sbitw: RewriteSbit(PrimitiveType.Word16); break;
                case Mnemonic.scc: RewriteSccClear(Registers.C); break;
                case Mnemonic.seq: RewriteScc(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.sfc: RewriteSccClear(Registers.F); break;
                case Mnemonic.sfs: RewriteSccSet(Registers.F); break;
                case Mnemonic.sge: RewriteScc(ConditionCode.GE, Registers.NZ); break;
                case Mnemonic.sgt: RewriteScc(ConditionCode.GT, Registers.N); break;
                case Mnemonic.shi: RewriteScc(ConditionCode.UGT, Registers.L); break;
                case Mnemonic.shs: RewriteScc(ConditionCode.UGE, Registers.LZ); break;
                case Mnemonic.sle: RewriteScc(ConditionCode.LT, Registers.N); break;
                case Mnemonic.slo: RewriteScc(ConditionCode.ULT, Registers.LZ); break;
                case Mnemonic.sls: RewriteScc(ConditionCode.ULE, Registers.L); break;
                case Mnemonic.slt: RewriteScc(ConditionCode.LT, Registers.NZ); break;
                case Mnemonic.sne: RewriteScc(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.spr: RewriteSpr(PrimitiveType.Word16); break;
                case Mnemonic.sprd: RewriteSpr(PrimitiveType.Word32); break;
                case Mnemonic.storb: RewriteStore(PrimitiveType.Byte); break;
                case Mnemonic.stord: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.storw: RewriteStore(PrimitiveType.Word16); break;
                case Mnemonic.subb: RewriteSub(PrimitiveType.Byte); break;
                case Mnemonic.subd: RewriteSub(PrimitiveType.Word32); break;
                case Mnemonic.subw: RewriteSub(PrimitiveType.Word16); break;
                case Mnemonic.subcb: RewriteSubc(PrimitiveType.Byte); break;
                case Mnemonic.subcw: RewriteSubc(PrimitiveType.Word16); break;
                case Mnemonic.tbit: RewriteTbit(PrimitiveType.Word16); break;
                case Mnemonic.tbitb: RewriteTbit(PrimitiveType.Byte); break;
                case Mnemonic.tbitw: RewriteTbit(PrimitiveType.Word16); break;
                case Mnemonic.wait: RewriteWait(); break;
                case Mnemonic.xorb: RewriteXor(PrimitiveType.Byte); break;
                case Mnemonic.xord: RewriteXor(PrimitiveType.Word32); break;
                case Mnemonic.xorw: RewriteXor(PrimitiveType.Word16); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, this.iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Cr16Rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private void Assign(Expression dst, Expression src)
        {
            if (dst.DataType.BitSize > src.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = m.Dpb(dst, tmp, 0);
            }
            m.Assign(dst, src);
        }

        private Expression Operand(int iop)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case SequenceStorage seq:
                return binder.EnsureSequence(seq.DataType, seq.Elements);
            case ImmediateOperand imm:
                return imm.Value;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                return m.Mem(mem.Width, ea);
            default:
                throw new AddressCorrelatedException(instr.Address, $"Unimplemented address mode {op.GetType().Name}.");
            }
        }

        private Expression EffectiveAddress(MemoryOperand mem)
        {
            if (mem.Base is null && mem.Index is null)
            {
                return Address.Ptr32((uint) mem.Displacement);
            }
            Expression ea;
            ea = Bind(mem.Base!);
            if (mem.Index is not null)
            {
                var idx = Bind(mem.Index);
                ea = m.IAdd(ea, idx);
            }
            if (mem.Displacement != 0)
            {
                ea = m.IAdd(ea, mem.Displacement);
            }
            return ea;
        }

        private Expression Bind(Storage stg)
        {
            if (stg is RegisterStorage reg)
                return binder.EnsureRegister(reg);
            else if (stg is SequenceStorage seq)
                return binder.EnsureSequence(seq.Width, seq.Elements);
            else
                throw new NotImplementedException();
        }

        private Expression MaybeSlice(int iop, PrimitiveType dt)
        {
            var exp = Operand(iop);
            if (exp.DataType.BitSize > dt.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(exp, dt));
                return tmp;
            }
            return exp;
        }

        private void RewriteAdd(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            var right = MaybeSlice(0, dt);
            Assign(Operand(1), m.IAdd(left, right));
            var flags = binder.EnsureFlagGroup(Registers.CF);
            m.Assign(flags, m.Cond(left));
        }

        private void RewriteAddc(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            var right = MaybeSlice(0, dt);
            var c = binder.EnsureFlagGroup(Registers.C);
            Assign(Operand(1), m.IAddC(left, right, c));
            var flags = binder.EnsureFlagGroup(Registers.CF);
            m.Assign(flags, m.Cond(left));
        }

        private void RewriteAddu(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            var right = MaybeSlice(0, dt);
            Assign(Operand(1), m.IAdd(left, right));
        }

        private void RewriteAnd(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            var right = MaybeSlice(0, dt);
            Assign(Operand(1), m.And(left, right));
        }

        private void RewriteAshu(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            if (instr.Operands[0] is ImmediateOperand cnt)
            {
                var value = cnt.Value.ToInt32();
                Expression src;
                if (value >= 0)
                {
                    src = m.Shl(left, value);
                }
                else
                {
                    src = m.Sar(left, -value);
                }
                Assign(Operand(1), src);
            }
            else
            {
                var sh = Operand(0);
                Assign(Operand(1), m.Fn(ashu_intrinsic.MakeInstance(dt, sh.DataType), left, sh));
            }
        }

        private void RewriteBal()
        {
            //$REVIEW: do we care about = ra?
            var target = Operand(1);
            m.Call(target, 0);
        }

        private void RewriteBcc(ConditionCode cc, FlagGroupStorage grf)
        {
            var id = binder.EnsureFlagGroup(grf);
            m.Branch(m.Test(cc, id), (Address)instr.Operands[0]);
        }

        private void RewriteBccClear(FlagGroupStorage grf)
        {
            m.Branch(m.Not(binder.EnsureFlagGroup(grf)), (Address)instr.Operands[0]);
        }

        private void RewriteBccSet(FlagGroupStorage grf)
        {
            m.Branch(binder.EnsureFlagGroup(grf), (Address) instr.Operands[0]);
        }

        private void RewriteBeq0(PrimitiveType dt)
        {
            m.Branch(m.Eq0(MaybeSlice(0, dt)), (Address) instr.Operands[1]);
        }

        private void RewriteBne0(PrimitiveType dt)
        {
            m.Branch(m.Ne0(MaybeSlice(0, dt)), (Address) instr.Operands[1]);
        }

        private void RewriteBr()
        {
            m.Goto(Operand(0));
        }

        private void RewriteCbit(PrimitiveType dt)
        {
            var cbit = CommonOps.ClearBit.MakeInstance(dt, PrimitiveType.Byte);
            var bit = Operand(0);
            var src = Operand(1);
            Assign(Operand(1), m.Fn(cbit, src, bit));
        }

        private void RewriteCinv()
        {
            var c = (CacheFlagOperand) instr.Operands[0];
            var sb = new StringBuilder();
            if (c.Flag.HasFlag(CacheFlag.I))
                sb.Append('i');
            if (c.Flag.HasFlag(CacheFlag.D))
                sb.Append('d');
            if (c.Flag.HasFlag(CacheFlag.U))
                sb.Append('u');
            var st = new StringType(PrimitiveType.Char, null, 0);
            m.SideEffect(m.Fn(
                cinv_instrinsic,
                new StringConstant(st, sb.ToString())));
        }

        private void RewriteCmp(PrimitiveType dt)
        {
            var right = MaybeSlice(0, dt);
            var left = MaybeSlice(1, dt);
            var dst = binder.EnsureFlagGroup(Registers.LNZ);
            m.Assign(dst, m.Cond(m.ISub(left, right)));
        }

        private void RewriteDi()
        {
            m.SideEffect(m.Fn(di_intrinsic));
        }

        private void RewriteEi()
        {
            m.SideEffect(m.Fn(ei_intrinsic));
        }


        private void RewriteEiwait()
        {
            m.SideEffect(m.Fn(eiwait_intrinsic));
        }

        private void RewriteExcp()
        {
            m.SideEffect(m.Fn(excp_intrinsic, Operand(0)));
        }

        private void RewriteJal()
        {
            //$REVIEW: continuation in op0 ignored.
            var target = Operand(1);
            m.Call(target, 0);
        }

        private void RewriteJcc(ConditionCode cc, FlagGroupStorage grf)
        {
            var id = binder.EnsureFlagGroup(grf);
            m.BranchInMiddleOfInstruction(
                m.Test(cc, id),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.Goto(Operand(0));
        }

        private void RewriteJccClear(FlagGroupStorage grf)
        {
            m.BranchInMiddleOfInstruction(
                binder.EnsureFlagGroup(grf),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.Goto(Operand(0));
        }

        private void RewriteJccSet(FlagGroupStorage grf)
        {
            m.BranchInMiddleOfInstruction(
                m.Not(binder.EnsureFlagGroup(grf)),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.Goto(Operand(0));
        }

        private void RewriteJr()
        {
            if (Registers.ra == instr.Operands[0])
            {
                m.Return(0, 0);
            }
            else
            {
                m.Goto(m.Shl(Operand(0), 1));
            }
        }

        private void RewriteLoad(PrimitiveType dt)
        {
            var src = Operand(0);
            var dst = Operand(1);
            Assign(dst, src);
        }

        private void RewriteLpr(PrimitiveType dt)
        {
            m.SideEffect(m.Fn(
                lpr_intrinsic.MakeInstance(dt),
                Operand(1),
                Operand(0)));
        }

        private void RewriteLsh(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            if (instr.Operands[0] is ImmediateOperand cnt)
            {
                var value = cnt.Value.ToInt32();
                if (value >= 0)
                {
                    Assign(Operand(1), m.Shl(left, value));
                }
                else
                {
                    Assign(Operand(1), m.Shr(left, -value));
                }
                return;
            }
            else
            {
                var sh = Operand(0);
                Assign(Operand(1), m.Fn(lsh_intrinsic.MakeInstance(dt, sh.DataType), left, sh));
            }
        }

        private void RewriteMov(PrimitiveType dt)
        {
            Expression src;
            if (instr.Operands[0] is ImmediateOperand imm)
            {
                src = Constant.Create(dt, imm.Value.ToUInt32());
            }
            else
            {
                src = MaybeSlice(0, dt);
            }
            Assign(Operand(1), src);
        }

        private void RewriteMovsx(PrimitiveType dtSrc)
        {
            var dtDst = PrimitiveType.Create(Domain.SignedInt, instr.Operands[1].Width.BitSize);
            var src = MaybeSlice(0, dtSrc);
            var dst = Operand(1);
            m.Assign(dst, m.Convert(src, dtSrc, dtDst));
        }

        private void RewriteMovzx(PrimitiveType dtSrc)
        {
            var dtDst = instr.Operands[1].Width;
            var src = MaybeSlice(0, dtSrc);
            var dst = Operand(1);
            m.Assign(dst, m.Convert(src, dtSrc, dtDst));
        }

        private void RewriteMul(PrimitiveType dt)
        {
            // MULi is signed according to manual.
            var left = MaybeSlice(1, dt);
            var right = MaybeSlice(0, dt);
            Assign(Operand(1), m.SMul(left, right));
        }

        private void RewriteMuls(PrimitiveType dt, PrimitiveType dtResult)
        {
            var left = SliceRp(instr.Operands[1], dt);
            var right = MaybeSlice(0, dt);
            Assign(Operand(1), m.SMul(dtResult, left, right));
        }

        private Expression SliceRp(MachineOperand op, PrimitiveType dt)
        {
            Expression e;
            if (op is SequenceStorage seq)
            {
                e = binder.EnsureIdentifier(seq.Elements[1]);
            }
            else if (op is RegisterStorage reg)
            {
                e = m.Slice(binder.EnsureRegister(reg), PrimitiveType.Word16);
            }
            else
            {
                EmitUnitTest();
                host.Warn(instr.Address, "NYI: {0}", instr);
                return Constant.Int32(0);
            }
            if (e.DataType.BitSize > dt.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(e, dt));
                e = tmp;
            }
            return e;
        }

        private void RewriteMulu(PrimitiveType dt)
        {
            var left = SliceRp(instr.Operands[1], dt);
            var right = Operand(0);
            Assign(Operand(1), m.SMul(PrimitiveType.UInt32, left, right));
        }

        private void RewriteOr(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            var right = MaybeSlice(0, dt);
            Assign(Operand(1), m.Or(left, right));
        }

        private void RewriteNop()
        {
            m.Nop();
        }

        private void RewritePop()
        {
            var sp = binder.EnsureRegister(arch.StackRegister);
            var ireg = (int) ((RegisterStorage) instr.Operands[1]).Domain;
            var count = ((ImmediateOperand) instr.Operands[0]).Value.ToInt32();
            for (int i = count-1; i >= 0; --i)
            {
                var reg = arch.GetRegister((StorageDomain) ((ireg + i) & 0xF), default)!;
                var id = binder.EnsureRegister(reg);
                m.Assign(id, m.Mem(reg.DataType, sp));
                m.Assign(sp, m.IAddS(sp, id.DataType.Size));
            }
        }

        private void RewritePopret()
        {
            RewritePop();
            m.Return(0, 0);
        }

        public void RewritePush()
        {
            var sp = binder.EnsureRegister(arch.StackRegister);
            var ireg = (int)((RegisterStorage)instr.Operands[1]).Domain;
            var count = ((ImmediateOperand) instr.Operands[0]).Value.ToInt32();
            for (int i = 0; i < count; ++i)
            {
                var reg = arch.GetRegister((StorageDomain) ((ireg + i) & 0xF), default)!;
                var id = binder.EnsureRegister(reg);
                m.Assign(sp, m.ISubS(sp, reg.DataType.Size));
                m.Assign(m.Mem(reg.DataType, sp), id);
            }
        }

        private void RewriteRetx()
        {
            m.SideEffect(m.Fn(retx_intrinsic));
            m.Return(0, 0);
        }

        private void RewriteSbit(PrimitiveType dt)
        {
            var sbit = CommonOps.SetBit.MakeInstance(dt, PrimitiveType.Byte);
            var bit = Operand(0);
            var src = Operand(1);
            Assign(Operand(1), m.Fn(sbit, src, bit));
        }

        private void RewriteScc(ConditionCode cc, FlagGroupStorage grf)
        {
            var id = binder.EnsureFlagGroup(grf);
            var dst = Operand(0);
            m.Assign(dst, m.Convert(
                m.Test(cc, id),
                PrimitiveType.Bool,
                PrimitiveType.Create(Domain.SignedInt, dst.DataType.BitSize)));
        }

        private void RewriteSccClear(FlagGroupStorage grf)
        {
            var id = binder.EnsureFlagGroup(grf);
            var dst = Operand(0);
            m.Assign(dst, m.Convert(
                m.Not(id),
                PrimitiveType.Bool,
                PrimitiveType.Create(Domain.SignedInt, dst.DataType.BitSize)));
        }

        private void RewriteSccSet(FlagGroupStorage grf)
        {
            var id = binder.EnsureFlagGroup(grf);
            var dst = Operand(0);
            m.Assign(dst, m.ExtendZ(m.Ne0(id), dst.DataType));
        }

        private void RewriteSpr(PrimitiveType dt)
        {
            Assign(
                Operand(1),
                m.Fn(
                    spr_intrinsic.MakeInstance(dt),
                    Operand(0)));
        }

        private void RewriteStore(PrimitiveType dt)
        {
            var src = MaybeSlice(0, dt);
            var dst = Operand(1);
            Assign(dst, src);
        }

        private void RewriteSub(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            var right = MaybeSlice(0, dt);
            Assign(Operand(1), m.ISub(left, right));
            var flags = binder.EnsureFlagGroup(Registers.CF);
            m.Assign(flags, m.Cond(left));
        }

        private void RewriteSubc(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            var right = MaybeSlice(0, dt);
            var c = binder.EnsureFlagGroup(Registers.C);
            Assign(Operand(1), m.ISubC(left, right, c));
            var flags = binder.EnsureFlagGroup(Registers.CF);
            m.Assign(flags, m.Cond(left));
        }

        private void RewriteTbit(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            var right = Operand(0);
            var dst = binder.EnsureFlagGroup(Registers.F);
            m.Assign(dst, m.Fn(CommonOps.Bit, left, right));
        }

        private void RewriteWait()
        {
            m.SideEffect(m.Fn(wait_intrinsic));
        }

        private void RewriteXor(PrimitiveType dt)
        {
            var left = MaybeSlice(1, dt);
            var right = MaybeSlice(0, dt);
            Assign(Operand(1), m.Xor(left, right));
        }

        private static readonly IntrinsicProcedure ashu_intrinsic = new IntrinsicBuilder("__a_shift", false)
            .GenericTypes("TValue", "TShift")
            .Param("TValue")
            .Param("TShift")
            .Returns("TValue");
        private static readonly IntrinsicProcedure cinv_instrinsic = new IntrinsicBuilder("__invalidate_cache", true)
            .Param(new StringType(PrimitiveType.Char, null, 0))
            .Void();
        private static readonly IntrinsicProcedure di_intrinsic = new IntrinsicBuilder("__di", true)
            .Void();
        private static readonly IntrinsicProcedure ei_intrinsic = new IntrinsicBuilder("__ei", true)
            .Void();
        private static readonly IntrinsicProcedure eiwait_intrinsic = new IntrinsicBuilder("__ei_wait", true)
            .Void();
        private static readonly IntrinsicProcedure excp_intrinsic = new IntrinsicBuilder("__raise_exception", true)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure lpr_intrinsic = new IntrinsicBuilder("__write_program_register", true)
            .GenericTypes("T")
            .Param(PrimitiveType.Word16)
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure lsh_intrinsic = new IntrinsicBuilder("__l_shift", false)
            .GenericTypes("TValue", "TShift")
            .Param("TValue")
            .Param("TShift")
            .Returns("TValue");
        private static readonly IntrinsicProcedure retx_intrinsic = new IntrinsicBuilder("__return_from_exception", true)
            .Void();
        private static readonly IntrinsicProcedure spr_intrinsic = new IntrinsicBuilder("__read_program_register", true)
            .GenericTypes("T")
            .Param(PrimitiveType.Word16)
            .Returns("T");
        private static readonly IntrinsicProcedure wait_intrinsic = new IntrinsicBuilder("__wait", true)
            .Void();
    }
}
