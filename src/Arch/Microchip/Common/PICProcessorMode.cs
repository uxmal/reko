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
using System;
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.Common
{
    using PIC16;
    using PIC18;

    public abstract class PICProcessorMode : IPICProcessorMode
    {
        private static SortedList<InstructionSetID, PICProcessorMode> modes = new SortedList<InstructionSetID, PICProcessorMode>()
            {
                { InstructionSetID.PIC16, new PIC16BasicMode() },
                { InstructionSetID.PIC16_ENHANCED, new PIC16EnhancedMode() },
                { InstructionSetID.PIC16_FULLFEATURED, new PIC16FullMode() },
                { InstructionSetID.PIC18, new PIC18LegacyMode() },
                { InstructionSetID.PIC18_EXTENDED, new PIC18EggMode() },
                { InstructionSetID.PIC18_ENHANCED, new PIC18EnhancedMode() },
            };

        /// <summary>
        /// Specialised default constructor for use only by derived class.
        /// </summary>
        protected PICProcessorMode()
        { }

        /// <summary>
        /// Gets the processor mode corresponding to the given processor name.
        /// </summary>
        /// <param name="procName">Name of the processor.</param>
        /// <returns>
        /// The processor mode.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown the processor name is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown the PIC database is not accessible.</exception>
        public static IPICProcessorMode GetMode(string procName)
        {
            if (string.IsNullOrEmpty(procName))
                throw new ArgumentNullException(nameof(procName));
            var db = PICCrownking.GetDB();
            if (db is null)
                throw new InvalidOperationException("Can't get the PIC database.");
            var pic = db.GetPIC(procName);
            if (pic is null)
                return null;
            if (modes.TryGetValue(pic.GetInstructionSetID, out PICProcessorMode mode))
            {
                mode.PICDescriptor = pic;
                return mode;
            }
            return null;
        }

        /// <summary>
        /// Gets the PIC descriptor.
        /// </summary>
        public PIC PICDescriptor { get; private set; }
        
        /// <summary>
        /// Gets the PIC name.
        /// </summary>
        string IPICProcessorMode.PICName => PICDescriptor?.Name;

        /// <summary>
        /// Creates a disassembler for the target processor.
        /// </summary>
        /// <param name="arch">The architecture of the processor.</param>
        /// <param name="rdr">The memory image reader.</param>
        /// <returns>
        /// The new disassembler.
        /// </returns>
        public abstract PICDisassemblerBase CreateDisassembler(PICArchitecture arch, EndianImageReader rdr);

        /// <summary>
        /// Creates the registers for the PIC.
        /// </summary>
        /// <param name="pic">The PIC descriptor.</param>
        public abstract void CreateRegisters();

        /// <summary>
        /// Creates the memory descriptor for the PIC memory.
        /// </summary>
        public abstract void CreateMemoryDescriptor();

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
        public abstract PICRewriter CreateRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host);

        /// <summary>
        /// Creates the processor state.
        /// </summary>
        /// <param name="arch">The architecture of the processor.</param>
        /// <returns>
        /// The new processor state.
        /// </returns>
        public abstract PICProcessorState CreateProcessorState(PICArchitecture arch);

        /// <summary>
        /// Makes memory address from constant value.
        /// </summary>
        /// <param name="c">A Constant to process.</param>
        /// <returns>
        /// The memory Address.
        /// </returns>
        public abstract Address MakeAddressFromConstant(Constant c);

        /// <summary>
        /// Creates a memory banked address.
        /// </summary>
        /// <param name="bsrReg">The Bank Select register value.</param>
        /// <param name="offset">The offset in the memory bank.</param>
        /// <returns>
        /// The new banked address.
        /// </returns>
        public abstract Address CreateBankedAddress(byte bsrReg, byte offset);

        /// <summary>
        /// Creates the PIC memory pointer scanner.
        /// </summary>
        /// <param name="rdr">The memory image reader.</param>
        /// <param name="knownLinAddresses">The known linear addresses.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// The new pointer scanner.
        /// </returns>
        public abstract PICPointerScanner CreatePointerScanner(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags);

    }

}
