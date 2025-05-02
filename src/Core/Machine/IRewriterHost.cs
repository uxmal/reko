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
using System.Diagnostics.CodeAnalysis;

namespace Reko.Core
{
    /// <summary>
    /// Provides services from the hosting environment to the rewriter.
    /// </summary>
    public interface IRewriterHost
    {
        /// <summary>
        /// If the binary has a notion of a global register, this value is non-null.
        /// </summary>
        Constant? GlobalRegisterValue { get; }

        /// <summary>
        /// Obtain a reference to the processor architecture with the name <paramref name="archMoniker"/>.
        /// </summary>
        /// <param name="archMoniker">The name of the desired architecture.</param>
        /// <returns></returns>
        IProcessorArchitecture GetArchitecture(string archMoniker);

        /// <summary>
        /// Given an address <paramref name="addrThunk"/>, returns the possible imported thing (procedure or 
        /// global variable) pointed to by Thunk.
        /// </summary>
        /// <param name="addrThunk"></param>
        /// <param name="addrInstr"></param>
        /// <returns></returns>
        Expression? GetImport(Address addrThunk, Address addrInstr);

        /// <summary>
        /// Given an address <paramref name="addrThunk"/>, returns the possible imported procedure
        /// 
        /// </summary>
        /// <param name="arch">Current <see cref="IProcessorArchitecture"/>.</param>
        /// <param name="addrThunk">Address of a thunk.</param>
        /// <param name="addrInstr">Address of calling instruction to thunk.
        /// </param>
        /// <returns>If it can be determined from the thunk, the imported procedure;
        /// otherwise null.
        /// </returns>
        ExternalProcedure? GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstr);

        /// <summary>
        /// Detects if a trampoline (<c>call [foo]</c> where <c>foo: jmp bar</c>)
        /// is jumping into the body of a procedure that was loaded with <c>GetProcAddress </c>or 
        /// the like.
        /// </summary>
        /// <param name="arch">Current <see cref="IProcessorArchitecture"/>.</param>
        /// <param name="addrImportThunk"></param>
        /// <returns></returns>

        ExternalProcedure? GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk);

        /// <summary>
        /// Read a value of size <paramref name="dt"/> from address <paramref name="addr"/>, 
        /// using the endianness of the <paramref name="arch"/> processor architecture.
        /// </summary>
        public bool TryRead(IProcessorArchitecture arch, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value);

        /// <summary>
        /// Report an error to the user. The message is formatted using <see cref="string.Format(string, object[])"/>.
        /// </summary>
        /// <param name="address">Address at which the error occurred.</param>
        /// <param name="format">Format string.</param>
        /// <param name="args">Arguments to be formatted.</param>
        void Error(Address address, string format, params object[] args);

        /// <summary>
        /// Report a warning to the user. The message is formatted using <see cref="string.Format(string, object[])"/>.
        /// </summary>
        /// <param name="address">Address at which the error occurred.</param>
        /// <param name="format">Format string.</param>
        /// <param name="args">Arguments to be formatted.</param>
        void Warn(Address address, string format, params object[] args);
    }

    /// <summary>
    /// Dummy implementation of <see cref="IRewriterHost"/> that does nothing.
    /// </summary>
    public class NullRewriterHost : IRewriterHost
    {
        /// <inheritdoc/>
        public Constant? GlobalRegisterValue => null;

        /// <inheritdoc/>
        public void Error(Address address, string format, params object[] args)
        {
        }

        /// <inheritdoc/>
        public IProcessorArchitecture GetArchitecture(string archMoniker)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        Expression? IRewriterHost.GetImport(Address addrThunk, Address addrInstr)
        {
            return null;
        }

        /// <inheritdoc/>
        public ExternalProcedure? GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstr)
        {
            return null;
        }

        /// <inheritdoc/>
        public ExternalProcedure? GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
        {
            return null;
        }

        /// <inheritdoc/>
        public bool TryRead(IProcessorArchitecture arch, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value)
        {
            value = null;
            return false;
        }

        /// <inheritdoc/>
        public void Warn(Address address, string format, params object[] args)
        {
        }
    }
}

