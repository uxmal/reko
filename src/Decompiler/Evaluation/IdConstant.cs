#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Typing;
using System;
using System.Diagnostics;

namespace Reko.Evaluation
{
	/// <summary>
	/// Implements constant propagation. Addresses are considered
    /// constants.
	/// </summary>
    public class IdConstant
    {
        public IdConstant()
        {
        }

        public Expression? Match(Identifier id, EvaluationContext ctx, Unifier unifier, DecompilerEventListener listener)
        {
            var src = ctx.GetValue(id);
            var cSrc = src as Constant;
            if (cSrc is null || !cSrc.IsValid)
            {
                if (src is not Address)
                    return null;
            }
            var idDst = id;
            Debug.Assert(src is not null);
            var dt = unifier.Unify(src.DataType, idDst.DataType);
            var pt = dt?.ResolveAs<PrimitiveType>();
            var ptr = dt?.ResolveAs<Pointer>();
            if (pt == null && ptr == null)
                return null;
            if (pt != null)
            {
                ctx.RemoveIdentifierUse(idDst);
                var ptDst = idDst.DataType.ResolveAs<PrimitiveType>();
                var cNew = src!.CloneExpression();
                if (src.DataType.IsWord &&
                    cSrc is not null &&
                    ptDst is not null &&
                    ptDst.Domain == Domain.Real)
                {
                    // Raw bitvector assigned to an real-valued register. We need to interpret the bitvector
                    // as a floating-point constant.
                    cNew = Constant.RealFromBitpattern(pt, cSrc);
                }
                cNew.DataType = dt!;
                return cNew;
            }
            if (ptr != null)
            {
                if (cSrc is not null)
                {
                    ctx.RemoveIdentifierUse(idDst);
                    var addr = Address.Create(ptr, cSrc.ToUInt64());
                    addr.DataType = ptr;
                    return addr;
                }
                if (src is Address)
                {
                    ctx.RemoveIdentifierUse(idDst);
                    var addr = src.CloneExpression();
                    addr.DataType = ptr;
                    return addr;
                }
            }
            listener.Warn(
                "Constant propagation failed. Resulting type is {0}, which isn't supported yet.", 
                dt!);
            return idDst;
        }
    }
}