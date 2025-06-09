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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Services;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Typing
{
    /// <summary>
    /// Rewrites all the expressions in the program based on the type information provided.
    /// </summary>
    /// <remarks>
    /// It's important that all Visit*(subclass-of-expression) methods preserve the 
    /// <see cref="Expression.TypeVariable"/> property when new expressions are created.
    /// </remarks>
    public class TypedExpressionRewriter : InstructionTransformer
    {
        private readonly Program program;
        private readonly TypeStore store;
        private readonly DataTypeComparer compTypes;
        private readonly TypedConstantRewriter tcr;
        private readonly Unifier unifier;
        private readonly IDecompilerEventListener eventListener;
        private DataType? dereferencedType;
        private Expression? basePtr;
        private Statement stmCur;

        public TypedExpressionRewriter(Program program, TypeStore store, IDecompilerEventListener eventListener)
        {
            this.program = program;
            this.store = store;
            this.eventListener = eventListener;
            this.compTypes = new DataTypeComparer();
            this.tcr = new TypedConstantRewriter(program, store, eventListener);
            this.unifier = new Unifier();
            this.stmCur = default!;
        }

        public void RewriteProgram(Program program)
        {
            int cProc = program.Procedures.Count;
            int i = 0;
            foreach (Procedure proc in program.Procedures.Values)
            {
                eventListener.Progress.ShowProgress("Rewriting expressions.", i++, cProc);
                RewriteFormals(proc.Signature);
                foreach (Statement stm in proc.Statements)
                {
                    if (eventListener.IsCanceled())
                        return;
                    try
                    {
                        stmCur = stm;
                        stm.Instruction = stm.Instruction.Accept(this);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Exception in TypedExpressionRewriter.RewriteProgram: {0} ({1})\r\n{2}",
                            proc, ex.Message, ex.StackTrace);
                        // reset flags after error
                        dereferencedType = null;
                        basePtr = null;
                    }
                }
            }
        }

        private void RewriteFormals(FunctionType sig)
        {
            if (!sig.HasVoidReturn)
                sig.Outputs[0].DataType = store.GetTypeVariable(sig.Outputs[0]).DataType;
            if (sig.Parameters is not null)
            {
                foreach (Identifier formalArg in sig.Parameters)
                {
                    if (store.TryGetTypeVariable(formalArg, out var tvParameter))
                        formalArg.DataType = tvParameter.DataType;
                }
            }
        }

        public override Instruction TransformAssignment(Assignment a)
        {
            return MakeAssignment(a.Dst, a.Src);
        }

        private Instruction MakeAssignment(Expression dst, Expression src)
        {
            src = src.Accept(this);
            var tvDst = store.GetTypeVariable(dst);
            dst = dst.Accept(this);
            var dtSrc = DataTypeOf(src);
            var dtDst = DataTypeOf(dst);
            if (!TypesAreCompatible(dtSrc, dtDst))
            {
                UnionType? uDst = dtDst.ResolveAs<UnionType>();
                UnionType? uSrc = dtSrc.ResolveAs<UnionType>();
                if (uDst is not null)
                {
                    dst = RewriteUnionLValue(dst, uDst, dtSrc);
                }
                else if (uSrc is not null)
                {
                    //var ceb = new ComplexExpressionBuilder(dtSrc, dtSrc, dtDst, null, src, null, 0);
                    //src = ceb.BuildComplex(false);
                    src = new Cast(dtDst, src);
                }
                else
                {
                    Debug.Print("{2} [{0}] = {3} [{1}] not supported.", dtDst, dtSrc, dst, src);
                    src = new Cast(dtDst, src);
                }
            }
            if (dst is Identifier idDst)
                return new Assignment(idDst, src);
            else
                return new Store(dst, src);
        }

        private Expression RewriteUnionLValue(
            Expression dst, UnionType uDst, DataType dtSrc)
        {
            var alt = uDst.FindAlternative(dtSrc);
            alt ??= UnionAlternativeChooser.Choose(uDst, null, false, 0);
            if (alt is null)
                return dst;
            return new FieldAccess(alt.DataType, dst, alt);
        }

        public override Instruction TransformCallInstruction(CallInstruction ci)
        {
            var exp = Rewrite(ci.Callee, VoidType.Instance);
            return new SideEffect(new Application(exp, VoidType.Instance));
        }

        public override Instruction TransformStore(Store store)
        {
            return MakeAssignment(store.Dst, store.Src);
        }

        public override Instruction TransformSideEffect(SideEffect side)
        {
            var instr = base.TransformSideEffect(side);
            return instr;
        }

        public Expression Rewrite(Expression expression, DataType? dereferencedType)
        {
            var oldDereferenced = this.dereferencedType;
            this.dereferencedType = dereferencedType;
            var exp = expression.Accept(this);
            this.dereferencedType = oldDereferenced;
            return exp;
        }

        public Expression RewriteComplexExpression(Expression complex, Expression? index, int offset, DataType? dereferencedType)
        {
            if (index is Constant cOther)
            {
                //$REVIEW: changing this to:
                // offset += cOther.ToInt32() causes a regression.
                // This needs further investigation.
                offset += (int) cOther.ToUInt32();
                index = null;
            }
            var ceb = new ComplexExpressionBuilder(program, store, basePtr, complex, index, offset);
            return ceb.BuildComplex(dereferencedType);
        }

        public override Expression VisitApplication(Application appl)
        {
            return base.VisitApplication(appl);
        }

        public override Expression VisitArrayAccess(ArrayAccess acc)
        {
            if (acc.Array is BinaryExpression bin &&
                bin.Operator.Type == OperatorType.IAdd &&
                bin.Right is Constant c)
            {
                // (x + C)[...]
                var arrayPtr = Rewrite(bin.Left, null);
                var index = Rewrite(acc.Index, null);
                return RewriteComplexExpression(
                    arrayPtr, index, c.ToInt32(), acc.DataType);
            }
            else
            {
                // (x)[...]
                var arrayPtr = Rewrite(acc.Array, null);
                var index = Rewrite(acc.Index, null);
                return RewriteComplexExpression(
                    arrayPtr, index, 0, acc.DataType);
            }
        }

        public override Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            var left = Rewrite(binExp.Left, null);
            var right = Rewrite(binExp.Right, null);

            if (binExp.Operator.Type == OperatorType.IAdd)
            {
                if (DataTypeOf(left).IsComplex)
                {
                    if (DataTypeOf(right).IsComplex)
                    {
                        // TODO: this spews a lot of output. To make it less annoying
                        // we display the errors only in debug builds, until we've 
                        // corrected the sources of the errors.
#if DEBUG
                        // This used to throw an awful lot of exceptions if 
                        // type inference failed. A common occurrence is 
                        // when all type variables have been placed into one
                        // massive union. We now give up instead and return 
                        // an un-rewritten expression.
                        this.eventListener.Warn(
                            eventListener.CreateStatementNavigator(program, stmCur),
                            "Both left and right sides of a binary expression can't be complex types.{0}{1}: {2} vs {3}.",
                            Environment.NewLine, binExp,
                            DataTypeOf(left),
                            DataTypeOf(right));
#endif
                        left = new BinaryExpression(binExp.Operator, binExp.DataType, left, right);
                        right = null;
                    }
                    return RewriteComplexExpression(left, right, 0, dereferencedType);
                }
                else if (DataTypeOf(right).IsComplex)
                {
                    return RewriteComplexExpression(right, left, 0, dereferencedType);
                }
                if (dereferencedType is not null)
                {
                    return RewriteComplexExpression(left, right, 0, dereferencedType);
                }
            }
            var binOp = binExp.Operator;
            binOp = binOp.Type switch
            {
                OperatorType.SMod => Operator.IMod,
                OperatorType.Uge => Operator.Ge,
                OperatorType.Ugt => Operator.Gt,
                OperatorType.Ule => Operator.Le,
                OperatorType.Ult => Operator.Lt,
                OperatorType.UMul => Operator.IMul,
                OperatorType.UMod => Operator.IMod,
                OperatorType.USub => Operator.ISub,
                OperatorType.Shr => Operator.Sar,
                _ => binOp,
            };
            var tvBinExp = store.GetTypeVariable(binExp);
            var binExpNew = new BinaryExpression(binOp, binExp.DataType, left, right);
            store.SetTypeVariable(binExpNew, tvBinExp);
            store.SetTypeVariableExpression(tvBinExp, stmCur?.Address, binExpNew);
            if (dereferencedType is not null)
                return RewriteComplexExpression(binExpNew, null, 0, dereferencedType);
            return binExpNew;
        }

        private DataType DataTypeOf(Expression exp)
        {
            return store.TryGetTypeVariable(exp, out var tv)
                ? tv.DataType
                : exp.DataType;
        }

        public override Expression VisitAddress(Address addr)
        {
            return tcr.Rewrite(addr, basePtr, dereferencedType);
        }

        public override Expression VisitConstant(Constant c)
        {
            return tcr.Rewrite(c, basePtr, dereferencedType);
        }

        public override Expression VisitIdentifier(Identifier id)
        {
            id.DataType = store.GetTypeVariable(id).DataType;
            return id;
        }

        public override Expression VisitMemoryAccess(MemoryAccess access)
        {
            var oldBase = this.basePtr;
            Expression ea;
            if (access.EffectiveAddress is SegmentedPointer segptr)
            {
                this.basePtr = segptr.BasePointer;
                ea = Rewrite(segptr.Offset, access.DataType);
            }
            else
            {
                this.basePtr = null;
                // Very aggressive; consider putting it under user
                // control.
                if (TryAggressiveRewriteOfStringPointer(access, out var strConst))
                {
                    ea = strConst;
                }
                else
                {
                    ea = Rewrite(access.EffectiveAddress, access.DataType);
                }
            }
            this.basePtr = oldBase;
            return ea;
        }


        /// <summary>
        /// Try to discover whether a memory access is fetching a (ptr char)
        /// value. If it is, and that pointer is in read-only memory, and
        /// also points to read only memory, assume it's safe to follow
        /// the pointer and read the string it points to.
        /// </summary>
        /// <param name="access">Memory access.</param>
        /// <param name="value">The resulting string constant if all the conditions
        /// above pass.</param>
        /// <returns>True if a string constant was fetched, otherwise false.
        /// </returns>
        private bool TryAggressiveRewriteOfStringPointer(
            MemoryAccess access, 
            [MaybeNullWhen(false)] out StringConstant value)
        {
            value = null;
            if (DataTypeOf(access) is not Pointer ptr)
            {
                return false;
            }
             
            var charType = TypedConstantRewriter.MaybeCharType(ptr.Pointee);
            if (charType is null ||
                !program.TryInterpretAsAddress(access.EffectiveAddress, false, out var addrPtr) ||
                 program.Memory.IsWriteable(addrPtr))
            {
                return false;
            }
            var arch = program.Architecture;
            if (!arch.TryRead(program.Memory, addrPtr, arch.PointerType, out var pch))
            {
                return false;
            }
            var addrString = program.Platform.MakeAddressFromConstant(pch, false);
            if (addrString is null ||
                program.Memory.IsWriteable(addrString.Value))
            {
                return false;
            }
            tcr.PromoteToCString(pch, charType);
            if (!arch.TryCreateImageReader(program.Memory, addrString.Value, out var rdr))
                return false;
            value = rdr.ReadCString(charType, program.TextEncoding);
            return true;
        }

        public override Expression VisitMkSequence(MkSequence seq)
        {
            var newSeq = seq.Expressions.Select(e => Rewrite(e, null)).ToArray();
            if (newSeq.Length == 2)
            {
                var head = newSeq[0];
                var tail = newSeq[1];
                var dtHead = DataTypeOf(head);
                if (dtHead is Pointer || (dtHead.Domain == Domain.Selector))
                {
                    if (seq.Expressions[1] is Constant c)
                    {
                        // reg:CCCC => reg->fldCCCC
                        return RewriteComplexExpression(head, null, c.ToInt32(), dereferencedType);
                    }
                    else
                    {
                        var oldBase = this.basePtr;
                        this.basePtr = head;
                        Expression exp = RewriteComplexExpression(
                            tail,
                            null,
                            0,
                            dereferencedType);
                        this.basePtr = oldBase;
                        return exp;
                    }
                }
                else
                {
                }
            }
            var newSeq2 = new MkSequence(seq.DataType, newSeq);
            store.SetTypeVariable(newSeq2, store.GetTypeVariable(seq));
            return newSeq2;
        }

        public override Expression VisitSegmentedAddress(SegmentedPointer address)
        {
            var oldBase = this.basePtr;
            this.basePtr = null;
            var basePtr = Rewrite(address.BasePointer, null);
            Expression result;
            if (address.Offset is Constant cEa)
            {
                uint uOffset = cEa.ToUInt32();
                result = RewriteComplexExpression(
                    basePtr, Constant.UInt32(uOffset), 0, address.DataType);
            }
            else
            {
                var newOffset = Rewrite(address.Offset, null);
                this.basePtr = basePtr;
                result = RewriteComplexExpression(
                    newOffset,
                    null,
                    0,
                    dereferencedType);
            }
            store.SetTypeVariable(result, store.GetTypeVariable(address));
            this.basePtr = oldBase;
            return result;
        }

        public override Expression VisitSlice(Slice slice)
        {
            var exp = base.VisitSlice(slice);
            if (exp is Slice newSlice && newSlice.Offset == 0)
            {
                //$REVIEW: here we convert SLICE(xxx, yy, 0) to the cast (yy) xxx.
                // Should SLICE(xxx, yy, nn) be cast to (yy) (xxx >> nn) as well?
                var newCast = new Cast(newSlice.DataType, newSlice.Expression);
                store.SetTypeVariable(newCast, store.GetTypeVariable(slice));
                return newCast;
            }
            return exp;
        }

        public override Expression VisitConversion(Conversion conversion)
        {
            var exp = conversion.Expression.Accept(this);
            return new Cast(conversion.DataType, exp);
        }

        private bool TypesAreCompatible(DataType dtSrc, DataType dtDst)
        {
            if (compTypes.Compare(dtSrc, dtDst) == 0)
                return true;
            return unifier.AreCompatible(dtSrc, dtDst);
        }

        public override Expression VisitUnaryExpression(UnaryExpression unary)
        {
            var uNew = base.VisitUnaryExpression(unary);
            var tv = store.GetTypeVariable(unary);
            store.SetTypeVariable(uNew, tv);
            //$TODO: SetTypeVariableExpression shouldn't be needed, SetTypeVariable should do it.
            store.SetTypeVariableExpression(tv, stmCur?.Address, uNew);
            return uNew;
        }
    }
}
