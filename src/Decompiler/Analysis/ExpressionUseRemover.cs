#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using System;

namespace Reko.Analysis
{
    public class ExpressionUseRemover : ExpressionVisitorBase
    {
        private readonly Statement user;
        private readonly SsaIdentifierCollection ssaIds;

        public ExpressionUseRemover(Statement user, SsaIdentifierCollection ssaIds)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
            this.ssaIds = ssaIds;
        }

        public override void VisitIdentifier(Identifier id)
        {
            ssaIds[id].Uses.Remove(user);
        }
    }
}
