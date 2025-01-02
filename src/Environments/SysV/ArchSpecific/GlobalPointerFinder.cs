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
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class GlobalPointerFinder
    {
        public static Constant? RiscV(IEnumerable<RtlInstructionCluster> enumerable)
        {
            const StorageDomain GpDomain = (StorageDomain) 3;

            var y = enumerable.SelectMany(s => s.Instructions)
                .Select(r => r as RtlAssignment)
                .TakeWhile(r => r is not null)
                .Take(10);
            var state = new Dictionary<Storage, Constant>();
            foreach (var rtl in y)
            {
                Debug.Assert(rtl is not null);
                if (rtl.Dst is Identifier id && id.Storage is RegisterStorage reg)
                {
                    var value = Evaluate(rtl.Src, state);
                    if (value is not null)
                        state[reg] = value;
                }
            }
            return state.Select(r => r.Key.Domain == GpDomain ? r.Value : null)
                .FirstOrDefault(c => c is not null);
        }

        private static Constant? Evaluate(Expression e, Dictionary<Storage, Constant> state)
        {
            switch (e)
            {
            case Constant c:
                return c;
            case Address addr:
                return Constant.Create(addr.DataType, addr.ToLinear());
            case Identifier i:
                if (!state.TryGetValue(i.Storage, out var value))
                    return null;
                return value;
            case BinaryExpression b:
                var left = Evaluate(b.Left, state);
                if (left is null)
                    return null;
                var right = Evaluate(b.Right, state);
                if (right is null)
                    return null;
                return b.Operator.ApplyConstants(b.DataType, left, right);
            default:
                return null;
            }
        }
    }
}
