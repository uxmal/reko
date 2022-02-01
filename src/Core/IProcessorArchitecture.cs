#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core.Assemblers;
using Reko.Core.Code;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Abstraction of a CPU architecture.
    /// </summary>
	public interface IProcessorArchitecture
	{
        /// <summary>
        /// Creates an IEnumerable of disassembled <see cref="MachineInstruction" />s which consumes 
        /// its input from the provided <paramref name="imageReader"/>.
        /// </summary>
        /// <remarks>The IEnumerable lets us use Linq expressions
        /// like Take() on a stream of disassembled instructions.</remarks>
        /// <param name="imageReader"></param>
        /// <returns>
        /// An <see cref="IEnumerable{MachineInstruction}"/>, which can be 
        /// viewed as a stream of disassembled instructions.
        /// </returns>
        IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader);

        /// <summary>
        /// Creates an instance of a ProcessorState appropriate for this
        /// processor.
        /// </summary>
        /// <returns>An instance of <see cref="ProcessorState"/>.</returns>
		ProcessorState CreateProcessorState();

        /// <summary>
        /// Returns a stream of machine-independent instructions, which it
        /// generates by successively disassembling machine-specific instructions
        /// and rewriting them into one or more machine-independent RtlInstructions
        /// codes. These are then returned as clusters of RtlInstructions.
        /// </summary>
        IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host);

        /// <summary>
        /// Given a set of addresses, returns a set of address where something
        /// is referring to one of those addresses. The referent may be a
        /// machine instruction calling or jumping to the address, or a 
        /// reference to the address stored in memory.
        /// reference
        /// </summary>
        /// <param name="rdr"></param>
        /// <param name="knownAddresses"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags);

        /// <summary>
        /// Creates a Frame instance appropriate for this architecture type.
        /// </summary>
        /// <returns></returns>
        Frame CreateFrame();

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred endianness of the processor.
        /// </summary>
        /// <param name="memoryArea">Memory area to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <returns>An <seealso cref="EndianImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea memoryArea, Address addr);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor, limited to the specified number of units.
        /// </summary>
        /// <param name="memoryArea">Memory area to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <param name="cbBytes">Number of memory units after which stop reading.</param>
        /// <returns>An <seealso cref="EndianImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea memoryArea, Address addr, long cbUnits);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred 
        /// endianness of the processor, limited to the specified offset
        /// range.
        /// </summary>
        /// <param name="memoryArea">Memory area from which to read.</param>
        /// <param name="offsetBegin">Starting offset within the memory area.</param>
        /// <param name="offsetEnd">Ending offset within the memory area.</param>
        /// <returns>An <seealso cref="EndianImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea memoryArea, long offsetBegin, long offsetEnd);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor.
        /// </summary>
        /// <param name="memoryArea">Program image to read</param>
        /// <param name="addr">offset from the start of the image</param>
        /// <returns>An <seealso cref="EndianImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea memoryArea, long off);

        /// <summary>
        /// Creates an <see cref="ImageWriter" /> with the preferred 
        /// endianness of the processor.
        /// </summary>
        /// <returns>An <see cref="ImageWriter"/> of the appropriate endianness.</returns>
        ImageWriter CreateImageWriter();

        /// <summary>
        /// Creates an <see cref="ImageWriter"/> with the preferred endianness
        /// of the processor, which will write into the given <paramref name="memoryArea"/>
        /// starting at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="memoryArea">Memory area to write to.</param>
        /// <param name="addr">Address to start writing at.</param>
        /// <returns>An <see cref="ImageWriter"/> of the appropriate endianness.</returns>
        ImageWriter CreateImageWriter(MemoryArea memoryArea, Address addr);
        string RenderInstructionOpcode(MachineInstruction instr, EndianImageReader rdr);

        /// <summary>
        /// Creates an <see cref="IAssembler"/> instance which can be used to translate
        /// assembly language to machine code for this processor architecture.
        /// </summary>
        /// <param name="asmDialect">On some processors there are many "dialects" of assembly
        /// language. This parameter allows the caller to select a dialect. Passing null
        /// uses the default, manufacturer dialect.</param>
        /// <returns></returns>
        IAssembler CreateAssembler(string? asmDialect);

        /// <summary>
        /// Reads a value from memory, respecting the processor's endianness. Use this
        /// instead of <see cref="ImageReader"/> when random access of memory is requored.
        /// </summary>
        /// <param name="mem">Memory area to read from</param>
        /// <param name="addr">Address to read from</param>
        /// <param name="dt">Data type of the data to be read</param>
        /// <param name="value">The value read from memory, if successful.</param>
        /// <returns>True if the read succeeded, false if the address was out of range.</returns>
        bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value);

        /// <summary>
        /// Reads a value from memory and interpret the resulting bits in a way appropriate
        /// for the processor and the given <see cref="PrimitiveType"/>.
        /// </summary>
        /// <param name="rdr"><see cref="EndianImageReader"/> from which to read.</param>
        /// <param name="dt">The <see cref="PrimitiveType"/> of the data to be read.</param>
        /// <param name="value">Variable receiving the read value, if reading was possible.</param>
        /// <returns>True if reading was possible.</returns>
        bool TryRead(EndianImageReader rdr, PrimitiveType dt, out Constant value);

        /// <summary>
        /// Optionally creates a comparer that compares instructions for equality. 
        /// Normalization means some attributes of the instruction are 
        /// treated as wildcards.
        /// </summary>
        /// <param name="norm"></param>
        /// <returns>An instruction comparer if the architecture supports it.</returns>
        IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm);

        /// <summary>
        /// Creates a frame application builder for this architecture.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="site"></param>
        /// <param name="callee"></param>
        /// <param name="ensureVariables"></param>
        /// <returns></returns>
        FrameApplicationBuilder CreateFrameApplicationBuilder(
            IStorageBinder binder,
            CallSite site,
            Expression callee);

        /// <summary>
        /// Create a <see cref="MemoryArea"/> appropriate for this processor.
        /// </summary>
        MemoryArea CreateMemoryArea(Address baseAddress, byte[] bytes);

        /// <summary>
        /// Reinterprets a string of raw bits as a floating point number appropriate
        /// for the current architecture.
        /// </summary>
        /// <param name="rawBits">Raw bits to be interpreted.</param>
        /// <returns></returns>
        Constant ReinterpretAsFloat(Constant rawBits);

        /// <summary>
        /// Creates a processor emulator for this architecture.
        /// </summary>
        /// <param name="segmentMap">The memory image containing the program 
        /// image and initial data.
        /// </param>
        /// <param name="envEmulator">Simulated operating system.</param>
        /// <returns>The emulator ready to run.</returns>
        IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator);

        /// <summary>
        /// Provide an architecture-defined CallingConvention.
        /// </summary>
        CallingConvention? GetCallingConvention(string? ccName);

        /// <summary>
        /// Returns a list of all the available mnemonics as strings.
        /// </summary>
        /// <returns>
        /// A <see cref="Dictionary{TKey, TValue}"/> mapping mnemonic names to their 
        /// internal Reko numbers.
        /// </returns>
        SortedList<string, int> GetMnemonicNames();

        /// <summary>
        /// Returns an internal Reko number for a given instruction mnemonic, or
        /// null if none is available.
        /// </summary>
        int? GetMnemonicNumber(string sMnemonic);
        
        /// <summary>
        /// Returns register whose name is <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the register</param>
        /// <returns>A <see cref="RegisterStorage"/> representing the register,
        /// or null if no such register exists.
        /// </returns>
        RegisterStorage? GetRegister(string name);

        /// <summary>
        /// Given a register, returns any sub register occupying the 
        /// given bit range.
        /// </summary>
        /// <param name="reg">Register to examine</param>
        /// <param name="offset">Bit offset of expected subregister.</param>
        /// <param name="width">Bit size of subregister.</param>
        /// <returns>If an invalid domain is passed, null is returned.</returns>
        RegisterStorage? GetRegister(StorageDomain domain, BitRange range);

        /// <summary>
        /// If the <paramref name="flags"/> parameter consists of multiple sub fields, separate them
        /// into distinct fields.
        /// </summary>
        /// <param name="flags"></param>
        IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags);

        /// <summary>
        /// Returns all registers of this architecture.
        /// </summary>
        /// <returns></returns>
        RegisterStorage[] GetRegisters(); 
        bool TryGetRegister(string name, out RegisterStorage reg); // Attempts to find a register with name <paramref>name</paramref>

        /// <summary>
        /// Get all processor flags of this architecture.
        /// </summary>
        /// <returns></returns>
        FlagGroupStorage[] GetFlags();

        FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf);          // Returns flag group matching the bitflags.

        /// <summary>
        /// Given the name of a flag register bit group, returns the corresponding
        /// FlagGroupStorage.
        /// </summary>
        /// <remarks>
        /// This method is used principally in deserialization from text.
        /// A critical assumption here is that all flag groups can be distinguished from each
        /// other. The safest approach seems to be to use single-character flag names for the 
        /// condition code bits which tend to be the most commonly used, and use a string prefix
        /// if bits from other registers are needed.
        /// </remarks>
        /// <param name="flagRegister"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        FlagGroupStorage? GetFlagGroup(string name);
        Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType);
        //$REFACTOR: this should probably live in FrameApplicationBuilder instead.
        Expression CreateFpuStackAccess(IStorageBinder binder, int offset, DataType dataType);  //$REVIEW: generalize these two methods?

        /// <summary>
        /// Attempt to inline the instructions at <paramref name="addrCallee"/>. If the instructions
        /// can be inlined, return them as a list of RtlInstructions.
        /// </summary>
        /// <remarks>
        /// This call is used to inline very short procedures. A specific application
        /// is to handle idioms like:
        /// <code>
        /// call foo
        /// ...
        /// foo proc
        ///    mov ebx,[esp+0]
        ///    ret
        /// </code>
        /// whose purpose is to collect the return address into a register. This idiom is 
        /// commonly used in position independent (PIC) code.
        /// </remarks>
        /// <param name="addrCallee">Address of a procedure that might need inlining.</param>
        /// <param name="addrContinuation">The address at which control should resume after 
        /// the call.</param>
        /// <param name="rdr">Image reader primed to start at <paramref name="addrCallee"/>.
        /// </param>
        /// <returns>null if no inlining was performed, otherwise a list of the inlined
        /// instructions.</returns>
        List<RtlInstruction>? InlineCall(Address addrCallee, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder);

        Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state);
        Address MakeSegmentedAddress(Constant seg, Constant offset);

        string GrfToString(RegisterStorage flagRegister, string prefix, uint grf);                       // Converts a union of processor flag bits to its string representation

        IServiceProvider Services { get; }                  // Access to services from the Reko process.
        string Name { get; }                                // Short name used to refer to an architecture.
        string? Description { get; set; }                    // Longer description used to refer to architecture. Typically loaded from app.config
        PrimitiveType FramePointerType { get; }             // Size of a pointer into the stack frame (near pointer in x86 real mode)
        PrimitiveType PointerType { get; }                  // Pointer size that reaches anywhere in the address space (far pointer in x86 real mode )
        PrimitiveType WordWidth { get; }                    // Processor's native word size
        int DefaultBase { get; }                            // Base used to render numbers.
        /// <summary>
        /// The size of the return address (in bytes) if pushed on stack.
        /// </summary>
        int ReturnAddressOnStack { get; }
        int InstructionBitSize { get; }                     // Instruction "granularity" or alignment.
        /// <summary>
        /// Size of the smallest addressable memory unit, in bits
        /// </summary>
        /// <remarks>Most modern CPU:s have byte addressability, so this will typically be 8.
        /// </remarks>
        int MemoryGranularity { get; }
        RegisterStorage StackRegister { get; set;  }        // Stack pointer used by this machine.
        RegisterStorage FpuStackRegister { get; }           // FPU stack pointer used by this machine, or null if none exists.
        uint CarryFlagMask { get; }                         // Used when building large adds/subs when carry flag is used.
        EndianServices Endianness { get; }              // Use this to handle endian-specific.

        /// <summary>
        /// Parses an address according to the preferred base of the 
        /// architecture.
        /// </summary>
        /// <param name="txtAddr"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        bool TryParseAddress(string? txtAddr, out Address addr);

        /// <summary>
        /// Given a <see cref="Constant"/>, returns an Address of the correct size for this architecture.
        /// </summary>
        /// <param name="c">Constant to be converted to address.</param>
        /// <param name="codeAlign">If true, aligns the address to a valid code address.</param>
        /// <returns>An address.</returns>
        Address MakeAddressFromConstant(Constant c, bool codeAlign);

        /// <summary>
        /// After the program has been loaded, the architecture is given a final
        /// chance to mutate the SegmentMap or any other property.
        /// </summary>
        /// <param name="program">The program to postprocess.</param>
        void PostprocessProgram(Program program);

        /// <summary>
        /// The dictionary contains options that were loaded from the 
        /// configuration file or the executable image. These can be used
        /// to customize the properties of the processor.
        /// </summary>
        /// <param name="options"></param>
        void LoadUserOptions(Dictionary<string, object>? options);

        /// <summary>
        /// Retrieves any settings on the architecture that may need persisting.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object>? SaveUserOptions();
    }

    /// <summary>
    /// Normalize enumeration controls the operation of instruction
    /// comparer. 
    /// </summary>
    [Flags]
    public enum Normalize
    {
        Nothing,        // Match identically
        Constants,      // all constants treated as wildcards
        Registers,      // all registers treated as wildcards.
    }

    /// <summary>
    /// Abstract base class from which most <see cref="IProcessorArchitecture"/>.
    /// implementations derive.
    /// </summary>
    [Designer("Reko.Gui.Design.ArchitectureDesigner,Reko.Gui")]
    public abstract class ProcessorArchitecture : IProcessorArchitecture
    {
        private RegisterStorage? regStack;

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        /// <param name="services">Object that provides services available in the execution environment.</param>
        /// <param name="archId">Short string identifier for the processor architecture.</param>
        /// <param name="options">A dictionary of architecture options to apply (e.g. processor endianness,
        /// word size, or processor features.)
        /// </param>
#nullable disable
        public ProcessorArchitecture(
            IServiceProvider services,
            string archId, 
            Dictionary<string, object> options)
        {
            this.Services = services;
            this.Name = archId;
            this.Options = options;
            this.MemoryGranularity = 8; // Most architectures are byte-addressable.
            this.DefaultBase = 16;      // Most architectures display hexadecimal.
        }
#nullable enable

        public IServiceProvider Services { get; }
        public string Name { get; }
        public string? Description { get; set; }
        public int DefaultBase { get; set; }
        public EndianServices Endianness { get; protected set; }
        public PrimitiveType FramePointerType { get; protected set; }
        public int MemoryGranularity { get; protected set; }
        public PrimitiveType PointerType { get; protected set; }
        public PrimitiveType WordWidth { get; protected set; }

        public Dictionary<string, object> Options { get; protected set; }

        /// <summary>
        /// The size of the return address (in bytes) if pushed on stack.
        /// </summary>
        /// <remarks>
        /// Size of the return address equals to pointer size on the most of
        /// architectures.
        /// </remarks>
        public virtual int ReturnAddressOnStack => PointerType.Size; //$TODO: deal with near/far calls in x86-realmode
        public int InstructionBitSize { get; protected set; }

        /// <summary>
        /// The stack register used by the architecture.
        /// </summary>
        /// <remarks>
        /// Many architectures reserve a specific register to be used as a stack
        /// pointer register, but not all. ProcessorArchitecture subclasses for 
        /// architectures that have a predefined register must set this property
        /// in their respective constructors. Architectures that don't have a 
        /// predefined register must leave it null and expect the IPlatform
        /// instance to set this property.
        /// </remarks>
        public RegisterStorage StackRegister
        {
            get
            {
                if (this.regStack == null)
                    throw new InvalidOperationException("This architecture has no stack pointer. The platform must define it.");
                return regStack;
            }
            set { this.regStack = value; }
        }

        public RegisterStorage FpuStackRegister { get; protected set; }
        public uint CarryFlagMask { get; protected set; }

        public virtual IAssembler CreateAssembler(string? asmDialect) => throw new NotSupportedException("This architecture doesn't support assembly language.");
        public abstract IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader);
        public Frame CreateFrame() { return new Frame(FramePointerType); }
        public EndianImageReader CreateImageReader(MemoryArea mem, Address addr) => this.Endianness.CreateImageReader(mem, addr);
        public EndianImageReader CreateImageReader(MemoryArea mem, Address addr, long cbUnits) => this.Endianness.CreateImageReader(mem, addr, cbUnits);
        public EndianImageReader CreateImageReader(MemoryArea mem, long offsetBegin, long offsetEnd) => Endianness.CreateImageReader(mem, offsetBegin, offsetEnd);
        public EndianImageReader CreateImageReader(MemoryArea mem, long off) => Endianness.CreateImageReader(mem, off);
        public ImageWriter CreateImageWriter() => Endianness.CreateImageWriter();
        public ImageWriter CreateImageWriter(MemoryArea mem, Address addr) => Endianness.CreateImageWriter(mem, addr);
        public bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value) => Endianness.TryRead(mem, addr, dt, out value);
        public virtual bool TryRead(EndianImageReader rdr, PrimitiveType dt, out Constant value) => rdr.TryRead(dt, out value);
        public abstract IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm);
        public abstract ProcessorState CreateProcessorState();
        public abstract IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags);
        public abstract IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host);
        
        public virtual IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException("Emulation has been implemented for this processor architecture yet.");
        }

        public virtual CallingConvention? GetCallingConvention(string? name)
        {
            // By default, there is no calling convention defined for architectures. Some
            // manufacturers however, define calling conventions.
            return null;
        }

        public abstract RegisterStorage? GetRegister(string name);

        public abstract RegisterStorage? GetRegister(StorageDomain domain, BitRange range);

        public abstract RegisterStorage[] GetRegisters();
        public virtual FlagGroupStorage[] GetFlags() => throw new NotImplementedException("GetFlags not implemented this architecture.");

        public virtual FrameApplicationBuilder CreateFrameApplicationBuilder(
            IStorageBinder binder,
            CallSite site,
            Expression callee)
        {
            return new FrameApplicationBuilder(this, binder, site, callee, false);
        }

        public virtual MemoryArea CreateMemoryArea(Address addr, byte[] bytes)
        {
            // Most CPU's -- but not all -- are byte-addressed.
            return new ByteMemoryArea(addr, bytes);
        }

        /// <summary>
        /// Create a stack access to a variable offset by <paramref name="cbOffsets"/>
        /// from the stack pointer
        /// </summary>
        /// <remarks>
        /// This method is the same for all _sane_ architectures. The crazy madness
        /// of x86 segmented memory accesses is dealt with in that processor's 
        /// implementation of this method.
        /// </remarks>
        /// <param name="bindRegister"></param>
        /// <param name="cbOffset"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public virtual Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
        {
            var sp = binder.EnsureRegister(StackRegister);
            return MemoryAccess.Create(sp, cbOffset, dataType);
        }

        public virtual Expression CreateFpuStackAccess(IStorageBinder binder, int offset, DataType dataType)
        {
            // Only Intel x86/x87 has a FPU stack
            throw new NotSupportedException();
        }

        /// <summary>
        /// For a particular mnemnic, returns its internal (Reko) number.
        /// </summary>
        /// <returns></returns>
        public abstract int? GetMnemonicNumber(string name);

        /// <summary>
        /// Returns a map of mnemonics to their internal (Reko) numbers.
        /// </summary>
        /// <returns></returns>
        public abstract SortedList<string, int> GetMnemonicNames();

        public virtual IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            throw new NotImplementedException($"Your architecture must implement {nameof(GetSubFlags)}.");
        }

        public virtual string RenderInstructionOpcode(MachineInstruction instr, EndianImageReader rdr)
        {
            // Assumes byte granularity.
            var bitSize = this.InstructionBitSize;
            var instrSize = PrimitiveType.CreateWord(bitSize);
            var sb = new StringBuilder();
            var numBase = this.DefaultBase;
            int digits = numBase switch
            {
                16 => (bitSize + 3) / 4,
                8 => (bitSize + 2) / 3,
                _ => throw new NotSupportedException($"Unsupported numeric base {this.DefaultBase}.")
            };
            var units = (instr.Length * this.MemoryGranularity) / this.InstructionBitSize;
            for (int i = 0; i < units; ++i)
            {
                if (rdr.TryRead(instrSize, out var v))
                {
                    sb.Append(Convert.ToString((long) v.ToUInt64(), numBase)
                        .PadLeft(digits, '0'));
                    sb.Append(' ');
                }
            }
            return sb.ToString().ToUpperInvariant();
        }
            
        public abstract bool TryGetRegister(string name, out RegisterStorage reg);
        public abstract FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf);
        public abstract FlagGroupStorage? GetFlagGroup(string name);
        public abstract string GrfToString(RegisterStorage flagRegister, string prefix, uint grf);
        public virtual List<RtlInstruction>? InlineCall(Address addrCallee, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder)
        {
            return null;
        }

        public virtual void LoadUserOptions(Dictionary<string, object>? options) { }
        public abstract Address MakeAddressFromConstant(Constant c, bool codeAlign);
        public virtual Address MakeSegmentedAddress(Constant seg, Constant offset) { throw new NotSupportedException("This architecture doesn't support segmented addresses."); }
        public virtual void PostprocessProgram(Program program) { }
        public abstract Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state);

        public virtual Constant ReinterpretAsFloat(Constant rawBits)
        {
            // Most platforms -- but certainly not all -- use IEEE 754 float representation.
            if (rawBits.DataType.Size == 4)
            {
                return Constant.FloatFromBitpattern(rawBits.ToInt32());
            }
            else if (rawBits.DataType.Size == 8)
            {
                return Constant.FloatFromBitpattern(rawBits.ToInt64());
            }
            throw new NotImplementedException($"Unsupported IEEE floating point size {rawBits.DataType.BitSize}.");
        }

        public virtual Dictionary<string, object>? SaveUserOptions() { return null; }

        public abstract bool TryParseAddress(string? txtAddr, out Address addr);
    }
}
