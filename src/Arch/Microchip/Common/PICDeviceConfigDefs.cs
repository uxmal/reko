#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// A class to provide support for PIC Device Configuration definitions (per Configuration Fuses Sector content).
    /// </summary>
    public class PICDeviceConfigDefs : IPICDeviceConfigDefs
    {

        private readonly SortedList<Address, PICDevConfigRegister> dcregisters;

        private PICDeviceConfigDefs(IPICDescriptor thePIC)
        {
            dcregisters = new SortedList<Address, PICDevConfigRegister>();
            foreach (var fuse in thePIC.ConfigurationFuses)
            {
                var dcreg = new PICDevConfigRegister(fuse);

                foreach (var dcilg in fuse.IllegalSettings)
                {
                    dcreg.AddIllegal(new DevConfigIllegal(dcilg));
                }
                foreach (var dcfld in fuse.ConfigFields.Where(f => !(f.IsHidden || f.IsLangHidden)))
                {
                    var dcrfdef = new DevConfigField(dcfld, dcreg.Address);
                    foreach (var dcsem in dcfld.Semantics)
                    {
                        dcrfdef.AddSemantic(new DevConfigSemantic(dcsem));
                    }
                    dcreg.AddField(dcrfdef);
                }
                dcregisters.Add(dcreg.Address, dcreg);

            }
        }

        public static IPICDeviceConfigDefs Create(IPICDescriptor pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            var dcrconf = new PICDeviceConfigDefs(pic);
            if (dcrconf.dcregisters.Count <0)
                throw new InvalidOperationException($"Can't create PIC Device Configuration definitions.");
            return dcrconf;
        }


        /// <summary>
        /// Gets a Device Configuration Register by its name.
        /// </summary>
        /// <param name="name">The name of the register.</param>
        /// <returns>
        /// A <see cref="PICDevConfigRegister"/> instance or null.
        /// </returns>
        public PICDevConfigRegister? GetDCR(string name)
            => dcregisters.FirstOrDefault(dcr => dcr.Value.Name == name).Value;

        /// <summary>
        /// Gets a Device Configuration Register by its memory address.
        /// </summary>
        /// <param name="addr">The memory address.</param>
        /// <returns>
        /// A <see cref="PICDevConfigRegister"/> instance or null.
        /// </returns>
        public PICDevConfigRegister? GetDCR(Address addr)
            => dcregisters.ContainsKey(addr) ? dcregisters[addr] : null;

        public DevConfigField? GetDCRField(string name)
            => dcregisters.Values.SelectMany(dcr =>dcr.Fields).ToList().FirstOrDefault(f => f.Name == name);

        /// <summary>
        /// Renders the Device Configuration Register states given its content.
        /// </summary>
        /// <param name="dcr">The Device Configuration Register of interest.</param>
        /// <param name="value">The value assigned to this register.</param>
        /// <returns>
        /// A human-readable string describing the configuration bits/fuses.
        /// </returns>
        public string Render(PICDevConfigRegister dcr, uint value)
        {
            value &= (uint)dcr.Impl;
            var ilg = dcr.Illegals.FirstOrDefault(p => p.Match(value));
            if (ilg is not null)
                return $"** Fuse=0x{value:X}: {ilg.Descr}**";
            var sems = dcr.Fields.Select(f => f.GetSemantic((uint)((value >> f.BitPos) & f.BitMask)));
            var flds = dcr.Fields.Zip(sems, (fl, se) => fl.Name + "=" + se.State);
            return String.Join(", ", flds);
        }

        /// <summary>
        /// Renders the Device Configuration state at given address for given value.
        /// </summary>
        /// <param name="addr">The Device Configuration Register memory address.</param>
        /// <param name="value">The value assigned to this register.</param>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        public string Render(Address addr, uint value)
        {
            var dcr = GetDCR(addr);
            if (dcr is not null)
                return Render(dcr, value);
            return String.Empty;
        }

    }

}
