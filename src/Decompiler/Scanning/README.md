## Reko.Scanning namespace

The classes in this namespace are concerned with **scanning** a binary image to
find as much executable binary code as possible, and reassemble such discovered
code into `Procedure`s. These `Procedure`s are then passed on to later phases
of the decompiler.

The general strategy is to identify linear *basic blocks*, all terminating in a
*Control Transfer Instruction* (like `goto` or `call`), and possible *procedures*.


### Recursive scanning

The scanning process starts by identifying entry points from information contained
in the binary or provided by the user. Some binary executable formats define one or
more entry points to the executable code as part of their metadata. These serve as
root vertices in the *Interprocedural Control Flow Graph* (ICFG). The root vertices
are fed to an instance of `RecursiveScanner`. This instance starts at the vertices 
and disassembles instructions until it reaches a *Control Flow Instruction* (CFI). 

There are four principal kinds of CFIs:
* Unconditional jumps, which may be direct (to a known target address) or indirect 
  (the target address is computed at run-time)
* Conditional jumps, or branches, which test a condition and change control flow
  depending on whether the test evaluated to true or false.
* Calls, which save a *continuation* either on a stack or in a register, and then
  transfer control somewhere else
* Returns, which transfer control to a destination determined by a continuation.

Visiting the CFIs results in the generation of 0 or more edges in the ICFG. The
`RecursiveScanner` traverses these edges using a depth-first search, until no more
edges can be discovered. 

The hardest problem for the `RecursiveScanner` is how to reason with indirect CTIs
where the target address is not known until runtime. Several heuristics are available
to help resolve indirect CTIs.

Switch statements are commonly implemented as indirect CTIs: a `goto r` instruction
where `r` is a register or a computed address. The `RecursiveScanner` can use 
the `BackwardSlicer` class to recover the computation of `r`, which in many cases
results in an expression like `addrBase + (index * addresSize)` which indexes into
an array of jump destinations.

Reko also maintains a `ProcessorState` instance while traversing the control flow
graph. The `ProcessorState` can be used to emulate the contents of registers,
and in that way run-time values of CTI targes may be estimated. 

Finally, once all possibilities are exhausted, the scanning process switches over
to *shingled scanning*

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


