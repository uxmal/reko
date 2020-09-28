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

    /// <summary>
    /// Interface to the PIC registers symbol table.
    /// </summary>
    public interface IPICRegisterSymTable
    {
        /// <summary>
        /// Adds a simple register. Returns null if no addition done.
        /// </summary>
        /// <param name="reg">The register.</param>
        /// <returns>
        /// A <seealso cref="RegisterStorage"/> or null if tentative of duplication.
        /// </returns>
        bool AddRegister(PICRegisterStorage reg);

        /// <summary>
        /// Adds a register's named bit field. Returns null if no addition done.
        /// </summary>
        /// <param name="field">The bit field.</param>
        /// <returns>
        /// A <seealso cref="PICRegisterBitFieldStorage"/> or null if tentative of duplication.
        /// </returns>
        bool AddRegisterBitField(PICRegisterBitFieldStorage field);

    }

}
