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

using System.Collections.Generic;

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// This interface permits to access the description of the PIC program memory space.
    /// </summary>
    public interface IProgramSpace
    {
        /// <summary>
        /// Enumerates the program memory regions with address range and attributes.
        /// </summary>
        IEnumerable<IPICMemoryRegion> MemoryRegions { get; }

        /// <summary>
        /// Enumerates the definition of the configuration fuses.
        /// </summary>
        IEnumerable<IDeviceFusesConfig> ConfigurationFuses { get; }

        /// <summary>
        /// Enumerates the device hard-coded infos (device config, device information).
        /// </summary>
        IEnumerable<IDeviceInfoRegister> DeviceHWInfos { get; }

    }

}
