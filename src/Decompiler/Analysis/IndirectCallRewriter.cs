#region License
/* 
 * Copyright (C) 1999-2016 Pavel Tomin.
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
        }

        public void Rewrite()
        {
            foreach (Statement stm in proc.Statements.ToList())
            {
                CallInstruction ci = stm.Instruction as CallInstruction;
                if (ci != null)
                {
                    try
                    {
                        RewriteCall(stm, ci);
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
        }

        private void RewriteCall(Statement stm, CallInstruction call)
        {
            var e = expander.Expand(call.Callee);
            var dt = e.Accept(asc);
            var pt = dt as Pointer;
            if (pt == null)
                return;
            var ft = pt.Pointee as FunctionType;
            if (ft == null)
                return;
            var returnId = ft.ReturnValue.DataType is VoidType ?
                null : ft.ReturnValue;
            var sigCallee = new FunctionType(null, returnId, ft.Parameters);
            var ab = new FrameApplicationBuilder(
                 program.Architecture, proc.Frame, call.CallSite,
                 call.Callee, true);
            stm.Instruction = ab.CreateInstruction(sigCallee, null);
            ssaIdTransformer.Transform(stm, call);
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
            SsaIdentifier sid;
            if (ssa.Identifiers.TryGetValue(id, out sid))
            {
                Assignment ass;
                if (sid.DefStatement != null &&
                    sid.DefStatement.Instruction.As(out ass))
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
        private Frame frame;
        private Statement stm;
        private CallInstruction call;

        public SsaIdentifierTransformer(SsaState ssa)
        {
            this.ssa = ssa;
            this.frame = ssa.Procedure.Frame;
        }

        public void Transform(Statement stm, CallInstruction call)
        {
            this.stm = stm;
            this.call = call;
            ssa.ReplaceDefinitions(stm, null);
            ssa.RemoveUses(stm);
            Transform(stm.Instruction);
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

        public override Expression VisitApplication(Application appl)
        {
            appl.Procedure = appl.Procedure.Accept(this);
            for (int i = 0; i < appl.Arguments.Length; ++i)
                appl.Arguments[i] = TransformArgument(appl.Arguments[i]);
            return appl;
        }

        private Expression TransformArgument(Expression arg)
        {
            var id = arg as Identifier;
            if (id == null)
                return arg;
            var usedId = FindUsedId(call, id.Storage);
            if (usedId != null)
                usedId.Accept(this);
            return usedId ?? InvalidArgument();
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
            SsaIdentifier sid;
            if (ssa.Identifiers.TryGetValue(id, out sid))
            {
                sid.DefExpression = defExp;
                sid.DefStatement = stm;
            }
        }

        private void UseId(Identifier id)
        {
            SsaIdentifier sid;
            if (ssa.Identifiers.TryGetValue(id, out sid))
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
    }
}
