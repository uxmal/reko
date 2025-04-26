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
using System;

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
        /// Given the components of a <see cref="FlagGroupStorage" />, ensures
        /// there is an identifier backed by that flag group storage.
        /// </summary>
        /// <param name="flagRegister">Backing flag register.</param>
        /// <param name="flagGroupBits">Flag group bits.</param>
        /// <param name="name">Name of the flag group storage.</param>
        /// <returns>An <see cref="Identifier"/> backed by that flag group storage.
        /// </returns>
        [Obsolete("Use EnsureFlagGroup(FlagGroupStorage) instead.")]
        Identifier EnsureFlagGroup(RegisterStorage flagRegister, uint flagGroupBits, string name);

        /// <summary>
        /// Given a FPU stack variable at FPU stack offset <paramref name="offset"/>,
        /// identifier backed by that FPU stack offset.
        /// </summary>
        /// <param name="offset">FPU stack offset.</param>
        /// <param name="dataType">Data type of the identifier.</param>
        /// <returns>An <see cref="Identifier"/> backed by that FPU stack offset.
        /// </returns>
        Identifier EnsureFpuStackVariable(int offset, DataType dataType);

        /// <summary>
        /// Given an identifier, ensures that is is an output register
        /// with the given data type.
        /// </summary>
        /// <param name="idOrig">Original identifier.</param>
        /// <param name="dtOutArgument">Data type of the argument.</param>
        /// <returns>An <see cref="Identifier"/> backed by that out argument.
        /// </returns>
        Identifier EnsureOutArgument(Identifier idOrig, DataType dtOutArgument);

        /// <summary>
        /// Given a <see cref="SequenceStorage"/>, ensures that there is an
        /// identifier backed by that sequence.
        /// </summary>
        /// <param name="sequence">Sequence storage.</param>
        /// <returns>An identifier backed by the sequence storage.</returns>
        Identifier EnsureSequence(SequenceStorage sequence);


        /// <summary>
        /// Given an sequence of <see cref="Storage"/>s, ensures there is an
        /// identifier backed by that sequence.
        /// </summary>
        /// <param name="dataType">Data type of the identifier.</param>
        /// <param name="elements">Sequence storage elements.</param>
        /// <returns>An identifier backed by the sequence storage.</returns>
        Identifier EnsureSequence(DataType dataType, params Storage[] elements);

        /// <summary>
        /// Given an sequence of <see cref="Storage"/>s, ensures there is an
        /// identifier backed by that sequence.
        /// </summary>
        /// <param name="dataType">Data type of the identifier.</param>
        /// <param name="elements">Sequence storage elements.</param>
        /// <returns>An identifier backed by the sequence storage.</returns>
        [Obsolete("", true)]
        Identifier EnsureSequence(DataType dataType, string name, params Storage [] elements);

        /// <summary>
        /// Given a stack offset, ensures that there is an identifier backed by
        /// that stack offset.
        /// </summary>
        /// <param name="offset">Stack offset.</param>
        /// <param name="dataType">Data type of the variable.</param>
        /// <param name="name">Optional name to give the identifier.</param>
        /// <returns></returns>
        Identifier EnsureStackVariable(int offset, DataType dataType, string? name);

        /// <summary>
        /// Creates a new temporary identifier with the given data type.
        /// </summary>
        /// <param name="dt">Data type to use for the identifier.</param>
        /// <returns>The new temporary identifier.</returns>
        Identifier CreateTemporary(DataType dt);


        /// <summary>
        /// Creates a new temporary identifier with the given name
        /// and data type.
        /// </summary>
        /// <param name="name">Name to use for the identifier.</param>
        /// <param name="dt">Data type to use for the identifier.</param>
        /// <returns>The new temporary identifier.</returns>
        Identifier CreateTemporary(string name, DataType dt);
    }
}
