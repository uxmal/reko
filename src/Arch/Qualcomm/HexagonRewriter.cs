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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Reko.Arch.Qualcomm
{
    public class HexagonRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly HexagonArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<HexagonPacket> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly HashSet<RegisterStorage> registersRead;
        private readonly List<RtlAssignment> deferredWrites;
        private readonly List<RtlTransfer> deferredTransfers;
        private readonly RtlEmitter m;
        private InstrClass iclass;

        public HexagonRewriter(HexagonArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new HexagonDisassembler(arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.registersRead = new HashSet<RegisterStorage>();
            this.deferredWrites = new List<RtlAssignment>();
            this.deferredTransfers = new List<RtlTransfer>();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instrs.Clear();
                registersRead.Clear();
                deferredWrites.Clear();
                var packet = dasm.Current;
                try
                {
                    ProcessPacket(packet);
                }
                catch (ArgumentException ex)
                {
                    EmitUnitTest(packet, ex.ParamName);
                    host.Error(packet.Address, "{0}", ex.Message);
                }
                catch (Exception ex)
                {
                    EmitUnitTest(packet, "");
                    host.Error(packet.Address, "{0}", ex.Message);
                }
                yield return m.MakeCluster(packet.Address, packet.Length, packet.InstructionClass);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest(HexagonPacket packet, string mnemonic)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("HexagonRw", packet, mnemonic, rdr, "");
        }

        private void ProcessPacket(HexagonPacket packet)
        {
            foreach (var instr in packet.Instructions)
            {
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    host.Error(
                        instr.Address,
                        string.Format("Hexagon instruction '{0}' is not supported yet.", instr));
                    EmitUnitTest(packet, instr.Mnemonic.ToString());
                    break;
                case Mnemonic.ASSIGN: RewriteAssign(instr.Operands[0], instr.Operands[1]); break;
                case Mnemonic.ADDEQ: RewriteAugmentedAssign(instr.Operands[0], m.IAdd, instr.Operands[1]); break;
                case Mnemonic.ANDEQ: RewriteAugmentedAssign(instr.Operands[0], m.And, instr.Operands[1]); break;
                case Mnemonic.OREQ: RewriteAugmentedAssign(instr.Operands[0], m.Or, instr.Operands[1]); break;
                case Mnemonic.SUBEQ: RewriteAugmentedAssign(instr.Operands[0], m.ISub, instr.Operands[1]); break;
                case Mnemonic.XOREQ: RewriteAugmentedAssign(instr.Operands[0], m.Xor, instr.Operands[1]); break;
                case Mnemonic.SIDEEFFECT: RewriteSideEffect(instr.InstructionClass, instr.Operands[0]); break;
                case Mnemonic.call: RewriteCall(instr); break;
                case Mnemonic.callr: RewriteCall(instr); break;
                case Mnemonic.deallocframe: RewriteDeallocFrame(); break;
                case Mnemonic.dealloc_return: RewriteDeallocReturn(); break;
                case Mnemonic.jump: RewriteJump(instr, instr.Operands[0]); break;
                case Mnemonic.jumpr: RewriteJumpr(instr.Operands[0]); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.rte: RewriteRte(); break;
                case Mnemonic.brkpt:
                case Mnemonic.dckill:
                case Mnemonic.ickill:
                case Mnemonic.isync:
                case Mnemonic.l2kill:
                case Mnemonic.syncht:
                    m.SideEffect(RewriteIntrinsic(instr.Mnemonic.ToString(), true, VoidType.Instance)); break;
                }
                m.Instructions.Last().Class = this.iclass;
            }
        }


        private Expression? OperandSrc(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rop: return binder.EnsureRegister(rop.Register);
            case ImmediateOperand imm: return imm.Value;
            case AddressOperand addr: return addr.Address;
            case ApplicationOperand app: return RewriteApplication(app);
            case MemoryOperand mem: var src = RewriteMemoryOperand(mem); MaybeEmitIncrement(mem); return src;
            case RegisterPairOperand pair: return binder.EnsureSequence(PrimitiveType.Word64, pair.HighRegister, pair.LowRegister);
            case DecoratorOperand dec: return RewriteDecorator(dec);
            }
            throw new NotImplementedException($"Hexagon rewriter for {op.GetType().Name} not implemented yet.");
        }

        private Expression RewriteDecorator(DecoratorOperand dec)
        {
            var exp = OperandSrc(dec.Operand);
            if (dec.NewValue)
            {
                //$TODO;
                return exp!;
            }
            if (dec.Inverted)
            {
                exp = m.Not(exp!);
                return exp;
            }
            if (dec.Carry)
            {
                var app = (ApplicationOperand) dec.Operand;
                var p = binder.EnsureRegister(((RegisterOperand)app.Operands[2]).Register);
                exp = m.IAdd(exp!, p);
                //$TODO: what about carry-out? it's in p.
                return exp;
            }
            if (dec.Width.BitSize < (int) dec.Operand.Width.BitSize)
            {
                exp = m.Slice(dec.Width, exp!, dec.BitOffset);
                return exp;
            }
            throw new NotImplementedException(dec.ToString());
        }

        private void OperandDst(MachineOperand op, Action<Expression, Expression> write, Expression src)
        {
            switch (op)
            {
            case RegisterOperand rop:
                write(binder.EnsureRegister(rop.Register), src);
                return;
            case MemoryOperand mem:
                write(RewriteMemoryOperand(mem), src);
                MaybeEmitIncrement(mem);
                return;
            case RegisterPairOperand pair:
                write(binder.EnsureSequence(PrimitiveType.Word64, pair.HighRegister, pair.LowRegister), src);
                return;
            case DecoratorOperand dec:
                if (dec.Width.BitSize < dec.Operand.Width.BitSize)
                {
                    var dst = binder.EnsureRegister(((RegisterOperand) dec.Operand).Register);
                    var dt = PrimitiveType.CreateWord(32 - dec.Width.BitSize);
                    if (dec.BitOffset == 0)
                    {
                        var hi = m.Slice(dt, dst, dec.Width.BitSize);
                        write(dst, m.Seq(hi, src));
                    }
                    else
                    {
                        var lo = m.Slice(dt, dst, 0);
                        write(dst, m.Seq(src, lo));
                    }
                    return;
                }
                break;
            case ApplicationOperand app:
                var appOps = app.Operands.Select(OperandSrc).Concat(new[] { src }).ToArray();
                m.SideEffect(host.Intrinsic(app.Mnemonic.ToString(), true, VoidType.Instance, appOps!));
                return;
            }
            throw new NotImplementedException($"Hexagon rewriter for {op.GetType().Name} not implemented yet.");
        }

        private Expression? RewriteApplication(ApplicationOperand app)
        {
            var ops = app.Operands.Select(o => OperandSrc(o)!).ToArray();
            var dt = app.Width;
            switch (app.Mnemonic)
            {
            case Mnemonic.add: return m.IAdd(ops[0], ops[1]);
            case Mnemonic.addasl: return RewriteAddAsl(ops[0], ops[1], ops[2]);
            case Mnemonic.allocframe: RewriteAllocFrame(app.Operands[0]); return null;
            case Mnemonic.and: return m.And(ops[0], ops[1]);
            case Mnemonic.asl: return m.Shl(ops[0], ops[1]);
            case Mnemonic.aslh: return m.Shl(ops[0], 16);
            case Mnemonic.asr: return m.Sar(ops[0], ops[1]);
            case Mnemonic.asrh: return m.Sar(ops[0], 16);
            case Mnemonic.cmp__eq: return m.Eq(ops[0], ops[1]);
            case Mnemonic.cmp__gt: return m.Gt(ops[0], ops[1]);
            case Mnemonic.cmp__gtu: return m.Ugt(ops[0], ops[1]);
            case Mnemonic.cmpb__eq: return RewriteCmp(PrimitiveType.Byte, m.Eq, ops[0], ops[1]);
            case Mnemonic.cmpb__gt: return RewriteCmp(PrimitiveType.Byte, m.Gt, ops[0], ops[1]);
            case Mnemonic.cmpb__gtu: return RewriteCmp(PrimitiveType.Byte, m.Ugt, ops[0], ops[1]);
            case Mnemonic.cmph__eq: return RewriteCmp(PrimitiveType.Word16, m.Eq, ops[0], ops[1]);
            case Mnemonic.cmph__gt: return RewriteCmp(PrimitiveType.Word16, m.Gt, ops[0], ops[1]);
            case Mnemonic.cmph__gtu: return RewriteCmp(PrimitiveType.Word16, m.Ugt, ops[0], ops[1]);
            case Mnemonic.dfcmp__eq: return m.FEq(ops[0], ops[1]);
            case Mnemonic.dfcmp__ge: return m.FGe(ops[0], ops[1]);
            case Mnemonic.dfcmp__uo: return host.Intrinsic("isunordered", false, PrimitiveType.Bool, ops[0], ops[1]);
            case Mnemonic.combine: return RewriteCombine(ops[0], ops[1]);
            case Mnemonic.convert_d2df: return m.Convert(ops[0], PrimitiveType.Int64, PrimitiveType.Real64);
            case Mnemonic.convert_df2sf: return m.Convert(ops[0], PrimitiveType.Real64, PrimitiveType.Real32);
            case Mnemonic.convert_sf2df: return m.Convert(ops[0], PrimitiveType.Real32, PrimitiveType.Real64);
            case Mnemonic.EQ: return m.Eq(ops[0], ops[1]);
            case Mnemonic.extract: return RewriteExtract(Domain.SignedInt, ops[0], app.Operands);
            case Mnemonic.extractu: return RewriteExtract(Domain.UnsignedInt, ops[0], app.Operands);
            case Mnemonic.loop0: RewriteLoop(0, ops); return null;
            case Mnemonic.loop1: RewriteLoop(1, ops); return null;
            case Mnemonic.lsl: return m.Shl(ops[0], ops[1]);
            case Mnemonic.lsr: return m.Shr(ops[0], ops[1]);
            case Mnemonic.mpy: return RewriteMpy(ops[0], ops[1]);
            case Mnemonic.mpyi: return RewriteMpyi(ops[0], ops[1]);
            case Mnemonic.mpyu: return RewriteMpyu(app.Width, ops[0], ops[1]);
            case Mnemonic.mux: return m.Conditional(ops[1].DataType, ops[0], ops[1], ops[2]);
            case Mnemonic.NE: return m.Ne(ops[0], ops[1]);
            case Mnemonic.neg: return m.Neg(ops[0]);
            case Mnemonic.not: return m.Not(ops[0]);
            case Mnemonic.or: return m.Or(ops[0], ops[1]);
            case Mnemonic.sub: return m.ISub(ops[0], ops[1]);
            case Mnemonic.sxtb: return RewriteExt(ops[0], PrimitiveType.SByte, PrimitiveType.Int32);
            case Mnemonic.sxth: return RewriteExt(ops[0], PrimitiveType.Int16, PrimitiveType.Int32);
            case Mnemonic.xor: return m.Xor(ops[0], ops[1]);
            case Mnemonic.zxtb: return RewriteExt(ops[0], PrimitiveType.Byte, PrimitiveType.UInt32);
            case Mnemonic.zxth: return RewriteExt(ops[0], PrimitiveType.UInt16, PrimitiveType.UInt32);

            case Mnemonic.abs: dt = PrimitiveType.Int32; goto intrinsicFunc;
            case Mnemonic.max: dt = PrimitiveType.Int32; goto intrinsicFunc;
            case Mnemonic.maxu: dt = PrimitiveType.UInt32; goto intrinsicFunc;
            case Mnemonic.min: dt = PrimitiveType.Int32; goto intrinsicFunc;
            case Mnemonic.minu: dt = PrimitiveType.UInt32; goto intrinsicFunc;

            case Mnemonic.all8:
            case Mnemonic.any8:
            case Mnemonic.bitsclr:
            case Mnemonic.bitsplit:
            case Mnemonic.bitsset:
            case Mnemonic.ciad:
            case Mnemonic.cl0:
            case Mnemonic.cl1:
            case Mnemonic.clb:
            case Mnemonic.clrbit:
            case Mnemonic.crswap:
            case Mnemonic.cswi:
            case Mnemonic.ct0:
            case Mnemonic.dfclass:
            case Mnemonic.fastcorner9:
            case Mnemonic.insert:   //$BUG: like DPB?
            case Mnemonic.memw_locked:
            case Mnemonic.setbit:
            case Mnemonic.start:
            case Mnemonic.stop:
            case Mnemonic.tlbw:
            case Mnemonic.tlbp:
            case Mnemonic.trap0:
            case Mnemonic.trap1:
            case Mnemonic.togglebit:
            case Mnemonic.tstbit:
                intrinsicFunc:
                return RewriteIntrinsic(app.Mnemonic.ToString(), true, dt, ops);

            case Mnemonic.dccleana:
            case Mnemonic.dccleaninva:
            case Mnemonic.dcfetch:
            case Mnemonic.dcinva:
            case Mnemonic.dczeroa:
                return RewriteIntrinsic(app.Mnemonic.ToString(), true, VoidType.Instance, ops);

            case Mnemonic.vavgh: return RewriteVectorIntrinsic(app.Mnemonic, false, PrimitiveType.Word16, ops);
            case Mnemonic.vcmpb__eq: return RewriteVectorIntrinsic(app.Mnemonic, false, PrimitiveType.Byte, ops);
            case Mnemonic.vmux: return RewriteVectorIntrinsic(app.Mnemonic, false, PrimitiveType.Byte, ops);
            case Mnemonic.vsplatb: return RewriteVectorIntrinsic(app.Mnemonic, false, PrimitiveType.Byte, ops);
            case Mnemonic.vsubh: return RewriteVectorIntrinsic(app.Mnemonic, false, PrimitiveType.Int16, ops);
            }
            throw new ArgumentException($"Hexagon rewriter for {app.Mnemonic} not implemented yet.", app.Mnemonic.ToString());
        }

        private Expression RewriteIntrinsic(string name, bool hasSideEffect, DataType dt, params Expression[] ops)
        {
            return host.Intrinsic(name, hasSideEffect, dt, ops);
        }

        private Expression RewriteMemoryOperand(MemoryOperand mem)
        {
            Expression ea;
            if (mem.Base != null)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.Index != null)
                {
                    Expression index = binder.EnsureRegister(mem.Index);
                    if (mem.Shift > 0)
                    {
                        index = m.IMul(index, 1 << mem.Shift);
                    }
                    ea = m.IAdd(ea, index);
                }
                if (mem.Offset != 0)
                {
                    ea = m.AddSubSignedInt(ea, mem.Offset);
                }
            }
            else
            {
                ea = Address.Ptr32((uint) mem.Offset);
                if (mem.Index != null)
                {
                    var idx = binder.EnsureRegister(mem.Index);
                    ea = m.IAdd(ea, idx);
                }
            }
            return m.Mem(mem.Width, ea);
        }


        private void MaybeEmitIncrement(MemoryOperand mem)
        {
            if (mem.AutoIncrement != null)
            {
                var reg = binder.EnsureRegister(mem.Base!);
                if (mem.AutoIncrement is int incr)
                    m.Assign(reg, m.AddSubSignedInt(reg, incr));
                else
                    m.Assign(reg, m.IAdd(reg, (Expression) mem.AutoIncrement));
            }
        }

        private Expression RewritePredicateExpression(MachineOperand conditionPredicate, bool invert)
        {
            var pred = OperandSrc(conditionPredicate);
            if (invert)
                pred = m.Not(pred!);
            return pred!;
        }


        private Expression RewriteAddAsl(Expression a, Expression b, Expression c)
        {
            return m.IAdd(a, m.Shl(b, c));
        }

        private void RewriteAllocFrame(MachineOperand opImm)
        {
            var ea = binder.CreateTemporary(PrimitiveType.Ptr32);
            m.Assign(ea, m.ISubS(binder.EnsureRegister(Registers.sp), 8));
            m.Assign(m.Mem32(ea), binder.EnsureRegister(Registers.fp));
            m.Assign(m.Mem32(m.IAddS(ea, 4)), binder.EnsureRegister(Registers.lr));
            m.Assign(binder.EnsureRegister(Registers.fp), ea);
            m.Assign(binder.EnsureRegister(Registers.sp), m.ISubS(ea, ((ImmediateOperand) opImm).Value.ToInt32()));
        }

        private void RewriteAssign(MachineOperand opDst, MachineOperand opSrc)
        {
            var src = OperandSrc(opSrc)!;
            OperandDst(opDst, (l, r) => { m.Assign(l, r); }, src);
            //$TODO: conditions.
        }

        private void RewriteAugmentedAssign(MachineOperand opDst, Func<Expression, Expression,Expression> fn, MachineOperand opSrc)
        {
            var src = OperandSrc(opSrc)!;
            OperandDst(opDst, (l, r) => { m.Assign(l, fn(l, r)); }, src);
        }


        private void RewriteCall(HexagonInstruction instr)
        {
            if (instr.ConditionPredicate != null)
            {
                var pred = RewritePredicateExpression(instr.ConditionPredicate, !instr.ConditionInverted);
                m.BranchInMiddleOfInstruction(pred, instr.Address + 4, InstrClass.ConditionalTransfer);
            }
            m.Call(OperandSrc(instr.Operands[0])!, 0);
        }

        private Expression RewriteCmp(DataType dt, Func<Expression,Expression,Expression> cmp, Expression a, Expression b)
        {
            return cmp(m.Slice(dt, a, 0), m.Slice(dt, b, 0));
        }

        private Expression RewriteCombine(Expression hi, Expression lo)
        {
            var cbLo = lo.DataType.BitSize;
            var dt = PrimitiveType.CreateWord(hi.DataType.BitSize + cbLo);
            if (hi is Constant cHi && lo is Constant cLo)
            {
                var c = Constant.Create(dt, (cHi.ToUInt64() << cbLo) | cLo.ToUInt64());
                return c;
            }
            if (hi is Identifier idHi && lo is Identifier idLo)
            {
                var idSeq = binder.EnsureSequence(dt, idHi.Storage, idLo.Storage);
                return idSeq;
            }
            return m.Seq(hi, lo);
        }

        private void RewriteDeallocFrame()
        {
            var ea = binder.CreateTemporary(PrimitiveType.Ptr32);
            m.Assign(ea, binder.EnsureRegister(Registers.fp));
            m.Assign(binder.EnsureRegister(Registers.lr), m.Mem32(m.IAddS(ea, 4)));
            m.Assign(binder.EnsureRegister(Registers.fp), m.Mem32(ea));
            m.Assign(binder.EnsureRegister(Registers.sp), m.IAddS(ea, 8));
        }

        private void RewriteDeallocReturn()
        {
            var ea = binder.CreateTemporary(PrimitiveType.Ptr32);
            m.Assign(ea, binder.EnsureRegister(Registers.fp));
            m.Assign(binder.EnsureRegister(Registers.lr), m.Mem32(m.IAddS(ea, 4)));
            m.Assign(binder.EnsureRegister(Registers.fp), m.Mem32(ea));
            m.Assign(binder.EnsureRegister(Registers.sp), m.IAddS(ea, 8));
            m.Return(0, 0);
        }

        private Expression RewriteExt(Expression e, PrimitiveType dtSlice, PrimitiveType dtResult)
        {
            return m.Convert(m.Slice(dtSlice, e, 0), dtSlice, dtResult);
        }

        private Expression RewriteExtract(Domain domain, Expression expression, MachineOperand[] operands)
        {
            var dt = PrimitiveType.Create(domain, operands[0].Width.BitSize);
            if (operands[1] is RegisterPairOperand pair)
            {
                var offset = binder.EnsureRegister(pair.LowRegister);
                var width = binder.EnsureRegister(pair.HighRegister);
                return m.And(
                    m.Shl(expression, offset),
                    m.ISub(m.Shl(Constant.UInt64(1), width), 1));
            }
            else
            {
                var width = ((ImmediateOperand) operands[2]).Value.ToInt32();
                var offset = ((ImmediateOperand) operands[1]).Value.ToInt32();
                var dtSlice = PrimitiveType.CreateBitSlice(width);
                var slice = m.Slice(dtSlice, expression, offset);
                return m.Convert(slice, dtSlice, dt);
            }
        }

        private void RewriteJump(HexagonInstruction instr, MachineOperand opDst)
        {
            var aop = (AddressOperand) opDst;
            if (instr.ConditionPredicate != null)
            {
                //if (instr.ConditionPredicateNew)
                //    throw new NotImplementedException(".NEW");
                var cond = OperandSrc(instr.ConditionPredicate)!;
                m.Branch(cond, aop.Address);
            }
            else
            {
                m.Goto(aop.Address);
            }
        }

        private void RewriteJumpr(MachineOperand opDst)
        {
            var rop = (RegisterOperand) opDst;
            if (rop.Register == Registers.lr)
            {
                this.iclass = InstrClass.Transfer | InstrClass.Return;
                m.Return(0, 0);
            }
            else
            {
                var dst = binder.EnsureRegister(rop.Register);
                m.Goto(dst);
            }
        }

        private void RewriteLoop(int n, Expression [] args)
        {
            m.SideEffect(host.Intrinsic($"__nyi_loop{n}", true, VoidType.Instance, args));
        }

        private Expression RewriteMpy(Expression a, Expression b)
        {
            var mul = m.IMul(a, b);
            mul.DataType = PrimitiveType.Word64;
            return m.Slice(a.DataType, mul, 32);
        }

        private Expression RewriteMpyi(Expression a, Expression b)
        {
            return m.IMul(a, b);
        }

        private Expression RewriteMpyu(DataType dtResult, Expression a, Expression b)
        {
            if (dtResult.BitSize == 64)
            {
                var umul = m.UMul(a, b);
                umul.DataType = PrimitiveType.Word64;
                return umul;
            }
            else
            {
                var mul = m.IMul(a, b);
                mul.DataType = PrimitiveType.Word64;
                mul = m.Slice(a.DataType, mul, 32);
                return mul;
            }
        }
        private void RewriteRte()
        {
            m.SideEffect(host.Intrinsic("rte", true, VoidType.Instance));
            m.Return(0, 0);
        }

        private void RewriteSideEffect(InstrClass iclass, MachineOperand op)
        {
            var app = (ApplicationOperand) op;
            var e = RewriteApplication(app);
            if (e != null)
            {
                m.SideEffect(e);
            }
        }
 
        private Expression RewriteVectorIntrinsic(Mnemonic mnemonic, bool hasSideEffect, PrimitiveType dtElem, Expression[] ops)
        {
            var cElem = ops[0].DataType.Size / dtElem.Size;
            var dtVector = new ArrayType(dtElem, cElem);
            return host.Intrinsic(mnemonic.ToString(), hasSideEffect, dtVector, ops);
        }

    }
}