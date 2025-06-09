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

using Reko.Arch.M68k;
using Reko.Arch.M68k.Machine;
using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Reko.Core.Loading;
using Reko.Core.Code;
using Reko.Core.Machine;
using Reko.Core.Memory;

namespace Reko.Environments.MacOS.Classic
{
    public class MacOSClassic : Platform
    {
        private MacOsRomanEncoding encoding;
        private Identifier ptrA5World;

        public MacOSClassic(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "macOs")
        {
            encoding = new MacOsRomanEncoding();
            ptrA5World = null!;
            A5World = null!;
            this.StructureMemberAlignment = 2;
            this.TrashedRegisters = CreateTrashedRegisters();
        }

        public override bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            return reg.Number == Registers.a7.Number || reg.Number == Registers.a5.Number;
        }

        public override CParser CreateCParser(TextReader rdr, ParserState? state)
        {
            //$TODO: consider making MacOSKeywords for MacOS-specific
            // extensions to the C.
            state ??= new ParserState();
            var lexer = new CLexer(rdr, CLexer.MsvcKeywords);
            var parser = new CParser(state, lexer);
            return parser;
        }

        private HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>
            {
                Registers.d0,
                Registers.a0,
            };
        }

        public override ICallingConvention GetCallingConvention(string? ccName)
        {
            switch (ccName)
            {
            case "pascal":
            case "__pascal":    
                return new PascalCallingConvention((M68kArchitecture) this.Architecture);
            default:
                return new M68kCallingConvention((M68kArchitecture) this.Architecture);
            }
        }

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            vector &= 0xFFFF;
            foreach (var module in metadata.Modules.Values)
            {
                if (module.ServicesByVector.TryGetValue(vector, out List<SystemService>? svcs))
                    return svcs.FirstOrDefault(s => s.SyscallInfo!.Matches(vector, state));
            }
            return null;
        }


        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override Encoding DefaultTextEncoding
        {
            get { return encoding; }
        }

        public ImageSegment A5World { get; set; }
        public uint A5Offset { get; set; }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.WChar_t: return 16;  //$REVIEW: Does MacOS support wchar_t?
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

        public override void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter m)
        {
            m.MStore(proc.Frame.FramePointer, proc.Frame.Continuation);
            if (this.A5World is not null)
            {
                var ptrA5World = EnsureA5Pointer();
                var a5 = proc.Frame.EnsureRegister(Registers.a5);
                //m.Assign(a5, this.A5World.Address);
                m.Assign(a5, m.Word32((uint) (this.A5World.Address.Offset + this.A5Offset)));
                //m.Assign(a5, ptrA5World);
            }
        }

        private Identifier EnsureA5Pointer()
        {
            if (this.ptrA5World is not null)
                return this.ptrA5World;

            var a5world_t = new StructureType("A5World_t", 0, true);
            var ptr = new Pointer(a5world_t, PointerType.BitSize);
            this.ptrA5World = Identifier.Global("a5world", ptr);
            return this.ptrA5World;
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If an indirect call uses the A5 register and the offset of the call lands in the 
        /// jump table in the A5 world, returns the address stored in the jump table
        /// to resolve the call.
        /// </summary>
        /// <param name="instr"></param>
        /// <returns>Null if the call wasn't to a valid A5 jumptable location.</returns>
        public override Address? ResolveIndirectCall(RtlCall instr)
        {
            if (instr.Target is not BinaryExpression bin)
                return null;
            if (bin.Operator.Type != OperatorType.IAdd)
                return null;
            if (!(bin.Left is Identifier idLeft) || idLeft.Storage != Registers.a5)
                return null;
            if (!(bin.Right is Constant cRight))
                return null;
            const uint SizeOfJmpOpcode = 2;
            uint offset = cRight.ToUInt32() + this.A5Offset + SizeOfJmpOpcode;
            if (!A5World.MemoryArea.TryReadBeUInt32(offset, out uint uAddr))
                return null;
            return Address.Ptr32(uAddr);
        }
    }
}
