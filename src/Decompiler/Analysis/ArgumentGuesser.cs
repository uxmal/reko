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
    /// <summary>
    /// Guesses the arguments to otherwise opaque calls by walking backwards
    /// from the call site and detect assignments which likely are intended
    /// to be the arguments of the call. This is a best effort transformation
    /// and may introduce errors, so use with caution.
    /// </summary>
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
                        var gargs = GuessArguments(stm, call, block, i - 1);
                        var gret = GuessReturnValue(stm, call);
                        if (gargs is not null || gret is not null)
                        {
                            ReplaceCallWithApplication(stm, call, pc, gargs, gret);
                            trace.Verbose("  rewritten as: {0}", stm);
                        }
                    }
                }
            }
        }

        private GuessedArguments? GuessArguments(Statement stmCall, CallInstruction call, Block block, int i)
        {
            var regWrites = new HashSet<SsaIdentifier>();
            var stackIds = new HashSet<SsaIdentifier>();
            var stackStores = new Dictionary<int, StackSlot>();
            var baseReg = DetermineBaseRegister(call);
            for (; i >= 0; --i)
            {
                if (eventListener.IsCanceled())
                    return null;
                var stm = block.Statements[i];
                switch (stm.Instruction)
                {
                case Assignment ass:
                    var sid = AssignmentToArgumentRegister(stmCall, ass.Dst);
                    if (sid is not null) {
                        regWrites.Add(sid);
                    }
                    sid = AssignmentToStackLocal(stmCall, ass);
                    if (sid is not null)
                    {
                        stackIds.Add(sid);
                    }
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
                    return new GuessedArguments(regWrites, stackIds, stackStores);
                case CallInstruction _:
                case PhiAssignment _:
                    return new GuessedArguments(regWrites, stackIds, stackStores);
                default:
                    trace.Warn("ArgumentGuesser.GuessArgument: unhandled instruction type: {0}", stm.Instruction.GetType());
                    return new GuessedArguments(regWrites, stackIds, stackStores);
                }
            }
            return new GuessedArguments(regWrites, stackIds, stackStores);
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
            else
                return (null, 0);
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
            if (e is Identifier id &&
                id.Storage == stg)
            {
                return (id, 0);
            }
            return (null, 0);
        }

        private SsaIdentifier? AssignmentToStackLocal(Statement stmCall, Assignment ass)
        {
            if (ass.Dst.Storage is StackLocalStorage)
            {
                var sid = ssa.Identifiers[ass.Dst];
                if (sid.Uses.Count == 0 ||
                    (sid.Uses.Count == 1 && sid.Uses[0] == stmCall))
                {
                    // A stack variable that is never used is a strong hint
                    // that its assignment was intended as an argument to the callee.
                    // However, we may be seeing the capture of a register that is callee
                    // save. Eg. the following x86 code:
                    //  push ebp
                    //  call ext_proc
                    // The ebp is not part of any x86 calling convention so it shouldn't
                    // be used as an argument register.
                    if (ass.Src is not Identifier idSrc)
                    {
                        return sid;
                    }
                    var sidSrc = ssa.Identifiers[idSrc];
                    if (sidSrc.DefStatement?.Instruction is not DefInstruction ||
                        idSrc.Storage is not RegisterStorage reg ||
                        platform.IsPossibleArgumentRegister(reg))
                    {
                        return sid;
                    }
                }
            }
            return null;
        }

        private SsaIdentifier? AssignmentToArgumentRegister(Statement stmCall, Identifier dst)
        {
            if (dst.Storage is RegisterStorage reg &&
                platform.IsPossibleArgumentRegister(reg))
            {
                var sid = ssa.Identifiers[dst];
                if (sid.Uses.Count == 0)
                {
                    // An argument register with no uses is a strong hint that
                    // its assignment was intended as an argument to the callee.
                    return sid;
                }
                if (sid.Uses.Count == 1 && sid.Uses[0] == stmCall)
                {
                    return sid;
                }
            }
            return null;
        }

        private Storage? GuessReturnValue(Statement stm, CallInstruction call)
        {
            var usedDefs = new HashSet<Storage>();
            foreach (var def in call.Definitions)
            {
                if (def.Expression is Identifier id && 
                    ssa.Identifiers[id].Uses.Count > 0)
                {
                    usedDefs.Add(def.Storage);
                }
            }
            return platform.PossibleReturnValue(usedDefs);
        }

        private void ReplaceCallWithApplication(
            Statement stmCall, 
            CallInstruction call,
            ProcedureConstant pc,
            GuessedArguments? gargs,
            Storage? gret)
        {
            var args = new List<Expression>();
            var arch = ssa.Procedure.Architecture;
            var binder = ssa.Procedure.Frame;
            if (gargs.HasValue)
            {
                //$TODO: sort by ABI order.
                foreach (var argreg in gargs.Value.Registers.OrderBy(r => r.Identifier.Name))
                {
                    args.Add(argreg.Identifier);
                }
                foreach (var stkarg in gargs.Value.StackIds.OrderBy(s => ((StackStorage) s.Identifier.Storage).StackOffset))
                {
                    args.Add(stkarg.Identifier);
                }
                foreach (var stackslot in gargs.Value.StackSlots.Values.OrderBy(s => s.Offset))
                {
                    ssa.RemoveUses(stackslot.stm);
                    ssa.Identifiers[stackslot.Dst.MemoryId].DefStatement = null;
                    var idTmp = binder.CreateTemporary(stackslot.Dst.DataType);
                    var sidTmp = ssa.Identifiers.Add(idTmp, stackslot.stm, stackslot.src, false);
                    stackslot.stm.Instruction = new Assignment(sidTmp.Identifier, stackslot.src);
                    ssa.AddUses(stackslot.stm, stackslot.src);
                    args.Add(sidTmp.Identifier);
                }
            }
            var application = new Application(pc, VoidType.Instance, args.ToArray());
            Instruction newInstr;
            if (gret is not null)
            {
                var idRet = MatchingReturnIdentifier(call, gret);
                newInstr = new Assignment(idRet, application);
            }
            else
            {
                newInstr = new SideEffect(application);
            }
            ssa.RemoveUses(stmCall);
            ssa.ReplaceDefinitions(stmCall, null);
            stmCall.Instruction = newInstr;
            ssa.AddDefinitions(stmCall);
            ssa.AddUses(stmCall);
        }

        private Identifier MatchingReturnIdentifier(CallInstruction call, Storage gret)
        {
            foreach (var def in call.Definitions)
            {
                if (def.Storage == gret)
                    return (Identifier) def.Expression;
            }
            throw new NotImplementedException();
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
                HashSet<SsaIdentifier> registerIds,
                HashSet<SsaIdentifier> stackIds,
                Dictionary<int, StackSlot>  stackSlots)
            {
                this.Registers = registerIds;
                this.StackIds = stackIds;
                this.StackSlots = stackSlots;
            }

            public HashSet<SsaIdentifier> Registers { get; }
            public HashSet<SsaIdentifier> StackIds{ get; }
            public Dictionary<int, StackSlot> StackSlots { get; }

            public void Deconstruct(
                out HashSet<SsaIdentifier> registers,
                out HashSet<SsaIdentifier> stackIds,
                out Dictionary<int, StackSlot> stackSlots)
            {
                registers = this.Registers;
                stackIds = this.StackIds;
                stackSlots = this.StackSlots;
            }
        }
    }
}
