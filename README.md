
# reko - a general purpose decompiler.

 [![Build Status](https://travis-ci.org/uxmal/reko.svg?branch=master)](https://travis-ci.org/uxmal/reko) [![Join the chat at https://gitter.im/uxmal/reko](https://badges.gitter.im/uxmal/reko.svg)](https://gitter.im/uxmal/reko?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
 
<img class="keyboard-shortcuts"  src="https://raw.githubusercontent.com/uxmal/reko/analysis-development/web/images/reko.png" /> 

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

If you're interested in contributing code, see the 
[road map](https://github.com/uxmal/reko/wiki/Roadmap) for areas to explore.
The [Wiki](https://github.com/uxmal/reko/wiki) has more information 
about the Reko project's internal workings.

## Recent versions

See NEWS.md for the change log.
