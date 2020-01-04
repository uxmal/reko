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

using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;

    public class PIC18EggRegisters : PIC18Registers
    {

        private PIC18EggRegisters()
        {
        }

        /// <summary>
        /// Creates the Extended PIC18 registers.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pic"/> is null.</exception>
        public static void Create(IPICDescriptor pic)
        {
            LoadRegisters(pic ?? throw new ArgumentNullException(nameof(pic)));
            var regs = new PIC18EggRegisters();
            regs.SetCoreRegisters();
            regs.SetRegistersValuesAtPOR();
        }

        /// <summary>
        /// This method sets each of the standard "core" registers of the "Egg" PIC18.
        /// They are retrieved from the registers symbol table which has been previously populated by loading the PIC definition as provided by Microchip.
        /// </summary>
        /// <remarks>
        /// This permits to still get a direct reference to standard registers and keeps having some flexibility on definitions.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if a register cannot be found in the symbol table.</exception>
        protected override void SetCoreRegisters()
        {

            base.SetCoreRegisters();

            // Shadow registers (if they exist).

            STATUS_CSHAD = PICRegisterStorage.None;
            WREG_CSHAD = PICRegisterStorage.None;
            BSR_CSHAD = PICRegisterStorage.None;
            if (TryGetRegister("STATUS_CSHAD", out var sta))
                STATUS_CSHAD = sta;
            if (TryGetRegister("WREG_CSHAD", out var wre))
                WREG_CSHAD = wre;
            if (TryGetRegister("BSR_CSHAD", out var bsr))
                BSR_CSHAD = bsr;

        }

        /// <summary>
        /// Registers values at Power-On Reset time for "Egg" PIC18.
        /// </summary>
        protected override void SetRegistersValuesAtPOR()
            => base.SetRegistersValuesAtPOR();
    }

}
