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
    /// This class implements the PIC16-ENhanced registers pool.
    /// </summary>
    public class PIC16ERegisters : PICRegisters
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pic">The PIC definition descriptor.</param>
        private PIC16ERegisters(PIC pic) : base(pic)
        {
        }

        /// <summary>
        /// Creates a new <see cref="PIC16ERegisters"/> instance.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <returns>
        /// A <see cref="PICRegistersBuilder"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pic"/> is null.</exception>
        public static PICRegisters Create(PIC pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            registers = new PIC16ERegisters(pic);
            return registers;
        }

        #endregion

        #region PIC16-Enhanced standard (core) registers and bit fields. 

        /// <summary>
        /// INDF0 special function register.
        /// </summary>
        public static PICRegisterStorage INDF0 { get; private set; }

        /// <summary>
        /// INDF1 special function register.
        /// </summary>
        public static PICRegisterStorage INDF1 { get; private set; }

        /// <summary>
        /// PCL special function register.
        /// </summary>
        public static PICRegisterStorage PCL { get; private set; }

        /// <summary>
        /// STATUS register.
        /// </summary>
        public static PICRegisterStorage STATUS { get; private set; }

        /// <summary>
        /// Carry bit in STATUS register.
        /// </summary>
        public static PICBitFieldStorage C { get; private set; }

        /// <summary>
        /// Digit-Carry bit in STATUS register..
        /// </summary>
        public static PICBitFieldStorage DC { get; private set; }

        /// <summary>
        /// Zero bit in STATUS register..
        /// </summary>
        public static PICBitFieldStorage Z { get; private set; }

        /// <summary>
        /// Power-Down bit in STATUS or PCON register..
        /// </summary>
        public static PICBitFieldStorage PD { get; private set; }

        /// <summary>
        /// Timed-Out bit in STATUS or PCON register..
        /// </summary>
        public static PICBitFieldStorage TO { get; private set; }

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
        /// BSR special function register.
        /// </summary>
        public static PICRegisterStorage BSR { get; private set; }

        /// <summary>
        /// WREG special function register.
        /// </summary>
        public static PICRegisterStorage WREG { get; private set; }

        /// <summary>
        /// PCLH special function register.
        /// </summary>
        public static PICRegisterStorage PCLATH { get; private set; }

        /// <summary>
        /// INTCON special function register.
        /// </summary>
        public static PICRegisterStorage INTCON { get; private set; }

        /// <summary>
        /// FSR0 pseudo-register (alias to FSR0H:FSR0L).
        /// </summary>
        public static PICRegisterStorage FSR0 { get; private set; }

        /// <summary>
        /// FSR1 pseudo-register (alias to FSR1H:FSR1L).
        /// </summary>
        public static PICRegisterStorage FSR1 { get; private set; }

        protected override void SetCoreRegisters()
        {

            // *True* PIC registers

            INDF0 = GetRegisterStorageByName("INDF0");
            INDF1 = GetRegisterStorageByName("INDF1");
            PCL = GetRegisterStorageByName("PCL");

            STATUS = GetRegisterStorageByName("STATUS");
            C = GetBitFieldStorageByName("C");
            DC = GetBitFieldStorageByName("DC");
            Z = GetBitFieldStorageByName("Z");
            PD = PeekBitFieldStorageByName("PD");
            if (PD is null)
                PD = PeekBitFieldStorageByName("nPD");
            TO = PeekBitFieldStorageByName("TO");
            if (TO is null)
                TO = PeekBitFieldStorageByName("nTO");

            FSR0L = GetRegisterStorageByName("FSR0L");
            FSR0H = GetRegisterStorageByName("FSR0H");
            FSR1L = GetRegisterStorageByName("FSR1L");
            FSR1H = GetRegisterStorageByName("FSR1H");
            FSR0 = GetRegisterStorageByName("FSR0");
            FSR1 = GetRegisterStorageByName("FSR1");

            BSR = GetRegisterStorageByName("BSR");
            WREG = GetRegisterStorageByName("WREG");
            PCLATH = GetRegisterStorageByName("PCLATH");
            INTCON = GetRegisterStorageByName("INTCON");
        }

        #endregion

    }

}
