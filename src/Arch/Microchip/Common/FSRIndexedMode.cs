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

namespace Reko.Arch.MicrochipPIC.Common
{
    public enum FSRIndexedMode : byte
    {
        /// <summary> No indirect/indexed read/write mode. </summary>
        None,
        /// <summary> Indexed read/write FSRx register + offset. </summary>
        INDEXED,
        /// <summary> Indexed read/write FSR2 register with offset. MOVFS, MOVSS </summary>
        FSR2INDEXED,
        /// <summary> Indirect read/write using FSRx register (thru INDFx register). </summary>
        INDF,
        /// <summary> Indirect read/write using FSRx register + WREG offset (thru PLUSWx register). </summary>
        PLUSW,
        /// <summary> Indirect read/write using post-decremented FSRx register (POSTDECx register or FSRn--). </summary>
        POSTDEC,
        /// <summary> Indirect read/write using post-incremented FSRx register (POSTINCx register or FSRn++). </summary>
        POSTINC,
        /// <summary> Indirect read/write using pre-decremented FSRx register (PREINCx register or --FSRn). </summary>
        PREDEC,
        /// <summary> Indirect read/write using pre-incremented FSRx register (PREINCx register or ++FSRn). </summary>
        PREINC

    }
}
