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
	/// The name 'DataType' is used to avoid conflicts with 'System.Type',
    /// which is part of the CLR.
	/// </remarks>
	public abstract class DataType : ICloneable
	{
		public const int BitsPerByte = 8;

		protected DataType()
		{
		}

		protected DataType(string name)
		{
			this.Name = name;
		}

        public virtual int BitSize { get { return Size * BitsPerByte; } }		//$REVIEW: Wrong for 36-bit machines
        public virtual bool IsComplex { get { return false; } }
        public virtual bool IsPointer { get { return false; } }
        public virtual bool IsIntegral { get { return false; } }
        public virtual string Name { get; set; }
        public Qualifier Qualifier { get; set; }
        public abstract int Size { get; set; }  // Size in bytes of the concrete datatype.

        public abstract void Accept(IDataTypeVisitor v);
        public abstract T Accept<T>(IDataTypeVisitor<T> v);
        public abstract DataType Clone(IDictionary<DataType, DataType> clonedTypes);
        //public abstract int GetInferredSize();                  // Computes the size of an item.
        object ICloneable.Clone() { return Clone(); }

        public DataType Clone()
        {
            return Clone(null);
        }

        public T ResolveAs<T>() where T : DataType
        {
            DataType dt = this;
            // Special case: ResolveAs<TypeReference> or ResolveAs<DataType>
            if ((dt is TypeReference) && (dt is T))
                return dt as T;
            TypeReference typeRef = dt as TypeReference;
            while (typeRef != null)
            {
                dt = typeRef.Referent;
                typeRef = dt as TypeReference;
            }
            TypeVariable tv = dt as TypeVariable;
            while (tv != null)
            {
                dt = tv.Class.DataType ?? tv.DataType;
                tv = dt as TypeVariable;
            }     
            EquivalenceClass eq = dt as EquivalenceClass;
            while (eq != null)
            {
                dt = eq.DataType;
                eq = dt as EquivalenceClass;
            }
            return dt as T;
        }

        public T TypeReferenceAs<T>() where T : DataType
        {
            DataType dt = this;
            TypeReference typeRef = dt as TypeReference;
            while (typeRef != null)
            {
                dt = typeRef.Referent;
                typeRef = dt as TypeReference;
            }
            return dt as T;
        }

        public bool IsWord()
        {
            if (BitSize == 0)
                return false;
            //$REFACTOR: CreateWord is inefficient.
            var wordType = PrimitiveType.CreateWord(BitSize);
            return wordType == this;
        }

        protected void ThrowBadSize()
		{
			throw new InvalidOperationException(string.Format("Can't set size of {0}.", GetType().Name));
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
