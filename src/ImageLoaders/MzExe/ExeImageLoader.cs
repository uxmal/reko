#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Services;
using Reko.Environments.Msdos;
using System;
using System.Collections.Generic;

namespace Reko.ImageLoaders.MzExe
{
	/// <summary>
	/// Loads EXE image files. These may contain executable code in varying formats. 
	/// </summary>
	public class ExeImageLoader : ImageLoader
	{
        private ImageLoader ldrDeferred;
        private IServiceProvider services;

		public ushort	e_magic;                     // 0000 - Magic number
		public ushort   e_cbLastPage;                // 0002 - Bytes on last page of file
		public ushort   e_cpImage;                   // 0004 - Pages in file
		public ushort   e_cRelocations;              // 0006 - Relocations

		public ushort   e_cparHeader;                // 0008 - Size of header in paragraphs
		public ushort   e_minalloc;                  // 000A - Minimum extra paragraphs needed
		public ushort   e_maxalloc;                  // 000C - Maximum extra paragraphs needed
		public ushort   e_ss;                        // 000E - Initial (relative) SS value

		public ushort   e_sp;                        // 0010 - Initial SP value
		public ushort   e_csum;                      // 0012 - Checksum
		public ushort   e_ip;                        // 0014 - Initial IP value
		public ushort   e_cs;                        // 0016 - Initial (relative) CS value

		public ushort   e_lfaRelocations;			 // 0018 - File address of relocation table
		public ushort   e_ovno;                      // 001A - Overlay number

		private const int MarkZbikowski = (('Z' << 8) | 'M');		// 'MZ' magic number expressed in little-endian.
		public const int CbPsp = 0x0100;			// Program segment prefix size in bytes.
		public const int CbPageSize = 0x0200;		// MSDOS pages are 512 bytes.
        private const int e_lfanewOffset = 0x003C;      // offset of the field where the LFA to a new
                                                        // EXE format header is located. This field is only valid
                                                        // if the e_lfaRelocations field >= 0x40

        public ExeImageLoader(IServiceProvider services, string filename, byte [] image) : base(services, filename, image)
		{
            this.services = services;
            ReadCommonExeFields();	
		
			if (e_magic != MarkZbikowski)
				throw new FormatException("Image is not an MS-DOS executable image.");
		}

        private ImageLoader GetDeferredLoader()
        {
            if (ldrDeferred == null)
                ldrDeferred = CreateDeferredLoader();
            return ldrDeferred;
        }

		public bool IsNewExecutable(uint e_lfanew)
		{
			return (uint) RawImage.Length > (uint) (e_lfanew + 1) && RawImage[e_lfanew] == 'N' && RawImage[e_lfanew+1] == 'E';
		}

		public bool IsPortableExecutable(uint e_lfanew)
		{
			return (uint) RawImage.Length > (uint) (e_lfanew + 1) && RawImage[e_lfanew] == 'P' && RawImage[e_lfanew+1] == 'E';
		}

		/// <summary>
		/// Loads a Microsoft .EXE file. There are several widely varying 
        /// sub-formats, so we need to discover what flavour it is before we
        /// can proceed.
		/// </summary>
        public override Program Load(Address addrLoad)
		{
			return GetDeferredLoader().Load(addrLoad);
		}

        private ImageLoader CreateDeferredLoader()
        {
            // The image may have been packed. We ask the unpacker service 
            // whether it can determine if the image is packed, and if so 
            // provide us with an image loader that knows how to do unpacking.

            var loaderSvc = services.RequireService<IUnpackerService>();

            uint? e_lfanew = LoadLfaToNewHeader();
            if (e_lfanew.HasValue)
            { 
                // It seems this file could have a new header.
                if (IsPortableExecutable(e_lfanew.Value))
                {
                    var peLdr = new PeImageLoader(services, Filename, base.RawImage, e_lfanew.Value);
                    uint peEntryPointOffset = peLdr.ReadEntryPointRva();
                    return loaderSvc.FindUnpackerBySignature(peLdr, peEntryPointOffset);
                }
                else if (IsNewExecutable(e_lfanew.Value))
                {
                    // http://support.microsoft.com/kb/65122
                    var neLdr = new NeImageLoader(services, Filename, base.RawImage, e_lfanew.Value);
                    return neLdr;
                }
            }

            // Fall back to loading real-mode MS-DOS program.
            var msdosLoader = new MsdosImageLoader(this);
            var msdosEntryPointOffset = (((e_cparHeader + e_cs) << 4) + e_ip) & 0xFFFFF;
            return loaderSvc.FindUnpackerBySignature(msdosLoader, (uint) msdosEntryPointOffset);
        }

