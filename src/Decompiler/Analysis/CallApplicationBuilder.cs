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
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
    using BindingDictionary = Dictionary<Storage, CallBinding>;

    /// <summary>
    /// Builds an application from a call instruction, ussing a <see cref="SsaState"/> instance.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CallApplicationBuilder : ApplicationBuilder, StorageVisitor<Expression?, (BindingDictionary map, ApplicationBindingType bindUses)>
    {
        private readonly SsaState ssaCaller;
        private readonly Statement stmCall;
        private readonly SsaMutator mutator;
        private readonly IProcessorArchitecture arch;
        private readonly int stackDepthOnEntry;
        private readonly Dictionary<Storage, CallBinding> defs;
        private readonly Dictionary<Storage, CallBinding> uses;
        private readonly MemIdentifierFinder midFinder;
        private readonly bool guessStackArgs;

        public CallApplicationBuilder(SsaState ssaCaller, Statement stmCall, CallInstruction call, Expression callee, bool guessStackArgs) : base(call.CallSite, callee)
        {
            this.ssaCaller = ssaCaller;
            this.stmCall = stmCall;
            this.mutator = new SsaMutator(ssaCaller);
            this.arch = ssaCaller.Procedure.Architecture;
            this.defs = call.Definitions.ToDictionary(d => d.Storage);
            this.uses = call.Uses.ToDictionary(u => u.Storage);
            this.stackDepthOnEntry = site.StackDepthOnEntry;
            this.midFinder = new MemIdentifierFinder();
            this.guessStackArgs = guessStackArgs;
        }

        public override Expression? Bind(Identifier id)
        {
            return id.Storage.Accept(this, (uses, ApplicationBindingType.In));
        }

        public override OutArgument BindOutArg(Identifier id)
        {
            var exp = id.Storage.Accept(this, (defs, ApplicationBindingType.Out));
            return new OutArgument(arch.FramePointerType, exp!);
        }

        public override Expression? BindReturnValue(Identifier id)
        {
            return id.Storage.Accept(this, (defs, ApplicationBindingType.Return));
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
            foreach (var de in ctx.map.Values
              .Where(d => d.Storage is FpuStackStorage))
            {
                if (((FpuStackStorage) de.Storage).FpuStackOffset == fpu.FpuStackOffset)
                    return de.Expression;
            }
            if (ctx.bindUses != ApplicationBindingType.In)
                return null;
            throw new NotImplementedException($"Offsets not matching? SP({fpu.FpuStackOffset}).");
        }

        public Expression VisitMemoryStorage(MemoryStorage global, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            throw new NotImplementedException();
        }

        public Expression VisitOutArgumentStorage(OutArgumentStorage arg, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            if (defs.TryGetValue(arg.OriginalIdentifier.Storage, out var binding))
            {
                return binding.Expression;
            }
            else
            {
                // This out variable is dead, but we need to create a dummy identifier
                // for it to maintain the consistency of the SSA graph.
                var sid = ssaCaller.Identifiers.Add(arg.OriginalIdentifier, stmCall, null, true);
                return sid.Identifier;
            }
        }

        public Expression? VisitRegisterStorage(RegisterStorage reg, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            // If the architecture has no subregisters, this test will
            // be true.
            if (ctx.map.TryGetValue(reg, out CallBinding cb))
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
                var sidSeq = ssaCaller.Identifiers.Add(idSeq, this.stmCall, null, true);
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

        public Expression VisitStackArgumentStorage(StackArgumentStorage stack, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            int localOff = stack.StackOffset - stackDepthOnEntry;
            foreach (var de in ctx.map
                .Where(d => d.Value.Storage is StackStorage))
            {
                if (((StackStorage) de.Value.Storage).StackOffset == localOff)
                    return de.Value.Expression;
            }

            // Attempt to inject a Mem[sp_xx + offset] expression if possible.
            if (guessStackArgs &&
                sigCallee != null &&
                ctx.map.TryGetValue(arch.StackRegister, out var stackBinding) &&
                this.TryFindMemBeforeCall(out var memId))
            {
                var sp_ssa = stackBinding.Expression;
                if (sp_ssa != null)
                {
                    var dt = PrimitiveType.Create(Domain.SignedInt, sp_ssa.DataType.BitSize);
                    var ea = sp_ssa;
                    int nOffset = stack.StackOffset - sigCallee.ReturnAddressOnStack;
                    if (nOffset != 0)
                    {
                        var offset = Constant.Create(dt, nOffset);
                        ea = new BinaryExpression(Operator.IAdd, sp_ssa.DataType, sp_ssa, offset);
                    }
                    return new MemoryAccess(memId, ea, stack.DataType);
                }
            }
            return FallbackArgument($"stackArg{localOff}", stack.DataType);
        }

        private bool TryFindMemBeforeCall(out MemoryIdentifier memId)
        {
            var block = stmCall.Block;
            var i = block.Statements.IndexOf(stmCall) - 1;
            for (;  i >= 0; --i)
            {
                memId = midFinder.Find(block.Statements[i].Instruction)!;
                if (memId != null)
                    return true;
            }
            memId = null!;
            return false;
        }

        public Expression VisitStackLocalStorage(StackLocalStorage local, (BindingDictionary map, ApplicationBindingType bindUses) ctx)
        {
            throw new NotImplementedException();
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
            var sid = ssaCaller.Identifiers.Add(id, null, null, false);
            DefineUninitializedIdentifier(stmCall, sid);
            sid.Uses.Add(stmCall);
            return sid.Identifier;
        }

        private void DefineUninitializedIdentifier(
            Statement stm,
            SsaIdentifier sid)
        {
            var value = InvalidConstant.Create(sid.Identifier.DataType);
            var ass = new Assignment(sid.Identifier, value);
            var newStm = InsertStatementBefore(ass, stm);
            sid.DefExpression = value;
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
        private Statement InsertStatementBefore(Instruction instr, Statement stmBefore)
        {
            var block = stmBefore.Block;
            var iPos = block.Statements.IndexOf(stmBefore);
            var linAddr = stmBefore.LinearAddress;
            return block.Statements.Insert(iPos, linAddr, instr);
        }

        private Identifier EnsureRegister(RegisterStorage reg)
        {
            var id = ssaCaller.Procedure.Frame.EnsureRegister(reg);
            var entryBlock = ssaCaller.Procedure.EntryBlock;
            var sid = ssaCaller.EnsureDefInstruction(id, entryBlock);
            sid.Uses.Add(stmCall);
            return sid.Identifier;
        }

        private class MemIdentifierFinder : InstructionVisitorBase
        {
            private MemoryIdentifier? mid;

            public MemoryIdentifier? Find(Instruction instr)
            {
                this.mid = null;
                instr.Accept(this);
                return mid;
    }

            public override void VisitStore(Store store)
            {
                store.Dst.Accept(this);
                if (mid != null)
                    return;
                store.Src.Accept(this);
            }

            public override void VisitMemoryAccess(MemoryAccess access)
            {
                this.mid = access.MemoryId;
                base.VisitMemoryAccess(access);
            }

            public override void VisitSegmentedAccess(SegmentedAccess access)
            {
                this.mid = access.MemoryId;
                base.VisitSegmentedAccess(access);
            }
        }
    }

    public enum ApplicationBindingType
    {
        In,         // Binding is an input parameter
        Return,     // Binding is a returned value
        Out,        // Binding is an out parameter
    }
}
