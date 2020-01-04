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
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Core.Types
{
	/// <summary>
	/// Performs type unification, returning a general unifier for the two
    /// data type parameters.
	/// </summary>
	public class Unifier
	{
		private readonly TypeFactory factory;
        private readonly TraceSwitch trace;
		private readonly IDictionary<Tuple<DataType, DataType>, bool> cache = new Dictionary<Tuple<DataType, DataType>, bool>();

        public Unifier()
            : this(new TypeFactory(), null)
        {
        }

		public Unifier(TypeFactory factory, TraceSwitch trace)
		{
			this.factory = factory;
            this.trace = trace;
        }

		public bool AreCompatible(DataType a, DataType b)
		{
			return AreCompatible(a, b, 0);
		}
		
		private bool AreCompatible(DataType a, DataType b, int depth)
		{
			var typePair = new Tuple<DataType, DataType>(a, b);

			if (cache.TryGetValue(typePair, out bool d))
				return d;

			d = DoAreCompatible(a, b, depth);
			cache[typePair] = d;

			return d;
		}

		private bool DoAreCompatible(DataType a, DataType b, int depth)
		{
			if (a == null || b == null)
				return false;

			if (depth > 20)
				throw new StackOverflowException("Way too deep");     //$BUG: discover why datatypes recurse so deep.
			
			PrimitiveType pa = a as PrimitiveType;
			PrimitiveType pb = b as PrimitiveType;
			if (pa != null && pb != null)
			{
				if (pa.Size != pb.Size)
					return false;
				return (pa.Domain &  pb.Domain) != 0;
			}

            TypeReference tra = a as TypeReference;
            TypeReference trb = b as TypeReference;
            if (tra != null && trb != null)
                return tra == trb;
            if (tra != null)
                return AreCompatible(tra.Referent, b, ++depth);
            if (trb != null)
                return AreCompatible(a, trb.Referent, ++depth);

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
				return IsCompatibleWithPointer(ptrA, b, ++depth);
			if (ptrB != null)
				return IsCompatibleWithPointer(ptrB, a, ++depth);

			MemberPointer mpA = a as MemberPointer;
			MemberPointer mpB = b as MemberPointer;
			if (mpA != null)
				return IsCompatibleWithMemberPointer(mpA, b, ++depth);
			if (mpB != null)
				return IsCompatibleWithMemberPointer(mpB, a, ++depth);

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
				return AreCompatible(aa.ElementType, ab.ElementType, ++depth);
			}

			UnionType ua = a as UnionType;
			UnionType ub = b as UnionType;
			if (ua != null && ub != null)
				return true;

			FunctionType fa = a as FunctionType;
			FunctionType fb = b as FunctionType;
			if (fa != null && fb != null)
			{
				return fa.Parameters.Length == fb.Parameters.Length;
			}

            CodeType ca = a as CodeType;
            CodeType cb = a as CodeType;
            if (ca != null && cb != null)
            {
                return true;
            }
            if (a is UnknownType unkA)
            {
                if (unkA.Size == 0 || a.Size == b.Size)
                    return true;
            }
            if (b is UnknownType unkB)
            {
                if (unkB.Size == 0 || a.Size == b.Size)
                    return true;
            }
            return false;
		}

		private bool AreCompatible(StructureType a, StructureType b)
		{
			if (a.Size > 0 && b.Size > 0)
			{
				return a.Size == b.Size;
			}
			return true;
		}

		private bool IsCompatibleWithPointer(Pointer ptrA, DataType b, int depth)
		{
			Pointer ptrB = b as Pointer;
            if (ptrB != null)
            {
                if (AreCompatible(ptrA.Pointee, ptrB.Pointee, ++depth))
                    return true;
                var arrayA = ptrA.Pointee as ArrayType;
                var arrayB = ptrB.Pointee as ArrayType;
                if (arrayA != null)
                    return AreCompatible(arrayA.ElementType, ptrB.Pointee, ++depth);
                else if (arrayB != null)
                    return AreCompatible(ptrA.Pointee, arrayB.ElementType, ++depth);
                else
                    return false;
            }
			PrimitiveType pb = b as PrimitiveType;
			if (pb != null)
			{
                if ((pb.Domain & Domain.Selector|Domain.Pointer) != 0 && pb.Size == ptrA.Size)
                    return true;
			}
			return false;
		}

		private bool IsCompatibleWithMemberPointer(MemberPointer mpA, DataType b, int depth)
		{
			MemberPointer mpB = b as MemberPointer;
            if (mpB != null)
                return
                    AreCompatible(mpA.BasePointer, mpB.BasePointer, ++depth) && 
				    AreCompatible(mpA.Pointee, mpB.Pointee, ++depth);
			PrimitiveType pb = b as PrimitiveType;
			if (pb != null && pb.BitSize == mpA.BitSize)
			{
				if (pb == PrimitiveType.Word16 || pb == PrimitiveType.Word32  ||
                    pb.Domain == Domain.Pointer ||
                    pb.Domain == Domain.Selector ||
                    pb.Domain == Domain.Offset)
					return true;
			}
			return false;
		}

        private int recDepth;

		public DataType Unify(DataType a, DataType b)
		{
            if (++recDepth > 100)
            {
                --recDepth;
                DebugEx.Error(trace, "Unifier: exceeded stack depth, giving up");
                if (a == null && b == null)
                    return null;
                if (a == null)
                    return b;
                if (b == null)
                    return a;
                return factory.CreateUnionType(null, null, new[] { a, b });
            }
            var u = UnifyInternal(a, b);
            --recDepth;
            return u;
		}

		private DataType UnifyInternal(DataType a, DataType b)
		{
			if (a == null)
				return b;
			if (b == null)
				return a;

			if (a == b)
				return a;

            if (a is UnknownType)
            {
                if (a.Size == 0 || a.Size == b.Size)
                    return b;
            }
            if (b is UnknownType)
            {
                if (b.Size == 0 || a.Size == b.Size)
                    return a;
            }

            if (a is VoidType)
                return b;
            if (b is VoidType)
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

                return UnifyPrimitives(pa, pb);
			}

			TypeVariable tA = a as TypeVariable;
			TypeVariable tB = b as TypeVariable;
            if (tA != null && tB != null)
            {
                return UnifyTypeVariables(tA, tB);
            }

            TypeReference trA = a as TypeReference;
            TypeReference trB = b as TypeReference;
            if (trA != null && trB != null)
            {
                if (trA == trB)
                    return trA;
                else 
                    return MakeUnion(a, b);
            }
            if (trA != null)
            {
                if (AreCompatible(trA.Referent, b))
                {
                    return new TypeReference(trA.Name, UnifyInternal(trA.Referent, b));
                }
            }
            if (trB != null)
            {
                if (AreCompatible(a, trB.Referent))
                {
                    return new TypeReference(trB.Name, UnifyInternal(trB.Referent, a));
                }
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
				return new Pointer(dt, Math.Max(ptrA.BitSize, ptrB.BitSize));
			}
            if (ptrA != null)
            {
                var dt = UnifyPointer(ptrA, b);
                if (dt != null)
                    return dt;
            }
            if (ptrB != null)
            {
                var dt = UnifyPointer(ptrB, a);
                if (dt != null)
                    return dt;
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
				var dt = UnifyMemberPointer(mpA, b);
                if (dt != null)
                    return dt;
			}
			if (mpB != null)
			{
				var dt = UnifyMemberPointer(mpB, a);
                if (dt != null)
                    return dt;
			}

			FunctionType funA = a as FunctionType;
			FunctionType funB = b as FunctionType;
			if (funA != null && funB != null)
			{
				return UnifyFunctions(funA, funB);
			}
            if (funA != null && b is CodeType)
            {
                return funA;
            }
            if (funB != null && a is CodeType)
            {
                return funB;
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
			if (strA != null && (strA.Size == 0 || strA.Size >= b.Size))
			{
                MergeIntoStructure(b, strA);
				return strA;
			}
			if (strB != null && (strB.Size == 0 || strB.Size >= a.Size))
			{
                MergeIntoStructure(a, strB);
				return strB;
			}
			if (strA != null || strB != null)
			{
				return MakeUnion(a, b);
			}
            CodeType ca = a as CodeType;
            CodeType cb = b as CodeType;
            if (ca != null && cb != null)
            {
                return ca;
            }
            if (tA != null)
            {
                return UnifyTypeVariable(tA, b);
            }
            if (tB != null)
            {
                return UnifyTypeVariable(tB, a);
            }
            return MakeUnion(a, b);
		}

        private DataType UnifyPrimitives(PrimitiveType pa, PrimitiveType pb)
        {
            Domain d = pa.Domain & pb.Domain;
            if (d != 0 && pa.BitSize == pb.BitSize)
            {
                return PrimitiveType.Create(d, pa.BitSize);
            }
            if (pa.Domain == Domain.SegPointer && pb.BitSize == 16)
                return pa;
            return MakeUnion(pa, pb);
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
			u.Alternatives.Add(new UnionAlternative(dt, u.Alternatives.Count));
		}

		public DataType UnifyFunctions(FunctionType a, FunctionType b)
		{
			if (a.Parameters.Length != b.Parameters.Length)
			{
				return MakeUnion(a, b);
			}
			DataType ret = Unify(a.ReturnValue.DataType, b.ReturnValue.DataType);
			Identifier [] args = new Identifier[a.Parameters.Length];
			for (int i = 0; i < args.Length; ++i)
			{
				var dt = Unify(a.Parameters[i].DataType, b.Parameters[i].DataType);
                args[i] = new Identifier(null, dt, null);   //$BUG: unify storages!
			}
			return factory.CreateFunctionType(new Identifier("", ret, null), args);
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
                    return MakeUnion(a, b);
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

                var nestedStructureUnifier = new NestedStructureUnifier(this);
                if (nestedStructureUnifier.Match(fa, fb))
                {
                    nestedStructureUnifier.Unify();
                    fa = nestedStructureUnifier.NextFieldA;
                    fb = nestedStructureUnifier.NextFieldB;
                }
                else if (fa.Offset < fb.Offset)
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
                    var fieldType = Unify(fa.DataType, fb.DataType);
                    if (!TryMakeFieldName(fa, fb, out string fieldName))
                        throw new NotSupportedException(
                            string.Format(
                                "Failed to unify field '{0}' in structure '{1}' with field '{2}' in structure '{3}'.",
                                fa.Name, a, fb.Name, b));
                    mem.Fields.Add(fa.Offset, fieldType, fieldName);
					fa = null;
					fb = null;
				}
			}
			if (fa != null)
			{
				mem.Fields.Add(fa);
				while (ea.MoveNext())
				{
					StructureField f = ea.Current;
					mem.Fields.Add(f.Clone());
				}
			}
			if (fb != null)
			{
				mem.Fields.Add(fb);
				while (eb.MoveNext())
				{
					StructureField f = eb.Current;
					mem.Fields.Add(f.Clone());
				}
			}
            mem.ForceStructure = a.ForceStructure | b.ForceStructure;
			return mem;
		}

        private bool TryMakeFieldName(StructureField fa, StructureField fb, out string name)
        {
            name = null;
            if (fa.IsNameSet && fb.IsNameSet && fa.Name != fb.Name)
                return false;
            if (fa.IsNameSet)
                name = fa.Name;
            if (fb.IsNameSet)
                name = fb.Name;
            return true;
        }

        class NestedStructureUnifier
        {
            private Unifier unifier;
            private StructureField fNestedStruct;
            private StructureField fOther;
            private StructureField fa;
            private StructureField fb;


            public NestedStructureUnifier(Unifier unifier)
            {
                this.unifier = unifier;
            }

            public bool Match(StructureField fa, StructureField fb)
            {
                this.fa = fa;
                this.fb = fb;
                this.fNestedStruct = null;
                this.fOther = null;
                this.NextFieldA = null;
                this.NextFieldB = null;
                var strFa = fa.DataType.TypeReferenceAs<StructureType>();
                var strFb = fb.DataType.TypeReferenceAs<StructureType>();
                // only one field should be nested structure
                if (
                    (strFa == null && strFb == null) ||
                    (strFa != null && strFb != null))
                    return false;

                // check which of two fields is nested structure and store it
                // and other field in corresponding variables.
                int strSize;
                if (strFa != null)
                {
                    fNestedStruct = fa;
                    fOther = fb;
                    strSize = strFa.GetInferredSize();
                }
                else
                {
                    fNestedStruct = fb;
                    fOther = fa;
                    strSize = strFb.GetInferredSize();
                }

                // check if other field is inside nested structure
                return (
                    fOther.Offset >= fNestedStruct.Offset &&
                    fOther.Offset < fNestedStruct.Offset + strSize);
            }

            public void Unify()
            {
                var str = unifier.factory.CreateStructureType(null, 0);
                str.Fields.Add(
                    fOther.Offset - fNestedStruct.Offset,
                    fOther.DataType);

                var fieldType = unifier.Unify(fNestedStruct.DataType, str);
                var field = new StructureField(
                    fNestedStruct.Offset,
                    fieldType,
                    fNestedStruct.Name);
                NextFieldA = (fOther == fa) ? null : field;
                NextFieldB = (fOther == fb) ? null : field;
            }

            public StructureField NextFieldA { get; private set; }
            public StructureField NextFieldB { get; private set; }
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
			return null;
		}

		public DataType UnifyMemberPointer(MemberPointer mpA, DataType b)
		{
			PrimitiveType pb = b as PrimitiveType;
			if (pb != null)
			{
                if (pb == PrimitiveType.Word16 || pb == PrimitiveType.Word32 ||
                    pb.Domain == Domain.Selector || pb.Domain == Domain.Offset)
				{
					//$REVIEW: line above should be if (mpA.Size = b.Size .... as in UnifyPointer.
					return mpA.Clone();
				}
			}
			return null;
		}

		public virtual DataType UnifyTypeVariables(TypeVariable tA, TypeVariable tB)
		{
			if (tA.Number == tB.Number)
				return tA;
			else
				return MakeUnion(tA, tB);
		}

        private DataType UnifyTypeVariable(TypeVariable tv, DataType dt)
        {
            // TypeVariable should be already unified with this DataType by
            // ExpressionTypeAscender so just return DataType
            return dt;
        }

        public UnionType UnifyUnions(UnionType u1, UnionType u2)
		{
			UnionType u = new UnionType(null, null);
			foreach (UnionAlternative a in u1.Alternatives.Values.ToList())
			{
				UnifyIntoUnion(u, a.DataType);
			}
			foreach (UnionAlternative a in u2.Alternatives.Values.ToList())
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
            throw new NotImplementedException($"Don't know how to unify {a} with {b}.");
		}
	}
}
