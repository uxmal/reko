/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Typing
{
	/// <summary>
	/// Performs type unification, returning a general unifier for the two parameters.
	/// </summary>
	public class Unifier
	{
		private TypeFactory factory;

		public Unifier(TypeFactory factory)
		{
			this.factory = factory;
		}

		public bool AreCompatible(DataType a, DataType b)
		{
			if (a == null || b == null)
				return false;

			PrimitiveType pa = a as PrimitiveType;
			PrimitiveType pb = b as PrimitiveType;
			if (pa != null && pb != null)
			{
				if (pa.Size != pb.Size)
					return false;
				return (pa.Domain &  pb.Domain) != 0;
			}

			TypeVariable tva = a as TypeVariable;
			TypeVariable tvb = b as TypeVariable;
			if (tva != null && tvb != null)
			{
				return tva.Number == tvb.Number;
			}

			EquivalenceClass eqA = a as EquivalenceClass;
			EquivalenceClass eqB = b as EquivalenceClass;
			if (eqA != null && eqB != null)
			{
				return eqA.Number == eqB.Number;
			}

			Pointer ptrA = a as Pointer;
			Pointer ptrB = b as Pointer;
			if (ptrA != null)
				return IsCompatibleWithPointer(ptrA, b);
			if (ptrB != null)
				return IsCompatibleWithPointer(ptrB, a);

			MemberPointer mpA = a as MemberPointer;
			MemberPointer mpB = b as MemberPointer;
			if (mpA != null)
				return IsCompatibleWithMemberPointer(mpA, b);
			if (mpB != null)
				return IsCompatibleWithMemberPointer(mpB, a);

			StructureType sa = a as StructureType;
			StructureType sb = b as StructureType;
			if (sa != null && sb != null)
			{
				return AreCompatible(sa, sb);
			}

			ArrayType aa = a as ArrayType;
			ArrayType ab = b as ArrayType;
			if (aa != null && ab != null)
			{
				return AreCompatible(aa.ElementType, ab.ElementType);
			}

			UnionType ua = a as UnionType;
			UnionType ub = b as UnionType;
			if (ua != null && ub != null)
				return true;

			FunctionType fa = a as FunctionType;
			FunctionType fb = b as FunctionType;
			if (fa != null && fb != null)
			{
				return fa.ArgumentTypes.Length == fb.ArgumentTypes.Length;
			}
			return false;
		}

		public bool AreCompatible(StructureType a, StructureType b)
		{
			if (a.Size > 0 && b.Size > 0)
			{
				return a.Size == b.Size;
			}
			return true;
		}

		public bool IsCompatibleWithPointer(Pointer ptrA, DataType b)
		{
			Pointer ptrB = b as Pointer;
			if (ptrB != null)
				return AreCompatible(ptrA.Pointee, ptrB.Pointee);
			PrimitiveType pb = b as PrimitiveType;
			if (pb != null)
			{
                if ((pb.Domain & Domain.Selector|Domain.Pointer) != 0 && pb.Size == ptrA.Size)
                    return true;
			}
			return false;
		}

		public bool IsCompatibleWithMemberPointer(MemberPointer mpA, DataType b)
		{
			MemberPointer ptrB = b as MemberPointer;
			if (ptrB != null)
				return AreCompatible(mpA.Pointee, ptrB.Pointee);
			PrimitiveType pb = b as PrimitiveType;
			if (pb != null)
			{
				if (pb == PrimitiveType.Word16 || pb.Domain == Domain.Pointer || pb.Domain == Domain.Selector)
					return true;
			}
			return false;
		}

		public DataType Unify(DataType a, DataType b)
		{
			return UnifyInternal(a, b);
		}

		public DataType UnifyInternal(DataType a, DataType b)
		{
			if (a == null)
				return b;
			if (b == null)
				return a;

			if (a == b)
				return a;

			if (a is UnknownType)
				return b;
			if (b is UnknownType)
				return a;

			UnionType ua = a as UnionType;
			UnionType ub = b as UnionType;
			if (ua != null && ub != null)
			{
				UnionType u2 = UnifyUnions(ua, ub);
				return u2.Simplify();
			}
			if (ua != null)
			{
				UnifyIntoUnion(ua, b);
				return ua.Simplify();
			}
			if (ub != null)
			{
				UnifyIntoUnion(ub, a);
				return ub.Simplify();
			}

			PrimitiveType pa = a as PrimitiveType;
			PrimitiveType pb = b as PrimitiveType;
			if (pa != null && pb != null)
			{
				if (pa == pb)
					return pa;
					
				Domain d = pa.Domain & pb.Domain;
				if (d != 0 && pa.Size == pb.Size)
				{
					return PrimitiveType.Create(d, pa.Size);
				}
				return MakeUnion(a, b);
			}

			TypeVariable tA = a as TypeVariable;
			TypeVariable tB = b as TypeVariable;
			if (tA != null && tB != null)
			{
				return UnifyTypeVariables(tA, tB);
			}
			if (tA != null || tB != null)
			{
				MakeUnion(a, b);
			}

			EquivalenceClass eqA = a as EquivalenceClass;
			EquivalenceClass eqB = b as EquivalenceClass;
			if (eqA != null && eqB != null)
			{
				if (eqA.Number == eqB.Number)
					return eqA;
				else
					return MakeUnion(eqA, eqB);
			}
			Pointer ptrA = a as Pointer;
			Pointer ptrB = b as Pointer;
			if (ptrA != null && ptrB != null)
			{
				DataType dt = UnifyInternal(ptrA.Pointee, ptrB.Pointee);
				return new Pointer(dt, Math.Max(ptrA.Size, ptrB.Size));
			}
			if (ptrA != null)
			{
				return UnifyPointer(ptrA, b);
			}
			if (ptrB != null)
			{
				return UnifyPointer(ptrB, a);
			}

			MemberPointer mpA = a as MemberPointer;
			MemberPointer mpB = b as MemberPointer;
			if (mpA != null && mpB != null)
			{
				DataType baseType = UnifyInternal(mpA.BasePointer, mpB.BasePointer);
				DataType pointee = UnifyInternal(mpA.Pointee, mpB.Pointee);
				return new MemberPointer(baseType, pointee, mpB.Size);
			}
			if (mpA != null)
			{
				return UnifyMemberPointer(mpA, b);
			}
			if (mpB != null)
			{
				return UnifyMemberPointer(mpB, a);
			}

			FunctionType funA = a as FunctionType;
			FunctionType funB = b as FunctionType;
			if (funA != null && funB != null)
			{
				return UnifyFunctions(funA, funB);
			}

			ArrayType arrA = a as ArrayType;
			ArrayType arrB = b as ArrayType;
			if (arrA != null && arrB != null)
			{
				return UnifyArrays(arrA, arrB);
			}
			if (arrA != null && arrA.ElementType.Size >= b.Size)
			{
				arrA.ElementType = Unify(arrA.ElementType, b);
				return arrA;
			}
			if (arrB != null && arrB.ElementType.Size >= a.Size)
			{
				arrB.ElementType = Unify(arrB.ElementType, a);
				return arrB;
			}

			StructureType strA = a as StructureType;
			StructureType strB = b as StructureType;
			if (strA != null && strB != null)
			{
				return UnifyStructures(strA, strB);
			}
			if (strA != null && strA.Size >= b.Size)
			{
                MergeIntoStructure(b, strA);
				return strA;
			}
			if (strB != null && strB.Size >= a.Size)
			{
                MergeIntoStructure(a, strB);
				return strB;
			}
			if (strA != null || strB != null)
			{
				return MakeUnion(a, b);
			}


			return MakeUnion(a, b);
		}

        private void MergeIntoStructure(DataType a, StructureType str)
        {
            StructureField f = str.Fields.AtOffset(0);
            if (f != null)
            {
                f.DataType = Unify(a, f.DataType);
            }
            else
            {
                str.Fields.Add(0, a, null);
            }
        }

		public DataType UnifyArrays(ArrayType a, ArrayType b)
		{
			if (a.ElementType.Size == b.ElementType.Size)
			{
				int cElems = a.Length;
				if (cElems < b.Length)
					cElems = b.Length;
				return new ArrayType(Unify(a.ElementType, b.ElementType), cElems);
			}
			return MakeUnion(a, b);
		}

		public void UnifyIntoUnion(UnionType u, DataType dt)
		{
			foreach (UnionAlternative alt in u.Alternatives.Values)
			{
				if (AreCompatible(alt.DataType, dt))
				{
					alt.DataType = Unify(alt.DataType, dt);
					return;
				}
			}
			u.Alternatives.Add(new UnionAlternative(dt));
		}

		public DataType UnifyFunctions(FunctionType a, FunctionType b)
		{
			if (a.ArgumentTypes.Length != b.ArgumentTypes.Length)
			{
				return MakeUnion(a, b);
			}
			DataType ret = Unify(a.ReturnType, b.ReturnType);
			DataType [] args = new DataType[a.ArgumentTypes.Length];
			for (int i = 0; i < args.Length; ++i)
			{
				args[i] = Unify(a.ArgumentTypes[i], b.ArgumentTypes[i]);
			}
			return factory.CreateFunctionType(null, ret, args, null);
		}

		/// <summary>
		/// Unifies two structures by merging the fields in offset order.
		/// </summary>
		/// <remarks>
		/// Fields are taken from 
		/// </remarks>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public DataType UnifyStructures(StructureType a, StructureType b)
		{
			int newSize = 0;
			if (a.Size != 0 && b.Size != 0)
			{
				if (a.Size != b.Size)
				{
					return MakeUnion(a, b);
				}
				else
				{
					newSize = a.Size;
				}
			} 
			else if (a.Size != 0)
			{
				newSize = a.Size;
			}
			else if (b.Size != 0)
			{
				newSize = b.Size;
			}


			string name = null;
			if (a.Name != null)
			{
				if (b.Name != null && a.Name != b.Name)
					throw new NotSupportedException("Both structures have names! Woo! Return null and make union of it?");
				else
					name = a.Name;
			}
			else
			{
				name = b.Name;
			}

			StructureType mem = factory.CreateStructureType(name, newSize);
			mem.IsSegment = (a.IsSegment || b.IsSegment);

			IEnumerator<StructureField> ea = a.Fields.GetEnumerator();
            IEnumerator<StructureField> eb = b.Fields.GetEnumerator();
			StructureField fa = null;
			StructureField fb = null;
			for (;;)
			{
				if (fa == null && ea.MoveNext())
					fa = ea.Current;
				if (fb == null && eb.MoveNext())
					fb = eb.Current;
			
				if (fa == null || fb == null)
					break;

				if (fa.Offset < fb.Offset)
				{
					mem.Fields.Add(fa.Clone());
					fa = null;
				}
				else if (fa.Offset > fb.Offset)
				{
					mem.Fields.Add(fb.Clone());
					fb = null;
				}
				else
				{
                    DataType fieldType = Unify(fa.DataType, fb.DataType);
					mem.Fields.Add(fa.Offset, fieldType);
					fa = null;
					fb = null;
				}
			}
			if (fa != null)
			{
				mem.Fields.Add(fa);
				while (ea.MoveNext())
				{
					StructureField f = (StructureField) ea.Current;
					mem.Fields.Add(f.Clone());
				}
			}
			if (fb != null)
			{
				mem.Fields.Add(fb);
				while (eb.MoveNext())
				{
					StructureField f = (StructureField) eb.Current;
					mem.Fields.Add(f.Clone());
				}
			}
			return mem;
		}

		public DataType UnifyPointer(Pointer ptrA, DataType b)
		{
			PrimitiveType pb = b as PrimitiveType;
			if (pb != null)
			{
				if ((ptrA.Size == 0 || pb.Size == 0 || ptrA.Size == pb.Size) &&
					(pb.Domain & Domain.Pointer|Domain.Selector) != 0)
				{
					return ptrA.Clone();
				}
			}
			return MakeUnion(ptrA, b);
		}

		public DataType UnifyMemberPointer(MemberPointer mpA, DataType b)
		{
			PrimitiveType pb = b as PrimitiveType;
			if (pb != null)
			{
				if (pb == PrimitiveType.Word16 || pb == PrimitiveType.Word32 || pb.Domain == Domain.Selector)
				{
					//$REVIEW: line above should be if (mpA.Size = b.Size .... as in UnifyPointer.
					return mpA.Clone();
				}
			}
			return MakeUnion(mpA, b);
		}

		public virtual DataType UnifyTypeVariables(TypeVariable tA, TypeVariable tB)
		{
			if (tA.Number == tB.Number)
				return tA;
			else
				return MakeUnion(tA, tB);
		}

		public UnionType UnifyUnions(UnionType u1, UnionType u2)
		{
			UnionType u = new UnionType(null, null);
			foreach (UnionAlternative a in u1.Alternatives.Values)
			{
				UnifyIntoUnion(u, a.DataType);
			}
			foreach (UnionAlternative a in u2.Alternatives.Values)
			{
				UnifyIntoUnion(u, a.DataType);
			}
			return u;
		}

		public UnionType MakeUnion(DataType a, DataType b)
		{
			return factory.CreateUnionType(null, null, new DataType [] { a, b } );
		}

		private DataType Nyi(DataType a, DataType b)
		{
			throw new NotImplementedException(string.Format("Don't know how to unify {0} with {1}.", a, b));
		}
	}
}
