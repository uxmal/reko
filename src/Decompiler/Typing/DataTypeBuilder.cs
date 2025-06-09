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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Linq;

namespace Reko.Typing
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
		private readonly ITypeStore store;
		private readonly TypeFactory factory;
		private readonly DataTypeBuilderUnifier unifier;
        private readonly IPlatform platform;

		public DataTypeBuilder(TypeFactory factory, ITypeStore store, IPlatform platform)
		{
			this.store = store;
			this.factory = factory;
			this.unifier = new DataTypeBuilderUnifier(factory, store);
            this.platform = platform;
		}

		public void BuildEquivalenceClassDataTypes()
		{
            store.BuildEquivalenceClassDataTypes(factory);
		}

        public DataType MergeIntoDataType(DataType dtNew, TypeVariable tv)
        {
            if (dtNew is null)
                return tv.OriginalDataType;

            DataType dtCurrent = tv.OriginalDataType;
            if (dtCurrent is not null)
            {
                dtNew = unifier.Unify(dtCurrent, dtNew)!;
            }
            tv.OriginalDataType = dtNew;
            return dtNew;
        }

        public DataType MergeIntoDataType(Expression exp, DataType? dtNew)
        {
            if (dtNew is null)
                return exp.DataType;

            DataType? dtCurrent = store.GetDataTypeOf(exp);
            if (dtCurrent is not null)
            {
                var u = unifier.Unify(dtCurrent, dtNew)!;
                dtNew = u;
            }
            store.SetDataTypeOf(exp, dtNew);
            return dtNew;
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
				Pointer ptr = factory.CreatePointer(seg, dt.BitSize);
				dt = ptr;
			}
			MergeIntoDataType(dt, type);
		}

        public DataType DataTypeTrait(Expression exp, DataType? dt)
        {
            if (dt == PrimitiveType.SegmentSelector)
            {
                var seg = factory.CreateStructureType(null, 0);
                seg.IsSegment = true;
                var ptr = factory.CreatePointer(seg, dt.BitSize);
                dt = ptr;
            }
            return MergeIntoDataType(exp, dt);
        }

		public DataType EqualTrait(Expression tv1, Expression tv2)
		{
            return null!;
		}

		public DataType FunctionTrait(Expression function, int funcPtrSize, TypeVariable ret, params TypeVariable [] actuals)
		{
            Identifier[] adt = actuals.Select(a => new Identifier("", a, null!)).ToArray();
			var fn = factory.CreateFunctionType(new Identifier("", ret, null!), adt);
			var pfn = factory.CreatePointer(fn, funcPtrSize * DataType.BitsPerByte);
			return MergeIntoDataType(function, pfn);
		}

		public DataType MemAccessArrayTrait(Expression? expBase, Expression expStruct, int structPtrBitSize, int offset, int elementSize, int length, Expression expField)
		{
			var element = factory.CreateStructureType(null, elementSize);
			if (expField is not null)
				element.Fields.Add(0, this.store.GetTypeVariable(expField));
            var tvElement = store.CreateTypeVariable(factory);
            tvElement.OriginalDataType = element;

			DataType dtArray = factory.CreateArrayType(tvElement, length);
		    return MemoryAccessCommon(expBase, expStruct, offset, dtArray, structPtrBitSize);
		}
		
		public DataType MemAccessTrait(Expression? tBase, Expression tStruct, int structPtrBitSize, Expression tField, int offset)
		{
			return MemoryAccessCommon(tBase, tStruct, offset, store.GetTypeVariable(tField), structPtrBitSize);
		}

		public DataType MemFieldTrait(Expression? tBase, Expression tStruct, Expression tField, int offset)
        {
            var s = factory.CreateStructureType(null, 0);
            var field = new StructureField(offset, store.GetTypeVariable(tField));
            s.Fields.Add(field);
            return MergeIntoDataType(tStruct, s);
        }

        public DataType MemoryAccessCommon(Expression? tBase, Expression tStruct, int offset, DataType tField, int structPtrBitSize)
        {
            var s = factory.CreateStructureType(null, 0);
            var field = new StructureField(offset, tField);
            s.Fields.Add(field);

            var pointer = tBase is not null
                ? (DataType)factory.CreateMemberPointer(store.GetTypeVariable(tBase), s, structPtrBitSize)
                : (DataType)factory.CreatePointer(s, structPtrBitSize);
            return MergeIntoDataType(tStruct, pointer);
        }

		public DataType MemSizeTrait(Expression? tBase, Expression tStruct, int size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException("size must be positive");
			var s = factory.CreateStructureType(null, size);
			var ptr = tBase is not null
                ? (DataType)factory.CreateMemberPointer(store.GetTypeVariable(tBase), s, platform.FramePointerType.Size)
				: (DataType)factory.CreatePointer(s, platform.PointerType.BitSize);
			return MergeIntoDataType(tStruct, ptr);
		}

		public DataType PointerTrait(Expression ptrExp, int ptrSize, Expression tPointee)
		{
			var ptr = factory.CreatePointer(store.GetDataTypeOf(tPointee)!, ptrSize * DataType.BitsPerByte);
            return MergeIntoDataType(ptrExp, ptr);
		}
		#endregion
	}
}
