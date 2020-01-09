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

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// A class defining some constants related to Microchip PIC database.
    /// </summary>
    public static class PICConstants
    {
        /// <summary>
        /// The filename of the local database file (ZIP archive).
        /// </summary>
        public const string LocalDBFilename = "picdb.zip";

        /// <summary>
        /// The filename of the parts information file inside the PIC database.
        /// </summary>
        public const string PartsInfoFilename = "partsinfo.xml";

        /// <summary>
        /// Relative pathname for PIC16 definition files in the PIC database.
        /// </summary>
        public const string ContentPIC16Path = EDCContent + "16xxxx";
        /// <summary>
        /// Relative pathname for PIC16 definition files in the PIC database.
        /// </summary>
        public const string ContentPIC18Path = EDCContent + "18xxxx";

        private const string EDCContent = @"content/edc/";

    }

}
