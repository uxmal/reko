/* 
 * Copyright (C) 1999-2008 John Källén.
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
		private Program prog;
		private TypeStore store;
		private DataTypeComparer compTypes;
		private TypedConstantRewriter tcr;

		public TypedExpressionRewriter(TypeStore store, Program prog)
		{
			this.store = store;
			this.prog = prog;
			this.tcr = new TypedConstantRewriter(store, prog.Globals);
			this.compTypes = new DataTypeComparer();
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
				if (index != null && Convert.ToInt32(index.Value) == 0)
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

		public void RewriteProgram()
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
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

		public override Instruction TransformAssignment(Assignment a)
		{
			return MakeAssignment(a.Dst, a.Src);
		}

		public Instruction MakeAssignment(Expression dst, Expression src)
		{
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
					ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(dtDst, dtSrc, dst, 0);
					dst = ceb.BuildComplex();
				}
				else if (uSrc != null)
				{
					ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(dtSrc, dtDst, src, 0);
					src = ceb.BuildComplex();
				}
				else
					throw new NotImplementedException(string.Format("[{0}] = [{1}] (in assignment {2} = {3}) not supported.", dtDst, dtSrc, dst, src));
			}
			return new Decompiler.Core.Absyn.AbsynAssignment(dst, src);
		}
		

		public override Instruction TransformStore(Store store)
		{
			return MakeAssignment(store.Dst, store.Src);
		}

		public override Expression TransformBinaryExpression(BinaryExpression binExp)
		{
			base.TransformBinaryExpression(binExp);

			DataType dtLeft = DataTypeOf(binExp.Left);
			DataType dtRight = DataTypeOf(binExp.Right);
			if (dtLeft.IsComplex)
			{
				if (dtRight.IsComplex)
					throw new TypeInferenceException("Both left and right sides of a binary expression can't be complex types.");
				Constant c = (Constant) binExp.Right;
				ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
					dtLeft, 
					binExp.Left.TypeVariable.OriginalDataType,
					new Dereference(null, binExp.Left),
					Convert.ToInt32(c.Value));
				return MkAddrOf(null, ceb.BuildComplex());
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
			
			decl.Id.DataType = decl.Id.TypeVariable.DataType;
			return decl;
		}

		public override Expression TransformMemoryAccess(MemoryAccess access)
		{
			TypedMemoryExpressionRewriter tmer = new TypedMemoryExpressionRewriter(store, prog.Globals);
			return tmer.Rewrite(access);
		}

		public override Expression TransformSegmentedAccess(SegmentedAccess access)
		{
			return access;
		}

		#endregion
	}
}
