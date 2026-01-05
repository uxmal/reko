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
    /// An exception that occurred during the processing of a statement.
    /// </summary>
    public class StatementCorrelatedException : Exception
    {
        /// <summary>
        /// Constructs a statement correlated exception.
        /// </summary>
        /// <param name="stm">The <see cref="Core.Statement"/> at which 
        /// the exception occurred.</param>
        public StatementCorrelatedException(Statement stm)
        {
            this.Statement = stm;
        }

        /// <summary>
        /// Constructs a statement correlated exception.
        /// </summary>
        /// <param name="stm">The <see cref="Core.Statement"/> at which 
        /// the exception occurred.</param>
        /// <param name="message">Error message.</param>
        public StatementCorrelatedException(Statement stm, string message) :
            base(message)
        {
            this.Statement = stm;
        }

        /// <summary>
        /// Constructs a statement correlated exception.
        /// </summary>
        /// <param name="stm">The <see cref="Core.Statement"/> at which 
        /// the exception occurred.</param>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public StatementCorrelatedException(Statement stm, string message, Exception innerException) :
            base(message, innerException)
        {
            this.Statement = stm;
        }

        /// <summary>
        /// The <see cref="Core.Statement"/> at which the exception occurred.
        /// </summary>
        public Statement Statement { get; }
    }
}
