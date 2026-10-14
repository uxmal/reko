#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Arch.Rca.Analysis;
using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Arch.Rca;

public class Rca1802Architecture : ProcessorArchitecture
{
    public Rca1802Architecture(IServiceProvider services, string name, Dictionary<string,object> options) 
        : base(services, "rca1802", options, Registers.RegisterBank)
    {
        this.Endianness = EndianServices.Big;  // No explicit endianness.
        this.WordWidth = PrimitiveType.Byte;
        this.PointerType = PrimitiveType.Ptr16;
        this.MemoryGranularity = 8;
        this.CodeMemoryGranularity = 8;
        this.FramePointerType = PrimitiveType.Ptr16;
        this.InstructionBitSize = 8;
        this.StackRegister = Registers.GpRegisters[2];
        this.WordWidth = PrimitiveType.Byte;
    }

    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
    {
        return new Rca1802Disassembler(this, imageReader);
    }

    public override T? CreateExtension<T>() where T : class
    {
        if (typeof(IAnalysisFactory).IsAssignableFrom(typeof(T)))
            return (T) (object) new Rca1802AnalysisFactory();
        return default;
    }

    public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
    {
        throw new NotImplementedException();
    }

    public override ProcessorState CreateProcessorState()
    {
        return new DefaultProcessorState(this);
    }

    public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        return new Rca1802Rewriter(this, rdr, state, binder, host);

    }

    public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, ulong grf)
    {
        if (flagRegister != Registers.Status)
            return null;
        return (grf & Registers.DF.FlagGroupBits) != 0 ? Registers.DF : null;
    }

    public override FlagGroupStorage? GetFlagGroup(string name)
    {
        throw new NotImplementedException();
    }

    public override SortedList<string, int> GetMnemonicNames()
    {
        throw new NotImplementedException();
    }

    public override int? GetMnemonicNumber(string name)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
    {
        if (flags is null || flags.FlagRegister != Registers.Status)
            yield break;
        if ((flags.FlagGroupBits & Registers.DF.FlagGroupBits) != 0)
            yield return Registers.DF;
    }

    public override string GrfToString(RegisterStorage flagRegister, string prefix, ulong grf)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The standard convention on RCA1802 is that the X register is set to
    /// 2 (indicating the R2 stack register) on entry to a procedure.
    /// </remarks>
    public override void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter m)
    {
        var x = proc.Frame.EnsureRegister(Registers.X);
        m.Assign(x, 2);
    }

    public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
    {
        return Address.Ptr16(c.ToUInt16());
    }

    public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
    {
        throw new NotImplementedException();
    }

    public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
    {
        return Address.TryParse16(txtAddr, out addr);
    }
}
/*
To enable interoperability between different programs, subroutines, and the operating system/monitor
(such as RCA's UT4 or standard software routines), RCA established the Standard Call and Return Technique (SCRT).

Hardware-Defined Registers

The hardware hardwires specific functions to only a few registers during start-up or interrupts:
• R0: Automatically serves as the Program Counter (PC) upon a hardware reset. It is also hardwired
  as the memory pointer for DMA operations (used on the Elf to load bytes from the front panel switches).
• R1: Automatically targeted as the Program Counter (PC) when a hardware interrupt is triggered.
• R2: Automatically targeted as the Stack Pointer (SP) or data save pointer during an interrupt.

 
RCA / SCRT Standard Register Conventions

When writing structured assembly or using standard subroutines on the COSMAC Elf, the following register allocations are universally expected:
Register	Standard Name / Function	Description
R0	IDLE / DMA / PC	Main Program Counter at boot. Serves as the DMA pointer for front-panel IO.
R1	INT_PC	Interrupt Program Counter.
R2	SP	    Standard Call and Return Stack Pointer.
R3	PC	    Main Program Counter for user applications once initialization finishes.
R4	CALL	Call Subroutine Program Counter (points to the software CALL execution handler).
R5	RETURN	Return Subroutine Program Counter (points to the software RETURN execution handler).
R6	LINK	Link register / Temporary pointer used by the SCRT handlers.
R7	ARG	    Argument/Parameter pointer passed into subroutines.
R8 to RE	DATA / POINTER	General-purpose data registers, indices, or variables.
RF	MEM / UTILITY	General-purpose workspace pointer, often used by monitor utilities. 
 
 */