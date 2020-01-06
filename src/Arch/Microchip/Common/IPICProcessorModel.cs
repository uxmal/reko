#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Libraries.Microchip;
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// Interface for PIC processor model.
    /// </summary>
    public interface IPICProcessorModel
    {
        /// <summary>
        /// Gets the PIC descriptor as defined by Microchip.
        /// </summary>
        IPICDescriptor PICDescriptor { get; }

        /// <summary>
        /// Gets the PIC name.
        /// </summary>
        string PICName { get; }

        /// <summary>
        /// Creates a disassembler for the target processor.
        /// </summary>
        /// <param name="arch">The architecture of the processor.</param>
        /// <param name="rdr">The memory image reader.</param>
        /// <returns>
        /// The new disassembler.
        /// </returns>
        PICDisassemblerBase CreateDisassembler(PICArchitecture arch, EndianImageReader rdr);

        /// <summary>
        /// Creates an emulator for the target processor.
        /// </summary>
        /// <param name="arch">The architecture of the processor.</param>
        /// <param name="segmentMap">The memory of the program to be emulated.</param>
        /// <param name="envEmulator">Emulated environment.</param>
        /// <returns>The created emulator.</returns>
        IProcessorEmulator CreateEmulator(PICArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator);

        /// <summary>
        /// Creates the registers for the PIC.
        /// </summary>
        /// <param name="pic">The PIC descriptor.</param>
        void CreateRegisters();

        /// <summary>
        /// Creates the memory descriptor for the PIC memory.
        /// </summary>
        void CreateMemoryDescriptor();

        /// <summary>
        /// Creates the instructions IL rewriter for the target processor.
        /// </summary>
        /// <param name="arch">The architecture of the processor.</param>
        /// <param name="dasm">The disassembler.</param>
        /// <param name="state">The processor state.</param>
        /// <param name="binder">The storage binder.</param>
        /// <param name="host">The host.</param>
        /// <returns>
        /// The new rewriter.
        /// </returns>
        PICRewriter CreateRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host);

        /// <summary>
        /// Creates the processor state.
        /// </summary>
        /// <param name="arch">The architecture of the processor.</param>
        /// <returns>
        /// The new processor state.
        /// </returns>
        PICProcessorState CreateProcessorState(PICArchitecture arch);

        /// <summary>
        /// Makes memory address from constant value.
        /// </summary>
        /// <param name="c">A Constant to process.</param>
        /// <returns>
        /// The memory Address.
        /// </returns>
        Address MakeAddressFromConstant(Constant c);

        /// <summary>
        /// Creates a memory banked address.
        /// </summary>
        /// <param name="bsrReg">The Bank Select register value.</param>
        /// <param name="offset">The offset in the memory bank.</param>
        Address CreateBankedAddress(byte bsrReg, byte offset);

        /// <summary>
        /// Creates the memory pointer scanner.
        /// </summary>
        /// <param name="rdr">The memory image reader.</param>
        /// <param name="knownLinAddresses">The known linear addresses.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// The new pointer scanner.
        /// </returns>
        PICPointerScanner CreatePointerScanner(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags);

        /// <summary>
        /// Postprocess the program image which has been loaded.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="arch">The architecture of the processor.</param>
        void PostprocessProgram(Program program, PICArchitecture arch);
    }
}
