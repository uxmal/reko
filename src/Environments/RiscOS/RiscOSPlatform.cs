#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.CLanguage;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Environments.RiscOS
{
    public class RiscOSPlatform : Platform
    {
        public RiscOSPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "riscOS")
        {
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return new HashSet<RegisterStorage>();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>();
        }

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            throw new NotImplementedException();
        }

        public override SystemService FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            switch (vector)
            {
            // http://www.riscos.com/support/developers/prm/errors.html#89849
            case 0x2B:
                return new SystemService
                {
                    Name = "OS_GenerateError",
                    Characteristics = new ProcedureCharacteristics {
                        Terminates = true,
                    },
                    Signature = FunctionType.Action(
                        new Identifier[] {
                            new Identifier("r0", PrimitiveType.Ptr32, Architecture.GetRegister("r0")!)
                        })
                };
            }
            throw new NotSupportedException(string.Format("Unknown RiscOS vector &{0:X}.", vector)); 
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.WChar_t: return 16;  //$REVIEW: Does RiscOS support wchar_t?
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 32;
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


    }
}
