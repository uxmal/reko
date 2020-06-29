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
                catch (Exception ex)
                {
                    EmitUnitTest(packet);
                    host.Error(packet.Address, "Error: {0}", ex.Message);
                }
                yield return m.MakeCluster(packet.Address, packet.Length, packet.InstructionClass);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest(HexagonPacket packet)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("HexagonRw", packet, rdr, "");
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
                    EmitUnitTest(packet);
                    break;
                case Mnemonic.ASSIGN: RewriteAssign(instr.Operands[0], instr.Operands[1]); break;
                case Mnemonic.SIDEEFFECT: RewriteSideEffect(instr.Operands[0]); break;
                case Mnemonic.dealloc_return: RewriteDeallocReturn(); break;
                case Mnemonic.jump: RewriteJump(instr, instr.Operands[0]); break;
                case Mnemonic.jumpr: RewriteJumpr(instr.Operands[0]); break;
                case Mnemonic.nop: m.Nop(); break;
                }
            }
        }

        private Expression OperandSrc(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rop: return binder.EnsureRegister(rop.Register);
            case ImmediateOperand imm: return imm.Value;
            case ApplicationOperand app: return RewriteApplication(app);
            case MemoryOperand mem: return RewriteMemoryOperand(mem);
            case RegisterPairOperand pair: return binder.EnsureSequence(PrimitiveType.Word64, pair.HighRegister, pair.LowRegister);
            }
            throw new NotImplementedException($"Hexagon rewriter for {op.GetType().Name} not implemented yet.");
        }


        private void OperandDst(MachineOperand op, Expression src)
        {
            switch (op)
            {
            case RegisterOperand rop:
                m.Assign(binder.EnsureRegister(rop.Register), src);
                return;
            case MemoryOperand mem:
                m.Assign(RewriteMemoryOperand(mem), src);
                return;
            case RegisterPairOperand pair:
                m.Assign(binder.EnsureSequence(PrimitiveType.Word64, pair.HighRegister, pair.LowRegister), src);
                return;
            case DecoratorOperand dec:
                if (dec.Width.BitSize < dec.Operand.Width.BitSize)
                {
                    var dst = binder.EnsureRegister(((RegisterOperand) dec.Operand).Register);
                    var dt = PrimitiveType.CreateWord(32 - dec.Width.BitSize);
                    if (dec.BitOffset == 0)
                    {
                        var hi = m.Slice(dt, dst, dec.Width.BitSize);
                        m.Assign(dst, m.Seq(hi, src));
                    }
                    else
                    {
                        var lo = m.Slice(dt, dst, 0);
                        m.Assign(dst, m.Seq(src, lo));
                    }
                    return;
                }
                break;
            }
            throw new NotImplementedException($"Hexagon rewriter for {op.GetType().Name} not implemented yet.");
        }

        private Expression RewriteApplication(ApplicationOperand app)
        {
            var ops = app.Operands.Select(OperandSrc).ToArray();
            switch (app.Mnemonic)
            {
            case Mnemonic.add: return m.IAdd(ops[0], ops[1]);
            case Mnemonic.and: return m.And(ops[0], ops[1]);
            case Mnemonic.asl: return m.Shl(ops[0], ops[1]);
            case Mnemonic.asr: return m.Sar(ops[0], ops[1]);
            case Mnemonic.cmp__eq: return m.Eq(ops[0], ops[1]);
            case Mnemonic.cmp__gt: return m.Gt(ops[0], ops[1]);
            case Mnemonic.cmp__gtu: return m.Ugt(ops[0], ops[1]);
            case Mnemonic.combine: return RewriteCombine(ops[0], ops[1]);
            case Mnemonic.lsr: return m.Shr(ops[0], ops[1]);
            case Mnemonic.dckill:
            case Mnemonic.dfclass:
            case Mnemonic.insert:   //$BUG: like DPB
            case Mnemonic.memw_locked:
                return RewriteIntrinsic(app.Mnemonic.ToString(), app.Width, ops); 
            }
            throw new NotImplementedException($"Hexagon rewriter for {app.Mnemonic} not implemented yet.");
        }

        private Expression RewriteIntrinsic(string name, DataType dt, Expression[] ops)
        {
            return host.PseudoProcedure(name, dt, ops);
        }

        private Expression RewriteMemoryOperand(MemoryOperand mem)
        {
            Expression ea;
            if (mem.Base != null)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.AutoIncrement != null)
                    throw new NotImplementedException("AutoINcr");
                if (mem.Index != null)
                    throw new NotImplementedException("Index");
                if (mem.Offset != 0)
                {
                    ea = m.AddSubSignedInt(ea, mem.Offset);
                }
            }
            else
            {
                throw new NotImplementedException(mem.ToString());
            }
            return m.Mem(mem.Width, ea);
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
            OperandDst(opDst, src);
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

        private void RewriteDeallocReturn()
        {
            var ea = binder.CreateTemporary(PrimitiveType.Ptr32);
            m.Assign(ea, binder.EnsureRegister(Registers.fp));
            m.Assign(binder.EnsureRegister(Registers.lr), m.Mem32(m.IAddS(ea, 4)));
            m.Assign(binder.EnsureRegister(Registers.fp), m.Mem32(ea));
            m.Assign(binder.EnsureRegister(Registers.sp), m.IAddS(ea, 8));
            m.Return(0, 0);
        }


        private void RewriteJump(HexagonInstruction instr, MachineOperand opDst)
        {
            var aop = (AddressOperand) opDst;
            if (instr.ConditionPredicate != null)
            {
                if (instr.ConditionPredicateNew)
                    throw new NotImplementedException(".NEW");
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

        private void RewriteSideEffect(MachineOperand op)
        {
            var app = (ApplicationOperand) op;
            var ops = app.Operands.Select(OperandSrc).ToArray();
            switch (app.Mnemonic)
            {
            default:
                throw new NotImplementedException($"Hexagon rewriter for {app.Mnemonic} not implemented yet.");
            case Mnemonic.allocframe: RewriteAllocFrame(app.Operands[0]); break;
            case Mnemonic.crswap:
                m.SideEffect(host.PseudoProcedure(app.Mnemonic.ToString(), VoidType.Instance, ops));
                break;
            }
        }
    }
}