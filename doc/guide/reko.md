# Reko user's guide
Reko is a binary executable decompiler. It takes one or more input binary executable files and decompiles them into a high-level language. It can be used interactively with a GUI shell, as a command line program, or as a .NET library in your own program.

This tutorial assumes you have at least some familiarity with the machine code or assembly language of the binary you're working with. For popular CPU architectures the internet has plenty of tutorials. You will also have some working familiarity with how compilers work; perhaps you've turned on the `-S` or `/FA` switch on your compiler and looked at the resulting assembly language. 

## Installation
On Windows machines, you can download a published installer from https://github.com/uxmal/reko/releases. Just run the MSI installer appropriate for your machine; currently 32-bit and 64-bit x86 Windows are supported. The installer will install both the GUI and the command line clients. The software will be installed under `$(Program Files)\jklSoft\Reko`. If you want to use the command line client, it will be convenient for you if you put `$(Program Files)\jklSoft\Reko` in your PATH environment variable.

On non-Windows machines, we recommend downloading the output of the AppVeyor continuous integration (CI) build https://ci.appveyor.com/project/uxmal/reko/build/artifacts. The zip file for the GUI client is called `WindowsDecompiler-xxxxx.zip` while the CLI client is called `CmdLine-xxxx.zip`. Unpack either of these zip files in a directory on your machine. No further installation or configuration is necessary, provided you have Mono 5.10 or later installed. It may be convenient for you to add the directory into which you installed Reko into your PATH environment variable.

If you wish to build Reko yourself, git clone https://github.com/uxmal/reko and follow the [build instructions](../build.md).

## Running the GUI client
On Windows, the installer will place a "Reko decompiler" shortcut in your start menu, from where you can launch the GUI client.

On non-Windows systems, you will need to execute the program `WindowsDecompiler.exe` as follows:
```
mono WindowsDecompiler.exe
```
Once the GUI client is running you can [start working with it](gui.md).

## Running the command line client
On Windows, assuming the Reko installation directory is in your PATH variable, you can run the Reko command line client as follows:
```
decompile some_binary_file.exe
```
On non-Windows machines, assuming the Reko installation directory is in your PATH variable you need to prefix the command line above with `mono`:
```
mono decompile some_binary_file.exe
```
The command line client has many options, which may be displayed by running the program with the `--help` switch. Working with the command line client is [detailed here](cli.md).

## Using Reko as a reverse engineering library
If you're comfortable with programming, you can access the Reko object model directly. For instance, if you want to write out the name and address of all the known procedures in a program, you could write:
```C#
using Reko;
using Reko.Core;

class DumpProcedures {
    public static int Main(string [] args) {
        var dec = DecompilerDriver.Create();
        if (!dec.Load("myfile.exe"))
            return -1;
        dec.ScanPrograms();
        foreach (var program in dec.Project.Programs) {
            foreach (var entry in program.Procedures) {
                Console.Write("Address {0}, procedure name {1}", entry.Key, entry.Value);
            }
        }
    }
}
```

Reko has a rich API you can use for reverse engineering. [Read on for more details](api.md).


## Quick start
Now that you're installed, it's time to learn what Reko is about. Try the [quick start](quickstart.md) if you're impatient, or read on to gain a deeper understanding of what is going on.

## The decompilation process
Compilation is inherently a lossy process. Information such as complex data types and comments are lost when transforming a high level source code program to a low level machine code executable binary. A useful decompilation usually requires assistance from a user. Users can supply type information that was discarded by the compiler, and add comments or give friendly names to procedures.
User can also write [scripts](scripting.md) for adding procedure names, comments, etc.

### Loading
The first stage is loading the binary into memory. Reko identifies the binary file format by looking for "magic numbers" in the binary file header and/or by looking at the file extension. Each different binary file format requires a different loader. Some binaries may in addition be packed and require unpacking / decompressing before further processing.

Once a loader has loaded the binary into memory, it is responsible for collecting metadata from the binary image. Often, binary files contain information about what processor architecture the binary is intended for, one or more entry points where executable machine code instructions are known to exist, and symbols that associate names -- and even data types -- with addresses.

The loader, when finished, provides Reko with:
* memory areas, which are flat ranges of bytes.
* Image segments, which associate addresses with memory areas.
* Symbolic metadata. Symbols associate addresses with names and possibly data types.

### Scanning
After loading the binary the **scanning** stage begins. The scanner extracts the parts of the binary that correspond to executable machine code, discovers cross-references between source instructions and target instructions, and are assembled into the program's control flow graph.

The scanner starts at addresses that are known to be executable machine code and starts translating the machine code instructions into a low-level, machine independent register transfer language (RTL). The scanner traces all potential paths in the machine code until no more machine code instructions can be found. Every time a `jump` instruction or a `call` instruction is encountered, an edge is added to the program's control flow graph.

A recursive scan will often fail to find executable code. Often this is because the scanner has difficulties time tracing indirect jumps or calls to their run-time destinations. In such cases, the user has the option of using a heuristic procedure to locate more machine code by scanning memory areas that were missed by the recursive algorithm. The heuristic procedure usually discovers the bulk of the remaining machine code and translates it into RTL.

Finally, the control graph is analyzed to collect clusters of non-branching instructions into **basic blocks**. Interconnected basic blocks are in turn collected into **procedure**s.

