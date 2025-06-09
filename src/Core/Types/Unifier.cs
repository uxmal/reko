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

using Reko.Core.Diagnostics;
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
        private static readonly TraceSwitch classTrace = new TraceSwitch(nameof(Unifier), "Traces unifier progress") { Level = TraceLevel.Off }; 

		private readonly TypeFactory factory;
        private readonly TraceSwitch trace;
		private readonly IDictionary<(DataType, DataType), bool> cache = new Dictionary<(DataType, DataType), bool>();

        /// <summary>
        /// Constructs a <see cref="Unifier"/> instance.
        /// </summary>
        public Unifier()
            : this(new TypeFactory(), classTrace)
        {
        }

        /// <summary>
        /// Constructs a <see cref="Unifier"/> instance.
        /// </summary>
        /// <param name="factory">Type factory to use.</param>
        public Unifier(TypeFactory factory)
            : this(factory, classTrace)
        {
        }

        /// <summary>
        /// Constructs a <see cref="Unifier"/> instance.
        /// </summary>
        /// <param name="factory">Type factory to use.</param>
        /// <param name="trace">Trace switch governing debug output.</param>
		public Unifier(TypeFactory factory, TraceSwitch trace)
		{
			this.factory = factory;
            this.trace = trace;
        }

        /// <summary>
        /// Returns true if unifying the two data types will not
        /// result in a union.
        /// </summary>
		public bool AreCompatible(DataType a, DataType b)
		{
			return AreCompatible(a, b, 0);
		}
		
		private bool AreCompatible(DataType a, DataType b, int depth)
		{
			var typePair = (a, b);

			if (cache.TryGetValue(typePair, out bool d))
				return d;

			d = DoAreCompatible(a, b, depth);
			cache[typePair] = d;

			return d;
		}

		private bool DoAreCompatible(DataType a, DataType b, int depth)
		{
			if (a is null || b is null)
				return false;

            if (depth > 20)
            {
                trace.Error("Way too deep");     //$BUG: discover why datatypes recurse so deep.
                return true;
            }
			
			PrimitiveType? pa = a as PrimitiveType;
			PrimitiveType? pb = b as PrimitiveType;

			if (pa is not null && pb is not null)
			{
                //$REVIEW: shouldn't this be .BitSize?
                // making the change gives massive regressions.
				if (pa.Size != pb.Size)
					return false;
                if ((pa.Domain & pb.Domain) != 0)
                    return true;
                if (pa.Domain.HasFlag(Domain.Pointer) &&
                    pb.Domain.HasFlag(Domain.SegPointer))
                    return true;
                if (pb.Domain.HasFlag(Domain.Pointer) &&
                    pa.Domain.HasFlag(Domain.SegPointer))
                    return true;
                return false;
            }

            TypeReference? tra = a as TypeReference;
            TypeReference? trb = b as TypeReference;
            if (tra is not null && trb is not null)
                return tra == trb;
            if (tra is not null)
                return AreCompatible(tra.Referent, b, ++depth);
            if (trb is not null)
                return AreCompatible(a, trb.Referent, ++depth);

			TypeVariable? tva = a as TypeVariable;
			TypeVariable? tvb = b as TypeVariable;
			if (tva is not null && tvb is not null)
			{
				return tva.Number == tvb.Number;
			}

			EquivalenceClass? eqA = a as EquivalenceClass;
			EquivalenceClass? eqB = b as EquivalenceClass;
			if (eqA is not null && eqB is not null)
			{
				return eqA.Number == eqB.Number;
			}

			Pointer? ptrA = a as Pointer;
			Pointer? ptrB = b as Pointer;
			if (ptrA is not null)
				return IsCompatibleWithPointer(ptrA, b, ++depth);
			if (ptrB is not null)
				return IsCompatibleWithPointer(ptrB, a, ++depth);

			MemberPointer? mpA = a as MemberPointer;
			MemberPointer? mpB = b as MemberPointer;
			if (mpA is not null)
				return IsCompatibleWithMemberPointer(mpA, b, ++depth);
			if (mpB is not null)
				return IsCompatibleWithMemberPointer(mpB, a, ++depth);

			StructureType? sa = a as StructureType;
			StructureType? sb = b as StructureType;
			if (sa is not null && sb is not null)
			{
				return AreCompatible(sa, sb);
			}

            ArrayType? aa = a as ArrayType;
			ArrayType? ab = b as ArrayType;
			if (aa is not null && ab is not null)
			{
				return AreCompatible(aa.ElementType, ab.ElementType, ++depth);
			}
            if (aa is not null && b.IsWord)
            {
                return aa.Size == b.Size;
            }
            else if (ab is not null && a.IsWord)
            {
                return ab.Size == a.Size;
            }
            

			UnionType? ua = a as UnionType;
			UnionType? ub = b as UnionType;
			if (ua is not null && ub is not null)
				return true;

			FunctionType? fa = a as FunctionType;
			FunctionType? fb = b as FunctionType;
			if (fa is not null && fb is not null)
			{
                if (fa.ParametersValid != fb.ParametersValid)
                    return false;
				return fa.Parameters!.Length == fb.Parameters!.Length;
			}

            CodeType? ca = a as CodeType;
            CodeType? cb = a as CodeType;
            if (ca is not null && cb is not null)
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
            if (b is Pointer ptrB)
            {
                if (AreCompatible(ptrA.Pointee, ptrB.Pointee, ++depth))
                    return true;
                var arrayA = ptrA.Pointee as ArrayType;
                var arrayB = ptrB.Pointee as ArrayType;
                if (arrayA is not null)
                    return AreCompatible(arrayA.ElementType, ptrB.Pointee, ++depth);
                else if (arrayB is not null)
                    return AreCompatible(ptrA.Pointee, arrayB.ElementType, ++depth);
                else
                    return false;
            }
            if (b is PrimitiveType pb)
            {
                if ((pb.Domain & Domain.Selector | Domain.Pointer) != 0 && pb.Size == ptrA.Size)
                    return true;
            }
            return false;
		}

		private bool IsCompatibleWithMemberPointer(MemberPointer mpA, DataType b, int depth)
		{
            if (b is MemberPointer mpB)
                return
                    AreCompatible(mpA.BasePointer, mpB.BasePointer, ++depth) &&
                    AreCompatible(mpA.Pointee, mpB.Pointee, ++depth);
            if (b is PrimitiveType pb && pb.BitSize == mpA.BitSize)
            {
                if (pb == PrimitiveType.Word16 || pb == PrimitiveType.Word32 ||
                    pb.Domain == Domain.Pointer ||
                    pb.Domain == Domain.Selector ||
                    pb.Domain == Domain.Offset)
                    return true;
            }
            return false;
		}

        /// <summary>
        /// Returns true if the data type <paramref name="a"/> can be unified
        /// into data type <paramref name="b"/>.
        /// </summary>
        private bool CanBeMergedInto(DataType a, DataType b)
        {
            if (AreCompatible(a, b))
                return true;
            var strB = b.ResolveAs<StructureType>();
            if (strB is null)
                return false;
            var ptA = a.ResolveAs<PrimitiveType>();
            if (ptA is null)
                return false;
            var firstFieldB = strB.Fields.AtOffset(0);
            if (firstFieldB is null)
                return true;
            var ptB = firstFieldB.DataType.ResolveAs<PrimitiveType>();
            if (ptB is null)
                return false;
            return ptA.Compare(ptB) == 0;
        }

        private int recDepth;

        //$TODO: change the signature to disallow nulls.

        /// <summary>
        /// Unifies two data types, returning a new type that is the
        /// mgu (minimal general unifier) of the two types.
        /// </summary>
        /// <param name="a">First type.</param>
        /// <param name="b">Second type.</param>
        /// <returns>The minimal general unifier of the types.
        /// </returns>
		public DataType? Unify(DataType? a, DataType? b)
		{
            if (++recDepth > 100)
            {
                --recDepth;
                trace.Error("Unifier: exceeded stack depth, giving up");
                if (a is null && b is null)
                    return null;
                if (a is null)
                    return b;
                if (b is null)
                    return a;
                return factory.CreateUnionType(null, null, new[] { a, b });
            }
            var u = UnifyInternal(a, b);
            --recDepth;
            return u;
		}

		private DataType? UnifyInternal(DataType? a, DataType? b)
		{
			if (a is null)
				return b;
			if (b is null)
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

            UnionType? ua = a as UnionType;
			UnionType? ub = b as UnionType;
			if (ua is not null && ub is not null)
			{
				UnionType u2 = UnifyUnions(ua, ub);
				return u2.Simplify();
			}
			if (ua is not null)
			{
				UnifyIntoUnion(ua, b);
				return ua.Simplify();
			}
			if (ub is not null)
			{
				UnifyIntoUnion(ub, a);
				return ub.Simplify();
			}

			PrimitiveType? pa = a as PrimitiveType;
			PrimitiveType? pb = b as PrimitiveType;
			if (pa is not null && pb is not null)
			{
				if (pa == pb)
					return pa;

                return UnifyPrimitives(pa, pb);
			}

			TypeVariable? tA = a as TypeVariable;
			TypeVariable? tB = b as TypeVariable;
            if (tA is not null && tB is not null)
            {
                return UnifyTypeVariables(tA, tB);
            }

            TypeReference? trA = a as TypeReference;
            TypeReference? trB = b as TypeReference;
            if (trA is not null && trB is not null)
            {
                if (trA == trB)
                    return trA;
                else 
                    return MakeUnion(a, b);
            }
            if (trA is not null)
            {
                if (CanBeMergedInto(b, trA.Referent))
                {
                    return new TypeReference(trA.Name, UnifyInternal(trA.Referent, b)!);
                }
            }
            if (trB is not null)
            {
                if (CanBeMergedInto(a, trB.Referent))
                {
                    return new TypeReference(trB.Name, UnifyInternal(trB.Referent, a)!);
                }
            }

			EquivalenceClass? eqA = a as EquivalenceClass;
			EquivalenceClass? eqB = b as EquivalenceClass;
			if (eqA is not null && eqB is not null)
			{
				if (eqA.Number == eqB.Number)
					return eqA;
				else
					return MakeUnion(eqA, eqB);
			}

			Pointer? ptrA = a as Pointer;
			Pointer? ptrB = b as Pointer;
			if (ptrA is not null && ptrB is not null)
			{
				DataType dt = UnifyInternal(ptrA.Pointee, ptrB.Pointee)!;
				return new Pointer(dt, Math.Max(ptrA.BitSize, ptrB.BitSize));
			}
            if (ptrA is not null)
            {
                var dt = UnifyPointer(ptrA, b);
                if (dt is not null)
                    return dt;
            }
            if (ptrB is not null)
            {
                var dt = UnifyPointer(ptrB, a);
                if (dt is not null)
                    return dt;
            }

			MemberPointer? mpA = a as MemberPointer;
			MemberPointer? mpB = b as MemberPointer;
			if (mpA is not null && mpB is not null)
			{
				DataType baseType = UnifyInternal(mpA.BasePointer, mpB.BasePointer)!;
				DataType pointee = UnifyInternal(mpA.Pointee, mpB.Pointee)!;
				return new MemberPointer(baseType, pointee, mpB.BitSize);
			}
			if (mpA is not null)
			{
				var dt = UnifyMemberPointer(mpA, b);
                if (dt is not null)
                    return dt;
			}
			if (mpB is not null)
			{
				var dt = UnifyMemberPointer(mpB, a);
                if (dt is not null)
                    return dt;
			}

			FunctionType? funA = a as FunctionType;
			FunctionType? funB = b as FunctionType;
			if (funA is not null && funB is not null)
			{
				return UnifyFunctions(funA, funB);
			}
            if (funA is not null && b is CodeType)
            {
                return funA;
            }
            if (funB is not null && a is CodeType)
            {
                return funB;
            }

			ArrayType? arrA = a as ArrayType;
			ArrayType? arrB = b.ResolveAs<ArrayType>();
			if (arrA is not null && arrB is not null)
			{
				return UnifyArrays(arrA, arrB);
			}
            arrA = a.ResolveAs<ArrayType>();
            arrB = b as ArrayType;
            if (arrA is not null && arrB is not null)
            {
                return UnifyArrays(arrA, arrB);
            }
            if (arrA is not null && arrA.ElementType.Size >= b.Size)
			{
				arrA.ElementType = Unify(arrA.ElementType, b)!;
				return arrA;
			}
			if (arrB is not null && arrB.ElementType.Size >= a.Size)
			{
				arrB.ElementType = Unify(arrB.ElementType, a)!;
				return arrB;
			}
            if (arrA is not null && b.IsWord && a.Size == b.Size)
            {
                return arrA;
            }
            else if (arrB is not null && a.IsWord && b.Size == a.Size)
            {
                return arrB;
            }

            StructureType? strA = a as StructureType;
			StructureType? strB = b as StructureType;
			if (strA is not null && strB is not null)
			{
				return UnifyStructures(strA, strB);
			}
			if (strA is not null && (strA.Size == 0 || strA.Size >= b.Size))
			{
                MergeIntoStructure(b, strA);
				return strA;
			}
			if (strB is not null && (strB.Size == 0 || strB.Size >= a.Size))
			{
                MergeIntoStructure(a, strB);
				return strB;
			}
			if (strA is not null || strB is not null)
			{
				return MakeUnion(a, b);
			}
            CodeType? ca = a as CodeType;
            CodeType? cb = b as CodeType;
            if (ca is not null && cb is not null)
            {
                return ca;
            }
            if (tA is not null)
            {
                return UnifyTypeVariable(tA, b);
            }
            if (tB is not null)
            {
                return UnifyTypeVariable(tB, a);
            }
            return MakeUnion(a, b);
		}

        private DataType UnifyPrimitives(PrimitiveType pa, PrimitiveType pb)
        {
            Domain d = pa.Domain & pb.Domain;
            if (pa.BitSize == pb.BitSize)
            {
                if (d != 0)
                {
                    return PrimitiveType.Create(d, pa.BitSize);
                }
                if (pa.Domain.HasFlag(Domain.SegPointer) &&
                    pb.Domain.HasFlag(Domain.Pointer))
                {
                    return PrimitiveType.Create(Domain.SegPointer, pa.BitSize);
                }
                if (pb.Domain.HasFlag(Domain.SegPointer) &&
                    pa.Domain.HasFlag(Domain.Pointer))
                {
                    return PrimitiveType.Create(Domain.SegPointer, pb.BitSize);
                }
            }
            if (pa.Domain == Domain.SegPointer && pb.BitSize == 16)
                return pa;
            return MakeUnion(pa, pb);
        }

        private void MergeIntoStructure(DataType a, StructureType str)
        {
            StructureField? f = str.Fields.AtOffset(0);
            if (f is not null)
            {
                f.DataType = UnifyFieldTypes(a, f.DataType)!;
            }
            else
            {
                str.Fields.Add(0, a);
            }
        }

		private DataType UnifyArrays(ArrayType a, ArrayType b)
		{
			if (a.ElementType.Size == b.ElementType.Size)
			{
				int cElems = a.Length;
				if (cElems < b.Length)
					cElems = b.Length;
				return new ArrayType(Unify(a.ElementType, b.ElementType)!, cElems);
			}
			return MakeUnion(a, b);
		}

        /// <summary>
        /// Unify a datatype into a union.
        /// </summary>
        /// <param name="u">Union to unify into.</param>
        /// <param name="dt">Datatype to unify.</param>

		public void UnifyIntoUnion(UnionType u, DataType dt)
		{
			foreach (UnionAlternative alt in u.Alternatives.Values)
			{
				if (AreCompatible(alt.DataType, dt))
				{
					alt.DataType = Unify(alt.DataType, dt)!;
					return;
				}
			}
			u.Alternatives.Add(new UnionAlternative(dt, u.Alternatives.Count));
		}

		private DataType UnifyFunctions(FunctionType a, FunctionType b)
		{
            if (!a.ParametersValid && !b.ParametersValid)
            {
                return a;
            }
            if (!a.ParametersValid)
            {
                return b;
            }
            if (!b.ParametersValid)
            {
                return a;
            }
			if (a.Parameters!.Length != b.Parameters!.Length)
			{
				return MakeUnion(a, b);
			}
			DataType ret = Unify(a.ReturnValue!.DataType, b.ReturnValue!.DataType)!;
			Identifier [] args = new Identifier[a.Parameters.Length];
			for (int i = 0; i < args.Length; ++i)
			{
				var dt = Unify(a.Parameters[i].DataType, b.Parameters[i].DataType)!;
                var name = a.Parameters[i].Name;
                args[i] = new Identifier(name, dt, a.Parameters[i].Storage);   //$BUG: unify storages!
			}
			return factory.CreateFunctionType(new Identifier("", ret, a.ReturnValue.Storage), args);
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

            string? name;
            if (a.Name is not null)
			{
				if (b.Name is not null && a.Name != b.Name)
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
			StructureField? fa = null;
			StructureField? fb = null;
			for (;;)
			{
				if (fa is null && ea.MoveNext())
					fa = ea.Current;
				if (fb is null && eb.MoveNext())
					fb = eb.Current;
			
				if (fa is null || fb is null)
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
                    var fieldType = UnifyFieldTypes(fa.DataType, fb.DataType);
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
			if (fa is not null)
			{
				mem.Fields.Add(fa);
				while (ea.MoveNext())
				{
					StructureField f = ea.Current;
					mem.Fields.Add(f.Clone());
				}
			}
			if (fb is not null)
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

        private DataType UnifyFieldTypes(DataType fa, DataType fb)
        {
            var afa = fa?.ResolveAs<ArrayType>();
            var afb = fb?.ResolveAs<ArrayType>();
            if (afa is not null && afb is not null)
            {
                return UnifyArrays(afa, afb)!;
            }
            if (afa is not null)
            {
                var dt = Unify(afa.ElementType, fb)!;
                return factory.CreateArrayType(dt, afa.Length);
            }
            if (afb is not null)
            {
                var dt = Unify(afb.ElementType, fa)!;
                return factory.CreateArrayType(dt, afb.Length);
            }
            return Unify(fa, fb)!;
        }

        private bool TryMakeFieldName(StructureField fa, StructureField fb, out string name)
        {
            name = null!;
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
            private readonly Unifier unifier;
            private StructureField? fNestedStruct;
            private StructureField? fOther;
            private StructureField? fa;
            private StructureField? fb;


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
                    (strFa is null && strFb is null) ||
                    (strFa is not null && strFb is not null))
                    return false;

                // check which of two fields is nested structure and store it
                // and other field in corresponding variables.
                int strSize;
                if (strFa is not null)
                {
                    fNestedStruct = fa;
                    fOther = fb;
                    strSize = strFa.MeasureSize();
                }
                else
                {
                    fNestedStruct = fb;
                    fOther = fa;
                    strSize = strFb!.MeasureSize();
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
                    fOther!.Offset - fNestedStruct!.Offset,
                    fOther.DataType);

                var fieldType = unifier.Unify(fNestedStruct.DataType, str)!;
                var field = new StructureField(
                    fNestedStruct.Offset,
                    fieldType,
                    fNestedStruct.Name);
                NextFieldA = (fOther == fa) ? null : field;
                NextFieldB = (fOther == fb) ? null : field;
            }

            public StructureField? NextFieldA { get; private set; }
            public StructureField? NextFieldB { get; private set; }
        }

        private DataType? UnifyPointer(Pointer ptrA, DataType b)
		{
            if (b is PrimitiveType pb)
            {
                if ((ptrA.Size == 0 || pb.Size == 0 || ptrA.Size == pb.Size) &&
                    (pb.Domain & Domain.Pointer | Domain.Selector) != 0)
                {
                    return ptrA.Clone();
                }
            }
            return null;
		}

		private DataType? UnifyMemberPointer(MemberPointer mpA, DataType b)
		{
            if (b is PrimitiveType pb)
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

        /// <summary>
        /// Unifies two type variables.
        /// </summary>
        /// <param name="tA">First type variable.</param>
        /// <param name="tB">Second type variable.</param>
        /// <returns>The union of the type variables.</returns>
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

        /// <summary>
        /// Unifies two union types.
        /// </summary>
        /// <param name="u1">First union type.</param>
        /// <param name="u2">Second union type.</param>
        /// <returns>The union of the types.</returns>
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

		private UnionType MakeUnion(DataType a, DataType b)
		{
			return factory.CreateUnionType(null, null, new DataType [] { a, b } );
		}

		private DataType Nyi(DataType a, DataType b)
		{
            throw new NotImplementedException($"Don't know how to unify {a} with {b}.");
		}
	}
}
