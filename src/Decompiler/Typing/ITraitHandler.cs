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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;

namespace Reko.Typing
{
	/// <summary>
	/// The methods of this interface are called by the TraitCollector as it traverses 
	/// the instructions and expressions of the program. The methods return a reference
    /// to the new data type.
	/// </summary>
	public interface ITraitHandler
	{
		/// <summary>
		/// The array <paramref>tArray</paramref> has <paramRef>elementSize</paramRef> and consists of <paramref>length</paramref> elements.
		/// </summary>
		void ArrayTrait(TypeVariable tArray, int elementSize, int length);

		void BuildEquivalenceClassDataTypes();

		/// <summary>
		/// <paramref>exp</paramref> has the data type <paramref>dataType</paramref>
		/// </summary>
        DataType DataTypeTrait(Expression exp, DataType dataType);

		/// <summary>
		/// The types <paramref>exp1</paramref> and <paramref>exp2</paramref> are assignable
		/// </summary>
		DataType EqualTrait(Expression exp1, Expression exp2);

		/// <summary>
		/// <paramref>function</paramref> is a function pointer whose return type is <paramref>ret</paramref> and whose
		/// actual parameters in this call are <paramref>actuals</paramref>
		/// </summary>
		DataType FunctionTrait(Expression function, int funcPtrSize, TypeVariable ret, params TypeVariable [] actuals);

		/// <summary>
		/// <paramref>tStruct</paramref> has a field of type <paramref>tField</paramref> at offset <paramref>offset</paramref>. Optionally,
		/// the memory access is a member pointer based on <paramref>tBase</paramref>
		/// </summary>
		/// <param name="eBase">Base pointer for member pointer accesses, or null for simple pointer accesses.</param>
		/// <param name="eStruct">Type variable of the structure whose field is being accessed.</param>
		/// <param name="structPtrSize">Size of the pointer associated with the structure field reference.</param>
		/// <param name="eField">Type variable for the field.</param>
		/// <param name="offset">Field offset within the structure (in bytes).</param>
        DataType MemAccessTrait(Expression eBase, Expression eStruct, int structPtrSize, Expression eField, int offset);
        DataType MemFieldTrait(Expression eBase, Expression eStruct, Expression eField, int offset);

		/// <summary>
		/// <paramref>tStruct</paramref> has an array at offset <paramref>offset</paramref> whose elementsize is <paramref>elementSize</paramref>
		/// and consists of <paramref>length items</paramref>.
		/// </summary>
		DataType MemAccessArrayTrait(Expression tBase, Expression tStruct, int structPtrSize, int offset, int elementSize, int length, Expression tField);

		DataType MemSizeTrait(Expression tBase, Expression tStruct, int size);
		DataType PointerTrait(Expression tPointer, int ptrSize, Expression tPointee);
	}
}
