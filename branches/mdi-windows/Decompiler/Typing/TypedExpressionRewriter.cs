/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Rewrites all the expressions in the program based on the type information provided.
	/// </summary>
	public class TypedExpressionRewriter : InstructionTransformer //, IDataTypeVisitor
	{
		private TypeStore store;
        private Identifier globals;
		private DataTypeComparer compTypes;
		private TypedConstantRewriter tcr;

		public TypedExpressionRewriter(TypeStore store, Identifier globals)
		{
			this.store = store;
            this.globals = globals;
			this.compTypes = new DataTypeComparer();
			this.tcr = new TypedConstantRewriter(store, globals);
		}

		public UnionType AsUnion(DataType dt)
		{
			dt = store.ResolvePossibleTypeVar(dt);
			return dt as UnionType;
		}


		public DataType DataTypeOf(Expression e)
		{
			if (e.TypeVariable != null)
				return e.TypeVariable.DataType;
			return e.DataType;
		}


		/// <summary>
		/// Creates an "addressof" expression.
		/// </summary>
		/// <param name="dt">Datatype of expression</param>
		/// <param name="e">expression to take address of</param>
		/// <returns></returns>
		public Expression MkAddrOf(DataType dt, Expression e)
		{
			// &*ptr == ptr
			Dereference d = e as Dereference;
			if (d != null)
				return d.Expression;
			
			// *&a[i] = a + i;
			// *&a[0] = a
			ArrayAccess acc = e as ArrayAccess;
			if (acc != null)
			{
				Constant index = acc.Index as Constant;
				if (index != null && index.ToInt32() == 0)
					return acc.Array;
			}
			return new UnaryExpression(Operator.addrOf, dt, e);
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
            if (sig.FormalArguments != null)
            {
                foreach (Identifier formalArg in sig.FormalArguments)
                {
                    formalArg.DataType = formalArg.TypeVariable.DataType;
                }
            }
        }

        public void RewriteProgram(Program prog)
		{
			{//$DEBUG
				System.IO.StringWriter sb = new System.IO.StringWriter();
				prog.TypeStore.Write(sb);
				System.Diagnostics.Debug.WriteLine(sb.ToString());
			}

			foreach (Procedure proc in prog.Procedures.Values)
			{
                RewriteFormals(proc.Signature);
				foreach (Block b in proc.RpoBlocks)
				{
					foreach (Statement stm in b.Statements)
					{
						stm.Instruction = stm.Instruction.Accept(this);
					}
				}
			}
		}


		#region InstructionTransformer methods //////////////////////

        public override Expression TransformArrayAccess(ArrayAccess acc)
        {
            acc.Array = NormalizeArrayPointer(acc.Array.Accept(this));
            acc.Index = acc.Index.Accept(this);
            return acc;
        }

        private Expression NormalizeArrayPointer(Expression arrayExp)
        {
            UnaryExpression u = arrayExp as UnaryExpression;
            if (u == null)
                return arrayExp;
            if (u.op != Operator.addrOf)
                return arrayExp;
            ArrayAccess a = u.Expression as ArrayAccess;
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
            TypeVariable tvDst = dst.TypeVariable;
            TypeVariable tvSrc = src.TypeVariable;
			src = src.Accept(this);
			DataType dtSrc = DataTypeOf(src);
			dst = dst.Accept(this);
			DataType dtDst = DataTypeOf(dst);
			if (compTypes.Compare(dtSrc, dtDst) != 0)
			{
				UnionType uDst = AsUnion(dtDst);
				UnionType uSrc = AsUnion(dtSrc);
				if (uDst != null)
				{
					ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(dtDst, dtDst, dtSrc, null, dst, null, 0);
					dst = ceb.BuildComplex();
				}
				else if (uSrc != null)
				{
					ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(dtSrc, dtSrc, dtDst, null, src, null, 0);
					src = ceb.BuildComplex();
				}
				else
					throw new NotImplementedException(string.Format("{0} [{1}] = {2} [{3}] (in assignment {4} = {5}) not supported.", tvDst, dtDst, tvSrc, dtSrc, dst, src));
			}
			return new Decompiler.Core.Absyn.AbsynAssignment(dst, src);
		}
		

		public override Instruction TransformStore(Store store)
		{
			return MakeAssignment(store.Dst, store.Src);
		}

        /// <summary>
        /// Rewrites an expression of the type (a + b)
        /// </summary>
        /// <param name="binExp"></param>
        /// <remarks>
        /// If [[a]] is a complex type, it's with high likelihood a pointer type. If this is the case,
        /// we want to return something like &(*a).b. If this sum is in a Mem context, the & is removed to yield
        /// (*a).b. <para>If [[a]] is a memptr(ptr(A), x), and b is a constant, then we want to return something like &A::b
        /// </para>
        /// </remarks>
        /// <returns>The rewritten expression</returns>
		public override Expression TransformBinaryExpression(BinaryExpression binExp)
		{
			base.TransformBinaryExpression(binExp);

			DataType dtLeft = DataTypeOf(binExp.Left);
			DataType dtRight = DataTypeOf(binExp.Right);
            if (binExp.op == Operator.add)
            {
                return TransformSum(binExp, dtLeft, dtRight);
            }
            else
            {
                return binExp;
            }
		}

        private Expression TransformSum(BinaryExpression binExp, DataType dtLeft, DataType dtRight)
        {
            if (dtLeft.IsComplex)
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
                        null,
                        binExp.Left,
                        null,
                        StructureField.ToOffset(c));
                    return ceb.BuildComplex();
                }
            }
            return binExp;
        }

		public override Expression TransformConstant(Constant c)
		{
			return tcr.Rewrite(c, false);
		}

		public override Instruction TransformDeclaration(Declaration decl)
		{
			base.TransformDeclaration(decl);
			
			decl.Identifier.DataType = decl.Identifier.TypeVariable.DataType;
			return decl;
		}

		public override Expression TransformMemoryAccess(MemoryAccess access)
		{
			TypedMemoryExpressionRewriter tmer = new TypedMemoryExpressionRewriter(store, globals);
			return tmer.Rewrite(access);
		}

		public override Expression TransformSegmentedAccess(SegmentedAccess access)
		{
			TypedMemoryExpressionRewriter tmer = new TypedMemoryExpressionRewriter(store, globals);
			return tmer.Rewrite(access);
		}

        public override Expression TransformMkSequence(MkSequence seq)
        {
            seq.Head = seq.Head.Accept(this);
            seq.Tail = seq.Tail.Accept(this);
            Constant c = seq.Tail as Constant;
            if (c == null)
                return seq;
            if (seq.Head.DataType is Pointer)
            {
                    ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                        seq.TypeVariable.DataType,
                        seq.Head.DataType,
                        seq.Head.TypeVariable.OriginalDataType,
                        null,
                        seq.Head,
                        null,
                        StructureField.ToOffset(c));
                    return ceb.BuildComplex();

            }
            return seq;
        }

		#endregion
	}
}
