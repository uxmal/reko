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
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Builds data types incrementally by accepting traits and modifying the corresponding
	/// type accordingly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Much of the type inference code in this namespace was inspired by the master's thesis
	/// "Entwicklung eines Typanalysesystem für einen Decompiler", 2004, by Raimar Falke.
	/// </para>
	/// </remarks>
	public class DataTypeBuilder : ITraitHandler
	{
		private TypeStore store;
		private TypeFactory factory;
		private DataTypeBuilderUnifier unifier;
        private IProcessorArchitecture arch;

		public DataTypeBuilder(TypeFactory factory, TypeStore store, IProcessorArchitecture arch)
		{
			this.store = store;
			this.factory = factory;
			this.unifier = new DataTypeBuilderUnifier(factory, store);
            this.arch = arch;
		}

		public UnionType MakeUnion(DataType a, DataType b)
		{
			throw new NotImplementedException();
		}

		public void BuildEquivalenceClassDataTypes()
		{
			UnionTypeVarsReplacer utv = new UnionTypeVarsReplacer(store);
			foreach (TypeVariable tv in store.TypeVariables)
			{
				if (tv.OriginalDataType != null)
					tv.OriginalDataType.Accept(utv);
			}
			Unifier u = new DataTypeBuilderUnifier(factory, store);
			foreach (TypeVariable tv in store.TypeVariables)
			{
                DataType dt = tv.OriginalDataType;
				EquivalenceClass c = tv.Class;
				DataType dtOld = c.DataType;
				if (dtOld != null)
					dt = u.Unify(dt, dtOld);
				else if (dt != null)
					dt = dt.Clone();
				c.DataType = dt;
			}
		}

        public void MergeIntoDataType(DataType dtNew, TypeVariable tv)
        {
            if (dtNew == null)
                return;

            DataType dtCurrent = tv.OriginalDataType; ;
            if (dtCurrent != null)
            {
                dtNew = unifier.Unify(dtCurrent, dtNew);
            }
            tv.OriginalDataType = dtNew;
        }


		#region ITraitHandler Members

		public void ArrayTrait(TypeVariable tArray, int elementSize, int length)
		{
			DataType elem = factory.CreateStructureType(null, elementSize);
			Pointer ptr = factory.CreatePointer(factory.CreateArrayType(elem, length), 0);
			MergeIntoDataType(ptr, tArray);
		}

		public void DataTypeTrait(TypeVariable type, DataType dt)
		{
			if (dt == PrimitiveType.SegmentSelector)
			{
				StructureType seg = factory.CreateStructureType(null, 0);
				seg.IsSegment = true;
				Pointer ptr = factory.CreatePointer(seg, dt.Size);
				dt = ptr;
			}
			MergeIntoDataType(dt, type);
		}

		public void EqualTrait(TypeVariable tv1, TypeVariable tv2)
		{
		}

		public void FunctionTrait(TypeVariable function, int funcPtrSize, TypeVariable ret, params TypeVariable [] actuals)
		{
			DataType [] adt = new DataType[actuals.Length];
			actuals.CopyTo(adt, 0);
			FunctionType f = factory.CreateFunctionType(null, ret, adt, null);
			Pointer ptr = factory.CreatePointer(f, funcPtrSize);
			MergeIntoDataType(ptr, function);
		}

		public void MemAccessArrayTrait(TypeVariable tBase, TypeVariable tStruct, int structPtrSize, int offset, int elementSize, int length, TypeVariable tField)
		{
			StructureType element = factory.CreateStructureType(null, elementSize);
			if (tField != null)
				element.Fields.Add(0, tField, null);
			ArrayType a = factory.CreateArrayType(element, length);

			MemoryAccessCommon(tBase, tStruct, new StructureField(offset, a), structPtrSize);
		}
		
		public void MemAccessTrait(TypeVariable tBase, TypeVariable tStruct, int structPtrSize, TypeVariable tField, int offset)
		{
			MemoryAccessCommon(tBase, tStruct, new StructureField(offset, tField), structPtrSize);
		}

		public void MemoryAccessCommon(TypeVariable tBase, TypeVariable tStruct, StructureField field, int structPtrSize)
		{
			StructureType s = factory.CreateStructureType(null, 0);
			s.Fields.Add(field);

			DataType pointer = tBase != null
				? (DataType)factory.CreateMemberPointer(tBase, s, structPtrSize)			//$REFACTOR: duplicated code (see memaccesstrait)
				: (DataType)factory.CreatePointer(s, structPtrSize);
			MergeIntoDataType(pointer, tStruct);
		}

		public void MemSizeTrait(TypeVariable tBase, TypeVariable tStruct, int size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException("size must be positive");
			StructureType s = factory.CreateStructureType(null, size);
			DataType ptr = tBase != null
				? (DataType)factory.CreateMemberPointer(tBase, s, arch.FramePointerType.Size)
				: (DataType)factory.CreatePointer(s, arch.PointerType.Size);
			MergeIntoDataType(ptr, tStruct);
		}

		public void PointerTrait(TypeVariable tPtr, int ptrSize, TypeVariable tPointee)
		{
			Pointer ptr = factory.CreatePointer(tPointee, ptrSize);
			MergeIntoDataType(ptr, tPtr);
		}

		public void UnknownTrait(TypeVariable tUnknown)
		{
			UnknownType unk = factory.CreateUnknown();
			MergeIntoDataType(unk, tUnknown);
		}
		#endregion
	}

	public class DataTypeBuilderUnifier : Unifier
	{
		private TypeStore store;
        private int nestedCalls;        //$DEBUG

		public DataTypeBuilderUnifier(TypeFactory factory, TypeStore store) : base(factory)
		{
			this.store = store;
		}

		public override DataType UnifyTypeVariables(TypeVariable tA, TypeVariable tB)
		{
            if (++nestedCalls > 300)        //$DEBUG
                nestedCalls.ToString();
            DataType dt = Unify(tA.Class.DataType, tB.Class.DataType);
            EquivalenceClass eq = store.MergeClasses(tA, tB);
            eq.DataType = dt;
            --nestedCalls;
            return eq.Representative;
		}
	}
}
