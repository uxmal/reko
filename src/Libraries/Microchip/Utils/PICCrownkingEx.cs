#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
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

using System.Collections.Generic;

namespace Reko.Libraries.Microchip
{
    using V1;

    /// <summary>
    /// Various extensions methods to manipulate PIC definitions.
    /// </summary>
    public static partial class PICCrownkingEx
    {

        /// <summary>
        /// A PICCrownking extension method that gets a PIC descriptor.
        /// </summary>
        /// <param name="db">The PIC database to retrieve definition from.</param>
        /// <param name="sPICName">Name of the PIC.</param>
        /// <returns>
        /// The PIC descriptor or null.
        /// </returns>
        public static IPICDescriptor GetPIC(this PICCrownking db, string sPICName)
            => db.GetPICAsXML(sPICName)?.ToObject<PIC_v1>().PICDescriptorInterface;

        /// <summary>
        /// A PICCrownking extension method that gets a PIC descriptor.
        /// </summary>
        /// <param name="db">The PIC database to retrieve definition from.</param>
        /// <param name="iProcID">Identifier for the processor.</param>
        /// <returns>
        /// The PIC descriptor or null.
        /// </returns>
        public static IPICDescriptor GetPIC(this PICCrownking db, int iProcID)
            => db.GetPICAsXML(iProcID)?.ToObject<PIC_v1>().PICDescriptorInterface;

    }

}
