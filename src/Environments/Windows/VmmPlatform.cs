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
using Reko.Core.Types;
using Reko.Core.CLanguage;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Serialization;
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
        }

        public override string DefaultCallingConvention
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new[] { X86Registers.eax, X86Registers.edx, X86Registers.ecx }
                .ToHashSet();
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            if (vector == 0x20 && state != null && segmentMap != null)
            {
                // Dynamic VxD call.
                //$TODO: look up the call to determine what parameters it uses.
                var addr = state.InstructionPointer + 2;
                var dwService = state.GetMemoryValue(addr + 2, PrimitiveType.Word32, segmentMap);
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

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            throw new NotImplementedException();
        }

        public override ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            return base.GetTrampolineDestination(addrInstr, instrs, host);
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
