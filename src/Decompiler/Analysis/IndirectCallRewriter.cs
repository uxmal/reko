#region License
/* 
 * Copyright (C) 1999-2018 Pavel Tomin.
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
    public class IndirectCallRewriter
    {
        private SsaState ssa;
        private Program program;
        private Procedure proc;
        private IndirectCallTypeAscender asc;
        private IndirectCallExpander expander;
        private SsaIdentifierTransformer ssaIdTransformer;
        private DecompilerEventListener eventListener;
        private ExpressionEmitter m;
        private bool changed;


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
            this.m = new ExpressionEmitter();
        }

        /// <summary>
        /// Rewrites indirect call statements to applications using
        /// user-defined data. Also generates statements that adjust
        /// the stack pointer according to the calling convention.
        /// </summary>
        /// <returns>True if statements were changed.</returns>
        public bool Rewrite()
        {
            changed = false;
            foreach (Statement stm in proc.Statements.ToList())
            {
                if (stm.Instruction is CallInstruction ci)
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
            return changed;
        }

        private void RewriteCall(Statement stm, CallInstruction call)
        {
            var e = expander.Expand(call.Callee);
            var pt = e.Accept(asc).ResolveAs<Pointer>();
            if (pt == null)
                return;
            var ft = pt.Pointee as FunctionType;
            if (ft == null)
                return;
            AdjustRegisterAfterCall(
                stm,
                call,
                program.Architecture.StackRegister,
                ft.StackDelta - call.CallSite.SizeOfReturnAddressOnStack);
            var ab = new ApplicationBuilder(
                program.Architecture, proc.Frame, call.CallSite,
                call.Callee, ft, false);
            stm.Instruction = ab.CreateInstruction();
            ssaIdTransformer.Transform(stm, call);
            DefineUninitializedIdentifiers(stm, call);
            changed = true;
        }

        /// <summary>
        /// Generates a "by-pass" of a register around a Call instruction.
        /// </summary>
        /// <remarks>
        /// If we know that a particular register is incremented/decremented
        /// by a specific constant amount after calling a procedure, we can remove
        /// any definitions of that register from the signature of the procedure.
        /// 
        /// Example: suppose we know that the SP register is incremented by 2 
        /// after the Call instruction:
        /// 
        /// call [something]
        ///     uses: SP_42, [other registers]
        ///     defs: SP_94, [other registers]
        ///     
        /// this function modifies it to look like this:
        /// 
        /// call [something]
        ///     uses: SP_42, [other registers]
        ///     defs: [other registers]
        /// SP_94 = SP_42 + 2
        /// </remarks>
        /// <param name="stm">The Statement containing the CallInstruction.</param>
        /// <param name="call">The CallInstruction.</param>
        /// <param name="register">The register whose post-call value is incremented.</param>
        /// <param name="delta">The amount by which to increment the register.</param>
        private void AdjustRegisterAfterCall(
            Statement stm,
            CallInstruction call,
            RegisterStorage register,
            int delta)
        {
            // Locate the post-call definition of the register, if any
            var defRegBinding = call.Definitions
                .Where(d => d.Identifier is Identifier idDef &&
                            idDef.Storage == register)
                .FirstOrDefault();
            if (defRegBinding == null)
                return;
            var defRegId = defRegBinding.Identifier as Identifier;
            if (defRegId == null)
                return;
            var usedRegExp = call.Uses
                .Select(u => u.Expression)
                .OfType<Identifier>()
                .Where(u => u.Storage == register)
                .FirstOrDefault();
            if (usedRegExp == null)
                return;

            // Generate an instruction that adjusts the register according to
            // the specified delta.
            var src = AddConstant(usedRegExp, register.DataType.Size, delta);
            var ass = new Assignment(defRegId, src);
            var defSid = ssa.Identifiers[defRegId];

            // Insert the instruction after the call statement.
            var adjustRegStm = InsertStatement(stm, ass);

            // Remove the bypassed register definition from
            // the call instructions.
            call.Definitions.Remove(defRegBinding);
            defSid.DefExpression = src;
            defSid.DefStatement = adjustRegStm;
            ssa.AddUses(adjustRegStm);
        }

        private void DefineUninitializedIdentifiers(
            Statement stm,
            CallInstruction call)
        {
            var trashedSids = call.Definitions.Select(d => d.Identifier)
                .Select(id => ssa.Identifiers[id])
                .Where(sid => sid.DefStatement == null);
            foreach (var sid in trashedSids)
            {
                DefineUninitializedIdentifier(stm, sid);
            }
        }

        private void DefineUninitializedIdentifier(
            Statement stm,
            SsaIdentifier sid)
        {
            var value = Constant.Invalid;
            var ass = new Assignment(sid.Identifier, value);
            var newStm = InsertStatement(stm, ass);
            sid.DefExpression = value;
            sid.DefStatement = newStm;
        }

        private Expression AddConstant(Expression e, int cSize, int cValue)
        {
            if (cValue == 0)
            {
                return e;
            }
            else if (cValue > 0)
            {
                return m.IAdd(e, Constant.Word(cSize, cValue));
            }
            else
            {
                return m.ISub(e, Constant.Word(cSize, -cValue));
            }
        }

        private Statement InsertStatement(Statement stm, Instruction instr)
        {
            var block = stm.Block;
            var iPos = block.Statements.IndexOf(stm);
            var linAddr = stm.LinearAddress;
            return block.Statements.Insert(iPos + 1, linAddr, instr);
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
            a.Dst = FindDefinedId(call, a.Dst.Storage);
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
            appl.Procedure = appl.Procedure.Accept(this);
            for (int i = 0; i < appl.Arguments.Length; ++i)
                appl.Arguments[i] = TransformArgument(appl.Arguments[i]);
            return appl;
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

        private Identifier FindDefinedId(CallInstruction call, Storage storage)
        {
            return call.Definitions.Select(d => d.Identifier).
                OfType<Identifier>().
                Where(usedId => usedId.Storage.Equals(storage)).
                FirstOrDefault();
        }

        private Identifier FindUsedId(CallInstruction call, Storage storage)
        {
            return call.Uses.Select(u => u.Expression).
                OfType<Identifier>().
                Where(usedId => usedId.Storage.Equals(storage)).
                FirstOrDefault();
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
