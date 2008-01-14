/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core.Types;
using System;
using System.IO;

namespace Decompiler.Core
{
	using BitSet = Decompiler.Core.Lib.BitSet;

	public interface IProcessorArchitecture
	{
		Assembler CreateAssembler();
		Disassembler CreateDisassembler(ProgramImage img, Address addr);
		Dumper CreateDumper();
		ProcessorState CreateProcessorState();
		BackWalker CreateBackWalker(ProgramImage img);
		CodeWalker CreateCodeWalker(ProgramImage img, Platform platform, Address addr, ProcessorState st, ICodeWalkerListener list);
		BitSet CreateRegisterBitset();
		Rewriter CreateRewriter(IProcedureRewriter prw, Procedure proc, IRewriterHost host, CodeEmitter emitter);

		MachineRegister GetRegister(int i);			// Returns register corresponding to number i.
		MachineRegister GetRegister(string name);	// Returns register whose name is 'name'
		MachineFlags GetFlagGroup(uint grf);		// Returns flag group matching the bitflags.
		MachineFlags GetFlagGroup(string name);	

		/// <summary>
		/// A bitset that represents those registers that may never be used as arguments to a procedure. 
		/// </summary>
		/// <remarks>
		/// Typically, the stack pointer register is one of these registers. Some architectures define
		/// global registers that are preserved across calls; these should also be present in this set.
		/// </remarks>
		BitSet ImplicitParameterRegisters { get; }

		string GrfToString(uint grf);

		PrimitiveType WordWidth { get; }					// Processor's native word size
	}
}
