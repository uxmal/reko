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

using Reko.Core;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// A class to provide support for PIC Device Configuration definitions (per ConfigFuseSector content).
    /// </summary>
    public class PICDeviceConfigDefs : IPICDeviceConfigDefs
    {


        private SortedList<Address, PICDevConfigRegister> dcregisters;


        private PICDeviceConfigDefs(PIC thePIC)
        {
            PIC = thePIC;
            dcregisters = new SortedList<Address, PICDevConfigRegister>();
            foreach (var cfs in PIC.ProgramSpace.Sectors.OfType<ConfigFuseSector>())
            {
                foreach (var dcrdef in cfs.Defs.OfType<DCRDef>())
                {
                    var dcreg = new PICDevConfigRegister(dcrdef);
                    foreach (var ilg in dcrdef.Illegals)
                    {
                        dcreg.AddIllegal(new DevConfigIllegal(ilg));
                    }
                    foreach (var dcrmode in dcrdef.DCRModes)
                    {
                        int ibit = 0;
                        foreach (var dcrfield in dcrmode.Fields)
                        {
                            switch (dcrfield)
                            {
                                case ProgBitAdjustPoint adj:
                                    ibit += adj.Offset;
                                    break;
                                case DCRFieldDef dcrfieldddef:
                                    dcrfieldddef.BitAddr = ibit; // Force the bit address as 'genPICdb' does not set it.
                                    ibit += dcrfieldddef.NzWidth;
                                    if (dcrfieldddef.IsHidden || dcrfieldddef.IsLangHidden)
                                        continue;
                                    var dcrfdef = new DevConfigField(dcrfieldddef, dcreg.Address);
                                    dcrfieldddef.DCRFieldSemantics.ForEach((dcsem) => dcrfdef.AddSemantic(new DevConfigSemantic(dcsem)));
                                    dcreg.AddField(dcrfdef);
                                    break;
                            }
                        }
                    }
                    dcregisters.Add(dcreg.Address, dcreg);
                }
            }
        }

        public static IPICDeviceConfigDefs Create(PIC thePIC)
        {
            if (thePIC is null)
                throw new ArgumentNullException(nameof(thePIC));
            if (thePIC.ProgramSpace is null)
                throw new InvalidOperationException($"Can't create PIC Device Configuration definitions.");
            var dcrconf = new PICDeviceConfigDefs(thePIC);
            return dcrconf;
        }


        /// <summary>
        /// Gets the target PIC for this Device Configuration Definitions.
        /// </summary>
        /// <value>
        /// The target PIC.
        /// </value>
        public PIC PIC { get; }


        /// <summary>
        /// Gets a Device Configuration Register by its name.
        /// </summary>
        /// <param name="name">The name of the register.</param>
        /// <returns>
        /// A <see cref="PICDevConfigRegister"/> instance or null.
        /// </returns>
        public PICDevConfigRegister GetDCR(string name)
            => dcregisters.FirstOrDefault(dcr => dcr.Value.Name == name).Value;

        /// <summary>
        /// Gets a Device Configuration Register by its memory address.
        /// </summary>
        /// <param name="addr">The memory address.</param>
        /// <returns>
        /// A <see cref="PICDevConfigRegister"/> instance or null.
        /// </returns>
        public PICDevConfigRegister GetDCR(Address addr)
            => dcregisters.ContainsKey(addr) ? dcregisters[addr] : null;

        public DevConfigField GetDCRField(string name)
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
            if (ilg != null)
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
            if (dcr != null)
                return Render(dcr, value);
            return String.Empty;
        }

    }

}
