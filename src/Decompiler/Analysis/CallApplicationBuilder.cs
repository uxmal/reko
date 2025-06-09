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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Analysis
{
    using BindingDictionary = Dictionary<Storage, CallBinding>;

    /// <summary>
    /// Builds an application from a call instruction, using a <see cref="SsaState"/> instance.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CallApplicationBuilder :
        ApplicationBuilder,
        StorageVisitor<Expression?, (BindingDictionary map, ApplicationBindingType bindUses)>
    {
        private readonly SsaState ssaCaller;
        private readonly Statement stmCall;
        private readonly SsaMutator mutator;
        private readonly IProcessorArchitecture arch;
        /// <summary>
        /// The definitions reaching the call being rewritten.
        /// </summary>
        private readonly BindingDictionary defs;
        /// <summary>
        /// The live uses leaving the call after it is called.
        /// </summary>
        private readonly BindingDictionary uses;
        private readonly bool guessStackArgs;

        public CallApplicationBuilder(SsaState ssaCaller, Statement stmCall, CallInstruction call, bool guessStackArgs)
            : base(call.CallSite)
        {
            this.ssaCaller = ssaCaller;
            this.stmCall = stmCall;
            this.mutator = new SsaMutator(ssaCaller);
            this.arch = ssaCaller.Procedure.Architecture;
            this.defs = call.Definitions.ToDictionary(d => d.Storage);
            this.uses = call.Uses.ToDictionary(u => u.Storage);
            this.guessStackArgs = guessStackArgs;
        }

        public override Expression? BindInArg(Storage stg)
        {
            return stg.Accept(this, (uses, ApplicationBindingType.In));
        }

        public override OutArgument BindOutArg(Storage stg)
        {
            var exp = stg.Accept(this, (defs, ApplicationBindingType.Out));
            return new OutArgument(stg.DataType, exp!);
        }

        public override Expression? BindReturnValue(Storage? stg)
        {
            if (stg is null)
                return null;
            return stg.Accept(this, (defs, ApplicationBindingType.Return));
        }

        public Expression? VisitFlagGroupStorage(FlagGroupStorage grf, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            if (!ctx.map.TryGetValue(grf, out var cb))
                return null;
            else
                return cb.Expression;
        }

        public Expression? VisitFpuStackStorage(FpuStackStorage fpu, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            foreach (var de in ctx.map.Values)
            {
                if (de.Storage is FpuStackStorage fpuStg &&
                    fpuStg.FpuStackOffset == fpu.FpuStackOffset)
                {
                    return de.Expression;
                }
            }
            if (ctx.bindUses != ApplicationBindingType.In)
                return null;
            throw new NotImplementedException($"Offsets not matching? SP({fpu.FpuStackOffset}).");
        }

        public Expression VisitMemoryStorage(MemoryStorage global, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            throw new NotImplementedException();
        }
        
        private Expression DoBindOutArgument(Storage arg, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            if (defs.TryGetValue(arg, out var binding))
            {
                return binding.Expression;
            }
            else
            {
                // This out variable is dead, but we need to create a dummy identifier
                // for it to maintain the consistency of the SSA graph.
                var sid = ssaCaller.Identifiers.Add(Identifier.Create(arg), stmCall, true);
                return sid.Identifier;
            }
        }

        public Expression? VisitRegisterStorage(RegisterStorage reg, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            if (ctx.bindUses != ApplicationBindingType.Out)
            {
                // If the architecture has no subregisters, this test will
                // be true.
                if (ctx.map.TryGetValue(reg, out CallBinding? cb))
                    return cb.Expression;

                // If the architecture has subregisters, we need a more
                // expensive test.
                //$TODO: perhaps this can be done with another level of lookup, 
                // eg by using a Dictionary<StorageDomain<Dictionary<Storage, CallBinding>>
                foreach (var de in ctx.map)
                {
                    if (reg.OverlapsWith(de.Value.Storage))
                    {
                        return de.Value.Expression;
                    }
                }
                if (ctx.bindUses == ApplicationBindingType.In)
                    return EnsureRegister(reg);
                return null;
            }
            else
            {
                return DoBindOutArgument(reg, ctx);
            }
        }

        public Expression VisitSequenceStorage(SequenceStorage seq, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            if (ctx.map.TryGetValue(seq, out var binding))
            {
                return binding.Expression;
            }
            if (ctx.bindUses == ApplicationBindingType.In)
            {
                var exps = seq.Elements
                    .Select(stg => stg.Accept(this, ctx) ?? InvalidConstant.Create(stg.DataType))
                    .ToArray();
                return new MkSequence(seq.DataType, exps);
            }
            else
            {
                // No available identifier, so synthesize one.
                var idSeq = ssaCaller.Procedure.Frame.EnsureSequence(seq.DataType, seq.Elements);
                var sidSeq = ssaCaller.Identifiers.Add(idSeq, this.stmCall, true);
                int bitOffset = seq.DataType.BitSize;
                foreach (var stg in seq.Elements)
                {
                    bitOffset -= (int) stg.BitSize;
                    if (ctx.map.TryGetValue(stg, out var b))
                    {
                        var idAlias = (Identifier) b.Expression;
                        var slice = new Slice(b.Expression.DataType, sidSeq.Identifier, bitOffset);
                        var ass = new AliasAssignment(idAlias, slice);
                        var stmAlias = mutator.InsertStatementAfter(ass, stmCall);
                        var sidAlias = ssaCaller.Identifiers[idAlias];
                        sidSeq.Uses.Add(stmAlias);
                        sidAlias.DefStatement = stmAlias;
                    }
                }
                return sidSeq.Identifier;
            }
        }

        public Expression VisitStackStorage(StackStorage stack, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            return BindInStackArg(stack, sigCallee?.ReturnAddressOnStack ?? 0, uses);
        }

        public override Expression BindInStackArg(StackStorage stack, int returnAdjustment)
        {
            return BindInStackArg(stack, returnAdjustment, uses);
        }

        private Expression BindInStackArg(StackStorage stack, int returnAdjustment, BindingDictionary map)
        {
            Debug.Assert(ssaCaller.Procedure.Architecture.IsStackArgumentOffset(stack.StackOffset));
            int localOff = stack.StackOffset;
            // See if the stack argument has already been bound in the
            // call instruction.
            foreach (var de in map.Values)
            {
                if (de.Storage is StackStorage stk &&
                    stk.StackOffset == localOff)
                    return de.Expression;
            }

            // Attempt to inject a Mem[sp_xx + offset] expression if possible.
            if (guessStackArgs &&
                map.TryGetValue(arch.StackRegister, out var stackBinding) &&
                this.TryFindMemBeforeCall(this.uses.Values, out Identifier? memId))
            {
                var sp_ssa = stackBinding.Expression;
                if (sp_ssa is not null)
                {
                    var dt = PrimitiveType.Create(Domain.SignedInt, sp_ssa.DataType.BitSize);
                    var ea = sp_ssa;
                    int nOffset = stack.StackOffset - returnAdjustment;
                    if (nOffset != 0)
                    {
                        var offset = Constant.Create(dt, nOffset);
                        ea = new BinaryExpression(Operator.IAdd, sp_ssa.DataType, sp_ssa, offset);
                    }
                    return new MemoryAccess(memId!, ea, stack.DataType);
                }
            }
            return FallbackArgument($"stackArg{localOff}", stack.DataType);
        }

        private bool TryFindMemBeforeCall(
            IEnumerable<CallBinding> uses,
            [MaybeNullWhen(false)] out Identifier? memId)
        {
            // See if there is a Mem captured by the call. This happens in
            // SsaTransform.GenerateUseDefsVarargs and SsaTransform.GenerateUseDefsForUnknownCallee
            // This should never fail, but we leave the failure path open so that
            // users can report the failure.
            var mem = ssaCaller.Procedure.Frame.Memory;
            foreach (var use in uses)
            {
                if (use.Storage is MemoryStorage)
                {
                    memId = (Identifier) use.Expression;
                    return true;
                }
            }

            // Every indirect call or varargs call should have had injected
            // Mem references into the call instruction. We should never
            // reach here.
            memId = null;
            return false;
        }

        public Expression VisitTemporaryStorage(TemporaryStorage temp, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Argument was not found in the map due to bugs at previous stages.
        /// Create '<paramref name="name"/> = Constant.Invalid' statement.
        /// </summary>
        /// <param name="name">Argument name</param>
        /// <param name="dt">Argument type</param>
        /// <returns>
        /// Identifier with name '<paramref name="name"/>' as fallback argument
        /// </returns>
        private Identifier FallbackArgument(string name, DataType dt)
        {
            var id = ssaCaller.Procedure.Frame.CreateTemporary(name, dt);
            var sid = ssaCaller.Identifiers.Add(id, null, false);
            DefineUninitializedIdentifier(stmCall, sid);
            sid.Uses.Add(stmCall);
            return sid.Identifier;
        }

        private static void DefineUninitializedIdentifier(
            Statement stm,
            SsaIdentifier sid)
        {
            var value = InvalidConstant.Create(sid.Identifier.DataType);
            var ass = new Assignment(sid.Identifier, value);
            var newStm = InsertStatementBefore(ass, stm);
            sid.DefStatement = newStm;
            var comment =
@"Failed to bind call argument.
Please report this issue at https://github.com/uxmal/reko";
            InsertStatementBefore(new CodeComment(comment), newStm);
        }

        /// <summary>
        /// Inserts the instruction <paramref name="instr"/> into a
        /// Block before the statement <paramref name="stmBefore"/>.
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="stmBefore"></param>
        /// <returns></returns>
        private static Statement InsertStatementBefore(Instruction instr, Statement stmBefore)
        {
            var block = stmBefore.Block;
            var iPos = block.Statements.IndexOf(stmBefore);
            return block.Statements.Insert(iPos, stmBefore.Address, instr);
        }

        private Identifier EnsureRegister(RegisterStorage reg)
        {
            var id = ssaCaller.Procedure.Frame.EnsureRegister(reg);
            var entryBlock = ssaCaller.Procedure.EntryBlock;
            var sid = ssaCaller.EnsureDefInstruction(id, entryBlock);
            sid.Uses.Add(stmCall);
            return sid.Identifier;
        }
    }

    public enum ApplicationBindingType
    {
        In,         // Binding is an input parameter
        Return,     // Binding is a returned value
        Out,        // Binding is an out parameter
    }
}