        /// <summary>
        /// Attempt to load the linear file address of the new header.
        /// </summary>
        /// <remarks>
        /// Windows will happily load PE executables even if e_lfaRelocations is 
        /// 0. So we fall back to first making sure we can read the lfanew field, and then
        /// make sure it doesn't point outside the image.
        /// </remarks>
        public uint? LoadLfaToNewHeader()
        {
            if (RawImage.Length < e_lfanewOffset + 4)
                return null;
            
            uint e_lfanew = MemoryArea.ReadLeUInt32(base.RawImage, e_lfanewOffset);
            if (e_lfanew == 0 || e_lfanew + 4 >= RawImage.Length)
                return null;
            return e_lfanew;
        }

        public override Address PreferredBaseAddress
		{
			get { return GetDeferredLoader().PreferredBaseAddress; }
            set { throw new NotImplementedException(); }
        }

		public void ReadCommonExeFields()
		{
			EndianImageReader rdr = new LeImageReader(RawImage, 0);

			e_magic = rdr.ReadLeUInt16();
			e_cbLastPage = rdr.ReadLeUInt16();
			e_cpImage = rdr.ReadLeUInt16();
			this.e_cRelocations = rdr.ReadLeUInt16();
			e_cparHeader = rdr.ReadLeUInt16();
			e_minalloc = rdr.ReadLeUInt16();
			e_maxalloc = rdr.ReadLeUInt16();
			e_ss = rdr.ReadLeUInt16();
			e_sp = rdr.ReadLeUInt16();
			e_csum = rdr.ReadLeUInt16();
			e_ip = rdr.ReadLeUInt16();
			e_cs = rdr.ReadLeUInt16();
			e_lfaRelocations = rdr.ReadLeUInt16();
			e_ovno = rdr.ReadLeUInt16();
		}

        public override RelocationResults Relocate(Program program, Address addrLoad)
		{
			return GetDeferredLoader().Relocate(program, addrLoad);
		}

