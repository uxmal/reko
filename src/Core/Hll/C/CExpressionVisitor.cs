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

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// A visitor interface to C syntax elements.
    /// </summary>
    /// <typeparam name="T">Type returned by the visitor.</typeparam>
    public interface CExpressionVisitor<T>
    {
        /// <summary>
        /// Called when visiting a <see cref="ConstExp"/>.
        /// </summary>
        /// <param name="constant">Visited constant.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitConstant(ConstExp constant);

        /// <summary>
        /// Called when visiting a <see cref="CIdentifier"/>.
        /// </summary>
        /// <param name="id">Visited identifier.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitIdentifier(CIdentifier id);

        /// <summary>
        /// Called when visiting a <see cref="Application"/>.
        /// </summary>
        /// <param name="application">Visited application.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitApplication(Application application);

        /// <summary>
        /// Called when visiting a <see cref="CArrayAccess"/>.
        /// </summary>
        /// <param name="aref">Visited array access.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitArrayAccess(CArrayAccess aref);

        /// <summary>
        /// Called when visiting a <see cref="MemberExpression"/>.
        /// </summary>
        /// <param name="member">Visited member expression.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitMember(MemberExpression member);

        /// <summary>
        /// Called when visiting a <see cref="CUnaryExpression"/>.
        /// </summary>
        /// <param name="unary">Visited unary expression.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitUnary(CUnaryExpression unary);

        /// <summary>
        /// Called when visiting a <see cref="CBinaryExpression"/>.
        /// </summary>
        /// <param name="binary">Visited binary expression.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitBinary(CBinaryExpression binary);

        /// <summary>
        /// Called when visiting a <see cref="AssignExpression"/>.
        /// </summary>
        /// <param name="assign">Visited assign expression.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitAssign(AssignExpression assign);

        /// <summary>
        /// Called when visiting a <see cref="CastExpression"/>.
        /// </summary>
        /// <param name="cast">Visited cast expression.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitCast(CastExpression cast);

        /// <summary>
        /// Called when visiting a <see cref="ConditionalExpression"/>.
        /// </summary>
        /// <param name="conditional">Visited conditional expression.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitConditional(ConditionalExpression conditional);

        /// <summary>
        /// Called when visiting a <see cref="IncrementExpression"/>.
        /// </summary>
        /// <param name="increment">Visited unary expression.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitIncrement(IncrementExpression increment);

        /// <summary>
        /// Called when visiting a <see cref="SizeofExpression"/>.
        /// </summary>
        /// <param name="sizeOf">Visited unary expression.</param>
        /// <returns>The value returned by the visitor.</returns>
        T VisitSizeof(SizeofExpression sizeOf);
    }
}
