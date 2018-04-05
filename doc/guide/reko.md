Reko user's guide
==
Reko is a decompiler that takes one or more input binaries and decompiles them into
a high-level language.


The decompilation process
==
Reko decompiles binaries using the following stages. 

Loading
===
The first stage is loading the binary into memory. Reko identifies the binary file format by
looking for "magic numbers" in the binary file header and/or by looking at the file extension.
Each different binary file format requires a different loader. Some binaries may in addition 
be packed and require unpacking / decompressing before further processing.

Once a loader has loaded the binary into memory, it is responsible for collecting metadata
from the binary image. Often, binary files contain information about what processor architecture
the binary is intended for, one or more entry points where executable machine code instructions
are known to exist, and symbols that associate names -- and even data types -- with addresses.

The loader, when finished, provides Reko with:
* memory areas, which are flat ranges of bytes.
* Image segments, which associate addresses with memory areas.
* Symbolic metadata. Symbols associate addresses with names and possibly data types.

Scanning
===
After loading the binary the **scanning** stage begins. The scanner extracts the parts of the
binary that correspond to executable machine code, discovers cross-references between source
instructions and target instructions, and are assembled into the program's control flow graph.

The scanner starts at addresses that are known to be executable machine code and starts
translating the machine code instructions into a low-level, machine independent
register transfer language (RTL). The scanner traces all potential paths in the machine code
until no more machine code instructions can be found. Every time a `jump` instruction or
a `call` instruction is encountered, an edge is added to the program's control flow graph.

A recursive scan will often fail to find executable code. Often this is because the scanner 
has difficulties time tracing indirect jumps or calls to their run-time destinations. In such cases,
the user has the option of using a heuristic procedure to locate more machine code by
scanning memory areas that were missed by the recursive algorithm. The heuristic procedure
usually discovers the bulk of the remaining machine code and translates it into RTL.

Finally, the control graph is analyzed to collect clusters of non-branching 
instructions into **basic blocks**. Interconnected basic blocks are in turn collected 
into **procedure**s.


