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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Abstract base class for calling convention implementations.
    /// </summary>
    public abstract class AbstractCallingConvention : ICallingConvention
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCallingConvention"/> class.
        /// </summary>
        /// <param name="name">Name of the calling convention</param>
        protected AbstractCallingConvention(string name)
        {
            this.Name = name;
            this.InArgumentComparer = null;
            this.OutArgumentComparer = null;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public virtual IComparer<Identifier>? InArgumentComparer { get; protected set; }

        /// <inheritdoc/>
        public virtual IComparer<Identifier>? OutArgumentComparer { get; protected set; }

        /// <inheritdoc/>
        public abstract void Generate(ICallingConventionBuilder ccr, int retAddressOnStack, DataType? dtRet, DataType? dtThis, List<DataType> dtParams);
        /// <inheritdoc/>
        public abstract bool IsArgument(Storage stg);
        /// <inheritdoc/>
        public abstract bool IsOutArgument(Storage stg);

        /// <summary>
        /// Returns a string representation of the calling convention.
        /// </summary>
        public override string ToString()
        {
            return Name.Length != 0 ? Name : "(default calling convention)";
        }
    }
}
