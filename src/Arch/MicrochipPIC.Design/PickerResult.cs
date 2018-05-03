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

namespace Reko.Arch.MicrochipPIC.Design
{
    public class PickerResult
    {
        /// <summary>
        /// Name of the PIC.
        /// </summary>
        public string PICName;

        /// <summary>
        /// True to permit decoding of Extended Execution mode of PIC18, false otherwise.
        /// This is a hint in case the configuration fuses are not part of the binary image.
        /// </summary>
        public bool AllowExtended;

        public override string ToString() => $"{PICName}{(AllowExtended ? " - extended" : "")}";

    }

}
