#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

namespace Reko.Evaluation
{
	/// <summary>
	/// Implements constant propagation. Addresses are considered
    /// constants.
	/// </summary>
    public class IdConstant
    {
        private EvaluationContext ctx;
        private readonly Unifier unifier;
        private readonly DecompilerEventListener listener;
        private Expression? src;
        private Identifier? idDst;
        private DataType? dt;
        private PrimitiveType? pt;
        private Pointer? ptr;

        public IdConstant(EvaluationContext ctx, Unifier u, DecompilerEventListener listener)
        {
            this.ctx = ctx;
            this.unifier = u;
            this.listener = listener;
        }

        public bool Match(Identifier id)
        {
            this.src = ctx.GetValue(id);
            var cSrc = src as Constant;
            if (cSrc == null || !cSrc.IsValid)
            {
                if (!(src is Address))
                    return false;
            }
            idDst = id;
            this.dt = unifier.Unify(src!.DataType, idDst.DataType);
            this.pt = dt!.ResolveAs<PrimitiveType>();
            this.ptr = dt.ResolveAs<Pointer>();
            return pt != null || this.ptr != null;
        }

        public Expression Transform()
        {
            if (this.pt != null)
            {
                ctx.RemoveIdentifierUse(idDst!);
                var pt = idDst!.DataType.ResolveAs<PrimitiveType>();
                var cNew = src!.CloneExpression();
                if (src.DataType.IsWord &&
                    src is Constant cSrc &&
                    pt != null &&
                    pt.Domain == Domain.Real)
                {
                    // Raw bitvector assigned to an real-valued register. We need to interpret the bitvector
                    // as a floating-point constant.
                    cNew = Constant.RealFromBitpattern(pt, cSrc.ToInt64());
                }
                cNew.DataType = dt!;
                return cNew;
            }
            if (this.ptr != null)
            {
                if (src is Constant cSrc)
                {
                    ctx.RemoveIdentifierUse(idDst!);
                    var addr = Address.Create(ptr, cSrc.ToUInt64());
                    addr.DataType = ptr;
                    return addr;
                }
                if (src is Address)
                {
                    ctx.RemoveIdentifierUse(idDst!);
                    var addr = src.CloneExpression();
                    addr.DataType = ptr;
                    return addr;
                }
            }
            listener.Warn(
                "Constant propagation failed. Resulting type is {0}, which isn't supported yet.", 
                dt!);
            return idDst!;
        }
    }
}