## Reko.Core.Machine

The `Reko.Core.Machine` namespace contains interfaces and base classes used to implement disassemblers for different processor architectures. A disassembler can be viewed as a filter that consumes a stream of machine code bytes or words, and generates a stream of disassembled instructions. Every processor architecture provides a disassembler implementation, which is modeled as an `IEnumerable<MachineInstruction>`; that is, a stream of machine instructions that can be iterated over or used in Linq expressions. Most disassembler implementations will use the convenient `DisassemblerBase` as a base class.

A `MachineInstruction` models a disassembled processor instruction. It may have 0 or more `MachineOperand`s. Derived classes can add more properties to `MachineInstruction` to support the needs of particular processors. 

Many disassemblers need to examine bitfields of the machine code. These bitfields may encode registers, constants of various widths, or multiple level of encodings. An abstract `Decoder` class is provided for the implementation of these decoders. The `Decode()` method of this class consumes an instruction opcode and performs the appropriate action. The `MaskDecoder` and the `BitFieldDecoder` can be used to construct "trees" of cascading decoders, which can be used to decode instructions with complex encodings.

