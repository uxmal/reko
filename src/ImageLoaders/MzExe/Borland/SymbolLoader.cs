#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Borland
{
    public class SymbolLoader
    {
        const ushort MagicNumber = 0x52FB;

        private ExeImageLoader exeLoader;
        private byte[] rawImage;
        private debug_header_16 header;
        private long name_pool_offset;

        public SymbolLoader(ExeImageLoader exeLoader, byte[] rawImage)
        {
            this.exeLoader = exeLoader;
            this.rawImage = rawImage;
        }

        public bool LoadDebugHeader()
        {
            uint dataSize = exeLoader.e_cpImage * (uint)ExeImageLoader.CbPageSize;
            if (exeLoader.e_cbLastPage > 0)
            {
                dataSize -= ExeImageLoader.CbPageSize;
            }
            dataSize += exeLoader.e_cbLastPage;
            var rdr = new LeImageReader(rawImage, dataSize);
            var srdr = new StructureReader<debug_header_16>(rdr);
            var header = srdr.Read();
            if (!header.HasValue || header.Value.magic_number != MagicNumber)
            {
                return false;
            }
            this.header = header.Value;
            this.name_pool_offset = -1;
            if (this.header.extension_size == 0x20)
            {
                var ext = new StructureReader<header_extension_20>(rdr);
                var extHdr = ext.Read();
                if (!extHdr.HasValue)
                {
                    //$TODO: warn
                    return false;
                }
                name_pool_offset = (int)extHdr.Value.name_pool_offset;
            }
            else if (this.header.extension_size == 0)
            {
                // Haven't seen one of these.
                return false;
            }
            else
            {
                //$TODO: warn and quit.
                return false;
            }
            name_pool_offset += rdr.Offset;
            LoadSymbolTable(rdr);
               //Module Table
               //Source File Table
               //Scopes Table
               //Line Number Table
               //Segments Table
               //Correlation Table
               //Type Table
               //Members Table
               //Class Table
               //Parent Table
               //Scope Class Table
               //Module Class Table
               //Coverage Map Table
               //Coverage Offsets Table
               //Browser Definitions Table
               //Optimized Symbols Table
               //Module Optimization Flags Table
               //Reference Information Table
               //Names Table

            return true;
        }

        private void LoadSymbolTable(ImageReader rdr)
        {
            var srdr = new StructureReader<symbol_record>(rdr);
            for (;;)
            {
                var sym = srdr.Read();
                if (!sym.HasValue)
                    break;
                Debug.Print($"  Symbol:  {ReadName(sym.Value.symbol_name)} ({sym.Value.symbol_name:X4})");
                Debug.Print($"    Type:  {sym.Value.symbol_type:X4}");
                Debug.Print($"    Addr:  {sym.Value.symbol_segment:X4}:{sym.Value.symbol_offset:X4}");
                Debug.Print($"    Types: {sym.Value.flags:X2}");
            }
        }

        private string ReadName(int offset)
        {
            int iStart = (int)(name_pool_offset + offset);
            int iEnd = iStart;
            while (iEnd < rawImage.Length && rawImage[iEnd] != 0)
            {
                ++iEnd;
            }
            if (iStart == iEnd)
                return "";
            return Encoding.ASCII.GetString(rawImage, iStart, iEnd - iStart);
        }

        public IDictionary<Address,ImageSymbol> LoadSymbols()
        {
            return new SortedList<Address, ImageSymbol>();
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct debug_header_16
        {
            public ushort magic_number;    //  To be sure who we are 
            public ushort version_id;      //  In case we change things 
            public uint names;             //  Names pool size in bytes 
            public ushort  names_count;       //  Number of names in pool 
            public ushort types_count;       //  Number of type entries 
            public ushort members_count;     //  Structure members table 
            public ushort symbols_count;     //  Number of symbols 
            public ushort globals_count;     //  Number of global symbols 
            public ushort modules_count;     //  Number of modules (units)
            public ushort locals_count;      //  optional; can be filler
            public ushort scopes_count;      //  Number of scopes in table
            public ushort lines_count;       //  Number of line nos 
            public ushort source_count;      //  Number of include files 
            public ushort segment_count;     //  number of segment records
            public ushort correlation_count; //  number of segment/file 
                                           //  correlations 
            public uint image_size;        //  The number of bytes in 
                                           //  the .EXE file if the 
                                           //  uninitialized part of 
                                           //  the data, plus this 
                                           //  debug info were removed. 
            public uint /*void far * */debugger_hook;     //  A far ptr into debugged 
                                                          //  program, meaning depends 
                                                          //  on program flags. For pascal 
                                                          //  overlays, is ptr to start of 
                                                          //  data area that contains info 
                                                          //  contains about the overlays. 
            public byte program_flags;     //  A byte of flags 
                                           //  0x01 = Case sensitive link 
                                           //  0x00 = Case insensitive link 
                                           //  0x02 = pascal overlay program
            public ushort stringsegoffset; //  No longer used 
            public ushort data_count;      //  size in bytes of data pool   
            public byte filler;            //  to force alignment 
            public ushort extension_size;  //  0, or 16, for now 
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        [Endian(Endianness.LittleEndian)]
        public struct header_extension_20
        {
            public ushort class_count;
            public ushort parent_count;
            public ushort global_classes_count; // unused
            public ushort overloads_count; // unues
            public ushort scope_class_count; 
            public ushort module_class_count;
            public ushort coverage_offset_count;
            public uint name_pool_offset;       // relative to start of symbol table
            public ushort browser_information_record_count;
            public ushort optimized_symbol_record_count;
            public ushort debugging_flags;
            public uint pad1;
            public uint pad2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [Endian(Endianness.LittleEndian)]
        public struct symbol_record
        {
            public ushort symbol_name;
            public ushort symbol_type;
            public ushort symbol_offset;
            public ushort symbol_segment;
            public byte flags;
            //ushort symbol_class : 3;
            //ushort has_valid_BP : 1;
            //ushort return_address_word_offset : 3;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [Endian(Endianness.LittleEndian)]
        public struct module_header
        {
            public ushort module_name;
            public byte language;
            public byte flags;
            //    memory_model : 3;
            //unsigned short underbars_on : 1;
            public ushort symbols_index;
            public ushort symbols_count;
            public ushort source_files_index;
            public ushort source_files_count;
            public ushort correlation_index;
            public ushort correlation_count;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [Endian(Endianness.LittleEndian)]
        struct source_file
        {
            public ushort source_file_name;
            public uint time_stamp;
        }

        /*
               The symbol table consists of a series of symbol
               definitions, sorted into ascending address order,
               with constant symbols (symbol_class == 5) at the
               end of each section (global or module local).

               Note also that globals are all static, absolute,
               or typedefs.

               No register globals are generated by Borland compilers at this time.

               symbol_name is the index of the symbol name.

               symbol_type is the index of the symbol type.

               symbol_offset is interpreted according to the
               symbol_class field.

               symbol_segment is the segment part of the symbol
               address for static symbols.
                
               For new .EXE files, the top two bits of
               symbol_segment are used to provide information
               about symbols in DLLs as follows: If
               SR_SS_DllEntry bit is non-zero, then
               SR_SS_OrdinalFlag determines whether or not the
               SR_SS_Ordinal field of symbol_segment is an
               ordinal value or not.

               For DLLs, symbol_offset is the name index of the
               module and symbol_name is name index of the DLL's
               entry point.

               symbol_class is one of the following:
               _________________________________________________

                 Value    Symbol class

                 0x0      Static, offset and segment give the
                          address.

                 0x1      Absolute symbol. The segment and
                          offset is the absolute address of the
                          symbol.

                 0x2      Auto, offset is treated as signed,
                          relative to BP.

                 0x3      Pascal var parameter. The offset is BP
                          relative and is the location of the
                          far pointer to the parameter.

                 0x4      Register. Offset is a register ID as
                          follows:

                     0x00  AX       0x0A  DL      0x14  FS      0x20  ST(0)
                     0x01  CX       0x0B  BL      0x15  GS      0x21  ST(1)
                     0x02  DX       0x0C  AH      0x18  EAX     0x22  ST(2)
                     0x03  BX       0x0D  CH      0x19  ECX     0x23  ST(3)
                     0x04  SP       0x0E  DH      0x1A  EDX     0x24  ST(4)
                     0x05  BP       0x0F  BH      0x1B  EBX     0x25  ST(5)
                     0x06  SI       0x10  ES      0x1C  ESP     0x26  ST(6)
                     0x07  DI       0x11  CS      0x1D  EBP     0x27  ST(7)
                     0x08  AL       0x12  SS      0x1E  ESI
                     0x09  CL       0x13  DS      0x1F  EDI

                 0x5      Constant. Up to 4-byte constant stored
                          in offset/segment.

                 0x6      Typedef. The offset field is ignored.

                 0x7      Structure/Union/Enum Tag. The offset
                          is a type index.
			______________________

$define SC_STATIC   0x0
$define SC_ABSOLUTE 0x1
$define SC_AUTO     0x2
$define SC_PASVAR   0x3
$define SC_REGISTER 0x4

$define SC_CONST    0x5
$define SC_TYPEDEF  0x6
$define SC_TAG      0x7

$define SR_SS_DllEntry     0x8000  //  symbol is a dll entry   
$define SR_SS_OrdinalFlag  0x4000  //  segment is ordinal value 
$define SR_SS_Ordinal      0x3fff  //  mask to obtain ordinal value 


               The has_valid_BP field is defined for functions
               only. If the bit is zero, the function does not
               set up a BP stack frame, if the value is one then
               a valid BP is set up.

               The return_address_word_offset field contains the
               offset in words from BP where the return address
               can be found if the has_valid_BP field is not
               zero. The size of the return address is
               determined from the function type.
         */
#if DOCS
               TLINK's debugging output is written at the end of
               the load image in the .EXE file. An image that
               does not include extra information beyond the
               image size has no debug information. If extra
               data is written beyond the load image, check the
               first word for the number 0x52fb.

               The debug information begins with a header
               describing the sizes of the remaining tables.
               This header is defined as follows:

                 struct  debug_header
                 {
                   ushort  magic_number;   //  To be sure who we are 
                   ushort  version_id;     //  In case we change things 
                   uint   names;          //  Names pool size in bytes 
                   uint   names_count;    //  Number of names in pool 
                   uint   types_count;    //  Number of type entries 
                   uint   members_count;  //  Structure members table 
                   uint   symbols_count;  //  Number of symbols 
                   uint   globals_count;  //  Number of global symbols 
                   uint   modules_count;  //  Number of modules (units)
                   uint   locals_count;   //  optional; can be filler
                   uint   scopes_count;   //  Number of scopes in table
                   uint   lines_count;    //  Number of line nos 
                   uint   source_count;   //  Number of include files 
                   uint   segment_count;  //  number of segment records
                   uint   correlation_count;//  number of segment/file 
                                                   //  correlations 
                   uint   image_size;     //  The number of bytes in 
                                                   //  the .EXE file if the 
                                                   //  uninitialized part of 
                                                   //  the data, plus this 
                                                   //  debug info were removed. 
                   void    far *debugger_hook;     //  A far ptr into debugged 
                                                   //  program, meaning depends 
                                                   //  on program flags. For pascal 
                                                   //  overlays, is ptr to start of 
                                                   //  data area that contains info 
                                                   //  contains about the overlays. 
                   unsigned char   program_flags;  //  A byte of flags 
                                                   //  0x01 = Case sensitive link 
                                                   //  0x00 = Case insensitive link 
                                                   //  0x02 = pascal overlay program
                   unsigned stringsegoffset;       //  No longer used 
                   ushort data_count;      //  size in bytes of data pool   
                   unsigned char filler;           //  to force alignment 
                   ushort extension_size;  //  0, or 16, for now 
                 };

                 struct header_extension
                 {
                   uint class_entries;         //  number of classes 
                   uint parent_entries;        //  number of parents 
                   uint global_classes;        //  number of global classes 
                                                        //  - NOT USED 
                   uint scope_class_entries;   //  number of scope classes 
                   uint module_class_entries;  //  number of module classes
                   uint CoverageOffsetCount;   //  number of coverage offsets
                   uint NamePoolOffset;        //  offset to start of name 
                                                        //  pool. This is relative 
                                                        //  to the symbols base 
                   uint BrowserEntries;     //  number of browser info recs 
                   uint OptSymEntries;      //  number of opt symbol recs 
                   unsigned int DebugFlags;          //  various flags  
                   uint refInfoSize;        //  size in bytes of ref 
                                                     //  info section 
                   char    filler [14];              //  padding 
                 };

                 typedef struct      //  Trailer at end of NEW EXE with debug info 
                 {
                   ushort Signature;       //  'NB' 
                   ushort Version;         //  MS debug info version number 
                   uint Size;             //  Codeview header offset =     
                                                   //  (EOF - Size)                 
                 } TMSDbgTrailer;

               The layout appears in the .EXE files as follows:

               EXE header
               fixups
               EXE image
               debug header
               Symbol Table
               Module Table
               Source File Table
               Scopes Table
               Line Number Table
               Segments Table
               Correlation Table
               Type Table
               Members Table
               Class Table
               Parent Table
               Scope Class Table
               Module Class Table
               Coverage Map Table
               Coverage Offsets Table
               Browser Definitions Table
               Optimized Symbols Table
               Module Optimization Flags Table
               Reference Information Table
               Names Table

               For new .EXE files, there will be an 8-byte
               Codeview header immediately before the debug
               header, and an 8-byte Codeview trailer
               immediately after the names table. TD symbols
               tables can be told apart from Microsoft-generated
               tables by the value 0xFFFFFFFF in the last 4
               bytes of the Codeview header.

               All symbols, global or not, appear in the symbols
               area. The globals appear first, with module and
               local symbols following. The globals field
               specifies how many of the symbols are globals.

               Identifiers are stored as indexes into the names
               pool. The index is to the relative identifier
               number (starting at 1). This way 64K distinct
               identifiers of arbitrary length can be stored.
               Names are stored uniquely, so that comparing
               indexes is as good as comparing strings. An
               identifier is stored in the pool as an ASCIIZ
               string (null-terminated string).


               Symbols




               Modules

               A module (or unit) consists of a set of objects,
               source files, and correlation records.

                 struct  module_header
                 {
                   uint   module_name;
                   unsigned char   language;
                   ushort  memory_model : 3;
                   ushort  underbars_on : 1;
                   uint   symbols_index;
                   ushort  symbols_count;
                   ushort  source_files_index;
                   ushort  source_files_count;
                   ushort  correlation_index;
                   ushort  correlation_count;
                 };

$define MM_TINY         0x0
$define MM_SMALL        0x1
$define MM_MEDIUM       0x2
$define MM_COMPACT      0x3
$define MM_LARGE        0x4
$define MM_HUGE         0x5
$define MM_SMALL386     0x6
$define MM_MEDIUM386    0x7
$define MM_COMPACT386   0x8
$define MM_LARGE386     0x9

               module_name is the index of the module's name.
               This name is the source file name given to the
               compiler, including the extension.

               symbols_index is the index of the first symbol in
               the symbol table for the module.

               symbols_count is the number of symbols defined
               local to the module.

               source_files_index is the index of the first
               source file record for the module.

               source_files_count is the number of source files
               in the module.

               correlation_index is the index of the correlation
               record for the module.

               correlation_count is the number of correlation
               entries in the module.

               language indicates the source language for the
               module.
               _________________________________________________

                 Value Language

                 0     Unknown
                 1     C
                 2     Pascal
                 3     Basic (not used)

                 4     assembly language
                 5     C++
		______________________________________

               memory_model determines default pointer sizes in
               type conversions.

               underbars_on is non-zero if underbars should be
               prepended for cdecl-style symbols in any search
               context in this module.


               Source files

                 struct  source_file
                 {
                   uint   source_file_name;
                   uint   time_stamp;
                 };

               Each source file with line numbers in the
               executable code will have a source file record in
               the list module source files. There will always
               be at least one source file record per module
               (assuming there is any executable code in the
               module). Each include file containing code will
               generate a single source-file record per
               inclusion.

               The line numbers for a segment within a source
               file will appear as a block in the line number
               table.

               The source files in a module will appear in the
               order of their appearance in the compilation
               process. Thus the main source file appears first,
               followed by each of the include files. Note that
               if an include file doesn't have executable code
               (and therefore no source line numbers), it
               shouldn't be included here. Thus, for most source
               files with no code in include files, there will
               be only one file entry per module. Of course, if
               no executable code appears in a module, there is
               no need for a source file record.

               The source file name will include any
               subdirectory information. Thus, if Turbo Debugger
               is run in the source directory (or with the
               source directory given in the appropriate TD
               option), it should be able to find all the
               source, even if it originated from some other
               source or had some peculiar file-name extension.
               For include files, the actual path name used to
               open the file is used. This way the debugger
               doesn't duplicate the compiler's include
               directory search logic.

               The date/time stamp determines if the source file
               has changed since the time of the link.


               Line numbers

                 struct  line_number
                 {
                   ushort  line_number_value;
                   ushort  line_number_offset;
                 };

               line_number_value is the module line number.

               line_number_offset is the offset of the line
               number relative to the segment value stored in
               the segment record referred to in the active
               correlation record.

               Only unique offsets have line numbers stored.
               When a statement spans several lines, there can
               be two line records with the same offset, but
               different line numbers.

               The line number records are address sorted; they
               are not necessarily line-number ordered.


               Scopes

                 struct  scope
                 {
                   uint   autos_index;
                   ushort  autos_count;
                   ushort  parent_scope;
                   uint   function_symbol;
                   ushort  scope_offset;
                   ushort  scope_length;
                 };

               autos_index and autos_count define the symbol
               table area containing this scope's symbols. The
               auto_start is the index into the symbols table of
               the first variable local to the scope.

               parent_scope is the index of the scope within the
               current module of the immediate enclosing scope.

               scope_offset and scope_length defines the ranges
               of code addresses the scope is valid for. The
               segment is that stored in the segment record
               referred to in the active correlation record.

               To handle nested units in pascal, there is a set
               of scopes at the beginning of the scopes table
               with a function_symbol of 0xffff. There is a
               one-to-one correspondence between these and the
               module (unit) records. These are the "unit
               scopes." The symbols that the record points to
               are the interfaced symbols of the unit.

               The "uses scope" record has a function_parent of
               0xfffe to establish the correct linking between
               the unit scope records. It does not contain
               information about the scope's symbols. Instead,
               autos_index is an index to the unit scope record
               that refers to the interfaced symbols. To look up
               a name, the scopes are traced using the
               scope_parent records, but the symbols are
               accessed by referring to the corresponding unit
               scope record.


               Segments

                 typedef struct /* segment info */
                 {
                   ushort  mod_index;
                   ushort  code_segment;
                   ushort  code_offset;
                   ushort  code_length;
                   ushort  scopes_index;
                   ushort  scopes_count;
                   ushort  correlation_index;
                   ushort  correlation_count;
                 } segrec;

               A segment record gives a code segment, offset,
               and length, and relates it to a particular
               module. It also gives an index into the scopes
               table for the scopes defined in the segment. The
               correlation table index and count allow the
               segment to be related to one or more source files
               and possibly to non-continuous groups of lines
               inside the files.

               The segment records are address-ordered by
               segment and then by offset within the segment.

               mod_index is the index of the module record for
               the corresponding module.

               code_segment is the base address of the segment
               in the image.

               code_offset is the offset from the base address
               of the segment in the image.

               code_length is the length of the segment.

               scopes_index is the index of the scope record of
               the starting scope for this segment.

               scopes_count is the count of scopes for this
               segment.

               correlation_index is the index of the correlation
               record for the starting correlation for this
               segment.

               correlation_count is the number of correlation
               records for this segment.


               Segment/source file correlations

               These records link a range of line numbers in a
               file to a particular segment record.

                 typedef struct
                 {
                   ushort segment_index;
                   ushort file_index;
                   uint  lines_index;
                   ushort lines_count;
                 } correlation;

               segment_index is the index of the segment record
               for this correlation.

               file_index is the index of the source file record
               for this correlation.

               lines_index is the index of the first line number
               record for this correlation.

               lines_count is the number of line number records
               for this correlation.


               Types

               The type table consists of a set of 12-byte
               entries. Each type contains one or (for a few
               types) two entries.

               The index value is used when a type is referred
               to. Since no operations need to search the type
               table itself (all accesses will use index
               numbers), any type that occupies more than one
               entry will not have a type id byte for the upper
               half. Thus type records are effectively either 8-
               or 16-bytes long, depending on the particular
               type. Also, since only two sizes are present, a
               program can treat the table as effectively as a
               table of fixed size objects.


               Simple types and common fields
		The fields in the following table are common to all types.
               _________________________________________________

                 Field        Size          Offset
		______________

                 type_id        1              0
                 type_name      4              1
                 type_size      2              5
		________________

               type_name is 0 if the type is unnamed or is the
               name index of the type name.

               type_size is the size in bytes of the object.
               This field is present in all type records.

               type_id values are

$define TID_VOID        0x00        //  Unknown or no type   
$define TID_LSTR        0x01        //  Basic Literal string 
$define TID_DSTR        0x02        //  Basic Dynamic string 
$define TID_PSTR        0x03        //  Pascal style string  

               _________________________________________________
                
	       Pascal strings
                        (12 bytes)
			
		 Field		Size	      Offset
			______________

                 max_size       1              7
		________________

$define TID_SCHAR       0x04        //  1 byte signed range   
$define TID_SINT        0x05        //  2 byte signed range   
$define TID_SLONG       0x06        //  4 byte signed range   
$define TID_SQUAD       0x07        //  8 byte signed int     

$define TID_UCHAR       0x08        //  1 byte unsigned range 
$define TID_UINT        0x09        //  2 byte unsigned range 
$define TID_ULONG       0x0A        //  4 byte unsigned range 
$define TID_UQUAD       0x0B        //  8 byte unsigned int   

$define TID_PCHAR       0x0C        //  Pascal character type 
               _________________________________________________
               
	         Ranges (24 bytes)
                 Field        Size          Offset
			______________

                 parent type    2              8
                 lower bound    4             12
                 upper bound    4             16
		________________

$define TID_FLOAT       0x0D        //  IEEE 32-bit real     
$define TID_TPREAL      0x0E        //  Turbo Pascal 6-byte real 
$define TID_DOUBLE      0x0F        //  IEEE 64-bit real     
$define TID_LDOUBLE     0x10        //  IEEE 80-bit real     
$define TID_BCD4        0x11        //  4 byte BCD           
$define TID_BCD8        0x12        //  8 byte BCD           
$define TID_BCD10       0x13        //  10 byte BCD          

               _________________________________________________
                
		BCD COBOL
                        (12 bytes)
		 Field		Size          Offset
			___________

                 decimal point     1              5
		_____________

$define TID_BCDCOB      0x14        //  COBOL BCD            

               _________________________________________________
               
	
		Pointers (12 bytes)
                 Field           Size          Offset
			___________

                 extra info        1              7
                 pointed-to type   4              8
		_____________

$define TID_NEAR        0x15        //  Near pointer         
$define TID_FAR         0x16        //  Far pointer          
$define TID_SEG         0x17        //  Segment pointer      
$define TID_NEAR386     0x18        //  386 32-bit offset ptr
$define TID_FAR386      0x19        //  386 48-bit far ptr   

               _________________________________________________
               
	
		C arrays (12 bytes)
                 Field           Size          Offset
			___________

                 element type      4              8
		_____________

$define TID_CARRAY      0x1A        //  C array - 0 based    

               _________________________________________________
                 Very large arrays
                        (12 bytes)
		 Field		Size          Offset
			___________
                 object size       2              7
                 element type      4              9
		_____________

$define TID_VLARRAY     0x1B        //  Very Large 0 based array 


                     Pascal arrays______________________________
                        (24 bytes)
                 Field           Size          Offset
			___________
                 element type      4              8
                 dimension type    4             12
		_____________

$define TID_PARRAY      0x1C        //  Pascal array         

                Structs and unions______________________________
                        (12 bytes)
                 Field           Size          Offset
			___________
                 members index     4              8
		_____________

$define TID_ADESC       0x1D        //  Basic array descriptor 
$define TID_STRUCT      0x1E        //  Structure            
$define TID_UNION       0x1F        //  Union 


                Very large structs______________________________
                        and unions
                        (24 bytes)
		Field		Size          Offset
			___________
                 object size       2              7
                 members index     4              9
		_____________

$define TID_VLSTRUCT    0x20        //  Very Large Structure 
$define TID_VLUNION     0x21        //  Very Large Union     

               _________________________________________________
                  Enums (24 bytes)
                 Field               Size      Offset
			___________
                 lower bound           2          12
                 upper bound           2          14
                 members index         4          16
		____________

$define TID_ENUM        0x22        //  Enumerated range     


                Functions______________________________
                        (12 bytes)
                 Field               Size      Offset
			___________
                 language              0:7       7:0
                                                    *
                 accepts var. args.    0:1       7:7
                 return type           4         8
               *
                These should be read as byte:bit
		________________

$define TID_FUNCTION    0x23        //  Function or procedure


                 Labels (12 bytes)______________________________

                 Field               Size      Offset
		___________
                 near/far              1          7
		_____________

$define TID_LABEL   0x24            //  Goto label           


                Sets (12 bytes)______________________________

                 Field               Size      Offset
		___________
                 parent type           4          8
		_____________

$define TID_SET     0x25        //  Pascal set 


                Binary files______________________________
                        (12 bytes)
                 Field               Size      Offset
			___________
                 element type          4          8
		_____________

$define TID_TFILE       0x26        //  Pascal text file     
$define TID_BFILE       0x27        //  Pascal binary file   


               Function prototypes______________________________
                        (24 bytes)
                 Field               Size      Offset
			___________
                 language              0:7       7:0
                                                    *
                 accepts var. args.    0:1       7:7
                 return type           4         8
                 parameter start       2         12
               *
                These should be read as byte:bit
		________________

$define TID_BOOL        0x28        //  Pascal boolean         
$define TID_PENUM       0x29        //  Pascal enum            
$define TID_PWORD       0x2A        //  pword (6 byte 386 ptr) 
$define TID_TBYTE       0x2B        //  tbyte 
$define TID_FUNCPROTOTYPE 0x2C /* Function with
                 full parameter
                                                    information.
                 */

               The language field is as follows:
               _________________________________________________

                 Value     Description
		__________________________

                 0x0       Near C function
                 0x1       Near Pascal function
                 0x2       Unused
                 0x3       Unused
                 0x4       Far C function
                 0x5       Far Pascal function
                 0x6       Unused
                 0x7       Interrupt function
		___________________


                 Special functions______________________________
                        (24 bytes)
                 Field                Size      Offset
			__________
                 language               1          7
                 return type            4          8
                 class type             4         12
                 virtual offset         2         16
                 symbol index           4         18
                 info bits              1         22
		____________

               class type is type index of class. virtual offset
               is offset into the virtual table. symbol index is
               the symbol index of this method. info bits are
               described in the following table.
               _________________________________________________

                 Value Description
		______________________________
                 0x01  member function

                 0x02  duplicate function

                 0x04  operator function

                 0x08  internal linkage

                 0x10  Pascal function passing 'this' as last
               parameter
	________________________________________

                 //  Special function for methods and duplicate functions. 
$define TID_SPECIALFUNC   0x2D


                Classes (12 bytes)______________________________

                 Field                Size      Offset
		__________
                 class index            4          8
		____________

$define TID_CLASS        0x2E       //  Class 


               Member pointers (24______________________________
                            bytes)
                 Field                Size      Offset
			__________
                 type index             4          8
                 class index            2         11
		____________

                 /* TID's 2F , 31-32 unused */
$define TID_HANDLEPTR   0x30    //  Handle-based pointer NOT USED
$define TID_MEMBERPTR   0x33    //  Member pointer       
$define TID_NEWMEMPTR   0x38     //  New style member pointer 

               TID_MEMBERPTR____________________________________

                 Field                Size      Offset
		__________

                 type index             4          8
                 base class index       2         12
		____________

               TID_NEWMEMBERPTR
	_________________________________

                 Field                Size      Offset
		__________

                 member ptr flags       1          7
                 pointer to type index  4          8
                 base class index       2         11
		____________

               TID_HANDLEPTR
	____________________________________

                 Field                Size      Offset
		__________

                 extra info byte        1          7
                 handle string index    4          8
                 type index             4         12
		____________


              Near and far______________________________
               references
                        (24 bytes)
		Field			Size      Offset
			__________
                 type index             4          8
                 class index            4         12
		____________

$define TID_NREF        0x34        //  Near reference pointer
$define TID_FREF        0x35        //  Far reference pointer


$define TID_WORDBOOL     0x36       //  Pascal word boolean   
$define TID_LONGBOOL     0x37       //  Pascal long boolean   
$define TID_GLOBALHANDLE 0x3E       //  Windows global handle 
$define TID_LOCALHANDLE  0x3F       //  Windows local handle  

                 /* These can be used to cast a type_rec pointer
                 to the appropriate
                     subtype */

$define _t_pstr(x)      (((struct type_rec
                 *)(x))->v.pstr)
$define _t_range(x)     (((struct type_rec
                 *)(x))->v.range)
$define _t_bcd(x)       (((struct type_rec
                 *)(x))->v.bcd)
$define _t_ptr(x)       (((struct type_rec
                 *)(x))->v.ptr)
$define _t_seg(x)       (((struct type_rec
                 *)(x))->v.seg)
$define _t_carray(x)    (((struct type_rec
                 *)(x))->v.carray)
$define _t_vlarray(x)   (((struct type_rec
                 *)(x))->v.vlarray)
$define _t_parray(x)    (((struct type_rec
                 *)(x))->v.parray)
$define _t_struct(x)    (((struct type_rec
                 *)(x))->v.struc)
$define _t_vlstruct(x)  (((struct type_rec
                 *)(x))->v.vlstruct)
$define _t_enumty(x)    (((struct type_rec
                 *)(x))->v.enumty)
$define _t_function(x)  (((struct type_rec
                 *)(x))->v.function)
$define _t_set(x)       (((struct type_rec
                 *)(x))->v.set)
$define _t_bfile(x)     (((struct type_rec
                 *)(x))->v.bfile)
$define _t_label(x)     (((struct type_rec
                 *)(x))->v.label)
$define _t_specfunc(x)  (((struct type_rec
                 *)(x))->v.specfunc)
$define _t_class(x)     (((struct type_rec
                 *)(x))->v.class)
$define _t_memberptr(x) (((struct type_rec
                 *)(x))->v.memberptr)

                 struct  type_rec
                 {
                     unsigned char   type_id;    //  The TID byte.             
                     uint   type_name;  //  Any associated type name. 
                     ushort  type_size;  //  The size of any object    
                                                 //    of this type.           
                     union
                     {
                         /* For TID_VOID, TID_LSTR, TID_DSTR,
                 TID_SQUAD,
                         TID_UQUAD, TID_FLOAT, TID_PREAL,
                 TID_DOUBLE,
                         TID_LDOUBLE, TID_BCD4,  TID_BCD8,
                 TID_BCD10,
                         TID_ADESC, TID_LABEL, TID_TFILE,
                 TID_BOOL,
                         TID_PWORD, TID_TBYTE types, no
                 additional info. */

                         struct
                         {   /* only for TID_PSTR */
                             unsigned char max_size; //  Max string size 
                         }   pstr;
                                             /*^L*/
                         struct
                         {
                             /* for TID_PCHAR, TID_SCHAR,
                 TID_SINT, TID_SLONG,
                             TID_UCHAR, TID_UINT and TID_ULONG
                 types */

                             unsigned char   filler;
                             uint   parent; //  Parent type   
                             long        lower;      //  Minimum value 
                             long        upper;      //  Maximum value 
                         } range;

                         struct
                         {   /* for TID_BCDCOB only */
                             unsigned char   decimal;  //  Number of digits to      
                                                       //   right of decimal point. 
                         }   bcd;

                         struct
                         {   /* TID_LABEL only */
                             unsigned char   nearfar;    //  0 for near, 1 for far 
                         }   label;

                         struct
                         {   //  for TID_NEAR, TID_FAR, TID_NEAR386, TID_FAR386 

                             unsigned char   extra_info; //  as follows:        
                             uint   type_index; //  pointed-to type    
                         }   ptr;

                         /* For TID_NEAR and TID_NEAR386:

                         0x0 segment register unspecified.

                         0x1 ES relative
                         0x2 CS relative
                         0x3 SS relative
                         0x4 DS relative
                         0x5 FS relative
                         0x6 GS relative

                         For TID_FAR and TID_FAR386:

                         0x0 far arithmetic.
                         0x1 huge arithmetic (real mode only).
                 */

                         struct
                         {   //  For TID_SEG, TID_NREF, TID_FREF 

                             unsigned char   filler;
                             uint   type_index; //  pointed-to type 
                         }   seg;

                         struct
                         {   /* For TID_CARRAY only */

                             unsigned char   filler;
                             uint   element;    //  Element type 
                         }   carray;

                         struct
                         {   /* For TID_VLARRAY only */

                             ushort  upper_size; //  Upper 16 bits of size 
                             uint   element;    //  Element type          
                         }   vlarray;

                         struct
                         {   /* For TID_PARRAY only */

                             unsigned char   filler;
                             uint   element;    //  Element type   
                             ushort  dimension;  //  Subscript type 
                         }   parray;

                         struct
                         {   /* For TID_STRUCT and TID_UNION */
                             unsigned char   filler;
                             uint   members;    //  Index of members 
                         }   struc;

                         struct
                         {   //  For TID_VLSTRUCT and TID_VLUNION 

                             ushort  upper_size; //  Upper 16 bits of size 
                             uint   members;    //  Index of members  
                         }   vlstruct;

                         struct
                         {   /* For TID_ENUM and TID_PENUM */

                             unsigned char   filler;
                             ushort  parent;     //  type of parent   
                             unsigned char   filler1;
                             unsigned char   filler2;
                             ushort  lower;      //  Bottom of range  
                             ushort  upper;      //  Top of enum range
                             uint   members;    //  Index of members 
                         }   enumty;

                         struct
                         {   /* For TID_FUNCTION only */

                             unsigned    language : 7;
                             unsigned    is_varargs : 1; //  Accepts Var args 
                             uint  return_type;
                         }   function;

                         /*
                         The language field is as follows:

                         0x0 Near C function
                         0x1 Near Pascal function
                         0x2 Unused.
                         0x3 Unused.
                         0x4 Far C function
                         0x5 Far Pascal function
                         0x6 Unused.
                         0x7 Interrupt function
                         */

                         struct
                         {   /* For TID_FUNCPROTOTYPE only */

                             unsigned    language : 7;     //  see TID_FUNCTION 
                             unsigned    is_varargs : 1;   //  Accepts Var args 
                             uint   return_type;
                             ushort  param_start;  //  starting index    
                                                           //  in members table 
                         }   funcprototype;

                         struct
                         {   /* For TID_SET only */

                             unsigned char   filler;
                             uint   parent;     //  Parent type 
                         }   set;

                         struct
                         {   /* For TID_BFILE only */

                             unsigned char   filler;
                             ushort  element;    //  File element type
                         }   bfile;

                         struct
                         {   /* For TID_SPECIALFUNC only */

                             unsigned char   language;
                             uint   return_type;
                             uint   class_type;
                             ushort  virtual_offset; //  in bytes 
                             uint   symbol_index;
                             unsigned int    filler    :12;
                             unsigned int    info_bits :4;
                         } specfunc;

                         struct
                         {   /* For TID_CLASS only */

                             unsigned char filler;
                             ushort class_index;
                         } class;

                         struct
                         {   /* For TID_MEMBERPTR */
                             unsigned char  filler;
                             uint  type_index;
                             ushort class_index;
                         } memberptr;
                     }   v;
                 };


               Members

               The members table holds two completely distinct
               kinds of information. Structures and unions point
               into this table for their lists of members. Enums
               store their list of name/value pairs here.

               Structure and union members
		struct struct_offset_rec
		{
		   unsigned    filler      : 6;
                   unsigned    offset_rec  : 1;
                   unsigned    filler2     : 1;
                   uint   new_offset;
                 };

                 //  The new_offset is the offset for the next member. 

                 struct  member_type
                 {
                   unsigned    bit_field_size  : 6;
                   unsigned    offset_rec      : 1;
                   unsigned    end_of_structure: 1;
                   uint  member_name;
                   uint  member_type;
                 };

                 /****************************************
                 The member_name is the index of the name.
                 The member_type is the index of the type.
                 ****************************************/

                 struct  enum_list_type
                 {
                   unsigned    filler      : 7;
                   unsigned    end_of_list : 1;
                   uint   enum_name;
                   signed   short  enum_value;
                 };

               end_of_list is 1 for the last enum value in the
               list.

               enum_name is the index of the name.

               enum_value is the value of the corresponding
               name.

                 typedef union
                 {
                   struct struct_offset_rec o;
                   struct member_type       m;
                   struct enum_list_type    e;
                 }   member_rec;

               bit_field_size is only important for bit field
               members. It is the size in bits of the member.
               For non-bit field members, the bit_field_size is
               0.

               offset_rec is zero for normal members, and non-
               zero for the special struct-offset record. If
               this bit is set, the next 2 bytes of the member
               record is a word holding the new structure offset
               in bytes. This is used for Pascal variant
               records.

               end_of_structure is 1 for the last field in a
               structure. This is the sign bit, so a simple
               negative/non-negative test will determine the end
               of the structure.

               Holes in the structure (due to alignment padding)
               are represented using an unnamed bit-field member
               with a zero name index and a zero type index.

               The offsets of union members are always zero. The
               offsets of structure members are computed from
               the sequence of the members in the table. The
               members are stored in ascending offset order. For
               a nested unnamed union inside a structure or an
               unnamed structure inside a union, these will
               appear as unnamed members. The debugger unravels
               this nesting to provide functionality to support
               unnamed structure/union members.


               Class table

                 typedef struct {
                   ushort parent_index; //  index into parent table 
                   ushort parent_count;
                   uint  member_index;
                   uint  name_index;   /* tag */
                   ushort virtual_ptr;  /* Offset from
                 top of class data of
                                                  virtual ptr*/
                   unsigned char info;
                                   /* Info bits:
                                      bit 0:   Class is a
                 virtual base class
                                      bit 1:   Class is public
                                      bit 2-7: Offset of method
                 in virtual table */
                 } class;

               The class table defines the inheritance
               characteristics for each class. If a derived
               class has multiple inheritance, there will be
               multiple entries in the class table, indicating
               different parent classes. If there are several
               classes derived from the same virtual base class,
               there will be separate class table entries for
               each virtual base class, and each base class
               entry will have the same symbol index.

               The first byte of the member record for a given
               class entry indicates the size of bitfields, and
               as a set of bits to indicate member attributes.
               These bits can be OR'd together to form the
               desired attribute.
               _________________________________________________

                 Value  Member attributes
		_______________________

                 0x80   Last member
                 0x60   Static member (member_type points to
                        symbol for the member)
                 0x50   Static member function
                 0x48   Method or member function (including
                        virtual and static methods)
                 0x44   Virtual method
                 0x42   Constructor
                 0x41   Destructor
		______________________________

               For example, a virtual destructor will have a
               value of 0x4D:

                   0x48    - method bit
                 & 0x44    - virtual bit
                 & 0x41    - destructor bit
                   ----
                   0x4D

                Special cases
		If member_record == 0x40, record is a reset
               offset record.

               If member_record == 0xc0, next record is a
               bitfield (only needed when bitfield has some of
               the previous attributes. Attributes are indicated
               in this preceding record so the first byte is
               free to indicate field length in the bitfield
               record.)

               If member_record == 0x43, record is a conversion
               method.

               If member_record == 0x80 and member_name == 0 and
               member_type == 0, then the Turbo Pascal linker
               has smart linked this class away.

               Non-static, non-bitfield data members are always
               0, or 0x80 if they're the last item.

               Bit combining doesn't apply to constructors,
               destructors and conversions bits, since they are
               mutually exclusive.


               Parent table

               Each entry in the parent table has the following
               format:

                 typedef struct
                 {
                   ushort class_index;       //  index into class table 
                 } parent;

               class_index is an index into the class table. If
               the highest bit is set, this parent is a virtual
               base class.


               Scope class table

                 typedef struct
                 {
                     ushort class_index;       //  index into class table 
                     ushort class_count;       //  number of classes      
                 } scope_class;

               A scope class table finds the classes defined
               within a particular scope. If any scope class
               records are needed, there must be one record for
               each scope record. This is identical to expanding
               the current scope record to contain the following
               fields, but it maintains backward compatibility
               with the earlier table, and allows non-object
               languages to avoid the overhead of bigger scope
               records.


               Module class table

                 typedef struct                    //  local classes          
                 {
                     ushort class_index;   //  index into class table 
                     ushort class_count;   //  number of classes      
                 } module_class;

               A module class table finds the classes and
               overloads defined within a particular module. If
               any module class records are needed, there must
               be one for each module record. This is identical
               to expanding the current module record to contain
               the following fields, but it maintains backward
               compatibility with the earlier table, and allows
               non-object languages to avoid the overhead of
               bigger module records.


               Coverage offset map table

                 typedef struct
                 {
                     ushort offset;  //  index into Coverage Offset Table 
                 } TCoverageOffsetMapTableEntry;

               This table defines the starting index into the
               coverage offset table (which follows) for the
               given segment. There are as many segment entries
               as there are segments in the segment table. This
               table can be viewed as an array of
               TCoverageOffsetMapRecord entries, with the number
               of entries the same as the number of segments
               records in the segment table. Entries with an
               index of 0 indicate that lack of coverage offsets
               for the given segment. Note that the values in
               this table are not necessarily in ascending
               order.


               Coverage offset table

                 typedef struct
                 {
                     ushort offset;        //  offset into segment 
                 } TCoverageOffsetTableEntry;

               Each entry in the table corresponds to a starting
               offset for a block of code that is "atomic,"
               meaning that if you start executing at the
               beginning of the block, you are guaranteed to
               reach the end.


               Browser definition table

                 struct TDefinitionRecord
                 {
                   uint  symbol_index;  //  The index of the symbol in   
                                                 //  the Symbols table            
                   ushort file_index;    //  Which file the symbol is in  
                   ushort line_number;   //  line number in the file      
                 };


               Optimized symbol table

                 struct opt_symbol_record {
                     ushort  opt_symbol_next;
                                     //  index to next record for this symbol 
                     ushort  opt_symbol_offset;
                                     //  offset is treated as a register enum 
                                     //  See the Symbols section for details  
                     unsigned char   opt_symbol_class;
                                     //  Interpreted as for symbol_record     
                     ushort opt_symbol_code_offset_start;
                                     //  start of optimization range          
                     ushort  opt_symbol_code_offset_end;
                 };                  //  end of optimization range            

               An  has an entry in the symbols table whose type
               is SC_REGISTER (0x4), but whose register ID
               (offset) is greater than or equal to 0x28. The
               register ID (minus 0x28) is an index into the
               optimized symbols table. The  at that index is
               the first record in a linked list of records,
               linked through the opt_symbol_next field. The end
               of the list is marked by a 0 in that field. This
               record will have accurate information as to the
               true location of the variable in the
               opt_symbol_offset and opt_symbol_class fields, as
               per the symbol_record specification. Note that
               opt_symbol_class refers to the combination of the
               three symbol record bit fields: symbol_class,
               has_valid_BP, and return_address_word_offset.

               The reason there is a list of opt_symbol_record
               objects is that a variable may exist in a
               register for some period of time, and then be
               "spilled" to a memory location, and possibly
               later reloaded into another register.


               Module Optimization Flags Table, Reference
               Information Table

               The DebugFlags field in the debug header
               extension currently have only one bit defined:

$define DBG_OPT         0x0001

               If this bit is set, then the application has
               optimized code somewhere in its modules. The
               ModuleFlags table contains a dword entry of flags
               for each module in the Module table. It is
               indexed by the same module index that is used to
               index the module table.

               Note that the optimizations performed may be different than the optimizations
               requested when the module was compiled.

               Each word currently describes the sorts of
               optimizations the compiler has done to the
               module. The following bits are defined:

$define MO_globalCSEs           0x0001
$define MO_localCSEs            0x0002
$define MO_inductVars           0x0004
$define MO_codeMotion           0x0008
$define MO_regAlloc             0x0010
$define MO_loadOptim            0x0020
$define MO_loopOpt              0x0040
$define MO_intrinsics           0x0080
$define MO_deadStorElim         0x0100
$define MO_copyProp             0x0200
$define MO_jumpOpt              0x0400
$define MO_speed_size           0x0800
$define MO_noAliasing           0x1000

               If the dword is 0, then the module contains no
               optimized code.

		Reference Information Table

               Names

               Any symbolic name encountered in the symbol
               tables is referenced via an index into this
               region. Each identifier is stored with a trailing
               null byte.


               Debugging Turbo Pascal overlays

               Data at address pointed to by debugger_hook:

                 typedef struct
                 {
                   ushort overlay_list; //  start of linked list of overlay 
                                                //  header segs 
                   ushort overlay_size; //  smallest overlay buffer that    
                                                //  can be used 
                   void far *  debugger_hook;   //  ptr to routine in debugger      
                 } overlay;

               A debugger must fill in debugger_hook after
               loading the program. debugger_hook is called by
               the overlay manager after any overlay is loaded.
               The  allows the debugger to set  in the newly
               loaded segment. When called, ES contains the base
               segment of the overlay header BX contains the
               offset that the overlay manager will jump to in
               the newly loaded code. (This is useful if an int
               3F has been traced--an int 3f is followed by data
               and is not returned.)

               The actual segment of a particular overlaid
               segment is at offset 10h in the overlay header.
               If this value is zero, then the segment is not
               loaded.

               Data objects in an overlaid segment will contain
               the segment of the overlay header and the true
               offset in the code segment.
#endif
    }
}
