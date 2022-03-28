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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.ImageLoaders.MzExe.Pe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe
{
    public abstract class AbstractPeLoader : ProgramImageLoader
    {
        protected const ushort MACHINE_x86_64 = (ushort) 0x8664u;
        protected const ushort MACHINE_m68k = (ushort) 0x0268;
        protected const ushort MACHINE_UNKNOWN = 0x0;         // The contents of this field are assumed to be applicable to any machine type
        protected const ushort MACHINE_AM33 = 0x1d3;          // Matsushita AM33
        protected const ushort MACHINE_AMD64 = (ushort) 0x8664;    // x64
        protected const ushort MACHINE_ARM = 0x01c0;          // ARM little endian -- "Modern" Windows
        protected const ushort MACHINE_ARM64 = 0xAA64;        // ARM64 little endian
        protected const ushort MACHINE_ARMNT = 0x01C4;        // ARM Thumb-2 little endian -- WinCE
        protected const ushort MACHINE_EBC = 0x0ebc;         // EFI byte code
        protected const ushort MACHINE_I386 = 0x014c;         // Intel 386 or later processors and compatible processors
        protected const ushort MACHINE_IA64 = 0x0200;         // Intel Itanium processor family
        protected const ushort MACHINE_M32R = 0x9041;         // Mitsubishi M32R little endian
        protected const ushort MACHINE_MIPS16 = 0x0266;       // MIPS16
        protected const ushort MACHINE_MIPSFPU = 0x0366;      // MIPS with FPU
        protected const ushort MACHINE_MIPSFPU16 = 0x0466;    // MIPS16 with FPU
        protected const ushort MACHINE_POWERPC = 0x01f0;      // Power PC little endian
        protected const ushort MACHINE_POWERPCFP = 0x01f1;    // Power PC with floating point support
        protected const ushort MACHINE_POWERPC_BE = (ushort) 0x0601;       // Big-endian PC: intended for PowerMac (!)
        protected const ushort MACHINE_XBOX360 = 0x01F2;         // Xbox 360
        protected const ushort MACHINE_R4000 = 0x0166;        // MIPS little endian
        protected const ushort MACHINE_RISCV32 = 0x5032;      // RISC-V 32-bit address space
        protected const ushort MACHINE_RISCV64 = 0x5064;      // RISC-V 64-bit address space
        protected const ushort MACHINE_RISCV128 = 0x5128;     // RISC-V 128-bit address space
        protected const ushort MACHINE_SH3 = 0x01a2;          // Hitachi SH3
        protected const ushort MACHINE_SH3DSP = 0x01a3;       // Hitachi SH3 DSP
        protected const ushort MACHINE_SH4 = 0x01a6;          // Hitachi SH4
        protected const ushort MACHINE_SH5 = 0x01a8;          // Hitachi SH5
        protected const ushort MACHINE_THUMB = 0x01c2;        // Thumb
        protected const ushort MACHINE_WCEMIPSV2 = 0x0169;    // MIPS little-endian WCE v2
        protected const ushort MACHINE_ALPHA = (ushort) 0x0184;


        protected const uint SectionFlagsInitialized = 0x00000040;
        protected const uint SectionFlagsDiscardable = 0x02000000;
        protected const uint SectionFlagsWriteable = 0x80000000;
        protected const uint SectionFlagsExecutable = 0x20000020;


        protected Dictionary<Address, ImportReference> importReferences;


        protected AbstractPeLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) 
            : base(services, imageLocation, imgRaw)
        {
            importReferences = new Dictionary<Address, ImportReference>();
        }

        protected abstract SizeSpecificLoader Create32BitLoader(AbstractPeLoader outer);
        protected abstract SizeSpecificLoader Create64BitLoader(AbstractPeLoader outer);

        protected static AccessMode AccessFromCharacteristics(uint characteristics)
        {
            AccessMode acc = AccessMode.Read;
            if ((characteristics & SectionFlagsWriteable) != 0)
            {
                acc |= AccessMode.Write;
            }
            if ((characteristics & SectionFlagsExecutable) != 0)
            {
                acc |= AccessMode.Execute;
            }
            return acc;
        }

        public IProcessorArchitecture CreateArchitecture(ushort peMachineType)
        {
            string arch;
            var cfgSvc = Services.RequireService<IConfigurationService>();
            switch (peMachineType)
            {
            case MACHINE_ALPHA: arch = "alpha"; break;
            case MACHINE_ARM: arch = "arm"; break;
            case MACHINE_ARM64: arch = "arm-64"; break;
            case MACHINE_ARMNT: arch = "arm-thumb"; break;
            case MACHINE_I386: arch = "x86-protected-32"; break;
            case MACHINE_x86_64: arch = "x86-protected-64"; break;
            case MACHINE_m68k: arch = "m68k"; break;
            case MACHINE_R4000: arch = "mips-le-32"; break;
            case MACHINE_POWERPC: arch = "ppc-le-32"; break;
            case MACHINE_POWERPC_BE: arch = "ppc-be-32"; break;
            case MACHINE_XBOX360: arch = "ppc-be-64"; break;
            default: throw new NotSupportedException($"Unsupported machine type 0x{peMachineType:X4} in PE header.");
            }
            return cfgSvc.GetArchitecture(arch)!;
        }

        protected SizeSpecificLoader CreateInnerLoader(ushort peMachineType)
        {
            switch (peMachineType)
            {
            case MACHINE_ALPHA:
            case MACHINE_ARM:
            case MACHINE_ARMNT:
            case MACHINE_I386:
            case MACHINE_m68k:
            case MACHINE_R4000:
            case MACHINE_POWERPC:
            case MACHINE_POWERPC_BE:
            case MACHINE_XBOX360:
                return Create32BitLoader(this);
            case MACHINE_x86_64:
            case MACHINE_ARM64:
                return Create64BitLoader(this);
            default: throw new ArgumentException(string.Format("Unsupported machine type 0x:{0:X4} in PE header.", peMachineType));
            }
        }

        public IPlatform CreatePlatform(
            ushort peMachineType, 
            IServiceProvider sp,
            IProcessorArchitecture arch)
        {
            string env;
            switch (peMachineType)
            {
            case MACHINE_ALPHA: env = "winAlpha"; break;
            case MACHINE_ARM: env = "winArm"; break;
            case MACHINE_ARM64: env = "winArm64"; break;
            case MACHINE_ARMNT: env = "winArm"; break;
            case MACHINE_I386: env = "win32"; break;
            case MACHINE_x86_64: env = "win64"; break;
            case MACHINE_m68k: env = "winM68k"; break;
            case MACHINE_R4000: env = "winMips"; break;
            case MACHINE_POWERPC: env = "winPpc32"; break;
            case MACHINE_POWERPC_BE: env = "winPpc32"; break;   //$REVIEW: this probably should be macOS-ppc
            case MACHINE_XBOX360: env = "xbox360"; break;
            default: throw new ArgumentException(string.Format("Unsupported machine type 0x:{0:X4} in PE hader.", peMachineType));
            }
            return Services.RequireService<IConfigurationService>()
                .GetEnvironment(env)
                .Load(Services, arch);
        }

        protected Relocator CreateRelocator(ushort peMachineType, Program program)
        {
            switch (peMachineType)
            {
            case MACHINE_ALPHA: return new AlphaRelocator(Services, program);
            case MACHINE_ARM: return new ArmRelocator(program);
            case MACHINE_ARM64: return new Arm64Relocator(Services, program);
            case MACHINE_ARMNT: return new ArmRelocator(program);
            case MACHINE_I386: return new i386Relocator(Services, program);
            case MACHINE_R4000: return new MipsRelocator(Services, program);
            case MACHINE_x86_64: return new x86_64Relocator(Services, program);
            case MACHINE_m68k: return new M68kRelocator(Services, program);
            case MACHINE_POWERPC: return new PowerPcRelocator(Services, program);
            case MACHINE_POWERPC_BE: return new PowerPcRelocator(Services, program);    //$REVIEW do we need a big-endian version of this?
            case MACHINE_XBOX360: return new PowerPcRelocator(Services, program);       //$REVIEW do we need a big-endian version of this?

            default: throw new NotSupportedException($"Unsupported machine type 0x:{peMachineType:X4} in PE header.");
            }
        }

        protected static string? ReadSectionName(EndianImageReader rdr)
        {
            byte[] bytes = rdr.ReadBytes(8);
            Encoding asc = Encoding.ASCII;
            char[] chars = asc.GetChars(bytes);
            int i;
            for (i = chars.Length - 1; i >= 0; --i)
            {
                if (chars[i] != 0)
                {
                    ++i;
                    break;
                }
            }
            if (i < 0)
            {
                return null;
            }
            return new String(chars, 0, i);
        }

        public string? ReadUtf8String(ByteMemoryArea imgLoaded, uint rva, int maxLength)
        {
            if (rva == 0)
                return null;
            EndianImageReader rdr = imgLoaded.CreateLeReader(rva);
            var bytes = new List<byte>();
            byte b;
            while ((b = rdr.ReadByte()) != 0)
            {
                bytes.Add(b);
                if (bytes.Count == maxLength)
                    break;
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        protected abstract class SizeSpecificLoader
        {
            protected AbstractPeLoader outer;

            public SizeSpecificLoader(AbstractPeLoader outer)
            {
                this.outer = outer;
            }

            public abstract (ImportReference?, int) ResolveImportDescriptorEntry(ByteMemoryArea mem, string dllName, EndianImageReader rdrIlt, EndianImageReader rdrIat);

            public abstract bool ImportedFunctionNameSpecified(ulong rvaEntry);

            public ImportReference CreateImportReference(ByteMemoryArea mem, string dllName, ulong rvaEntry, Address addrThunk)
            {
                if (!ImportedFunctionNameSpecified(rvaEntry))
                {
                    return new OrdinalImportReference(
                        addrThunk, dllName, (int) rvaEntry & 0xFFFF, SymbolType.ExternalProcedure);
                }
                else
                {
                    string? fnName = outer.ReadUtf8String(mem, (uint) rvaEntry + 2, 0);
                    return new NamedImportReference(
                        addrThunk, dllName, fnName!, SymbolType.ExternalProcedure);
                }
            }

            public abstract Address ReadPreferredImageBase(EndianImageReader rdr);
            public abstract long ReadWord(EndianImageReader rdr);
        }

        protected class Pe32Loader : SizeSpecificLoader
        {
            public Pe32Loader(AbstractPeLoader outer) : base(outer) { }

            public override bool ImportedFunctionNameSpecified(ulong rvaEntry)
            {
                return (rvaEntry & 0x80000000) == 0;
            }

            public override Address ReadPreferredImageBase(EndianImageReader rdr)
            {
                uint rvaBaseOfData = rdr.ReadLeUInt32();        // Only exists in PE32, not PE32+
                return Address.Ptr32(rdr.ReadLeUInt32());
            }

            public override long ReadWord(EndianImageReader rdr)
            {
                return rdr.ReadInt32();
            }

            public override (ImportReference?, int) ResolveImportDescriptorEntry(ByteMemoryArea mem, string dllName, EndianImageReader rdrIlt, EndianImageReader rdrIat)
            {
                Address addrThunk = rdrIat.Address;
                uint iatEntry = rdrIat.ReadLeUInt32();
                uint iltEntry = rdrIlt.ReadLeUInt32();
                if (iltEntry == 0)
                    return (null, 0);

                var impRef = CreateImportReference(mem, dllName, iltEntry, addrThunk);
                outer.importReferences.Add(addrThunk, impRef);

                return (impRef, 32);
            }
        }

        protected class Pe64Loader : SizeSpecificLoader
        {
            public Pe64Loader(AbstractPeLoader outer) : base(outer) { }

            public override bool ImportedFunctionNameSpecified(ulong rvaEntry)
            {
                return (rvaEntry & 0x8000000000000000u) == 0;
            }

            public override Address ReadPreferredImageBase(EndianImageReader rdr)
            {
                return Address64.Ptr64(rdr.ReadLeUInt64());
            }

            public override long ReadWord(EndianImageReader rdr)
            {
                return rdr.ReadInt64();
            }

            public override (ImportReference?, int) ResolveImportDescriptorEntry(
                ByteMemoryArea mem,
                string dllName,
                EndianImageReader rdrIlt,
                EndianImageReader rdrIat)
            {
                Address addrThunk = rdrIat.Address;
                ulong iatEntry = rdrIat.ReadLeUInt64();
                ulong iltEntry = rdrIlt.ReadLeUInt64();
                if (iltEntry == 0)
                    return (null, 0);
                var impRef = CreateImportReference(mem, dllName, iltEntry, addrThunk);
                outer.importReferences.Add(addrThunk, impRef);
                return (impRef, 64);
            }
        }



    }
}
