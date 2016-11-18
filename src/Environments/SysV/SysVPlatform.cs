﻿#region License
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

using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Environments.SysV
{
    //$TODO: rename to Elf-Neutral? Or Posix?
    public class SysVPlatform : Platform
    {
        private RegisterStorage[] trashedRegs;

        public SysVPlatform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "elf-neutral")
        {
            LoadTrashedRegisters();
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
        {
            switch (Architecture.Name)
            {
            case "mips-be-32":
            case "mips-le-32":
            case "mips-be-64":
            case "mips-le-64":
                return new MipsProcedureSerializer(Architecture, typeLoader, defaultConvention);
            case "ppc32":
                return new PowerPcProcedureSerializer(Architecture, typeLoader, defaultConvention);
            case "sparc32":
                return new SparcProcedureSerializer(Architecture, typeLoader, defaultConvention);
            case "x86-protected-32":
                return new X86ProcedureSerializer(Architecture, typeLoader, defaultConvention);
            case "x86-protected-64":
                return new X86_64ProcedureSerializer(Architecture, typeLoader, defaultConvention);
            case "xtensa":
                return new XtensaProcedureSerializer(Architecture, typeLoader, defaultConvention);
            case "arm":
                return new Arm32ProcedureSerializer(Architecture, typeLoader, defaultConvention);
            case "m68k":
                return new M68kProcedureSerializer(Architecture, typeLoader, defaultConvention);
            default:
                throw new NotImplementedException(string.Format("Procedure serializer for {0} not implemented yet.", Architecture.Description));
            }
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return new HashSet<RegisterStorage>();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return this.trashedRegs.ToHashSet();
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            return null;
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Char: return 1;
            case CBasicType.WChar_t: return 2;
            case CBasicType.Short: return 2;
            case CBasicType.Int: return 4;
            case CBasicType.Long: return 4;
            case CBasicType.LongLong: return 8;
            case CBasicType.Float: return 4;
            case CBasicType.Double: return 8;
            case CBasicType.LongDouble: return 8;
            case CBasicType.Int64: return 8;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ProcedureBase GetTrampolineDestination(ImageReader rdr, IRewriterHost host)
        {
            var rw = Architecture.CreateRewriter(
                rdr,
                Architecture.CreateProcessorState(),
                Architecture.CreateFrame(), host);
            var rtlc = rw.FirstOrDefault();
            if (rtlc == null || rtlc.Instructions.Count == 0)
                return null;

            // Match x86 pattern.
            // jmp [destination]
            Address addrTarget = null;
            var jump = rtlc.Instructions[0] as RtlGoto;
            if (jump != null)
            {
                var pc = jump.Target as ProcedureConstant;
                if (pc != null)
                    return pc.Procedure;
                var access = jump.Target as MemoryAccess;
                if (access == null)
                    return null;
                addrTarget = access.EffectiveAddress as Address;
                if (addrTarget == null)
                {
                    var wAddr = access.EffectiveAddress as Constant;
                    if (wAddr == null)
                    {
                        return null;
                    }
                    addrTarget = MakeAddressFromConstant(wAddr);
                }
            }
            if (addrTarget == null)
                return null;
            ProcedureBase proc = host.GetImportedProcedure(addrTarget, rtlc.Address);
            if (proc != null)
                return proc;
            return host.GetInterceptedCall(addrTarget);
        }

        public override void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter m)
        {
            switch (Architecture.Name)
            {
            case "mips-be-32":
                // MIPS ELF ABI: r25 is _always_ set to the address of a procedure on entry.
                m.Assign(proc.Frame.EnsureRegister(Architecture.GetRegister(25)), Constant.Word32((uint)addr.ToLinear()));
                break;
            }
        }

        private void LoadTrashedRegisters()
        {
            if (Services != null)
            {
                var cfgSvc = Services.RequireService<IConfigurationService>();
                var pa = cfgSvc.GetEnvironment(this.PlatformIdentifier).Architectures.SingleOrDefault(a => a.Name == Architecture.Name);
                if (pa != null)
                {
                    this.trashedRegs = pa.TrashedRegisters
                        .Select(r => Architecture.GetRegister(r))
                        .Where(r => r != null)
                        .ToArray();
                    return;
                }
            }
            this.trashedRegs = new RegisterStorage[0];
        } 

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            //$REVIEW: looks a lot like Win32library, perhaps push to parent class?
            EnsureTypeLibraries(PlatformIdentifier);
            var sig = Metadata.Lookup(procName);
            if (sig == null)
                return null;
            var proc = new ExternalProcedure(procName, sig);
            var characteristics = CharacteristicsLibs.Select(cl => cl.Lookup(procName))
                .Where(c => c != null)
                .FirstOrDefault();
            if (characteristics != null)
                proc.Characteristics = characteristics;
            return proc;
        }

        public override ExternalProcedure SignatureFromName(string fnName)
        {
            StructField_v1 field = null;
            try
            {
                var gcc = new GccMangledNameParser(fnName, this.PointerType.Size);
                field = gcc.Parse();
            }
            catch (Exception ex)
            {
                Debug.Print("*** Error parsing {0}. {1}", fnName, ex.Message);
                return null;
            }
            if (field == null)
                return null;
            var sproc = field.Type as SerializedSignature;
            if (sproc != null)
            {
                var loader = new TypeLibraryDeserializer(this, false, Metadata);
                var sser = this.CreateProcedureSerializer(loader, sproc.Convention);
                var sig = sser.Deserialize(sproc, this.Architecture.CreateFrame());    //$BUGBUG: catch dupes?
                return new ExternalProcedure(field.Name, sig)
                {
                    EnclosingType = sproc.EnclosingType
                };
            }
            return null;
        }
    }
}
