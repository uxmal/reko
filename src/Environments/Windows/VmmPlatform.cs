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

using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X86Registers = Reko.Arch.X86.Registers;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Models the Windows 3.x -> Windows 95 -> Windows ME VMM environment
    /// </summary>
    public class VmmPlatform : Platform
    {
        public VmmPlatform(IServiceProvider services, IProcessorArchitecture arch) : 
            base(services, arch, "win-vmm")
        {
            this.StructureMemberAlignment = 4;
            this.TrashedRegisters = new[] { X86Registers.eax, X86Registers.edx, X86Registers.ecx }
                .ToHashSet();
        }

        public override string DefaultCallingConvention => "";

        public override CParser CreateCParser(TextReader rdr, ParserState? state)
        {
            state ??= new ParserState();
            var lexer = new CLexer(rdr, CLexer.MsvcKeywords);
            var parser = new CParser(state, lexer);
            return parser;
        }

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            if (vector == 0x20 && state is not null && memory is not null)
            {
                // Dynamic VxD call.
                //$TODO: look up the call to determine what parameters it uses.
                var addr = state.InstructionPointer + 2;
                var dwService = state.GetMemoryValue(addr + 2, PrimitiveType.Word32, memory);
                return new SystemService
                {
                    Name = $"SVC${dwService:X8}",
                    Signature = FunctionType.Action(),
                    Characteristics = new ProcedureCharacteristics
                    {
                        ReturnAddressAdjustment = 6,
                    }
                };
            }
            return null;
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
