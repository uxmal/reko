#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
    /// Models the floating point division operator.
    /// </summary>
    public class FDivOperator : BinaryOperator
    {
        internal FDivOperator() : base(OperatorType.FDiv) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
        {
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(dt);
            return BuildConstant(dt, c2.DataType, c1.ToReal64() / c2.ToReal64());
        }

        /// <inheritdoc/>
        public override string AsCompound()
        {
            return " /= ";
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return " / ";
        }
    }
}
