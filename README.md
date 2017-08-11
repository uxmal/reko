
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

Reko has the ambition of supporting decompilation of various 
processor architectures and executable file formats with minimal user
intervention. For a complete list, see the
[supported binaries](https://github.com/uxmal/reko/wiki/Supported-binaries) 
page.

Please note that many software licenses prohibit decompilation or
other reverse engineering of their machine code binaries. Use this
decompiler only if you have legal rights to decompile the binary
(for instance if the binary is your own.)

## Hacking

To build reko, start by cloning https://github.com/uxmal/reko. You
can use an IDE or the command line to build the solution file
`Reko-decompiler.sln`. If you are an IDE user, use Visual
Studio 2013 or later, or MonoDevelop version 5.10 or later. If you
wish to build using the command line, use the commands

```cmd
msbuild Reko-decompiler.sln
```

on Windows machines or

```sh
xbuild Reko-decompiler.sln
```

on machines with the Mono toolchain. All external dependencies
needed to build are included in the `external` directory.

**Note**: some users have reported difficulties with certain
Linux distributions that don't support the 4.0 CLR framework
(see issue #251 for details). One workaround that has been
identified is to specify the CLR target framework explicitly
at the xbuild command line:

```sh
xbuild /p:TargetFrameworkVersion="v4.5"  Reko-decompiler.sln
```

You may be able to work around the problem reported in issue #251
by downloading the appropriate Mono CLR binaries from
git://github.com/mono/reference-assemblies.git

**Note**: please let us know if you still are not able to compile,
so we can help you fix the issue.

### Warnings and errors related to WiX

You will receive warnings or errors when loading the solution in Visual Studio
or MonoDevelop if you haven't installed the WiX toolset on your
development machine. You can safely ignore the warnings; the WiX
toolset is only used when making MSI installer packages, and isn't even
supported in MonoDevelop. You will not need to build an installer if
you're already able to compile the project: the build process copies
all the necessary files into If you do want to build an MSI installer
with the WiX toolchain, you can download it here:
http://wixtoolset.org/releases/

### How do I start Reko?

The solution folder `Drivers` contains the executables that act
as user interfaces: the directory `WindowsDecompiler` contains
the GUI client for Windows users; `MonoDecompiler` contains the GUI
client for Mono users; `CmdLine` is a command line driver.

If you're interested in contributing code, see the
[road map](https://github.com/uxmal/reko/wiki/Roadmap) for areas to explore.
The [Wiki](https://github.com/uxmal/reko/wiki) has more information
about the Reko project's internal workings.

## Recent versions

See NEWS.md for the change log.
