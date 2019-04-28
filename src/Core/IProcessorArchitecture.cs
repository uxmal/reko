#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Reko.Core
{
    /// <summary>
    /// Abstraction of a CPU architecture.
    /// </summary>
	public interface IProcessorArchitecture
	{
        /// <summary>
        /// Creates an IEnumerable of disassembled MachineInstructions which consumes 
        /// its input from the provided <paramref name="imageReader"/>.
        /// </summary>
        /// <remarks>The IEnumerable lets us use Linq expressions
        /// like Take() on a stream of disassembled instructions.</remarks>
        /// <param name="imageReader"></param>
        /// <returns></returns>
        IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader);

        /// <summary>
        /// Creates an instance of a ProcessorState appropriate for this processor.
        /// </summary>
        /// <param name="map">Segment map with descriptions of segments</param>
        /// <returns></returns>
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
        /// <param name="img">Program image to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <returns>An <seealso cref="ImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea img, Address addr);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred 
        /// endianness of the processor, limited to the specified address range.
        /// </summary>
        /// <param name="img">Program image to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <returns>An <seealso cref="ImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea memoryArea, Address addrBegin, Address addrEnd);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor.
        /// </summary>
        /// <param name="img">Program image to read</param>
        /// <param name="addr">offset from the start of the image</param>
        /// <returns>An <seealso cref="ImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea img, ulong off);

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

        /// <summary>
        /// Creates a comparer that compares instructions for equality. 
        /// Normalization means some attributes of the instruction are 
        /// treated as wildcards.
        /// </summary>
        /// <param name="norm"></param>
        /// <returns></returns>
        IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm);

        /// <summary>
        /// Given a register <paramref name="reg"/>, retrieves all architectural
        /// registers that overlap all or part of it.
        /// </summary>
        /// <param name="reg">Register whose alias we're interested in.</param>
        /// <returns>A sequence of aliases.</returns>
        IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg);

        /// <summary>
        /// Returns a list of all the available opcodes as strings.
        /// </summary>
        SortedList<string, int> GetOpcodeNames();           // Returns all the processor opcode names and their internal Reko numbers.
        
        /// <summary>
        /// Returns an internal Reko opcode for a given instruction name, or
        /// null if none is available.
        /// </summary>
        int? GetOpcodeNumber(string name);
        
        /// <summary>
        /// Returns register corresponding to number i, or null if there is no
        /// such register.
        /// </summary>
        RegisterStorage GetRegister(int i);

        /// <summary>
        /// Returns register whose name is 'name'
        /// </summary>
        RegisterStorage GetRegister(string name);

        /// <summary>
        /// Given a register, returns any sub register occupying the 
        /// given bit range.
        /// </summary>
        /// <param name="reg">Register to examine</param>
        /// <param name="offset">Bit offset of expected subregister.</param>
        /// <param name="width">Bit size of subregister.</param>
        /// <returns></returns>
        RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width);
        
        /// <summary>
        /// Given a set, removes any aliases of reg from the set.
        /// </summary>
        void RemoveAliases(ISet<RegisterStorage> ids, RegisterStorage reg); 

        /// <summary>
        /// Find the widest sub-register that covers the register reg.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
        RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> regs);

        /// <summary>
        /// Returns all registers of this architecture.
        /// </summary>
        /// <returns></returns>
        RegisterStorage[] GetRegisters(); 
        bool TryGetRegister(string name, out RegisterStorage reg); // Attempts to find a register with name <paramref>name</paramref>
        FlagGroupStorage GetFlagGroup(uint grf);		    // Returns flag group matching the bit flags.
		FlagGroupStorage GetFlagGroup(string name);
        Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType);

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
        List<RtlInstruction> InlineCall(Address addrCallee, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder);

        Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state);
        Address MakeSegmentedAddress(Constant seg, Constant offset);

        string GrfToString(uint grf);                       // Converts a union of processor flag bits to its string representation

        string Name { get; }                                // Short name used to refer to an architecture.
        string Description { get; set; }                    // Longer description used to refer to architecture. Typically loaded from app.config
        PrimitiveType FramePointerType { get; }             // Size of a pointer into the stack frame (near pointer in x86 real mode)
        PrimitiveType PointerType { get; }                  // Pointer size that reaches anywhere in the address space (far pointer in x86 real mode )
		PrimitiveType WordWidth { get; }					// Processor's native word size
        int InstructionBitSize { get; }                     // Instruction "granularity" or alignment.
        RegisterStorage StackRegister { get; set;  }        // Stack pointer used by this machine.
        uint CarryFlagMask { get; }                         // Used when building large adds/subs when carry flag is used.

        /// <summary>
        /// Parses an address according to the preferred base of the architecture.
        /// </summary>
        /// <param name="txtAddr"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        bool TryParseAddress(string txtAddr, out Address addr);

        /// <summary>
        /// Given a constant, returns an Address of the correct size for this architecture.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>An address.</returns>
        Address MakeAddressFromConstant(Constant c);

        /// <summary>
        /// After the program has been loaded, the architecture is given a final
        /// chance to mutate the SegmentMap or any other property.
        /// </summary>
        /// <param name="program">The program to postprocess.</param>
        void PostprocessProgram(Program program);
        
        /// <summary>
        /// Reads a value from memory, respecting the processor's endianness. Use this
        /// instead of ImageWriter when random access of memory is requored.
        /// </summary>
        /// <param name="mem">Memory area to read from</param>
        /// <param name="addr">Address to read from</param>
        /// <param name="dt">Data type of the data to be read</param>
        /// <param name="value">The value read from memory, if successful.</param>
        /// <returns>True if the read succeeded, false if the address was out of range.</returns>
        bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value);

        /// <summary>
        /// The dictionary contains options that were loaded from the 
        /// configuration file or the executable image. These can be used
        /// to customize the properties of the processor.
        /// </summary>
        /// <param name="options"></param>
        void LoadUserOptions(Dictionary<string, object> options);

        Dictionary<string, object> SaveUserOptions();

        /// <summary>
        /// Provide an architecture-defined CallingConvention.
        /// </summary>
        CallingConvention GetCallingConvention(string ccName);
    }

    /// <summary>
    /// Normalize enumeration controls the operation of instruction comparer. 
    /// </summary>
    [Flags]
    public enum Normalize
    {
        Nothing,        // Match identically
        Constants,      // all constants treated as wildcards
        Registers,      // all registers treated as wildcards.
    }

    /// <summary>
    /// Abstract base class from which most IProcessorArchitecture 
    /// implementations derive.
    /// </summary>
    [Designer("Reko.Gui.Design.ArchitectureDesigner,Reko.Gui")]
    public abstract class ProcessorArchitecture : IProcessorArchitecture
    {
        private RegisterStorage regStack;

        public ProcessorArchitecture(string archId)
        {
            this.Name = archId;
        }

        public string Name { get; }
        public string Description { get; set; }
        public PrimitiveType FramePointerType { get; protected set; }
        public PrimitiveType PointerType { get; protected set; }
        public PrimitiveType WordWidth { get; protected set; }
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

        public uint CarryFlagMask { get; protected set; }

        public abstract IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader);
        public Frame CreateFrame() { return new Frame(FramePointerType); }
        public abstract EndianImageReader CreateImageReader(MemoryArea img, Address addr);
        public abstract EndianImageReader CreateImageReader(MemoryArea img, Address addrBegin, Address addrEnd);
        public abstract EndianImageReader CreateImageReader(MemoryArea img, ulong off);
        public abstract ImageWriter CreateImageWriter();
        public abstract ImageWriter CreateImageWriter(MemoryArea img, Address addr);
        public abstract IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm);
        public abstract ProcessorState CreateProcessorState();
        public abstract IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags);
        public abstract IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host);

        public virtual IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg) { yield return reg; }

        public virtual CallingConvention GetCallingConvention(string name)
        {
            // By default, there is no calling convention defined for architectures. Some
            // manufacturers however, define calling conventions.
            return null;
        }

        public abstract RegisterStorage GetRegister(int i);
        public abstract RegisterStorage GetRegister(string name);
        public abstract RegisterStorage[] GetRegisters();

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

        /// <summary>
        /// For a particular opcode name, returns its internal (Reko) number.
        /// </summary>
        /// <returns></returns>
        public abstract int? GetOpcodeNumber(string name);

        /// <summary>
        /// Returns a map of opcode names to their internal (Reko) numbers.
        /// </summary>
        /// <returns></returns>
        public abstract SortedList<string, int> GetOpcodeNames();

        /// <summary>
        /// Get the improper sub-register of <paramref name="reg"/> that starts
        /// at offset <paramref name="offset"/> and is of size 
        /// <paramref name="width"/>.
        /// </summary>
        /// <remarks>
        /// Most architectures not have sub-registers, and will use this 
        /// default implementation. This method is overridden for 
        /// architectures like x86 and Z80, where sub-registers <code>(ah, al, etc)</code>
        /// do exist.
        /// </remarks>
        /// <param name="reg"></param>
        /// <param name="offset"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public virtual RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            return (offset == 0 && reg.BitSize == (ulong)width) ? reg : null;
        }

        public virtual RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> regs) { return (regs.Contains(reg)) ? reg : null; }
        public virtual void RemoveAliases(ISet<RegisterStorage> ids, RegisterStorage reg) { ids.Remove(reg); }

        public abstract bool TryGetRegister(string name, out RegisterStorage reg);
        public abstract FlagGroupStorage GetFlagGroup(uint grf);
        public abstract FlagGroupStorage GetFlagGroup(string name);
        public abstract string GrfToString(uint grf);
        public virtual List<RtlInstruction> InlineCall(Address addrCallee, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder)
        {
            return null;
        }

        public virtual void LoadUserOptions(Dictionary<string, object> options) { }
        public abstract Address MakeAddressFromConstant(Constant c);
        public virtual Address MakeSegmentedAddress(Constant seg, Constant offset) { throw new NotSupportedException("This architecture doesn't support segmented addresses."); }
        public virtual void PostprocessProgram(Program program) { }
        public abstract Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state);
        public virtual Dictionary<string, object> SaveUserOptions() { return null; }

        public abstract bool TryParseAddress(string txtAddr, out Address addr);
        public abstract bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value);
    }

}
