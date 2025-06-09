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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Parse result can either have a value or an error.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParseResult<T>
    {
        private T result;
        private string? error;

        /// <summary>
        /// Construct a successful parse result.
        /// </summary>
        /// <param name="result">Parse result.</param>
        public ParseResult(T result)
        {
            this.result = result;
        }

        /// <summary>
        /// Construct a failed parse result.
        /// </summary>
        /// <param name="error">Error describing the failure.</param>
        public ParseResult(string error)
        {
            this.error = error ?? throw new ArgumentNullException(nameof(error));
            this.result = default!;
        }

        /// <summary>
        /// The parsed value, if successful.
        /// </summary>
        public T Result
        {
            get
            {
                if (error is not null)
                    throw new InvalidOperationException("Mustn't access the result if an error was encountered.");
                return result!;
            }
        }
    }

    /// <summary>
    /// Helper class for <see cref="ParseResult{T}"/>.
    /// </summary>
    public static class ParseResult
    {
        /// <summary>
        /// Creates a successful parse result.
        /// </summary>
        /// <typeparam name="T">Type of the parse result</typeparam>.
        /// <param name="item">Parse result.</param>
        /// <returns>An instance off <see cref="ParseResult{T}"/>.</returns>
        public static ParseResult<T> Ok<T>(T item){
            return new ParseResult<T>(item);
        }
    }
}
