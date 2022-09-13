## Reko.Scanning namespace

The classes in this namespace are concerned with **scanning** a binary image to
find as much executable binary code as possible, and reassemble such discovered
code into `Procedure`s. These `Procedure`s are then passed on to later phases
of the decompiler.

### Strategy

The Scanner's primary goal is to inspect the raw binary *storage units* (which in
most modern CPU architectures are `byte`s) and identify *basic blocks*, sequences
of machine instructions ending with a *Control Transfer Instruction* (CTI), like
`goto` or `call`. Each CTI transfers the processor to one or more one addresses,
each of which is the beginning of a new basic block. The identification of basic
blocks continues recursively in this way until no more basic blocks can be found.

The resulting "soup" of basic blocks and the edges between them constitute the *Interprocedural Control Flow Graph* (ICFG). The ICFG is established by following
the control flow of the program starting from *entry points*, indicated by the
metadata in the executable, and optional user-specified procedures.

Once the basic blocks of the program have been discovered, they are grouped together
into subgraphs called *clusters*. These are the *weakly connected components* (WCCs)
of the ICFG, with the added constraint that the incoming edges into a block
which is the target of one or more `call` instructions are removed. The clusters
are further refined until the entry point is discovered. Finally, the cluster's 
entry point is used as the starting point for a depth first search that results
in the generation of procedures. 


### Recursive scanning

The scanning process starts by identifying entry points from information contained
in the binary. Most binary executable formats define one or more entry points to
the executable code as part of their metadata. In addition, users may supplement 
the binary file's metadata by adding their own procedure entry point metadata.

The entry points serve as root vertices in the ICFG and are fed to an instance of `RecursiveScanner`. This scanner starts at the root vertices and disassembles
instructions until it reaches a *Control Flow Instruction* (CFI). There are four
principal kinds of CFIs:
* Unconditional jumps, which may be direct (to a known target address) or indirect 
  (the target address is computed at run-time)
* Conditional jumps, or branches, which test a condition and change control flow
  depending on whether the test evaluated to true or false.
* Calls, which save a *continuation* either on a stack or in a register, and then
  transfer control somewhere else
* Returns, which transfer control to a destination determined by a continuation.

Visiting a CFI results in the generation of 0 or more edges in the ICFG. The
`RecursiveScanner` traverses these edges using a depth-first search, until no more
edges can be discovered. 

The hardest problem for the `RecursiveScanner` is how to reason about *indirect CTI*s.
These are transfer instructions where the target address is not known until 
runtime. Several heuristics are available to help resolve indirect CTIs.

Case or switch statements are commonly implemented as indirect CTIs: a `goto r` 
instruction where `r` is a register or a computed address. The `RecursiveScanner`
can use the `BackwardSlicer` class to recover the computation of `r`, which in many
cases results in an expression like `addrBase + (index * addresSize)` which indexes
into an array of jump destinations. The expression is evaluated to yield one or 
more address to which the scanner can continue its search.

Reko also maintains a `ProcessorState` instance while traversing the control flow
graph. The `ProcessorState` can be used to emulate the contents of registers,
and in that way run-time values of indirect CTI targets may be estimated. 

Finally, once the depth first search terminates, the recursive scanner has found 
all basic blocks it can reach by following edges of the ICFG. However, there 
may still be many basic block which weren't reached. At this point the the
scanning process switches over to *shingled scanning*.

### Shingled scanning

The shingled scanner performs a linear scan of every memory unit where the
`RecursiveScanner` failed to find executable code. On architectures where 
instructions are of varying lengths, this results in some instructions overlapping
each other, like shingles on a roof. An extension to the ICFG graph is constructed
from these instructions, consisting of subgraphs corresponding to procedures, which
will have a lot more "entry nodes" than the real ones. These need to be discarded.

A second pass is needed to identify conflicting basic blocks and eliminate the 
false basic blocks. 
Several heuristics are used:
* Basic blocks that end in jumps or calls to blocks found by the recursive scanner
  are considered more correct than blocks that don't.
* Basic blocks that are called by other shingled blocks are considered more
  correct than blocks that aren't.
* Basic blocks that follow directly after blocks containing padding are considered
  more correct


