#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using BitSet = Decompiler.Core.Lib.BitSet;

namespace Decompiler.Core
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
        IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader);

        /// <summary>
        /// Creates an instance of a ProcessorState appropriate for this processor.
        /// </summary>
        /// <returns></returns>
		ProcessorState CreateProcessorState();

        /// <summary>
        /// Creates a BitSet large enough to fit all the registers.
        /// </summary>
        /// <returns></returns>
		BitSet CreateRegisterBitset();

        /// <summary>
        /// Returns a stream of machine-independent instructions, which it generates by successively disassembling
        /// machine-specific instructions and rewriting them into one or more machine-independent RtlInstructions codes. These are then 
        /// returned as clusters of RtlInstructions.
        /// </summary>
        IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host);

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
        IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags);

        /// <summary>
        /// Creates a Frame instance appropriate for this architecture type.
        /// </summary>
        /// <returns></returns>
        Frame CreateFrame();

        /// <summary>
        /// Creates an <see cref="ImageReader" /> with the preferred endianness of the processor.
        /// </summary>
        /// <param name="img">Program image to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <returns>An imagereader of the appropriate endianness</returns>
        ImageReader CreateImageReader(LoadedImage img, Address addr);

        /// <summary>
        /// Creates an <see cref="ImageReader" /> with the preferred endianness of the processor.
        /// </summary>
        /// <param name="img">Program image to read</param>
        /// <param name="addr">offset from the start of the image</param>
        /// <returns>An imagereader of the appropriate endianness</returns>
        ImageReader CreateImageReader(LoadedImage img, ulong off);

        /// <summary>
        /// Creates a procedure serializer that understands the calling conventions used on this
        /// processor.
        /// </summary>
        /// <param name="typeLoader">Used to resolve data types</param>
        /// <param name="defaultConvention">Default calling convention, if none specified.</param>
        ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention);

		RegisterStorage GetRegister(int i);			        // Returns register corresponding to number i.
		RegisterStorage GetRegister(string name);	        // Returns register whose name is 'name'
        bool TryGetRegister(string name, out RegisterStorage reg); // Attempts to find a register with name <paramref>name</paramref>
        FlagGroupStorage GetFlagGroup(uint grf);		    // Returns flag group matching the bitflags.
		FlagGroupStorage GetFlagGroup(string name);
        Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType);
        Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state);

        string GrfToString(uint grf);                       // Converts a union of processor flag bits to its string representation

        PrimitiveType FramePointerType { get; }             // Size of a pointer into the stack frame (near pointer in x86 real mode)
        PrimitiveType PointerType { get; }                  // Pointer size that reaches anywhere in the address space (far pointer in x86 real mode )
		PrimitiveType WordWidth { get; }					// Processor's native word size
        int InstructionBitSize { get; }                     // Instruction "granularity" or alignment.
        RegisterStorage StackRegister { get; }              // Stack pointer used by this machine.
        uint CarryFlagMask { get; }                         // Used when building large adds/subs when carry flag is used.

        /// <summary>
        /// Parses an address according to the preferred base of the architecture.
        /// </summary>
        /// <param name="txtAddr"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        bool TryParseAddress(string txtAddr, out Address addr);

        Address MakeAddressFromConstant(Constant c);
    }
}
