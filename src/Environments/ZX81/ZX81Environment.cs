#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

// https://cdn.hackaday.io/files/289631239152992/ZX81_dual_2018-02-09.htm

using Reko.Arch.Z80;
using Reko.Core;
using Reko.Core.Hll.C;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.ZX81
{
    /// <summary>
    /// Implements the very meager ZX81 environment.
    /// </summary>
    public class ZX81Environment : Platform
    {
        private ZX81Encoding encoding;

        public ZX81Environment(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "zx81")
        {
            encoding = new ZX81Encoding();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            // A wild guess, but it's better than nothing. 
            return new HashSet<RegisterStorage> { Registers.a };
        }

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            throw new NotImplementedException();
        }

        public override SystemService FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            throw new NotImplementedException();
        }

        public override string DefaultCallingConvention
        {
            get { throw new NotImplementedException(); }
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 16;
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override Encoding DefaultTextEncoding { get { return encoding; } }
    }
}
