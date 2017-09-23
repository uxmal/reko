#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        /// <remarks>This was previously an IEnumerator, but making it IEnumerable lets us use Linq expressions
        /// like Take().</remarks>
        /// <param name="imageReader"></param>
        /// <returns></returns>
        IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader);

        /// <summary>
        /// Creates an instance of a ProcessorState appropriate for this
        /// processor.
        /// </summary>
        /// <returns></returns>
		ProcessorState CreateProcessorState();

        /// <summary>
        /// Returns a stream of machine-independent instructions, which it
        /// generates by successively disassembling machine-specific
        /// instructions and rewriting them into one or more machine-
        /// independent RtlInstructions codes. These are then returned as
        /// clusters of RtlInstructions.
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
        /// <returns>An imagereader of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea img, Address addr);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred 
        /// endianness of the processor, limited to the specified address
        /// range.
        /// </summary>
        /// <param name="img">Program image to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <returns>An imagereader of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea memoryArea, Address addrBegin, Address addrEnd);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor.
        /// </summary>
        /// <param name="img">Program image to read</param>
        /// <param name="addr">offset from the start of the image</param>
        /// <returns>An imagereader of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea img, ulong off);

        /// <summary>
        /// Creates an <see cref="ImageWriter" /> with the preferred 
        /// endianness of the processor.
        /// </summary>
        /// <returns></returns>
        ImageWriter CreateImageWriter();
        ImageWriter CreateImageWriter(MemoryArea memoryArea, Address addr);

        /// <summary>
        /// Creates a comparer that compares instructions for equality. 
        /// Normalization means some attributes of the instruction are 
        /// trated as wildcards.
        /// </summary>
        /// <param name="norm"></param>
        /// <returns></returns>
        IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm);

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

        IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg);

        /// <summary>
        /// Returns a list of all the available opcodes.
        /// </summary>
        /// <returns></returns>
        SortedList<string, int> GetOpcodeNames();           // Returns all the processor opcode names and their internal Reko numbers.
        int? GetOpcodeNumber(string name);                  // Returns an internal Reko opcode for an instruction, or null if none is available.
        RegisterStorage GetRegister(int i);                 // Returns register corresponding to number i.
        RegisterStorage GetRegister(string name);           // Returns register whose name is 'name'

        RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width);
        void RemoveAliases(ISet<RegisterStorage> ids, RegisterStorage reg);  // Removes any aliases of reg from the set

        /// <summary>
        /// Find the widest subregister that covers the register reg.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
        RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> regs);

        RegisterStorage GetPart(RegisterStorage reg, DataType width);
        RegisterStorage[] GetRegisters();                   // Returns all registers of this architecture.
        bool TryGetRegister(string name, out RegisterStorage reg); // Attempts to find a register with name <paramref>name</paramref>
        FlagGroupStorage GetFlagGroup(uint grf);		    // Returns flag group matching the bitflags.
		FlagGroupStorage GetFlagGroup(string name);
        Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType);
        //$REFACTOR: this should probably live in FrameApplicationBuilder instead.
        Expression CreateFpuStackAccess(IStorageBinder binder, int offset, DataType dataType);  //$REVIEW: generalize these two methods?
        Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state);
        Address MakeSegmentedAddress(Constant seg, Constant offset);

        string GrfToString(uint grf);                       // Converts a union of processor flag bits to its string representation

        string Name { get; set; }                           // Short name used to refer to an architecture.
        string Description { get; set; }                    // Longer description used to refer to architecture. Typically loaded from app.config
        PrimitiveType FramePointerType { get; }             // Size of a pointer into the stack frame (near pointer in x86 real mode)
        PrimitiveType PointerType { get; }                  // Pointer size that reaches anywhere in the address space (far pointer in x86 real mode )
		PrimitiveType WordWidth { get; }					// Processor's native word size
        int InstructionBitSize { get; }                     // Instruction "granularity" or alignment.
        RegisterStorage StackRegister { get; set;  }        // Stack pointer used by this machine.
        RegisterStorage FpuStackRegister { get; }           // FPU stack pointer used by this machine, or null if none exists.
        uint CarryFlagMask { get; }                         // Used when building large adds/subs when carry flag is used.

        /// <summary>
        /// Parses an address according to the preferred base of the 
        /// architecture.
        /// </summary>
        /// <param name="txtAddr"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        bool TryParseAddress(string txtAddr, out Address addr);

        Address MakeAddressFromConstant(Constant c);

        /// <summary>
        /// The dictionary contains options that were loaded from the config file or the executable image. These can be used
        /// to customize the properties of the processor.
        /// </summary>
        /// <param name="options"></param>
        void LoadUserOptions(Dictionary<string, object> options);

        Dictionary<string, object> SaveUserOptions();
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

    [Designer("Reko.Gui.Design.ArchitectureDesigner,Reko.Gui")]
    public abstract class ProcessorArchitecture : IProcessorArchitecture
    {
        public string Name { get; set; }
        public string Description {get; set; }
        public PrimitiveType FramePointerType { get; protected set; }
        public PrimitiveType PointerType { get; protected set; }
        public PrimitiveType WordWidth { get; protected set; }
        public int InstructionBitSize { get; protected set; }
        public RegisterStorage StackRegister { get; set; }
        public RegisterStorage FpuStackRegister { get; protected set; }
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
        public abstract RegisterStorage GetRegister(int i);
        public abstract RegisterStorage GetRegister(string name);
        public abstract RegisterStorage[] GetRegisters();

        public virtual FrameApplicationBuilder CreateFrameApplicationBuilder(
            IStorageBinder binder,
            CallSite site,
            Expression callee)
        {
            return new FrameApplicationBuilder(this, binder, site, callee, false);
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
        /// Get the improper subregister of <paramref name="reg"/> that starts
        /// at offset <paramref name="offset"/> and is of size 
        /// <paramref name="width"/>.
        /// </summary>
        /// <remarks>
        /// Most architectures not have subregisters, and will use this 
        /// default implementation. This method is overridden for 
        /// architectures like x86 and Z80, where subregisters (ah al etc)
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

        public virtual RegisterStorage GetPart(RegisterStorage reg, DataType dt)
        {
            return GetSubregister(reg, 0, dt.BitSize);
        }

        public virtual RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> regs) { return (regs.Contains(reg)) ? reg : null; }
        public virtual void RemoveAliases(ISet<RegisterStorage> ids, RegisterStorage reg) { ids.Remove(reg); }

        public abstract bool TryGetRegister(string name, out RegisterStorage reg);
        public abstract FlagGroupStorage GetFlagGroup(uint grf);
        public abstract FlagGroupStorage GetFlagGroup(string name);
        public abstract string GrfToString(uint grf);
        public virtual void LoadUserOptions(Dictionary<string, object> options) { }
        public abstract Address MakeAddressFromConstant(Constant c);
        public virtual Address MakeSegmentedAddress(Constant seg, Constant offset) { throw new NotSupportedException("This architecture doesn't support segmented addresses."); }
        public abstract Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state);
        public virtual Dictionary<string, object> SaveUserOptions() { return null; }

        public abstract bool TryParseAddress(string txtAddr, out Address addr);
    }
}
