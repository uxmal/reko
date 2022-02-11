#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    public class ArgumentGuesser
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(ArgumentGuesser), "Trace ArgumentGuesser")
        {
            Level = TraceLevel.Info,
        };

        private readonly IPlatform platform;
        private readonly SsaState ssa;
        private readonly Storage stackPointer;
        private readonly Storage framePointer;
        private readonly DecompilerEventListener eventListener;

        public ArgumentGuesser(IPlatform platform, SsaState ssa, DecompilerEventListener eventListener)
        {
            this.platform = platform;
            this.ssa = ssa;
            this.eventListener = eventListener;
            this.stackPointer = ssa.Procedure.Architecture.StackRegister;
            this.framePointer = ssa.Procedure.Frame.FramePointer.Storage;
        }

        public void Transform()
        {
            foreach (var block in ssa.Procedure.ControlGraph.Blocks)
            {
                for (int i = 0; i < block.Statements.Count; ++i)
                {
                    if (eventListener.IsCanceled())
                        return;
                    var stm = block.Statements[i]; 
                    if (stm.Instruction is CallInstruction call && 
                        call.Callee is ProcedureConstant pc &&
                        pc.Procedure is ExternalProcedure extProc &&
                        !extProc.Signature.ParametersValid)
                    {
                        trace.Verbose("ArgGuess: {0:X}: call to {1}", stm.LinearAddress, extProc);
                        var gargs = GuessArguments(call, block, i - 1);
                        if (gargs is not null)
                        {
                            ReplaceCallWithApplication(stm, call, pc, gargs.Value);
                        }
                    }
                }
            }
        }

        private GuessedArguments? GuessArguments(CallInstruction call, Block block, int i)
        {
            var regWrites = new HashSet<SsaIdentifier>();
            var stackStores = new Dictionary<int, StackSlot>();
            var baseReg = DetermineBaseRegister(call);
            for (; i >= 0; --i)
            {
                var stm = block.Statements[i];
                switch (stm.Instruction)
                {
                case Assignment ass:
                    var sid = AssignmentToArgumentRegister(ass.Dst);
                    if (sid is not null && sid.Uses.Count == 0)
                        regWrites.Add(sid);
                    break;
                case Store store:
                    if (baseReg is not null)
                    {
                        var (mem, offset) = StackStore(store.Dst, baseReg.Storage);
                        if (mem is not null)
                            stackStores[offset] = new StackSlot(offset, stm, mem, store.Src);
                    }
                    break;
                case SideEffect _:
                    return new GuessedArguments(regWrites, stackStores);
                default:
                    throw new NotImplementedException($"GuessArgument: {stm.Instruction.GetType()}");
                }
            }
            return new GuessedArguments(regWrites, stackStores);
        }

        private Identifier? DetermineBaseRegister(CallInstruction call)
        {
            foreach (var use in call.Uses)
            {
                if (use.Storage == this.stackPointer)
                {
                    var (idSp, _) = StackOffset(use.Expression, this.stackPointer);
                    if (idSp is not null)
                        return idSp;
                    (idSp, _) = StackOffset(use.Expression, this.framePointer);
                    if (idSp is not null)
                        return idSp;
                }
            }
            return null;
        }

        private (MemoryAccess? mem, int offset) StackStore(Expression dst, Storage stg)
        {
            if (dst is not MemoryAccess mem)
                return (null, 0);
            var (id, stackOffset) = StackOffset(mem.EffectiveAddress, stg);
            if (id is not null)
                return (mem, stackOffset);
            throw new NotImplementedException();
        }

        private (Identifier?, int) StackOffset(Expression e, Storage stg)
        {
            if (e is BinaryExpression bin &&
                bin.Left is Identifier sp &&
                sp.Storage == stg &&
                bin.Right is Constant c)
            {
                if (bin.Operator == BinaryOperator.IAdd)
                {
                    return (sp, (int) c.ToInt64());
                }
                else if (bin.Operator == BinaryOperator.ISub)
                {
                    return (sp, -(int) c.ToInt64());
                }
            }
            return (null, 0);
        }

        private SsaIdentifier? AssignmentToArgumentRegister(Identifier dst)
        {
            return null;
        }

        private void ReplaceCallWithApplication(
            Statement stmCall, 
            CallInstruction call,
            ProcedureConstant pc,
            GuessedArguments gargs)
        {
            var args = new List<Expression>();
            var arch = ssa.Procedure.Architecture;
            var binder = ssa.Procedure.Frame;
            foreach (var stackslot in gargs.StackSlots.Values.OrderBy(s => s.Offset))
            {
                ssa.RemoveUses(stackslot.stm);
                ssa.Identifiers[stackslot.Dst.MemoryId].DefStatement = null;
                var idTmp = binder.CreateTemporary(stackslot.Dst.DataType);
                var sidTmp = ssa.Identifiers.Add(idTmp, stackslot.stm, stackslot.src, false);
                stackslot.stm.Instruction = new Assignment(sidTmp.Identifier, stackslot.src);
                args.Add(sidTmp.Identifier);
            }
            ssa.RemoveUses(stmCall);
            ssa.ReplaceDefinitions(stmCall, null);
            stmCall.Instruction = new SideEffect(new Application(pc, VoidType.Instance, args.ToArray()));
            var ssam = new SsaMutator(this.ssa);
            ssa.AddDefinitions(stmCall);
            ssa.AddUses(stmCall);
        }

        private class StackSlot
        {
            public readonly int Offset;
            public readonly Statement stm;
            public readonly MemoryAccess Dst;
            public readonly Expression src;

            public StackSlot(int offset, Statement stm, MemoryAccess dst, Expression src)
            {
                this.Offset = offset;
                this.stm = stm;
                this.Dst = dst;
                this.src = src;
            }
        }

        private struct GuessedArguments
        {
            public GuessedArguments(
                HashSet<SsaIdentifier> registers,
                Dictionary<int, StackSlot>  stackSlots)
            {
                this.Registers = registers;
                this.StackSlots = stackSlots;
            }

            public HashSet<SsaIdentifier> Registers { get; }
            public Dictionary<int, StackSlot> StackSlots { get; }

            public void Deconstruct(
                out HashSet<SsaIdentifier> registers,
                out Dictionary<int, StackSlot> stackSlots)
            {
                registers = this.Registers;
                stackSlots = this.StackSlots;
            }
        }
    }
}
