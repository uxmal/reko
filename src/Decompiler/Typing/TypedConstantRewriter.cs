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
        private Program program;
        private Platform platform;
		private TypeStore store;
		private Identifier globals;
		private Constant c;
		private PrimitiveType pOrig;
		private bool dereferenced;

		public TypedConstantRewriter(Program program)
		{
            this.program = program;
            this.platform = program.Platform;
            this.store = program.TypeStore;
            this.globals = program.Globals;
		}

        /// <summary>
        /// Rewrites a machine word constant depending on its data type.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="dereferenced"></param>
        /// <returns></returns>
        public Expression Rewrite(Constant c, bool dereferenced)
        {
            this.c = c;
            DataType dtInferred = c.TypeVariable.DataType.ResolveAs<DataType>();
            this.pOrig = c.TypeVariable.OriginalDataType as PrimitiveType;
            this.dereferenced = dereferenced;
            return dtInferred.Accept(this);
        }

        public Expression Rewrite(Address addr, bool dereferenced)
        {
            this.c = Constant.UInt32(addr.ToUInt32());  //$BUG: won't work for x86.
            var dtInferred = addr.TypeVariable.DataType.ResolveAs<DataType>();
            this.pOrig = addr.TypeVariable.OriginalDataType as PrimitiveType;
            this.dereferenced = dereferenced;
            return dtInferred.Accept(this);
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
                        return pGlob.Pointee.ResolveAs<StructureType>();
                    }
                    pGlob = globals.DataType as Pointer;
                    if (pGlob != null)
                    {
                        return pGlob.Pointee as StructureType;
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
        
        public Expression VisitArray(ArrayType at)
		{
			throw new ArgumentException("Constants cannot have array values yet.");
		}

        public Expression VisitCode(CodeType c)
        {
            throw new NotImplementedException();
        }

        public Expression VisitEnum(EnumType e)
        {
            string name;
            if (e.Members.TryGetValue(c.ToInt64(), out name))
                return new Identifier(name, e, TemporaryStorage.None);
            return new Cast(e, c);
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
			Expression baseExpr = new ScopeResolution(baseType);

            var dt = memptr.Pointee.ResolveAs<DataType>();
            var f = EnsureFieldAtOffset(baseType, dt, c.ToInt32());
            Expression ex = new FieldAccess(memptr.Pointee, baseExpr, f.Name);
			if (dereferenced)
			{
				ex.DataType = memptr.Pointee;
			}
			else
			{
                var array = f.DataType as ArrayType;
                if (array != null)
                {
                    ex.DataType = new MemberPointer(p, array.ElementType, platform.PointerType.Size);
                }
                else
                {
                    ex = new UnaryExpression(Operator.AddrOf, memptr, ex);
                }
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
                // Null pointer.
                if (c.IsZero)
                {
                    var np = Address.Create(ptr, 0);
                    np.TypeVariable = c.TypeVariable;
                    np.DataType = c.DataType;
                    return np;
                }

                // An invalid pointer -- often used as sentinels in code.
                if (!program.Image.IsValidLinearAddress(c.ToUInt64()))
                {
                    //$TODO: probably should use a reinterpret_cast here.
                    var ce = new Cast(c.DataType, c);
                    ce.TypeVariable = c.TypeVariable;
                    ce.DataType = ptr;
                    return ce;
                }
                
                var dt = ptr.Pointee.ResolveAs<DataType>();
                StructureField f = EnsureFieldAtOffset(GlobalVars, dt, c.ToInt32());
                var ptrGlobals = new Pointer(GlobalVars, platform.PointerType.Size);
                e = new FieldAccess(ptr.Pointee, new Dereference(ptrGlobals, globals), f.Name);
                if (dereferenced)
                {
                    e.DataType = ptr.Pointee;
                }
                else
                {
                    var array = dt as ArrayType;
                    if (array != null) // C language rules 'promote' arrays to pointers.
                    {
                        //$BUG: no factory?
                        e.DataType = new Pointer(array.ElementType, platform.PointerType.Size);
                    }
                    else
                    {
                        e = new UnaryExpression(Operator.AddrOf, ptr, e);
                    }
                }
            }
			return e;
		}

        private StructureField EnsureFieldAtOffset(StructureType str, DataType dt, int offset)
        {
            StructureField f = str.Fields.AtOffset(offset);
            if (f == null)
            {
                //$TODO: overlaps and conflicts.
                //$TODO: strings.
                f = new StructureField(offset, dt);
                str.Fields.Add(f);
            }
            return f;
        }

		public Expression VisitPrimitive(PrimitiveType pt)
		{
			if (pt.Domain == Domain.Real && (pOrig.Domain & Domain.Integer) != 0)
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
            return typeref.Referent.Accept(this);
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
