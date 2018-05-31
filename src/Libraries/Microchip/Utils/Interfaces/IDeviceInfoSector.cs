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
    /// This interface provides access to a device information sector (either DIA or DCI sector)
    /// </summary>
    public interface IDeviceInfoSector : IPICMemoryRegion
    {
        /// <summary> Enumerates the device information registers. </summary>
        IEnumerable<IDeviceInfoRegister> Registers { get; }
    }

    /// <summary>
    /// This interface provides access to a device information register.
    /// </summary>
    public interface IDeviceInfoRegister
    {
        /// <summary> Gets the address of the device information register. </summary>
        int Addr { get; }

        /// <summary> Gets the name of the device information register. </summary>
        string Name { get; }

        /// <summary> Gets the bit width of the device information register. </summary>
        int BitWidth { get; }
    }

}
