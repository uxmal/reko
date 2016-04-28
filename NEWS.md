## Recent versions of Reko

### Version 0.6.0.0
* Combined code and low-level viewer, to allow users to compare Reko's output
  with the original binary.
* A graph viewer, to help users to visualize procedure call graph 
* Image map view shows an overview of the image bytes as pixels 
* Allow user to specify the encoding used for text when displaying character data
* New reko configuration and project file formats
* Improvements in user-specified data types
* More MIPS instructions supported
* SPARC support greatly expanded
* Added initial support for DEC VAX processor architecture
* Support for SEGA Genesis platform added
* Much better support for ELF binaries, both executables and relocatable object files
* Refactored central ImageMap class to support binaries with large address space "gaps" 

### Version 0.5.5.0
* Shingled disassembler implementation, in preparation for heuristic discovery of code.
* Support for WinCE / MIPS
* Improved data type inference
* New project file format supports saving of more user options.
* Better support for MonoDevelop
* Many many small bugfixes

### Version 0.5.4.0
* Support for renaming decompiled procedures and changing their signatures.
  Changing signatures will affect following decompilation stages, as
  expected.
* Support for the Windows - OS/2 New Executable format (NE EXE)
* Implemented continuous integration using Travis CI

### Version 0.5.3.0
* Support for the MIPS architecture, and MIPS support in the 
  PE and ELF image format
* TextViewer now supports free selection of text.

### Version 0.5.2.0
* New structural analysis inspired by "Native x86 Decompilation Using 
  Semantics-Preserving Structural Analysis and Iterative Control-Flow 
  Structuring" (Schwartz, Lee, Woo, Brumley) yields much improved
  structured C-like code; the implementation itself is much simpler 
  than the previous one.
* C2Xml tool understands C++11 attributes, in particular the new 
  [[reko::reg(<regname>)]] which permits specifying the register in
  which an argument is passed to or returned from a procedure.
* Implemented navigation history for CodeView window.
* Added support for ELF x86-64 and PE Thumb binaries.
* Many bugs fixed and code refactored.

### Version 0.5.1.0
* Integrated the Capstone.NET disassembler, now used for the ARM and 
  ARM Thumb architectures.
* Support for Arm Thumb PE executables.
* Bug fixes for #14 and #17.

### Version 0.5.0.0
* Moved project from SourceForge
* Renamed project to 'Reko'
* Started implementation of heuristic static analysis.

### Version 0.4.5.0 
* Command line interpreter supports --default-to option
* More ARM instructions implemented
* CP/M environment added
* 64-bit Windows enviroment added

### Version 0.4.4.0
* Mostly bugfixes

### Version 0.4.3.0
* Beginnings of new tabbed GUI
* Command line interpreter now understands a few switches; try typing
    decompile --help
* More x86 and m68k instructions supported

### Version 0.4.2.0 
* Added support for PS3 and System V ELF files.
* For fun: a C64 BASIC "decompiler"!
* Fixed the following submitted bugs
#8: Can not load ELF or Amiga Hunk binaries 
#9: Can not run the decompiler from the command line
* Implemented more x86 and PowerPC instructions
* Improvements in SSA transformation code in preparation for
  move to new decompilation model
* ImageSegmentRenders allow viewing of the structured data
  in image segments.

### Version 0.4.1.0 
* (Crude) support for loading 32- and 64-bit PowerPC ELF binaries
* Many PowerPC opcodes supported.
* User interface bugfixes (broken keyboard accelerators etc)
* The Copy command (Ctrl+C) is supported in windows where it 
  makes sense. 

### Version 0.4.0.0
* Support added for loading packed binaries using unpacker scripts
  written in OdbgScript.
* Modest beginning of an X86 emulator added to the solution
* New memory navigation bar assists in overview of binary image
* More x86 opcodes supported.
* Many bugs fixed.

Special thanks to halsten, who assisted in the implementation and 
testing of the OdbgScript loader. 
 
