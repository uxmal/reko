#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.Diagnostics;
using System.IO;

namespace Reko.Typing
{
    /// <summary>
    /// Rewrites all the expressions in the program based on the type information provided.
    /// </summary>
    [Obsolete("",false)]
    public class TypedExpressionRewriter : InstructionTransformer //, IDataTypeVisitor
    {
        private Program program;
        private Platform platform;
        private TypeStore store;
        private Identifier globals;
        private DataTypeComparer compTypes;
        private TypedConstantRewriter tcr;
        private ExpressionEmitter m;
        private Unifier unifier;
        private Expression basePointer;

        private bool dereferenced;

        public TypedExpressionRewriter(Program program)
        {
            this.program = program;
            this.platform = program.Platform;
            this.store = program.TypeStore;
            this.globals = program.Globals;
            this.compTypes = new DataTypeComparer();
            this.tcr = new TypedConstantRewriter(program);
            this.m = new ExpressionEmitter();
            this.unifier = new Unifier();
        }

        public DataType DataTypeOf(Expression e)
        {
            if (e.TypeVariable != null)
                return e.TypeVariable.DataType;
            return e.DataType;
        }

        /// <summary>
        /// Builds an ArrayAccess expression.
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="e"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Expression MkArrayAccess(DataType elementType, Expression e, int idx)
        {
            return new ArrayAccess(elementType, e, Constant.Word32(idx));
        }

        private void RewriteFormals(ProcedureSignature sig)
        {
            if (sig.ReturnValue != null)
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

        public void RewriteProgram(Program program)
        {
            foreach (Procedure proc in program.Procedures.Values)
            {
                RewriteFormals(proc.Signature);
                foreach (Statement stm in proc.Statements)
                {
                    try
                    {
                        stm.Instruction = stm.Instruction.Accept(this);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(
                            string.Format("Exception in TypedExpressionRewriter.RewriteProgram: {0} ({1})\r\n{2}", proc, ex.Message, ex.StackTrace));
                    }
                }
            }
        }

        #region InstructionTransformer methods //////////////////////

        public override Expression VisitArrayAccess(ArrayAccess acc)
        {
#if !NEW
            var arr = Rewrite(acc.Array, false);
            var idx = Rewrite(acc.Index, false);
            var result = new ArrayAccess(acc.TypeVariable.DataType, arr, idx);
            return result;
#else
            var arrayPtr = acc.Array.Accept(this);
            var elemType = arrayPtr.DataType.ResolveAs<ArrayType>().ElementType;
            //$TODO: what if index is complex?
            var index = TypedMemoryExpressionRewriter.RescaleIndex(acc.Index, elemType);
            return new ArrayAccess(elemType, arrayPtr, index);
#endif
        }

        private Expression NormalizeArrayPointer(Expression arrayExp)
        {
            var u = arrayExp as UnaryExpression;
            if (u == null)
                return arrayExp;
            if (u.Operator != Operator.AddrOf)
                return arrayExp;
            var a = u.Expression as ArrayAccess;
            if (a == null)
                return arrayExp;
            return a.Array;
        }

        public override Instruction TransformAssignment(Assignment a)
        {
            return MakeAssignment(a.Dst, a.Src);
        }

        public Instruction MakeAssignment(Expression dst, Expression src)
        {
            var tvDst = dst.TypeVariable;
            var tvSrc = src.TypeVariable;
            src = src.Accept(this);
            DataType dtSrc = DataTypeOf(src);
            dst = dst.Accept(this);
            DataType dtDst = DataTypeOf(dst);
            if (!TypesAreCompatible(dtSrc, dtDst))
            {
                UnionType uDst = dtDst.ResolveAs<UnionType>();
                UnionType uSrc = dtSrc.ResolveAs<UnionType>();
                if (uDst != null)
                {
                    var ceb = new ComplexExpressionBuilder(dtDst, dtDst, dtSrc, null, dst, null, 0);
                    dst = ceb.BuildComplex(false);
                }
                else if (uSrc != null)
                {
                    var ceb = new ComplexExpressionBuilder(dtSrc, dtSrc, dtDst, null, src, null, 0);
                    src = ceb.BuildComplex(false);
                }
                else
                    throw new NotImplementedException(string.Format("{0} [{1}] = {2} [{3}] (in assignment {4} = {5}) not supported.", tvDst, dtDst, tvSrc, dtSrc, dst, src));
            }
            var idDst = dst as Identifier;
            if (idDst != null)
                return new Assignment(idDst, src);
            else
                return new Store(dst, src);
        }

        private bool TypesAreCompatible(DataType dtSrc, DataType dtDst)
        {
            if (compTypes.Compare(dtSrc, dtDst) == 0)
                return true;
            return unifier.AreCompatible(dtSrc, dtDst);
        }

        public override Instruction TransformCallInstruction(CallInstruction ci)
        {
            //var proc = ci.Callee.Accept(new TypedMemoryExpressionRewriter(program));
            return new SideEffect(
                new Application(ci.Callee, VoidType.Instance));
        }

        public override Instruction TransformStore(Store store)
        {
            return MakeAssignment(store.Dst, store.Src);
        }

        public Expression Rewrite(Expression exp, bool dereferenced)
        {
            var old = this.dereferenced;
            this.dereferenced = dereferenced;
            exp = exp.Accept(this);
            this.dereferenced = old;
            return exp;
        }

        /// <summary>
        /// Rewrites an expression of the type (a + b)
        /// </summary>
        /// <param name="binExp"></param>
        /// <remarks>
        /// If [[a]] is a complex type, it's with high likelihood a pointer type. If this is the case,
        /// we want to return something like &(*a).b. If this sum is in a Mem context, the & is removed to yield
        /// (*a).b. 
        /// <para>
        /// If [[a]] is a memptr(ptr(A), x), and b is a constant, then we want to return something like &A::b
        /// </para>
        /// </remarks>
        /// <returns>The rewritten expression</returns>
		public override Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            var left = binExp.Left.Accept(this);
            var right = binExp.Right.Accept(this);
            DataType dtLeft = DataTypeOf(binExp.Left);
            DataType dtRight = DataTypeOf(binExp.Right);
            if (binExp.Operator == Operator.IAdd && dtLeft.IsComplex)
            {
                return TransformComplexSum(binExp, dtLeft, dtRight);
            }
            else
            {
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
                store.SetTypeVariableExpression(binExp.TypeVariable, binExp);
                return binExp;
            }
        }

