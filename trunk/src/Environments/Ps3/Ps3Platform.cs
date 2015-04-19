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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompiler.Environments.Ps3
{
    public class Ps3Platform : Platform
    {
        public Ps3Platform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch) 
        {
        }

        public override string DefaultCallingConvention { get { return ""; } }

        public override PrimitiveType PointerType { get { return PrimitiveType.Pointer32; } }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            // Bizarrely, pointers are 32-bit on this 64-bit platform.
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address MakeAddressFromLinear(ulong uAddr)
        {
            return Address.Ptr32((uint)uAddr);
        }
    }
}
