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
using Reko.Core.IO;
using System;
using System.Runtime.InteropServices;

namespace Reko.ImageLoaders.Coff
{
    public class CoffSection
    {
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Endian(Endianness.LittleEndian)]
    public unsafe struct CoffSectionHeader
    {
        public fixed byte Name[8];
        public uint VirtualSize;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLineNumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLineNumbers;
        public CoffSectionCharacteristics Characteristics;
    }


    /// <summary>
    /// Symbol storage classes
    /// </summary>
    public static class SymbolStorageClass
    {
public const int IMAGE_SYM_CLASS_END_OF_FUNCTION = 0xFF;    // A special symbol that represents the end of function, for debugging purposes.
public const int IMAGE_SYM_CLASS_NULL = 0;                  // No assigned storage class.
public const int IMAGE_SYM_CLASS_AUTOMATIC = 1;             // The automatic(stack) variable.The Value field specifies the stack frame offset.
public const int IMAGE_SYM_CLASS_EXTERNAL = 2;              // A value that Microsoft tools use for external symbols. The Value field indicates the size if the section number is IMAGE_SYM_UNDEFINED (0). If the section number is not zero, then the Value field specifies the offset within the section.
public const int IMAGE_SYM_CLASS_STATIC = 3;                // The offset of the symbol within the section.If the Value field is zero, then the symbol represents a section name.
public const int IMAGE_SYM_CLASS_REGISTER = 4;      // A register variable.The Value field specifies the register number.
public const int IMAGE_SYM_CLASS_EXTERNAL_DEF = 5; // A symbol that is defined externally.
public const int IMAGE_SYM_CLASS_LABEL = 6;                 // A code label that is defined within the module. The Value field specifies the offset of the symbol within the section.
public const int IMAGE_SYM_CLASS_UNDEFINED_LABEL = 7; // A reference to a code label that is not defined.
public const int IMAGE_SYM_CLASS_MEMBER_OF_STRUCT = 8; // The structure member.The Value field specifies the n th member.
public const int IMAGE_SYM_CLASS_ARGUMENT = 9; // A formal argument (parameter) of a function. The Value field specifies the n th argument.
public const int IMAGE_SYM_CLASS_STRUCT_TAG = 10; // The structure tag-name entry.
public const int IMAGE_SYM_CLASS_MEMBER_OF_UNION = 11; // A union member.The Value field specifies the n th member.
public const int IMAGE_SYM_CLASS_UNION_TAG = 12;            // The Union tag-name entry.
public const int IMAGE_SYM_CLASS_TYPE_DEFINITION = 13;      // A Typedef entry.
public const int IMAGE_SYM_CLASS_UNDEFINED_STATIC = 14;     // A static data declaration.
public const int IMAGE_SYM_CLASS_ENUM_TAG = 15;             // An enumerated type tagname entry.
public const int IMAGE_SYM_CLASS_MEMBER_OF_ENUM = 16;       // A member of an enumeration.The Value field specifies the n th member.
public const int IMAGE_SYM_CLASS_REGISTER_PARAM = 17;       // A register parameter.
public const int IMAGE_SYM_CLASS_BIT_FIELD = 18;            // A bit-field reference. The Value field specifies the n th bit in the bit field.
public const int IMAGE_SYM_CLASS_BLOCK = 100;       // A.bb (beginning of block) or.eb(end of block) record. The Value field is the relocatable address of the code location.
public const int IMAGE_SYM_CLASS_FUNCTION = 101;    // A value that Microsoft tools use for symbol records that define the extent of a function: begin function(.bf ), end function( .ef ), and lines in function( .lf ). For.lf records, the Value field gives the number of source lines in the function.For.ef records, the Value field gives the size of the function code.
public const int IMAGE_SYM_CLASS_END_OF_STRUCT = 102;       // An end-of-structure entry.
public const int IMAGE_SYM_CLASS_FILE = 103;                // A value that Microsoft tools, as well as traditional COFF format, use for the source-file symbol record.The symbol is followed by auxiliary records that name the file.
public const int IMAGE_SYM_CLASS_SECTION = 104;             // A definition of a section (Microsoft tools use STATIC storage class instead).
public const int IMAGE_SYM_CLASS_WEAK_EXTERNAL = 105;       // A weak external.For more information, see Auxiliary Format 3: Weak Externals.
public const int IMAGE_SYM_CLASS_CLR_TOKEN = 107;           // A CLR token symbol. The name is an ASCII string that consists of the hexadecimal value of the token. For more information, see CLR Token Definition (Object Only).
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 18)]
    [Endian(Endianness.LittleEndian)]
    public unsafe struct CoffSymbol
    {
        [FieldOffset(0)]
        public fixed byte e_name[8];
        [FieldOffset(0)]
        public uint e_zeroes;
        [FieldOffset(4)]
        public uint e_offset;
        [FieldOffset(8)]
        public uint e_value;
        [FieldOffset(12)]
        public short e_scnum;
        [FieldOffset(14)]
        public ushort e_type;
        [FieldOffset(16)]
        public byte e_sclass;
        [FieldOffset(17)]
        public byte e_numaux;
    }

    [StructLayout(LayoutKind.Explicit, Pack =1, Size =18)]
    public struct AuxSectionDefinition
    {
        [FieldOffset(0)]
        public uint Length;                 // The size of section data; the same as SizeOfRawData in the section header.
        [FieldOffset(4)]
        public ushort NumberOfRelocations;  // The number of relocation entries for the section.
        [FieldOffset(6)]
        public ushort NumberOfLinenumbers;  // The number of line-number entries for the section.
        [FieldOffset(8)]
        public uint CheckSum;               // The checksum for communal data. It is applicable if the IMAGE_SCN_LNK_COMDAT flag is set in the section header.For more information, see COMDAT Sections (Object Only).
        [FieldOffset(12)]
        public ushort Number;               // One-based index into the section table for the associated section.This is used when the COMDAT selection setting is 5.
        [FieldOffset(14)]
        public byte Selection;              // The COMDAT selection number.This is applicable if the section is a COMDAT section.
//15
//3
//Unused
    }

    [Flags]
    public enum CoffSectionCharacteristics : uint
    {
        //        	0x00000001  // Reserved for future use.
        //0x00000002  // Reserved for future use.
        //0x00000004  // Reserved for future use.
        // The section should not be padded to the next boundary.
        // This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES.
        // This is valid only for object files.
        IMAGE_SCN_TYPE_NO_PAD = 0x00000008,
        //0x00000010  //Reserved for future use.
        //The section contains executable code.
        IMAGE_SCN_CNT_CODE = 0x00000020,
        // The section contains initialized data.
        IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040,
        //The section contains uninitialized data.
        IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080,
        //        Reserved for future use.
        IMAGE_SCN_LNK_OTHER = 0x00000100,
        //The section contains comments or other information.The.drectve section has this type.This is valid for object files only.
        IMAGE_SCN_LNK_INFO = 0x00000200,


        //0x00000400 Reserved for future use.
        //The section will not become part of the image.This is valid only for object files.
        IMAGE_SCN_LNK_REMOVE = 0x00000800,
        //The section contains COMDAT data.For more information, see COMDAT Sections (Object Only). This is valid only for object files.

        IMAGE_SCN_LNK_COMDAT = 0x00001000,
        //The section contains data referenced through the global pointer (GP).
        IMAGE_SCN_GPREL = 0x00008000,
        //Reserved for future use.
        IMAGE_SCN_MEM_PURGEABLE = 0x00020000,
        //Reserved for future use.
        IMAGE_SCN_MEM_16BIT = 0x00020000,
        //Reserved for future use.
        IMAGE_SCN_MEM_LOCKED = 0x00040000,

        //Reserved for future use.
        IMAGE_SCN_MEM_PRELOAD = 0x00080000,

        IMAGE_SCN_ALIGN_MASK = 0x00F00000,

        //Align data on a 1-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_1BYTES = 0x00100000,
        //Align data on a 2-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_2BYTES = 0x00200000,
        //Align data on a 4-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_4BYTES = 0x00300000,
        //Align data on an 8-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_8BYTES = 0x00400000,
        //Align data on a 16-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_16BYTES = 0x00500000,
        // Align data on a 32-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_32BYTES = 0x00600000,
        // Align data on a 64-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_64BYTES = 0x00700000,
        // Align data on a 128-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_128BYTES = 0x00800000,
        // Align data on a 256-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_256BYTES = 0x00900000,
        // Align data on a 512-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_512BYTES = 0x00A00000,
        // Align data on a 1024-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_1024BYTES = 0x00B00000,
        // Align data on a 2048-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_2048BYTES = 0x00C00000,
        // Align data on a 4096-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_4096BYTES = 0x00D00000,
        // Align data on an 8192-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_8192BYTES = 0x00E00000,
        // The section contains extended relocations.
        IMAGE_SCN_LNK_NRELOC_OVFL = 0x01000000,
        // The section can be discarded as needed.
        IMAGE_SCN_MEM_DISCARDABLE = 0x02000000,
        // The section cannot be cached.
        IMAGE_SCN_MEM_NOT_CACHED = 0x04000000,
        // The section is not pageable.
        IMAGE_SCN_MEM_NOT_PAGED = 0x08000000,
        // The section can be shared in memory.
        IMAGE_SCN_MEM_SHARED = 0x10000000,
        // The section can be executed as code.
        IMAGE_SCN_MEM_EXECUTE = 0x20000000,
        // The section can be read.
        IMAGE_SCN_MEM_READ = 0x40000000,
        // The section can be written.
        IMAGE_SCN_MEM_WRITE = 0x80000000
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 10)]
    public struct CoffRelocation
    {
        [FieldOffset(0)]
        public uint VirtualAddress;     // The address of the item to which relocation is applied.
                                        // This is the offset from the beginning of the section, plus
                                        // the value of the section's RVA/Offset field. See Section
                                        // Table (Section Headers). For example, if the first byte of
                                        // the section has an address of 0x10, the third byte has an address
                                        // of 0x12.
        [FieldOffset(4)]
        public uint SymbolTableIndex;   // A zero-based index into the symbol table.
                                        // This symbol gives the address that is to be used for the relocation.
                                        // If the specified symbol has section storage class, then the symbol's
                                        // address is the address with the first section of the same name.
        [FieldOffset(8)]
        public ushort Type;             // A value that indicates the kind of relocation that should be performed.
                                        // Valid relocation types depend on machine type.

        // The Type field of the relocation record indicates what kind of relocation should be performed.
        // Different relocation types are defined for each type of machine.

        //The following relocation type indicators are defined for x64 and compatible processors.

        public const int IMAGE_REL_AMD64_ABSOLUTE = 0x0000;      // The relocation is ignored.
        public const int IMAGE_REL_AMD64_ADDR64 = 0x0001;      // The 64-bit VA of the relocation target.
        public const int IMAGE_REL_AMD64_ADDR32 = 0x0002;      // The 32-bit VA of the relocation target.
        public const int IMAGE_REL_AMD64_ADDR32NB = 0x0003;      // The 32-bit address without an image base (RVA).
        public const int IMAGE_REL_AMD64_REL32 = 0x0004;      // The 32-bit relative address from the byte following the relocation.
        public const int IMAGE_REL_AMD64_REL32_1 = 0x0005;      // The 32-bit address relative to byte distance 1 from the relocation.
        public const int IMAGE_REL_AMD64_REL32_2 = 0x0006;      // The 32-bit address relative to byte distance 2 from the relocation.
        public const int IMAGE_REL_AMD64_REL32_3 = 0x0007;      // The 32-bit address relative to byte distance 3 from the relocation.
        public const int IMAGE_REL_AMD64_REL32_4 = 0x0008;      // The 32-bit address relative to byte distance 4 from the relocation.
        public const int IMAGE_REL_AMD64_REL32_5 = 0x0009;      // The 32-bit address relative to byte distance 5 from the relocation.
        public const int IMAGE_REL_AMD64_SECTION = 0x000A;      // The 16-bit section index of the section that contains the target. This is used to support debugging information.
        public const int IMAGE_REL_AMD64_SECREL = 0x000B;      // The 32-bit offset of the target from the beginning of its section.This is used to support debugging information and static thread local storage.
        public const int IMAGE_REL_AMD64_SECREL7 = 0x000C;      // A 7-bit unsigned offset from the base of the section that contains the target.
        public const int IMAGE_REL_AMD64_TOKEN = 0x000D;      // CLR tokens.
        public const int IMAGE_REL_AMD64_SREL32 = 0x000E;      // A 32-bit signed span-dependent value emitted into the object.
        public const int IMAGE_REL_AMD64_PAIR = 0x000F;      // A pair that must immediately follow every span-dependent value.
        public const int IMAGE_REL_AMD64_SSPAN32 = 0x0010;      // A 32-bit signed span-dependent value that is applied at link time.



        //The following relocation type indicators are defined for ARM processors.

        public const int IMAGE_REL_ARM_ABSOLUTE = 0x0000;      // The relocation is ignored.
        public const int IMAGE_REL_ARM_ADDR32 = 0x0001;      // The 32-bit VA of the target.
        public const int IMAGE_REL_ARM_ADDR32NB = 0x0002;      // The 32-bit RVA of the target.
        public const int IMAGE_REL_ARM_BRANCH24 = 0x0003;      // The 24-bit relative displacement to the target.
        public const int IMAGE_REL_ARM_BRANCH11 = 0x0004;      // The reference to a subroutine call. The reference consists of two 16-bit instructions with 11-bit offsets.
        public const int IMAGE_REL_ARM_REL32 = 0x000A;      // The 32-bit relative address from the byte following the relocation.
        public const int IMAGE_REL_ARM_SECTION = 0x000E;      // The 16-bit section index of the section that contains the target. This is used to support debugging information.
        public const int IMAGE_REL_ARM_SECREL = 0x000F;      // The 32-bit offset of the target from the beginning of its section.This is used to support debugging information and static thread local storage.
        public const int IMAGE_REL_ARM_MOV32 = 0x0010;      // The 32-bit VA of the target.This relocation is applied using a MOVW instruction for the low 16 bits followed by a MOVT for the high 16 bits.
        public const int IMAGE_REL_THUMB_MOV32 = 0x0011;      // The 32-bit VA of the target.This relocation is applied using a MOVW instruction for the low 16 bits followed by a MOVT for the high 16 bits.
        public const int IMAGE_REL_THUMB_BRANCH20 = 0x0012;      // The instruction is fixed up with the 21 - bit relative displacement to the 2 - byte aligned target.The least significant bit of the displacement is always zero and is not stored.This relocation corresponds to a Thumb - 2 32 - bit conditional B instruction.
        public const int IMAGE_REL_THUMB_BRANCH24 = 0x0014;      // The instruction is fixed up with the 25 - bit relative displacement to the 2 - byte aligned target.The least significant bit of the displacement is zero and is not stored.This relocation corresponds to a Thumb - 2 B instruction.
        public const int IMAGE_REL_THUMB_BLX23 = 0x0015;      // The instruction is fixed up with the 25 - bit relative displacement to the 4 - byte aligned target.The low 2 bits of the displacement are zero and are not stored.
                                                              // This relocation corresponds to a Thumb-2 BLX instruction.
        public const int IMAGE_REL_ARM_PAIR = 0x0016;      // The relocation is valid only when it immediately follows a ARM_REFHI or THUMB_REFHI.Its SymbolTableIndex contains a displacement and not an index into the symbol table.



        //The following relocation type indicators are defined for ARM64 processors.

        public const int IMAGE_REL_ARM64_ABSOLUTE = 0x0000;      // The relocation is ignored.
        public const int IMAGE_REL_ARM64_ADDR32 = 0x0001;      // The 32 - bit VA of the target.
        public const int IMAGE_REL_ARM64_ADDR32NB = 0x0002;      // The 32 - bit RVA of the target.
        public const int IMAGE_REL_ARM64_BRANCH26 = 0x0003;      // The 26 - bit relative displacement to the target, for B and BL instructions.
        public const int IMAGE_REL_ARM64_PAGEBASE_REL21 = 0x0004;      // The page base of the target, for ADRP instruction.
        public const int IMAGE_REL_ARM64_REL21 = 0x0005;      // The 12 - bit relative displacement to the target, for instruction ADR
        public const int IMAGE_REL_ARM64_PAGEOFFSET_12A = 0x0006;      // The 12 - bit page offset of the target, for instructions ADD / ADDS(immediate) with zero shift.
        public const int IMAGE_REL_ARM64_PAGEOFFSET_12L = 0x0007;      // The 12 - bit page offset of the target, for instruction LDR(indexed, unsigned immediate).
        public const int IMAGE_REL_ARM64_SECREL = 0x0008;      // The 32 - bit offset of the target from the beginning of its section.This is used to support debugging information and static thread local storage.
        public const int IMAGE_REL_ARM64_SECREL_LOW12A = 0x0009;      // Bit 0:11 of section offset of the target, for instructions ADD/ADDS(immediate) with zero shift.
        public const int IMAGE_REL_ARM64_SECREL_HIGH12A = 0x000A;      // Bit 12:23 of section offset of the target, for instructions ADD/ADDS(immediate) with zero shift.
        public const int IMAGE_REL_ARM64_SECREL_LOW12L = 0x000B;      // Bit 0:11 of section offset of the target, for instruction LDR(indexed, unsigned immediate).
        public const int IMAGE_REL_ARM64_TOKEN = 0x000C;      // CLR token.
        public const int IMAGE_REL_ARM64_SECTION = 0x000D;      // The 16-bit section index of the section that contains the target. This is used to support debugging information.
        public const int IMAGE_REL_ARM64_ADDR64 = 0x000E;      // The 64-bit VA of the relocation target.
        public const int IMAGE_REL_ARM64_BRANCH19 = 0x000F;      // The 19-bit offset to the relocation target, for conditional B instruction.
        public const int IMAGE_REL_ARM64_BRANCH14 = 0x0010;      // The 14-bit offset to the relocation target, for instructions TBZ and TBNZ.
        public const int IMAGE_REL_ARM64_REL32 = 0x0011;      // The 32-bit relative address from the byte following the relocation.

        //The following relocation type indicators are defined for SH3 and SH4 processors. SH5-specific relocations are noted as SHM (SH Media).

        public const int IMAGE_REL_SH3_ABSOLUTE = 0x0000;      // The relocation is ignored.
        public const int IMAGE_REL_SH3_DIRECT16 = 0x0001;      // A reference to the 16-bit location that contains the VA of the target symbol.
        public const int IMAGE_REL_SH3_DIRECT32 = 0x0002;      // The 32-bit VA of the target symbol.
        public const int IMAGE_REL_SH3_DIRECT8 = 0x0003;      // A reference to the 8-bit location that contains the VA of the target symbol.
        public const int IMAGE_REL_SH3_DIRECT8_WORD = 0x0004;      // A reference to the 8-bit instruction that contains the effective 16-bit VA of the target symbol.
        public const int IMAGE_REL_SH3_DIRECT8_LONG = 0x0005;      // A reference to the 8-bit instruction that contains the effective 32-bit VA of the target symbol.
        public const int IMAGE_REL_SH3_DIRECT4 = 0x0006;      // A reference to the 8-bit location whose low 4 bits contain the VA of the target symbol.
        public const int IMAGE_REL_SH3_DIRECT4_WORD = 0x0007;      // A reference to the 8-bit instruction whose low 4 bits contain the effective 16-bit VA of the target symbol.
        public const int IMAGE_REL_SH3_DIRECT4_LONG = 0x0008;      // A reference to the 8-bit instruction whose low 4 bits contain the effective 32-bit VA of the target symbol.
        public const int IMAGE_REL_SH3_PCREL8_WORD = 0x0009;      // A reference to the 8-bit instruction that contains the effective 16-bit relative offset of the target symbol.
        public const int IMAGE_REL_SH3_PCREL8_LONG = 0x000A;      // A reference to the 8-bit instruction that contains the effective 32-bit relative offset of the target symbol.
        public const int IMAGE_REL_SH3_PCREL12_WORD = 0x000B;      // A reference to the 16-bit instruction whose low 12 bits contain the effective 16-bit relative offset of the target symbol.
        public const int IMAGE_REL_SH3_STARTOF_SECTION = 0x000C;      // A reference to a 32-bit location that is the VA of the section that contains the target symbol.
        public const int IMAGE_REL_SH3_SIZEOF_SECTION = 0x000D;      // A reference to the 32-bit location that is the size of the section that contains the target symbol.
        public const int IMAGE_REL_SH3_SECTION = 0x000E;      // The 16-bit section index of the section that contains the target. This is used to support debugging information.
        public const int IMAGE_REL_SH3_SECREL = 0x000F;      // The 32-bit offset of the target from the beginning of its section.This is used to support debugging information and static thread local storage.
        public const int IMAGE_REL_SH3_DIRECT32_NB = 0x0010;      // The 32-bit RVA of the target symbol.
        public const int IMAGE_REL_SH3_GPREL4_LONG = 0x0011;      // GP relative.
        public const int IMAGE_REL_SH3_TOKEN = 0x0012;      // CLR token.
        public const int IMAGE_REL_SHM_PCRELPT = 0x0013;      // The offset from the current instruction in longwords.If the NOMODE bit is not set, insert the inverse of the low bit at bit 32 to select PTA or PTB.
        public const int IMAGE_REL_SHM_REFLO = 0x0014;      // The low 16 bits of the 32-bit address.
        public const int IMAGE_REL_SHM_REFHALF = 0x0015;      // The high 16 bits of the 32-bit address.
        public const int IMAGE_REL_SHM_RELLO = 0x0016;      // The low 16 bits of the relative address.
        public const int IMAGE_REL_SHM_RELHALF = 0x0017;      // The high 16 bits of the relative address.
        public const int IMAGE_REL_SHM_PAIR = 0x0018;      // The relocation is valid only when it immediately follows a REFHALF, RELHALF, or RELLO relocation.The SymbolTableIndex field of the relocation contains a displacement and not an index into the symbol table.
        public const int IMAGE_REL_SHM_NOMODE = 0x8000;      // The relocation ignores section mode.


        //The following relocation type indicators are defined for PowerPC processors.

        public const int IMAGE_REL_PPC_ABSOLUTE = 0x0000;      // The relocation is ignored.
        public const int IMAGE_REL_PPC_ADDR64 = 0x0001;      // The 64-bit VA of the target.
        public const int IMAGE_REL_PPC_ADDR32 = 0x0002;      // The 32-bit VA of the target.
        public const int IMAGE_REL_PPC_ADDR24 = 0x0003;      // The low 24 bits of the VA of the target.This is valid only when the target symbol is absolute and can be sign-extended to its original value.
        public const int IMAGE_REL_PPC_ADDR16 = 0x0004;      // The low 16 bits of the target's VA.
        public const int IMAGE_REL_PPC_ADDR14 = 0x0005;      // The low 14 bits of the target's VA. This is valid only when the target symbol is absolute and can be sign-extended to its original value.
        public const int IMAGE_REL_PPC_REL24 = 0x0006;      // A 24-bit PC-relative offset to the symbol's location.
        public const int IMAGE_REL_PPC_REL14 = 0x0007;      // A 14-bit PC-relative offset to the symbol's location.
        public const int IMAGE_REL_PPC_ADDR32NB = 0x000A;      // The 32-bit RVA of the target.
        public const int IMAGE_REL_PPC_SECREL = 0x000B;      // The 32-bit offset of the target from the beginning of its section.This is used to support debugging information and static thread local storage.
        public const int IMAGE_REL_PPC_SECTION = 0x000C;      // The 16-bit section index of the section that contains the target.This is used to support debugging information.
        public const int IMAGE_REL_PPC_SECREL16 = 0x000F;      // The 16-bit offset of the target from the beginning of its section.This is used to support debugging information and static thread local storage.
        public const int IMAGE_REL_PPC_REFHI = 0x0010;      // The high 16 bits of the target's 32-bit VA. This is used for the first instruction in a two-instruction sequence that loads a full address. This relocation must be immediately followed by a PAIR relocation whose SymbolTableIndex contains a signed 16-bit displacement that is added to the upper 16 bits that was taken from the location that is being relocated.
        public const int IMAGE_REL_PPC_REFLO = 0x0011;      // The low 16 bits of the target's VA.
        public const int IMAGE_REL_PPC_PAIR = 0x0012;      // A relocation that is valid only when it immediately follows a REFHI or SECRELHI relocation.Its SymbolTableIndex contains a displacement and not an index into the symbol table.
        public const int IMAGE_REL_PPC_SECRELLO = 0x0013;      // The low 16 bits of the 32-bit offset of the target from the beginning of its section.
        public const int IMAGE_REL_PPC_GPREL = 0x0015;      // The 16-bit signed displacement of the target relative to the GP register.
        public const int IMAGE_REL_PPC_TOKEN = 0x0016;      // The CLR token.


        //The following relocation type indicators are defined for Intel 386 and compatible processors.

        public const int IMAGE_REL_I386_ABSOLUTE = 0x0000;      // The relocation is ignored.
        public const int IMAGE_REL_I386_DIR16 = 0x0001;      // Not supported.
        public const int IMAGE_REL_I386_REL16 = 0x0002;      // Not supported.
        public const int IMAGE_REL_I386_DIR32 = 0x0006;      // The target's 32-bit VA.
        public const int IMAGE_REL_I386_DIR32NB = 0x0007;      // The target's 32-bit RVA.
        public const int IMAGE_REL_I386_SEG12 = 0x0009;      // Not supported.
        public const int IMAGE_REL_I386_SECTION = 0x000A;      // The 16-bit section index of the section that contains the target. This is used to support debugging information.
        public const int IMAGE_REL_I386_SECREL = 0x000B;      // The 32-bit offset of the target from the beginning of its section.This is used to support debugging information and static thread local storage.
        public const int IMAGE_REL_I386_TOKEN = 0x000C;      // The CLR token.
        public const int IMAGE_REL_I386_SECREL7 = 0x000D;      // A 7-bit offset from the base of the section that contains the target.
        public const int IMAGE_REL_I386_REL32 = 0x0014;      // The 32-bit relative displacement to the target.This supports the x86 relative branch and call instructions.



        //The following relocation type indicators are defined for the Intel Itanium processor family
        //and compatible processors. Note that relocations on instructions use the bundle's offset and
        //slot number for the relocation offset.

        public const int IMAGE_REL_IA64_ABSOLUTE = 0x0000;      // The relocation is ignored.
        public const int IMAGE_REL_IA64_IMM14 = 0x0001;      // The instruction relocation can be followed by an ADDEND relocation whose value is added to the target address before it is inserted into the specified slot in the IMM14 bundle.The relocation target must be absolute or the image must be fixed.
        public const int IMAGE_REL_IA64_IMM22 = 0x0002;      // The instruction relocation can be followed by an ADDEND relocation whose value is added to the target address before it is inserted into the specified slot in the IMM22 bundle.The relocation target must be absolute or the image must be fixed.
        public const int IMAGE_REL_IA64_IMM64 = 0x0003;      // The slot number of this relocation must be one (1). The relocation can be followed by an ADDEND relocation whose value is added to the target address before it is stored in all three slots of the IMM64 bundle.
        public const int IMAGE_REL_IA64_DIR32 = 0x0004;      // The target's 32-bit VA. This is supported only for /LARGEADDRESSAWARE:NO images.
        public const int IMAGE_REL_IA64_DIR64 = 0x0005;      // The target's 64-bit VA.
        public const int IMAGE_REL_IA64_PCREL21B = 0x0006;      // The instruction is fixed up with the 25 - bit relative displacement to the 16 - bit aligned target.The low 4 bits of the displacement are zero and are not stored.
        public const int IMAGE_REL_IA64_PCREL21M = 0x0007;      // The instruction is fixed up with the 25 - bit relative displacement to the 16 - bit aligned target.The low 4 bits of the displacement, which are zero, are not stored.
        public const int IMAGE_REL_IA64_PCREL21F = 0x0008;      // The LSBs of this relocation's offset must contain the slot number whereas the rest is the bundle address. The bundle is fixed up with the 25-bit relative displacement to the 16-bit aligned target. The low 4 bits of the displacement are zero and are not stored.
        public const int IMAGE_REL_IA64_GPREL22 = 0x0009;      // The instruction relocation can be followed by an ADDEND relocation whose value is added to the target address and then a 22 - bit GP - relative offset that is calculated and applied to the GPREL22 bundle.
        public const int IMAGE_REL_IA64_LTOFF22 = 0x000A;      // The instruction is fixed up with the 22 - bit GP - relative offset to the target symbol's literal table entry. The linker creates this literal table entry based on this relocation and the ADDEND relocation that might follow.
        public const int IMAGE_REL_IA64_SECTION = 0x000B;      // The 16 - bit section index of the section contains the target.This is used to support debugging information.
        public const int IMAGE_REL_IA64_SECREL22 = 0x000C;      // The instruction is fixed up with the 22 - bit offset of the target from the beginning of its section.This relocation can be followed immediately by an ADDEND relocation, whose Value field contains the 32 - bit unsigned offset of the target from the beginning of the section.
        public const int IMAGE_REL_IA64_SECREL64I = 0x000D;      // The slot number for this relocation must be one(1).The instruction is fixed up with the 64 - bit offset of the target from the beginning of its section.This relocation can be followed immediately by an ADDEND relocation whose Value field contains the 32 - bit unsigned offset of the target from the beginning of the section.
        public const int IMAGE_REL_IA64_SECREL32 = 0x000E;      // The address of data to be fixed up with the 32 - bit offset of the target from the beginning of its section.
        public const int IMAGE_REL_IA64_DIR32NB = 0x0010;      // The target's 32-bit RVA.
        public const int IMAGE_REL_IA64_SREL14 = 0x0011;      // This is applied to a signed 14 - bit immediate that contains the difference between two relocatable targets.This is a declarative field for the linker that indicates that the compiler has already emitted this value.
        public const int IMAGE_REL_IA64_SREL22 = 0x0012;      // This is applied to a signed 22 - bit immediate that contains the difference between two relocatable targets.This is a declarative field for the linker that indicates that the compiler has already emitted this value.
        public const int IMAGE_REL_IA64_SREL32 = 0x0013;      // This is applied to a signed 32 - bit immediate that contains the difference between two relocatable values.This is a declarative field for the linker that indicates that the compiler has already emitted this value.
        public const int IMAGE_REL_IA64_UREL32 = 0x0014;      // This is applied to an unsigned 32 - bit immediate that contains the difference between two relocatable values.This is a declarative field for the linker that indicates that the compiler has already emitted this value.
        public const int IMAGE_REL_IA64_PCREL60X = 0x0015;      // A 60 - bit PC - relative fixup that always stays as a BRL instruction of an MLX bundle.
        public const int IMAGE_REL_IA64_PCREL60B = 0x0016;      // A 60 - bit PC - relative fixup.If the target displacement fits in a signed 25 - bit field, convert the entire bundle to an MBB bundle with NOP.B in slot 1 and a 25 - bit BR instruction(with the 4 lowest bits all zero and dropped) in slot 2.
        public const int IMAGE_REL_IA64_PCREL60F = 0x0017;      // A 60 - bit PC - relative fixup.If the target displacement fits in a signed 25 - bit field, convert the entire bundle to an MFB bundle with NOP.F in slot 1 and a 25 - bit(4 lowest bits all zero and dropped) BR instruction in slot 2.
        public const int IMAGE_REL_IA64_PCREL60I = 0x0018;      // A 60 - bit PC - relative fixup.If the target displacement fits in a signed 25 - bit field, convert the entire bundle to an MIB bundle with NOP.I in slot 1 and a 25 - bit(4 lowest bits all zero and dropped) BR instruction in slot 2.
        public const int IMAGE_REL_IA64_PCREL60M = 0x0019;      // A 60 - bit PC - relative fixup.If the target displacement fits in a signed 25 - bit field, convert the entire bundle to an MMB bundle with NOP.M in slot 1 and a 25 - bit(4 lowest bits all zero and dropped) BR instruction in slot 2.
        public const int IMAGE_REL_IA64_IMMGPREL64 = 0x001a;      // A 64 - bit GP - relative fixup.
        public const int IMAGE_REL_IA64_TOKEN = 0x001b;      // A CLR token.
        public const int IMAGE_REL_IA64_GPREL32 = 0x001c;      // A 32 - bit GP - relative fixup.
        public const int IMAGE_REL_IA64_ADDEND = 0x001F;      // The relocation is valid only when it immediately follows one of the following relocations: IMM14, IMM22, IMM64, GPREL22, LTOFF22, LTOFF64, SECREL22, SECREL64I, or SECREL32.Its value contains the addend to apply to instructions within a bundle, not for data.



        //The following relocation type indicators are defined for MIPS processors.

        public const int IMAGE_REL_MIPS_ABSOLUTE = 0x0000;      // The relocation is ignored.
        public const int IMAGE_REL_MIPS_REFHALF = 0x0001;      // The high 16 bits of the target's 32-bit VA.
        public const int IMAGE_REL_MIPS_REFWORD = 0x0002;      // The target's 32-bit VA.
        public const int IMAGE_REL_MIPS_JMPADDR = 0x0003;      // The low 26 bits of the target's VA. This supports the MIPS J and JAL instructions.
        public const int IMAGE_REL_MIPS_REFHI = 0x0004;      // The high 16 bits of the target's 32-bit VA. This is used for the first instruction in a two-instruction sequence that loads a full address. This relocation must be immediately followed by a PAIR relocation whose SymbolTableIndex contains a signed 16-bit displacement that is added to the upper 16 bits that are taken from the location that is being relocated.
        public const int IMAGE_REL_MIPS_REFLO = 0x0005;      // The low 16 bits of the target's VA.
        public const int IMAGE_REL_MIPS_GPREL = 0x0006;      // A 16 - bit signed displacement of the target relative to the GP register.
        public const int IMAGE_REL_MIPS_LITERAL = 0x0007;      // The same as IMAGE_REL_MIPS_GPREL.
        public const int IMAGE_REL_MIPS_SECTION = 0x000A;      // The 16 - bit section index of the section contains the target.This is used to support debugging information.
        public const int IMAGE_REL_MIPS_SECREL = 0x000B;      // The 32 - bit offset of the target from the beginning of its section.This is used to support debugging information and static thread local storage.
        public const int IMAGE_REL_MIPS_SECRELLO = 0x000C;      // The low 16 bits of the 32-bit offset of the target from the beginning of its section.
        public const int IMAGE_REL_MIPS_SECRELHI = 0x000D;      // The high 16 bits of the 32-bit offset of the target from the beginning of its section.An IMAGE_REL_MIPS_PAIR relocation must immediately follow this one.The SymbolTableIndex of the PAIR relocation contains a signed 16-bit displacement that is added to the upper 16 bits that are taken from the location that is being relocated.
        public const int IMAGE_REL_MIPS_JMPADDR16 = 0x0010;      // The low 26 bits of the target's VA. This supports the MIPS16 JAL instruction.
        public const int IMAGE_REL_MIPS_REFWORDNB = 0x0022;      // The target's 32-bit RVA.
        public const int IMAGE_REL_MIPS_PAIR = 0x0025;      // The relocation is valid only when it immediately follows a REFHI or SECRELHI relocation.Its SymbolTableIndex contains a displacement and not an index into the symbol table.


        //The following relocation type indicators are defined for the Mitsubishi M32R processors.

        public const int IMAGE_REL_M32R_ABSOLUTE = 0x0000;      // The relocation is ignored.
        public const int IMAGE_REL_M32R_ADDR32 = 0x0001;      // The target's 32-bit VA.
        public const int IMAGE_REL_M32R_ADDR32NB = 0x0002;      // The target's 32-bit RVA.
        public const int IMAGE_REL_M32R_ADDR24 = 0x0003;      // The target's 24-bit VA.
        public const int IMAGE_REL_M32R_GPREL16 = 0x0004;      // The target's 16-bit offset from the GP register.
        public const int IMAGE_REL_M32R_PCREL24 = 0x0005;      // The target's 24-bit offset from the program counter (PC), shifted left by 2 bits and sign-extended
        public const int IMAGE_REL_M32R_PCREL16 = 0x0006;      // The target's 16-bit offset from the PC, shifted left by 2 bits and sign-extended
        public const int IMAGE_REL_M32R_PCREL8 = 0x0007;      // The target's 8-bit offset from the PC, shifted left by 2 bits and sign-extended
        public const int IMAGE_REL_M32R_REFHALF = 0x0008;      // The 16 MSBs of the target VA.
        public const int IMAGE_REL_M32R_REFHI = 0x0009;      // The 16 MSBs of the target VA, adjusted for LSB sign extension.This is used for the first instruction in a two-instruction sequence that loads a full 32-bit address. This relocation must be immediately followed by a PAIR relocation whose SymbolTableIndex contains a signed 16-bit displacement that is added to the upper 16 bits that are taken from the location that is being relocated.
        public const int IMAGE_REL_M32R_REFLO = 0x000A;      // The 16 LSBs of the target VA.
        public const int IMAGE_REL_M32R_PAIR = 0x000B;      // The relocation must follow the REFHI relocation.Its SymbolTableIndex contains a displacement and not an index into the symbol table.
        public const int IMAGE_REL_M32R_SECTION = 0x000C;      // The 16-bit section index of the section that contains the target. This is used to support debugging information.
        public const int IMAGE_REL_M32R_SECREL = 0x000D;      // The 32-bit offset of the target from the beginning of its section.This is used to support debugging information and static thread local storage.
        public const int IMAGE_REL_M32R_TOKEN = 0x000E;      // The CLR token.
    }
}