        private Expression TransformComplexSum(BinaryExpression binExp, DataType dtLeft, DataType dtRight)
        {
            if (dtRight.IsComplex)
                throw new TypeInferenceException(
                    "Both left and right sides of a binary expression can't be complex types.{0}{1}: {2}:[{3}] {4}:[{5}].",
                    Environment.NewLine, binExp,
                    binExp.Left.TypeVariable, dtLeft,
                    binExp.Right.TypeVariable, dtRight);
            Constant c = binExp.Right as Constant;
            if (c != null)
            {
                ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                    binExp.TypeVariable.DataType,
                    dtLeft,
                    binExp.Left.TypeVariable.OriginalDataType,
                    basePointer,
                    binExp.Left,
                    null,
                    StructureField.ToOffset(c));
                return ceb.BuildComplex(dereferenced);
            }
            return binExp;
        }

        public override Expression VisitConstant(Constant c)
        {
            return tcr.Rewrite(c, dereferenced);
        }

        public override Instruction TransformDeclaration(Declaration decl)
        {
            base.TransformDeclaration(decl);

            decl.Identifier.DataType = decl.Identifier.TypeVariable.DataType;
            return decl;
        }

        public override Expression VisitMemoryAccess(MemoryAccess access)
        {
            var oldBase = this.basePointer;
            this.basePointer = null;
            var exp = Rewrite(access.EffectiveAddress, true);
            this.basePointer = oldBase;
            return exp;
        }

        public override Expression VisitSegmentedAccess(SegmentedAccess access)
        {
            var oldBase = this.basePointer;
            this.basePointer = access.BasePointer;
            var exp = Rewrite(access.EffectiveAddress, true);
            this.basePointer = oldBase;
            return exp;
        }

