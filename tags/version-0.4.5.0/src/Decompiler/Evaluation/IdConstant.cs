 #region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Typing;
using System;

namespace Decompiler.Evaluation
{
	/// <summary>
	/// Implements constant propagation.
	/// </summary>
    public class IdConstant
    {
        private EvaluationContext ctx;
        private Unifier unifier;
        private Constant cSrc;
        private Identifier idDst;

        public IdConstant(EvaluationContext ctx, Unifier u)
        {
            this.ctx = ctx;
            this.unifier = u;
        }

        public bool Match(Identifier id)
        {
            Expression e = ctx.GetValue(id);
            cSrc = e as Constant;
            if (cSrc == null || !cSrc.IsValid)
                return false;
            idDst = id;
            return true;
        }

        public Expression Transform()
        {
            ctx.RemoveIdentifierUse(idDst);
            if (!cSrc.IsValid)
                return cSrc;
            DataType dt = unifier.Unify(cSrc.DataType, idDst.DataType);
            if (dt is PrimitiveType)
                return Constant.Create(dt, cSrc.ToInt64());
            throw new NotSupportedException(string.Format("Resulting type is {0}, which isn't supported yet.", dt));
        }
    }
}