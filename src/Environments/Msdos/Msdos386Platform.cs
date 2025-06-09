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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Serialization;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Reko.Environments.Msdos
{
    /// <summary>
    /// Platform services for the 32-bit Extended MS-DOS operating environment.
    /// </summary>
    public class Msdos386Platform : Platform
    {
        private readonly HashSet<RegisterStorage> implicitRegs = new HashSet<RegisterStorage>
        {
            Registers.cs,
            Registers.ss,
            Registers.sp,
            Registers.esp,
            Registers.Top,
        };

        private SystemService[] interruptServices;

        public Msdos386Platform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "ms-dos-386")
        {
            interruptServices = null!;
            this.StructureMemberAlignment = 1;
            this.TrashedRegisters = CreateTrashedRegisters();
        }

        public override string DefaultCallingConvention => "cdecl";

        public override bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            return implicitRegs.Contains(reg);
        }

        public override CParser CreateCParser(TextReader rdr, ParserState? state)
        {
            state ??= new ParserState();
            var lexer = new CLexer(rdr, CLexer.MsvcKeywords);
            var parser = new CParser(state, lexer);
            return parser;
        }

        private HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            //$REVIEW: is ebx preserved?
            return new HashSet<RegisterStorage>
            {
                Registers.eax,
                Registers.ecx,
                Registers.edx,
                Registers.ebx,
                Registers.esp,
                Registers.Top,
            };
        }

        public override TypeLibrary EnsureTypeLibraries(string envName)
        {
            var metadata = base.EnsureTypeLibraries(envName);
            LoadInterruptServices();
            return metadata;
        }

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            EnsureTypeLibraries(PlatformIdentifier);
            if (this.Metadata is not null && this.Metadata.Modules.TryGetValue("", out var module))
            {
                if (!module.ServicesByVector.TryGetValue(vector, out var services))
                    return null;
                foreach (SystemService service in services)
                {
                    if (service.SyscallInfo is not null && service.SyscallInfo.Matches(vector, state))
                        return service;
                }
            }
            foreach (SystemService svc in interruptServices)
            {
                if (svc.SyscallInfo!.Matches(vector, state))
                    return svc;
            }
            return null;
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
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

        public override ICallingConvention GetCallingConvention(string? ccName)
        {
            return new X86CallingConvention(
                4,
                4,
                true,
                false);
        }

        public void LoadInterruptServices()
        {
            var fsSvc = Services.RequireService<IFileSystemService>();
            var prefix = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var libPath = Path.Combine(prefix, "realmodeintservices.xml");
            if (!fsSvc.FileExists(libPath))
            {
                libPath = Path.Combine(Directory.GetCurrentDirectory(), "realmodeintservices.xml");
            }

            SerializedLibrary lib;
            using (Stream stm = fsSvc.CreateFileStream(libPath, FileMode.Open, FileAccess.Read))
            {
                lib = SerializedLibrary.LoadFromStream(stm);
            }

            this.interruptServices = lib.Procedures
                .Cast<SerializedService>()
                .Select(s => ExtendRegisters(s))
                .Select(s => s.Build(this, Metadata!))
                .ToArray();
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This hack loops through all the registers in a <see cref="SerializedService"/> loaded
        /// from the realmodeintservices.xml file and replaces sequences that look like segmented 
        /// 32-bit pointers with their flat 32-bit equivalents.
        /// It's a placeholder and should be replaced by a "protectedmodeintservices.xml" that should
        /// generated from realmodeintservices.xml.
        /// </summary>
        private SerializedService ExtendRegisters(SerializedService svc)
        {
            if (svc.Signature is not null && svc.Signature.Arguments is not null)
            {
                var args = new List<Argument_v1>();
                foreach (var arg in svc.Signature.Arguments)
                {
                    if (arg.Type is PointerType_v1 ptr &&
                        arg.Kind is SerializedSequence seq &&
                        seq.Registers is not null && 
                        seq.Registers.Length == 2 &&
                        seq.Registers[1].Name is not null)
                    {
                        var off = seq.Registers[1].Name;
                        var eoff = "e" + off;
                        var argNew = new Argument_v1
                        {
                            Name = arg.Name,
                            Kind = new Register_v1 { Name = eoff },
                            OutParameter = arg.OutParameter,
                            Type = arg.Type,
                        };
                        args.Add(argNew);
                    }
                    else
                    {
                        args.Add(arg);
                    }
                }
                svc.Signature.Arguments = args.ToArray();
            }
            return svc;
        }

        /// Translated from Japanese text at:
        /// https://raw.githubusercontent.com/nabe-abk/free386/main/doc-ja/api.txt
        /*
GDT:
　　Selector value Base address Size attribute
	000h 00000000h 0 B-Null Selector
	008h LOW-In memory      64KB    R / X Free386 CS
	010h (same as 08h)      64KB    R / W Free386 DS
	018h (same as 08h)      64KB    R / X: 286 Free386 CS (unused)
	020h (same as 08h)      64KB    R / W: 286 Free386 DS (unused)
	028h Free386 Internal   512 B   LDT LDT (Selector to be LLDT)
	030h Free386 For        512 B   R / W LDT access inside the main unit
	038h Free386 For        512 B   R / W GDT access in the main unit
	040h 00000000h          --- MB  R / W Mounted memory (RAM) as a whole (*)
	050h Free386 For        512 B   R / X IDT access inside the main unit
	060h 00000000h         1088 KB  R / X DOS entire memory
	068h --------           --- KB  R / X VCPI Server CS
	070h --------           --- KB  R / W VCPI server data area
	078h -------- --- KB? /? By VCPI server
	080h Free386 Inside 128 B TSS In use TSS
	088h (same as 80h) 128 BR / W for TSS access

	(*) Although it is arranged, the page is not always pasted to the end.
	    Correctness depends on VCPI because it gets the size from the VCPI server.

LDT:
　　Selector value Base address Size attribute
	004h (same as 08h) 256 B    R / W PSP
	00ch (1MB or more) --- MB   R / X User Program CS
	014h (same as 0ch) 〃  MB   R / W user program DS
	024h (same as 04h) 256 B    R / W PSP
	02ch LOW-In memory --- B    R / W ENV / DOS environment memory
	034h 00000000h     1088KB   R / W DOS entire memory

	(*) Do not apply 0ch or 14h to the selector of your program.
	    It will be an obstacle when trying to multitask.
*/
    }
}
