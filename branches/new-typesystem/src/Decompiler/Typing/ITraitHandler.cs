/* 
 * Copyright (C) 1999-2008 John Källén.
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
	/// The methods of this interface are called by the TraitCollector as it traverses 
	/// the program. 
	/// </summary>
	public interface ITraitHandler
	{
		/// <summary>
		/// The array <paramref>tArray</paramref> has <paramRef>elementSize</paramRef> and consists of <paramref>length</paramref> elements.
		/// </summary>
		void ArrayTrait(TypeVariable tArray, int elementSize, int length);

		void BuildEquivalenceClassDataTypes();

		/// <summary>
		/// <paramref>type</paramref> has the data type <paramref>dataType</paramref>
		/// </summary>
		void DataTypeTrait(TypeVariable type, DataType dataType);

		/// <summary>
		/// The types <paramref>tv1</paramref> and <paramref>tv2</paramref> are assignable
		/// </summary>
		void EqualTrait(TypeVariable tv1, TypeVariable tv2);

		/// <summary>
		/// <paramref>function</paramref> is a function pointer whose return type is <paramref>ret</paramref> and whose
		/// actual parameters in this call are <paramref>actuals</paramref>
		/// </summary>
		void FunctionTrait(TypeVariable function, int funcPtrSize, TypeVariable ret, params TypeVariable [] actuals);

		/// <summary>
		/// <paramref>tStruct</paramref> has a field of type <paramref>tField</paramref> at offset <paramref>offset</paramref>. Optionally,
		/// the memory access is a member pointer based on <paramref>tBase</paramref>
		/// </summary>
		void MemAccessTrait(TypeVariable tBase, TypeVariable tStruct, int structPtrSize, TypeVariable tField, int offset);

		/// <summary>
		/// <paramref>tStruct</paramref> has an array at offset <paramref>offset</paramref> whose elementsize is <paramref>elementSize</paramref>
		/// and consists of <paramref>length items</paramref>.
		/// </summary>
		void MemAccessArrayTrait(TypeVariable tBase, TypeVariable tStruct, int structPtrSize, int offset, int elementSize, int length, TypeVariable tAccess);

		void MemSizeTrait(TypeVariable tBase, TypeVariable tStruct, int size);
		void PointerTrait(TypeVariable tPointer, int ptrSize, TypeVariable tPointee);
		void UnknownTrait(TypeVariable tUnknown);

	}
}