		void RelocateNewExe(object neHeader)
		{
		}
	}
}
/*
Values for the executable types understood by various environments:
 MZ	old-style DOS executable (see #01594)
 ZM	used by some very early DOS linkers, and still supported as an
      alternate to the MZ signature by MS-DOS, PC DOS, PTS-DOS, and S/DOS
 NE	Windows or OS/2 1.x segmented ("new") executable (see #01596)
 LE	Windows virtual device driver (VxD) linear executable (see #01609)
 LX	variant of LE used in OS/2 2.x (see #01609)
 W3	Windows WIN386.EXE file; a collection of LE files
 W4	Windows95 VMM32.VXD file
 PE	Win32 (Windows NT and Win32s) portable executable based on Unix COFF
 DL	HP 100LX/200LX system manager compliant executable (.EXM)
 MP	old PharLap .EXP (see #01619)
 P2	PharLap 286 .EXP (see #01620)
 P3	PharLap 386 .EXP (see #01620)

Format of .EXE file header:
Offset	Size	Description	(Table 01594)
 00h  2 BYTEs	.EXE signature, either "MZ" or "ZM" (5A4Dh or 4D5Ah)
          (see also #01593)
 02h	WORD	number of bytes in last 512-byte page of executable
 04h	WORD	total number of 512-byte pages in executable (includes any
        partial last page)
 06h	WORD	number of relocation entries
 08h	WORD	header size in paragraphs
 0Ah	WORD	minimum paragraphs of memory required to allocate in addition
          to executable's size
 0Ch	WORD	maximum paragraphs to allocate in addition to executable's size
 0Eh	WORD	initial SS relative to start of executable
 10h	WORD	initial SP
 12h	WORD	checksum (one's complement of sum of all words in executable)
 14h	DWORD	initial CS:IP relative to start of executable
 18h	WORD	offset within header of relocation table
        40h or greater for new-format (NE,LE,LX,W3,PE,etc.) executable
 1Ah	WORD	overlay number (normally 0000h = main program)
---new executable---
 1Ch  4 BYTEs	???
 20h	WORD	behavior bits
 22h 26 BYTEs	reserved for additional behavior info
 3Ch	DWORD	offset of new executable (NE,LE,etc) header within disk file,
        or 00000000h if plain MZ executable
---Borland TLINK---
 1Ch  2 BYTEs	??? (apparently always 01h 00h)
 1Eh	BYTE	signature FBh
 1Fh	BYTE	TLINK version (major in high nybble, minor in low nybble)
 20h  2 BYTEs	??? (v2.0 apparently always 72h 6Ah, v3.0+ seems always 6Ah 72h)
---ARJ self-extracting archive---
 1Ch  4 BYTEs	signature "RJSX" (older versions, new signature is "aRJsfX" in
        the first 1000 bytes of the file)
---LZEXE 0.90 compressed executable---
 1Ch  4 BYTEs	signature "LZ09"
---LZEXE 0.91 compressed executable---
 1Ch  4 BYTEs	signature "LZ91"
---PKLITE compressed executable---
 1Ch	BYTE	minor version number
 1Dh	BYTE	bits 0-3: major version
        bit 4: extra compression
        bit 5: huge (multi-segment) file
 1Eh  6 BYTEs	signature "PKLITE" (followed by copyright message)
---LHarc 1.x self-extracting archive---
 1Ch  4 BYTEs	unused???
 20h  3 BYTEs	jump to start of extraction code
 23h  2 BYTEs	???
 25h 12 BYTEs	signature "LHarc's SFX "
---LHA 2.x self-extracting archive---
 1Ch  8 BYTEs	???
 24h 10 BYTEs	signature "LHa's SFX " (v2.10) or "LHA's SFX " (v2.13)
---TopSpeed C 3.0 CRUNCH compressed file---
 1Ch	DWORD	018A0001h
 20h	WORD	1565h
---PKARCK 3.5 self-extracting archive---
 1Ch	DWORD	00020001h
 20h	WORD	0700h
---BSA (Soviet archiver) self-extracting archive---
 1Ch	WORD	000Fh
 1Eh	BYTE	A7h
---LARC self-extracting archive---
 1Ch  4 BYTEs	???
 20h 11 BYTEs	"SFX by LARC "
---LH self-extracting archive---
 1Ch  8 BYTEs	???
 24h  8 BYTEs	"LH's SFX "
---RAR self-extracting archive---
 1Ch  4 BYTEs	signature "RSFX"
---other linkers---
 1Ch	var	optional information
---
  N   N DWORDs	relocation items
        each is the segment:offset from start of load image at which
          to add the actual load segment to the indicated WORD
Notes:	if the word at offset 02h is 4, it should be treated as 00h, since
      pre-1.10 versions of the MS linker set it that way
    if both minimum and maximum allocation (offset 0Ah/0Ch) are zero, the
      program is loaded as high in memory as possible (DOS only checks
      the maximum allocation, however)
    the maximum allocation is set to FFFFh by default
    additional data may be contained in the file beyond the end of the
      load image described by the .EXE header; this data may be overlays,
      the actual executable for newer-format executables, or debugging
      information (see #01600,#01624)
    relocations entries need not be in any particular order, although they
      are typically stored in order from beginning to end of the load
      image
SeeAlso: #01596

Format of ROM Module Header:
Offset	Size	Description	(Table 01595)
 00h  2 BYTEs	ROM signature 55h, AAh
 02h	BYTE	size of ROM in 512-byte blocks
 03h  3 BYTEs	POST initialization entry point (near JMP instruction)
 06h	ROM Program Name List [array]
    Offset	Size	Description
     00h	BYTE	length of ROM program's name (00h if end of name list)
     01h  N BYTEs	program name
     N+1  3 BYTEs	program entry point (near JMP instruction)

Format of new executable header:
Offset	Size	Description	(Table 01596)
 00h  2 BYTEs	"NE" (4Eh 45h) signature
 02h  2 BYTEs	linker version (major, then minor)
 04h	WORD	offset from start of this header to entry table (see #01603)
 06h	WORD	length of entry table in bytes
 08h	DWORD	file load CRC (0 in Borland's TPW)
 0Ch	BYTE	program flags (see #01597)
 0Dh	BYTE	application flags (see #01598)
 0Eh	WORD	auto data segment index
 10h	WORD	initial local heap size
 12h	WORD	initial stack size (added to data seg, 0000h if SS <> DS)
 14h	DWORD	program entry point (CS:IP), "CS" is index into segment table
 18h	DWORD	initial stack pointer (SS:SP), "SS" is segment index
        if SS=automatic data segment and SP=0000h, the stack pointer is
          set to the top of the automatic data segment, just below the
          local heap
 1Ch	WORD	segment count
 1Eh	WORD	module reference count
 20h	WORD	length of nonresident names table in bytes
 22h	WORD	offset from start of this header to segment table (see #01601)
 24h	WORD	offset from start of this header to resource table
 26h	WORD	offset from start of this header to resident names table
 28h	WORD	offset from start of this header to module reference table
 2Ah	WORD	offset from start of this header to imported names table
        (array of counted strings, terminated with a string of length
          00h)
 2Ch	DWORD	offset from start of file to nonresident names table
 30h	WORD	count of moveable entry point listed in entry table
 32h	WORD	file alignment size shift count
        0 is equivalent to 9 (default 512-byte pages)
 34h	WORD	number of resource table entries
 36h	BYTE	target operating system
        00h unknown
        01h OS/2
        02h Windows
        03h European MS-DOS 4.x
        04h Windows 386
        05h BOSS (Borland Operating System Services)
        81h PharLap 286|DOS-Extender, OS/2
        82h PharLap 286|DOS-Extender, Windows
 37h	BYTE	other EXE flags (see #01599)
 38h	WORD	offset to return thunks or start of gangload area
 3Ah	WORD	offset to segment reference thunks or length of gangload area
 3Ch	WORD	minimum code swap area size
 3Eh  2 BYTEs	expected Windows version (minor version first)
Note:	this header is documented in detail in the Windows 3.1 SDK Programmer's
      Reference, Vol 4.
SeeAlso: #01594

Bitfields for new executable program flags:
Bit(s)	Description	(Table 01597)
 0-1	DGROUP type
      0 = none
      1 = single shared
      2 = multiple (unshared)
      3 = (null)
 2	global initialization
 3	protected mode only
 4	8086 instructions
 5	80286 instructions
 6	80386 instructions
 7	80x87 instructions

Bitfields for new executable application flags:
Bit(s)	Description	(Table 01598)
 0-2	application type
    001 full screen (not aware of Windows/P.M. API)
    010 compatible with Windows/P.M. API
    011 uses Windows/P.M. API
 3	is a Family Application (OS/2)
 5	0=executable, 1=errors in image
 6	non-conforming program (valid stack is not maintained)
 7	DLL or driver rather than application
    (SS:SP info invalid, CS:IP points at FAR init routine called with
      AX=module handle which returns AX=0000h on failure, AX nonzero on
      successful initialization)

Bitfields for other new .EXE flags:
Bit(s)	Description	(Table 01599)
 0	supports long filenames
 1	2.X protected mode
 2	2.X proportional font
 3	gangload area

Format of Codeview trailer (at end of executable):
Offset	Size	Description	(Table 01600)
 00h	WORD	signature 4E42h ('NB')
 02h	WORD	Microsoft debug info version number
 04h	DWORD	Codeview header offset
SeeAlso: #01624

Format of new executable segment table record:
Offset	Size	Description	(Table 01601)
 00h	WORD	offset in file (shift left by alignment shift to get byte offs)
 02h	WORD	length of image in file (0000h = 64K)
 04h	WORD	segment attributes (see #01602)
 06h	WORD	number of bytes to allocate for segment (0000h = 64K)
Note:	the first segment table entry is entry number 1
SeeAlso: #01604

Bitfields for segment attributes:
Bit(s)	Description	(Table 01602)
 0	data segment rather than code segment
 1	unused???
 2	real mode
 3	iterated
 4	movable
 5	sharable
 6	preloaded rather than demand-loaded
 7	execute-only (code) or read-only (data)
 8	relocations (directly following code for this segment)
 9	debug info present
 10,11	80286 DPL bits
 12	discardable
 13-15	discard priority

Format of new executable entry table item (list):
Offset	Size	Description	(Table 01603)
 00h	BYTE	number of entry points (00h if end of entry table list)
 01h	BYTE	segment number (00h if end of entry table list)
 02h 3N BYTEs	entry records
        Offset	Size	Description
         00h	BYTE	flags
                bit 0: exported
                bit 1: single data
                bits 2-7: unused???
         01h	WORD	offset within segment

Format of new executable relocation data (immediately follows segment image):
Offset	Size	Description	(Table 01604)
 00h	WORD	number of relocation items
 02h 8N BYTEs	relocation items
        Offset	Size	Description
         00h	BYTE	relocation type
                00h LOBYTE
                02h BASE
                03h PTR
                05h OFFS
                0Bh PTR48
                0Dh OFFS32
         01h	BYTE	flags
                bit 2: additive
         02h	WORD	offset within segment
         04h	WORD	target address segment
         06h	WORD	target address offset
SeeAlso: #01601,#01605

Format of new executable resource data:
Offset	Size	Description	(Table 01605)
 00h	WORD	alignment shift count for resource data
 02h  N RECORDs resources
    Format of resource record:
    Offset	Size	Description
     00h	WORD	type ID
            0000h if end of resource records
            >= 8000h if integer type
            else offset from start of resource table to type string
     02h	WORD	number of resources of this type
     04h	DWORD	reserved for runtime use
     08h  N Resources (see #01606)
Note:	resource type and name strings are stored immediately following the
      resource table, and are not null-terminated
SeeAlso: #01606

Format of new executable resource entry:
Offset	Size	Description	(Table 01606)
 00h	WORD	offset in alignment units from start of file to contents of
        the resource data
 02h	WORD	length of resource image in bytes
 04h	WORD	flags
        bit 4: moveable
        bit 5: shareable
        bit 6: preloaded
 06h	WORD	resource ID
        >= 8000h if integer resource
        else offset from start of resource table to resource string
 08h	DWORD	reserved for runtime use
Notes:	resource type and name strings are stored immediately following the
      resource table, and are not null-terminated
    strings are counted strings, with a string of length 0 indicating the
      end of the resource table
SeeAlso: #01605,#01607

Format of new executable module reference table [one bundle of entries]:
Offset	Size	Description	(Table 01607)
 00h	BYTE	number of records in this bundle (00h if end of table)
 01h	BYTE	segment indicator
        00h unused
        FFh movable segment, segment number is in entry
        else segment number of fixed segment
 02h  N RECORDs
    Format of segment record
    Offset	Size	Description
     00h	BYTE	flags
            bit 0: entry is exported
            bit 1: entry uses global (shared) data
            bits 7-3: number of parameter words
    ---fixed segment---
     01h	WORD	offset
    ---moveable segment---
     01h  2 BYTEs	INT 3F instruction (CDh 3Fh)
     03h	BYTE	segment number
     05h	WORD	offset
Note:	table entries are numbered starting from 1
SeeAlso: #01608

Format of new executable resident/nonresident name table entry:
Offset	Size	Description	(Table 01608)
 00h	BYTE	length of string (00h if end of table)
 01h  N BYTEs	ASCII text of string
 N+1	WORD	ordinal number (index into entry table)
Notes:	the first string in the resident name table is the module name; the
      first entry in the nonresident name table is the module description
    the strings are case-sensitive; if the executable was linked with
      /IGNORECASE, all strings are in uppercase
SeeAlso: #01607

Format of Linear Executable (enhanced mode executable) header:
Offset	Size	Description	(Table 01609)
 00h  2 BYTEs	"LE" (4Ch 45h) signature (Windows)
        "LX" (4Ch 58h) signature (OS/2)
 02h	BYTE	byte order (00h = little-endian, nonzero = big-endian)
 03h	BYTE	word order (00h = little-endian, nonzero = big-endian)
 04h	DWORD	executable format level
 08h	WORD	CPU type (see also INT 15/AH=C9h)
        01h Intel 80286 or upwardly compatible
        02h Intel 80386 or upwardly compatible
        03h Intel 80486 or upwardly compatible
        04h Intel Pentium (80586) or upwardly compatible
        20h Intel i860 (N10) or compatible
        21h Intel "N11" or compatible
        40h MIPS Mark I (R2000, R3000) or compatible
        41h MIPS Mark II (R6000) or compatible
        42h MIPS Mark III (R4000) or compatible
 0Ah	WORD	target operating system
        01h OS/2
        02h Windows
        03h European DOS 4.0
        04h Windows 386
 0Ch	DWORD	module version
 10h	DWORD	module type (see #01610)
 14h	DWORD	number of memory pages
 18h	Initial CS:EIP
    DWORD	object number
    DWORD	offset
 20h	Initial SS:ESP
    DWORD	object number
    DWORD	offset
 28h	DWORD	memory page size
 2Ch	DWORD	(Windows LE) bytes on last page
        (OS/2 LX) page offset shift count
 30h	DWORD	fixup section size
 34h	DWORD	fixup section checksum
 38h	DWORD	loader section size
 3Ch	DWORD	loader section checksum
 40h	DWORD	offset of object table (see #01611)
 44h	DWORD	object table entries
 48h	DWORD	object page map table offset (see #01613)
 4Ch	DWORD	object iterate data map offset
 50h	DWORD	resource table offset
 54h	DWORD	resource table entries
 58h	DWORD	resident names table offset (see #01614)
 5Ch	DWORD	entry table offset (see #01615,#01616)
 60h	DWORD	module directives table offset
 64h	DWORD	Module Directives entries
 68h	DWORD	Fixup page table offset
 6Ch	DWORD	Fixup record table offset (see #01618)
 70h	DWORD	imported modules name table offset
 74h	DWORD	imported modules count
 78h	DWORD	imported procedures name table offset
 7Ch	DWORD	per-page checksum table offset
 80h	DWORD	data pages offset
 84h	DWORD	preload page count
 88h	DWORD	non-resident names table offset
 8Ch	DWORD	non-resident names table length
 90h	DWORD	non-resident names checksum
 94h	DWORD	automatic data object
 98h	DWORD	debug information offset
 9Ch	DWORD	debug information length
 A0h	DWORD	preload instance pages number
 A4h	DWORD	demand instance pages number
 A8h	DWORD	extra heap allocation
 ACh 12 BYTEs	reserved
 B8h	DWORD	offset of VERSIONINFO resource (MS-Windows VxD only)
 BCh	DWORD	pointer to ??? (dynamically-loadable VxDs only???)
 C0h	WORD	device ID (MS-Windows VxD only)
 C2h	WORD	DDK version (MS-Windows VxD only)
Note:	used by EMM386.EXE, QEMM, and Windows 3.0 Enhanced Mode drivers

Bitfields for Linear Executable module type:
Bit(s)	Description	(Table 01610)
 2	initialization (only for DLLs) 0 = global, 1 = per-process
 4	no internal fixups in executable image
 5	no external fixups in executable image
 8-10	API compatibility
    0 = unknown
    1 = incompatible with PM windowing \
    2 = compatible with PM windowing    > (only for
    3 = uses PM windowing API	   /	programs)
 13	module not loadable (only for programs)
 15-17	module type
    000 program
    001 library (DLL)
    011 protected memory library module
    100 physical device driver
    110 virtual device driver
 30	per-process library termination
    (requires valid CS:EIP, can't be set for .EXE)

Format of object table entry:
Offset	Size	Description	(Table 01611)
 00h	DWORD	virtual size in bytes
 04h	DWORD	relocation base address
 08h	DWORD	object flags (see #01612)
 0Ch	DWORD	page map index
 10h	DWORD	page map entries (see #01613)
 14h  4 BYTEs	reserved??? (apparently always zeros)

Bitfields for object flags:
Bit(s)	Description	(Table 01612)
 0	readable
 1	writable
 2	executable
 3	resource
 4	discardable
 5	shared
 6	preloaded
 7	invalid
 8-9	type
    00 normal
    01 zero-filled
    10 resident
    11 resident and contiguous
 10	resident and long-lockable
 11	reserved
 12	16:16 alias required
 13	"BIG" (Huge: 32-bit)
 14	conforming
 15	"OBJECT_I/O_PRIVILEGE_LEVEL"
 16-31	reserved

Format of object page map table entry:
Offset	Size	Description	(Table 01613)
 00h	BYTE	??? (usually 00h)
 01h	WORD	(big-endian) index to fixup table
        0000h if no relocation info
 03h	BYTE	type (00h hard copy in file, 03h some relocation needed)

Format of resident names table entry:
Offset	Size	Description	(Table 01614)
 00h	BYTE	length of name
 01h  N BYTEs	name
 N+1  3 BYTEs	???

Format of LE linear executable entry table:
Offset	Size	Description	(Table 01615)
 00h	BYTE	number of entries in table
 01h 10 BYTEs per entry
        Offset	Size	Description
         00h	BYTE	bit flags
                bit 0: non-empty bundle
                bit 1: 32-bit entry
         01h	WORD	object number
         03h	BYTE	entry type flags
                bit 0: exported
                bit 1: uses single data rather than instance
                bit 2: reserved
                bits 3-7: number of stack parameters
         04h	DWORD	offset of entry point
         08h  2 BYTEs	???
Note:	empty bundles (bit flags at 00h = 00h) are used to skip unused indices,
      and do not contain the remaining nine bytes

Format of LX linear executable entry table [array]:
Offset	Size	Description	(Table 01616)
 00h	BYTE	number of bundles following (00h = end of entry table)
 01h	BYTE	bundle type
        00h empty
        01h 16-bit entry
        02h 286 callgate entry
        03h 32-bit entry
        04h forwarder entry
        bit 7 set if additional parameter typing information is present
---bundle type 00h---
 no additional fields
---bundle type 01h---
 02h	WORD	object number
 04h	BYTE	entry flags
        bit 0: exported
        bits 7-3: number of stack parameters
 05h	WORD	offset of entry point in object (shifted by page size shift)
---bundle type 02h---
 02h	WORD	object number
 04h	BYTE	entry flags
        bit 0: exported
        bits 7-3: number of stack parameters
 05h	WORD	offset of entry point in object
 07h	WORD	reserved for callgate selector (used by loader)
---bundle type 03h---
 02h	WORD	object number
 04h	BYTE	entry flags
        bit 0: exported
        bits 7-3: number of stack parameters
 05h	DWORD	offset of entry point in object
---bundle type 04h---
 02h	WORD	reserved
 04h	BYTE	forwarder flags
        bit 0: import by ordinal
        bits 7-1 reserved
 05h	WORD	module ordinal
        (forwarder's index into Import Module Name table)
 07h	DWORD	procedure name offset or import ordinal number
Note:	all fields after the first two bytes are repeated N times

Bitfields for linear executable fixup type:
Bit(s)	Description	(Table 01617)
 7	ordinal is BYTE rather than WORD
 6	16-rather than 8-object number/module ordinal
 5	addition with DWORD rather than WORD
 4	relocation info has size with new two bytes at end
 3	reserved (0)
 2	set if add to destination, clear to replace destination
 1-0	type
    00 internal fixup
    01 external fixup, imported by ordinal
    10 external fixup, imported by name
    11 internal fixup via entry table

Format of linear executable fixup record:
Offset	Size	Description	(Table 01618)
 00h	BYTE	type
        bits 7-4: modifier (0001 single, 0011 multiple)
        bits 3-0: type
            0000 byte offset
            0010 word segment
            0011 16-bit far pointer (DWORD)
            0101 16-bit offset
            0110 32-bit far pointer (PWORD)
            0111 32-bit offset
            1000 near call or jump, WORD/DWORD based on seg attrib
 01h	BYTE	linear executable fixup type (see #01617)
---if single type---
 02h	WORD	offset within page
 04h	relocation information
    ---internal fixup---
    BYTE	object number
    ---external,ordinal---
    BYTE	one-based module number in Import Module table
    BYTE/WORD ordinal number
    WORD/DWORD value to add (only present if modifier bit 4 set)
    ---external,name---
    BYTE	one-based module number in Import Module table
    WORD	offset in Import Procedure names
    WORD/DWORD value to add (only present if modifier bit 4 set)
---if multiple type---
 02h	BYTE	number of items
 03h	var	relocation info as for "single" type (above)
      N WORDs	offsets of items to relocate

Format of old Phar Lap .EXP file header:
Offset	Size	Description	(Table 01619)
 00h  2 BYTEs	"MP" (4Dh 50h) signature
 02h	WORD	remainder of image size / page size (page size = 512h)
 04h	WORD	size of image in pages
 06h	WORD	number of relocation items
 08h	WORD	header size in paragraphs
 0Ah	WORD	minimum number of extra 4K pages to be allocated at the end
          of program, when it is loaded
 0Ch	WORD	maximum number of extra 4K pages to be allocated at the end
          of program, when it is loaded
 0Eh	DWORD	initial ESP
 12h	WORD	word checksum of file
 14h	DWORD	initial EIP
 18h	WORD	offset of first relocation item
 1Ah	WORD	overlay number
 1Ch	WORD	??? (wants to be 1)
SeeAlso: #01620

Format of new Phar Lap .EXP file header:
Offset	Size	Description	(Table 01620)
 00h  2 BYTEs	signature ("P2" for 286 .EXP executable, "P3" for 386 .EXP)
 02h	WORD	level (01h flat-model file, 02h multisegmented file)
 04h	WORD	header size
 06h	DWORD	file size in bytes
 0Ah	WORD	checksum
 0Ch	DWORD	offset of run-time parameters within file (see #01622)
 10h	DWORD	size of run-time parameters in bytes
 14h	DWORD	offset of relocation table within file
 18h	DWORD	size of relocation table in bytes
 1Ch	DWORD	offset of segment information table within file (see #01621)
 20h	DWORD	size of segment information table in bytes
 24h	WORD	size of segment information table entry in bytes
 26h	DWORD	offset of load image within file
 2Ah	DWORD	size of load image on disk
 2Eh	DWORD	offset of symbol table within file or 00000000h
 32h	DWORD	size of symbol table in bytes
 36h	DWORD	offset of GDT within load image
 3Ah	DWORD	size of GDT in bytes
 3Eh	DWORD	offset of LDT within load image
 42h	DWORD	size of LDT in bytes
 46h	DWORD	offset of IDT within load image
 4Ah	DWORD	size of IDT in bytes
 4Eh	DWORD	offset of TSS within load image
 52h	DWORD	size of TSS in bytes
 56h	DWORD	minimum number of extra bytes to be allocated at end of program
        (level 1 executables only)
 5Ah	DWORD	maximum number of extra bytes to be allocated at end of program
        (level 1 executables only)
 5Eh	DWORD	base load offset (level 1 executables only)
 62h	DWORD	initial ESP
 66h	WORD	initial SS
 68h	DWORD	initial EIP
 6Ch	WORD	initial CS
 6Eh	WORD	initial LDT
 70h	WORD	initial TSS
 72h	WORD	flags
        bit 0: load image is packed
        bit 1: 32-bit checksum is present
        bits 4-2: type of relocation table
 74h	DWORD	memory requirements for load image
 78h	DWORD	32-bit checksum (optional)
 7Ch	DWORD	size of stack segment in bytes
 80h 256 BYTEs	reserved (0)
SeeAlso: #01619,#01623

Format of Phar Lap segment information table entry:
Offset	Size	Description	(Table 01621)
 00h	WORD	selector number
 02h	WORD	flags
 04h	DWORD	base offset of selector
 08h	DWORD	minimum number of extra bytes to be allocated to the segment

Format of 386|DOS-Extender run-time parameters:
Offset	Size	Description	(Table 01622)
 00h  2 BYTEs	signature "DX" (44h 58h)
 02h	WORD	minimum number of real-mode params to leave free at run time
 04h	WORD	maximum number of real-mode params to leave free at run time
 06h	WORD	minimum interrupt buffer size in KB
 08h	WORD	maximum interrupt buffer size in KB
 0Ah	WORD	number of interrupt stacks
 0Ch	WORD	size in KB of each interrupt stack
 0Eh	DWORD	offset of byte past end of real-mode code and data
 12h	WORD	size in KB of call buffers
 14h	WORD	flags
        bit 0: file is virtual memory manager
        bit 1: file is a debugger
 16h	WORD	unprivileged flag (if nonzero, executes at ring 1, 2, or 3)
 18h 104 BYTEs	reserved (0)

Format of Phar Lap repeat block header:
Offset	Size	Description	(Table 01623)
 00h	WORD	byte count
 02h	BYTE	repeat string length

Format of Borland debugging information header (following load image):
Offset	Size	Description	(Table 01624)
 00h	WORD	signature 52FBh
 02h	WORD	version ID
 04h	DWORD	size of name pool in bytes
 08h	WORD	number of names in name pool
 0Ah	WORD	number of type entries
 0Ch	WORD	number of structure members
 0Eh	WORD	number of symbols
 10h	WORD	number of global symbols
 12h	WORD	number of modules
 14h	WORD	number of locals (optional)
 16h	WORD	number of scopes in table
 18h	WORD	number of line-number entries
 1Ah	WORD	number of include files
 1Ch	WORD	number of segment records
 1Eh	WORD	number of segment/file correlations
 20h	DWORD	size of load image after removing uninitialized data and debug
          information
 24h	DWORD	debugger hook; pointer into debugged program whose meaning
          depends on program flags
 28h	BYTE	program flags
        bit 0: case-sensitive link
        bit 1: pascal overlay program
 29h	WORD	no longer used
 2Bh	WORD	size of data pool in bytes
 2Dh	BYTE	padding
 2Eh	WORD	size of following header extension (currently 00h, 10h, or 20h)
 30h	WORD	number of classes
 32h	WORD	number of parents
 34h	WORD	number of global classes (currently unused)
 36h	WORD	number of overloads (currently unused)
 38h	WORD	number of scope classes
 3Ah	WORD	number of module classes
 3Ch	WORD	number of coverage offsets
 3Eh	DWORD	offset relative to symbol base of name pool
 42h	WORD	number of browser information records
 44h	WORD	number of optimized symbol records
 46h	WORD	debugging flags
 48h  8 BYTEs	padding



*/

