#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;

namespace Reko.Core
{
    public interface IRewriterHost 
    {
        PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity);
        /// <summary>
        /// Generates a call to an intrinsic procedure named <paramref name="name"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="returnType"></param>
        /// <param name="args"></param>
        /// <returns>An Application expression.</returns>
        Expression PseudoProcedure(string name, DataType returnType, params Expression [] args);
        Expression CallIntrinsic(string name, FunctionType fnType, params Expression [] args);
        Expression PseudoProcedure(string name, ProcedureCharacteristics c, DataType returnType, params Expression [] args);


        /// <summary>
        /// Obtain a reference to the processor architecture with the name <paramref name="archMoniker"/>.
        /// </summary>
        /// <param name="archMoniker">The name of the desired architecture.</param>
        /// <returns></returns>
        IProcessorArchitecture GetArchitecture(string archMoniker);

        /// <summary>
        /// Given an address addrThunk, returns the possible imported thing (procedure or 
        /// global variable) pointed to by Thunk.
        /// </summary>
        /// <param name="addrThunk"></param>
        /// <param name="addrInstr"></param>
        /// <returns></returns>
        Expression GetImport(Address addrThunk, Address addrInstr);
        ExternalProcedure GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstr);
        ExternalProcedure GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk);

        void Error(Address address, string format, params object[] args);
        void Warn(Address address, string format, params object[] args);
    }
}

