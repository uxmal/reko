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

using Reko.Core;
using Reko.Core.Types;

namespace Reko.Scanning
{
    public interface IScannerQueue
    {
        void EnqueueImageSymbol(ImageSymbol sym, bool isEntryPoint);
        void EnqueueProcedure(IProcessorArchitecture arch, Address addr);
        void EnqueueUserProcedure(IProcessorArchitecture arch, Address addr, FunctionType sig, string name);
        void EnqueueUserGlobalData(Address addr, DataType dt, string name);

        //$REVIEW: these methods don't belong here. 
        // Callers should use DecompilerEventListener or IRewriterHost.
        void Warn(Address addr, string message);
        void Warn(Address addr, string message, params object[] args);
        void Error(Address addr, string message, params object[] args);
    }
}