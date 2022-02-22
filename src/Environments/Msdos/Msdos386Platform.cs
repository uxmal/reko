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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Hll.C;
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
        }

        public override string DefaultCallingConvention => "cdecl";

        public override bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            return implicitRegs.Contains(reg);
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
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

        public override void EnsureTypeLibraries(string envName)
        {
            base.EnsureTypeLibraries(envName);
            LoadInterruptServices(Architecture);
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            EnsureTypeLibraries(PlatformIdentifier);
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

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            return new X86CallingConvention(
                4,
                4,
                4,
                true,
                false);
        }

        public void LoadInterruptServices(IProcessorArchitecture arch)
        {
            var prefix = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var libPath = Path.Combine(prefix, "realmodeintservices.xml");
            if (!File.Exists(libPath))
            {
                libPath = Path.Combine(Directory.GetCurrentDirectory(), "realmodeintservices.xml");
            }

            SerializedLibrary lib;
            var fsSvc = Services.RequireService<IFileSystemService>();
            using (Stream stm = fsSvc.CreateFileStream(libPath, FileMode.Open, FileAccess.Read))
            {
                lib = SerializedLibrary.LoadFromStream(stm);
            }

            this.interruptServices = lib.Procedures
                .Cast<SerializedService>()
                .Select(s => ExtendRegisters(s))
                .Select(s => s.Build(this, Metadata))
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
            if (svc.Signature != null && svc.Signature.Arguments != null)
            {
                var args = new List<Argument_v1>();
                foreach (var arg in svc.Signature.Arguments)
                {
                    if (arg.Type is PointerType_v1 ptr &&
                        arg.Kind is SerializedSequence seq &&
                        seq.Registers != null && 
                        seq.Registers.Length == 2 &&
                        seq.Registers[1].Name != null)
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

    }
}
