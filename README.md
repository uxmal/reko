# reko - a general purpose decompiler.

 [![Build Status](https://travis-ci.org/uxmal/reko.svg?branch=master)](https://travis-ci.org/uxmal/reko)
 
**Reko** (Swedish: "decent, obliging") is a C# project containing 
a decompiler for machine code binaries.  This project is freely 
available under the GNU General Public License.

The project consists of front ends, core decompiler engine, and back
ends to help it achieve its goals.  A command-line, a Windows GUI,
and a ASP.NET front end exist at the time of writing.  The decompiler
engine receives inputs from the front ends in the form of either
individual executable files or decompiler project files. Reko
project files contain additional information about a binary file,
helpful to the decompilation process or for formatting the output.
The decompiler engine then proceeds to analyze the input binary.

Please note that many software licenses prohibit decompilation or
other reverse engineering of their machine code binaries. Use this
decompiler only if you have legal rights to decompiler the binary
(for instance if the binary is your own.) 

## Hacking
You should be able to build reko by simply cloning https://github.com/uxmal/reko
and opening the `reko-decompiler.sln` solution file with Visual 
Studio 2012 or later, or MonoDevelop. All external dependencies 
are included in the `external` directory. **Note:** please let us 
know if you are not able to compile, so we can fix the issue for 
you.

If you're interesting in contributing code, see the 
[road map](https://github.com/uxmal/reko/wiki/Roadmap) for areas to explore.
The [Wiki](https://github.com/uxmal/reko/wiki) has more information 
about the Reko project's internal workings.

## Recent versions

### Version 0.5.4.0
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

