#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Arch.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.RiscOS
{
    public class RiscOSPlatform : Platform
    {
        public RiscOSPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch)
        {
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override string PlatformIdentifier { get { return "riscOS"; } }

        public override BitSet CreateImplicitArgumentRegisters()
        {
            return Architecture.CreateRegisterBitset();
        }

        public override ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
        {
            throw new NotImplementedException();
        }

        public override SystemService FindService(int vector, ProcessorState state)
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
                    Signature = new ProcedureSignature(null,
                        new Identifier("r0", PrimitiveType.Pointer32, A32Registers.r0))
                };
            }
            throw new NotSupportedException(string.Format("Unknown RiscOS vector &{0:X}.", vector)); 
        }

        public override ProcedureBase GetTrampolineDestination(ImageReader imageReader, IRewriterHost host)
        {
            return null;
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }


    }
}
