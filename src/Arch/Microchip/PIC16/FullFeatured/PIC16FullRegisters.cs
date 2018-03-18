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
    /// This class implements the Full-featured PIC16 registers pool.
    /// </summary>
    public class PIC16FullRegisters : PIC16Registers
    {

        /// <summary>
        /// Private constructor.
        /// </summary>
        private PIC16FullRegisters()
        {
        }

        /// <summary>
        /// Creates the Full-Featured PIC16 registers.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pic"/> is null.</exception>
        public static void Create(PIC pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            PICRegisters.LoadRegisters(pic);
            new PIC16FullRegisters().SetCoreRegisters();
        }


        #region Full-featured PIC16 standard (core) registers and bit fields. 

        /// <summary>
        /// INDF0 special function register.
        /// </summary>
        public static PICRegisterStorage INDF0 { get; private set; }

        /// <summary>
        /// INDF1 special function register.
        /// </summary>
        public static PICRegisterStorage INDF1 { get; private set; }

        /// <summary>
        /// FSR0L special function register.
        /// </summary>
        public static PICRegisterStorage FSR0L { get; private set; }

        /// <summary>
        /// FSR0H special function register.
        /// </summary>
        public static PICRegisterStorage FSR0H { get; private set; }

        /// <summary>
        /// FSR1L special function register.
        /// </summary>
        public static PICRegisterStorage FSR1L { get; private set; }

        /// <summary>
        /// FSR1H special function register.
        /// </summary>
        public static PICRegisterStorage FSR1H { get; private set; }

        /// <summary>
        /// FSR0 pseudo-register (alias to FSR0H:FSR0L).
        /// </summary>
        public static PICRegisterStorage FSR0 { get; private set; }

        /// <summary>
        /// FSR1 pseudo-register (alias to FSR1H:FSR1L).
        /// </summary>
        public static PICRegisterStorage FSR1 { get; private set; }

        /// <summary>
        /// BSR special function register.
        /// </summary>
        public static PICRegisterStorage BSR { get; protected set; }


        public override void SetCoreRegisters()
        {

            // *True* PIC registers

            INDF0 = PICRegisters.GetRegister("INDF0");
            INDF1 = PICRegisters.GetRegister("INDF1");
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

            FSR0L = PICRegisters.GetRegister("FSR0L");
            FSR0H = PICRegisters.GetRegister("FSR0H");
            FSR1L = PICRegisters.GetRegister("FSR1L");
            FSR1H = PICRegisters.GetRegister("FSR1H");
            FSR0 = PICRegisters.GetRegister("FSR0");
            FSR1 = PICRegisters.GetRegister("FSR1");

            BSR = PICRegisters.GetRegister("BSR");
            WREG = PICRegisters.GetRegister("WREG");
            STKPTR = PICRegisters.GetRegister("STKPTR");
            PCLATH = PICRegisters.GetRegister("PCLATH");
            INTCON = PICRegisters.GetRegister("INTCON");
        }

        #endregion

    }

}
