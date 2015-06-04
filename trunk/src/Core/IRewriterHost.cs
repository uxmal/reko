#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core
{
    public interface IRewriterHost 
    {
        PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity);
        Expression PseudoProcedure(string name, DataType returnType, params Expression [] args);

        ExternalProcedure GetImportedProcedure(Address addrThunk, Address addrInstr);
        ExternalProcedure GetInterceptedCall(Address addrImportThunk);

        void Error(Address address, string message);
    }
}

