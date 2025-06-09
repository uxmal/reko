#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Services;
using System;
using System.Linq;

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
        private readonly SsaState ssa;
        private readonly IReadOnlyProgram program;
        private readonly Procedure proc;
        private readonly IndirectCallTypeAscender asc;
        private readonly IndirectCallExpander expander;
        private readonly SsaIdentifierTransformer ssaIdTransformer;
        private readonly IEventListener eventListener;
        private readonly SsaMutator ssam;

        public IndirectCallRewriter(
            IReadOnlyProgram program,
            SsaState ssa,
            IEventListener eventListener)
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
            if (pt is null)
                return false;
            if (pt.Pointee is not FunctionType ft)
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
            var fpuStackReg = proc.Architecture.FpuStackRegister;
            if (fpuStackReg is not null)
            {
                ssam.AdjustRegisterAfterCall(
                    stm,
                    call,
                    fpuStackReg,
                    -ft.FpuStackDelta);
            }
            var ab = program.Architecture.CreateFrameApplicationBuilder(
                 proc.Frame, call.CallSite);
            ssa.RemoveUses(stm);
            stm.Instruction = ab.CreateInstruction(call.Callee, ft, null);
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
        public IndirectCallTypeAscender(IReadOnlyProgram program) :
            base(program, new TypeFactory())
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
        private readonly SsaState ssa;

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
                if (sid!.DefStatement is not null &&
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
            return InvalidConstant.Create(phi.DataType);
        }
    }

    /// <summary>
    /// Replace application parameters with ssa identifiers
    /// </summary>
    class SsaIdentifierTransformer : InstructionTransformer
    {
        private readonly SsaState ssa;
        private readonly ArgumentTransformer argumentTransformer;
        private Statement stm;
        private CallInstruction? call;

        public SsaIdentifierTransformer(SsaState ssa)
        {
            this.ssa = ssa;
            this.argumentTransformer = new ArgumentTransformer(this);
            this.stm = default!;
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
            var src = a.Src.Accept(this);
            var dst = (Identifier?) FindDefinedId(call!, a.Dst.Storage);
            if (dst is null)
                return new SideEffect(src);
            DefId(dst);
            return new Assignment(dst, src);
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

        private static Expression InvalidArgument(DataType dt)
        {
            return InvalidConstant.Create(dt);
        }

        public override Expression VisitIdentifier(Identifier id)
        {
            UseId(id);
            return id;
        }

        private void DefId(Identifier id)
        {
            if (ssa.Identifiers.TryGetValue(id, out var sid))
            {
                sid.DefStatement = stm;
            }
        }

        private void UseId(Identifier id)
        {
            if (ssa.Identifiers.TryGetValue(id, out SsaIdentifier? sid))
            {
                sid!.Uses.Add(stm!);
            }
        }

        private static Expression? FindDefinedId(CallInstruction call, Storage storage)
        {
            return call.Definitions
                .Where(d => d.Storage.Equals(storage))
                .Select(d => d.Expression)
                .FirstOrDefault();
        }

        private static Expression? FindUsedId(CallInstruction call, Storage storage)
        {
            return call.Uses
                .Where(u => u.Storage.Equals(storage))
                .Select(u => u.Expression)
                .FirstOrDefault();
        }

        abstract class UsedIdsTransformer : InstructionTransformer
        {
            private readonly SsaIdentifierTransformer outer;

            public UsedIdsTransformer(SsaIdentifierTransformer outer)
            {
                this.outer = outer;
            }

            public abstract Expression VisitMemoryIdentifier(Identifier id);

            public override Expression VisitIdentifier(Identifier id)
            {
                if (id.Storage is MemoryStorage)
                    return VisitMemoryIdentifier(id);
                var usedId = FindUsedId(outer.call!, id.Storage);
                usedId?.Accept(outer);
                return usedId ?? InvalidArgument(id.DataType);
            }
        }

        class ArgumentTransformer : UsedIdsTransformer
        {
            private readonly SsaIdentifierTransformer outer;

            public ArgumentTransformer(SsaIdentifierTransformer outer) :
                base(outer)
            {
                this.outer = outer;
            }

            public override Expression VisitMemoryIdentifier(Identifier id)
            {
                var sid = outer.ssa.Identifiers.Add(
                    id, outer.stm, false);
                sid.DefStatement = null!;
                sid.Uses.Add(outer.stm!);
                return sid.Identifier;
            }
        }

        class DestinationTransformer : UsedIdsTransformer
        {
            private readonly SsaIdentifierTransformer outer;

            public DestinationTransformer(SsaIdentifierTransformer outer) :
                base(outer)
            {
                this.outer = outer;
            }

            public override Expression VisitMemoryIdentifier(Identifier id)
            {
                var sid = outer.ssa.Identifiers.Add(
                    id, outer.stm, false);
                return sid.Identifier;
            }
        }
    }
}
