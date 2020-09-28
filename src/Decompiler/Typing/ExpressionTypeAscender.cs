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
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Typing
{
    /// <summary>
    /// Collect type information by pulling type information from
    /// the leaves of expression trees to their roots.
    /// </summary>
    /// <remarks>
    ///    root
    ///  ↑ /  \ ↑
    /// leaf  leaf
    /// </remarks>
    public class ExpressionTypeAscender : ExpressionTypeAscenderBase
    {
        private Unifier unifier;

        public ExpressionTypeAscender(
            Program program,
            TypeStore store,
            TypeFactory factory) :
                base(program, factory)
        {
            this.unifier = new DataTypeBuilderUnifier(factory, store);
        }

        protected override DataType RecordDataType(DataType dt, Expression exp)
        {
            exp.TypeVariable.DataType = unifier.Unify(exp.TypeVariable.DataType, dt);
            exp.TypeVariable.OriginalDataType = unifier.Unify(exp.TypeVariable.OriginalDataType, dt);
            return exp.TypeVariable.DataType;
        }

        protected override DataType EnsureDataType(DataType dt, Expression exp)
        {
            if (exp.TypeVariable.DataType == null)
            {
                exp.TypeVariable.DataType = dt;
                exp.TypeVariable.OriginalDataType = dt;
            }
            return exp.TypeVariable.DataType;
        }
    }
}
