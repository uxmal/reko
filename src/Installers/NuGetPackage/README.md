## Reko Decompiler runtime
The Reko Decompler runtime provides reverse engineering tools
intended for use within .NET programs.

Included in the NuGet are:
* Disassemblers and lifters for various CPU architectures,
  including general purpose CPUs (like x86, ARM, MIPS and Risc-V),
  microcontrollers (like H8, MSP430, Xtensa), and historical
  CPU architectures (like Z80, 6502, M68k, VAX, PDP-10)
  For a full list of supported architectures, visit the [Reko project](https://github.com/uxmal/reko/wiki/Supported-binaries).
* File loaders for executable file formats (like MS-DOS EXE,
  Windows NE and PE executables, ELF, and COFF)
* Classes and interfaces for representing concepts like
  "memory", "registers", "machine instruction",  "disassembler", 
  "emulator".

### Disassembling a fragment of ARM machine code
```C#
using Reko.Arch.Arm.AArch32;
using Reko.Core;
using Reko.Core.Memory;
using System.ComponentModel.Design;

// Create some ARM machine code and put it in a
// memory area at a specific address.
byte[] bytes = new byte[] { 0x3A, 0xB7, 0xB1, 0xE3 };
Address addr = Address.Ptr32(0x0100_0000);
var mem = new ByteMemoryArea(addr, bytes);

// Create an instance of the ARM32 architecture object.
var sc = new ServiceContainer();
var armArch = new Reko.Arch.Arm.Arm32Architecture(sc, "arm32", new());

// Create a little-endian image reader starting at the
// address addr, and disassemble instructions until
// the end of the memory area has been reached.
var reader = mem.CreateLeReader(addr);
var disassembler = armArch.CreateDisassembler(reader);
foreach (AArch32Instruction instr in disassembler)
{
    Console.WriteLine("{0}: {1}", instr.Address, instr);
}
```

### Links
* [The Reko project](https://github.com/uxmal/reko)
* [Supported CPUs and file formats](https://github.com/uxmal/reko/wiki/Supported-binaries)
* [Issue tracker](https://github.com/uxmal/reko/issues) - for feature requests and bug reports



