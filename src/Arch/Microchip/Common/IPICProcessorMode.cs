#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// Interface for PIC processor mode builders.
    /// </summary>
    public interface IPICProcessorMode
    {
        /// <summary>
        /// Gets the PIC descriptor.
        /// </summary>
        PIC PICDescriptor { get; }

        /// <summary>
        /// Gets the PIC name.
        /// </summary>
        string PICName { get; }

        /// <summary>
        /// Gets the identifier name of the processor architecture.
        /// </summary>
        string ArchitectureID { get; }

        /// <summary>
        /// Creates the PIC architecture.
        /// </summary>
        /// <param name="mode">The processor mode.</param>
        /// <returns>
        /// The new architecture.
        /// </returns>
        PICArchitecture CreateArchitecture();

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
        /// Creates the registers for the PIC.
        /// </summary>
        /// <param name="pic">The PIC descriptor.</param>
        void CreateRegisters();

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

    }

}
