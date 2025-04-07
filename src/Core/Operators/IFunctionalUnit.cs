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

using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Core.Operators
{
    /// <summary>
    /// Represents a functional unit on a CPU. This unit can evaluate 
    /// constant data and create instances of the implementing class
    /// using the provided argument expressions.
    /// </summary>
    public interface IFunctionalUnit
    {
        /// <summary>
        /// Applies this functional unit to the given constant values, and
        /// returns a constant of type <paramref name="dtReturnType"/>.
        /// </summary>
        /// <param name="dtReturnType">The <see cref="DataType"/> of the 
        /// returned constant.</param>
        /// <param name="cs">The input constant arguments to this operation.
        /// </param>
        /// <returns>
        /// A <see cref="Constant"/> if the operation is successful, 
        /// null if it couldn't be completed.
        /// </returns>
        Constant? ApplyConstants(DataType dtReturnType, params Constant[] cs);

        /// <summary>
        /// Creates an instance of an application of this functional unit.
        /// </summary>
        /// <param name="dtReturnType">The return type of the application.
        /// </param>
        /// <param name="exprs">The input arguments to the operation.</param>
        /// <returns>A <see cref="Application"/> of this functional unit
        /// to its arguments.
        /// </returns>
        Expression Create(DataType dtReturnType, params Expression[] exprs);
    }
}