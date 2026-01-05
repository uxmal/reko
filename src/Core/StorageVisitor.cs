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

namespace Reko.Core
{
    /// <summary>
    /// Visitor pattern for storages, parametrized by the returned value of
    /// all the visitor methods.
    /// </summary>
    /// <typeparam name="T">Returned value</typeparam>
	public interface StorageVisitor<T>
	{
        /// <summary>
        /// Called when a <see cref="FlagGroupStorage"/> is visited.
        /// </summary>
        /// <param name="grf">Flag group being visited.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
		T VisitFlagGroupStorage(FlagGroupStorage grf);

        /// <summary>
        /// Called when a <see cref="FpuStackStorage"/> is visited.
        /// </summary>
        /// <param name="fpu">FPU stack entry being visited.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
		T VisitFpuStackStorage(FpuStackStorage fpu);

        /// <summary>
        /// Called when a <see cref="MemoryStorage"/> is visited.
        /// </summary>
        /// <param name="mem">Memory storage being visited.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
		T VisitMemoryStorage(MemoryStorage mem);

        /// <summary>
        /// Called when a <see cref="RegisterStorage"/> is visited.
        /// </summary>
        /// <param name="reg">Register being visited.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
        T VisitRegisterStorage(RegisterStorage reg);

        /// <summary>
        /// Called when a <see cref="SequenceStorage"/> is visited.
        /// </summary>
        /// <param name="seq">Sequence storage being visited.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
        T VisitSequenceStorage(SequenceStorage seq);

        /// <summary>
        /// Called when a <see cref="StackStorage"/> is visited.
        /// </summary>
        /// <param name="stack">Stack storage being visited.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
		T VisitStackStorage(StackStorage stack);

        /// <summary>
        /// Called when a <see cref="TemporaryStorage"/> is visited.
        /// </summary>
        /// <param name="temp">Stack storage being visited.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
		T VisitTemporaryStorage(TemporaryStorage temp);
    }

    /// <summary>
    /// Visitor interface for storages, parametrized by the returned value of
    /// all the visitor methods and a context.
    /// </summary>
    /// <typeparam name="T">Returned value</typeparam>
    /// <typeparam name="C">Context object type.</typeparam>

    public interface StorageVisitor<T, C>
    {
        /// <summary>
        /// Called when a <see cref="FlagGroupStorage"/> is visited.
        /// </summary>
        /// <param name="grf">Flag group being visited.</param>
        /// <param name="context">Context object.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
		T VisitFlagGroupStorage(FlagGroupStorage grf, C context);

        /// <summary>
        /// Called when a <see cref="FpuStackStorage"/> is visited.
        /// </summary>
        /// <param name="fpu">FPU stack entry being visited.</param>
        /// <param name="context">Context object.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
		T VisitFpuStackStorage(FpuStackStorage fpu, C context);

        /// <summary>
        /// Called when a <see cref="MemoryStorage"/> is visited.
        /// </summary>
        /// <param name="mem">Memory storage being visited.</param>
        /// <param name="context">Context object.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
		T VisitMemoryStorage(MemoryStorage mem, C context);

        /// <summary>
        /// Called when a <see cref="RegisterStorage"/> is visited.
        /// </summary>
        /// <param name="reg">Register being visited.</param>
        /// <param name="context">Context object.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
        T VisitRegisterStorage(RegisterStorage reg, C context);

        /// <summary>
        /// Called when a <see cref="SequenceStorage"/> is visited.
        /// </summary>
        /// <param name="seq">Sequence storage being visited.</param>
        /// <param name="context">Context object.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
        T VisitSequenceStorage(SequenceStorage seq, C context);

        /// <summary>
        /// Called when a <see cref="StackStorage"/> is visited.
        /// </summary>
        /// <param name="stack">Stack storage being visited.</param>
        /// <param name="context">Context object.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
		T VisitStackStorage(StackStorage stack, C context);

        /// <summary>
        /// Called when a <see cref="TemporaryStorage"/> is visited.
        /// </summary>
        /// <param name="temp">Stack storage being visited.</param>
        /// <param name="context">Context object.</param>
        /// <returns>A value of type <typeparamref name="T"/></returns>
		T VisitTemporaryStorage(TemporaryStorage temp, C context);
    }
}
