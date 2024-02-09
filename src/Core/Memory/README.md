## Reko.Core.Memory

The `Reko.Core.Memory` namespace contains types that abstract various types of
computer memories. Memories vary in how they are organized and how individial
data units are accessed. Although the majority of contemporary processor
architectures access memory using byte addressing, this is not always the case
for all architectures. Therefore, an abstraction is needed to isolate the
other components of Reko from the low-level details of the memory system of a
particular processor.

The contents of program binaries are loaded into a `MemoryArea`, or rather one
of the concrete subclasses of `MemoryArea`. Currently Reko supports memory
areas whose storage units are 8 bits (which satisfies the requirements of most
contemporary processors), 16 bits (for MIL-STD-1750A and Microchip PIC
processors) and 64 bits (for Cray machines). In addition, Reko's PDP-10
implementation has support for memories with 36-bit word granularity.

Data can be read from a `MemoryArea` by random access or sequentially. Random
access of the backing storage for the memory is done using array-valued
properties (called `Bytes` or `Words` as appropriate). Data is read in a
sequential fashion by using one of the implementations of the `ImageReader`
interface. `ImageReader` implementations exist for big- and little-endian 
interpretation of quantities whose representation is larger than the smallest
addressable unit.

Finally, an `IMemory` interface is provided to abstract away all the details
of memory areas and `ImageSegments`. It supports many of the operations of 
`MemoryArea` but random access of memory is easier to use. This comes at
a slight added cost of performance.
