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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Types;
using Reko.Core.Machine;

namespace Reko.Core
{
    /// <summary>
    /// Objects implementing this interface know how to bind a storage
    /// location to an identifier.
    /// </summary>
    public interface IStorageBinder
    {
        Identifier EnsureIdentifier(Storage stgForeign);
        Identifier EnsureRegister(RegisterStorage reg);
        Identifier EnsureFlagGroup(FlagGroupStorage grf);
        Identifier EnsureFlagGroup(RegisterStorage flagRegister, uint flagGroupBits, string name, DataType dataType);
        Identifier EnsureFpuStackVariable(int v, DataType dataType);
        Identifier EnsureOutArgument(Identifier idOrig, DataType outArgumentPointer);
        Identifier EnsureSequence(DataType dataType, params Storage[] elements);
        Identifier EnsureSequence(DataType dataType, string name, params Storage [] elements);
        Identifier EnsureStackVariable(int offset, DataType dataType);
        Identifier CreateTemporary(DataType dt);
        Identifier CreateTemporary(string name, DataType dt);
    }
}