        public override Expression VisitMkSequence(MkSequence seq)
        {
            var head = seq.Head.Accept(this);
            var tail = seq.Tail.Accept(this);
            Constant c = seq.Tail as Constant;
            var ptHead = head.TypeVariable.DataType as PrimitiveType;
            if (head.TypeVariable.DataType is Pointer || (ptHead != null && ptHead.Domain == Domain.Selector))
            {
                if (c != null)
                {
                    var seg = head.TypeVariable.DataType.ResolveAs<Pointer>().Pointee;
                    var fa = new FieldAccess(
                        seq.TypeVariable.DataType.ResolveAs<Pointer>().Pointee,
                        new Dereference(head.TypeVariable.DataType, head),
                        seg.ResolveAs<StructureType>().Fields.AtOffset(c.ToInt32()));
                    return fa;
                }
                else
                {
                    var ceb = new ComplexExpressionBuilder(
                        seq.TypeVariable.DataType,
                        seq.TypeVariable.DataType,
                        seq.TypeVariable.OriginalDataType,
                        head,
                        new MemberPointerSelector(seq.DataType, head, tail),
                        null,
                        0);
                    return ceb.BuildComplex(dereferenced);
                }
            }
            else
            {
            }
            return new MkSequence(seq.DataType, head, tail);
        }

#endregion
    }

    /// <summary>
    /// Rewrites all the expressions in the program based on the type information provided.
    /// </summary>
    public class TypedExpressionRewriter2 : InstructionTransformer
    {
        private Program program;
        private Platform platform;
        private TypeStore store;
        private Identifier globals;
        private DataTypeComparer compTypes;
        private TypedConstantRewriter tcr;
        private ExpressionEmitter m;
        private Unifier unifier;
        private bool dereferenced;
        private Expression basePtr;

        public TypedExpressionRewriter2(Program program)
        {
            this.program = program;
            this.platform = program.Platform;
            this.store = program.TypeStore;
            this.globals = program.Globals;
            this.compTypes = new DataTypeComparer();
            this.tcr = new TypedConstantRewriter(program);
            this.m = new ExpressionEmitter();
            this.unifier = new Unifier();
        }

        public void RewriteProgram(Program program)
        {
            foreach (Procedure proc in program.Procedures.Values)
            {
                RewriteFormals(proc.Signature);
                foreach (Statement stm in proc.Statements)
                {
                    try
                    {
                        stm.Instruction = stm.Instruction.Accept(this);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(
                            string.Format("Exception in TypedExpressionRewriter.RewriteProgram: {0} ({1})\r\n{2}", proc, ex.Message, ex.StackTrace));
                    }
                }
            }
        }

        private void RewriteFormals(ProcedureSignature sig)
        {
            if (sig.ReturnValue != null)
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
            dst = dst.Accept(this);
            var dtSrc = DataTypeOf(src);
            var dtDst = DataTypeOf(dst);
            if (!TypesAreCompatible(dtSrc, dtDst))
            {
                UnionType uDst = dtDst.ResolveAs<UnionType>();
                UnionType uSrc = dtSrc.ResolveAs<UnionType>();
                if (uDst != null)
                {
                    var ceb = new ComplexExpressionBuilder(dtDst, dtDst, dtSrc, null, dst, null, 0);
                    dst = ceb.BuildComplex(false);
                }
                else if (uSrc != null)
                {
                    var ceb = new ComplexExpressionBuilder(dtSrc, dtSrc, dtDst, null, src, null, 0);
                    src = ceb.BuildComplex(false);
                }
                else
                    throw new NotImplementedException(string.Format("{2} [{0}] = {3} [{1}] not supported.", dtDst, dtSrc, dst, src));
            }
            var idDst = dst as Identifier;
            if (idDst != null)
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
            var cOther = index as Constant;
            if (cOther != null)
            {
                offset += cOther.ToInt32();
                index = null;
            }
            var ceb = new ComplexExpressionBuilder2(null, basePtr, complex, index, offset);
            return ceb.BuildComplex(dereferenced);
        }

        public override Expression VisitArrayAccess(ArrayAccess acc)
        {
            BinaryExpression bin;
            Constant c;
            if (acc.Array.As(out bin) &&
                bin.Operator == Operator.IAdd &&
                bin.Right.As(out c))
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
            store.SetTypeVariableExpression(binExp.TypeVariable, binExp);
            return binExp;
        }

        private static DataType DataTypeOf(Expression exp)
        {
            return exp.TypeVariable != null ? exp.TypeVariable.DataType : exp.DataType;
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
            var head = Rewrite(seq.Head, dereferenced);
            var tail = Rewrite(seq.Tail, dereferenced);
            Constant c = seq.Tail as Constant;
            var ptHead = DataTypeOf( head) as PrimitiveType;
            if (head.TypeVariable.DataType is Pointer || (ptHead != null && ptHead.Domain == Domain.Selector))
            {
                if (c != null)
                {
                    var seg = DataTypeOf(head).ResolveAs<Pointer>().Pointee;
                    var dtSeq = DataTypeOf(seq).ResolveAs<Pointer>().Pointee;
                    var deref = new Dereference(DataTypeOf(head), head);
                    var field = seg.ResolveAs<StructureType>().Fields.AtOffset(c.ToInt32());
                    var fa = new FieldAccess(dtSeq, deref, field);
                    return fa;
                }
                else
                {
                    var ceb = new ComplexExpressionBuilder(
                        seq.TypeVariable.DataType,
                        seq.TypeVariable.DataType,
                        seq.TypeVariable.OriginalDataType,
                        head,
                        new MemberPointerSelector(seq.DataType,
                            new Dereference(head.DataType, head),
                        tail),
                        null,
                        0);
                    var x = ceb.BuildComplex(dereferenced);
                    return x;
                }
            }
            else
            {
            }
            return new MkSequence(seq.DataType, head, tail);
        }

        public override Expression VisitSegmentedAccess(SegmentedAccess access)
        {
            var oldBase = this.basePtr;
            this.basePtr = null;
            var basePtr = Rewrite(access.BasePointer, false);
            Constant cEa;
            Expression result;
            if (access.EffectiveAddress.As(out cEa))
            {
                uint uOffset = cEa.ToUInt32();
                result = RewriteComplexExpression(basePtr, Constant.UInt32(uOffset), 0, true);
            }
            else
            {
                this.basePtr = basePtr;
                result = Rewrite(access.EffectiveAddress, true);
            }
            this.basePtr = oldBase;
            return result;
        }

        private bool TypesAreCompatible(DataType dtSrc, DataType dtDst)
        {
            if (compTypes.Compare(dtSrc, dtDst) == 0)
                return true;
            return unifier.AreCompatible(dtSrc, dtDst);
        }
    }
}
