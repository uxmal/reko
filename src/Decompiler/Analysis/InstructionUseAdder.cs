#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

namespace Reko.Analysis
{
    public class InstructionUseAdder : InstructionVisitorBase
    {
        private Statement user;
        private SsaIdentifierCollection ssaIds;

        public InstructionUseAdder(Statement user, SsaIdentifierCollection ssaIds)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            this.user = user; this.ssaIds = ssaIds;
        }

        #region InstructionVisitor Members

        public override void VisitAssignment(Assignment a)
        {
            a.Src.Accept(this);
        }

        public override void VisitDeclaration(Declaration decl)
        {
            if (decl.Expression != null)
                decl.Expression.Accept(this);
        }

        public override void VisitDefInstruction(DefInstruction def)
        {
        }

        public override void VisitPhiAssignment(PhiAssignment phi)

        {
            phi.Src.Accept(this);
        }

        #endregion

        #region IExpressionVisitor Members

        public override void VisitIdentifier(Identifier id)
        {
            ssaIds[id].Uses.Add(user);
        }

        #endregion
    }
}
