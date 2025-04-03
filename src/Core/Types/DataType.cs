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

using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.Core.Types
{
	/// <summary>
	/// Represents concrete C-like data types inferred by the decompiler as 
    /// part of the decompilation process.
	/// </summary>
	/// <remarks>
	/// The name 'DataType' is used to avoid conflicts with <see cref="System.Type" />,
    /// which is part of the CLR.
	/// </remarks>
	public abstract class DataType : ICloneable
	{
		public const int BitsPerByte = 8;

        private string? name;

		protected DataType(Domain domain)
		{
            this.Domain = domain;
		}

		protected DataType(Domain domain, string? name)
		{
            this.Domain = domain;
			this.name = name!;
		}

        public virtual int BitSize { get { return Size * BitsPerByte; } }       //$REVIEW: Wrong for 36-bit machines

        /// <summary>
        /// The type <see cref="Domain"/> of this <see cref="DataType"/>.
        /// </summary>
        public Domain Domain { get; protected set; }

        public virtual bool IsComplex { get { return false; } }
        public virtual bool IsPointer { get { return false; } }
        public virtual bool IsIntegral { get { return false; } }
        public virtual bool IsReal => false;

        /// <summary>
        /// The name of this data type.
        /// </summary>
        public virtual string Name { get { return name!; } set { name = value; } }

        /// <summary>
        /// Zero or more type qualifiers of this data type.
        /// </summary>
        public Qualifier Qualifier { get; set; }

        /// <summary>
        /// Size of the data type measured in storage units.
        /// </summary>
        /// <remarks>
        /// Storage units are commonly, but not always, eight-bit octets, or "bytes".
        /// </remarks>
        public abstract int Size { get; set; }

        public abstract void Accept(IDataTypeVisitor v);
        public abstract T Accept<T>(IDataTypeVisitor<T> v);
        public abstract DataType Clone(IDictionary<DataType, DataType>? clonedTypes);
        object ICloneable.Clone() { return Clone(); }

        public DataType Clone()
        {
            return Clone(null);
        }

        /// <summary>
        /// Compute the size of the data type.
        /// </summary>
        /// <remarks>
        /// We don't trust the <see cref="DataType.Size"/> property on complex types
        /// because the types may be inferred.
        /// For instance, inferred <see cref="StructureType"/>s
        /// don't have a value for their Size properties.
        /// </remarks>
        //$TODO: convert to virtual method.
        public int MeasureSize()
        {
            DataType dt = this;
            int offset = 0;
            for (; ; )
            {
                switch (dt)
                {
                case PrimitiveType _:
                case Pointer _:
                case MemberPointer _:
                case VoidType _:
                case UnknownType _:
                case EnumType _:
                    return offset + dt.Size;
                case StructureType st:
                    if (st.Size > 0)
                        return st.Size; // Trust the user/metadata
                    if (st.Fields.Count == 0)
                        return 0;
                    var firstField = st.Fields[0];
                    offset -= Math.Min(0, firstField.Offset);
                    var field = st.Fields[^1];
                    offset += field.Offset;
                    dt = field.DataType;
                    break;
                case UnionType ut:
                    int unionSize = 0;
                    foreach (var alt in ut.Alternatives.Values)
                    {
                        unionSize = Math.Max(unionSize, alt.DataType.MeasureSize());
                    }
                    return offset + unionSize;
                case ArrayType array:
                    var elemSize = array.ElementType.MeasureSize();
                    return offset + elemSize * array.Length;
                case ClassType cls:
                    if (cls.Size > 0)
                        return cls.Size; // Trust the user/metadata
                    if (cls.Fields.Count == 0)
                        return 0;
                    var cfield = cls.Fields[^1];
                    if (cfield is null)
                        return 0;
                    offset += cfield.Offset;
                    dt = cfield.DataType;
                    break;
                case TypeVariable tv:
                    dt = tv.DataType;
                    break;
                case EquivalenceClass eq:
                    dt = eq.DataType;
                    break;
                case TypeReference tref:
                    dt = tref.Referent;
                    break;
                case ReferenceTo refto:
                    dt = refto.Referent;
                    break;
                case CodeType _:
                case FunctionType _:
                    // Functions can vary in size, but must be at least this big.
                    dt = PrimitiveType.Byte;
                    break;
                default:
                    throw new NotImplementedException($"MeasureSize: {dt.GetType().Name} not implemented.");
                }
            }
        }

        public int MeasureBitSize(int bitsPerUnit)
        {
            DataType dt = this;
            int bitOffset = 0;
            for (; ; )
            {
                switch (dt)
                {
                case PrimitiveType _:
                case Pointer _:
                case MemberPointer _:
                case VoidType _:
                case UnknownType _:
                case EnumType _:
                    return bitOffset + dt.BitSize;
                case StructureType st:
                    if (st.Size > 0)
                        return st.Size * bitsPerUnit; // Trust the user/metadata
                    if (st.Fields.Count == 0)
                        return 0;
                    var firstField = st.Fields[0];
                    bitOffset -= Math.Min(0, bitsPerUnit * firstField.Offset);
                    var field = st.Fields[^1];
                    bitOffset += field.Offset * bitsPerUnit;
                    dt = field.DataType;
                    break;
                case UnionType ut:
                    int unionBitSize = 0;
                    foreach (var alt in ut.Alternatives.Values)
                    {
                        unionBitSize = Math.Max(unionBitSize, alt.DataType.MeasureBitSize(bitsPerUnit));
                    }
                    return bitOffset + unionBitSize;
                case ArrayType array:
                    var elemBitSize = array.ElementType.MeasureBitSize(bitsPerUnit);
                    return bitOffset + elemBitSize * array.Length;
                case ClassType cls:
                    if (cls.Size > 0)
                        return cls.Size * bitsPerUnit; // Trust the user/metadata
                    if (cls.Fields.Count == 0)
                        return 0;
                    var cfield = cls.Fields[^1];
                    if (cfield is null)
                        return 0;
                    bitOffset += cfield.Offset * bitsPerUnit;
                    dt = cfield.DataType;
                    break;
                case TypeVariable tv:
                    dt = tv.DataType;
                    break;
                case EquivalenceClass eq:
                    dt = eq.DataType;
                    break;
                case TypeReference tref:
                    dt = tref.Referent;
                    break;
                case ReferenceTo refto:
                    dt = refto.Referent;
                    break;
                case CodeType _:
                case FunctionType _:
                    dt = PrimitiveType.Byte;
                    break;
                default:
                    throw new NotImplementedException($"MeasureSize: {dt.GetType().Name} not implemented.");
                }
            }
        }

        public T? ResolveAs<T>() where T : DataType
        {
            DataType dt = this;
            // Special case: ResolveAs<TypeReference> or ResolveAs<DataType>
            if ((dt is TypeReference) && (dt is T))
                return dt as T;
            TypeReference? typeRef = dt as TypeReference;
            while (typeRef != null)
            {
                dt = typeRef.Referent;
                typeRef = dt as TypeReference;
            }
            TypeVariable? tv = dt as TypeVariable;
            while (tv != null)
            {
                if (tv.Class is null)
                    return null;
                dt = tv.Class.DataType ?? tv.DataType;
                tv = dt as TypeVariable;
            }     
            EquivalenceClass? eq = dt as EquivalenceClass;
            while (eq != null)
            {
                dt = eq.DataType;
                eq = dt as EquivalenceClass;
            }
            return dt as T;
        }

        public T? TypeReferenceAs<T>() where T : DataType
        {
            DataType dt = this;
            TypeReference? typeRef = dt as TypeReference;
            while (typeRef != null)
            {
                dt = typeRef.Referent;
                typeRef = dt as TypeReference;
            }
            return dt as T;
        }

        /// <summary>
        /// This property is true if the type refers to a datum solely defined by its size.
        /// </summary>
        public virtual bool IsWord => false;

        protected void ThrowBadSize()
		{
			throw new InvalidOperationException($"Can't set size of {GetType().Name}.");
		}

		public sealed override string ToString()
		{
            var sw = new StringWriter();
            var typeGraphWriter = new TypeGraphWriter(new TextFormatter(sw));
            this.Accept(typeGraphWriter);
            return sw.ToString();
		}
    }
}
