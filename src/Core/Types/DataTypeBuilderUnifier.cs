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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Core.Types
{
    public class DataTypeBuilderUnifier : Unifier
    {
        private ITypeStore store;

        public DataTypeBuilderUnifier(TypeFactory factory, ITypeStore store)
            : base(factory, null)
        {
            this.store = store;
        }

        public override DataType UnifyTypeVariables(TypeVariable tA, TypeVariable tB)
        {
            var dt = Unify(tA.Class.DataType, tB.Class.DataType);
            var eq = store.MergeClasses(tA, tB);
            eq.DataType = dt;
            return eq.Representative;
        }
    }
}
