#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Rewrites a constant based on its type.
	/// </summary>
	public class TypedConstantRewriter : IDataTypeVisitor<Expression>
	{
		private TypeStore store;
		private Identifier globals;
		private Constant c;
		private PrimitiveType pOrig;
		private bool dereferenced;

		public TypedConstantRewriter(TypeStore store, Identifier globals)
		{
			this.store = store;
			this.globals = globals;
		}

		private StructureType GlobalVars
		{
            get
            {
                if (globals != null && globals.TypeVariable != null)
                {
                    var pGlob = globals.TypeVariable.DataType as Pointer;
                    if (pGlob != null)
                    {
                        object o = store.ResolvePossibleTypeVar(pGlob.Pointee);
                        return (StructureType)o;
                    }
                }
                return null;
            }
		}

        //$REVIEW: special cased code; we need to handle segments appropriately and remove this function.
        private bool IsSegmentPointer(Pointer ptr)
        {
            EquivalenceClass eq = ptr.Pointee as EquivalenceClass;
            if (eq == null)
                return false;
            StructureType str = eq.DataType as StructureType;
            if (str == null)
                return false;
            return str.IsSegment;
        }

		public Expression Rewrite(Constant c, bool dereferenced)
		{
			this.c = c;
			DataType dtInferred = store.ResolvePossibleTypeVar(c.TypeVariable.DataType);
			pOrig = c.TypeVariable.OriginalDataType as PrimitiveType;
			this.dereferenced = dereferenced;
			return dtInferred.Accept(this);
		}

        public Expression VisitArray(ArrayType at)
		{
			throw new NotImplementedException();
		}

        public Expression VisitEquivalenceClass(EquivalenceClass eq)
		{
			throw new NotImplementedException();
		}

		public Expression VisitFunctionType(FunctionType ft)
		{
			throw new NotImplementedException();
		}

		public Expression VisitMemberPointer(MemberPointer memptr)
		{
			// The constant is a member pointer.
			Pointer p = (Pointer) memptr.BasePointer;
			EquivalenceClass eq = (EquivalenceClass) p.Pointee;
			StructureType baseType = (StructureType) eq.DataType;
			Expression baseExpr = new ScopeResolution(baseType, baseType.Name);

			ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                c.DataType,
				baseType, baseType,
                null,
                null,
                null,
                StructureField.ToOffset(c));
			Expression ex = ceb.BuildComplex();
			if (dereferenced)
			{
				ex.DataType = memptr.Pointee;
			}
			else
			{
				ex = new UnaryExpression(Operator.AddrOf, memptr, ex);
			}
            return ex;
		}

		public Expression VisitPointer(Pointer ptr)
		{
			Expression e = c;
            if (IsSegmentPointer(ptr))
            {
                return e;
            } 
            else if (GlobalVars != null)
            {
                StructureField f = GlobalVars.Fields.AtOffset(c.ToInt32());
                if (f == null)
                    throw new InvalidOperationException(string.Format("Expected a global variable with address 0x{0:X8}.", c.ToInt32()));

                var ptrGlobals = new Pointer(GlobalVars, 4);    //$REVIEW: hard coded ptr size
                e = new FieldAccess(ptr.Pointee, new Dereference(ptrGlobals, globals), f.Name);
                if (dereferenced)
                {
                    e.DataType = ptr.Pointee;
                }
                else
                {
                    e = new UnaryExpression(Operator.AddrOf, ptr, e);
                }
            }
			return e;
		}

		public Expression VisitPrimitive(PrimitiveType pt)
		{
			if (pt.Domain == Domain.Real && pOrig.IsIntegral)
			{
				return(Constant.RealFromBitpattern(pt, c.ToInt64()));
			}
			else
			{
                return c;
			}
		}

        public Expression VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

		public Expression VisitStructure(StructureType str)
		{
			throw new NotImplementedException();
		}

		public Expression VisitUnion(UnionType ut)
		{
			// A constant can't have a union value, so we coerce it to the appropriate type.
			UnionAlternative a = ut.FindAlternative(pOrig);
            if (a == null)
            {
                // This is encountered when the original type is a [[word]] but
                // the alternatives are, say, [[int32]] and [[uint32]]. In this case
                // we pick an arbitrary type and go with it.
                //$REVIEW: should emit a warning.
                a = ut.Alternatives.Values[0];
            }
			c.TypeVariable.DataType = a.DataType;
            return c;
		}

        public Expression VisitTypeReference(TypeReference typeref)
        {
            throw new NotImplementedException();
        }

		public Expression VisitTypeVariable(TypeVariable tv)
		{
			throw new NotImplementedException();
		}

        public Expression VisitUnknownType(UnknownType ut)
		{
			throw new NotImplementedException();
		}

        public Expression VisitVoidType(VoidType vt)
        {
            throw new NotImplementedException();
        }
	}
}
