#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using System.Linq;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Analysis
{
    /// <summary>
    /// Try to rewrite indirect call statements to applications using
    /// user-defined data (e.g. global variables, parameters of procedures).
    /// </summary>
    /// <remarks>
    /// //$REVIEW: Once analysis-development branch is complete it will make
    /// dealing with MIPS ELF binaries a lot nicer. Currently, almost all
    /// calls in a MIPS ELF binary are decompiled into messy indirect 
    /// calls.
    /// </remarks>
    public class IndirectCallRewriter
    {
        private SsaState ssa;
        private Program program;
        private Procedure proc;
        private IndirectCallTypeAscender asc;
        private IndirectCallExpander expander;
        private SsaIdentifierTransformer ssaIdTransformer;
        private DecompilerEventListener eventListener;
        private SsaMutator ssam;

        public IndirectCallRewriter(
            Program program,
            SsaState ssa,
            DecompilerEventListener eventListener)
        {
            this.program = program;
            this.proc = ssa.Procedure;
            this.ssa = ssa;
            this.asc = new IndirectCallTypeAscender(program);
            this.expander = new IndirectCallExpander(ssa);
            this.ssaIdTransformer = new SsaIdentifierTransformer(ssa);
            this.eventListener = eventListener;
            this.ssam = new SsaMutator(ssa);
        }

        /// <summary>
        /// Rewrites indirect call statements to applications using
        /// user-defined data. Also generates statements that adjust
        /// the stack pointer according to the calling convention.
        /// </summary>
        /// <returns>True if statements were changed.</returns>
        public bool Rewrite()
        {
            bool changed = false;
            foreach (Statement stm in proc.Statements.ToList())
            {
                if (stm.Instruction is CallInstruction ci)
                {
                    try
                    {
                        changed |= TryRewriteCall(stm, ci);
                    }
                    catch (Exception ex)
                    {
                        eventListener.Error(
                            eventListener.CreateStatementNavigator(program, stm),
                            ex,
                            "Indirect call rewriter encountered an error while processing the statement {0}.",
                            stm);
                    }
                }
            }
            return changed;
        }

        private bool TryRewriteCall(Statement stm, CallInstruction call)
        {
            var e = expander.Expand(call.Callee);
            var pt = e.Accept(asc).ResolveAs<Pointer>();
            if (pt == null)
                return false;
            if (!(pt.Pointee is FunctionType ft))
                return false;
            RewriteCall(stm, call, ft);
            return true;
        }

        public void RewriteCall(Statement stm, CallInstruction call, FunctionType ft)
        {
            ssam.AdjustRegisterAfterCall(
                stm,
                call,
                ssa.Procedure.Architecture.StackRegister,
                ft.StackDelta - call.CallSite.SizeOfReturnAddressOnStack);
            ssam.AdjustRegisterAfterCall(
                stm,
                call,
                program.Architecture.FpuStackRegister,
                -ft.FpuStackDelta);
            var ab = program.Architecture.CreateFrameApplicationBuilder(
                 proc.Frame, call.CallSite, call.Callee);
            stm.Instruction = ab.CreateInstruction(ft, null);
            ssaIdTransformer.Transform(stm, call);
            ssam.DefineUninitializedIdentifiers(stm, call);
        }
            }

    /// <summary>
    /// Pulling type information from the leaves of expression trees to their
    /// roots without store it.
    /// </summary>
    class IndirectCallTypeAscender : ExpressionTypeAscenderBase
    {
        public IndirectCallTypeAscender(Program program) :
            base (program, new TypeFactory())
        {
        }

        protected override DataType RecordDataType(DataType dt, Expression exp)
        {
            return dt;
        }

        protected override DataType EnsureDataType(DataType dt, Expression exp)
        {
            return dt;
        }
    }

    /// <summary>
    /// Replace ssa identifiers with expressions that defines them. Also
    /// replace phi functions.
    /// </summary>
    class IndirectCallExpander : InstructionTransformer
    {
        private SsaState ssa;

        public IndirectCallExpander(SsaState ssa)
        {
            this.ssa = ssa;
        }

        public Expression Expand(Expression e)
        {
            return e.CloneExpression().Accept(this);
        }

        public override Expression VisitIdentifier(Identifier id)
        {
            if (ssa.Identifiers.TryGetValue(id, out var sid))
            {
                if (sid.DefStatement != null &&
                    sid.DefStatement.Instruction is Assignment ass &&
                    ass.Dst == id)
                {
                    return Expand(ass.Src);
                }
            }
            return id;
        }

        public override Expression VisitPhiFunction(PhiFunction phi)
        {
            return Constant.Invalid;
        }
    }

    /// <summary>
    /// Replace application parameters with ssa identifiers
    /// </summary>
    class SsaIdentifierTransformer : InstructionTransformer
    {
        private SsaState ssa;
        private IStorageBinder binder;
        private Statement stm;
        private CallInstruction call;
        private ArgumentTransformer argumentTransformer;

        public SsaIdentifierTransformer(SsaState ssa)
        {
            this.ssa = ssa;
            this.binder = ssa.Procedure.Frame;
            this.argumentTransformer = new ArgumentTransformer(this);
        }

        public void Transform(Statement stm, CallInstruction call)
        {
            this.stm = stm;
            this.call = call;
            ssa.ReplaceDefinitions(stm, null);
            ssa.RemoveUses(stm);
            stm.Instruction = Transform(stm.Instruction);
        }

        public override Instruction TransformAssignment(Assignment a)
        {
            a.Src = a.Src.Accept(this);
            a.Dst = (Identifier)FindDefinedId(call, a.Dst.Storage);
            if (a.Dst == null)
                return new SideEffect(a.Src);
            DefId(a.Dst, a.Src);
            return a;
        }

        public override Instruction TransformStore(Store store)
        {
            store.Src = store.Src.Accept(this);
            store.Dst = TransformDst(store.Dst);
            return store;
        }

        public override Expression VisitApplication(Application appl)
        {
            var proc = appl.Procedure.Accept(this);
            var args = new Expression[appl.Arguments.Length];
            for (int i = 0; i < appl.Arguments.Length; ++i)
                args[i] = TransformArgument(appl.Arguments[i]);
            return new Application(proc, appl.DataType, args);
        }

        private Expression TransformDst(Expression dst)
        {
            var dstTransformer = new DestinationTransformer(this);
            return dst.Accept(dstTransformer);
        }

        private Expression TransformArgument(Expression arg)
        {
            return arg.Accept(argumentTransformer);
        }

        private Expression InvalidArgument()
        {
            return Constant.Word32(Constant.Invalid.ToUInt32());
        }

        public override Expression VisitIdentifier(Identifier id)
        {
            UseId(id);
            return id;
        }

        private void DefId(Identifier id, Expression defExp)
        {
            if (ssa.Identifiers.TryGetValue(id, out var sid))
            {
                sid.DefExpression = defExp;
                sid.DefStatement = stm;
            }
        }

        private void UseId(Identifier id)
        {
            if (ssa.Identifiers.TryGetValue(id, out var sid))
            {
                sid.Uses.Add(stm);
            }
        }

        private Expression FindDefinedId(CallInstruction call, Storage storage)
        {
            return call.Definitions
                .Where(d => d.Storage.Equals(storage))
                .Select(d => d.Expression)
                .FirstOrDefault();
        }

        private Expression FindUsedId(CallInstruction call, Storage storage)
        {
            return call.Uses
                .Where(u => u.Storage.Equals(storage))
                .Select(u => u.Expression)
                .FirstOrDefault();
        }

        abstract class UsedIdsTransformer : InstructionTransformer
        {
            SsaIdentifierTransformer outer;

            public UsedIdsTransformer(SsaIdentifierTransformer outer)
            {
                this.outer = outer;
            }

            public abstract Expression VisitMemoryIdentifier(MemoryIdentifier id);

            public override Expression VisitIdentifier(Identifier id)
            {
                if (id is MemoryIdentifier memoryId)
                    return VisitMemoryIdentifier(memoryId);
                var usedId = outer.FindUsedId(outer.call, id.Storage);
                if (usedId != null)
                    usedId.Accept(outer);
                return usedId ?? outer.InvalidArgument();
            }
        }

        class ArgumentTransformer : UsedIdsTransformer
        {
            SsaIdentifierTransformer outer;

            public ArgumentTransformer(SsaIdentifierTransformer outer) :
                base(outer)
            {
                this.outer = outer;
            }

            public override Expression VisitMemoryIdentifier(MemoryIdentifier id)
            {
                var sid = outer.ssa.Identifiers.Add(
                    id, outer.stm, null, false);
                sid.DefStatement = null;
                sid.Uses.Add(outer.stm);
                return sid.Identifier;
            }
        }

        class DestinationTransformer : UsedIdsTransformer
        {
            SsaIdentifierTransformer outer;

            public DestinationTransformer(SsaIdentifierTransformer outer) :
                base(outer)
            {
                this.outer = outer;
            }

            public override Expression VisitMemoryIdentifier(MemoryIdentifier id)
            {
                var sid = outer.ssa.Identifiers.Add(
                    id, outer.stm, null, false);
                return sid.Identifier;
            }
        }
    }
}
