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

namespace Reko.Core
{
    /// <summary>
    /// Objects implementing this interface know how to bind a storage
    /// location to an <see cref="Identifier"/>.
    /// </summary>
    public interface IStorageBinder
    {
        /// <summary>
        /// Ensures that an identifier is bound for the given <see cref="Storage"/>,
        /// creating a new identifier if necessary.
        /// </summary>
        /// <param name="stgForeign">The <see cref="Storage"/> to be ensured.</param>
        /// <returns>The existing <see cref="Identifier"/> for the given <see cref="Storage"/>,
        /// or a newly created identifier if the storage wasn't present before.
        /// </returns>
        Identifier EnsureIdentifier(Storage stgForeign);

        /// <summary>
        /// Ensures that an identifier is bound for the given <see cref="RegisterStorage"/>.
        /// </summary>
        /// <param name="reg">Register to bind.</param>
        /// <returns>The existing <see cref="Identifier"/> for the given <see cref="Storage"/>,
        /// or a newly created identifier if the storage wasn't present before.
        /// </returns>
        Identifier EnsureRegister(RegisterStorage reg);

        /// <summary>
        /// Ensures that an identifier is bound for the given <see cref="FlagGroupStorage"/>.
        /// </summary>
        /// <param name="grf">Flag group to bind.</param>
        /// <returns>The existing <see cref="Identifier"/> for the given <see cref="Storage"/>,
        /// or a newly created identifier if the storage wasn't present before.
        /// </returns>
        Identifier EnsureFlagGroup(FlagGroupStorage grf);


        /// <summary>
        /// Ensures that an identifier is bound for the given <see cref="FlagGroupStorage"/>.
        /// </summary>
        /// <param name="flagRegister">The flag register the flag group is located in.</param>
        /// <param name="flagGroupBits">Flag group to bind.</param>
        /// <param name="name">Name to give the flag group.</param>
        /// <returns>The existing <see cref="Identifier"/> for the given <see cref="Storage"/>,
        /// or a newly created identifier if the storage wasn't present before.
        /// </returns>
        Identifier EnsureFlagGroup(RegisterStorage flagRegister, uint flagGroupBits, string name);

        /// <summary>
        /// Ensures that an identifier is bound for an FPU stack entry at the given <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset from the FPU stack top (upon entry to the procedure).</param>
        /// <param name="dataType">Data type of the identifier.</param>
        /// <returns>An existing <see cref="Identifier"/>,
        /// or a newly created identifier if the storage wasn't present before.
        /// </returns>
        Identifier EnsureFpuStackVariable(int offset, DataType dataType);
        Identifier EnsureOutArgument(Identifier idOrig, DataType dtOutArgument);

        /// <summary>
        /// Ensures that an identifier is bound for the <see cref="SequenceStorage"/>.
        /// </summary>
        /// <param name="sequence">A <see cref="SequenceStorage"/> modeling an orderd register tuple.</param>
        /// <returns>An existing <see cref="Identifier"/>,
        /// or a newly created identifier if the sequence storage wasn't present before.
        /// </returns>
        Identifier EnsureSequence(SequenceStorage sequence);
        Identifier EnsureSequence(DataType dataType, params Storage[] elements);
        Identifier EnsureSequence(DataType dataType, string name, params Storage [] elements);
        Identifier EnsureStackVariable(int offset, DataType dataType, string? name);
        Identifier CreateTemporary(DataType dt);
        Identifier CreateTemporary(string name, DataType dt);
    }
}
