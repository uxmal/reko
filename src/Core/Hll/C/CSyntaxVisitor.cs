#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Text;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Visitor interface for C syntax trees.
    /// </summary>
    /// <typeparam name="T">Type returned by visitor.</typeparam>
    public interface CSyntaxVisitor<T>
    {
        /// <summary>
        /// Called when a type is visited.
        /// </summary>
        /// <param name="type">The visited type.</param>
        /// <returns>Returned value.</returns>
        T VisitType(CType type);

        /// <summary>
        /// Called when a declaration is visited.
        /// </summary>
        /// <param name="decl">The visited declaration.</param>
        /// <returns>Returned value.</returns>
        T VisitDeclaration(Decl decl);

        /// <summary>
        /// Called when a decl-specification is visited.
        /// </summary>
        /// <param name="declSpec">The visited decl-specification.</param>
        /// <returns>Returned value.</returns>
        T VisitDeclSpec(DeclSpec declSpec);

        /// <summary>
        /// Called when a init-declarator is visited.
        /// </summary>
        /// <param name="initDeclarator">The visited init-declarator.</param>
        /// <returns>Returned value.</returns>
        T VisitInitDeclarator(InitDeclarator initDeclarator);

        /// <summary>
        /// Called when a enumerator is visited.
        /// </summary>
        /// <param name="enumerator">The visited enumerator.</param>
        /// <returns>Returned value.</returns>
        T VisitEnumerator(Enumerator enumerator);

        /// <summary>
        /// Called when a statement is visited.
        /// </summary>
        /// <param name="stm">The visited statement.</param>
        /// <returns>Returned value.</returns>
        T VisitStatement(Stat stm);

        /// <summary>
        /// Called when an expression is visited.
        /// </summary>
        /// <param name="stm">The visited expression.</param>
        /// <returns>Returned value.</returns>
        T VisitExpression(CExpression stm);

        /// <summary>
        /// Called when a parameter declaration is visited.
        /// </summary>
        /// <param name="paramDecl">The visited parameter declaration.</param>
        /// <returns>Returned value.</returns>
        T VisitParamDeclaration(ParamDecl paramDecl);

        /// <summary>
        /// Called when an attribute is visited.
        /// </summary>
        /// <param name="cAttribute">The visited attribute.</param>
        /// <returns></returns>
        T VisitAttribute(CAttribute cAttribute);

    }
}
