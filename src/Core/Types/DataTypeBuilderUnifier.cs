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

namespace Reko.Core.Types
{
    /// <summary>
    /// Unifies two <see cref="TypeVariable"/>s by merging their
    /// data types.
    /// </summary>
    public class DataTypeBuilderUnifier : Unifier
    {
        private readonly ITypeStore store;

        /// <summary>
        /// Constructs a <see cref="DataTypeBuilderUnifier"/> instance.
        /// </summary>
        /// <param name="factory">Type factory to use.</param>
        /// <param name="store">Type store to use.</param>
        public DataTypeBuilderUnifier(TypeFactory factory, ITypeStore store)
            : base(factory)
        {
            this.store = store;
        }

        /// <inheritdoc/>
        public override DataType UnifyTypeVariables(TypeVariable tA, TypeVariable tB)
        {
            var dt = Unify(tA.Class.DataType, tB.Class.DataType)!;
            var eq = store.MergeClasses(tA, tB);
            eq.DataType = dt;
            return eq.Representative;
        }
    }
}
