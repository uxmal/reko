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
using Reko.Core.Pascal;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
                case Mnemonic.callr: RewriteCallr(instr); break;
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
                    m.SideEffect(RewriteIntrinsic(instr.Mnemonic.ToString(), VoidType.Instance)); break;
                }
            }
        }


        private Expression OperandSrc(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rop: return binder.EnsureRegister(rop.Register);
            case ImmediateOperand imm: return imm.Value;
            case AddressOperand addr: return addr.Address;
            case ApplicationOperand app: return RewriteApplication(app);
            case MemoryOperand mem: var src =  RewriteMemoryOperand(mem); MaybeEmitIncrement(mem); return src;
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
                return exp;
            }
            throw new NotImplementedException();
        }

        private void OperandDst(MachineOperand op, Action<Expression,Expression> write, Expression src)
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
                m.SideEffect(host.PseudoProcedure(app.Mnemonic.ToString(), VoidType.Instance, appOps));
                return;
            }
            throw new NotImplementedException($"Hexagon rewriter for {op.GetType().Name} not implemented yet.");
        }

        private Expression RewriteApplication(ApplicationOperand app)
        {
            var ops = app.Operands.Select(OperandSrc).ToArray();
            switch (app.Mnemonic)
            {
            case Mnemonic.add: return m.IAdd(ops[0], ops[1]);
            case Mnemonic.addasl: return RewriteAddAsl(ops[0], ops[1], ops[2]);
            case Mnemonic.allocframe: RewriteAllocFrame(app.Operands[0]); return null;
            case Mnemonic.and: return m.And(ops[0], ops[1]);
            case Mnemonic.asl: return m.Shl(ops[0], ops[1]);
            case Mnemonic.asr: return m.Sar(ops[0], ops[1]);
            case Mnemonic.cmp__eq: return m.Eq(ops[0], ops[1]);
            case Mnemonic.cmp__gt: return m.Gt(ops[0], ops[1]);
            case Mnemonic.cmp__gtu: return m.Ugt(ops[0], ops[1]);
            case Mnemonic.cmph__eq: return RewriteCmph(m.Eq, ops[0], ops[1]);
            case Mnemonic.cmph__gtu: return RewriteCmph(m.Ugt, ops[0], ops[1]);
            case Mnemonic.combine: return RewriteCombine(ops[0], ops[1]);
            case Mnemonic.extractu: return RewriteExtract(Domain.UnsignedInt, ops[0], app.Operands);
            case Mnemonic.loop0: RewriteLoop(0, ops); return null;
            case Mnemonic.loop1: RewriteLoop(1, ops); return null;
            case Mnemonic.lsr: return m.Shr(ops[0], ops[1]);
            case Mnemonic.mux: return m.Conditional(ops[1].DataType, ops[0], ops[1], ops[2]);
            case Mnemonic.not: return m.Not(ops[0]);
            case Mnemonic.or: return m.And(ops[0], ops[1]);
            case Mnemonic.sub: return m.ISub(ops[0], ops[1]);
            case Mnemonic.xor: return m.And(ops[0], ops[1]);
            case Mnemonic.clrbit:
            case Mnemonic.crswap:
            case Mnemonic.cswi:
            case Mnemonic.dccleana:
            case Mnemonic.dccleaninva:
            case Mnemonic.dfclass:
            case Mnemonic.insert:   //$BUG: like DPB?
            case Mnemonic.max:
            case Mnemonic.memw_locked:
            case Mnemonic.min:
            case Mnemonic.setbit:
            case Mnemonic.stop:
            case Mnemonic.tlbw:
            case Mnemonic.tlbp:
            case Mnemonic.trap0:
            case Mnemonic.trap1:
            case Mnemonic.tstbit:
                return RewriteIntrinsic(app.Mnemonic.ToString(), app.Width, ops); 
            }
            throw new ArgumentException($"Hexagon rewriter for {app.Mnemonic} not implemented yet.", app.Mnemonic.ToString());
        }

        private Expression RewriteIntrinsic(string name, DataType dt, params Expression[] ops)
        {
            return host.PseudoProcedure(name, dt, ops);
        }

        private Expression RewriteMemoryOperand(MemoryOperand mem)
        {
            Expression ea;
            if (mem.Base != null)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.Index != null)
                    throw new NotImplementedException("Index");
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
                var reg = binder.EnsureRegister(mem.Base);
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
                pred = m.Not(pred);
            return pred;
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
            var src = OperandSrc(opSrc);
            OperandDst(opDst, (l, r) => { m.Assign(l, r); }, src);
        }

        private void RewriteAugmentedAssign(MachineOperand opDst, Func<Expression, Expression,Expression> fn, MachineOperand opSrc)
        {
            var src = OperandSrc(opSrc);
            OperandDst(opDst, (l, r) => { m.Assign(l, fn(l, r)); }, src);
        }


        private void RewriteCallr(HexagonInstruction instr)
        {
            if (instr.ConditionPredicate != null)
            {
                var pred = RewritePredicateExpression(instr.ConditionPredicate, !instr.ConditionInverted);
                m.BranchInMiddleOfInstruction(pred, instr.Address + 4, InstrClass.ConditionalTransfer);
            }
            m.Call(OperandSrc(instr.Operands[0]), 0);
        }

        private Expression RewriteCmph(Func<Expression,Expression,Expression> cmp, Expression a, Expression b)
        {
            return cmp(m.Slice(PrimitiveType.Word16, a, 0), m.Slice(PrimitiveType.Word16, b, 0));
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


        private Expression RewriteExtract(Domain domain, Expression expression, MachineOperand[] operands)
        {
            var dt = PrimitiveType.Create(domain, operands[0].Width.BitSize);
            if (operands[1] is RegisterPairOperand pair)
            {
                throw new NotImplementedException();
                var offset = binder.EnsureRegister(pair.LowRegister);
                var width = binder.EnsureRegister(pair.HighRegister);
            }
            else
            {
                var width = ((ImmediateOperand) operands[2]).Value.ToInt32();
                var offset = ((ImmediateOperand) operands[1]).Value.ToInt32();
                var dtSlice = PrimitiveType.CreateBitSlice(width);
                var slice = m.Slice(dtSlice, expression, offset);
                return m.Cast(dt, slice);
            }
        }

        private void RewriteJump(HexagonInstruction instr, MachineOperand opDst)
        {
            var aop = (AddressOperand) opDst;
            if (instr.ConditionPredicate != null)
            {
                //if (instr.ConditionPredicateNew)
                //    throw new NotImplementedException(".NEW");
                var cond = OperandSrc(instr.ConditionPredicate);
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
            m.SideEffect(host.PseudoProcedure($"__nyi_loop{n}", VoidType.Instance, args));
        }

        private void RewriteRte()
        {
            m.SideEffect(host.PseudoProcedure("rte", VoidType.Instance));
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
    }
}