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

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Core.Types
{
	/// <summary>
	/// Imposes a total ordering of DataTypes and compares two data types to 
    /// see if they are equal or less than each other.
	/// </summary>
	public class DataTypeComparer : IComparer<DataType>, IDataTypeVisitor<int>, IEqualityComparer<DataType>
	{
        private const int Prim = 0;
        private const int Enum = 1;
        private const int Ptr = 2;
        private const int MemPtr = 3;
        private const int Fn = 4;
        private const int Array = 5;
        private const int String = 6;
        private const int Struct = 7;
        private const int Union = 8;
        private const int TRef = 9;
        private const int TVar = 10;
        private const int EqClass = 11;
        private const int Code = 12;
        private const int Ref = 13;
        private const int Unk = 14;
        private const int Void = 15;

        private IDictionary<Tuple<DataType, DataType>, int> compareResult;

		private static DataTypeComparer ourGlobalComparer = new DataTypeComparer();

        public DataTypeComparer()
        {
            this.compareResult = new Dictionary<Tuple<DataType, DataType>, int>();
        }

        /// <summary>
        /// Implements a partial ordering on data types, where 
        /// primitives &lt; pointers &lt; arrays &lt; structs &lt; unions
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(DataType x, DataType y)
        {
            return Compare(x, y, 0);
        }

        public int Compare(DataType x, DataType y, int count)
        {
            var typePair = new Tuple<DataType, DataType>(x, y);

            int d;

            // avoid infinite recursion
            if (compareResult.TryGetValue(typePair, out d))
                return d;

            compareResult[typePair] = 0;
            d = CompareInternal(x, y, count);
            compareResult[typePair] = d;

            return d;
        }

        public int CompareInternal(DataType x, DataType y, int count)
        {
            if (count > 20)
                throw new ApplicationException("Way too deep");     //$BUG: discover why datatypes recurse so deep.
            int prioX = x.Accept(this);
			int prioY = y.Accept(this);
			int dPrio = prioX - prioY;
			if (dPrio != 0)
				return dPrio;

            if (x is VoidType)
                return 0;

            if (x is UnknownType unkX && y is UnknownType unkY)
                return unkX.Size.CompareTo(unkY.Size);
            if (x is UnknownType)
                return -1;
            if (y is UnknownType)
                return 1;

			PrimitiveType ix = x as PrimitiveType;
			PrimitiveType iy = y as PrimitiveType;
			if (ix != null && iy != null)
			{
				return ix.Compare(iy);
			}
			if (ix != null)
				return -1;
			if (iy != null)
				return 1;

            if (x is EnumType || y is EnumType)
                throw new NotImplementedException();

            CodeType cx = x as CodeType;
            CodeType cy = y as CodeType;
            if (cx != null && cy != null)
            {
                return 0; 
            }
            if (cx != null)
                return -1;
            if (cy != null)
                return 1;

			TypeVariable tx = x as TypeVariable;
			TypeVariable ty = y as TypeVariable;
			if (tx != null && ty != null)
			{
				return tx.Number - ty.Number;
			}

            TypeReference tr_x = x as TypeReference;
            TypeReference tr_y = y as TypeReference;
            if (tr_x != null && tr_y != null)
            {
                return StringComparer.InvariantCulture.Compare(tr_x.Name, tr_y.Name);
            }

			EquivalenceClass ex = x as EquivalenceClass;
			EquivalenceClass ey = y as EquivalenceClass;
			if (ex != null && ey != null)
			{
				return ex.Number - ey.Number;
			}

			Pointer ptrX = x as Pointer;
			Pointer ptrY = y as Pointer;
			if (ptrX != null && ptrY != null)
			{
				return Compare(ptrX.Pointee, ptrY.Pointee, ++count);
			}

			MemberPointer mX = x as MemberPointer;
			MemberPointer mY = y as MemberPointer;
			if (mX != null && mY != null)
			{
				int d = Compare(mX.BasePointer, mY.BasePointer, ++count);
				if (d != 0)
					return d;
				return Compare(mX.Pointee, mY.Pointee, ++count);
			}

            ReferenceTo rX = x as ReferenceTo;
            ReferenceTo rY = y as ReferenceTo;
            if (rX != null && rY != null)
            {
                return Compare(rX.Referent, rY.Referent, ++count);
            }

			StructureType sX = x as StructureType;
			StructureType sY = y as StructureType;
			if (sX != null && sY != null)
			{
				return Compare(sX, sY, ++count);
			}

			UnionType ux = x as UnionType;
			UnionType uy = y as UnionType;
			if (ux != null && uy != null)
			{
				return Compare(ux, uy, ++count);
			}
			ArrayType ax = x as ArrayType;
			ArrayType ay = y as ArrayType;
			if (ax != null && ay != null)
			{
				return Compare(ax, ay, ++count);
			}

            StringType strX = x as StringType;
            StringType strY = y as StringType;
            if (strX != null && strY != null)
            {
                return Compare(strX, strY, ++count);
            }

            FunctionType fnX = x as FunctionType;
            FunctionType fnY = y as FunctionType;
            if (fnX != null && fnY != null)
            {
                return Compare(fnX, fnY, ++count);
            }
			throw new NotImplementedException(string.Format("NYI: comparison between {0} and {1}", x.GetType(), y.GetType()));
		}

		public int Compare(UnionType x, UnionType y, int count)
		{
			int d;
			d = x.Alternatives.Count - y.Alternatives.Count;
			if (d != 0)
				return d;
			++count;
            for (int i = 0; i < x.Alternatives.Count; ++i)
            {
				UnionAlternative ax = x.Alternatives.Values[i];
                UnionAlternative ay = y.Alternatives.Values[i];
				d = Compare(ax.DataType, ay.DataType, count);
				if (d != 0)
					return d;
			}
			return 0;
		}

		public int Compare(ArrayType x, ArrayType y, int count)
		{
			int d = Compare(x.ElementType, y.ElementType, ++count);
			if (d != 0)
				return d;
			return x.Length - y.Length;
		}

        public int Compare(StringType x, StringType y, int count)
        {
            int d = Compare(x.ElementType, y.ElementType, ++count);
            if (d != 0)
                return d;
            if (x.LengthPrefixType == null && y.LengthPrefixType == null)
                return 0;
            if (x.LengthPrefixType == null)
                return -1;
            if (y.LengthPrefixType == null)
                return 1;
            return Compare(x.LengthPrefixType, y.LengthPrefixType, ++count);
        }

		public int Compare(StructureType x, StructureType y, int count)
		{
			int d;
			if (x.Size > 0 && y.Size > 0)
			{
				d = x.Size - y.Size;
				if (d != 0)
					return d;
			}
			d = x.Fields.Count - y.Fields.Count;
			if (d != 0)
				return d;

			++count;
			IEnumerator<StructureField> ex = x.Fields.GetEnumerator();
            IEnumerator<StructureField> ey = y.Fields.GetEnumerator();
			while (ex.MoveNext())
			{
				ey.MoveNext();
				StructureField fx = ex.Current;
				StructureField fy = ey.Current;
				d = fx.Offset - fy.Offset;
				if (d != 0)
					return d;
				d = Compare(fx.DataType, fy.DataType, count);
				if (d != 0)
					return d;
			}
			return 0;
		}

        public int Compare(FunctionType x, FunctionType y, int count)
        {
            int d = x.Parameters.Length - y.Parameters.Length;
            if (d != 0)
                return d;
            ++count;
            for (int i = 0; i < x.Parameters.Length; ++i)
            {
                d = Compare(x.Parameters[i].DataType, y.Parameters[i].DataType, count);
                if (d != 0)
                    return d;
            }
            return Compare(x.ReturnValue.DataType, y.ReturnValue.DataType, count);
        }

        public bool Equals(DataType a, DataType b)
        {
            return Compare(a, b) == 0;
        }

        public int GetHashCode(DataType dt)
        {
            switch (dt)
            {
            case PrimitiveType pt:
                return pt.GetHashCode();
            case UnknownType _:
                return dt.GetType().GetHashCode();
            case Pointer ptr:                ;
                return GetHashCode(ptr.Pointee) * 11 ^ ptr.GetType().GetHashCode();
            case ReferenceTo rt:
                return GetHashCode(rt.Referent) * 11 ^ rt.GetType().GetHashCode();
            case FunctionType ft:
                if (ft.ParametersValid)
                {
                    int hash = 0;
                    if (ft.ReturnValue != null)
                    {
                        hash = GetHashCode(ft.ReturnValue.DataType);
                    }
                    foreach (var p in ft.Parameters)
                    {
                        hash = hash * 11 ^ GetHashCode(p.DataType);
                    }
                    return hash;
                }
                else
                {
                    return ft.ReturnAddressOnStack + ft.StackDelta;
                }
            }
            return dt.GetType().GetHashCode();
        }

		#region IDataTypeVisitor Members /////////////////////////////////////////

		public int VisitArray(ArrayType at)
		{
			return Array;
		}

        public int VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public int VisitCode(CodeType c)
        {
            return Code;
        }

        public int VisitEnum(EnumType e)
        {
            return Enum;
        }

		public int VisitEquivalenceClass(EquivalenceClass eq)
		{
			return EqClass;
		}

		public int VisitFunctionType(FunctionType ft)
		{
			return Fn;
		}

		public int VisitMemberPointer(MemberPointer memptr)
		{
			return MemPtr;
		}

		public int VisitPrimitive(PrimitiveType pt)
		{
			return Prim;
		}

        public int VisitString(StringType str)
        {
            return String;
        }

		public int VisitStructure(StructureType str)
		{
			return Struct;
		}

		public int VisitPointer(Pointer ptr)
		{
			return Ptr;
		}

        public int VisitReference(ReferenceTo refTo)
        {
            return Ref;
        }

        public int VisitTypeReference(TypeReference typeref)
        {
            return TRef;
        }

		public int VisitTypeVariable(TypeVariable tv)
		{
			return TVar;
		}

		public int VisitUnion(UnionType ut)
		{
			return Union;
		}

		public int VisitUnknownType(UnknownType ut)
		{
			return Unk;
		}

        public int VisitVoidType(VoidType vt)
        {
            return Void;
        }
		#endregion

        //$REVIEW: this is thread-unsafe. We keep it because we have really deep type comparisons due to
        // unresolved bugs in type inference. Once those are resolved, performance should improve.
		public static DataTypeComparer Instance => ourGlobalComparer;
	}
}
