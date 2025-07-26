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
using Reko.Core.Services;
using Reko.Core.Types;
using System.Diagnostics;

namespace Reko.Evaluation
{
    /// <summary>
    /// Implements constant propagation. Addresses are considered
    /// constants.
    /// </summary>
    public class IdConstant
    {
        /// <summary>
        /// Perform constant propagation on an identifier, unifying the
        /// data type of the identifier with the data type of the source.
        /// </summary>
        /// <param name="id">Identtifier to constant propagate.</param>
        /// <param name="ctx">Evaluation context to use.</param>
        /// <param name="unifier">Unifier used to unify constant value with identifier use.</param>
        /// <param name="listener">Used for diagnostic output.</param>
        /// <returns>Either a constant or a null expression if no
        /// matching constant was found.
        /// </returns>
        public Expression? Match(Identifier id, EvaluationContext ctx, Unifier unifier, IEventListener listener)
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
            if (ptr is null)
            {
                var cNew = src!.CloneExpression();
                if (src.DataType.IsWord &&
                    cSrc is not null &&
                    idDst.DataType.Domain == Domain.Real)
                {
                    // Raw bitvector assigned to an real-valued register. We need to interpret the bitvector
                    // as a floating-point constant.
                    cNew = Constant.RealFromBitpattern(idDst.DataType, cSrc);
                }
                cNew.DataType = dt!;
                return cNew;
            }
            else
            {
                if (cSrc is not null)
                {
                    var addr = Address.Create(ptr, cSrc.ToUInt64());
                    addr.DataType = ptr;
                    return addr;
                }
                if (src is Address)
                {
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