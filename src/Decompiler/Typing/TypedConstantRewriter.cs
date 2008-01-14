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
	/// Rewrites a constant based on its type.
	/// </summary>
	public class TypedConstantRewriter : IDataTypeVisitor
	{
		private TypeStore store;
		private Identifier globals;
		private StructureType globalVars;
		private Constant c;
		private PrimitiveType pOrig;
		private Expression exp;
		private bool dereferenced;

		public TypedConstantRewriter(TypeStore store, Identifier globals)
		{
			this.store = store;
			this.globals = globals;
			if (globals != null && globals.TypeVariable != null)
			{
				Pointer pGlob = globals.TypeVariable.DataType as Pointer;
				if (pGlob != null)
				{
					object o = store.ResolvePossibleTypeVar(pGlob.Pointee);
					this.globalVars = (StructureType) o;
				}
			}
		}

		public Expression Rewrite(Constant c, bool dereferenced)
		{
			this.c = c;
			DataType dtInferred = store.ResolvePossibleTypeVar(c.TypeVariable.DataType);
			pOrig = c.TypeVariable.OriginalDataType as PrimitiveType;
			this.dereferenced = dereferenced;
			dtInferred.Accept(this);
			return exp;
		}

		private void Return(Expression exp)
		{
			this.exp = exp; 
		}

		public void VisitArray(ArrayType at)
		{
			throw new NotImplementedException();
		}

		public void VisitEquivalenceClass(EquivalenceClass eq)
		{
			throw new NotImplementedException();
		}

		public void VisitFunctionType(FunctionType ft)
		{
			throw new NotImplementedException();
		}

		public void VisitMemberPointer(MemberPointer memptr)
		{
			Return(c);
		}

		public void VisitPointer(Pointer ptr)
		{
			StructureField f = globalVars.Fields.AtOffset(c.AsInt32());
			if (f == null)
				throw new InvalidOperationException(string.Format("Expected a global variable with address 0x{0:X8}", c.AsInt32()));

			Expression e = new FieldAccess(ptr.Pointee, new Dereference(null, globals), f.Name);
			if (dereferenced)
			{
				e.DataType = ptr.Pointee;
			}
			else
			{
				e = new UnaryExpression(Operator.addrOf, ptr, e);
			}
			Return(e);
		}

		public void VisitPrimitive(PrimitiveType pt)
		{
			if (pt.Domain == Domain.Real && pOrig.IsIntegral)
			{
				Return(Constant.RealFromBitpattern(pt, Convert.ToInt64(c.Value)));
			}
			else
			{
				Return(c);
			}
		}

		public void VisitStructure(StructureType str)
		{
			throw new NotImplementedException();
		}


		public void VisitUnion(UnionType ut)
		{
			// A constant can't have a union value, so we coerce it to the appropriate type.
			UnionAlternative a = ut.FindAlternative(pOrig);
			if (a == null)
				throw new TypeInferenceException("Couldn't find alternative of type {0} in {1}", pOrig, ut);
			c.TypeVariable.DataType = pOrig;
			Return(c);
		}


		public void VisitTypeVar(TypeVariable tv)
		{
			throw new NotImplementedException();
		}

		public void VisitUnknownType(UnknownType ut)
		{
			throw new NotImplementedException();
		}
	}
}
