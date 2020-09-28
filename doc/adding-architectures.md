# How to add support for a new processor architecture in Reko

##  Creating the bare minimum files and integrating into the build system.

For the example below, the fictitious processor **Foo-2000** is used. Naturally, you would use the real name of your processor instead.

1. Add a new `Foo2000.csproj` file under $(REKO)/src/Arch. Make sure it has a reference to the Reko `Core` project. 
2. Add the new project to the `Architectures` solution folder.
3. In the new project's properties, set the assembly name to `Reko.Arch.Foo2000` and the namespace to `Reko.Arch.Foo2000`.
4. Add the following classes for your project:
  Foo2000Architecture - the central class that Reko will use to create architecture-specific objects.
  Foo2000Disassembler - a disassembler that knows how to disassemble Foo2000 machine code
  Foo2000Instruction - the instruction type for your architecture.
  Foo2000ProcessorState - a class that models the processor state.
  Foo2000Rewriter - a class that rewrites a stream of `Foo2000Instruction`s into a stream of RTL instruction clusters.
5. Pick a short alias for your processor architecture (e.g. "foo2000")
6. In the file $(REKO)/src/Drivers/reko.config, add a new `<Architecture>` element under the `<Architectures>` element. It should look like this:
```xml
<Architecture Name="foo2000" Description="Foo-2000" Type="Reko.Arch.Foo2000.Foo2000Architecture,Reko.Arch.Foo2000" />
```
The `Name` attribute should be the short alias you picked in step 5. The `Description` should be a brief description of the architecture. Finally,
the `Type` attribute must be the fully qualified name of the architecture class you created in step 4.
7. Edit the `$(REKO)/src/Drivers/Common.items` file, adding a new `<Architectures>` item following the pattern of the existing ones.
8. Add unit test files for at least the disassambler and rewriter classes under $(REKO)/src/UnitTests/Arch:
    Foo2000DisassemblerTests.cs
    Foo2000RewriterTests.cs
9. Add a reference to the `Foo2000.csproj` to the WiX installer project.
10. Add a `<File>` element in the section commented `<!-- Architectures -->`

The `Foo2000` architecture is now part of the Reko solution. Now it's time to write the minimal code to make it load.

## Implementing the bare minimum interfaces

1. Start with `Foo2000Architecture`. All Reko architectures implement the `Reko.Core.IProcessorArchitecture` interface. You can choose to implement this interface directly or use the abstract base class `Reko.Core.ProcessorArchitecture`. Most architecture implementations choose the latter. Stub out all the abstract methods, for now.
2. There should be at least a constructor which accepts a single string parameter, the `archId` or alias defined above, and passes on to the base class. In the constructor set the following properties:
* `InstructionBitSize`: the "granularity" -- in bits -- of instructions of the architecture. X86 and most 8-bit microprocessors have 8 bit granularity, M68k and ARM Thumb have 16-bit granularity, most RISC architectures have 32-bit granularity.
* `FramePointerType`: the size of the stack pointer as a Reko data type. Typically `PrimitiveType.Ptr16`, `PrimitiveType.Ptr32`, or `PrimitiveType.Ptr64`.
* `PointerType`: the size of a typical pointer variable
* `WordWidth`: the size of a typical word of this architecture
* `CarryFlagMask`: if the architecture has condition codes, this is the bit mask for the Carry flag in the program status word or flag register
* `StackRegister`: if the architecture defines it, the stack register as a `RegisterStorage`.
3. You may find it convenient to have a separate static class `Registers` to manage all the architectural registers of the processor. Most of the architecture implementations have a  `Registers` class: browse around for inspiration.
4. Every architecture that is capable of reading more than one addressable memory unit (read 'byte'/'octet') has a default endianness. Express the appropriate endianness by implementing the various overloaded `CreateImageReader` and `CreateImageWriter` methods. Little-endian architectures should return instances of `Reko.Core.LeImageReader`/`LeImageWriter`, while big-endian architectures should return instances of `Reko.Core.BeImageReader`/`BeImageWriter`. Note: if your architecture supports switching endianness, see below.
5. Implement the `Foo2000Architecture.CreateDisassembler` method. It should simply create an instance of `Foo2000Disassembler` and return it.

## Implementing a disassembler.

Reko regards a disassembler as an object which, given an `ImageReader` positioned at a stream of bytes, returns a stream of disassembled machine instructions.
The vast majority of disassemblers are implemented as subclasses of `Reko.Core.DisassemblerBase<T>` an abstract generic class where `T` stands for the particular machine instruction type used by the processor, in this example `Foo2000Instruction`. The disassembler class `Foo2000Disassembler` should have a constructor that accepts the `EndianImageReader` passed to `Foo200Architecture.CreateDisassembler`. 

The only method to implement is `DisassembleInstruction()` which reads one or more bytes from the image reader, decodes them, and returns a `Foo2000Instruction`. There are some cases your disassembler will need to handle. If the image reader is at the end of its range, so that attempting to read from it with any of the `TryRead...` methods returns false, you should return `null` from `DisassembleInstruction()`. If the image reader is positioned such that there are 1 or more bytes available, but not sufficient to make a valid instruction, then `DisassembleInstruction()` should return a `Foo2000Instruction` whose `InstructionClass` is `Invalid`. Otherwise, the method should return a properly initialized `Foo2000Instruction`.

The implementation of `DisassembleInstruction` is usually quite mechanical. Depending on the granularity of the machine instructions, you will call `EndianImageReader.TryReadByte`, `TryReadUInt16`, `TryReadUInt32` etc to read the opcode of the instruction. Once read, the opcode needs to be decoded. Typically you will create arrays of decoders, indexed by masked pieces of the opcode. You can extract bitfields conveniently with the `Reko.Core.Lib.BitField` class and the static `Reko.Core.Lib.Bits` utility class.

The returned `Foo2000Instruction` will have 0 or more `MachineOperand`s, corresponding to the operands in the assembler language of Foo2000. The most common type of machine operands are `RegisterOperand` and `ImmediateOperand`, both of which are provided for you in `Reko.Core.Machine`. Because memory operands vary dramatically across all architectures, you will have to implement your own family of `MemoryOperand`s. Depending on the complexity of the address modes of Foo2000, this may be as simple as a single class with the `BaseRegister`, `Offset`, and `IndexRegister` fields, or the nightmare of M68000, which holds the dubious honor of having the most complex addressing modes in Reko.

You are strongly encouraged to develop unit tests to validate proper disassembly of your instructions. Look at the various existing disassemblers for examples of how to write such unit tests. Most of the Reko architectures use `Reko.UnitTests.Arch.DisassemblerTestBase<T>` as a convenient base class for implementing the disassembler.

## Implementing a MachineInstruction subclass

Your disassembler will emit instructions. Reko only uses the `InstructionClass`, `Address`, and `Length` properties of the instruction. However you will need more properties to keep track of the opcode and operands of the disassembled instruction. Your implementation of `Render` should render the instruction 





