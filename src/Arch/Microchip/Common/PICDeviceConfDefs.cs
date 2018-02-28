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

using Reko.Core;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// A class to provide support for PIC Device Configuration definitions (per ConfigFuseSector content).
    /// </summary>
    public class PICDeviceConfDefs : IDeviceConfigDefs
    {

        #region Member fields

        private SortedList<Address, DevConfigRegister> dcregisters;

        #endregion

        #region Constructors

        private PICDeviceConfDefs(PIC thePIC)
        {
            PIC = thePIC;
            dcregisters = new SortedList<Address, DevConfigRegister>();
            foreach (var cfs in PIC.ProgramSpace.Sectors.OfType<ConfigFuseSector>())
            {
                foreach (var dcrdef in cfs.Defs.OfType<DCRDef>())
                {
                    var dcreg = new DevConfigRegister(dcrdef);
                    foreach (var dcrmode in dcrdef.DCRModes)
                    {
                        int ibit = 0;
                        foreach (var dcrfield in dcrmode.Fields)
                        {
                            switch (dcrfield)
                            {
                                case DataBitAdjustPoint adj:
                                    ibit += adj.Offset;
                                    break;
                                case DCRFieldDef dcrfieldddef:
                                    if (dcrfieldddef.IsHidden || dcrfieldddef.IsLangHidden)
                                        continue;
                                    var dcrfdef = new DevConfigField(dcrfieldddef, dcreg.Address, ibit);
                                    ibit += dcrfieldddef.NzWidth;
                                    dcreg.AddField(dcrfdef);
                                    foreach (var dcsem in dcrfieldddef.DCRFieldSemantics)
                                    {
                                        dcrfdef.AddSemantic(new DevConfigSemantic(dcsem));
                                    }
                                    break;
                            }
                        }
                    }
                    dcregisters.Add(dcreg.Address, dcreg);
                }
            }
        }

        public static PICDeviceConfDefs Create(PIC thePIC)
        {
            if (thePIC is null)
                throw new ArgumentNullException(nameof(thePIC));
            if (thePIC.ProgramSpace is null)
                throw new InvalidOperationException($"Can't create PIC Device Configuration definitions.");
            var dcrconf = new PICDeviceConfDefs(thePIC);
            return dcrconf;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the target PIC for this Device Configuration Definitions.
        /// </summary>
        /// <value>
        /// The target PIC.
        /// </value>
        public PIC PIC { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a Device Configuration Register by its name.
        /// </summary>
        /// <param name="name">The name of the register.</param>
        /// <returns>
        /// A <see cref="DevConfigRegister"/> instance or null.
        /// </returns>
        public DevConfigRegister GetDCR(string name)
            => dcregisters.FirstOrDefault(dcr => dcr.Value.Name == name).Value;

        /// <summary>
        /// Gets a Device Configuration Register by its memory address.
        /// </summary>
        /// <param name="addr">The memory address.</param>
        /// <returns>
        /// A <see cref="DevConfigRegister"/> instance or null.
        /// </returns>
        public DevConfigRegister GetDCR(Address addr)
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
        public string Render(DevConfigRegister dcr, int value)
        {
            value &= dcr.Impl;
            var sems = dcr.Fields.Select(f => f.GetSemantic((value >> f.BitPos) & f.BitMask));
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
        public string Render(Address addr, int value)
        {
            var dcr = GetDCR(addr);
            if (dcr != null)
                return Render(dcr, value);
            return String.Empty;
        }

        #endregion

    }

}
