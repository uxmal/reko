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

using Reko.Core.Expressions;

namespace Reko.Core.Output;

/// <summary>
/// Interface for classes that generate code to create mock identifiers.
/// </summary>
public interface IMockIdentifierWriter
{
    /// <summary>
    /// Writes code to generate the procedure continuation.
    /// </summary>
    void WriteContinuation();


    /// <summary>
    /// Writes code to generate the procedure frame pointer.
    /// </summary>
    void WriteFramePointer();

    /// <summary>
    /// Writes code to generate an identifier.
    /// </summary>
    /// <param name="id">Identifier to generate.</param>
    /// <param name="storageName">Name of storage variable to use.</param>
    /// <param name="mockGenerator">Used to render type references.</param>
    void WriteIdentifier(Identifier id, string storageName, MockGenerator mockGenerator);
}
