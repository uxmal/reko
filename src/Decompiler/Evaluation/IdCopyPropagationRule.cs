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
using Reko.Core.Code;
using Reko.Core.Expressions;
using System;

namespace Reko.Evaluation
{
	/// <summary>
	/// Implements copy propagation.
	/// </summary>
	public class IdCopyPropagationRule
	{
        private EvaluationContext ctx;
        private Identifier idOld;
		private Identifier idNew;

        public IdCopyPropagationRule(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(Identifier id)
        {
            idOld = id;
            idNew = ctx.GetValue(id) as Identifier;
            return idNew != null && idNew != idOld;
        }

        public Expression Transform()
        {
            ctx.RemoveIdentifierUse(idOld);
            ctx.UseExpression(idNew);
            return idNew;
        }
	}
}
