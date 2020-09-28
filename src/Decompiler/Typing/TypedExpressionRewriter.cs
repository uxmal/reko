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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Typing
{
    /// <summary>
    /// Rewrites all the expressions in the program based on the type information provided.
    /// </summary>
    public class TypedExpressionRewriter : InstructionTransformer
    {
        private readonly Program program;
        private readonly Identifier globals;
        private readonly DataTypeComparer compTypes;
        private readonly TypedConstantRewriter tcr;
        private readonly ExpressionEmitter m;
        private readonly Unifier unifier;
        private bool dereferenced;
        private Expression basePtr;
        private DecompilerEventListener eventListener;

        public TypedExpressionRewriter(Program program, DecompilerEventListener eventListener)
        {
            this.program = program;
            this.globals = program.Globals;
            this.eventListener = eventListener;
            this.compTypes = new DataTypeComparer();
            this.tcr = new TypedConstantRewriter(program, eventListener);
            this.m = new ExpressionEmitter();
            this.unifier = new Unifier();
        }

        public void RewriteProgram(Program program)
        {
            int cProc = program.Procedures.Count;
            int i = 0;
            foreach (Procedure proc in program.Procedures.Values)
            {
                eventListener.ShowProgress("Rewriting expressions.", i++, cProc);
                RewriteFormals(proc.Signature);
                foreach (Statement stm in proc.Statements)
                {
                    if (eventListener.IsCanceled())
                        return;
                    try
                    {
                        stm.Instruction = stm.Instruction.Accept(this);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(
                            string.Format("Exception in TypedExpressionRewriter.RewriteProgram: {0} ({1})\r\n{2}", proc, ex.Message, ex.StackTrace));
                        // reset flags after error
                        dereferenced = false;
                    }
                }
            }
        }

        private void RewriteFormals(FunctionType sig)
        {
            if (!sig.HasVoidReturn)
                sig.ReturnValue.DataType = sig.ReturnValue.TypeVariable.DataType;
            if (sig.Parameters != null)
            {
                foreach (Identifier formalArg in sig.Parameters)
                {
                    if (formalArg.TypeVariable != null)
                        formalArg.DataType = formalArg.TypeVariable.DataType;
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
            var tvDst = dst.TypeVariable;
            dst = dst.Accept(this);
            var dtSrc = DataTypeOf(src);
            var dtDst = DataTypeOf(dst);
            if (!TypesAreCompatible(dtSrc, dtDst))
            {
                UnionType uDst = dtDst.ResolveAs<UnionType>();
                UnionType uSrc = dtSrc.ResolveAs<UnionType>();
                if (uDst != null)
                {
                    // ceb = new ComplexExpressionBuilder(dtDst, dtDst, dtSrc, null, dst, null, 0);
                    tvDst.DataType = dtDst;
                    tvDst.OriginalDataType = dtSrc;
                    dst.TypeVariable = tvDst;
                    var ceb = new ComplexExpressionBuilder(dtSrc, null, dst, null, 0);
                    dst = ceb.BuildComplex(false);
                }
                else if (uSrc != null)
                {
                    //throw new NotImplementedException();
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

        public override Instruction TransformCallInstruction(CallInstruction ci)
        {
            var exp = Rewrite(ci.Callee, true);
            return new SideEffect(new Application(exp, VoidType.Instance));
        }

        public override Instruction TransformDeclaration(Declaration decl)
        {
            base.TransformDeclaration(decl);
            decl.Identifier.DataType = decl.Identifier.TypeVariable.DataType;
            return decl;
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

        public Expression Rewrite(Expression expression, bool dereferenced)
        {
            var oldDereferenced = this.dereferenced;
            this.dereferenced = dereferenced;
            var exp = expression.Accept(this);
            this.dereferenced = oldDereferenced;
            return exp;
        }

        public Expression RewriteComplexExpression(Expression complex, Expression index, int offset, bool dereferenced)
        {
            if (index is Constant cOther)
            {
                //$REVIEW: changing this to:
                // offset += cOther.ToInt32() causes a regression.
                // This needs further investigation.
                offset += (int) cOther.ToUInt32();
                index = null;
            }
            var ceb = new ComplexExpressionBuilder(null, basePtr, complex, index, offset);
            return ceb.BuildComplex(dereferenced);
        }

        public override Expression VisitApplication(Application appl)
        {
            return base.VisitApplication(appl);
        }

        public override Expression VisitArrayAccess(ArrayAccess acc)
        {
            if (acc.Array is BinaryExpression bin &&
                bin.Operator == Operator.IAdd &&
                bin.Right is Constant c)
            {
                // (x + C)[...]
                var arrayPtr = Rewrite(bin.Left, false);
                var index = Rewrite(acc.Index, false);
                return RewriteComplexExpression(arrayPtr, index, c.ToInt32(), true);
            }
            else
            {
                // (x)[...]
                var arrayPtr = Rewrite(acc.Array, false);
                var index = Rewrite(acc.Index, false);
                return RewriteComplexExpression(arrayPtr, index, 0, true);
            }
        }

        public override Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            var left = Rewrite(binExp.Left, false);
            var right = Rewrite(binExp.Right, false);

            if (binExp.Operator == Operator.IAdd)
            {
                if (DataTypeOf(left).IsComplex)
                {
                    if (DataTypeOf(right).IsComplex)
                        throw new TypeInferenceException(
                            "Both left and right sides of a binary expression can't be complex types.{0}{1}: {2} vs {3}.",
                            Environment.NewLine, binExp,
                            DataTypeOf(left),
                            DataTypeOf(right));
                    return RewriteComplexExpression(left, right, 0, dereferenced);
                }
                else if (DataTypeOf(right).IsComplex)
                {
                    return RewriteComplexExpression(right, left, 0, dereferenced);
                }
            }
            var binOp = binExp.Operator;
            if (binOp == Operator.Uge)
                binOp = Operator.Ge;
            else if (binOp == Operator.Ugt)
                binOp = Operator.Gt;
            else if (binOp == Operator.Ule)
                binOp = Operator.Le;
            else if (binOp == Operator.Ult)
                binOp = Operator.Lt;
            else if (binOp == Operator.UMul)
                binOp = Operator.IMul;
            else if (binOp == Operator.USub)
                binOp = Operator.ISub;
            else if (binOp == Operator.Shr)
                binOp = Operator.Sar;
            binExp = new BinaryExpression(binOp, binExp.DataType, left, right) { TypeVariable = binExp.TypeVariable };
            program.TypeStore.SetTypeVariableExpression(binExp.TypeVariable, binExp);
            return binExp;
        }

        private static DataType DataTypeOf(Expression exp)
        {
            return exp.TypeVariable != null ? exp.TypeVariable.DataType : exp.DataType;
        }

        public override Expression VisitAddress(Address addr)
        {
            return tcr.Rewrite(addr, dereferenced);
        }

        public override Expression VisitConstant(Constant c)
        {
            return tcr.Rewrite(c, this.dereferenced);
        }

        public override Expression VisitMemoryAccess(MemoryAccess access)
        {
            var oldBase = this.basePtr;
            this.basePtr = null;
            var ea = Rewrite(access.EffectiveAddress, true);
            this.basePtr = oldBase;
            return ea;
        }

        public override Expression VisitMkSequence(MkSequence seq)
        {
            var newSeq = seq.Expressions.Select(e => Rewrite(e, false)).ToArray();
            if (newSeq.Length == 2)
            {
                var head = newSeq[0];
                var tail = newSeq[1];
                var dtHead = DataTypeOf(head);
                if (dtHead is Pointer || (dtHead is PrimitiveType ptHead && ptHead.Domain == Domain.Selector))
                {
                    if (seq.Expressions[1] is Constant c)
                    {
                        // reg:CCCC => reg->fldCCCC
                        return RewriteComplexExpression(head, null, c.ToInt32(), dereferenced);
                    }
                    else
                    {
                        var oldBase = this.basePtr;
                        this.basePtr = head;
                        Expression exp = RewriteComplexExpression(
                            tail,
                            null,
                            0,
                            dereferenced);
                        this.basePtr = oldBase;
                        return exp;
                    }
                }
                else
                {
                }
            }
            return new MkSequence(seq.DataType, newSeq)
            {
                TypeVariable = seq.TypeVariable,
            };
        }

        public override Expression VisitSegmentedAccess(SegmentedAccess access)
        {
            var oldBase = this.basePtr;
            this.basePtr = null;
            var basePtr = Rewrite(access.BasePointer, false);
            Expression result;
            if (access.EffectiveAddress is Constant cEa)
            {
                uint uOffset = cEa.ToUInt32();
                result = RewriteComplexExpression(basePtr, Constant.UInt32(uOffset), 0, true);
            }
            else
            {
                this.basePtr = basePtr;
                result = Rewrite(access.EffectiveAddress, true);
            }
            result.TypeVariable = access.TypeVariable;
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
                return new Cast(newSlice.DataType, newSlice.Expression)
                {
                    TypeVariable = slice.TypeVariable
                };
            }
            return exp;
        }

        private bool TypesAreCompatible(DataType dtSrc, DataType dtDst)
        {
            if (compTypes.Compare(dtSrc, dtDst) == 0)
                return true;
            return unifier.AreCompatible(dtSrc, dtDst);
        }
    }
}
