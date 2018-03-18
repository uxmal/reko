#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
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

using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.PIC18
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
        public static void Create(PIC pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            PICRegisters.LoadRegisters(pic);
            new PIC18EggRegisters().SetCoreRegisters();
        }

        /// <summary>
        /// This method sets each of the standard "core" registers of the PIC18.
        /// They are retrieved from the registers symbol table which has been previously populated by loading the PIC definition.
        /// </summary>
        /// <remarks>
        /// This permits to still get a direct reference to standard registers and keeps having some flexibility on definitions.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if a register cannot be found in the symbol table.</exception>
        public override void SetCoreRegisters()
        {

            base.SetCoreRegisters();

            // Shadow registers (if they exist).

            if (PICRegisters.TryGetRegister("STATUS_CSHAD", out var sta, PICRegisterStorage.None))
                STATUS_CSHAD = sta;
            if (PICRegisters.TryGetRegister("WREG_CSHAD", out var wre, PICRegisterStorage.None))
                WREG_CSHAD = wre;
            if (PICRegisters.TryGetRegister("BSR_CSHAD", out var bsr, PICRegisterStorage.None))
                BSR_CSHAD = bsr;

        }

    }

}
