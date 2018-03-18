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

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    /// <summary>
    /// This class implements the Basic PIC16 registers pool.
    /// </summary>
    public class PIC16BasicRegisters : PIC16Registers
    {

        /// <summary>
        /// Private constructor.
        /// </summary>
        private PIC16BasicRegisters()
        {
        }

        /// <summary>
        /// Creates the Basic PIC16 registers.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pic"/> is null.</exception>
        public static void Create(PIC pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            PICRegisters.LoadRegisters(pic);
            new PIC16BasicRegisters().SetCoreRegisters();
        }

        /// <summary> Register Page in STATUS register. </summary>
        public static PICBitFieldStorage RP { get; protected set; }

        /// <summary> INDF special function register. </summary>
        public static PICRegisterStorage INDF { get; private set; }

        /// <summary> FSR pseudo-register (alias to FSRH:FSRL). </summary>
        public static PICRegisterStorage FSR { get; private set; }

        public override void SetCoreRegisters()
        {

            INDF = PICRegisters.GetRegister("INDF");
            PCL = PICRegisters.GetRegister("PCL");

            STATUS = PICRegisters.GetRegister("STATUS");
            C = PICRegisters.GetBitField("C");
            DC = PICRegisters.GetBitField("DC");
            Z = PICRegisters.GetBitField("Z");
            if (!PICRegisters.TryGetBitField("nPD", out var pd))
            {
                PICRegisters.TryGetBitField("PD", out pd);
            }
            PD = pd;
            if (!PICRegisters.TryGetBitField("nTO", out var to))
            {
                PICRegisters.TryGetBitField("TO", out to);
            }
            TO = to;
            RP = PICRegisters.GetBitField("RP");
            FSR = PICRegisters.GetRegister("FSR");

            WREG = PICRegisters.GetRegister("WREG");
            STKPTR = PICRegisters.GetRegister("STKPTR");
            PCLATH = PICRegisters.GetRegister("PCLATH");
            INTCON = PICRegisters.GetRegister("INTCON");
        }

    }

}
