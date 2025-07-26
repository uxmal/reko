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

namespace Reko.Evaluation;

public partial class ExpressionSimplifier
{
    /// <inheritdoc/>
    public virtual (Expression, bool) VisitIdentifier(Identifier id)
    {
        var e = idConst.Match(id, this.ctx, unifier, listener);
        if (e is not null)
        {
            return (e, true);
        }
        if (ctx.GetValue(id) is ProcedureConstant pc)
        {
            return (pc, true);
        }
        if (e is not null)
        {
            return (e, true);
        }
        // jkl: Copy propagation causes real problems when used during trashed register analysis.
        // If needed in other passes, it should be an option for expression e
        e = IdCopyPropagation(id, ctx);
        if (e is not null)
        {
            return (e, true);
        }
        e = idBinIdc.Match(id, ctx);
        if (e is not null)
        {
            return (e, true);
        }
        return (id, false);
    }

    private static Expression? IdCopyPropagation(Identifier id, EvaluationContext ctx)
    {
        var idOld = id;
        if (ctx.GetValue(id) is not Identifier idNew || idNew == idOld)
            return null;

        return idNew;
    }

}
